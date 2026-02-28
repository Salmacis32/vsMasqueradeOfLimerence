namespace Masquerade.Character
{
    public struct CharacterStats
    {
        public CharacterStats() { }
        internal CharacterStats(float cd, float hp, float spd, float gro, float mag, float lck, float arm, float reg, float gre, float crs)
        {
            Cooldown = cd;
            MaxHp = hp;
            MoveSpeed = spd;
            Growth = gro;
            Magnet = mag;
            Luck = lck;
            Armor = arm;
            HpRegen = reg;
            Greed = gre;
            Curse = crs;
        }
        public float Cooldown { get; set; }
        public float MaxHp { get; set; }
        public float MoveSpeed { get; set; }
        public float Growth { get; set; }
        public float Magnet { get; set; }
        public float Luck { get; set; }
        public float Armor { get; set; }
        public float HpRegen { get; set; }
        // Move this to a PlayerStats or something
        /*
        public float Revivals;
        public float Rerolls;
        public float Skips;
        */
        public float Curse { get; set; }

        public float Greed { get; set; }
    }
}
