using Il2Col = Il2CppSystem.Collections.Generic;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
namespace Masquerade.Util
{

    /// <summary>
    /// Extensions for non il2cpp Lists
    /// </summary>
    public static partial class ListExt
    {
        /// <summary>
        /// Return as Il2CppSystem.List
        /// </summary>
        public static Il2Col.List<T> ToIl2CppList<T>(this List<T> list)
        {
            var il2CppList = new Il2Col.List<T>();
            foreach (var item in list)
                il2CppList.Add(item);

            return il2CppList;
        }

        /// <summary>
        /// Return as Normal List
        /// </summary>
        public static IList<T> ToSystemList<T>(this Il2Col.List<T> list)
        {
            var sysList = new List<T>();
            foreach (var item in list)
                sysList.Add(item);

            return sysList;
        }

        /// <summary>
        /// Return as Il2CppReferenceArray
        /// </summary>
        public static Il2CppReferenceArray<T> ToIl2CppReferenceArray<T>(this List<T> list)
            where T : Il2CppSystem.Object
        {
            var il2cppArray = new Il2CppReferenceArray<T>(list.Count);

            for (var i = 0; i < list.Count; i++)
                il2cppArray[i] = list[i];

            return il2cppArray;
        }
        /*
        /// <summary>
        /// Return a duplicate of this
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Il2Col.List<T> Duplicate<T>(this Il2Col.List<T> list)
        {
            var newList = new Il2Col.List<T>();
            foreach (var item in list)
                newList.Add(item);

            return newList;
        }

        /// <summary>
        /// Return a duplicate of this as type TCast
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCast"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Il2Col.List<TCast> DuplicateAs<TSource, TCast>(
            this Il2Col.List<TSource> list)
            where TSource : Il2CppSystem.Object where TCast : Il2CppSystem.Object
        {
            var newList = new Il2Col.List<TCast>();
            foreach (var item in list)
                newList.Add(item.TryCast<TCast>());

            return newList;
        }

        /// <summary>
        /// Check if this has any items of type TCast
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCast">The Type you're checking for</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool HasItemsOfType<TSource, TCast>(this Il2Col.List<TSource> list)
            where TSource : Il2CppSystem.Object
            where TCast : Il2CppSystem.Object
        {
            foreach (var item in list)
            {
                try
                {
                    if (item.IsType<TCast>())
                        return true;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return false;
        }

        /// <summary>
        /// Return the first item of type TCast
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCast">The Type of the Item you want</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static TCast GetItemOfType<TSource, TCast>(this Il2CppSystem.Collections.Generic.List<TSource> list)
            where TCast : Il2CppSystem.Object
            where TSource : Il2CppSystem.Object
        {
            if (!HasItemsOfType<TSource, TCast>(list))
                return null;

            foreach (var item in list)
            {
                try
                {
                    if (item.IsType(out TCast tryCast))
                        return tryCast;
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return null;
        }

        /// <summary>
        /// Return all Items of type TCast
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCast">The Type of the Items you want</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Il2CppSystem.Collections.Generic.List<TCast> GetItemsOfType<TSource, TCast>(
            this Il2CppSystem.Collections.Generic.List<TSource> list)
            where TSource : Il2CppSystem.Object
            where TCast : Il2CppSystem.Object
        {
            var results = new System.Collections.Generic.List<TCast>();
            foreach (var item in list)
            {
                try
                {
                    if (item.IsType(out TCast tryCast))
                        results.Add(tryCast);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            return results;
        }

        /// <summary>
        /// Return this with the first Item of type TCast removed
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCast">The Type of the Item you want to remove</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Il2CppSystem.Collections.Generic.List<TSource> RemoveItemOfType<TSource, TCast>(
            this Il2CppSystem.Collections.Generic.List<TSource> list)
            where TSource : Il2CppSystem.Object
            where TCast : Il2CppSystem.Object
        {
            var item = GetItemOfType<TSource, TCast>(list);
            return item != null ? RemoveItem(list, item) : list;
        }

        /// <summary>
        /// Return this with the first Item of type TCast removed
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCast">The Type of the Item you want to remove</typeparam>
        /// <param name="list"></param>
        /// <param name="itemToRemove">The specific Item to remove</param>
        /// <returns></returns>
        public static Il2CppSystem.Collections.Generic.List<TSource> RemoveItem<TSource, TCast>(
            this Il2CppSystem.Collections.Generic.List<TSource> list, TCast itemToRemove)
            where TSource : Il2CppSystem.Object where TCast : Il2CppSystem.Object
        {
            if (!HasItemsOfType<TSource, TCast>(list))
                return list;

            var newList = list;
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item is null || !item.Equals(itemToRemove.TryCast<TCast>()))
                    continue;

                newList.RemoveAt(i);
                break;
            }

            return newList;
        }

        /// <summary>
        /// Return this with all Items of type TCast removed
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TCast">The Type of the Items that you want to remove</typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static Il2CppSystem.Collections.Generic.List<TSource> RemoveItemsOfType<TSource, TCast>(
            this Il2CppSystem.Collections.Generic.List<TSource> list)
            where TSource : Il2CppSystem.Object
            where TCast : Il2CppSystem.Object
        {
            if (!HasItemsOfType<TSource, TCast>(list))
                return list;

            var newList = list;
            var numRemoved = 0;
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (item is null || !item.IsType<TCast>())
                    continue;

                newList.RemoveAt(i - numRemoved);
                numRemoved++;
            }

            return newList;
        }
        */
    }
}