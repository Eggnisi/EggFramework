#region

//文件创建者：Egg
//创建时间：02-22 02:29

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EggFramework.Util;
using UnityEngine;

namespace EggFramework
{
    public static class BuffUtil
    {
        private static bool _inited;

        private static readonly Dictionary<Type, Type>                     _handle2Param           = new();
        private static readonly Dictionary<Type, Type>                     _handle2Interface       = new();
        private static readonly Dictionary<Type, List<BuffSerializeParam>> _handle2SerializeParams = new();
        private static readonly Dictionary<string, Type>                   _handleName2Type        = new();
        private static readonly Dictionary<Type, IBuffParamValueGetter>    _valueGetterInst        = new();
        private static readonly Dictionary<string, Type>                   _valueGetterName2Type   = new();

        private static readonly Dictionary<Type, IBuffParamValidator> _validatorInst      = new();
        private static readonly Dictionary<string, Type>              _validatorName2Type = new();

        private static readonly Dictionary<Type, IBuffParamParser> _parserInst      = new();
        private static readonly Dictionary<string, Type>           _parserName2Type = new();

        private static void MakeSureInited()
        {
            if (_inited) return;
            _inited = true;
            var handleTypes = TypeUtil.GetDerivedClassesFromGenericInterfaces(typeof(IBuffCallbackHandle<,>));
            foreach (var handleType in handleTypes)
            {
                var interfaces = handleType.GetInterfaces();
                if (interfaces.Length <= 0) continue;
                if (interfaces[0].IsGenericType)
                {
                    _handle2Interface[handleType] = interfaces[0];
                    var paramType = _handle2Param[handleType] = interfaces[0].GenericTypeArguments[1];
                    _handleInst[handleType]           = Activator.CreateInstance(handleType);
                    _handleName2Type[handleType.Name] = handleType;
                    var propertyInfos = paramType.GetSerializeFieldInfos();
                    var paramList     = new List<BuffSerializeParam>();
                    foreach (var propertyInfo in propertyInfos)
                    {
                        var valueGetterName = GetValueGetterName(propertyInfo);
                        var parserName      = GetParserName(propertyInfo);
                        var validatorName   = GetValidatorName(propertyInfo);

                        paramList.Add(new BuffSerializeParam
                        {
                            ParamName       = propertyInfo.Name,
                            ParamType       = GetParamType(propertyInfo),
                            ValueGetterName = valueGetterName,
                            ParserName      = parserName,
                            ValidatorName   = validatorName,
                        });
                    }

                    _handle2SerializeParams[handleType] = paramList;
                }
            } //

            InitValueGetter();
            InitValidator();
            InitParser();
        }

        private static string GetValueGetterName(FieldInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(BuffParamValueGetterAttribute), false);
            var name  = "";
            if (attrs.Length > 0)
            {
                name = ((BuffParamValueGetterAttribute)attrs[0]).Type.Name;
            }

            return name;
        }

        private static string GetParserName(FieldInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(BuffParamParserAttribute), false);
            var name  = "";
            if (attrs.Length > 0)
            {
                name = ((BuffParamParserAttribute)attrs[0]).Type.Name;
            }

