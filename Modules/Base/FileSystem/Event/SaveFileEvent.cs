#region

//文件创建者：Egg
//创建时间：04-25 06:07

#endregion

namespace EggFramework
{
    public struct SaveFileEvent<T> where T : IFileInfoPayload
    {
        public FileInfo<T> FileInfo;
    }

    public struct SaveFileEvent
    {
        
    }
}