using Il2CppInterop.Runtime;
using Il2Col = Il2CppSystem.Collections.Generic;
using System.Linq;

namespace Masquerade.Util
{
    /// <summary>
    /// Extensions for il2cpp dictionaries
    /// </summary>
    public static class Il2CppDictionaryExtensions
    {
        /// <summary>
        /// Deconstruct method of IL2CPP KeyValuePairs
        /// </summary>
        public static void Deconstruct<K, V>(this Il2Col.KeyValuePair<K, V> kvp, out K k, out V v)
        {
            k = kvp.key;
            v = kvp.value;
        }

        /// <summary>
        /// Get all of the keys from this Dictionary as a list
        /// </summary>
        public static Il2Col.List<TKey> GetKeys<TKey, TValue>(this Il2Col.Dictionary<TKey, TValue> keyValuePairs) =>
            keyValuePairs.Keys().ToIl2CppList();

        /// <summary>
        /// Get all of the values from this Dictionary as a list
        /// </summary>
        public static Il2Col.List<TValue> GetValues<TKey, TValue>(this Il2Col.Dictionary<TKey, TValue> keyValuePairs) =>
            keyValuePairs.Values().ToIl2CppList();

        /// <summary>
        /// Get all of the keys from this Dictionary
        /// </summary>
        public static IEnumerable<TKey> Keys<TKey, TValue>(
            this Il2Col.Dictionary<TKey, TValue> keyValuePairs)
        {
            foreach (var (k, _) in keyValuePairs)
            {
                yield return k;
            }
        }

        public static bool ContainsIl2CppKey<TKey, TValue>(this Il2Col.Dictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.Keys().Any(x => x.Equals(key));

        /// <summary>
        /// Get all of the values from this Dictionary
        /// </summary>
        public static IEnumerable<TValue> Values<TKey, TValue>(
            this Il2Col.Dictionary<TKey, TValue> keyValuePairs)
        {
            foreach (var (_, v) in keyValuePairs)
            {
                yield return v;
            }
        }

        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this Il2Col.Dictionary<TKey, TValue> dictionary)
        {
            var newDict = new Dictionary<TKey, TValue>();
            foreach (var (k, v) in dictionary)
            {
                newDict.Add(k, v);
            }
            return newDict;
        }

        public static Il2Col.Dictionary<TKey, TValue> ToIl2CppDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        {
            var newDict = new Il2Col.Dictionary<TKey, TValue>();
            foreach (var (k, v) in dictionary)
            {
                newDict.Add(k, v);
            }
            return newDict;
        }
    }
}