            return name;
        }

        private static string GetValidatorName(FieldInfo propertyInfo)
        {
            var attrs = propertyInfo.GetCustomAttributes(typeof(BuffParamValidatorAttribute), false);
            var name  = "";
            if (attrs.Length > 0)
            {
                name = ((BuffParamValidatorAttribute)attrs[0]).Type.Name;
            }

            return name;
        }

        private static void InitValueGetter()
        {
            var valueGetterTypes = TypeUtil.GetDerivedClasses(typeof(IBuffParamValueGetter));
            foreach (var valueGetterType in valueGetterTypes)
            {
                _valueGetterInst[valueGetterType] = (IBuffParamValueGetter)Activator.CreateInstance(valueGetterType);
                _valueGetterName2Type[valueGetterType.Name] = valueGetterType;
            }
        }

        private static void InitValidator()
        {
            var valueGetterTypes = TypeUtil.GetDerivedClasses(typeof(IBuffParamValidator));
            foreach (var valueGetterType in valueGetterTypes)
            {
                _validatorInst[valueGetterType] = (IBuffParamValidator)Activator.CreateInstance(valueGetterType);
                _validatorName2Type[valueGetterType.Name] = valueGetterType;
            }
        }

        private static void InitParser()
        {
            var parserTypes = TypeUtil.GetDerivedClasses(typeof(IBuffParamParser));
            foreach (var parserType in parserTypes)
            {
                _parserInst[parserType]           = (IBuffParamParser)Activator.CreateInstance(parserType);
                _parserName2Type[parserType.Name] = parserType;
            }
        }

        private static string GetParamType(FieldInfo fieldInfo)
        {
            if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
            {
                var elementType = fieldInfo.FieldType.GenericTypeArguments[0];
                if (ParamTypeIsValid(elementType.Name, fieldInfo))
                {
                    return elementType.Name + "List__";
                }

                return "非法参数";
            }

            return ParamTypeIsValid(fieldInfo.FieldType.Name, fieldInfo) ? fieldInfo.FieldType.Name : "非法参数";
        }

        public static List<string> GetValueGetterByName(string valueGetterName)
        {
            var type = _valueGetterName2Type.GetValueOrDefault(valueGetterName, null);
            if (type == null)
            {
                return null;
            }

            return _valueGetterInst[type].ValueGetter();
        }

        public static bool ValidateByValueGetter(string valueGetterName, string value, ref string errMsg)
        {
            var type = _validatorName2Type.GetValueOrDefault(valueGetterName, null);
            if (type == null)
            {
                return false;
            }

            return _validatorInst[type].Validate(value, ref errMsg);
        }

        public static List<string> GetBuffCallbackNames()
        {
            MakeSureInited();
            var list = new List<string>();

            foreach (var (key, value) in _handle2Param)
            {
                list.Add(key.Name);
            }

            return list;
        }

        public static Type GetHandleTypeByHandleName(string handleName)
        {
            return _handleName2Type.GetValueOrDefault(handleName, null);
        }

        private static Type GetHandleInterfaceByHandleType(Type type)
        {
            return _handle2Interface.GetValueOrDefault(type, null);
        }

        private static bool ParamTypeIsValid(string paramType, FieldInfo fieldInfo)
        {
            var attrs        = fieldInfo.GetCustomAttributes(typeof(BuffParamParserAttribute), false);
            var customParser = attrs.Length > 0;
            if (paramType is "Int32" or "String" or "Boolean" or "Single" || customParser)
                return true;
            return false;
        }

        public static List<BuffSerializeParam> GetSerializeParamsByHandleName(string name)
        {
            MakeSureInited();
            var type = GetHandleTypeByHandleName(name);
            if (type == null) return null;
            if (_handle2SerializeParams.TryGetValue(type, out var list)) return list.ToList();
            Debug.LogError("类型不是IBuffCallbackHandle");
            return null;
        }

        public static List<BuffSerializeParam> GetSerializeParamsByHandleType(Type type)
        {
            MakeSureInited();
            if (_handle2SerializeParams.TryGetValue(type, out var list)) return list;
            Debug.LogError("类型不是IBuffCallbackHandle");
            return null;
        }

        private static readonly Dictionary<Type, object> _handleInst = new();

        public static void HandleBuff(BuffRunTimeInfo buffRunTimeInfo, BuffSerializeCallback buffSerializeCallback,
            params object[] paramList)
        {
            MakeSureInited();
            var handleType    = GetHandleTypeByHandleName(buffSerializeCallback.CallbackName);
            var interfaceType = GetHandleInterfaceByHandleType(handleType);
            var paramInst     = ParseParam(handleType, buffSerializeCallback.Params);
            interfaceType!.GetMethod("Handle")!.Invoke(_handleInst[handleType],
                new[] { buffRunTimeInfo, paramInst, paramList });
        }

        private static object ParseByParserName(string parserName, string value)
        {
            var type = _parserName2Type.GetValueOrDefault(parserName, null);
            if (type == null)
            {
                return false;
            }

            return _parserInst[type].Parse(value);
        }

        private static object ParseParam(Type handleType, List<BuffSerializeParam> @params)
        {
            var paramType = _handle2Param[handleType];
            var paramInst = Activator.CreateInstance(paramType);
            foreach (var fieldInfo in paramType.GetSerializeFieldInfos())
            {
                var isList = typeof(IList).IsAssignableFrom(fieldInfo.FieldType);
                foreach (var sParam in @params)
                {
                    if (sParam.ParamName != fieldInfo.Name) continue;

                    Type elementType = null;
                    if (isList)
                    {
                        elementType = fieldInfo.FieldType.GenericTypeArguments[0];
                    }

                    fieldInfo.SetValue(paramInst,
                        string.IsNullOrEmpty(sParam.ValueGetterName)
                            ? ParseValue(sParam, isList, elementType)
                            : ParseValue(sParam, isList, elementType, true));
                }
            }

            object ParseValue(BuffSerializeParam spa, bool isList, Type elementType, bool custom = false)
            {
                var value     = custom ? spa.CustomValue : spa.ParamValue;
                var listValue = custom ? spa.CustomListValue : spa.ListValue;

                if (isList)
                {
                    var ret = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    foreach (var se in listValue)
                    {
                        ret.Add(spa.ParamType.Replace("List__", "") switch
                        {
                            "Int32" => int.TryParse(se, out var intValue) ? intValue : 0,
                            "Single" => float.TryParse(se, out var floatValue) ? floatValue : 0,
                            "Boolean" => se == "1",
                            "String" => se,
                            _ when !string.IsNullOrEmpty(spa.ParserName) => ParseByParserName(spa.ParserName, se),
                            _ => null
                        });
                    }

                    return ret;
                }

                return spa.ParamType switch
                {
                    "Int32" => int.TryParse(value, out var intValue) ? intValue : 0,
                    "Single" => float.TryParse(value, out var floatValue) ? floatValue : 0,
                    "Boolean" => value == "1",
                    "String" => value,
                    _ when !string.IsNullOrEmpty(spa.ParserName) => ParseByParserName(spa.ParserName, value),
                    _ => null
                };
            }

            return paramInst;
        }
    }
}