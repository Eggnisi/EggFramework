#region

// 作者： Egg
// 时间： 2025 07.21 11:11

#endregion

namespace EggFramework.Executable
{
        
    // 表达式语句
    public class ExpressionStatement : Statement
    {
        public Expression Expression { get; }

        public ExpressionStatement(Expression expression)
        {
            Expression = expression;
        }
    }
}