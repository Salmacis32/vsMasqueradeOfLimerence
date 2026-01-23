namespace Masquerade.Equipment
{
    public class LevelingManager
    {
        private static readonly string[] _booleanDataNames = { WeaponStats.CanHitWalls, WeaponDataNames.EquipmentIsAccessory, WeaponDataNames.Sealable, WeaponDataNames.StartsUnlocked, WeaponDataNames.StartsSeen };
        private HashSet<WeaponDataInfo> _weaponData;

        public LevelingManager()
        {
            _weaponData = new HashSet<WeaponDataInfo>();
        }

        public void AddAtLevel(int level, string stat, int value)
        {
            _weaponData.Add(new WeaponDataInfo(level, stat, value));
        }

        public void AddAtLevel(int level, string stat, float value)
        {
            _weaponData.Add(new WeaponDataInfo(level, stat, value));
        }

        public void AddAtLevel(int level, string flag, bool value)
        {
            _weaponData.Add(new WeaponDataInfo(level, flag, value));
        }

        public void AddAtLevel(int level, string datapoint, string value)
        {
            _weaponData.Add(new WeaponDataInfo(level, datapoint, value));
        }

        // Todo: Combine multiple instances of the same stat
        public void AddStatGrowth(int minLevel, int maxLevel, string stat, float value, int levelInterval = 1)
        {
            var levels = GrowthOperation(minLevel, maxLevel, levelInterval);
            foreach (var level in levels)
            {
                AddAtLevel(level, stat, value);
            }
        }

        public void AddStatGrowth(int minLevel, int maxLevel, string stat, int value, int levelInterval = 1)
        {
            var levels = GrowthOperation(minLevel, maxLevel, levelInterval);
            foreach (var level in levels)
            {
                AddAtLevel(level, stat, value);
            }
        }

        internal IEnumerable<WeaponDataInfo> GetDataAtLevel(int l)
        {
            return _weaponData.Where(x => x.Level == l);
        }

        private IEnumerable<int> GrowthOperation(int minLevel, int maxLevel, int levelInterval)
        {
            HashSet<int> levels = new HashSet<int>();
            for (int l = 1; l <= maxLevel; l++)
            {
                if (minLevel == l || maxLevel == l || (l - minLevel) % levelInterval == 0)
                    levels.Add(l);
            }
            return levels;
        }
    }
}