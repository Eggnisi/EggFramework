#region

// 作者： Egg
// 时间： 2025 07.21 11:12

#endregion

namespace EggFramework.Executable
{
    // 成员访问表达式
    public class MemberAccessExpression : Expression
    {
        public Expression Object { get; }
        public string MemberName { get; }

        public MemberAccessExpression(Expression obj, string memberName)
        {
            Object = obj;
            MemberName = memberName;
        }
    }
}