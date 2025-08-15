#region

//文件创建者：Egg
//创建时间：03-25 09:40

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EggFramework.Util.EggCMD
{
    public static class TokenSplitter
    {
        public class Token
        {
            public int    Length;
            public int    StartIndex;
            public string Value;
        }
        public static List<Token> SplitString(string input, char delimiter, bool saveTransferMeaning)
        {
            List<Token> result         = new List<Token>();
            StringBuilder      currentSegment = new StringBuilder();
            bool               inSingleQuote  = false;
            bool               inDoubleQuote  = false;
            int                startIndex     = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                // 处理转义字符
                if (currentChar == '\\')
                {
                    if (i + 1 < input.Length)
                    {
                        if (saveTransferMeaning)
                            currentSegment.Append(currentChar);
                        currentSegment.Append(input[i + 1]);
                        i++; // 跳过转义字符
                    }
                    else
                    {
                        currentSegment.Append(currentChar);
                    }

                    continue;
                }

                // 处理单引号
                if (currentChar == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    if (saveTransferMeaning)
                        currentSegment.Append(currentChar);
                    continue;
                }

                // 处理双引号
                if (currentChar == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    if (saveTransferMeaning)
                        currentSegment.Append(currentChar);
                    continue;
                }

                // 处理分隔符
                if (!inSingleQuote && !inDoubleQuote && currentChar == delimiter)
                {
                    result.Add(new Token
                    {
                        StartIndex = startIndex,
                        Length     = i - startIndex,
                        Value      = currentSegment.ToString()
                    });
                    currentSegment.Clear();
                    startIndex = i + 1; // 重置起始索引到下一个字符
                    continue;
                }

                currentSegment.Append(currentChar);
            }

            // 处理最后一个token
            if (currentSegment.Length > 0)
            {
                result.Add(new Token
                {
                    StartIndex = startIndex,
                    Length     = input.Length - startIndex,
                    Value      = currentSegment.ToString()
                });
            }

            return result;
        }
        
        public static List<string> ParseTokens(string trimLine,char delimiter)
        {
            trimLine = trimLine.Trim();
            var tokens = SplitString(trimLine, delimiter, false).Select(token => token.Value)
                .ToList();
            return tokens;
        }
        
        public static bool ParseParam(string value, Type targetType, out object ret)
        {
            ret = null;
            if (targetType == typeof(string))
            {
                ret = value;
                return true;
            }

            // 1. 查找TryParse方法：public static bool TryParse(string, out T)
            var tryParseMethod = targetType.GetMethod(
                "TryParse",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string), targetType.MakeByRefType() },
                null
            );

            if (tryParseMethod == null)
                return false;

            // 2. 准备调用参数
            var parameters = new object[2];
            parameters[0] = value;
            parameters[1] = targetType.IsValueType ? Activator.CreateInstance(targetType) : null;

            try
            {
                // 3. 调用TryParse方法
                var success = (bool)tryParseMethod.Invoke(null, parameters);
                if (success)
                {
                    ret = parameters[1]; // 解析成功时返回结果
                    return true;
                }

                ret = null; // 解析失败时返回空
                return false;
            }
            catch
            {
                ret = null;
                return false;
            }
        }
    }
}