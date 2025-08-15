#region

// 作者： Egg
// 时间： 2025 07.21 11:11

#endregion

namespace EggFramework.Executable
{
    // 赋值表达式
    public class AssignmentExpression : Expression
    {
        public Expression Left { get; }
        public Expression Right { get; }

        public AssignmentExpression(Expression left, Expression right)
        {
            Left = left;
            Right = right;
        }
    }
}