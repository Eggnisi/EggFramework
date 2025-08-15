#region

// 作者： Egg
// 时间： 2025 07.19 06:46

#endregion

namespace EggFramework.Executable
{
    // 变量表达式（如 $health）
    public sealed class VariableExpression : Expression
    {
        public string VariableName { get; } // 去掉$前缀的变量名
        public VariableExpression(string name) => VariableName = name;
    }
}