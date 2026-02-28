using Masquerade.Character;
using Masquerade.Equipment;
using Masquerade.Models;

namespace Masquerade
{
    public class EquipmentContainer : IInstanced, IContainer
    {
        public EquipmentContainer() 
        {
        }

        public int InstanceId { get; internal set; } = -1;

        public int TypeId { get; internal set; }

        public string Name { get; internal set; }

        public int Level { get; internal set; }

        public EquipmentStats EquipStats { get; internal set; }

        public CharacterStats ModifierStats { get; internal set; }

        public bool UpdateContainerNextTick { get; }

        public void UpdateContainer()
        {
            throw new NotImplementedException();
        }
    }
}
