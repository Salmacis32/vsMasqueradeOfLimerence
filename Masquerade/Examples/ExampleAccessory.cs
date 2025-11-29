using Masquerade.Equipment;
using Masquerade.Stats;

namespace Masquerade.Examples
{
    public sealed class ExampleAccessory : ModAccessory
    {
        public override string DisplayName => "Example Accessory";

        protected override ICollection<LevelUp> PopulateLevelUps()
        {
            var levelUps = new HashSet<LevelUp>();

            var increase = new CharacterModifierStats() { MaxHealth = 100 };
            levelUps.Add(new LevelUp(2).IncreaseStats(increase));

            return levelUps;
        }
    }
}
