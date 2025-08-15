#region

//文件创建者：Egg
//创建时间：09-08 10:20

#endregion

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using EggFramework.MonoUtil;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace EggFramework.ObjectPool
{
    public sealed partial class ObjectPoolSystem : AbstractModule, IObjectPoolSystem
    {
        //如果是GameObject,需要统一挂载到PoolRoot下
        private readonly Dictionary<string, GameObject> _objectPoolRoots = new();

        private readonly Dictionary<string, List<object>> _objectInPool = new();
        private readonly Dictionary<string, List<object>> _objectInUse  = new();

        //如果是GameObject,需要使用Prefab进行克隆
        private readonly Dictionary<string, GameObject>     _objectPrefabs     = new();
        private readonly Dictionary<string, Action<object>> _beforeGetAction   = new();
        private readonly Dictionary<string, Action<object>> _afterReturnAction = new();

        private readonly Dictionary<string, float>                     _objectPoolTimer = new();
        private readonly Dictionary<string, Dictionary<object, float>> _objectTimer     = new();

        private          ObjectPoolData                       _objectPoolData;
        private          MonoUpdater                          _updater;
        private readonly Dictionary<string, ObjectPoolConfig> _configs = new();

        protected override void OnInit()
        {
            MakeSureRoot();
            _updater = Root.AddComponent<MonoUpdater>();
            _updater.RegisterOnUpdate(OnUpdate);
        }

        private void OnUpdate()
        {
            foreach (var key in _objectPoolTimer.Keys)
            {
                if (_objectPoolTimer[key] >= 0)
                {
                    _objectPoolTimer[key] -= Time.unscaledDeltaTime;
                    //如果正在使用的对象数量少于低水位并且长期不新使用对象
                    if (_objectPoolTimer[key] < 0 && GetObjectCountInUse(key) < GetPreloadedObjectCount(key))
                    {
                        Debug.Log($"对象池{key}长期不使用，尝试释放至低水位");
                        if (_objectPrefabs.ContainsKey(key))
                        {
                            TryReleaseObjectPool(key, obj => { Object.Destroy((GameObject)obj); });
                        }
                        else
                        {
                            TryReleaseObjectPool(key, null);
                        }
                    }
                    else if (_objectPoolTimer[key] < 0) _objectPoolTimer[key] = 0.01f;
                }
            }

            foreach (var key in _objectTimer.Keys)
            {
                var dic = _objectTimer[key];
                foreach (var dicKey in dic.Keys)
                {
                    if (!(dic[dicKey] >= 0)) continue;
                    dic[dicKey] -= Time.unscaledDeltaTime;
                    //池中对象数量大于零，并且释放后不低于低水位
                    if (dic[dicKey] < 0 && GetObjectCountInPool(key) > 0 &&
                        GetPreloadedObjectCount(key) <= GetTotalObjectCount(key))
                    {
                        if (_objectPrefabs.ContainsKey(key))
                        {
                            TryReleaseObject(key, obj => { Object.Destroy((GameObject)obj); }, dicKey);
                        }
                        else
                        {
                            TryReleaseObject(key, null, dicKey);
                        }
                    }
                    //不满足条件，下次依然计时
                    else if (dic[dicKey] < 0) dic[dicKey] = 0.01f;
                }
            }
        }

        protected override async UniTask OnAsyncInit()
        {
            _objectPoolData = await Addressables.LoadAssetAsync<ObjectPoolData>(nameof(ObjectPoolData)).ToUniTask();
            foreach (var objectPoolConfig in _objectPoolData.PoolConfigs)
            {
                _configs[objectPoolConfig.EntityId] = objectPoolConfig;
            }
        }

        private void MakeSureRoot()
        {
            if (Root is not null) return;
            Root = new GameObject("ObjectPoolRoot");
            Object.DontDestroyOnLoad(Root);
        }

        public GameObject Root { get; private set; }

        public GameObject GetPoolRoot(string entityId)
        {
            return _objectPoolRoots.GetValueOrDefault(entityId, null);
        }

        //如果是预制体，需要先注册并且加载才能使用
        public async UniTask RegisterPrefabs(IEnumerable<string> prefabNames, Action<object> beforeGet = null,
            Action<object> afterReturn = null)
        {
            if (!_objectPoolData)
            {
                Debug.LogWarning("对象池配置数据没有初始化，部分功能无法使用");
            }

            await UniTask.WhenAll(prefabNames.Select(prefabName => RegisterPrefab(prefabName, beforeGet, afterReturn)));
        }

        public async UniTask RegisterPrefab(string prefabName, Action<object> beforeGet = null,
            Action<object> afterReturn = null)
        {
            var prefab = await Addressables.LoadAssetAsync<GameObject>(prefabName).ToUniTask();
            if (prefab == null)
            {
                Debug.LogError("资产不存在");
                return;
            }

            if (!_objectPrefabs.TryAdd(prefabName, prefab))
            {
                Debug.LogError("资产名称冲突");
            }
            else
            {
                CreatePoolRoot(prefabName);
            }

            //添加默认初始化方法和清理方法
            RegisterCallbacks(prefabName, beforeGet ?? (o => ((GameObject)o).SetActive(true)),
                afterReturn ?? ((o) =>
                {
                    var obj = (GameObject)o;
                    obj.SetActive(false);
                    obj.transform.SetParent(_objectPoolRoots[prefabName].transform);
                }));

            var config = GetObjectPoolConfig(prefabName);
            //预加载
            BatchExpand(prefabName,
                () => Object.Instantiate(_objectPrefabs[prefabName], _objectPoolRoots[prefabName].transform),
                config.PreloadCount);
        }

        private ObjectPoolConfig GetObjectPoolConfig(string prefabName)
        {
            if (_objectPoolData)
            {
                return _configs.TryGetValue(prefabName, out var config) ? config : _objectPoolData.DefaultPoolConfig;
            }

            return null;
        }

        private void CreatePoolRoot(string prefabName)
        {
            var root = new GameObject($"{prefabName}Pool");
            MakeSureRoot();
            root.transform.SetParent(Root.transform);
            _objectPoolRoots.Add(prefabName, root);
        }

        public void RegisterCallbacks(string entityId, Action<object> beforeGet, Action<object> afterReturn)
        {
            if (beforeGet != null)
            {
                if (!_beforeGetAction.TryAdd(entityId, beforeGet))
                {
                    _beforeGetAction[entityId] += beforeGet;
                }
            }

            if (afterReturn != null)
            {
                if (!_afterReturnAction.TryAdd(entityId, afterReturn))
                {
                    _afterReturnAction[entityId] += afterReturn;
                }
            }
        }

        public GameObject GetGameObject(string entityId)
        {
            var pool = GetPool<GameObject>(entityId);

            GameObject ret;
            if (pool.Count <= 0)
            {
                //尝试请求扩容
                ret = (GameObject)TryExpandObjectPool(entityId,
                    () => Object.Instantiate(_objectPrefabs[entityId], _objectPoolRoots[entityId].transform));
                if (!ret) return null;
            }
            else
            {
                ret = (GameObject)pool[0];
                //尝试请求扩容
                TryExpandObjectPool(entityId,
                    () => Object.Instantiate(_objectPrefabs[entityId], _objectPoolRoots[entityId].transform));
            }

            ApplyStatistics(entityId, ret);
            return ret;
        }

        private void ApplyStatistics(string entityId, object obj)
        {
            if (_objectInPool.TryGetValue(entityId, out var list))
            {
                list.Remove(obj);
            }

            if (_objectInUse.TryGetValue(entityId, out var useList))
            {
                useList.Add(obj);
            }
            else _objectInUse[entityId] = new List<object> { obj };

            _objectPeakUse[entityId] = Mathf.Max(_objectInUse[entityId].Count, _objectPeakUse[entityId]);
            var config = GetObjectPoolConfig(entityId);
            //使用对象池重置计时器
            if (config.ReleaseMode == EObjectPoolReleaseMode.UsePoolTimeout)
            {
                _objectPoolTimer[entityId] = config.TimeOut;
            }

            //使用对象计时器，提供对象之前清空计时器
            if (config.ReleaseMode == EObjectPoolReleaseMode.UseObjectTimeout)
            {
                if (_objectTimer.TryGetValue(entityId, out var dic))
                {
                    dic[obj] = -1;
                }
                else _objectTimer[entityId] = new Dictionary<object, float> { { obj, -1 } };
            }

            _beforeGetAction[entityId]?.Invoke(obj);
        }

        private void BatchExpand(string entityId, Func<object> createFunc, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var obj = createFunc.Invoke();
                if (_objectInPool.TryGetValue(entityId, out var list))
                {
                    list.Add(obj);
                }
                else _objectInPool[entityId] = new List<object> { obj };
            }

            _objectCreate[entityId] = GetTotalCreatedCount(entityId) + count;
        }

        private void TryReleaseObject(string entityId, Action<object> releaseFunc, object obj)
        {
            if (_objectInPool.TryGetValue(entityId, out var list))
            {
                list.Remove(obj);
                releaseFunc?.Invoke(obj);
                _objectRelease[entityId] = GetTotalReleasedCount(entityId) + 1;
            }
            else
            {
                throw new Exception($"对象池还未创建，无法释放，对象Id{entityId}");
            }
        }

        private void TryReleaseObjectPool(string entityId, Action<object> releaseFunc)
        {
            var lowCount     = GetPreloadedObjectCount(entityId);
            var releaseCount = GetTotalObjectCount(entityId) - lowCount;
            if (releaseCount > GetObjectCountInPool(entityId))
            {
                throw new Exception($"池中没有这么多数量的对象用于释放，对象ID{entityId}");
            }

            BatchRelease(entityId, releaseFunc, releaseCount);
        }

        private void BatchRelease(string entityId, Action<object> releaseFunc, int count)
        {
            if (_objectInPool.TryGetValue(entityId, out var list))
            {
                for (var i = 0; i < count; i++)
                {
                    var obj = list[0];
                    list.RemoveAt(0);
                    releaseFunc?.Invoke(obj);
                }

                _objectRelease[entityId] = GetTotalReleasedCount(entityId) + count;
            }
            else
            {
                throw new Exception($"对象池还未创建，无法释放，对象Id{entityId}");
            }
        }

        private object TryExpandObjectPool(string entityId, Func<object> createFunc)
        {
            var config = GetObjectPoolConfig(entityId);

            switch (config.ExpandMode)
            {
                case EObjectPoolExpandMode.AutoExpand:
                    //如果对象总数小于高水位正常扩容
                    if (GetTotalObjectCount(entityId) < GetPoolCapacity(entityId))
                    {
                        var ret = createFunc.Invoke();
                        _objectCreate[entityId] = GetTotalCreatedCount(entityId) + 1;
                        if (_objectInPool.TryGetValue(entityId, out var list))
                        {
                            list.Add(ret);
                        }
                        else _objectInPool[entityId] = new List<object> { ret };

                        return ret;
                    }

                    if (!config.ForceRetrieve) return null;
                    //强制回收
                {
                    var obj = ForceRetrieve(entityId);
                    if (_objectInPool.TryGetValue(entityId, out var list))
                    {
                        list.Add(obj);
                    }
                    else _objectInPool[entityId] = new List<object> { obj };

                    return obj;
                }

                //到达上限，直接退出
                case EObjectPoolExpandMode.PredictiveExpand:
                {
                    //如果池内可用对象小于使用中对象的20%
                    if (GetObjectCountInPool(entityId) < GetObjectCountInUse(entityId) * 0.2f)
                    {
                        var createCount =
                            Mathf.Min(
                                Mathf.CeilToInt(GetObjectCountInUse(entityId) * 0.2f) - GetObjectCountInPool(entityId),
                                GetPoolCapacity(entityId) - GetTotalObjectCount(entityId));
                        BatchExpand(entityId, createFunc, createCount);
                    }
                }
                    break;
                case EObjectPoolExpandMode.DontExpand:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        private object ForceRetrieve(string entityId)
        {
            if (!_objectInUse.TryGetValue(entityId, out var list)) throw new Exception("没有对象可以用于强制回收");
            if (list.Count <= 0) throw new Exception("没有对象可以用于强制回收");
            var ret = list[0];
            ReturnObj(entityId, ret);
            return ret;
        }

        public T GetObj<T>(string entityId) where T : new()
        {
            var pool = GetPool<T>(entityId);
            T   ret;
            if (pool.Count <= 0)
            {
                ret = (T)TryExpandObjectPool(entityId,
                    () => new T());
                if (ret == null) return default;
            }
            else
            {
                ret = (T)pool[0];
            }

            ApplyStatistics(entityId, ret);
            return ret;
        }

        private List<object> GetPool<T>(string entityId) where T : new()
        {
            if (typeof(T) == typeof(GameObject))
            {
                if (!_objectPrefabs.TryGetValue(entityId, out _))
                {
                    throw new Exception($"没有注册Prefab{entityId}");
                }

                //生成GameObject挂载的PoolRoot
                if (!_objectPoolRoots.TryGetValue(entityId, out _))
                {
                    CreatePoolRoot(entityId);
                }
            }

            if (_objectInPool.TryGetValue(entityId, out var list))
            {
                return list;
            }

            list                    = new List<object>();
            _objectInPool[entityId] = list;
            return list;
        }

        public void ReturnObj<T>(string entityId, T obj)
        {
            if (obj == null) return;
            if (!_objectInUse.TryGetValue(entityId, out var pool))
            {
                Debug.Log($"该物体不受对象池管理{typeof(T).Name}");
                return;
            }

            if (pool.Remove(obj))
            {
                if (_objectInPool.TryGetValue(entityId, out var list))
                {
                    list.Add(obj);
                }
                else _objectInPool[entityId] = new List<object> { obj };

                if (_afterReturnAction.TryGetValue(entityId, out var action))
                {
                    action?.Invoke(obj);
                }

                //如果使用对象计时器
                var config = GetObjectPoolConfig(entityId);
                if (config.ReleaseMode == EObjectPoolReleaseMode.UseObjectTimeout)
                {
                    if (_objectTimer.TryGetValue(entityId, out var dic))
                    {
                        dic[obj] = config.TimeOut;
                    }
                    else _objectTimer[entityId] = new Dictionary<object, float> { { obj, config.TimeOut } };
                }

                if (config.ReleaseMode == EObjectPoolReleaseMode.PredictiveRelease)
                {
                    PredictiveRelease(entityId);
                }
            }
            else
            {
                Debug.LogError($"该对象{entityId}不在对象池中");
            }
        }

        private void PredictiveRelease(string entityId)
        {
            if (GetObjectCountInPool(entityId) > GetObjectCountInUse(entityId) * 0.3f)
            {
                var releaseCount = Mathf.Min(GetTotalObjectCount(entityId) - GetPreloadedObjectCount(entityId),
                    Mathf.CeilToInt(GetObjectCountInPool(entityId) - GetObjectCountInUse(entityId) * 0.3f));
                if (_objectPrefabs.ContainsKey(entityId))
                {
                    BatchRelease(entityId, obj => { Object.Destroy((GameObject)obj); }, releaseCount);
                }
                else BatchRelease(entityId, null, releaseCount);
            }
        }

        public void Clear(bool clearPrefabs = false)
        {
            for (int i = Root.transform.childCount - 1; i >= 0; i--)
            {
                Object.DestroyImmediate(Root.transform.GetChild(i).gameObject);
            }

            //TODO:
            _objectInPool.Clear();
            _objectInUse.Clear();
            _objectPoolRoots.Clear();

            if (clearPrefabs)
            {
                _beforeGetAction.Clear();
                _afterReturnAction.Clear();
                _objectPrefabs.Clear();
            }
        }

        public bool ContainPrefab(string entityId)
        {
            return _objectPrefabs.ContainsKey(entityId);
        }
    }
}