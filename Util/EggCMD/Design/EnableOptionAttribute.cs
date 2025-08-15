#region

//文件创建者：Egg
//创建时间：03-26 04:28

#endregion

using System;

namespace EggFramework.Util.EggCMD
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EnableOptionAttribute : Attribute
    {
        public string Option;
        public string Comment;
        public Type   TargetType;

        public EnableOptionAttribute(string option, string comment, Type targetType = null)
        {
            Option     = option;
            Comment    = comment;
            TargetType = targetType;
        }
    }
}