#region

//文件创建者：Egg
//创建时间：05-02 10:53

#endregion

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("print")]
    public sealed class PrintHandle : CommandHandle<string>
    {
        protected override void Handle(
            [CommandComment("打印文字")] string p1)
        {
            CommandLogger.Log(p1);
        }
    }
}