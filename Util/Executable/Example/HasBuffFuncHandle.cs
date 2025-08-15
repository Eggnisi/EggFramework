#region

//文件创建者：Egg
//创建时间：07-24 03:49

#endregion

namespace EggFramework.Executable
{
    [FuncHandle("has_buff")]
    public sealed class HasBuffFuncHandle : FuncHandle<string, bool>
    {
        protected override bool Handle(string p1) => p1 == "flame";
    }
}