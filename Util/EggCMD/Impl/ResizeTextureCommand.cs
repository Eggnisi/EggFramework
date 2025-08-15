#region

//文件创建者：Egg
//创建时间：05-23 05:18

#endregion

#if UNITY_EDITOR

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("resizeTexture")]
    [CommandComment("批量调整纹理大小")]
    public sealed class ResizeTextureCommand : CommandHandle<string>
    {
        protected override void Handle([CommandComment("指定路径")] string p1)
        {
            PythonUtil.ResizeTextures(p1);
        }
    }
}
#endif