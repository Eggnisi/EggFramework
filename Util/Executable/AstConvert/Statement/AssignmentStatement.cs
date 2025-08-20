#region

// 作者： Egg
// 时间： 2025 07.21 11:13

#endregion

namespace EggFramework.Executable
{
    // 赋值语句
    public class AssignmentStatement : Statement
    {
        public string VariableName { get; }
        public Expression Expression { get; }

        public AssignmentStatement(string variableName, Expression expression)
        {
            VariableName = variableName;
            Expression = expression;
        }
    }
}