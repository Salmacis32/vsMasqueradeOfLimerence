using Masquerade.Character;
using Masquerade.Models;

namespace Masquerade
{
    public class CharacterContainer : IInstanced
    {
        public CharacterContainer() 
        {
            _equipmentIds = new HashSet<int>();
            _modEquipment = new HashSet<ModEquipment>();
            _modEffects = new HashSet<ModCharacterEffects>();
        }

        public int InstanceId { get; internal set; } = -1;

        public string Name { get; internal set; }

        public float Exp {  get; internal set; }

        public int Level { get; internal set; }

        public CharacterStats Stats { get; internal set; }

        public float CurrentHP { get; internal set; }

        private HashSet<ModEquipment> _modEquipment;
        private HashSet<ModCharacterEffects> _modEffects;

        private HashSet<int> _equipmentIds;

        internal void AddEquipmentId(int contentId)
        {
            if (InstanceId < 0 || HasEquipmentId(contentId))
                return;

            _equipmentIds.Add(contentId);
        }

        internal void RemoveEquipmentId(int contentId)
        {
            if (InstanceId < 0 || !HasEquipmentId(contentId))
                return;

            _equipmentIds.Remove(contentId);
        }

        public bool HasEquipment(int contentId) => _equipmentIds.Contains(contentId) && _modEquipment.Any(x => x.ContentId == contentId);
        public bool HasAccessory<T>() where T : ModAccessory
        {
            var contentId = Masquerade.Api.GetModAccessory<T>().ContentId;
            return _equipmentIds.Contains(contentId) && _modEquipment.Any(x => x.ContentId == contentId);
        }
        private bool HasEquipmentId(int contentId) => _equipmentIds.Contains(contentId);

        internal void AddModEquipment(ModEquipment equip)
        {
            if (!_equipmentIds.Contains(equip.ContentId))
                return;

            _modEquipment.Add(equip);
        }

        internal void RemoveModEquipment(int contentId)
        {
            if (!_equipmentIds.Contains(contentId))
                return;

            _modEquipment.Remove(GetEquipment(contentId));
            _equipmentIds.Remove(contentId);
        }

        public ModEquipment GetEquipment(int contentId) => _modEquipment.Single(x => x.ContentId == contentId);
    }
}
