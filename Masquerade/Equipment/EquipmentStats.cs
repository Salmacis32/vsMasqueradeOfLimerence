namespace Masquerade.Equipment
{
    public struct EquipmentStats
    {
        public EquipmentStats() { }
        public EquipmentStats(float pwr, float are, float spd, int amt, float crt, float mul, float del, float itv, float cha, int pen, float? dur = null, int? lim = null, float? kbk = null, bool wal = false) 
        {
            Power = pwr;
            Area = are;
            ProjectileSpeed = spd;
            Amount = amt;
            Duration = dur;
            CritChance = crt;
            CritMultiplier = mul;
            Knockback = kbk;
            RepeatDelay = del;
            Interval = itv;
            Chance = cha;
            ProjectileLimit = lim;
            CanHitWalls = wal;
            PierceAmount = pen;
        }

        public float Power { get; set; }
        public float Area { get; set; }
        public float ProjectileSpeed { get; set; }
        public int Amount { get; set; }
        public float? Duration { get; set; }
        public float CritChance { get; set; }
        public float CritMultiplier { get; set; }
        public float? Knockback { get; set; }
        public int Charges { get; set; }
        public int PierceAmount { get; set; }
        public float RepeatDelay { get; set; }
        public float Interval { get; set; }
        public float Chance { get; set; }
        public int? ProjectileLimit { get; set; }
        public bool IntervalAddsDuration { get; set; }
        public int HitBoxDelay { get; set; }
        public bool CanHitWalls { get; set; }
    }
}
