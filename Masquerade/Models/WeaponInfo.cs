
using HarmonyLib;
using Il2CppVampireSurvivors.Data;
using Il2CppVampireSurvivors.Data.Weapons;
using Il2Col = Il2CppSystem.Collections.Generic;

namespace Masquerade.Models
{
    /// <summary>
    /// Class used to contain weapon data for custom weapons
    /// </summary>
    public class WeaponInfo
    {
        public int WeaponId;
        public IEnumerable<WeaponData> LeveledWeaponData { get; set; }

        public WeaponInfo()
        {
            LeveledWeaponData = new List<WeaponData>();
        }

        public WeaponInfo(IEnumerable<WeaponData> weaponDataList)
        {
            if (weaponDataList is null) throw new ArgumentNullException(nameof(weaponDataList));
            LeveledWeaponData = weaponDataList;
        }

        public WeaponInfo(WeaponType weaponType, Il2Col.List<WeaponData> weaponDataList)
        {
            if (weaponDataList is null) throw new ArgumentNullException(nameof(weaponDataList));
            LeveledWeaponData = new List<WeaponData>();
            foreach (var weaponData in weaponDataList)
            {
                LeveledWeaponData.AddItem(weaponData);
            }
            WeaponId = (int)weaponType;
        }

        public WeaponInfo(Il2Col.KeyValuePair<WeaponType, Il2Col.List<WeaponData>> weaponKvp)
        {
            LeveledWeaponData = new List<WeaponData>();
            var i = 0;
            foreach (var weaponData in weaponKvp.Value)
            {
                i++;
                if (weaponData.level == 0) weaponData.level = i;
                LeveledWeaponData = LeveledWeaponData.AddItem(weaponData);
            }
            WeaponId = (int)weaponKvp.Key;
        }

        /// <summary>
        /// Tries and get data at a given level. Will set 0 to 1 and grab the last level if incorrect level is passed
        /// </summary>
        /// <param name="level">The level of the weapon data requested</param>
        public WeaponData TryGetDataAtLevel(int level)
        {
            if (level == 0) level = 1;
            if (!LeveledWeaponData.Any(x => x.level == level)) level = LeveledWeaponData.Max(x => x.level);
            return GetDataAtLevel(level);
        }

        /// <summary>
        /// Gets data at given level. Throws exception if level does not exist.
        /// </summary>
        /// <param name="level">The level of the weapon data requested</param>
        public WeaponData GetDataAtLevel(int level)
        {
            if (!LeveledWeaponData.Any(x => x.level == level)) throw new ArgumentOutOfRangeException(nameof(level), $"Level {level} does not exist for weapon {WeaponId}");
            return LeveledWeaponData.Single(x => x.level == level);
        }

        public WeaponType IdAsType { get => (WeaponType)WeaponId; }

        public string WeaponName { get => LeveledWeaponData?.Select(x => x?.name).FirstOrDefault(); }

        public string WeaponDescription { get => LeveledWeaponData?.Select(x => x?.description).FirstOrDefault(); }

        public string WeaponTips { get => LeveledWeaponData?.Select(x => x?.tips).FirstOrDefault(); }
    }
}
