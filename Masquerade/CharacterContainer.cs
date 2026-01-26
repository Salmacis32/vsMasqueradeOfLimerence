using Masquerade.Character;
using Masquerade.Models;

namespace Masquerade
{
    public class CharacterContainer : IInstanced
    {
        public CharacterContainer() 
        {
            _equipmentGlobalIds = new HashSet<int>();
            _equipmentModIds = new HashSet<int>();
        }

        public int InstanceId { get; internal set; }

        public string Name { get; internal set; }

        public float Exp {  get; internal set; }

        public int Level { get; internal set; }

        public CharacterStats Stats { get; internal set; }

        public float CurrentHP { get; internal set; }

        private Lazy<HashSet<ModEquipment>> _modEquipment;
        private Lazy<HashSet<ModCharacterEffects>> _modEffects;

        private HashSet<int> _equipmentGlobalIds;
        private HashSet<int> _equipmentModIds;

        internal void AddEquipmentModId(int instanceId)
        {
            if (InstanceId < 0 || _equipmentModIds.Any(x => x == instanceId))
                return;

            _equipmentModIds.Add(instanceId);
        }

        internal void RemoveEquipmentModId(int instanceId)
        {
            if (InstanceId < 0 || !_equipmentModIds.Any(x => x == instanceId))
                return;

            _equipmentModIds.Remove(instanceId);
        }
    }
}
