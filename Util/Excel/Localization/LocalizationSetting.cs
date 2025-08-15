#region

//文件创建者：Egg
//创建时间：11-10 10:45

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Localization
{
    [Serializable]
    public sealed class LocalizationSetting
    {
        [LabelText("AppId")]   public string AppId;
        [LabelText("密钥")]      public string SecretKey;
        [LabelText("本地化数据路径")] public string LocalizationDataSavePathRoot = "Assets/Localization";

        [ValueDropdown("@EggFramework.Util.TranslateUtil.SupportedSystemLanguages")] [LabelText("默认语言")]
        public SystemLanguage DefaultLanguage = SystemLanguage.ChineseSimplified;

        [LabelText("支持的其他语言")]
        [ValueDropdown("@EggFramework.Util.TranslateUtil.SupportedSystemLanguages", IsUniqueList = true)]
        public List<SystemLanguage> SupportedLanguages = new();
    }
}