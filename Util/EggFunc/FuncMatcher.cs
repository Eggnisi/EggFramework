#region

// 作者： Egg
// 时间： 2025 07.19 08:55

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.Util;
using EggFramework.Util.EggCMD;
using UnityEngine;

namespace EggFramework
{
    public static class FuncMatcher
    {
        private static bool _inited;
        private static readonly Dictionary<string, Dictionary<int, List<IFuncHandle>>> _funcHandles = new();
        private static readonly Dictionary<IFuncHandle, Type> _inst2TypeDic = new();
        private static readonly Dictionary<IFuncHandle, List<Type>> _inst2ParamTypeDic = new();

        public static IFuncHandle Match(string funcId, List<string> rawParams)
        {
            MakeSureInited();
            if (_funcHandles.TryGetValue(funcId, out var funcDic))
            {
                if (funcDic.TryGetValue(rawParams.Count, out var list))
                {
                    if (list.Any(handle =>
                        {
                            var valid = handle.ParamValidCheck(rawParams);
                            return valid;
                        }))
                        return list.FirstOrDefault(handle => handle.ParamValidCheck(rawParams));
                    Debug.LogError(
                        $"Invalid Param Type: Param type is not match, please use help {funcId} for more information");
                    Debug.LogError($"Raw param list: {string.Join(' ', rawParams)}");
                    return null;
                }

                Debug.LogError($"Invalid Param Count: Can't find func handle with {rawParams.Count} params");
            }
            else Debug.LogError($"Invalid Func Name: Can't find func {funcId}");

            return null;
        }

        private static void MakeSureInited()
        {
            if (_inited) return;
            _inited = true;
            var types = TypeUtil.GetDerivedClasses(typeof(IFuncHandle));
            foreach (var type in types)
            {
                var inst = (IFuncHandle)Activator.CreateInstance(type);
                _inst2TypeDic[inst] = type;
                var baseType = type.BaseType;
                var funcName = GetHandleData(type);
                if (!_funcHandles.TryGetValue(funcName, out var funcHandleDic))
                {
                    funcHandleDic = new Dictionary<int, List<IFuncHandle>>();
                    _funcHandles[funcName] = funcHandleDic;
                }

                var paramCount = baseType!.IsGenericType ? baseType.GenericTypeArguments.Length - 1 : 0;
                _inst2ParamTypeDic[inst] =
                    baseType!.IsGenericType ? baseType.GenericTypeArguments.ToList() : new List<Type>();
                _inst2ParamTypeDic[inst].RemoveAt(_inst2ParamTypeDic[inst].Count - 1);
                if (_funcHandles[funcName].TryGetValue(paramCount, out var list))
                {
                    list.Add(inst);
                }
                else _funcHandles[funcName][paramCount] = new List<IFuncHandle> { inst };
            }
        }

        public static List<Type> GetHandleParamTypes(IFuncHandle handle)
        {
            MakeSureInited();
            return _inst2ParamTypeDic.GetValueOrDefault(handle, new List<Type>());
        }

        private static string GetHandleData(Type type)
        {
            var attrs = type.GetCustomAttributes(typeof(FuncHandleAttribute), false);
            if (attrs.Length != 1)
            {
                Debug.LogError(
                    "Invalid Func Handle: Any class inherit from \'FuncHandle\' must have an attribute of \'FuncHandleAttribute\'");
                throw new Exception(
                    "Invalid Func Handle: Any class inherit from \'FuncHandle\' must have an attribute of \'FuncHandleAttribute\'");
            }

            var attr = (FuncHandleAttribute)attrs[0];
            var funcId = attr.TargetFunc?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(attr.TargetFunc))
            {
                Debug.LogError(
                    "Invalid Func Handle: TargetFunc of FuncHandleAttribute can be null or empty");
                throw new Exception(
                    "Invalid Func Handle: TargetFunc of FuncHandleAttribute can be null or empty");
            }

            return funcId;
        }
    }
}