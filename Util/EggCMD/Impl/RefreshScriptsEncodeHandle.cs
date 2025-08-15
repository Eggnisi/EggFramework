#region

//文件创建者：Egg
//创建时间：06-27 01:01

#endregion

#if UNITY_EDITOR


namespace EggFramework.Util.EggCMD
{
    [CommandHandle("refreshScriptsEncode")]
    public sealed class RefreshScriptsEncodeHandle : CommandHandle
    {
        protected override void Handle() => PythonUtil.RefreshScriptsEncode();
    }
}
#endif