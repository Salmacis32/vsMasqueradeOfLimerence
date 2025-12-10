namespace Masquerade.Equipment
{
    public struct LevelUpInfo
    {
        public int Level { get; set; }
        public IDictionary<string, float> StatChanges { get; set; } = new Dictionary<string, float>();
        /// <summary>
        /// Creates a level that sets an equipment state to true or false at a given level.
        /// Sets the <see cref="LevelUpType"/> to <see cref="LevelUpType.SetState"/>.
        /// </summary>
        /// <param name="atLevel">Level requirement for the level up</param>
        /// <param name="setTrue">Whether to set the state value to true</param>
        /// <param name="stateName">The name of the state to be set <seealso cref="WeaponStats"/></param>
        public LevelUpInfo(int atLevel, string stateName, bool setTrue) 
        { 
            Level = atLevel; 
            StatChanges.Add(stateName, (setTrue) ? 1f : 0); 
        }

        /// <summary>
        /// Creates a level that sets the gains to stats an equipment grants at the given levels.
        /// Will apply the stat change over every level between <paramref name="minLevel"/> and <paramref name="maxLevel"/>.
        /// </summary>
        /// <param name="atLevel">Level requirement for the level up</param>
        /// <param name="statName">The name of the stat to be added or set to <seealso cref="CharacterStats"/><seealso cref="EquipmentStats"/><seealso cref="WeaponStats"/></param>
        /// <param name="statValue">The value of the stat to be changed by/to</param>
        public LevelUpInfo(int atLevel, string statName, float statValue)
        {
            Level = atLevel;
            StatChanges.Add(statName, statValue);
        }
    }
}
