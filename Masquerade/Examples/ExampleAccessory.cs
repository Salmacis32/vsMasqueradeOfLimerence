using Masquerade.Equipment;

namespace Masquerade.Examples
{
    public sealed class ExampleAccessory : ModAccessory
    {
        public override string DisplayName => "Example Accessory";

        public ExampleAccessory() : base()
        {
            StatGrowth.Add(new StatGrowthInfo(1, 6, CharacterStats.MaxHp, 100));
        }
    }
}
