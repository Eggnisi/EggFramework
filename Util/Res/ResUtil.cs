#region

//文件创建者：Egg
//创建时间：10-15 08:29

#endregion


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using EggFramework.Util.Res;
using Sirenix.Utilities;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
#endif
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace EggFramework.Util
{
    public static class ResUtil
    {
        private static readonly Dictionary<string, Object> _resCacheDictionary = new();

        public static async UniTask<T> LoadAssetAsync<T>(string name) where T : Object
        {
            if (_resCacheDictionary.TryGetValue(name, out var ret))
                return (T)ret;
            _resCacheDictionary.Add(name, await Addressables.LoadAssetAsync<T>(name).ToUniTask());
            return (T)_resCacheDictionary[name];
        }

        public static TextAsset SettingConfig;

        public static async UniTask LoadSetting()
        {
            SettingConfig ??= await Addressables.LoadAssetAsync<TextAsset>(nameof(SettingConfig));
        }

#if UNITY_EDITOR

        private static void InnerRefreshResRefDatas(ResSetting setting)
        {
            var types = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
            foreach (var type in types)
            {
                InnerRefreshResRefData(setting, type);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RefreshResRef()
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ResSetting), new ResSetting());
            InnerRefreshResRefDatas(setting);
        }

        public static void RefreshResRefData(Type type)
        {
            var setting = StorageUtil.LoadFromSettingFile(nameof(ResSetting), new ResSetting());
            InnerRefreshResRefData(setting, type);
        }

        private static Object GetOrCreateResRefDataAsset(Type type, ResSetting setting)
        {
            var savePath = $"{setting.ResRefDataSavePath}/{type.Name}.asset";
            var data     = GetOrCreateAsset(savePath, type);
            AddAssetToGroup(setting.ResRefDataAddressableGroupName, savePath, true);
            return data;
        }

        public static T GetAssetWithName<T>(string name) where T : Object
        {
            var res = AssetDatabase.GetAllAssetPaths().Where(path => DirectoryUtil.ExtractName(path) == name).ToList();
            if (res.Count > 0)
            {
                return AssetDatabase.LoadAssetAtPath<T>(res[0]);
            }

            return null;
        }

        private static void InnerRefreshResRefData(ResSetting setting, Type type)
        {
            var data      = GetOrCreateResRefDataAsset(type, setting);
            var baseType  = type!.BaseType;
            var dataType  = baseType!.GenericTypeArguments[0];
            var refType   = baseType.GetNestedType("ResRef").MakeGenericType(dataType);
            var assets    = GetAssets(dataType);
            var fieldInfo = baseType.GetField("ResRefs");
            var list      = Activator.CreateInstance(fieldInfo!.FieldType);
            foreach (var asset in assets)
            {
                var refData = Activator.CreateInstance(refType);
                refType.GetField("Name").SetValue(refData, AssetDatabase.GetAssetPath((Object)asset));
                refType.GetField("Data").SetValue(refData, (Object)asset);
                (list as IList)?.Add(refData);
            }

            fieldInfo.SetValue(data, list);
            EditorUtility.SetDirty(data);
        }

        public static List<string> GetPrefabPaths()
        {
            return AssetDatabase.FindAssets("t:Prefab").Select(AssetDatabase.GUIDToAssetPath)
                .ToList();
        }

        public static List<string> GetPrefabPaths(List<string> folders)
        {
            return AssetDatabase.FindAssets("t:Prefab", folders.ToArray()).Select(AssetDatabase.GUIDToAssetPath)
                .ToList();
        }

        public static T GetAsset<T>() where T : Object
        {
            return (T)GetAsset(typeof(T));
        }

        public static T GetAsset<T>(string assetName) where T : Object
        {
            return (T)GetAsset(typeof(T), assetName);
        }

        public static Object GetAsset(Type type, string assetName)
        {
            var guids = AssetDatabase.FindAssets($"t:{type.FullName}");
            if (guids.Length <= 0) return null;
            foreach (var guid in guids)
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type);
                if (asset.name == assetName) return asset;
            }

            return null;
        }

        public static Object GetAsset(Type type)
        {
            var guids = AssetDatabase.FindAssets($"t:{type.FullName}");
            if (guids.Length <= 0) return null;
            return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), type);
        }

