#region

//文件创建者：Egg
//创建时间：04-08 07:19

#endregion

using System;

namespace EggFramework
{
    [Serializable]
    public sealed class FileInfo<T> where T : IFileInfoPayload
    {
        public string   FileName;
        public DateTime CreateTime;
        public DateTime LastModifyTime;
        public T        Payload;
    }
}