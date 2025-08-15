#region

//文件创建者：Egg
//创建时间：11-01 10:39

#endregion

using UnityEngine;

namespace EggFramework.Modules.UI
{
    public interface IUIFormLogic
    {
        GameObject Inst { get; }
        void OnInit();
        void OnRecycle();
        void OnOpen();
        void OnClose();
        void OnCover();
        void OnReveal();
        void OnPause();
        void OnResume();
        void OnUpdate();
    }
}