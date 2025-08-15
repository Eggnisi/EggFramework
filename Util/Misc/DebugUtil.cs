#region

//文件创建者：Egg
//创建时间：04-10 11:03

#endregion

using System.Collections;
using System.Text;
using UnityEngine;

namespace EggFramework.Util
{
    public static class DebugUtil
    {
        public static void LogObject(object obj, StringBuilder builder = null)
        {
            if (obj == null) return;
            var type  = obj.GetType();
            var isNew = builder == null;
            builder ??= new StringBuilder();
            if (typeof(IList).IsAssignableFrom(type))
            {
                LogArray((IList)obj, builder);
                Debug.Log(builder.ToString());
                return;
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                LogDic((IDictionary)obj, builder);
                Debug.Log(builder.ToString());
                return;
            }

            var fields = type.GetSerializeFieldInfos();
            foreach (var fieldInfo in fields)
            {
                if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                {
                    builder.Append(" ");
                    builder.Append(fieldInfo.Name);
                    builder.Append(":");
                    LogArray((IList)fieldInfo.GetValue(obj), builder);
                    continue;
                }

                if (typeof(IDictionary).IsAssignableFrom(fieldInfo.FieldType))
                {
                    builder.Append(" ");
                    builder.Append(fieldInfo.Name);
                    builder.Append(":");
                    LogDic((IDictionary)fieldInfo.GetValue(obj), builder);
                    continue;
                }
                builder.Append(" ");
                builder.Append(fieldInfo.Name);
                builder.Append(":");
                switch (1)
                {
                    case 1 when fieldInfo.FieldType == typeof(int):
                    case 1 when fieldInfo.FieldType == typeof(char):
                    case 1 when fieldInfo.FieldType == typeof(string):
                    case 1 when fieldInfo.FieldType == typeof(float):
                    case 1 when fieldInfo.FieldType == typeof(bool):
                    case 1 when fieldInfo.FieldType == typeof(double):
                        builder.Append(fieldInfo.GetValue(obj));
                        break;
                    default:
                        LogObject(fieldInfo.GetValue(obj), builder);
                        break;
                }
            }

            if (isNew)
                Debug.Log(builder.ToString());
        }

        private static void LogDic(IDictionary dic, StringBuilder builder)
        {
            foreach (DictionaryEntry o in dic)
            {
                builder.Append(" Key:");
                LogObject(o.Key, builder);
                builder.Append(" Value:");
                LogObject(o.Value, builder);
            }
        }

        private static void LogArray(IList list, StringBuilder builder)
        {
            foreach (var o in list)
            {
                LogObject(o, builder);
            }
        }
    }
}