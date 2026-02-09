namespace Masquerade.Equipment
{
    public static class EquipmentStatNames
    {
        /// <summary>
        /// The damage factor of weapons.
        /// </summary>
        /// <value>
        /// power
        /// </value>
        /// <remarks>
        /// Default: 1f
        /// </remarks>
        public const string Power = "power";
        /// <summary>
        /// The area factor of weapons.
        /// </summary>
        /// <value>
        /// area
        /// </value>
        /// <remarks>
        /// Default: 1f
        /// </remarks>
        public const string Area = "area";
        /// <summary>
        /// The speed factor of the projectile spawned by weapons.
        /// </summary>
        /// <value>
        /// speed
        /// </value>
        /// <remarks>
        /// Default: 1f
        /// </remarks>
        public const string ProjectileSpeed = "speed";
        /// <summary>
        /// The amount of times the weapon or weapons will re-fire before cooldown.
        /// </summary>
        /// <value>
        /// amount
        /// </value>
        /// <remarks>
        /// Default: 1
        /// </remarks>
        public const string Amount = "amount";
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
        public const string Duration = "duration";
    }
}
