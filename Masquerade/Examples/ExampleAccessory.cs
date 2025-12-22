using Masquerade.Equipment;

namespace Masquerade.Examples
{
    public sealed class ExampleAccessory : ModAccessory
    {
        public override string DisplayName => "Example Accessory";

        public override int MaxLevel => 6;

        public ExampleAccessory() : base()
        {
            AddStatGrowth(minLevel: 1, maxLevel: 6, CharacterStats.MaxHp, value: 100);
        }
    }
}
