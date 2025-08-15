#region

//文件创建者：Egg
//创建时间：05-02 08:49

#endregion

#if UNITY_EDITOR
namespace EggFramework.Util.EggCMD
{
    [CommandHandle("countCodeLine")]
    [CommandComment("计算代码行数")]
    public sealed class CountCodeLineHandle : CommandHandle
    {
        protected override void Handle() =>  CommandLogger.Log(PythonUtil.CountCodeLine());
        
    }
}
#endif