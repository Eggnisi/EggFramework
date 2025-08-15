#region

//文件创建者：Egg
//创建时间：02-22 05:43

#endregion

using System;

namespace EggFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BuffParamParserAttribute : Attribute
    {
        public Type Type;

        public BuffParamParserAttribute(Type type)
        {
            Type = type;
        }
    }
}