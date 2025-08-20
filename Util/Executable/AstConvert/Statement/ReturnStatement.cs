#region

//文件创建者：Egg
//创建时间：08-20 07:46

#endregion

namespace EggFramework.Executable
{
    public sealed class ReturnStatement : Statement
    {
        public Expression ReturnValue { get; }

        public ReturnStatement(Expression returnValue)
        {
            ReturnValue = returnValue;
        }
    }
}