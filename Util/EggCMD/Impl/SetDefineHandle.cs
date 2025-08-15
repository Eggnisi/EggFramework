#region

//文件创建者：Egg
//创建时间：05-06 10:37

#endregion

#if UNITY_EDITOR

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("setDefine")]
    [CommandComment("设置脚本定义符号")]
    [CommandTokenSource(typeof(DefineTokenSource))]
    [EnableOption("-enable", "是否启用")]
    public sealed class SetDefineHandle : CommandHandle<string>
    {
        protected override void Handle([CommandComment("脚本定义符号")] string p1)
        {
            var enable = !_context.OptionExist("-enable") || _context.GetFirstOptionValue("-enable") == "true";
            DefineUtil.SetDefineActiveForCurrentTargetGroup(p1, enable);
        }
    }
}
#endif