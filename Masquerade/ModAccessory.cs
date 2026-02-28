using Masquerade.Equipment;

namespace Masquerade
{
    public abstract class ModAccessory : ModEquipment
    {
        public ModAccessory() : base()
        {
            AddLevelUp(WeaponDataNames.EquipmentIsAccessory, true);
        }

        public override int AppearenceRate => EquipmentDefaults.AccessoryRarity;

        public override int MaxLevel => EquipmentDefaults.MaxAccessoryLevel;

        public virtual void OnAccessoryAdded(CharacterContainer character) { }
        public virtual void OnAccessoryRemoved(CharacterContainer character) { }

        public virtual void OnLevelUp() { }
    }
}
