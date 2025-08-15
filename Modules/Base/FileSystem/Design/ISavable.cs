#region

//文件创建者：Egg
//创建时间：09-10 02:23

#endregion

using System;

namespace EggFramework
{
    public interface ISavable
    {
        Action OnLoad { get; set; }
        Action OnSave { get; set; }
    }
}