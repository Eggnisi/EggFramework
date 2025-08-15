using System;
using System.Collections;
using QFramework;
using System.Collections.Generic;
using EggFramework.Storage;
using UnityEngine;

namespace EggFramework
{
    public sealed class FileSystem : AbstractModule, IFileSystem
    {
        public string                CurrentFile => _currentFile;
        public IReadOnlyList<string> FileNames   => _fileNames;

        private List<string> _fileNames { get; set; } = new();

        private bool _inited;

        private bool _loaded;

        private          IStorage                   _storage;
        private readonly List<ISavable>             _savableProperties = new();
        private readonly Dictionary<string, object> _defaultValues     = new();
        private          string                     _currentFile       = string.Empty;

        public void DeleteFile(string fileName)
        {
            MakeSureInited();
            _storage.AssignSavePath(fileName);
            _storage.Clear();
            _storage.WriteToDisk();
            _fileNames.Remove(fileName);
        }

        public void DeleteAll()
        {
            MakeSureInited();
            foreach (var fileName in _fileNames)
            {
                DeleteFile(fileName);
            }
        }

        private Action<string> _onFileLoaded;
        private Action<string> _beforeFileSaved;

        public IUnRegister RegisterOnFileLoaded(Action<string> action)
        {
            _onFileLoaded += action;
            return new CustomUnRegister(() => _onFileLoaded -= action);
        }

        public IUnRegister RegisterBeforeFileSaved(Action<string> action)
        {
            _beforeFileSaved += action;
            return new CustomUnRegister(() => _beforeFileSaved -= action);
        }

        private void MakeSureInited()
        {
            if (string.IsNullOrEmpty(_currentFile))
            {
                if (_fileNames == null) OnInit();
                
                if (_fileNames!.Count == 0) _fileNames.Add("file1");
                _currentFile = _fileNames[0];
                _storage.Save("FileNames", _fileNames);
                _storage.WriteToDisk();
            }
        }

        private T Load<T>(string key, T defaultValue, out bool isNew)
        {
            MakeSureInited();
            return _storage.Load(key, defaultValue, out isNew);
        }

        private T LoadFileInner<T>(T defaultValue) where T : IFileInfoPayload
        {
            MakeSureInited();
            var fileInfo = SaveFileInfo(_currentFile, defaultValue);
            this.SendEvent(new LoadFileEvent<T>
            {
                FileInfo = fileInfo
            });
            LoadFileInner();
            return fileInfo.Payload;
        }

        private void LoadFileInner()
        {
            MakeSureInited();
            if (!_fileNames.Contains(_currentFile)) CreateFile(_currentFile);
            else SaveFileName();
            
            _storage.AssignSavePath(_currentFile);
            _storage.ReadFromDisk();
            foreach (var savable in _savableProperties)
            {
                savable.OnLoad?.Invoke();
            }

            this.SendEvent(new LoadFileEvent());
            _onFileLoaded?.Invoke(_currentFile);
            _loaded = true;
        }

        public T LoadFile<T>(string fileName = "file1", T defaultValue = default) where T : IFileInfoPayload
        {
            _currentFile = fileName;

            return LoadFileInner(defaultValue);
        }

        public void LoadFile(string fileName = "file1")
        {
            _currentFile = fileName;

            LoadFileInner();
        }

        private void SaveFileName()
        {
            MakeSureInited();
            _storage.AssignSavePath();
            _storage.ReadFromDisk();
            _storage.Save("FileNames", _fileNames);
            _storage.WriteToDisk();
        }

        private FileInfo<T> SaveFileInfo<T>(string fileName, T payload, bool replacePayload = false)
            where T : IFileInfoPayload
        {
            MakeSureInited();

            if (!_fileNames.Contains(fileName))
            {
                _fileNames.Add(fileName);
                SaveFileName();
            }
            
            _storage.AssignSavePath();
            _storage.ReadFromDisk();
            var infos      = _storage.Load("FileInfos" + typeof(T).Name, new List<FileInfo<T>>(), out _);
            var targetInfo = infos.Find(file => file.FileName == fileName);
            if (targetInfo != null)
            {
                targetInfo.LastModifyTime = DateTime.Now;
                if (replacePayload)
                    targetInfo.Payload = payload;
            }
            else
            {
                targetInfo = new FileInfo<T>
                {
                    FileName       = fileName,
                    CreateTime     = DateTime.Now,
                    LastModifyTime = DateTime.Now,
                    Payload        = payload
                };
                infos.Add(targetInfo);
            }
            
            _storage.Save("FileInfos" + typeof(T).Name, infos);
            _storage.WriteToDisk();
            return targetInfo;
        }

        private void Save<T>(string key, T value)
        {
            MakeSureInited();
            _storage.Save(key, value);
        }

        public void SaveFile<T>(T payload) where T : IFileInfoPayload
        {
            if (!_loaded)
            {
                Debug.LogWarning("需要先加载一个存档才行");
                return;
            }

            var fileInfo = SaveFileInfo(_currentFile, payload, true);
            this.SendEvent(new SaveFileEvent<T>
            {
                FileInfo = fileInfo
            });
            SaveFile();
        }

