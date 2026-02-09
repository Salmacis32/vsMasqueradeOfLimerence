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
            container.AddModEquipment(inst);
            SyncCharacterContainer(ref container, controller);
            return inst;
        }

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
            container.CharacterType = (int)controller.CharacterType;
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
            var stats = new CharacterStats();

            stats.Armor = controller.PArmor();
            stats.Cooldown = controller.PCooldown();
            stats.Curse = controller.PCurse();
            stats.Greed = controller.PGreed();
            stats.Growth = controller.PGrowth();
            stats.HpRegen = controller.PRegen();
            stats.Luck = controller.PLuck();
            stats.MaxHp = controller.MaxHp();
            stats.MoveSpeed = controller.PMoveSpeed();
            
            //stats.Magnet = controller.Magnet.
            // Add Player specific stats (rerolls, skips, banishes)

            return stats;
        }

        private EquipmentContainer CreateEquipmentContainer(Il2CppVampireSurvivors.Objects.Equipment equipment)
        {
            var container = new EquipmentContainer();
            container.Name = equipment.Type.ToString();
            container.EquipmentType = (int)equipment.Type;
            container.Level = equipment.Level;
            container.InstanceId = GetNextFreeGlobalInstanceId();

            var weapon = equipment.TryCast<Weapon>();
            var accessory = equipment.TryCast<Accessory>();
            var es = new EquipmentStats();
            var cs = new CharacterStats();
            if (accessory != null)
            {
                var data = SafeAccess.GetProperty<WeaponData>(accessory, "CurrentAccessoryData");
                if (data == null)
                {
                    Masquerade.Logger.Error($"Attempted to load CurrentAccessoryData for equipment {container.EquipmentType} instance {container.InstanceId} and failed!");
                    return container;
                }
                es = SetEquipStats(data);
                cs = SetModifierStats(data);
            }
            else if (weapon != null) {
                var data = SafeAccess.GetProperty<WeaponData>(weapon, "CurrentWeaponData");
                if (data == null)
                {
                    Masquerade.Logger.Error($"Attempted to load CurrentWeaponData for equipment {container.EquipmentType} instance {container.InstanceId} and failed!");
                    return container;
                }
                es = SetEquipStats(data);
            }

            container.EquipStats = es;
            container.ModifierStats = cs;
            return container;   
        }

        private EquipmentStats SetEquipStats(WeaponData data)
        {
            var es = new EquipmentStats();
            es.Area = data.area;
            es.Interval = data.interval;
            es.Amount = data.amount;
            es.Power = data.power;
            es.CritChance = data.critChance;
            es.CritMultiplier = data.critMul;
            es.CanHitWalls = data.hitsWalls;
            es.Chance = data.chance;
            es.PierceAmount = data.penetrating;
            es.ProjectileSpeed = data.speed;

            var kb = SafeAccess.GetProperty<Il2CppSystem.Nullable<float>>(data, "knockback");
            es.Knockback = (kb != null) ? kb.Value : 0f;
            var pl = SafeAccess.GetProperty<Il2CppSystem.Nullable<int>>(data, "poolLimit");
            es.ProjectileLimit = (pl != null) ? pl.Value : 0;
            return es;
        }

        private CharacterStats SetModifierStats(WeaponData data)
        {
            var cs = new CharacterStats();
            cs.Growth = data.growth;
            cs.Armor = data.armor;
            cs.Skips = data.skips;
            cs.MaxHp = data.maxHp;
            cs.HpRegen = data.regen;
            cs.Greed = data.greed;
            cs.Cooldown = data.cooldown;
            cs.Luck = data.luck;
            cs.Curse = data.curse;
            cs.Magnet = data.magnet;
            cs.Revivals = data.revivals;
            cs.Rerolls = data.rerolls;
            cs.Skips = data.skips;
            cs.MoveSpeed = data.speed;
            return cs;
        }
    }
}
