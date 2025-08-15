#region

//文件创建者：Egg
//创建时间：11-10 01:03

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EggFramework.Util.Excel;
using EggFramework.Util.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EggFramework.Util
{
    public static class LocalizationUtil
    {
        private static          bool                                 _inited;
        private static          SystemLanguage                       _currentSystemLanguage;
        private static          LocalizationConfig                   _config;
        private static readonly Dictionary<string, LocalizationData> _localizationDatas = new();

        public static async UniTask Init()
        {
            if (_inited) return;
            _currentSystemLanguage = StorageUtil.LoadByJson("GameLanguage", SystemLanguage.ChineseSimplified);
            _config = StorageUtil.LoadFromSettingFile(nameof(LocalizationConfig), new LocalizationConfig());
            var tasks = Enumerable.Select(_config.LocalizationTables, LoadTable).ToList();
            await UniTask.WhenAll(tasks);
            _inited = true;
        }

        public static T Localize<T>(T entity) where T : IExcelEntity
        {
            var tableName = entity.GetType().Name;
            var fields    = entity.GetType().GetSerializeFieldInfos().ToList();
            if (_localizationDatas.TryGetValue(tableName, out var data))
            {
                var package = data.Packages.Find(pk => pk.Language == _currentSystemLanguage);
                foreach (var col in package.LocalizationCols)
                {
                    foreach (var fieldInfo in fields)
                    {
                        if (fieldInfo.Name == col.ColName)
                        {
                            var entryItem = col.LocalizationItems.Find(item =>
                                item.EntryKey == tableName + "_" + col.ColName + "_" + fieldInfo.GetValue(entity));
                            if (entryItem == null)
                            {
                                Debug.LogError("没有找到对应的翻译文本，使用默认文本，或重新生成翻译");
                                return entity;
                            }

                            fieldInfo.SetValue(entity, entryItem.Value);
                        }
                    }
                }

                return entity;
            }

            return entity;
        }

        public static void ChangeLanguage(SystemLanguage language)
        {
            _currentSystemLanguage = language;
            StorageUtil.SaveByJson("GameLanguage", language);
        }

        private static async UniTask LoadTable(string tableName)
        {
            _localizationDatas.TryAdd(tableName,
                await Addressables.LoadAssetAsync<LocalizationData>(tableName + "_LocalizationData"));
        }

        public static string GetLocalizationString(string key)
        {
            var firstIndex  = key.IndexOf("_", StringComparison.Ordinal);
            var secondIndex = key.IndexOf("_", firstIndex + 1, StringComparison.Ordinal);
            var table       = key[..firstIndex];
            var colName     = key[(firstIndex + 1)..secondIndex];

            if (_localizationDatas.TryGetValue(table, out var data))
            {
                var targetPackage = data.Packages.Find(pa => pa.Language == _currentSystemLanguage);
                if (targetPackage == null)
                {
                    Debug.LogError($"不支持该语言{_currentSystemLanguage}");
                    return null;
                }

                var col = targetPackage.LocalizationCols.Find(col => col.ColName == colName);
                if (col == null)
                {
                    Debug.LogError($"没有找到对应字段{colName}");
                    return null;
                }

                return col.LocalizationItems.Find(item => item.EntryKey == key).Value;
            }

            Debug.LogError($"该表{table}没有创建本地化数据");

            return null;
        }
    }
}