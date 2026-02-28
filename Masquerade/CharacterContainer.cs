using Masquerade.Character;
using Masquerade.Models;

namespace Masquerade
{
    public class CharacterContainer : IInstanced, IContainer
    {
        private HashSet<EquipmentContainer> _equipment;

        private HashSet<ModCharacterEffects> _modEffects;

        private HashSet<ModEquipment> _modEquipment;

        public CharacterContainer()
        {
            _modEquipment = new HashSet<ModEquipment>();
            _modEffects = new HashSet<ModCharacterEffects>();
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

        public EquipmentContainer GetEquipment(int contentId)
        {
            if (!HasEquipment(contentId))
            {
                Masquerade.Logger.Error($"{Name} {InstanceId} tried to load equipment container with id {contentId} and failed!");
                return null;
            }

            return _equipment.SingleOrDefault(x => x.TypeId == contentId);
        }

        public ModEquipment GetModEquipment(int contentId)
        {
            if (!HasModEquipment(contentId))
            {
                Masquerade.Logger.Error($"{Name} {InstanceId} tried to load mod equipment with id {contentId} and failed!");
                return null;
            }

            return _modEquipment.SingleOrDefault(x => x.ContentId == contentId);
        }

        public ModEquipment GetModEquipment<T>() where T : ModEquipment
        {
            if (!HasModEquipment<T>())
            {
                Masquerade.Logger.Error($"{Name} {InstanceId} tried to load mod equipment {typeof(T).Name} and failed!");
                return null;
            }

            return _modEquipment.SingleOrDefault(x => x.GetType() == typeof(T));
        }

        public bool HasAccessory<T>() where T : ModAccessory => HasEquipment<T>();

        public bool HasEquipment(int contentId) => _equipment.Any(x => x.TypeId == contentId);

        public bool HasModEquipment<T>() where T : ModEquipment => HasEquipment<T>() && _modEquipment.Any(x => x.GetType().IsAssignableTo(typeof(T)));

        public bool HasModEquipment(int contentId) => HasEquipment(contentId) && _modEquipment.Any(x => x.ContentId == contentId);

        public bool HasEquipmentInstance(int instanceId) => _equipment.Any(x => x.InstanceId == instanceId);

        public bool HasEquipment<T>() where T : ModEquipment => _modEquipment.Any(x => x.GetType() == typeof(T));

        internal void AddEquipmentContainer(EquipmentContainer container)
        {
            if (container == null)
                return;
            else if (HasEquipment(container.TypeId))
            {
                Masquerade.Logger.Warning($"Cannot add {container.Name} to character {Name} {InstanceId} as it already has equipment with content id {container.TypeId}");
                return;
            }

            _equipment.Add(container);
        }

        internal void AddModEquipment(ModEquipment equip)
        {
            if (HasModEquipment(equip.ContentId))
                return;

            _modEquipment.Add(equip);
        }

        internal void RemoveEquipmentContainer(int contentId)
        {
            if (!HasEquipment(contentId))
                return;

            _equipment = _equipment.Where(x => x.TypeId != contentId).ToHashSet();
        }

        internal void RemoveModEquipment(int contentId)
        {
            if (_modEquipment.Any(x => x.ContentId == contentId))
                _modEquipment.Remove(GetModEquipment(contentId));
            if (_equipment.Any(x => x.TypeId == contentId))
                _equipment.Remove(GetEquipment(contentId));
        }

        public void UpdateContainer()
        {
            throw new NotImplementedException();
        }
    }
}