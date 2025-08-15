#region

//文件创建者：Egg
//创建时间：10-31 08:07

#endregion

using QFramework;

namespace EggFramework.Modules.UI
{
    public interface IUIForm
    {
        string Name { get; }
        string AssetName { get; }
        IUIFormLogic UIFormLogic { get; }
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