using Masquerade.Equipment;
using Masquerade.Stats;
using Il2CppVampireSurvivors.Objects;
using Masquerade.Models;

namespace Masquerade
{
    public abstract class ModEquipment : ModContent
    {
        public ModEquipment()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (DisplayName is null) DisplayName = ContentName;
            if (TextureName is null) TextureName = ContentName;
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
        public virtual string TextureName { get; protected set; }

        public virtual string Tips { get; protected set; } = "";

        public CharacterContainer Owner { get; set; }

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