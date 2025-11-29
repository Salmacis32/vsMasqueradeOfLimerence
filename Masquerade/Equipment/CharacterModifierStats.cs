namespace Masquerade.Stats
{
    public struct CharacterModifierStats
    {
        /// <summary>
        /// The bonus to cooldown that is applied to the interval of the character's equipment.
        /// </summary>
        /// <value>
        /// cooldown
        /// </value>
        /// <remarks>
        /// Negative numbers reduce interval, positive numbers increase it.
        /// Default: 0f
        /// </remarks>
        public float Cooldown { get; set; }
        public float MaxHealth { get; set; }
        public float MoveSpeed { get; set; }
        public float Growth { get; set; }
        public float Magnet { get; set; }
        public float Luck { get; set; }
        public float Armor { get; set; }
        public float HealthRegen { get; set; }
        public float Revivals { get; set; }
        public float Rerolls { get; set; }
        public float Skips { get; set; }
        public float Curse { get; set; }
    }
}
