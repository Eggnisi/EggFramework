#region

//文件创建者：Egg
//创建时间：03-26 02:23

#endregion

using System;

namespace EggFramework.Util.EggCMD
{
    public static class CommandOutputModifier
    {
        public static string UserPrefix()
        {
            return $"<User {GetFormatDateNow()}>";
        }
#if UNITY_EDITOR
        
        public static void SetDateFormat(string format)
        {
            StorageUtil.SaveToSettingFile(nameof(CommandOutputModifier) + "DataFormat", format);
        }
#endif
        
        public static string GetDateFormat()
        {
            return StorageUtil.LoadFromSettingFile(nameof(CommandOutputModifier) + "DataFormat","yyyy-MM-dd hh:mm:ss");
        }

        public static bool IsFormatDateStringValid(string format)
        {
            try
            {
                // 使用一个已知的 DateTime 对象测试格式字符串
                DateTime now        = DateTime.Now;
                string   testOutput = now.ToString(format);
                return true; // 如果没有抛出异常，则格式字符串是合法的
            }
            catch (FormatException)
            {
                return false; // 格式字符串非法
            }
        }

        public static string GetFormatDateNow()
        {
            return DateTime.Now.ToString(GetDateFormat());
        }

        public static string ErrorWrap(string value)
        {
            return $"<color=#FF0000>{value}</color>";
        }
        
        public static string WarningWrap(string value)
        {
            return $"<color=#FFFF00>{value}</color>";
        }
    }
}