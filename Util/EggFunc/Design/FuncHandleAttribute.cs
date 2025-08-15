#region

// 作者： Egg
// 时间： 2025 07.19 09:03

#endregion

using System;

namespace EggFramework
{
    public sealed class FuncHandleAttribute : Attribute
    {
        public string TargetFunc;

        public FuncHandleAttribute(string targetFunc) => TargetFunc = targetFunc;
    }
}