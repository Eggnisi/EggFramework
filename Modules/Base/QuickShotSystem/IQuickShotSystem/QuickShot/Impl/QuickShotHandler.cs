#region

//文件创建者：Egg
//创建时间：09-08 12:23

#endregion

using System.Collections.Generic;
using QFramework;
using UnityEngine;

namespace EggFramework
{
    public sealed class QuickShotHandler
    {
        private Dictionary<string, object> _quickShotData = new();

        private readonly List<IQuickShotProperty> _quickShotProperties = new();

        public void RegisterQuickShotProperty<T>(string propName, QuickShotProperty<T> quickShotProperty)
        {
            if (!_quickShotProperties.Contains(quickShotProperty))
            {
                _quickShotProperties.Add(quickShotProperty);
            }

            var canQuickShot = (IQuickShotProperty)quickShotProperty;

            canQuickShot.Upload += () =>
            {
                //Save
                if (!_quickShotData.TryAdd(propName, quickShotProperty.Value))
                {
                    _quickShotData[propName] = quickShotProperty.Value;
                }
            };

            canQuickShot.Download += () =>
            {
                if (!_quickShotData.TryGetValue(propName, out var value))
                {
                    quickShotProperty.Value = default;
                }
                else
                {
                    quickShotProperty.Value = (T)value;
                }
            };
        }

        public void UnRegisterQuickShotProperty<T>(QuickShotProperty<T> quickShotProperty)
        {
            _quickShotProperties.Remove(quickShotProperty);
        }

        public void DownloadQuickShot(Dictionary<string, object> quickShotData)
        {
            _quickShotData = new Dictionary<string, object>(quickShotData);
            foreach (var prop in _quickShotProperties)
            {
                prop.Download?.Invoke();
            }
        }

        public Dictionary<string, object> UploadQuickShotData()
        {
            foreach (var prop in _quickShotProperties)
            {
                prop.Upload?.Invoke();
            }

            return new Dictionary<string, object>(_quickShotData);
        }
        
        public void BeforeUpload()
        {
            foreach (var quickShotProperty in _quickShotProperties)
            {
                quickShotProperty.BeforeUpload();
            }
        }

        public void AfterDownload()
        {
            foreach (var quickShotProperty in _quickShotProperties)
            {
                quickShotProperty.AfterDownload();
            }
        }
    }
}