        public void SaveFile<T>(string fileName, T payload) where T : IFileInfoPayload
        {
            if (!_loaded)
            {
                Debug.LogWarning("需要先加载一个存档才行");
                return;
            }

            _currentFile = fileName;
            var fileInfo = SaveFileInfo(_currentFile, payload, true);
            this.SendEvent(new SaveFileEvent<T>
            {
                FileInfo = fileInfo
            });
            SaveFile();
        }

        public void SaveFile()
        {
            if (!_loaded)
            {
                Debug.LogWarning("需要先加载一个存档才行");
                return;
            }

            _beforeFileSaved?.Invoke(_currentFile);
            _storage.AssignSavePath(_currentFile);
            _storage.Clear();
            foreach (var savable in _savableProperties)
            {
                savable.OnSave?.Invoke();
            }
            
            _storage.WriteToDisk();
            SaveFileName();
            this.SendEvent(new SaveFileEvent());
        }

        protected override void OnInit()
        {
            if (!_inited)
            {
                _storage = this.GetUtility<IStorage>();
                _storage.AssignSavePath();
                _storage.ReadFromDisk();
                _fileNames = _storage.Load("FileNames", new List<string>(), out _);
                _inited    = true;
            }
        }
        public void CreateFile(string fileName)
        {
            MakeSureInited();

            if (_fileNames.Contains(fileName))
            {
                Debug.LogError($"已存在存档{fileName}");
                return;
            }

            _fileNames.Add(fileName);
            SaveFileName();
        }

        public IReadOnlyList<FileInfo<T>> GetFileInfos<T>() where T : IFileInfoPayload
        {
            MakeSureInited();
            _storage.AssignSavePath();
            _storage.ReadFromDisk();
            return _storage.Load("FileInfos" + typeof(T).Name, new List<FileInfo<T>>(), out _);
        }

        public void Register<T>(BsProperty<T> savable, string name)
        {
            var type = typeof(T);
            if (!_savableProperties.Contains(savable))
            {
                _savableProperties.Add(savable);
                switch (type.IsGenericType)
                {
                    case true when type.GetGenericTypeDefinition() == typeof(List<>):
                        _defaultValues.Add(name, CopyList(type, savable.Value));
                        break;
                    case true when type.GetGenericTypeDefinition() == typeof(Dictionary<,>):
                        _defaultValues.Add(name, CopyDic(type, savable.Value));
                        break;
                    case true when type.GetGenericTypeDefinition() == typeof(HashSet<>):
                        _defaultValues.Add(name, CopyHashSet(type, savable.Value));
                        break;
                    default:
                        _defaultValues.Add(name, savable.Value);
                        break;
                }

                var saveItem = (ISavable)savable;
                saveItem.OnLoad += () =>
                {
                    var value = Load(name, (T)_defaultValues[name], out var isNew);
                    if (!isNew)
                        savable.Value = value;
                    else
                    {
                        savable.Value = type.IsGenericType switch
                        {
                            true when type.GetGenericTypeDefinition() == typeof(List<>) => (T)CopyList(type,
                                _defaultValues[name]),
                            true when type.GetGenericTypeDefinition() == typeof(Dictionary<,>) => (T)CopyDic(type,
                                _defaultValues[name]),
                            true when type.GetGenericTypeDefinition() == typeof(HashSet<>) => (T)CopyHashSet(type,
                                _defaultValues[name]),
                            _ => (T)_defaultValues[name]
                        };
                    }
                };
                saveItem.OnSave += () => Save(name, savable.Value);
            }
            else Debug.Log("重复注册值");
        }

        private object CopyHashSet(Type type, object value)
        {
            var elementType = type.GetGenericArguments()[0];
            var newSet = (IEnumerable)Activator.CreateInstance(
                typeof(HashSet<>).MakeGenericType(elementType), value);

            return newSet;
        }

        private object CopyDic(Type type, object value)
        {
            var genericArgs = type.GetGenericArguments();
            var   keyType     = genericArgs[0];
            var   valueType   = genericArgs[1];
            var newDict = (IDictionary)Activator.CreateInstance(
                typeof(Dictionary<,>).MakeGenericType(keyType, valueType)
            );
            foreach (DictionaryEntry entry in (IDictionary)value)
            {
                newDict.Add(entry.Key, entry.Value);
            }

            return newDict;
        }

        private object CopyList(Type type, object value)
        {
            var elementType = type.GetGenericArguments()[0];
            var newList = (IList)Activator.CreateInstance(
                typeof(List<>).MakeGenericType(elementType)
            );

            foreach (var item in (IEnumerable)value)
            {
                newList.Add(item);
            }

            return newList;
        }

        public void Unregister<T>(BsProperty<T> savable)
        {
            if (_savableProperties.Contains(savable))
            {
                _savableProperties.Remove(savable);
                var saveItem = (ISavable)savable;
                saveItem.OnLoad = null;
                saveItem.OnSave = null;
            }
            else Debug.Log("重复注销值");
        }

        public void CreateFile<T>(string fileName, T payload) where T : IFileInfoPayload
        {
            MakeSureInited();
            if (_fileNames.Contains(fileName)) return;
            _fileNames.Add(fileName);
            SaveFileInfo(fileName, payload, true);
            SaveFileName();
        }
    }
}