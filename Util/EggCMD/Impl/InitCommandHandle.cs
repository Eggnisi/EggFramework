#region

//文件创建者：Egg
//创建时间：03-26 03:00

#endregion

#if UNITY_EDITOR

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("init")]
    [CommandComment("初始化项目代码(慎用)")]
    [EnableOption("-f", "不提醒，直接初始化")]
    public sealed class InitCommandHandle : CommandHandle<string>
    {
        protected override void Handle([CommandComment("命名空间")] string nameSpace)
        {
            var msg = string.Empty;
            ScriptInitUtil.InitProject(nameSpace, !_context.OptionExist("-f"), ref msg);
            CommandLogger.Log(msg);
        }
    }
}
#endif