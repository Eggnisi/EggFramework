#region

// 作者： Egg
// 时间： 2025 07.19 07:32

#endregion

using System;
using System.Collections;
using System.Linq;
using EggFramework.Util;

namespace EggFramework.Executable
{
    public sealed class ScriptExecutor : IScriptExecutor
    {
        private readonly ScriptParser _parser = new();
        public void ExecuteStatement(Statement stmt, ExecutionContext context)
        {
            if (stmt is FunctionCallStatement funcCall)
            {
                EvaluateFunctionCall(funcCall.CallExpression, context);
            }
            else if (stmt is BranchStatement branch)
            {
                bool condition = Convert.ToBoolean(EvaluateExpression(branch.Condition, context));
                if (condition)
                    ExecuteStatement(branch.TrueBranch, context);
                else if (branch.FalseBranch != null)
                    ExecuteStatement(branch.FalseBranch, context);
            }
            else if (stmt is BlockStatement block)
            {
                foreach (var subStmt in block.Statements)
                    ExecuteStatement(subStmt, context);
            }

            if (stmt is AssignmentStatement assignment)
            {
                object value = EvaluateExpression(assignment.Expression, context);
                context.Variables[assignment.VariableName] = value;
            }

            if (stmt is ExpressionStatement exprStmt)
            {
                EvaluateExpression(exprStmt.Expression, context);
            }
        }

        public void ExecuteStatement(string stmt, ExecutionContext context)
        {
            var ast = _parser.ParseStatement(stmt);
            if (ast == null)
                throw new Exception("Failed to parse statement: " + stmt);
            ExecuteStatement(ast, context);
        }

        public object EvaluateExpression(Expression expr, ExecutionContext context)
        {
            if (expr is ConstantExpression constExpr)
                return constExpr.Value;

            if (expr is VariableExpression varExpr)
            {
                if (!context.Variables.TryGetValue(varExpr.VariableName, out var value))
                    throw new Exception($"Undefined variable: ${varExpr.VariableName}");
                return value;
            }

            if (expr is ArrayAccessExpression arrExpr)
            {
                return ((IList)EvaluateExpression(arrExpr.Array, context))[
                    (int)EvaluateExpression(arrExpr.Index, context)];
            }

            if (expr is MemberAccessExpression memberExpr)
            {
                return EvaluateExpression(memberExpr.Object, context)
                    .GetMemberValue(VariableUtil.SnakeCaseToPascalCase(memberExpr.MemberName));
            }

            if (expr is FunctionCallExpression funcExpr)
                return EvaluateFunctionCall(funcExpr, context);

            if (expr is UnaryOperationExpression unaryOp)
            {
                object operand = EvaluateExpression(unaryOp.Operand, context);

                if (unaryOp.Operator == "-")
                {
                    if (operand is int i) return -i;
                    if (operand is float f) return -f;
                    throw new Exception($"Unary '-' operator requires numeric operand, got {operand?.GetType().Name}");
                }

                throw new Exception($"Unsupported unary operator: {unaryOp.Operator}");
            }

            if (expr is BinaryOperationExpression binOp)
            {
                object left  = EvaluateExpression(binOp.Left,  context);
                object right = EvaluateExpression(binOp.Right, context);

                switch (binOp.Operator)
                {
                    // 逻辑运算符 (只支持布尔类型)
                    case "&&":
                    case "||":
                        if (left is bool leftBool && right is bool rightBool)
                        {
                            return binOp.Operator == "&&"
                                ? leftBool && rightBool
                                : leftBool || rightBool;
                        }

                        throw new Exception($"Logical operator '{binOp.Operator}' requires boolean operands");

                    // 比较运算符 (支持数值和字符串)
                    case ">":
                    case "<":
                    case ">=":
                    case "<=":
                        return CompareValues(left, right, binOp.Operator);

                    // 相等性比较 (支持所有类型)
                    case "==":
                        return AreEqual(left, right);
                    case "!=":
                        return !AreEqual(left, right);

                    // 算术运算符 (只支持数值类型)
                    case "+":
                        return AddValues(left, right);
                    case "-":
                        return SubtractValues(left, right);
                    case "*":
                        return MultiplyValues(left, right);
                    case "/":
                        return DivideValues(left, right);
                    case "%":
                        return ModuloValues(left, right);

                    default:
                        throw new Exception($"Unsupported operator: {binOp.Operator}");
                }
            }

            throw new Exception($"Unknown expression type: {expr.GetType().Name}");
        }

        public object EvaluateExpression(string expr, ExecutionContext context)
        {
            var ast = _parser.ParseExpression(expr);
            if (ast == null)
                throw new Exception("Failed to parse expression: " + expr);
            return EvaluateExpression(ast, context);
        }

