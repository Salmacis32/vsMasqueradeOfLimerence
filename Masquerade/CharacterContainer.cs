using Masquerade.Character;
using Masquerade.Models;
using Masquerade.Util;

namespace Masquerade
{
    public class CharacterContainer : IInstanced, IContainer
    {
        private HashSet<EquipmentContainer> _equipment;

        private HashSet<ModCharacterEffect> _modEffects;

        private HashSet<ModEquipment> _modEquipment;

        public CharacterContainer()
        {
            _modEquipment = new HashSet<ModEquipment>();
            _modEffects = new HashSet<ModCharacterEffect>();
            _equipment = new HashSet<EquipmentContainer>();
        }

        public int TypeId { get; internal set; }
        public float CurrentHP { get; internal set; }
        public float Exp { get; internal set; }
        public int InstanceId { get; internal set; } = -1;
        public int Level { get; internal set; }
        public string Name { get; internal set; }
        public CharacterStats Stats { get; internal set; }

        public bool UpdateContainerNextTick => throw new NotImplementedException();

        public EquipmentContainer GetEquipment(int weaponTypeId)
        {
            if (!HasEquipment(weaponTypeId))
            {
                Masquerade.Instance.LoggerInstance.Error($"{Name} {InstanceId} tried to load equipment container with WeaponType {weaponTypeId} and failed!");
                return null;
            }

            return _equipment.SingleOrDefault(x => x.TypeId == weaponTypeId);
        }

        public ModEquipment GetModEquipment(int weaponTypeId)
        {
            if (!HasModEquipment(weaponTypeId))
            {
                Masquerade.Instance.LoggerInstance.Error($"{Name} {InstanceId} tried to load mod equipment with WeaponType {weaponTypeId} and failed!");
                return null;
            }

            return _modEquipment.SingleOrDefault(x => x.WeaponTypeId == weaponTypeId);
        }

        public ModEquipment GetModEquipment<T>() where T : ModEquipment
        {
            if (!HasModEquipment<T>())
            {
                Masquerade.Instance.LoggerInstance.Error($"{Name} {InstanceId} tried to load mod equipment {typeof(T).Name} and failed!");
                return null;
            }

            return _modEquipment.SingleOrDefault(x => x.GetType() == typeof(T));
        }

        public bool HasModAccessory<T>() where T : ModAccessory => HasModEquipment<T>();

        public bool HasEquipment(int weaponTypeId) => _equipment.Any(x => x.TypeId == weaponTypeId);

        public bool HasModEquipment<T>() where T : ModEquipment => _modEquipment.Any(x => x.GetType() == typeof(T)) && _modEquipment.Any(x => x.GetType().IsAssignableTo(typeof(T)));

        public bool HasModEquipment(int weaponTypeId) => _modEquipment.Any(x => x.WeaponTypeId == weaponTypeId);

        public bool HasEffect<T>() where T : ModCharacterEffect => _modEffects.Any(x => x.GetType() == typeof(T));

        public bool HasEffect(Type type) => _modEffects.Any(x => x.GetType() == type);

        public void AddModEffect(ModCharacterEffect effect, bool canStack)
        {
            if (!canStack && HasEffect(effect.GetType()))
            {
                LoggerHelper.Logger.Warning($"Character already has an instance of {effect.FullName}.");
                return;
            }

            _modEffects.Add(effect);
        }

        internal void AddEquipmentContainer(EquipmentContainer container)
        {
            if (container == null)
                return;
            else if (HasEquipment(container.TypeId))
            {
                LoggerHelper.Logger.Warning($"Cannot add {container.Name} to character {Name} {InstanceId} as it already has equipment with content id {container.TypeId}");
                return;
            }

            _equipment.Add(container);
        }

        internal void AddModEquipment(ModEquipment equip)
        {
            if (HasModEquipment(equip.WeaponTypeId) || !HasEquipment(equip.WeaponTypeId))
                return;

            _modEquipment.Add(equip);
        }

        internal void RemoveEquipmentContainer(int contentId)
        {
            if (!HasEquipment(contentId))
                return;

            _equipment = _equipment.Where(x => x.TypeId != contentId).ToHashSet();
        }

        internal void RemoveModEquipment(int typeId)
        {
            if (_modEquipment.Any(x => x.WeaponTypeId == typeId))
                _modEquipment.Remove(GetModEquipment(typeId));
            if (_equipment.Any(x => x.TypeId == typeId))
                _equipment.Remove(GetEquipment(typeId));
        }

        public void UpdateContainer()
        {
            throw new NotImplementedException();
        }
    }
}