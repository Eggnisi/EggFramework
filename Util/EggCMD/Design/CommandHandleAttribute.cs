#region

//文件创建者：Egg
//创建时间：03-25 10:30

#endregion

using System;

namespace EggFramework.Util.EggCMD
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandHandleAttribute : Attribute
    {
        public string TargetCommand;
        public int Priority;

        public CommandHandleAttribute(string targetCommand, int priority = 1)
            => (TargetCommand, Priority) = (targetCommand, priority);
    }
}