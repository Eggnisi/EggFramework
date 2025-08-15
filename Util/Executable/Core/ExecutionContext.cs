#region

//文件创建者：Egg
//创建时间：07-26 06:55

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EggFramework.Util;
using EggFramework.Util.EggCMD;
using UnityEngine;

namespace EggFramework.Executable
{
   public sealed class ExecutionContext
    {
        public ExecutionContext() => RegisterFunctions();
        
        private void RegisterFunctions()
        {
            var commandSet = CommandHelper.Helpers.Select(help => help.Key).ToHashSet();
            var funcSet = TypeUtil.GetDerivedClasses(typeof(IFuncHandle))
                .Select(type => type.GetAttribute<FuncHandleAttribute>()?.TargetFunc).Where(func => func != null)
                .ToHashSet();
            // 正确创建函数字典
            var commandDic = GetDicBySet(commandSet);
            var funcDic    = GetDicBySet(funcSet);
            // 将字典中的函数添加到上下文
            foreach (var pair in commandDic) Functions[pair.Key] = pair.Value;
            foreach (var pair in funcDic)
            {
                if (!Functions.TryAdd(pair.Key, pair.Value))
                {
                    Debug.LogWarning("[脚本解析器]：已经存在同名命令，函数名冲突，无法添加到上下文中：" + pair.Key);
                }
            }

            return;

            Dictionary<string, Func<object[], object>> GetDicBySet(HashSet<string> set)
                => set.ToDictionary(
                    key => key, // 字典的键是命令字符串
                    key => (Func<object[], object>)(paramList =>
                    {
                        var builder = new StringBuilder();
                        builder.Append(key);
                        foreach (var o in paramList)
                        {
                            builder.Append(" ");
                            if (o is string str)
                                builder.Append($"\"{str.Trim('"')}\"");
                            else builder.Append(o);
                        }

                        var command = builder.ToString();
                        if (CommandHelper.CommandExists(key))
                        {
                            CommandManager.DoCommand(command);
                            return command;
                        }

                        return FuncManager.DoFunc(command);
                    })
                );
        }

        public Dictionary<string, object>                 Variables { get; } = new();
        public Dictionary<string, Func<object[], object>> Functions { get; } = new();
    }
}