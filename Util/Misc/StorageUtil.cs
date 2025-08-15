#region

//文件创建者：Egg
//创建时间：10-23 11:28

#endregion

using EggFramework.Storage;
using Newtonsoft.Json;

#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

namespace EggFramework.Util
{
    public static class StorageUtil
    {
#if UNITY_EDITOR

        private static TextAsset _settingConfigInst;

        private static TextAsset _settingConfig
        {
            get
            {
                if (_settingConfigInst) return _settingConfigInst;
                _settingConfigInst = GetOrCreateSettingFile();
                return _settingConfigInst;
            }
        }

        private static TextAsset GetOrCreateSettingFile()
        {
            var path = "Assets/Settings/EggFramework/SettingConfig.json";
            DirectoryUtil.MakeSureDirectory("Assets/Settings/EggFramework");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, "{}");
            }

            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
            ResUtil.AddAssetToGroup("SettingConfig", path, true, true);
            return AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        }

#endif
        private static JsonStorage _jsonStorage;
        private static IStorage    _storage => _jsonStorage ??= new JsonStorage();

        public static T LoadByJson<T>(string key, T defaultValue, string fileName)
        {
            return _storage.LoadByJson(key, defaultValue, fileName);
        }

        public static T LoadByJson<T>(string key, T defaultValue)
        {
            return _storage.LoadByJson(key, defaultValue);
        }
#if UNITY_EDITOR

        public static void SaveToSettingFile<T>(string key, T value)
        {
            var path = AssetDatabase.GetAssetPath(_settingConfig);
            _storage.SaveToSettingFile(key, value, path.Replace(".json", ""));
            EditorUtility.SetDirty(_settingConfig);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif
        public static T LoadFromSettingFile<T>(string key, T defaultValue)
        {
#if UNITY_EDITOR
            var json = _settingConfig.text;
#else
            var json = ResUtil.SettingConfig.text;
#endif
            return _storage.LoadFromSettingFile(key, defaultValue, json);
        }

        public static void SaveByJson<T>(string key, T value, string fileName)
        {
            _storage.SaveByJson(key, value, fileName);
        }

        public static void SaveByJson<T>(string key, T value)
        {
            _storage.SaveByJson(key, value);
        }

        public static T LoadByPlayerPref<T>(string key, T defaultValue)
        {
            var json = PlayerPrefs.GetString(key, JsonConvert.SerializeObject(defaultValue));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void SaveByPlayerPref<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            PlayerPrefs.SetString(key, json);
        }
#if UNITY_EDITOR
        public static T LoadByEditorPref<T>(string key, T defaultValue)
        {
            var json = EditorPrefs.GetString(key, JsonConvert.SerializeObject(defaultValue));
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static void SaveByEditorPref<T>(string key, T value)
        {
            var json = JsonConvert.SerializeObject(value);
            EditorPrefs.SetString(key, json);
        }
#endif
    }
}