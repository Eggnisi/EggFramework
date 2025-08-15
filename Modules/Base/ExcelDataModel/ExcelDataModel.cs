#region

//文件创建者：Egg
//创建时间：10-23 01:47

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EggFramework.Util;
using EggFramework.Util.Excel;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EggFramework
{
    public sealed class ExcelDataModel : AbstractModel, IExcelDataModel
    {
        private          ExcelRefData                                         _data;
        private readonly Dictionary<string, List<IExcelEntity>>               _cacheEntity          = new();
        private readonly Dictionary<string, Dictionary<string, IExcelEntity>> _cacheEntityDicString = new();
        private readonly Dictionary<string, Dictionary<int, IExcelEntity>>    _cacheEntityDicInt    = new();

        protected override void OnInit()
        {
        }
        
        public T GetEntityDataById<T>(string id) where T : IExcelEntity
        {
            var table = InnerGetEntityDataById<T>(id);
            Debug.Assert(table != null, $"非法的{typeof(T).Name}Id:{id}");
            return table;
        }
        
        public T GetEntityDataById<T>(int id) where T : IExcelEntity
        {
            var table = InnerGetEntityDataById<T>(id);
            Debug.Assert(table != null, $"非法的{typeof(T).Name}Id:{id}");
            return table;
        }

        private T InnerGetEntityDataById<T>(string id) where T : IExcelEntity
        {
            var type = typeof(T);
            if (_cacheEntityDicString.TryGetValue(type.Name, out var dic))
            {
                return (T)dic.GetValueOrDefault(id, null);
            }

            return default;
        }

        private T InnerGetEntityDataById<T>(int id) where T : IExcelEntity
        {
            var type = typeof(T);
            if (_cacheEntityDicInt.TryGetValue(type.Name, out var dic))
            {
                return (T)dic.GetValueOrDefault(id, null);
            }

            return default;
        }

        public void AssertEntityExist<T>(string id) where T : IExcelEntity
        {
            var table = InnerGetEntityDataById<T>(id);
            Debug.Assert(table != null, $"非法的{typeof(T).Name}Id:{id}");
        }

        public bool Inited { get; private set; }

        public List<T> GetEntityData<T>() where T : IExcelEntity
        {
            var type = typeof(T);
            if (_cacheEntity.TryGetValue(type.Name, out var list))
            {
                return list.Select(ele => (T)ele).ToList();
            }

            var dataRef = _data.ExcelDataRefs.Find(excelDataRef => excelDataRef.Name == type.Name);
            if (string.IsNullOrEmpty(dataRef.Name)) return null;
            return null;
        }

        private Action _tempAction;

        public void TriggerOnFinishInitOrLater(Action action)
        {
            if (Inited) action?.Invoke();
            else _tempAction += action;
        }

        public override async UniTask AsyncInit()
        {
            _data = await Addressables.LoadAssetAsync<ExcelRefData>(nameof(ExcelRefData)).ToUniTask();
            var entitySOTypes = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            var taskList      = new List<UniTask>();
            foreach (var entitySoType in entitySOTypes)
            {
                taskList.Add(CacheData(entitySoType));
            }

            await UniTask.WhenAll(taskList);
            Inited = true;
            _tempAction?.Invoke();
        }

        private async UniTask CacheData(Type entitySoType)
        {
            var  data    = await Addressables.LoadAssetAsync<ScriptableObject>(entitySoType.Name).ToUniTask();
            var  list    = (IEnumerable)data.GetType().GetField("RawDataList").GetValue(data);
            var  newList = new List<IExcelEntity>();
            Type eleType = null;
            var  name    = entitySoType.Name.Replace("SO", "");
            _cacheEntityDicInt.Add(name, new Dictionary<int, IExcelEntity>());
            _cacheEntityDicString.Add(name, new Dictionary<string, IExcelEntity>());
            foreach (var o in list)
            {
                eleType ??= o.GetType();
                var cloneMethod = eleType.GetMethod("Clone");
                var clone       = (IExcelEntity)cloneMethod?.Invoke(o, new object[] { });
                var idField     = eleType.GetField("Id");
                if (idField != null)
                {
                    if (idField.FieldType == typeof(int))
                    {
                        _cacheEntityDicInt[name].TryAdd((int)idField.GetValue(o), clone);
                    }
                    else if (idField.FieldType == typeof(string))
                    {
                        _cacheEntityDicString[name].TryAdd((string)idField.GetValue(o), clone);
                    }
                    else
                    {
                        Debug.LogError($"表：{name}，非法主键类型，Id字段只能为int或者string");
                    }
                }
                else Debug.Log($"表：{name}未指定主键");

                newList.Add(clone);
            }

            _cacheEntity.Add(entitySoType.Name.Replace("SO", ""), newList);
        }
    }
}