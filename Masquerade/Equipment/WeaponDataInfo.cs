using static UnityEngine.CullingGroup;

namespace Masquerade.Equipment
{
    internal struct WeaponDataInfo
    {
        internal int Level { get; private set; }
        internal string Name { get; private set; }
        internal float? ValueFloat { get; private set; }
        internal string ValueString { get; private set; }
        internal int? ValueInt { get; private set; }
        internal bool? ValueBool { get; private set; }

        internal WeaponDataInfo(int level, string statName, float statValue)
        {
            Level = level;
            Name = statName;
            ValueFloat = statValue;
        }

        internal WeaponDataInfo(int level, string statName, int statValue)
        {
            Level = level;
            Name = statName;
            ValueInt = statValue;
        }


        internal WeaponDataInfo(int level, string dataName, string dataValue)
        {
            Level = level;
            Name = dataName;
            ValueString = dataValue;
        }

        internal WeaponDataInfo(int level, string flagName, bool flagValue)
        {
            Level = level;
            Name = flagName;
            ValueBool = flagValue;
        }

        internal bool HasAnyValues() => ValueString != null || ValueFloat.HasValue || ValueInt.HasValue || ValueBool.HasValue;
    }
}
