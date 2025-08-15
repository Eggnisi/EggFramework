#region

//文件创建者：Egg
//创建时间：02-22 02:29

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework
{
    [Serializable]
    public sealed class BuffSerializeParam
    {
        [BoxGroup("参数信息"), HorizontalGroup("参数信息/基础"), LabelText("参数名"), ReadOnly]
        public string ParamName;

        [HorizontalGroup("参数信息/基础"), LabelText("参数类型"), ReadOnly]
        public string ParamType;

        [HorizontalGroup("参数信息/中间件"), LabelText("自定义取值器"), ShowIf("@!string.IsNullOrEmpty(ValueGetterName)"), ReadOnly]
        public string ValueGetterName;

        [HorizontalGroup("参数信息/中间件"), LabelText("自定义校验器"), ShowIf("@!string.IsNullOrEmpty(ValidatorName)"), ReadOnly]
        public string ValidatorName;

        [HorizontalGroup("参数信息/中间件"), LabelText("自定义分析器"), ShowIf("@!string.IsNullOrEmpty(ParserName)"), ReadOnly]
        public string ParserName;

        [ValidateInput("ValidateInput"), LabelText("参数值"),
         ShowIf("@!ParamType.EndsWith(\"List__\") && string.IsNullOrEmpty(ValueGetterName)")]
        public string ParamValue;

        [LabelText("参数值"), ValidateInput("ValidateInputList"),
         ShowIf("@ParamType.EndsWith(\"List__\") && string.IsNullOrEmpty(ValueGetterName)")]
        public List<string> ListValue;

        [ValidateInput("ValidateInputList"),
         ValueDropdown("@EggFramework.BuffUtil.GetValueGetterByName(ValueGetterName)"),
         LabelText("参数值"),
         ShowIf("@ParamType.EndsWith(\"List__\") && !string.IsNullOrEmpty(ValueGetterName)")]
        public List<string> CustomListValue;

        [ValidateInput("ValidateInput"), ValueDropdown("@EggFramework.BuffUtil.GetValueGetterByName(ValueGetterName)"),
         ShowIf("@!ParamType.EndsWith(\"List__\") && !string.IsNullOrEmpty(ValueGetterName)"), LabelText("参数值")]
        public string CustomValue;

#if UNITY_EDITOR

        private bool ValidateInputList(List<string> value, ref string errMsg)
        {
            if (value == null) return true;
            for (var i = 0; i < value.Count; i++)
            {
                var se     = value[i];
                var result = ValidateInput(se, ref errMsg);
                if (result) continue;
                errMsg = "索引为" + i + "的数据未通过校验，错误信息为：" + errMsg;
                return false;
            }

            return true;
        }


        private bool ValidateInput(string value, ref string errMsg)
        {
            if (!string.IsNullOrEmpty(ValidatorName))
            {
                return BuffUtil.ValidateByValueGetter(ValidatorName, value, ref errMsg);
            }

            switch (ParamType)
            {
                case "Int32" or "Int32List__":
                {
                    errMsg = "类型不匹配";
                    return int.TryParse(value, out _);
                }
                case "Single" or "SingleList__":
                {
                    errMsg = "类型不匹配";
                    return float.TryParse(value, out _);
                }
                case "Boolean" or "BooleanList__":
                {
                    errMsg = "布尔值必须为0或1";
                    return value is "1" or "0";
                }
                case "String" or "StringList__":
                {
                    return true;
                }
                default:
                {
                    errMsg = string.IsNullOrEmpty(ParserName) ? "不支持的类型" : "自定义分析器需要搭配自定义校验器使用，否则有风险";
                    return false;
                }
            }
        }
#endif
    }
}