#region

//文件创建者：Egg
//创建时间：03-27 08:36

#endregion

using System.IO;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("clearArchive")]
    [CommandComment("清除本地存档")]
    public sealed class ClearLocalArchiveHandle : CommandHandle
    {
        protected override void Handle()
        {
            if (Application.isPlaying) CommandLogger.LogWarning("运行时删除本地存档，缓冲区内数据仍可能写入本地");
            Directory.Delete(Application.persistentDataPath, true);
            CommandLogger.Log("本地存档已清除");
        }
    }
}