using Masquerade.Equipment;
using Masquerade.Stats;

namespace Masquerade
{
    public abstract class ModEquipment : ModContent
    {
        public ModEquipment()
        {
            if (DisplayName is null) DisplayName = ContentName;
            LevelingManager = new LevelingManager();
            AddLevelUp(WeaponDataNames.ShopPrice, Price);
            AddLevelUp(WeaponDataNames.ShopRarity, AppearenceRate);

            AddLevelUp(WeaponDataNames.StartsUnlocked, ShopTags.HasFlag(ShopTags.StartsUnlocked));
            AddLevelUp(WeaponDataNames.Sealable, !ShopTags.HasFlag(ShopTags.Unsealable));
            AddLevelUp(WeaponDataNames.StartsSeen, ShopTags.HasFlag(ShopTags.StartsSeen));
        }

        public virtual int AppearenceRate { get; protected set; }
        public virtual string Description { get; protected set; } = "Modded Item";
        public virtual string DisplayName { get; protected set; }

        public LevelingManager LevelingManager { get; private set; }

        public virtual int MaxLevel { get; protected set; }
        public virtual int Price { get; protected set; } = EquipmentDefaults.Price;
        public virtual ShopTags ShopTags { get; protected set; }
        public virtual string TextureName { get => ContentName; }

        public virtual string Tips { get; protected set; } = "";

        public void AddLevelUp(string stat, float value, int atLevel = 1)
        {
            LevelingManager.AddAtLevel(atLevel, stat, value);
        }

        public void AddLevelUp(string flag, bool setTrue, int atLevel = 1)
        {
            LevelingManager.AddAtLevel(atLevel, flag, setTrue);
        }

        public void AddLevelUp(string stat, int value, int atLevel = 1)
        {
            LevelingManager.AddAtLevel(atLevel, stat, value);
        }

        public void AddLevelUp(string datapointName, string datapointValue, int atLevel = 1)
        {
            LevelingManager.AddAtLevel(atLevel, datapointName, datapointValue);
        }
    }
}