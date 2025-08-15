#region

//文件创建者：Egg
//创建时间：03-26 01:33

#endregion

using System;
using System.Collections;
using System.Text;
using QFramework;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    public static class CommandLogger
    {
        public enum ELoggerLevel
        {
            Msg,
            Warning,
            Error
        }

        private const string INDENT = "    ";

        public static void Log(object value, bool showChild = false)
        {
            if (!showChild)
            {
                OutputLine(value.ToString());
            }
            else
            {
                InnerLog(value, ELoggerLevel.Msg, 0);
            }
        }

        private static void InnerLog(object value, ELoggerLevel level, int indent)
        {
            if (value == null) return;
            var type = value.GetType();
            if (typeof(IList).IsAssignableFrom(type))
            {
                OutputLine("[", level, indent);
                foreach (var o in (IList)value)
                {
                    InnerLog(o, level, indent + 1);
                    if (((IList)value).IndexOf(o) != ((IList)value).Count - 1)
                        OutputLine(",", level, indent);
                }

                OutputLine("]", level, indent);

                return;
            }

            if (typeof(IDictionary).IsAssignableFrom(type))
            {
                OutputLine(type.Name + ":", level, indent);
                foreach (DictionaryEntry o in (IDictionary)value)
                {
                    InnerLog(o.Key,   level, indent + 1);
                    InnerLog(o.Value, level, indent + 1);
                }

                return;
            }

            if (type == typeof(string) ||
                type == typeof(double) ||
                type == typeof(char) ||
                type == typeof(float) ||
                type == typeof(int) ||
                type == typeof(bool) ||
                type == typeof(Enum)
               )
            {
                OutputLine(value.ToString(), level, indent);
                return;
            }

            var fields = type.GetSerializeFieldInfos();
            OutputLine("{", level, indent);
            foreach (var fieldInfo in fields)
            {
                if (fieldInfo.FieldType == typeof(string) ||
                    fieldInfo.FieldType == typeof(double) ||
                    fieldInfo.FieldType == typeof(char) ||
                    fieldInfo.FieldType == typeof(float) ||
                    fieldInfo.FieldType == typeof(int) ||
                    fieldInfo.FieldType == typeof(bool) ||
                    fieldInfo.FieldType == typeof(Enum)
                   )
                {
                    OutputLine($"{fieldInfo.Name}<{fieldInfo.FieldType.Name}>:{fieldInfo.GetValue(value)}", level,
                        indent + 1);
                }
                else
                {
                    if (typeof(IList).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        var eleType = fieldInfo.FieldType.GenericTypeArguments[0];
                        OutputLine($"{fieldInfo.Name}<List<{eleType.Name}>>:", level, indent + 1);
                    }
                    else
                        OutputLine($"{fieldInfo.Name}<{fieldInfo.FieldType.Name}>:", level, indent + 1);

                    InnerLog(fieldInfo.GetValue(value), level, indent + 2);
                }
            }

            OutputLine("}", level, indent);
        }

        private static Action<string> _customOutputAction;

        public static IUnRegister RegisterOnOutput(Action<string> action)
        {
            _customOutputAction += action;
            return new CustomUnRegister(() => { _customOutputAction -= action; });
        }

        private static void OutputLine(string value, ELoggerLevel level = ELoggerLevel.Msg, int indent = 0)
        {
            var stringBuilder = new StringBuilder();
            for (var i = 0; i < indent; i++)
            {
                stringBuilder.Append(INDENT);
            }

            stringBuilder.Append(level switch
            {
                ELoggerLevel.Msg     => value,
                ELoggerLevel.Warning => CommandOutputModifier.WarningWrap(value),
                ELoggerLevel.Error   => CommandOutputModifier.ErrorWrap(value),
                _                    => string.Empty
            });

            _customOutputAction?.Invoke(stringBuilder.ToString());
#if UNITY_EDITOR
            if (CommandEditorWindow.Enable)
            {
                var window = EditorWindow.GetWindow<CommandEditorWindow>();
                window.OutputString(stringBuilder.ToString(), false);
            }
            else
#endif
            {
                switch (level)
                {
                    case ELoggerLevel.Msg:
                        Debug.Log(stringBuilder.ToString());
                        break;
                    case ELoggerLevel.Warning:
                        Debug.LogWarning(stringBuilder.ToString());
                        break;
                    case ELoggerLevel.Error:
                        Debug.LogError(stringBuilder.ToString());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(level), level, null);
                }
            }
        }

        public static void LogError(object value, bool showChild = false)
        {
            if (!showChild)
            {
                OutputLine(value.ToString(), ELoggerLevel.Error);
            }
            else
            {
                InnerLog(value, ELoggerLevel.Error, 0);
            }
        }

        public static void LogWarning(object value, bool showChild = false)
        {
            if (!showChild)
            {
                OutputLine(value.ToString(), ELoggerLevel.Warning);
            }
            else
            {
                InnerLog(value, ELoggerLevel.Warning, 0);
            }
        }
    }
}