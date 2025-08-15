#region

//文件创建者：Egg
//创建时间：03-27 10:27

#endregion

using System;

namespace EggFramework.Util.EggCMD
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandTokenSourceAttribute : Attribute
    {
        public Type TokenSourceType;
        public CommandTokenSourceAttribute(Type type)
        {
            TokenSourceType = type;
        }
    }
}