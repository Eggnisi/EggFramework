using System;
using QFramework;
using UnityEngine;

namespace EggFramework.Storage
{
    public interface IStorage : IUtility
    {
#if UNITY_EDITOR
        void AssignEditorSavePath(string path);
#endif
        void AssignSavePath(string path = "fileInfo");
        void ReadFromDisk();
        void ReadFromText(string json);
        void WriteToDisk();
        T Load<T>(string key, T defaultValue, out bool newValue);
        void Save<T>(string key, T saveData);
        void Clear();
    }

    public static class StorageExtensions
    {
        public static T LoadFromSettingFile<T>(this IStorage self, string key, T defaultValue, string json)
        {
            self.ReadFromText(json);
            return self.Load(key, defaultValue, out _);
        }
#if UNITY_EDITOR
        public static void SaveToSettingFile<T>(this IStorage self, string key, T value, string path)
        {
            self.AssignEditorSavePath(path);
            self.ReadFromDisk();
            self.Save(key, value);
            self.WriteToDisk();
        }
#endif

        public static T LoadByJson<T>(this IStorage self, string key, T defaultValue, string fileName = "setting")
        {
            self.AssignSavePath(fileName);
            self.ReadFromDisk();
            return self.Load(key, defaultValue, out _);
        }

        public static void SaveByJson<T>(this IStorage self, string key, T value, string fileName = "setting")
        {
            self.AssignSavePath(fileName);
            self.ReadFromDisk();
            self.Save(key, value);
            self.WriteToDisk();
        }
    }
}