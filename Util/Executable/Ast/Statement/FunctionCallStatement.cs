namespace EggFramework.Executable
{
    // 函数调用语句（如 attack(2)）
    public sealed class FunctionCallStatement : Statement
    {
        public FunctionCallExpression CallExpression { get; }
        public FunctionCallStatement(FunctionCallExpression call) => CallExpression = call;
    }
}