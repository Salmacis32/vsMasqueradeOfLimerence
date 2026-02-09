using Masquerade.Character;
using Masquerade.Stats;

namespace Masquerade.Examples
{
    public sealed class ExampleAccessory : ModAccessory
    {
        public override string DisplayName => "Example Accessory";

        public override string Description => "Grants movespeed and max hp.";

        public override string Tips => "This is an example!";

        public override int AppearenceRate => 5000;

        public ExampleAccessory() : base()
        {
            LevelingManager.AddStatGrowth(minLevel: 1, maxLevel: 5, CharacterStatNames.MaxHp, value: 0.2f, levelInterval: 2);
            LevelingManager.AddStatGrowth(minLevel: 2, maxLevel: 5, CharacterStatNames.MoveSpeed, value: 0.2f, levelInterval: 2);
        }

        /// <summary>
        /// Makes the item start seen by player so it appears in the collection for testing purposes
        /// </summary>
        public override ShopTags ShopTags => ShopTags.StartsSeen | ShopTags.StartsUnlocked;

        public override void OnAccessoryAdded(CharacterContainer character)
        {
            Masquerade.Logger.Msg($"{nameof(ExampleAccessory)} Added to character {character.Name} level {character.Level}");
        }

        public override void OnAccessoryRemoved(CharacterContainer character)
        {
            Masquerade.Logger.Msg($"{nameof(ExampleAccessory)} Removed from character {character.Name} level {character.Level}");
        }

        public override void OnLevelUp()
        {
            Masquerade.Logger.Msg($"{nameof(ExampleAccessory)} Leveled Up on character {Owner.Name}");
        }
    }
}
