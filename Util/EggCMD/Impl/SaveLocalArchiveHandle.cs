#region

//文件创建者：Egg
//创建时间：03-27 08:36

#endregion

using System.IO;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("saveArchive")]
    [CommandComment("保存本地存档")]
    public sealed class SaveLocalArchiveHandle : CommandHandle
    {
        protected override void Handle()
        {
            TypeUtil.GetFirstActiveArchitecture().GetSystem<IFileSystem>().SaveFile();
            CommandLogger.Log("存档成功");
        }
    }
}