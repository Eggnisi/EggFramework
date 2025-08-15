#region

// 作者： Egg
// 时间： 2025 07.19 06:58

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace EggFramework.Executable
{
    public class ScriptParser : IScriptParser
    {
        private          string       _input;
        private          int          _position;
        private readonly List<string> _tokens = new();

        // 词法分析：将输入字符串分解为token
        private void Tokenize()
        {
            _tokens.Clear();
            var token = new StringBuilder();
            bool inString = false;
            bool inComment = false;

            for (int i = 0; i < _input.Length; i++)
            {
                char c = _input[i];

                // 处理注释
                if (c == '/' && i + 1 < _input.Length && _input[i + 1] == '/')
                {
                    inComment = true;
                    continue;
                }

                if (inComment && c == '\n')
                {
                    inComment = false;
                    continue;
                }

                if (inComment) continue;

                // 处理字符串
                if (c == '"')
                {
                    inString = !inString;
                    token.Append(c);
                    continue;
                }

                // 分隔符处理 - 添加对 '.' 和 '[' ']' 的支持
                if (!inString && (IsDelimiter(c) || char.IsWhiteSpace(c)))
                {
                    if (token.Length > 0)
                    {
                        _tokens.Add(token.ToString());
                        token.Clear();
                    }

                    if (!IsDelimiter(c) || char.IsWhiteSpace(c)) continue;
                    // 处理双字符运算符 (&&, ||, ==, !=)
                    if (i + 1 < _input.Length && IsDoubleCharOperator(c, _input[i + 1]))
                    {
                        _tokens.Add("" + c + _input[i + 1]);
                        i++; // 跳过下一个字符
                    }
                    else
                    {
                        // 添加对数组访问的支持
                        if (c is '[' or ']')
                        {
                            _tokens.Add(c.ToString());
                        }
                        else
                        {
                            _tokens.Add(c.ToString());
                        }
                    }
                }
                else
                {
                    token.Append(c);
                }
            }

            if (token.Length > 0) _tokens.Add(token.ToString());
        }

        private static bool IsDelimiter(char c) => "(){};,$&|!=><.[]".Contains(c);

        private static bool IsDoubleCharOperator(char c1, char c2) =>
            (c1 == '&' && c2 == '&') || (c1 == '|' && c2 == '|') ||
            (c1 == '=' && c2 == '=') || (c1 == '!' && c2 == '=');
        
        public Statement ParseStatement(string input)
        {
            _input = $"{{{input}}}";
            Tokenize();
            if (_tokens.Count == 0) return null;
            _position = 0;
            return InnerParseStatement();
        }
        
        public Expression ParseExpression(string input)
        {
            _input = input;
            Tokenize();
            if (_tokens.Count == 0) return null;
            _position = 0;
            return ParseExpression();
        }

        private Statement InnerParseStatement()
        {
            if (PeekToken() == "if") return ParseBranch();
            if (PeekToken() == "{") return ParseBlock();
            if (PeekToken() == "var") return ParseAssignment();
            return ParseExpressionStatement();
        }

        // 新增：解析赋值语句
        private AssignmentStatement ParseAssignment()
        {
            ExpectToken("var");
            ExpectToken("$");
            string variableName = NextToken();
            ExpectToken("=");
            Expression expr = ParseExpression();
            if (PeekToken() == ";") ExpectToken(";");
            return new AssignmentStatement(variableName, expr);
        }

        // 新增：解析表达式语句
        private ExpressionStatement ParseExpressionStatement()
        {
            Expression expr = ParseExpression();
            if (PeekToken() == ";") ExpectToken(";");
            return new ExpressionStatement(expr);
        }

        private BranchStatement ParseBranch()
        {
            ExpectToken("if");
            ExpectToken("(");
            Expression condition = ParseExpression();
            ExpectToken(")");
            Statement trueBranch = ParseBlock();

            Statement falseBranch = null;
            if (PeekToken() == "else")
            {
                ExpectToken("else");
                falseBranch = ParseBlock();
            }

            return new BranchStatement(condition, trueBranch, falseBranch);
        }

        private BlockStatement ParseBlock()
        {
            var statements = new List<Statement>();
            ExpectToken("{");
            while (PeekToken() != "}" && !IsEnd())
            {
                statements.Add(InnerParseStatement());
                if (PeekToken() == ";") ExpectToken(";");
            }

            if (PeekToken() == "}") ExpectToken("}");
            return new BlockStatement(statements);
        }

        // 更新表达式解析流程
        private Expression ParseExpression() => ParseAssignmentExpression();

        // 新增：解析赋值表达式
        private Expression ParseAssignmentExpression()
        {
            // 先解析左侧表达式（可能是变量或成员访问）
            Expression left = ParseLogicalExpression();

            // 检查是否有赋值操作
            if (PeekToken() == "=")
            {
                ExpectToken("=");
                Expression right = ParseAssignmentExpression();
                return new AssignmentExpression(left, right);
            }

            return left;
        }

        // 更新：基本表达式解析
        private Expression ParsePrimaryExpression()
        {
            string token = PeekToken();

            if (token == "(")
            {
                ExpectToken("(");
                Expression expr = ParseExpression();
                ExpectToken(")");
                return expr;
            }

            if (token.StartsWith("$"))
            {
                // 跳过$
                NextToken();
                return ParseIdentifierExpression();
            }

            if (token.StartsWith("\""))
            {
                return new ConstantExpression(NextToken().Trim('"'));
            }

            if (double.TryParse(token, out double num))
            {
                NextToken();
                return new ConstantExpression(Convert.ChangeType(num,
                    num % 1 == 0 ? typeof(int) : typeof(float)));
            }

            if (char.IsLetter(token[0]))
            {
                var next = NextToken();
                if (PeekToken() == "(")
                    return ParseFunctionCall(new ConstantExpression(next));
            }

            throw new Exception($"Unexpected token: {token}");
        }

        // 新增：解析标识符表达式（可能是变量、函数调用或成员访问）
        private Expression ParseIdentifierExpression()
        {
            Expression expr = new VariableExpression(NextToken());

            // 处理后续的点访问或数组访问
            while (true)
            {
                if (PeekToken() == ".")
                {
                    ExpectToken(".");
                    string memberName = NextToken();
                    expr = new MemberAccessExpression(expr, memberName);
                }
                else if (PeekToken() == "[")
                {
                    ExpectToken("[");
                    Expression index = ParseExpression();
                    ExpectToken("]");
                    expr = new ArrayAccessExpression(expr, index);
                }
                else if (PeekToken() == "(")
                {
                    // 函数调用
                    expr = ParseFunctionCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        // 新增：解析函数调用（基于已有表达式）
        private FunctionCallExpression ParseFunctionCall(Expression target)
        {
            ExpectToken("(");
            var args = new List<Expression>();

            while (PeekToken() != ")")
            {
                args.Add(ParseExpression());
                if (PeekToken() == ",") ExpectToken(",");
            }

            ExpectToken(")");
            return new FunctionCallExpression(target, args);
        }

        private static bool IsRelationalOperator(string token) =>
            new List<string>() { ">", "<", ">=", "<=", "==", "!=" }.Contains(token);

        private string PeekToken() => _position < _tokens.Count ? _tokens[_position] : null;
        private string NextToken() => _position < _tokens.Count ? _tokens[_position++] : null;
        private bool IsEnd() => _position >= _tokens.Count;

        private void ExpectToken(string expected)
        {
            var actual = NextToken();
            if (actual != expected)
                throw new Exception($"Expected '{expected}', got '{actual}'");
        }

        private Expression ParseLogicalExpression()
        {
            // 先解析优先级较高的 AND 表达式
            Expression left = ParseLogicalAndExpression();

            // 处理 OR 表达式（优先级较低）
            while (PeekToken() == "||")
            {
                string op = NextToken(); // 消耗 "||"
                Expression right = ParseLogicalAndExpression(); // 解析右侧的 AND 表达式
                left = new BinaryOperationExpression(left, op, right);
            }

            return left;
        }

        private Expression ParseLogicalAndExpression()
        {
            // 先解析基础关系表达式
            Expression left = ParseRelationalExpression();

            // 处理 AND 表达式（优先级较高）
            while (PeekToken() == "&&")
            {
                string op = NextToken(); // 消耗 "&&"
                Expression right = ParseRelationalExpression(); // 解析右侧的关系表达式
                left = new BinaryOperationExpression(left, op, right);
            }

            return left;
        }

        // 关系表达式（>, <, >=, <=, ==, !=）
        private Expression ParseRelationalExpression()
        {
            Expression left = ParseAdditiveExpression();
            if (IsRelationalOperator(PeekToken()))
            {
                string op = NextToken();
                Expression right = ParseAdditiveExpression();
                return new BinaryOperationExpression(left, op, right);
            }

            return left;
        }

        // 加减表达式（+, -）
        private Expression ParseAdditiveExpression()
        {
            Expression left = ParseMultiplicativeExpression();
            while (PeekToken() == "+" || PeekToken() == "-")
            {
                string op = NextToken();
                Expression right = ParseMultiplicativeExpression();
                left = new BinaryOperationExpression(left, op, right);
            }

            return left;
        }

        // 乘除模表达式（*, /, %）
        private Expression ParseMultiplicativeExpression()
        {
            Expression left = ParseUnaryExpression();
            while (PeekToken() == "*" || PeekToken() == "/" || PeekToken() == "%")
            {
                string op = NextToken();
                Expression right = ParseUnaryExpression();
                left = new BinaryOperationExpression(left, op, right);
            }

            return left;
        }

        // 一元表达式（负号）
        private Expression ParseUnaryExpression()
        {
            if (PeekToken() == "-")
            {
                string op = NextToken();
                Expression operand = ParsePrimaryExpression();
                return new UnaryOperationExpression(op, operand);
            }

            return ParsePrimaryExpression();
        }
    }
}