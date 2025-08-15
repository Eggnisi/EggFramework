#region

//文件创建者：Egg
//创建时间：12-06 01:39

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using EggFramework.Util;

namespace EggFramework
{
    public static class CloneMapUtil<T>
    {
        public static T Clone(T target)
        {
            var type   = typeof(T);
            var ret    = (object)Activator.CreateInstance<T>();
            var fields = type.GetSerializeFieldInfos();
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType.IsGenericType)
                {
                    if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        fieldInfo.SetValue(ret,
                            CloneList((IList)fieldInfo.GetValue(target), fieldInfo.FieldType.GenericTypeArguments[0]));
                    }
                    else if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        fieldInfo.SetValue(ret,
                            CloneDictionary((IDictionary)fieldInfo.GetValue(target),
                                fieldInfo.FieldType.GenericTypeArguments[0],
                                fieldInfo.FieldType.GenericTypeArguments[1]));
                    }
                }
                else
                {
                    fieldInfo.SetValue(ret, fieldInfo.GetValue(target));
                }
            }

            return (T)ret;
        }

        private static IList CloneList(IList list, Type elementType)
        {
            var ret = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            foreach (var o in list)
            {
                ret.Add(o);
            }

            return ret;
        }

        private static IDictionary CloneDictionary(IDictionary dic, Type keyType, Type valType)
        {
            var ret = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(keyType, valType));

            foreach (DictionaryEntry o in dic)
            {
                ret.Add(o.Key, o.Value);
            }

            return ret;
        }
    }
}