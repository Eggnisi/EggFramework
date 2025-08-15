#region

//文件创建者：Egg
//创建时间：02-22 03:52

#endregion

using System;
using System.Collections.Generic;

namespace EggFramework
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class BuffParamValueGetterAttribute : Attribute
    {
        public Type Type;

        public BuffParamValueGetterAttribute(Type type)
        {
            Type = type;
        }
    }
}