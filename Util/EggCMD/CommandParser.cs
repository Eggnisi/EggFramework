#region

//文件创建者：Egg
//创建时间：03-25 09:28

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    public static class CommandParser
    {
        public const char COMMAND_PREFIX = '#';
        public const char TOKEN_DELIMITER = ' ';
        public const string OPTION_PREFIX1 = "--";
        public const char OPTION_PREFIX2 = '-';

        public static List<RawCommand> ParseRawCommands(string command)
        {
            var ret = new List<RawCommand>();
            var commandLines = TokenSplitter.SplitString(command, COMMAND_PREFIX, true);
            foreach (var commandLine in commandLines)
            {
                var trimLine = commandLine.Value.Trim();
                if (!string.IsNullOrEmpty(trimLine))
                {
                    var tokens = TokenSplitter.ParseTokens(trimLine, TOKEN_DELIMITER);
                    if (tokens.Any(token => token is OPTION_PREFIX1 or "-"))
                        throw new NotSupportedException("InvalidToken: Token can not be \"-\" or \"--\"");
                    ret.Add(ParseRawCommand(tokens));
                }
            }

            return ret;
        }

        private static bool IsOptionToken(string token)
        {
            return (token.StartsWith(OPTION_PREFIX1) || token.StartsWith(OPTION_PREFIX2)) &&
                   !int.TryParse(token, out _) && !float.TryParse(token, out _);
        }

        private static RawCommand ParseRawCommand(List<string> tokens)
        {
            var ret = new RawCommand();
            var tokenIndex = 0;
            ret.CommandId = tokens[tokenIndex];
            tokenIndex++;
            while (tokenIndex < tokens.Count && !IsOptionToken(tokens[tokenIndex]))
            {
                ret.Params.Add(tokens[tokenIndex]);
                tokenIndex++;
            }

            var curOption = string.Empty;
            while (tokenIndex < tokens.Count)
            {
                if (IsOptionToken(tokens[tokenIndex]))
                {
                    curOption = tokens[tokenIndex];
                    ret.OptionDic[curOption] = new List<string>();
                    tokenIndex++;
                }
                else
                {
                    ret.OptionDic[curOption].Add(tokens[tokenIndex]);
                    tokenIndex++;
                }
            }

            return ret;
        }

        public static int ParseTokenIndex(string textFieldText, int textFieldCursorIndex)
        {
            var tokens = TokenSplitter.SplitString(textFieldText, TOKEN_DELIMITER, true);
            tokens = tokens.Where(token => !string.IsNullOrWhiteSpace(token.Value)).ToList();
            for (var index = 0; index < tokens.Count; index++)
            {
                var commandToken = tokens[index];
                if (commandToken.StartIndex + commandToken.Length >= textFieldCursorIndex &&
                    textFieldCursorIndex >= commandToken.StartIndex)
                {
                    return index;
                }
            }

            return 0;
        }
    }
}