#region

// 作者： Egg
// 时间： 2025 07.19 09:25

#endregion

namespace EggFramework
{
    [FuncHandle("add")]
    public sealed class AddFuncHandle : FuncHandle<int,int,int>
    {
        protected override int Handle(int p1, int p2)
        {
            return p1 + p2;
        }
    }
}