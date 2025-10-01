#region

// 作者： Egg
// 时间： 2025 07.21 11:13

#endregion

namespace EggFramework.Executable
{
    // 数组访问表达式
    public class ArrayAccessExpression : Expression
    {
        public Expression Array { get; }
        public Expression Index { get; }

        public ArrayAccessExpression(Expression array, Expression index)
        {
            Array = array;
            Index = index;
        }
    }
}