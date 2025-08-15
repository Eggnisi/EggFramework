#region

// 作者： Egg
// 时间： 2025 07.19 06:47

#endregion

namespace EggFramework.Executable
{
    // 常量表达式（支持int, float, string）
    public sealed class ConstantExpression : Expression
    {
        public object Value { get; } // 实际存储值
        public ConstantExpression(object value) => Value = value;
    }
}