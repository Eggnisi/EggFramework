#region

//文件创建者：Egg
//创建时间：04-21 06:57

#endregion

using System;
using System.Collections.Generic;

namespace EggFramework.ObjectPool
{
    public sealed partial class ObjectPoolSystem
    {
        private readonly Dictionary<string, int> _objectPreload  = new();
        private readonly Dictionary<string, int> _objectCapacity = new();

        private readonly Dictionary<string, int> _objectCreate  = new();
        private readonly Dictionary<string, int> _objectRelease = new();
        private readonly Dictionary<string, int> _objectPeakUse = new();

        public int GetObjectCountInPool(string entityId)
        {
            return _objectInPool.GetValueOrDefault(entityId, new List<object>()).Count;
        }

        public int GetObjectCountInUse(string entityId)
        {
            return _objectInUse.GetValueOrDefault(entityId, new List<object>()).Count;
        }

        public int GetPreloadedObjectCount(string entityId)
        {
            return _objectPreload.GetValueOrDefault(entityId);
        }

        public int GetTotalObjectCount(string entityId)
        {
            return GetObjectCountInPool(entityId) + GetObjectCountInUse(entityId);
        }

        public int GetPoolCapacity(string entityId)
        {
            return _objectCapacity.GetValueOrDefault(entityId);
        }

        public bool IsPoolFull(string entityId)
        {
            return GetObjectCountInUse(entityId) == GetTotalObjectCount(entityId);
        }

        public int GetTotalCreatedCount(string entityId)
        {
            return _objectCreate.GetValueOrDefault(entityId);
        }

        public int GetTotalReleasedCount(string entityId)
        {
            return _objectRelease.GetValueOrDefault(entityId);
        }

        public int GetPeakUsageCount(string entityId)
        {
            return _objectPeakUse.GetValueOrDefault(entityId);
        }
    }
}