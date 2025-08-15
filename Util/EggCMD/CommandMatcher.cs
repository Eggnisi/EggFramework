#region

//文件创建者：Egg
//创建时间：03-25 10:35

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    public static class CommandMatcher
    {
        private static          bool                                                      _inited;
        private static readonly Dictionary<string, Dictionary<int, List<ICommandHandle>>> _commandHandles    = new();
        private static readonly Dictionary<ICommandHandle, Type>                          _inst2TypeDic      = new();
        private static readonly Dictionary<ICommandHandle, List<Type>>                    _inst2ParamTypeDic = new();

        public static ICommandHandle Match(string commandName, List<string> rawParams)
        {
            MakeSureInited();
            if (_commandHandles.TryGetValue(commandName, out var commandDic))
            {
                if (commandDic.TryGetValue(rawParams.Count, out var list))
                {
                    if (list.Any(handle => handle.ParamValidCheck(rawParams)))
                        return list.FirstOrDefault(handle => handle.ParamValidCheck(rawParams));
                    CommandLogger.LogError($"Invalid Param Type: Param type is not match, Command: {commandName}, ParamCount: {rawParams.Count}\n" +
                                           $"RawCommand:{GetCommandString(commandName, rawParams)}");
                    return null;
                }

                CommandLogger.LogError($"Invalid Param Count: Can't find command handle with {rawParams.Count} params\n" +
                                       $"RawCommand:{GetCommandString(commandName, rawParams)}");
            }
            else CommandLogger.LogError($"Invalid Command Name: Can't find command {commandName}");

            return null;
        }

        private static string GetCommandString(string commandName,List<string> rawParams)
        {
            var builder = new StringBuilder();
            builder.Append(commandName);
            builder.Append(' ');
            rawParams.ForEach(param =>
            {
                builder.Append(param);
                builder.Append(' ');
            });
            return builder.ToString();
        }

        private static void MakeSureInited()
        {
            if (_inited) return;
            _inited = true;
            var types       = TypeUtil.GetDerivedClasses(typeof(ICommandHandle));
            var priorityDic = new Dictionary<ICommandHandle, int>();
            foreach (var type in types)
            {
                var inst = (ICommandHandle)Activator.CreateInstance(type);
                _inst2TypeDic[inst] = type;
                var baseType = type.BaseType;
                var (commandName, priority) = GetHandleData(type);
                priorityDic[inst]           = priority;
                if (!_commandHandles.TryGetValue(commandName, out var commandHandleDic))
                {
                    commandHandleDic             = new Dictionary<int, List<ICommandHandle>>();
                    _commandHandles[commandName] = commandHandleDic;
                }

                var paramCount = baseType!.IsGenericType ? baseType.GenericTypeArguments.Length : 0;
                _inst2ParamTypeDic[inst] =
                    baseType!.IsGenericType ? baseType.GenericTypeArguments.ToList() : new List<Type>();
                if (_commandHandles[commandName].TryGetValue(paramCount, out var list))
                {
                    list.Add(inst);
                }
                else _commandHandles[commandName][paramCount] = new List<ICommandHandle> { inst };
            }

            foreach (var (key, value) in _commandHandles)
            {
                foreach (var (i, list) in value)
                {
                    list.Sort((t1, t2) =>
                    {
                        if (priorityDic[t1] > priorityDic[t2]) return 1;
                        if (priorityDic[t1] < priorityDic[t2]) return -1;
                        return 0;
                    });
                }
            }
        }

        public static List<Type> GetHandleParamTypes(ICommandHandle handle)
        {
            MakeSureInited();
            return _inst2ParamTypeDic.GetValueOrDefault(handle, new List<Type>());
        }

        private static (string, int) GetHandleData(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(CommandHandleAttribute), false);
            if (attrs.Length != 1)
            {
                CommandLogger.LogError(
                    "Invalid Command Handle: Any class inherit from \'CommandHandle\' must have an attribute of \'CommandHandleAttribute\'");
                throw new Exception(
                    "Invalid Command Handle: Any class inherit from \'CommandHandle\' must have an attribute of \'CommandHandleAttribute\'");
            }

            var attr        = (CommandHandleAttribute)attrs[0];
            var commandName = attr.TargetCommand?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(attr.TargetCommand))
            {
                CommandLogger.LogError(
                    "Invalid Command Handle: TargetCommand of CommandHandleAttribute can be null or empty");
                throw new Exception(
                    "Invalid Command Handle: TargetCommand of CommandHandleAttribute can be null or empty");
            }

            return (commandName, attr.Priority);
        }
    }
}