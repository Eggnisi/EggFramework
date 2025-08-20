#region

// 作者： Egg
// 时间： 2025 07.19 06:47

#endregion

using System.Collections.Generic;

namespace EggFramework.Executable
{
    // 函数调用表达式（如 has_buff("flame")）
    public sealed class FunctionCallExpression : Expression
    {
        public Expression  FunctionName { get; }
        public List<Expression> Arguments { get; } // 参数列表（支持任意数量）

        public FunctionCallExpression(Expression  name, List<Expression> args)
            => (FunctionName, Arguments) = (name, args);
    }
}