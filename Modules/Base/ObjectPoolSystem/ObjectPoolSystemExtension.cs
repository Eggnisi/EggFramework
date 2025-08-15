#region

//文件创建者：Egg
//创建时间：01-30 07:14

#endregion

using System.Collections.Generic;
using EggFramework.ObjectPool;
using EggFramework.TimeSystem;
using QFramework;

namespace EggFramework
{
    public static class ObjectPoolSystemExtension
    {
        private static Dictionary<int, int> _hashDic = new();

        public static void ReturnObj<T>(this IObjectPoolSystem self, string entityId, T obj, float time)
        {
            var ts   = self.GetSystem<ITimeSystem>();
            var hash = obj.GetHashCode();
            var id   = ts.RegisterDelayTask(() => { self.ReturnObj(entityId, obj); }, time);
        }
    }
}