        // 辅助方法：比较两个值
        private static bool CompareValues(object left, object right, string op)
        {
            // 字符串比较
            if (left is string leftStr && right is string rightStr)
            {
                int comparison = string.Compare(leftStr, rightStr, StringComparison.Ordinal);
                return op switch
                {
                    ">"  => comparison > 0,
                    "<"  => comparison < 0,
                    ">=" => comparison >= 0,
                    "<=" => comparison <= 0,
                    _    => throw new Exception($"Unsupported comparison operator for strings: {op}")
                };
            }

            // 数值比较
            if (TryGetNumericValues(left, right, out double leftNum, out double rightNum))
            {
                return op switch
                {
                    ">"  => leftNum > rightNum,
                    "<"  => leftNum < rightNum,
                    ">=" => leftNum >= rightNum,
                    "<=" => leftNum <= rightNum,
                    _    => throw new Exception($"Unsupported comparison operator: {op}")
                };
            }

            throw new Exception($"Comparison operator '{op}' requires numeric or string operands");
        }

        // 辅助方法：检查相等性
        private static bool AreEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null || right == null) return false;

            // 数值类型特殊处理 (允许 int 和 float 比较)
            if (TryGetNumericValues(left, right, out double leftNum, out double rightNum))
            {
                return Math.Abs(leftNum - rightNum) < double.Epsilon;
            }

            return left.Equals(right);
        }

        // 辅助方法：加法
        private static object AddValues(object left, object right)
        {
            // 字符串拼接
            if (left is string || right is string)
                return $"{left}{right}";

            // 数值加法
            if (!TryGetNumericValues(left, right, out double leftNum, out double rightNum))
                throw new Exception($"Addition requires numeric or string operands");
            // 如果都是整数，返回整数
            if (left is int && right is int)
                return (int)(leftNum + rightNum);

            // 否则返回浮点数
            return (float)(leftNum + rightNum);
        }

        // 辅助方法：减法
        private static object SubtractValues(object left, object right)
        {
            if (!TryGetNumericValues(left, right, out double leftNum, out double rightNum))
                throw new Exception($"Subtraction requires numeric operands");
            if (left is int && right is int)
                return (int)(leftNum - rightNum);

            return (float)(leftNum - rightNum);
        }

        // 辅助方法：乘法
        private static object MultiplyValues(object left, object right)
        {
            if (!TryGetNumericValues(left, right, out double leftNum, out double rightNum))
                throw new Exception($"Multiplication requires numeric operands");
            if (left is int && right is int)
                return (int)(leftNum * rightNum);

            return (float)(leftNum * rightNum);
        }

        // 辅助方法：除法
        private static object DivideValues(object left, object right)
        {
            if (!TryGetNumericValues(left, right, out double leftNum, out double rightNum))
                throw new Exception($"Division requires numeric operands");
            if (Math.Abs(rightNum) < double.Epsilon)
                throw new DivideByZeroException("Division by zero");

            // 除法总是返回浮点数
            return (float)(leftNum / rightNum);
        }

        // 辅助方法：取模
        private static object ModuloValues(object left, object right)
        {
            if (!TryGetNumericValues(left, right, out double leftNum, out double rightNum))
                throw new Exception($"Modulo requires numeric operands");
            if (Math.Abs(rightNum) < double.Epsilon)
                throw new DivideByZeroException("Modulo by zero");

            // 如果都是整数，使用整数取模
            if (left is int && right is int)
                return (int)leftNum % (int)rightNum;

            // 否则使用浮点数取模
            return (float)(leftNum % rightNum);
        }

        // 辅助方法：尝试转换为数值
        private static bool TryGetNumericValues(object left, object right, out double leftNum, out double rightNum)
        {
            leftNum  = 0;
            rightNum = 0;

            try
            {
                leftNum  = Convert.ToDouble(left);
                rightNum = Convert.ToDouble(right);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private object EvaluateFunctionCall(FunctionCallExpression funcCall, ExecutionContext context)
        {
            var args = funcCall.Arguments
                .Select(arg => EvaluateExpression(arg, context))
                .ToArray();
            if (funcCall.FunctionName is ConstantExpression constantExpr)
            {
                return context.Functions[(string)EvaluateExpression(constantExpr, context)](args);
            }

            if (funcCall.FunctionName is MemberAccessExpression memberAccessExpr)
            {
                return EvaluateExpression(memberAccessExpr.Object, context)
                    .InvokeMemberFunc(VariableUtil.SnakeCaseToPascalCase(memberAccessExpr.MemberName), args);
            }

            return null;
        }
    }
}