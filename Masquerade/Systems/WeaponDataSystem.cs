using Masquerade.Equipment;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace Masquerade.Systems
{
    internal static class WeaponDataSystem
    {
        private static readonly string[] BooleanDataNames = { WeaponStats.CanHitWalls, "isPowerup", "sealable", "isUnlocked", "seen" };
        internal static IDictionary<int, JArray> GenerateCustomWeaponData(IDictionary<MasqMod, IEnumerable<ModEquipment>> equipment)
        {
            var allWeaponData = new Dictionary<int, JArray>();
            Func<KeyValuePair<MasqMod, IEnumerable<ModEquipment>>, IEnumerable<ModEquipment>> function = x => { return x.Value; };
            var mods = equipment.Where(x => !x.Key.IgnoreWeapons);
            var accessories = mods.SelectMany(function);

            foreach (var equip in accessories)
            {
                allWeaponData.Add(equip.ContentId, (JArray)SerializeEquipment(equip));
            }

            return allWeaponData;
        }

        private static JToken SerializeEquipment(ModEquipment eq)
        {
            int levels = eq.MaxLevel;
            Func<LevelUpInfo, IEnumerable<KeyValuePair<string, float>>> function = x => { return x.StatChanges; };

            JTokenWriter tokenWriter = new JTokenWriter();
            tokenWriter.WriteStartArray();
            for(int l = 1; l <= levels; l++)
            {
                tokenWriter.WriteStartObject();
                tokenWriter.WritePropertyName("level"); tokenWriter.WriteValue(l);
                if (l == 1)
                {
                    tokenWriter.WritePropertyName("name"); tokenWriter.WriteValue(eq.DisplayName);
                    tokenWriter.WritePropertyName("bulletType"); tokenWriter.WriteValue(eq.ContentId.ToString());
                    // The following are all for testing only
                    tokenWriter.WritePropertyName("contentGroup"); tokenWriter.WriteValue("EXTRA");
                    tokenWriter.WritePropertyName("texture"); tokenWriter.WriteValue(eq.ContentName);
                    tokenWriter.WritePropertyName("frameName"); tokenWriter.WriteValue(eq.TextureName);
                }
                var growth = eq.StatGrowth.Where(x => x.StartLevel == l || x.EndLevel == l || (l - x.StartLevel) % x.Interval == 0);
                foreach (var stat in growth)
                {
                    tokenWriter.WritePropertyName(stat.StatName);
                    tokenWriter.WriteValue(stat.StatValue);
                }
                var levelUp = eq.LevelUpInfo.Where(x => x.Level == l);
                foreach (var change in levelUp.SelectMany(function))
                {
                    tokenWriter.WritePropertyName(change.Key);
                    tokenWriter.WriteValue((BooleanDataNames.Contains(change.Key)) ? CheckTrue(change.Value) : change.Value);
                }
                tokenWriter.WriteEndObject();
            }
            tokenWriter.WriteEndArray();
            return tokenWriter.Token;
        }

        private static bool CheckTrue(float value) => value > 0;
    }
}
