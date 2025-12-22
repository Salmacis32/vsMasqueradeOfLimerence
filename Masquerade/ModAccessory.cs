using Masquerade.Equipment;

namespace Masquerade
{
    public abstract class ModAccessory : ModEquipment
    {
        public ModAccessory() : base()
        {
            AddLevelUp(1, "isPowerup", true);
        }
    }
}
