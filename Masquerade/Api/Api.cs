using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2CppVampireSurvivors.Objects;
using Il2CppVampireSurvivors.Objects.Characters;
using Il2CppVampireSurvivors.Objects.Weapons;
using Masquerade.Character;
using Masquerade.Equipment;
using Masquerade.Models;
using Masquerade.Util;
using System.Data;

namespace Masquerade.Api
{
    public partial class MasqueradeApi
    {
        private HashSet<CharacterContainer> _characters;
        private int _nextFreeGlobalInstanceId = 0;
        internal IEnumerable<MasqMod> LoadedMods;

        internal ContentFactory<ModAccessory> AccessoryFactory { get; set; }

        internal MasqueradeApi() { AccessoryFactory = new ContentFactory<ModAccessory>(); _characters = new HashSet<CharacterContainer>(); LoadedMods = []; }

        internal ModAccessory GetOrAddModAccessoryInstance(int contentId, CharacterContainer container, CharacterController controller, bool resync = true)
        {
            if (!AccessoryFactory.DoesContentExist(contentId))
            {
                Masquerade.Logger.Error($"Content {contentId} is not of type ModAccessory");
                return null;
            }
            if (contentId >= 0 && container.HasModEquipment(contentId))
            {
                if (resync)
                    SyncCharacterContainer(ref container, controller);
                return container.GetModEquipment(contentId) as ModAccessory;
            }

            var proto = Masquerade.Api.GetModAccessory(contentId);
            var inst = Activator.CreateInstance(proto.GetType()) as ModAccessory;
            if (inst == null)
            {
                Masquerade.Logger.Error($"Unable to create instance of accessory {contentId}");
                return null;
            }
            inst.Mod = proto.Mod;
            inst.ContentId = proto.ContentId;
            inst.Owner = container;
            SyncCharacterContainer(ref container, controller);
            inst.Container = container.GetEquipment(inst.ContentId);
            return inst;
        }

        internal IEnumerable<CharacterContainer> GetCharacterInstances() => _characters;

        internal CharacterContainer GetOrAddCharacterInstance(CharacterController controller, GlobalInstanceComponent component, bool resync = true)
        {
            var instanceId = component.GlobalInstanceId;
            if (instanceId >= 0 && _characters.Any(x => x.InstanceId == instanceId))
            {
                var character = _characters.SingleOrDefault(x => x.InstanceId == instanceId);
                if (character == null)
                {
                    Masquerade.Logger.Error($"Too many characters assigned to instance {instanceId}!");
                    return null;
                }

                if(resync)
                    SyncCharacterContainer(ref character, controller);

                return character;
            }

            var inst = new CharacterContainer();
            SyncCharacterContainer(ref inst, controller);
            if (inst == null)
            {
                Masquerade.Logger.Error($"Unable to create instance of character {controller.name}");
                return null;
            }
            _characters.Add(inst);
            instanceId = GetNextFreeGlobalInstanceId();
            inst.InstanceId = instanceId;
            component.GlobalInstanceId = instanceId;
            return inst;
        }

        // Add this to: RemoveAllEquipmentFromPlayers
        internal void DeleteEquipmentInstance(int contentId, CharacterContainer container)
        {
            var instance = _characters.SingleOrDefault(x => x.InstanceId == contentId);
            if (contentId < 0|| !container.HasEquipment(contentId))
                return;

            container.RemoveModEquipment(contentId);
        }

        internal void CleanUpInstances()
        {
            _characters.Clear();
            _nextFreeGlobalInstanceId = 0;
        }

        internal static bool ModdedCheck(WeaponType item)
        {
            int contentId = (int)item;
            if (!Masquerade.Api.IsModdedContent(contentId) || !Masquerade.Api.IsContentAccessory(contentId))
                return false;

            return true;
        }

        private int GetNextFreeGlobalInstanceId()
        {
            if (_characters.Any(x => x.InstanceId == _nextFreeGlobalInstanceId || x.HasEquipmentInstance(_nextFreeGlobalInstanceId)))
            {
                _nextFreeGlobalInstanceId++;
                return GetNextFreeGlobalInstanceId();
            }
            return _nextFreeGlobalInstanceId;
        }

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

        private EquipmentContainer CreateEquipmentContainer(Il2CppVampireSurvivors.Objects.Equipment equipment)
        {
            var container = new EquipmentContainer();
            container.Name = equipment.Type.ToString();
            container.TypeId = (int)equipment.Type;
            container.Level = equipment.Level;
            container.InstanceId = GetNextFreeGlobalInstanceId();

            var weapon = equipment.TryCast<Weapon>();
            var accessory = equipment.TryCast<Accessory>();
            var es = new EquipmentStats();
            var cs = new CharacterStats();
            if (accessory != null)
            {
                var data = SafeAccess.GetProperty<WeaponData>(accessory, nameof(accessory.CurrentAccessoryData));
                if (data == null)
                {
                    Masquerade.Logger.Error($"Attempted to load CurrentAccessoryData for equipment {container.TypeId} instance {container.InstanceId} and failed!");
                    return container;
                }
                es = SetEquipStats(data);
                cs = SetModifierStats(data);
            }
            else if (weapon != null) {
                var data = SafeAccess.GetProperty<WeaponData>(weapon, nameof(weapon.CurrentWeaponData));
                if (data == null)
                {
                    Masquerade.Logger.Error($"Attempted to load CurrentWeaponData for equipment {container.TypeId} instance {container.InstanceId} and failed!");
                    return container;
                }
                es = SetEquipStats(data);
                cs = SetModifierStats(data);
            }

            container.EquipStats = es;
            container.ModifierStats = cs;
            return container;
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
    }
}
