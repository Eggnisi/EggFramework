#region

// 作者： Egg
// 时间： 2025 07.19 06:46

#endregion

using System.Collections.Generic;

namespace EggFramework.Executable
{
    // 语句块（用于支持多条语句，可扩展性设计）
    public sealed class BlockStatement : Statement
    {
        public List<Statement> Statements { get; } = new();
        public BlockStatement(IEnumerable<Statement> statements) => Statements.AddRange(statements);
    }
}