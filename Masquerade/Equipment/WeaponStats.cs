namespace Masquerade.Stats
{
    /// <summary>
    /// The default stats for weapons.
    /// </summary>
    public struct WeaponStats
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
        public float CritChance { get; set; }
        /// <summary>
        /// The multiplier to power that a critical hit will add to the damage.
        /// </summary>
        /// <value>
        /// critMul
        /// </value>
        /// <remarks>
        /// Default: 2.0f
        /// </remarks>
        public float CritDamage { get; set; }
        /// <summary>
        /// The knockback factor of the weapon.
        /// </summary>
        /// <value>
        /// knockback
        /// </value>
        /// <remarks>
        /// Default: 0f;
        /// </remarks>
        public float Knockback { get; set; }
        /// <summary>
        /// The amount of times the weapon can be used before going on cooldown
        /// </summary>
        /// <value>
        /// charges
        /// </value>
        /// <remarks>
        /// Weapon must be set up to use charges manually.
        /// Default: 0
        /// </remarks>
        public int Charges { get; set; }
        /// <summary>
        /// The amount of enemies that the weapon is allowed to affect in one cycle.
        /// </summary>
        /// <value>
        /// penetrating
        /// </value>
        /// <remarks>
        /// Default: 1
        /// </remarks>
        public int PierceAmount { get; set; }
        
        /// <summary>
        /// The amount of time between each use of an weapon with an amount of more than 1.
        /// </summary>
        /// <value>
        /// repeatInterval
        /// </value>
        /// <remarks>
        /// Default: 0f
        /// </remarks>
        public float RepeatDelay { get; set; }
        /// <summary>
        /// The cooldown factor of the weapon.
        /// </summary>
        /// <value>
        /// interval
        /// </value>
        /// <remarks>
        /// Value is multiplied to character's cooldown.
        /// Default: 1f
        /// </remarks>
        public float Interval { get; set; }
        /// <summary>
        /// The factor chance of whether the weapon will fire on this interval.
        /// </summary>
        /// <value>
        /// chance
        /// </value>
        /// <remarks>
        /// Must be set up manually to be used.
        /// Default: 0f
        /// </remarks>
        public float Chance { get; set; }
        /// <summary>
        /// The maximum amount of projectiles this weapon can have spawned at one time.
        /// </summary>
        /// <value>
        /// poolLimit
        /// </value>
        /// <remarks>
        /// Only set if the weapon needs to have limited amount of projectiles pooled.
        /// Default: null
        /// </remarks>
        public int? ProjectileLimit { get; set; }
        /// <summary>
        /// Whether or not the duration is added to the interval.
        /// </summary>
        /// <value>
        /// intervalDependsOnDuration
        /// </value>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool DoesIntervalAddDuration { get; set; }
        /// <summary>
        /// The delay in tics to allow the hitbox of a single projectile to be reused.
        /// </summary>
        /// <value>
        /// hitBoxDelay
        /// </value>
        /// <remarks>
        /// Default: null
        /// </remarks>
        public float? HitBoxDelay { get; set; }
        /// <summary>
        /// Whether or not the projectiles will interact with stage terrain.
        /// </summary>
        /// <value>
        /// hitsWalls
        /// </value>
        /// <remarks>
        /// Default: false
        /// </remarks>
        public bool CanHitWalls { get; set; }
    }
}
