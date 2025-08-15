#region

//文件创建者：Egg
//创建时间：07-26 07:07

#endregion

using QFramework;

namespace EggFramework.Executable
{
    public interface IScriptParser : IUtility
    {
        Statement ParseStatement(string input);
        Expression ParseExpression(string input);
    }
}