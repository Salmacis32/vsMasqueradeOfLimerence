using Masquerade.Stats;

namespace Masquerade.Examples
{
    public sealed class ExampleAccessory : ModAccessory
    {
        public override string DisplayName => "Example Accessory";

        public override string Description => "Grants movespeed and max hp.";

        public override string Tips => "This is an example!";

        public override int MaxLevel => 6;

        public ExampleAccessory() : base()
        {
            AddStatGrowth(minLevel: 1, maxLevel: 6, CharacterStats.MaxHp, value: 0.5f, levelInterval: 2);
            AddStatGrowth(minLevel: 2, maxLevel: 6, CharacterStats.MoveSpeed, value: 0.5f, levelInterval: 2);
        }

        /// <summary>
        /// Makes the item start seen by player so it appears in the collection
        /// </summary>
        public override ShopTags ShopTags => base.ShopTags | ShopTags.StartsSeenByPlayer | ShopTags.StartsUnlocked;
    }
}
