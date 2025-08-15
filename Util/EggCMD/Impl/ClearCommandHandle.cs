#region

//文件创建者：Egg
//创建时间：03-26 02:33

#endregion

#if UNITY_EDITOR


using UnityEditor;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("clear")]
    [CommandComment("清除屏幕")]
    public sealed class ClearCommandHandle : CommandHandle
    {
        protected override void Handle()
        {
            if (CommandEditorWindow.Enable)
            {
                var window = EditorWindow.GetWindow<CommandEditorWindow>();
                window.ClearString();
            }
        }
    }
}
#endif