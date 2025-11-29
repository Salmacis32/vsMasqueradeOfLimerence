using Masquerade.Stats;

namespace Masquerade.Equipment
{
    public class LevelUp
    {
        public int Level { get; private set; }
        public CharacterModifierStats? CharacterStatsIncrease { get; set; }
        public WeaponStats? WeaponStatsIncrease { get; set; }
        public EquipmentStats? EquipmentStatsIncrease { get; set; }
        public LevelUp(int level) { Level = level; }
        public virtual bool ApplyCustomEffects(ModEquipment item) { return true; }
    }

    public static class LevelUpBuilder
    {
        public static LevelUp IncreaseStats(this LevelUp levelUp, CharacterModifierStats stats)
        {
            levelUp.CharacterStatsIncrease = stats;
            return levelUp;
        }
        public static LevelUp IncreaseStats(this LevelUp levelUp, WeaponStats stats)
        {
            levelUp.WeaponStatsIncrease = stats;
            return levelUp;
        }
        public static LevelUp IncreaseStats(this LevelUp levelUp, EquipmentStats stats)
        {
            levelUp.EquipmentStatsIncrease = stats;
            return levelUp;
        }
    }
}
