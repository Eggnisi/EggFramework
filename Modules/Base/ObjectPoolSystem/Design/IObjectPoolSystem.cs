#region

//文件创建者：Egg
//创建时间：09-08 10:19

#endregion

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using QFramework;
using UnityEngine;

namespace EggFramework.ObjectPool
{
    public interface IObjectPoolSystem : ISystem
    {
        GameObject Root { get; }

        GameObject GetPoolRoot(string entityId);

        //如果是预制体，需要先注册并且加载才能使用
        UniTask RegisterPrefabs(IEnumerable<string> prefabNames, Action<object> beforeGet = null,
            Action<object> afterReturn = null);

        UniTask RegisterPrefab(string prefabName, Action<object> beforeGet = null,
            Action<object> afterReturn = null);

        void RegisterCallbacks(string entityId, Action<object> beforeGet, Action<object> afterReturn);
        GameObject GetGameObject(string entityId);
        T GetObj<T>(string entityId) where T : new();
        void ReturnObj<T>(string entityId, T obj);
        void Clear(bool clearPrefabs = false);
        bool ContainPrefab(string entityId);

        #region 运行时查询

        int GetObjectCountInPool(string entityId);    //池内对象数量
        int GetObjectCountInUse(string entityId);     //使用中的对象数量
        int GetPreloadedObjectCount(string entityId); //获取预加载数量（低水位）
        int GetTotalObjectCount(string entityId);     //获取总数（池内和池外）
        int GetPoolCapacity(string entityId);         //获取池容量（高水位）
        bool IsPoolFull(string entityId);             //是否满池

        #endregion

        #region 统计信息

        int GetTotalCreatedCount(string entityId);  // 对象总创建次数
        int GetTotalReleasedCount(string entityId); // 对象总销毁次数
        int GetPeakUsageCount(string entityId);     // 历史峰值使用量

        #endregion
    }
}