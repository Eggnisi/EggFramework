#region

// 作者： Egg
// 时间： 2025 07.19 06:45

#endregion

namespace EggFramework.Executable
{
    // 条件语句（if-else结构）
    public sealed class BranchStatement : Statement
    {
        public Expression Condition { get; } // 条件表达式
        public Statement TrueBranch { get; } // 条件为真时执行的语句
        public Statement FalseBranch { get; } // 条件为假时执行的语句（可为null）

        public BranchStatement(Expression condition, Statement trueBranch, Statement falseBranch = null)
            => (Condition, TrueBranch, FalseBranch) = (condition, trueBranch, falseBranch);
    }
}