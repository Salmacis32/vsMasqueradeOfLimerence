using Masquerade.Equipment;
using Newtonsoft.Json.Linq;

namespace Masquerade.Systems
{
    internal static class WeaponDataSystem
    {
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
            JTokenWriter tokenWriter = new JTokenWriter();
            tokenWriter.WriteStartArray();
            for(int l = 1; l <= levels; l++)
            {
                tokenWriter.WriteStartObject();
                if (l == 1)
                {
                    tokenWriter.WritePropertyName(WeaponDataNames.Level); tokenWriter.WriteValue(l);
                    tokenWriter.WritePropertyName(WeaponDataNames.Name); tokenWriter.WriteValue(eq.DisplayName);
                    tokenWriter.WritePropertyName(WeaponDataNames.ProjectileName); tokenWriter.WriteValue(eq.ContentId.ToString());
                    tokenWriter.WritePropertyName(WeaponDataNames.TextureName); tokenWriter.WriteValue(eq.TextureName);
                    tokenWriter.WritePropertyName(WeaponDataNames.SpriteName); tokenWriter.WriteValue(eq.TextureName);
                    tokenWriter.WritePropertyName(WeaponDataNames.ContentGroup); tokenWriter.WriteValue("EXTRA");
                }
                var growth = eq.LevelingManager.GetDataAtLevel(l);
                foreach (var stat in growth)
                {
                    if (!stat.HasAnyValues())
                        continue;
                    tokenWriter.WritePropertyName(stat.Name);
                    if (stat.ValueBool.HasValue)
                        tokenWriter.WriteValue(stat.ValueBool);
                    else if (stat.ValueFloat.HasValue)
                        tokenWriter.WriteValue(stat.ValueFloat);
                    else if (stat.ValueInt.HasValue)
                        tokenWriter.WriteValue(stat.ValueInt);
                    else if (stat.ValueString != null)
                        tokenWriter.WriteValue(stat.ValueString);
                }
                tokenWriter.WriteEndObject();
            }
            tokenWriter.WriteEndArray();
            return tokenWriter.Token;
        }
    }
}
