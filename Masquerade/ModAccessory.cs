using Masquerade.Equipment;
using Masquerade.Stats;

namespace Masquerade
{
    public abstract class ModAccessory : ModEquipment
    {
        public ModAccessory() : base()
        {
            AddFirstLevel("isPowerup", true);
        }

        public override int AppearenceRate => 90;
    }
}
