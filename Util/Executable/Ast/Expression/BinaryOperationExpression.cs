#region

// 作者： Egg
// 时间： 2025 07.19 06:51

#endregion

namespace EggFramework.Executable
{
    // 二元运算表达式（如 health > 8）
    public sealed class BinaryOperationExpression : Expression
    {
        public Expression Left { get; }
        public string Operator { get; } // 支持：">", "<", ">=", "<=", "==", "!=", "&&", "||"
        public Expression Right { get; }

        public BinaryOperationExpression(Expression left, string op, Expression right)
            => (Left, Operator, Right) = (left, op, right);
    }
}