using Masquerade.Equipment;

namespace Masquerade
{
    public abstract class ModAccessory : ModEquipment
    {
        public ModAccessory() : base()
        {
            LevelUpInfo.Add(new LevelUpInfo(1, "isPowerup", true));
        }
    }
}
