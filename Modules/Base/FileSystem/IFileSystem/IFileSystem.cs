using System;
using QFramework;
using System.Collections.Generic;

namespace EggFramework
{
    public interface IFileSystem : ISystem
    {
        string                CurrentFile { get; }
        IReadOnlyList<string> FileNames   { get; }
        IReadOnlyList<FileInfo<T>> GetFileInfos<T>() where T : IFileInfoPayload;
        void Register<T>(BsProperty<T> savable, string name);
        void Unregister<T>(BsProperty<T> savable);
        void CreateFile<T>(string fileName, T payload) where T : IFileInfoPayload;
        void CreateFile(string fileName);

        //自动加载当前的存档
        T LoadFile<T>(string fileName = "file1", T defaultValue = default) where T : IFileInfoPayload;
        void LoadFile(string fileName = "file1");
        void SaveFile<T>(T payload) where T : IFileInfoPayload;

        void SaveFile<T>(string fileName, T payload) where T : IFileInfoPayload;
        void SaveFile();
        void DeleteFile(string fileName= "file1");
        void DeleteAll();

        IUnRegister RegisterOnFileLoaded(Action<string> action);
        
        IUnRegister RegisterBeforeFileSaved(Action<string> action);
    }
}