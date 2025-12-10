namespace Masquerade.Equipment
{
    public struct StatGrowthInfo
    {
        public int StartLevel { get; private set; }
        public int EndLevel { get; private set; }
        public int Interval { get; private set; }
        public string StatName { get; private set; }
        public float StatValue { get; private set; }

        /// <summary>
        /// Creates a level that sets the gains to stats an equipment grants at the given levels.
        /// Will apply the stat change over every level between <paramref name="minLevel"/> and <paramref name="maxLevel"/>.
        /// </summary>
        /// <param name="minLevel">The first level that will apply the stat change</param>
        /// <param name="maxLevel">The last level</param>
        /// <param name="statName">The name of the stat to be added or set to <seealso cref="CharacterStats"/><seealso cref="EquipmentStats"/><seealso cref="WeaponStats"/></param>
        /// <param name="statValue">The value of the stat to be changed by/to</param>
        /// <param name="levelInterval">How many levels are needed to apply the change</param>
        public StatGrowthInfo(int minLevel, int maxLevel, string statName, float statValue, int levelInterval = 1) 
        { 
            StartLevel = minLevel; EndLevel = maxLevel;
            StatName = statName; StatValue = statValue;
            Interval = levelInterval;
        }
    }
}
