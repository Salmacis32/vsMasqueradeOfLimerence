using Masquerade.Equipment;
using Masquerade.Stats;

namespace Masquerade
{
    public abstract class ModEquipment : ModContent
    {
        public ModEquipment()
        {
            if (DisplayName is null) DisplayName = ContentName;
            LevelUpInfo = new List<LevelUpInfo>();
            StatGrowth = new List<StatGrowthInfo>();
        }

        public virtual int AppearenceRate { get; protected set; } = 1;
        public virtual string Description { get; protected set; } = string.Empty;
        public virtual string DisplayName { get; protected set; }

        /// <summary>
        /// Collection of level-up stat changes at specific levels.
        /// </summary>
        /// <remarks>
        /// Will NOT create empty level ups between levels. Any levels past the max level will be set to the max level instead.
        /// Only used for initial prefab creation, will not have any effect during gameplay.
        /// </remarks>
        public ICollection<LevelUpInfo> LevelUpInfo { get; protected set; }

        public virtual int MaxLevel { get; protected set; } = 1;
        public virtual int Price { get; protected set; } = 40;
        public virtual ShopTags ShopTags { get; protected set; }
        public virtual string TextureName { get; protected set; } = string.Empty;

        public virtual string Tips { get; protected set; } = string.Empty;

        public virtual ICollection<StatGrowthInfo> StatGrowth { get; protected set; }

        protected void AddLevelUp(int atLevel, string stat, float value)
        {
            AddLevelUp(new LevelUpInfo(atLevel, stat, value));
        }

        protected void AddLevelUp(int atLevel, string state, bool setTrue)
        {
            AddLevelUp(new LevelUpInfo(atLevel, state, setTrue));
        }

        protected void AddLevelUp(LevelUpInfo levelUpInfo)
        {
            LevelUpInfo.Add(levelUpInfo);
        }

        protected void AddStatGrowth(int minLevel, int maxLevel, string stat, float value, int levelInterval = 1)
        {
            AddStatGrowth(new StatGrowthInfo(minLevel, maxLevel, stat, value, levelInterval));
        }

        protected void AddStatGrowth(StatGrowthInfo statGrowthInfo)
        {
            StatGrowth.Add(statGrowthInfo);
        }
    }
}