#endif
        public static void LoadByResourceAsync<T>(string path, Action<T> callBack) where T : Object
        {
            var request = Resources.LoadAsync<T>(path);
            request.completed += (res) => { callBack?.Invoke((T)request.asset); };
        }
#if UNITY_EDITOR

        public static T GetOrCreateAsset<T>(string path) where T : Object
        {
            return (T)GetOrCreateAsset(path, typeof(T));
        }

        public static Object GetOrCreateAsset(string path, Type type)
        {
            var data = GetAsset(type);
            DirectoryUtil.MakeSureDirectory(DirectoryUtil.ExtractFolder(path));
            if (data != null) return data;
            data = ScriptableObject.CreateInstance(type);
            AssetDatabase.CreateAsset(data, path);
            AssetDatabase.Refresh();
            return data;
        }

        public static IEnumerable<T> GetAssetsInFolder<T>(string folderPath) where T : Object
        {
            return AssetDatabase.GetAllAssetPaths().Where(path => path.Contains(folderPath))
                .Select(AssetDatabase.LoadAssetAtPath<T>).Where(t => t != null);
        }

        public static IEnumerable<T> GetAssets<T>() where T : Object
        {
            return AssetDatabase.GetAllAssetPaths()
                .Select(AssetDatabase.LoadAssetAtPath<T>).Where(t => t);
        }

        public static IEnumerable<T> GetAssets<T>(IEnumerable<string> folderPaths) where T : Object
        {
            return AssetDatabase.GetAllAssetPaths().Where(path => folderPaths.Any(path.Contains))
                .Select(AssetDatabase.LoadAssetAtPath<T>).Where(t => t != null);
        }

        public static IEnumerable GetAssets(Type type, IEnumerable<string> folderPaths)
        {
            return AssetDatabase.GetAllAssetPaths().Where(path => folderPaths.Any(path.Contains))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, type)).Where(t => t != null);
        }

        public static IEnumerable GetAssets(Type type)
        {
            return AssetDatabase.GetAllAssetPaths()
                .Select(path => AssetDatabase.LoadAssetAtPath(path, type)).Where(t => t != null);
        }

        public static List<string> GetCustomAssetPaths<T>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).FullName}").Select(AssetDatabase.GUIDToAssetPath)
                .ToList();
        }

        public static IEnumerable GetCustomAssets(Type type)
        {
            return AssetDatabase.FindAssets($"t:{type.FullName}").Select(AssetDatabase.GUIDToAssetPath)
                .Select(path => AssetDatabase.LoadAssetAtPath(path, type))
                .ToList();
        }

        public static List<T> GetCustomAssets<T>() where T : ScriptableObject
        {
            return AssetDatabase.FindAssets($"t:{typeof(T).FullName}").Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .ToList();
        }

        public static List<string> GetAssetPaths<T>()
        {
            return AssetDatabase.GetAllAssetPaths()
                .Where(path => AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(T)).ToList();
        }

        public static List<string> GetAssetPaths<T>(IEnumerable<string> folderPaths)
        {
            return AssetDatabase.GetAllAssetPaths().Where(path => folderPaths.Any(path.Contains))
                .Where(path => AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(T)).ToList();
        }

        public static List<string> GetAssetFolderPaths<T>()
        {
            return AssetDatabase.GetAllAssetPaths()
                .Where(path => AssetDatabase.GetMainAssetTypeAtPath(path) == typeof(T))
                .Select(path => path[..path.LastIndexOf("/", StringComparison.Ordinal)]).ToList();
        }

        private static AddressableAssetSettings _cachedSetting;

        private static AddressableAssetSettings GetSetting()
        {
            if (_cachedSetting) return _cachedSetting;
            _cachedSetting = AddressableAssetSettingsDefaultObject.Settings;
            if (!_cachedSetting)
            {
                var path = "Assets/AddressableAssetsData";
                DirectoryUtil.MakeSureDirectory(path);
                _cachedSetting =
                    AddressableAssetSettings.Create(path, "Default", true, true);
            }

            return _cachedSetting;
        }

        private static void SaveSetting()
        {
            // 保存设置
            EditorUtility.SetDirty(_cachedSetting);
            AssetDatabase.SaveAssets();
            AddressableAssetSettingsDefaultObject.Settings = _cachedSetting;
        }

        public static void AddAssetToGroup(string groupName, string path, bool simplifyName = false,
            bool isBuildGroup = false,
            IEnumerable<string> labels = null)
        {
            var settings = GetSetting();

            var group = settings.FindGroup(groupName);
            if (!group)
            {
                group = settings.CreateGroup(groupName, false, false, true, null);
                if (isBuildGroup)
                {
                    group.AddSchema<ContentUpdateGroupSchema>();
                    group.AddSchema<BundledAssetGroupSchema>();
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupSchemaModified, group, true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            if (File.Exists(path))
            {
                var entry =
                    settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
                if (entry == null)
                {
                    Debug.LogError("Failed to add asset to Addressable Group.");
                }
                else if (simplifyName)
                {
                    entry.address = DirectoryUtil.ExtractName(path);
                    if (labels != null)
                        entry.labels.AddRange(labels);
                }
            }
            else
            {
                Debug.LogError("Asset not found at path: " + path);
            }

            SaveSetting();
        }

        public static void ClearAssetGroup(string groupName)
        {
            var settings = GetSetting();

            var group = settings.FindGroup(groupName);
            if (group != null)
            {
                var list = group.entries.ToList();
                foreach (var addressableAssetEntry in list)
                {
                    group.RemoveAssetEntry(addressableAssetEntry);
                }
            }

            SaveSetting();
        }

        public static void RemoveAssetFromGroup(string groupName, string path)
        {
            var settings = GetSetting();
            var group    = settings.FindGroup(groupName);
            if (group != null)
            {
                var entry = group.GetAssetEntry(AssetDatabase.AssetPathToGUID(path));
                if (entry != null)
                    group.RemoveAssetEntry(entry);
            }

            SaveSetting();
        }

        public static void RemoveAssetsFromGroup(string groupName, IEnumerable<string> paths)
        {
            var settings = GetSetting();
            var group    = settings.FindGroup(groupName);
            if (group != null)
            {
                foreach (var path in paths)
                {
                    var entry = group.GetAssetEntry(AssetDatabase.AssetPathToGUID(path));
                    if (entry != null)
                        group.RemoveAssetEntry(entry);
                }
            }

            SaveSetting();
        }

        public static void AddAssetsToGroup(string groupName, IEnumerable<string> paths, bool simplifyName = false,
            bool isBuildGroup = false,
            IEnumerable<string> labels = null)
        {
            var settings = GetSetting();
            var group    = settings.FindGroup(groupName);
            if (group == null)
            {
                group = settings.CreateGroup(groupName, false, false, true, null);
                if (isBuildGroup)
                {
                    group.AddSchema<ContentUpdateGroupSchema>();
                    group.AddSchema<BundledAssetGroupSchema>();
                    settings.SetDirty(AddressableAssetSettings.ModificationEvent.GroupSchemaModified, group, true);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            foreach (var path in paths)
            {
                if (File.Exists(path))
                {
                    var entry =
                        settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(path), group, false, false);
                    if (entry == null)
                    {
                        Debug.LogError("Failed to add asset to Addressable Group.");
                    }
                    else if (simplifyName)
                    {
                        entry.SetAddress(DirectoryUtil.ExtractName(path));
                        if (labels != null)
                            entry.labels.AddRange(labels);
                    }
                }
                else
                {
                    Debug.LogError("Asset not found at path: " + path);
                }
            }

            SaveSetting();
        }
#endif
    }
}