#region

//文件创建者：Egg
//创建时间：02-22 05:42

#endregion

using System;

namespace EggFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BuffParamValidatorAttribute : Attribute
    {
        public Type Type;

        public BuffParamValidatorAttribute(Type type)
        {
            Type = type;
        }
    }
}