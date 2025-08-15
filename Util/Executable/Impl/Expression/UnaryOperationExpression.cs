namespace EggFramework.Executable
{
    public class UnaryOperationExpression : Expression
    {
        public string Operator { get; } 
        public Expression Operand { get; }
        public UnaryOperationExpression(string op, Expression operand)
            => (Operator, Operand) = (op, operand);
    }
}