using Newtonsoft.Json.Linq;

namespace Masquerade.Systems
{
    internal static class WeaponDataSystem
    {
        internal static IDictionary<int, JArray> GenerateCustomWeaponData(IDictionary<MasqMod, IEnumerable<ModEquipment>> equipment)
        {
            var allWeaponData = new Dictionary<int, JArray>();

            // Figure out how to make selectmany work here, this is stupid
            foreach (var mod in equipment.Where(x => !x.Key.IgnoreWeapons))
            {
                foreach (var equip in mod.Value)
                {
                    allWeaponData.Add(equip.ContentId, (JArray)SerializeEquipment(equip));
                }
            }

            return allWeaponData;
        }

        private static JToken SerializeEquipment(ModEquipment eq)
        {
            int levels = eq.MaxLevel;

            JTokenWriter tokenWriter = new JTokenWriter();
            tokenWriter.WriteStartArray();
            for(int l = 1; l <= levels; l++)
            {
                tokenWriter.WriteStartObject();
                var growth = eq.StatGrowth.Where(x => x.StartLevel == l || x.EndLevel == l || (l - x.StartLevel) % x.Interval == 0);
                foreach (var stat in growth)
                {
                    tokenWriter.WritePropertyName(stat.StatName);
                    tokenWriter.WriteValue(stat.StatValue);
                }
                tokenWriter.WriteEndObject();
            }
            tokenWriter.WriteEndArray();
            return tokenWriter.Token;
        }
    }
}
