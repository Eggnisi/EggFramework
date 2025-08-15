#region

//文件创建者：Egg
//创建时间：11-01 12:09

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace EggFramework.Modules.UI
{
    public interface IUISystem
    {
        GameObject RequestUIInst(string assetName);
        
        IUIForm OpenUIForm(string formName);

        IUIForm CloseUIForm(string formName);

        IUIForm GetUIForm(string formName);

        UIGroup GetUIGroup(string groupName);

        bool HasUIGroup(string groupName);

        IEnumerable<UIGroup> GetUIGroups();
        void RefocusUIForm(IUIForm uiForm);
    }
}