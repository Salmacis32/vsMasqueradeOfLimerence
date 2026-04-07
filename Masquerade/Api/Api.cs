using Il2CppVampireSurvivors.App.Tools;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Weapons;
using Masquerade.Character;
using Masquerade.Equipment;
using Masquerade.Models;
using Masquerade.Util;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        internal IEnumerable<MasqMod> LoadedMods;
        private HashSet<CharacterContainer> _characters;
        private int[] _nextIds = new int[3];

        internal MasqueradeApi()
        {
            AccessoryFactory = new EquipmentFactory<ModAccessory>();
            CharacterEffectFactory = new ContentFactory<ModCharacterEffect>();
            _characters = new HashSet<CharacterContainer>();
            LoadedMods = [];
            _nextIds = new int[3] { 0 /* GlobalId */, 0 /* ContentId*/, Common.WEAPONTYPE_START_ID };
        }

        internal EquipmentFactory<ModAccessory> AccessoryFactory { get; set; }
        internal ContentFactory<ModCharacterEffect> CharacterEffectFactory { get; set; }

        internal bool ModdedEquipmentCheck(WeaponType item)
        {
            int weaponTypeId = (int)item;
            if (!Masquerade.Api.IsModdedEquipment(weaponTypeId))
                return false;

            return Masquerade.Api.AccessoryFactory.DoesEquipmentExist(weaponTypeId);
        }

        internal void CleanUpInstances()
        {
            _characters.Clear();
            _nextIds[Common.GLOBAL_INSTANCE_ID_INDEX] = 0;
            Masquerade.Instance.ModEffectSystem.CleanUpInstances();
        }

        // Add this to: RemoveAllEquipmentFromPlayers
        internal void DeleteEquipmentInstance(int instanceId, CharacterContainer container)
        {
            var instance = _characters.SingleOrDefault(x => x.InstanceId == instanceId);
            if (instanceId < 0 || !container.HasEquipment(instanceId))
                return;

            container.RemoveModEquipment(instance.TypeId);
        }

        internal IEnumerable<CharacterContainer> GetCharacterInstances() => _characters;

        internal int GetNextFreeContentId()
        {
            if (GetContentIds()?.Any() ?? false)
                _nextIds[Common.CONTENT_ID_INDEX] = GetContentIds().Max() + 1;
            return _nextIds[Common.CONTENT_ID_INDEX];
        }

        internal int GetNextFreeWeaponTypeId(IEnumerable<ModAccessory> cachedList = null)
        {
            var cache = cachedList;
            if (cache == null) cache = AccessoryFactory.GetAllContent();

            if (cache.Any(x => x.WeaponTypeId >= _nextIds[Common.WEAPON_ID_INDEX]))
            {
                _nextIds[Common.WEAPON_ID_INDEX] = cache.Max(x => x.WeaponTypeId) + 1;
                return GetNextFreeWeaponTypeId(cache);
            }
            return _nextIds[Common.WEAPON_ID_INDEX];
        }

        internal CharacterContainer GetOrAddCharacterInstance(CharacterController controller, bool resync = true)
        {
            var component = controller.gameObject.GetOrAddComponent<GlobalInstanceComponent>();
            var instanceId = component.GlobalInstanceId;
            if (instanceId >= 0 && _characters.Any(x => x.InstanceId == instanceId))
            {
                var character = _characters.SingleOrDefault(x => x.InstanceId == instanceId);
                if (character == null)
                {
                    LoggerHelper.Logger.Error($"Too many characters assigned to instance {instanceId}!");
                    return null;
                }

                if (resync)
                    SyncCharacterContainer(ref character, controller);

                return character;
            }

            var inst = new CharacterContainer();
            SyncCharacterContainer(ref inst, controller);
            if (inst == null)
            {
                LoggerHelper.Logger.Error($"Unable to create instance of character {controller.name}");
                return null;
            }
            _characters.Add(inst);
            instanceId = GetNextFreeGlobalInstanceId();
            inst.InstanceId = instanceId;
            component.GlobalInstanceId = instanceId;
            LoggerHelper.Logger.Msg($"Added global instance id {component.GlobalInstanceId} to character {controller.name}.");
            return inst;
        }

        internal ModAccessory GetOrAddModAccessoryInstance(int weaponTypeId, CharacterContainer container, CharacterController controller, bool resync = true)
        {
            if (!IsModdedEquipment(weaponTypeId))
            {
                LoggerHelper.Logger.Error($"Content {weaponTypeId} is not of type ModAccessory");
                return null;
            }
            if (weaponTypeId >= Common.WEAPONTYPE_START_ID && container.HasEquipment(weaponTypeId) && container.HasModEquipment(weaponTypeId))
            {
                if (resync)
                    SyncCharacterContainer(ref container, controller);
                return container.GetModEquipment(weaponTypeId) as ModAccessory;
            }

            var proto = GetModAccessoryByType(weaponTypeId);
            var inst = Activator.CreateInstance(proto.GetType()) as ModAccessory;
            if (inst == null)
            {
                LoggerHelper.Logger.Error($"Unable to create instance of accessory {weaponTypeId}");
                return null;
            }
            inst.Mod = proto.Mod;
            inst.ContentId = proto.ContentId;
            inst.WeaponTypeId = proto.WeaponTypeId;
            inst.Owner = container;
            SyncCharacterContainer(ref container, controller);
            inst.Container = container.GetEquipment(inst.WeaponTypeId);
            LoggerHelper.Logger.Msg($"Created ModAccessory {inst.DisplayName} instance {inst.ContentId} of typeId {inst.WeaponTypeId} on character {container.Name} id {container.InstanceId}");
            return inst;
        }

        private EquipmentContainer CreateEquipmentContainer(Il2CppVampireSurvivors.Objects.Equipment equipment)
        {
            var container = new EquipmentContainer();
            container.Name = equipment.Type.ToString();
            container.TypeId = (int)equipment.Type;
            container.Level = equipment.Level;

            var weapon = equipment.TryCast<Weapon>();
            var accessory = equipment.TryCast<Accessory>();
            var es = new EquipmentStats();
            var cs = new CharacterStats();
            if (accessory != null)
            {
                var data = SafeAccess.GetProperty<WeaponData>(accessory, nameof(accessory.CurrentAccessoryData));
                if (data == null)
                {
                    LoggerHelper.Logger.Error($"Attempted to load CurrentAccessoryData for equipment {container.TypeId} and failed!");
                    return container;
                }
                es = SetEquipStats(data);
                cs = SetModifierStats(data);
            }
            else if (weapon != null)
            {
                var data = SafeAccess.GetProperty<WeaponData>(weapon, nameof(weapon.CurrentWeaponData));
                if (data == null)
                {
                    LoggerHelper.Logger.Error($"Attempted to load CurrentWeaponData for equipment {container.TypeId} and failed!");
                    return container;
                }
                es = SetEquipStats(data);
                cs = SetModifierStats(data);
            }

            container.EquipStats = es;
            container.ModifierStats = cs;
            return container;
        }

        private IEnumerable<int> GetContentIds()
        {
            var ids = AccessoryFactory.GetContentIds();
            ids = ids.Concat(CharacterEffectFactory.GetContentIds());
            return ids;
        }

        private int GetNextFreeGlobalInstanceId()
        {
            if (_characters.Any(x => x.InstanceId == _nextIds[Common.GLOBAL_INSTANCE_ID_INDEX]))
            {
                _nextIds[Common.GLOBAL_INSTANCE_ID_INDEX]++;
                return GetNextFreeGlobalInstanceId();
            }
            return _nextIds[Common.GLOBAL_INSTANCE_ID_INDEX];
        }

        private EquipmentStats SetEquipStats(WeaponData data)
        {
            var kb = SafeAccess.GetProperty<Il2CppSystem.Nullable<float>>(data, nameof(data.knockback));
            var pl = SafeAccess.GetProperty<Il2CppSystem.Nullable<int>>(data, nameof(data.poolLimit));
            var dr = SafeAccess.GetProperty<Il2CppSystem.Nullable<int>>(data, nameof(data.duration));

            var es = new EquipmentStats(data.power, data.area, data.speed, data.amount, data.critChance, data.critMul, data.repeatInterval, data.interval, data.chance, data.penetrating,
                (dr != null && dr.HasValue) ? dr.Value : null, (pl != null && pl.HasValue) ? pl.Value : null, (kb != null && kb.HasValue) ? kb.Value : null, data.hitsWalls);
            return es;
        }

        private CharacterStats SetModifierStats(WeaponData data) => new CharacterStats(data.cooldown, data.maxHp, data.speed, data.growth, data.magnet, data.luck,
            data.armor, data.regen, data.greed, data.curse);

        private void SyncCharacterContainer(ref CharacterContainer container, CharacterController controller)
        {
            container.TypeId = (int)controller.CharacterType;
            container.CurrentHP = controller.CurrentHealth();
            container.Exp = controller.Xp;
            container.Name = controller.CharacterType.ToString();
            container.Level = controller.Level;
            container.Stats = SyncCharacterStats(controller);

            foreach (var equip in controller.AccessoriesManager.ActiveEquipment)
            {
                if (equip != null && !container.HasEquipment((int)equip.Type))
                    container.AddEquipmentContainer(CreateEquipmentContainer(equip));
            }
        }

        private CharacterStats SyncCharacterStats(CharacterController controller)
        {
            var stats = new CharacterStats(controller.PCooldown(), controller.MaxHp(), controller.PMoveSpeed(),
                controller.PGrowth(), controller.Magnet.scale, controller.PLuck(), controller.PArmor(),
                controller.PRegen(), controller.PGreed(), controller.PCurse());

            // Add Player specific stats (rerolls, skips, banishes)

            return stats;
        }
    }
}