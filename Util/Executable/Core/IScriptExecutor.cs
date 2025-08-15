#region

//文件创建者：Egg
//创建时间：07-26 06:57

#endregion

using QFramework;

namespace EggFramework.Executable
{
    public interface IScriptExecutor : IUtility
    {
        void ExecuteStatement(Statement stmt, ExecutionContext context);
        void ExecuteStatement(string stmt, ExecutionContext context);
        object EvaluateExpression(Expression expr, ExecutionContext context);
        object EvaluateExpression(string expr, ExecutionContext context);
    }
}