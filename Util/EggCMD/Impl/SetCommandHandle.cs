#region

//文件创建者：Egg
//创建时间：03-26 02:47

#endregion

#if UNITY_EDITOR


using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("set")]
    public sealed class SetCommandHandle : CommandHandle<string, string>
    {
        [CommandComment(                               "修改配置信息")]
        protected override void Handle([CommandComment("配置键")] string p1, [CommandComment("配置值")] string p2)
        {
            if (p1 == "dateFormat")
            {
                if (CommandOutputModifier.IsFormatDateStringValid(p2))
                {
                    var rawFormat = CommandOutputModifier.GetDateFormat();
                    CommandOutputModifier.SetDateFormat(p2);
                    CommandLogger.Log($"date format change from {rawFormat} to {p2}");
                }
                else CommandLogger.LogError($"date format is invalid: {p2}");
            }
            else CommandLogger.LogError($"Invalid command param {p1}");
        }
    }
}
#endif