#region

//文件创建者：Egg
//创建时间：02-22 02:50

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework
{
    [Serializable]
    public sealed class BuffSerializeCallback
    {
        [ValueDropdown("@EggFramework.BuffUtil.GetBuffCallbackNames()"), OnValueChanged("Refresh"),
         LabelText("@GetChineseName(CallbackName)")]
        public string CallbackName;

        [InfoBox("对于bool值，0为false，1为true")]
        [ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true)]
        [LabelText("回调参数")]
        public List<BuffSerializeParam> Params;

        private void Refresh()
        {
            Params = BuffUtil.GetSerializeParamsByHandleName(CallbackName);
        }

        private string GetChineseName(string callbackName)
        {
            if (string.IsNullOrEmpty(callbackName)) return "未指定";
            var type  = BuffUtil.GetHandleTypeByHandleName(callbackName);
            var attrs = type.GetCustomAttributes(typeof(LabelTextAttribute), false);
            if (attrs.Length > 0) return ((LabelTextAttribute)attrs[0]).Text;
            return "未命名回调处理器";
        }

        public void Apply(BuffRunTimeInfo buffRunTimeInfo, params object[] paramList)
        {
            BuffUtil.HandleBuff(buffRunTimeInfo, this, paramList);
        }
    }
}