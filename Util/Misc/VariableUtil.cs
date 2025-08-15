#region

//文件创建者：Egg
//创建时间：10-09 11:04

#endregion

using System.Text;

namespace EggFramework.Util
{
    public static class VariableUtil
    {
        public static string PascalCase2BIG_SNAKE_CASE(string pascal)
        {
            var builder     = new StringBuilder();
            var lastIsUpper = false;
            for (var index = 0; index < pascal.Length; index++)
            {
                var c = pascal[index];
                
                if (c == '-')
                {
                    builder.Append('_');
                    continue;
                }
                
                if (index == 0)
                {
                    lastIsUpper = false;
                    builder.Append(char.ToUpper(c));
                    continue;
                }

                if (char.IsUpper(c))
                {
                    if (!lastIsUpper)
                        builder.Append("_");
                    builder.Append(c);
                    lastIsUpper = true;
                    continue;
                }

                if (char.IsLower(c))
                {
                    lastIsUpper = false;
                    builder.Append(char.ToUpper(c));
                    continue;
                }

                if (char.IsDigit(c))
                {
                    lastIsUpper = false;
                    builder.Append(c);
                    continue;
                }

                if (char.IsWhiteSpace(c))
                {
                    lastIsUpper = false;
                    continue;
                }

                builder.Append(c);
            }

            return builder.ToString();
        }
        
        public static string SnakeCaseToPascalCase(string snake)
        {
            if (string.IsNullOrEmpty(snake))
                return snake;

            var builder = new StringBuilder();
            bool nextUpper = true; // 标记下一个字符需要大写（用于单词首字母）

            for (int i = 0; i < snake.Length; i++)
            {
                char c = snake[i];
        
                if (c == '_')
                {
                    nextUpper = true; // 遇到下划线，标记下一个字符需要大写
                    continue;
                }
        
                if (nextUpper)
                {
                    builder.Append(char.ToUpper(c)); // 单词首字母大写
                    nextUpper = false;
                }
                else
                {
                    builder.Append(c); // 非首字母保持原样（原输入已经是小写）
                }
            }

            return builder.ToString();
        }

        public static string BasicTypeName2KeyWord(string typeName)
        {
            return typeName switch
            {
                "Int32"   => "int",
                "Single"  => "float",
                "String"  => "string",
                "Double"  => "double",
                "Boolean" => "bool",
                _         => typeName
            };
        }
    }
}