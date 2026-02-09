namespace Masquerade.Equipment
{
    /// <summary>
    /// The default stats for weapons.
    /// </summary>
    public static class WeaponStatNames
    {
        /// <summary>
        /// The chance of the weapon causing a critical hit.
        /// </summary>
        /// <value>
        /// critChance
        /// </value>
        /// <remarks>
        /// Default: 0f
        /// </remarks>
        public const string CritChance = "critChance";
        /// <summary>
        /// The multiplier to power that a critical hit will add to the damage.
        /// </summary>
        /// <value>
        /// critMul
        /// </value>
        /// <remarks>
        /// Default: 2.0f
        /// </remarks>
        public const string CritMultiplier = "critMul";
        /// <summary>
        /// The knockback factor of the weapon.
        /// </summary>
        /// <remarks>
        /// Default: 0f;
        /// </remarks>
        public const string Knockback = "knockback";
        /// <summary>
        /// The amount of times the weapon can be used before going on cooldown
        /// </summary>
        /// <remarks>
        /// Weapons must be set up to use charges manually.
        /// Default: 0
        /// </remarks>
        public const string Charges = "charges";
        /// <summary>
        /// The amount of enemies that the weapon is allowed to affect in one cycle.
        /// </summary>
        /// <remarks>
        /// Default: 1
        /// </remarks>
        public const string PierceAmount = "penetrating";
        /// <summary>
        /// The amount of time between each use of an weapon with an amount of more than 1.
        /// </summary>
        /// <remarks>
        /// Default: 0f
        /// </remarks>
        public const string RepeatDelay = "repeatInterval";
        /// <summary>
        /// The cooldown factor of the weapon.
        /// </summary>
        /// <remarks>
        /// Value is multiplied to character's cooldown.
        /// Default: 1f
        /// </remarks>
        public const string Interval = "interval";
        /// <summary>
        /// The factor chance of whether the weapon will fire on this interval.
        /// </summary>
        /// <remarks>
        /// Must be set up manually to be used.
        /// Default: 0f
        /// </remarks>
        public const string Chance = "chance";
        /// <summary>
        /// The maximum amount of projectiles this weapon can have spawned at one time.
        /// </summary>
        /// <remarks>
        /// Only set if the weapon needs to have limited amount of projectiles pooled.
        /// Default: null
        /// </remarks>
        public const string ProjectileLimit = "poolLimit";
        /// <summary>
        /// Whether or not the duration is added to the interval.
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public const string DoesIntervalAddDuration = "intervalDependsOnDuration";
        /// <summary>
        /// The delay in tics to allow the hitbox of a single projectile to be reused.
        /// </summary>
        /// <remarks>
        /// Default: null
        /// </remarks>
        public const string HitBoxDelay = "hitBoxDelay";
        /// <summary>
        /// Whether or not the projectiles will interact with stage terrain.
        /// </summary>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public const string CanHitWalls = "hitsWalls";
    }
}
