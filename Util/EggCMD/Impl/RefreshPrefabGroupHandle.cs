#region

//文件创建者：Egg
//创建时间：03-27 08:53

#endregion

#if UNITY_EDITOR


using EggFramework.Util.Res;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("refresh")]
    [CommandTokenSource(typeof(RefreshTokenSource))]
    public sealed class RefreshPrefabGroupHandle : CommandHandle<string>
    {
        protected override void Handle(
            [CommandComment("刷新目标")] string p1)
        {
            switch (p1)
            {
                case "prefabGroup":
                    ResEditorWindowView.RefreshPrefabData();
                    break;
                case "res":
                    ResEditorWindowView.Refresh();
                    break;
            }
        }
    }
}
#endif