#region

//文件创建者：Egg
//创建时间：09-17 01:27

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace EggFramework
{
    public static class LinqExtension
    {
        public static T GetRandom<T>(this List<T> list)
        {
            return list.Count == 0 ? default : list[Random.Range(0, list.Count)];
        }

        public static bool AddIfNotExist<T>(this List<T> list, T element)
        {
            if (list == null) return false;
            if (list.Contains(element)) return false;
            list.Add(element);
            return true;
        }

        public static void Shuffle<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return;
            for (var i = 0; i < list.Count; i++)
            {
                var randomIndex = Random.Range(0, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        public static bool AddIfNotExist<T>(this List<T> list, T element, Func<T, T, bool> equalFunc)
        {
            if (list == null || equalFunc == null) return false;
            if (list.Any((ele) => equalFunc.Invoke(ele, element))) return false;
            list.Add(element);
            return true;
        }
    }
}