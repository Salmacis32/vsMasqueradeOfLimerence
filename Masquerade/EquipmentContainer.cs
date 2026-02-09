using Masquerade.Character;
using Masquerade.Equipment;
using Masquerade.Models;

namespace Masquerade
{
    public class EquipmentContainer : IInstanced
    {
        public EquipmentContainer() 
        {
        }

        public int InstanceId { get; internal set; } = -1;

        public int EquipmentType { get; internal set; }

        public string Name { get; internal set; }

        public int Level { get; internal set; }

        public EquipmentStats EquipStats { get; internal set; }

        public CharacterStats ModifierStats { get; internal set; }
    }
}
