using Masquerade.Equipment;
using Masquerade.Stats;

namespace Masquerade
{
    public abstract class ModEquipment : ModContent
    {

        public virtual string DisplayName { get; protected set; }

        public virtual string Description { get; protected set; } = string.Empty;

        public virtual string TextureName { get; protected set; } = string.Empty;

        public virtual string Tips { get; protected set; } = string.Empty;

        public virtual int LevelUpWeight { get; protected set; } = 1;

        public virtual int Price { get; protected set; } = 40;

        public virtual ShopTags ShopTags { get; protected set; }

        /// <summary>
        /// Collection of level-up effects.
        /// </summary>
        /// <remarks>
        /// Level-ups marked with negatives, 0, or 1 will apply upon receiving the item.
        /// Will NOT create empty level ups between levels. Levels will populate one after the other regardless of gaps.
        /// Only used for initial prefab creation, will not have any effect during gameplay.
        /// </remarks>
        public ICollection<LevelUp> LevelUps { get; private set; }

        public ModEquipment() 
        {
            if (DisplayName is null) DisplayName = ContentName;
            LevelUps = PopulateLevelUps() ?? [];
        }

        protected abstract ICollection<LevelUp> PopulateLevelUps();

        public int MaxLevel { get => LevelUps.DistinctBy(x => x.Level).Count(x => x.Level != 1) + 1; }
    }
}
