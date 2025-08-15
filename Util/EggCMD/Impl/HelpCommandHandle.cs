#region

//文件创建者：Egg
//创建时间：03-26 03:09

#endregion

#if UNITY_EDITOR


namespace EggFramework.Util.EggCMD
{
    [CommandHandle("help")]
    [CommandComment("显示帮助信息")]
    [CommandTokenSource(typeof(HelpTokenSource))]
    public sealed class HelpCommandHandle : CommandHandle<string>
    {
        [CommandComment(                               "展示指定命令的帮助信息")]
        protected override void Handle([CommandComment("命令名称")] string p1)
        {
            var helper = CommandHelper.GetHelper(p1);
            if (helper != null)
            {
                CommandLogger.Log(CommandHelper.GetFormatDescription(helper));
            }
            else CommandLogger.LogError($"Invalid command name: {p1}");
        }
    }

    [CommandHandle("help")]
    public sealed class HelpCommandHandle1 : CommandHandle
    {
        protected override void Handle()
        {
            var helpers = CommandHelper.GetAllHelpers();
            foreach (var helper in helpers)
            {
                CommandLogger.Log(CommandHelper.GetFormatDescription(helper));
            }
        }
    }
}
#endif