namespace Masquerade.Equipment
{
    public struct EquipmentStats
    {
        /// <summary>
        /// The damage factor of the weapons.
        /// </summary>
        /// <value>
        /// power
        /// </value>
        /// <remarks>
        /// Default: 1f
        /// </remarks>
        public float Power { get; set; }
        /// <summary>
        /// The area factor of the weapons.
        /// </summary>
        /// <value>
        /// area
        /// </value>
        /// <remarks>
        /// Default: 1f
        /// </remarks>
        public float Area { get; set; }
        /// <summary>
        /// The speed factor of the projectile spawned by weapons.
        /// </summary>
        /// <value>
        /// speed
        /// </value>
        /// <remarks>
        /// Default: 1f
        /// </remarks>
        public float Speed { get; set; }
        /// <summary>
        /// The amount of times the weapon or weapons will re-fire before cooldown.
        /// </summary>
        /// <value>
        /// amount
        /// </value>
        /// <remarks>
        /// Default: 1
        /// </remarks>
        public int Amount { get; set; }
        /// <summary>
        /// The amount of time each individual use of the weapon or weapons takes.
        /// </summary>
        /// <value>
        /// duration
        /// </value>
        /// <remarks>
        /// Must be set for weapons to fire.
        /// Default: 1000f
        /// </remarks>
        public float Duration { get; set; }
        /* Move this to ModWeapon
        public EquipmentStats()
        {
            Power = 1f;
            Area = 1f;
            Speed = 1f;
            Amount = 1;
            Duration = 1000f;
        }
        */
    }
}
