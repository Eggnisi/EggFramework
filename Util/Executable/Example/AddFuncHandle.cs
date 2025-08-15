#region

//文件创建者：Egg
//创建时间：07-24 03:50

#endregion

namespace EggFramework.Executable
{
    [FuncHandle("add")]
    public sealed class AddFuncHandle : FuncHandle<int, int, int>
    {
        protected override int Handle(int p1, int p2) => p1 + p2;
    }
}