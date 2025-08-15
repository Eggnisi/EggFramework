#region

//文件创建者：Egg
//创建时间：03-25 11:30

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace EggFramework.Util.EggCMD
{
    public static class CommandActuator
    {
        public static void Execute(ICommandHandle handle, List<string> paramList)
        {
            var types = CommandMatcher.GetHandleParamTypes(handle);
            if (types.Count != paramList.Count)
            {
                CommandLogger.LogError("Invalid Param Count");
                throw new Exception("Invalid Param Count");
            }
            var switchedParams =
                types.Zip(paramList, (t, v) => TokenSplitter.ParseParam(v, t, out var val) ? val : null);
            typeof(ICommandHandle)!.GetMethod("Handle")!.Invoke(handle, new object[] { switchedParams.ToArray() });
        }
    }
}