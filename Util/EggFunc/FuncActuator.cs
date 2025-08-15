#region

// 作者： Egg
// 时间： 2025 07.19 09:00

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.Util.EggCMD;
using UnityEngine;

namespace EggFramework
{
    public static class FuncActuator
    {
        public static object Execute(IFuncHandle handle, List<string> paramList)
        {
            var types = FuncMatcher.GetHandleParamTypes(handle);
            if (types.Count != paramList.Count)
            {
                Debug.LogError("Invalid Param Count");
                throw new Exception("Invalid Param Count");
            }
            var switchedParams =
                types.Zip(paramList, (t, v) => TokenSplitter.ParseParam(v, t, out var val) ? val : null);
            return typeof(IFuncHandle)!.GetMethod("Handle")!.Invoke(handle, new object[] { switchedParams.ToArray() });
        }
    }
}