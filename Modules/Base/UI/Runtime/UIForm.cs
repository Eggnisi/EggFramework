#region

//文件创建者：Egg
//创建时间：a

#endregion

using EggFramework.Modules.UI;
using QFramework;
using UnityEngine;

namespace EggFramework.Modules.UI
{
    public abstract class UIForm : IUIForm
    {
        public string Name { get; }

        public IUIFormLogic UIFormLogic { get; }

        protected IUISystem _uiSystem;

        protected UIForm(IUISystem uiSystem, string name, string assetName, IUIFormLogic uiFormLogic)
        {
            _uiSystem   = uiSystem;
            UIFormLogic = uiFormLogic;
            Name        = name;
            AssetName   = assetName;
        }

        public string AssetName { get; }

        public void OnInit()
        {
            UIFormLogic.OnInit();
        }

        public void OnRecycle()
        {
            UIFormLogic.OnRecycle();
        }

        public void OnOpen()
        {
            if (UIFormLogic.Inst == null)
            {
                UIFormLogic.OnInit();
            }

            UIFormLogic.OnOpen();
        }

        public void OnClose()
        {
            UIFormLogic.OnClose();
        }

        public void OnCover()
        {
            UIFormLogic.OnCover();
        }

        public void OnReveal()
        {
            UIFormLogic.OnReveal();
        }

        public void OnPause()
        {
            UIFormLogic.OnPause();
        }

        public void OnResume()
        {
            UIFormLogic.OnResume();
        }

        public void OnUpdate()
        {
            UIFormLogic.OnUpdate();
        }
    }
}