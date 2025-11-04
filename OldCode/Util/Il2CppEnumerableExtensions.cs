using Il2CppInterop.Runtime.InteropTypes;
using Il2CppSystem.Linq;
using Il2CppSystem.Diagnostics.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Il2Col = Il2CppSystem.Collections.Generic;

namespace Masquerade.Util
{
    public static class Il2CppEnumerableExtensions
    {
        /// <summary>
        /// Return as Il2CppSystem.List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static Il2Col.List<T> ToIl2CppList<T>(this IEnumerable<T> enumerable)
        {
            var il2CppList = new Il2Col.List<T>();

            using var enumerator = enumerable.GetEnumerator();
            while (enumerator.MoveNext())
                il2CppList.Add(enumerator.Current);

            return il2CppList;
        }

        /// <summary>
        /// Return as Normal List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static IList<T> ToSystemList<T>(this Il2Col.IEnumerable<T> enumerable)
        {
            var enumerator = enumerable.ToList();

            return enumerator.ToSystemList<T>();
        }

        /// <summary>
        /// Return as Normal Hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static HashSet<T> ToHashSet<T>(this Il2Col.HashSet<T> hash)
        {
            var set = new HashSet<T>();
            var enumerator = hash.GetEnumerator();
            while (enumerator.MoveNext())
            {
                set.Add(enumerator.Current);
            }

            return set;
        }

        /// <summary>
        /// Return as Normal Hashset
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static Il2Col.HashSet<T> ToIl2CppHashSet<T>(this HashSet<T> hash)
        {
            var set = new Il2Col.HashSet<T>();
            var enumerator = hash.GetEnumerator();
            while (enumerator.MoveNext())
            {
                set.Add(enumerator.Current);
            }

            return set;
        }

        /// <summary>
        /// Return as Il2CppSystem.List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public static Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<T> ToIl2CppArray<T>(this IEnumerable<T> enumerable) where T : unmanaged
        {
            var count = enumerable.Count();
            var il2CppList = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppStructArray<T>(count);

            using var enumerator = enumerable.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                il2CppList[i] = enumerator.Current;
                i++;
            }

            return il2CppList;
        }

        public static Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<T> ToIl2CppRefArray<T>(this IEnumerable<T> enumerable) where T : Il2CppObjectBase
        {
            var count = enumerable.Count();
            var il2CppList = new Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppReferenceArray<T>(count);

            using var enumerator = enumerable.GetEnumerator();
            int i = 0;
            while (enumerator.MoveNext())
            {
                il2CppList[i] = enumerator.Current;
                i++;
            }

            return il2CppList;
        }
    }
}
