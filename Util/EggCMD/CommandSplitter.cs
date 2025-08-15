#region

//文件创建者：Egg
//创建时间：03-25 09:40

#endregion

using System.Collections.Generic;
using System.Text;

namespace EggFramework.Util.EggCMD
{
    public static class CommandSplitter
    {
        public class CommandToken
        {
            public int    Length;
            public int    StartIndex;
            public string Value;
        }
        
        public static List<CommandToken> SplitString(string input, char delimiter, bool saveTransferMeaning)
        {
            List<CommandToken> result         = new List<CommandToken>();
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
                    result.Add(new CommandToken
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
                result.Add(new CommandToken
                {
                    StartIndex = startIndex,
                    Length     = input.Length - startIndex,
                    Value      = currentSegment.ToString()
                });
            }

            return result;
        }
    }
}