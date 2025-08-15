#region

//文件创建者：Egg
//创建时间：09-10 02:24

#endregion

using System;
using QFramework;

namespace EggFramework
{
    //对接FileSystem进行持久化
    public sealed class BsProperty<T> : BindableProperty<T>, ISavable
    {
        public BsProperty(T defaultValue = default) => mValue = defaultValue;
        Action ISavable.OnLoad { get; set; } = () => { };
        Action ISavable.OnSave { get; set; } = () => { };
    }
}