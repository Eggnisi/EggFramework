#region

//文件创建者：Egg
//创建时间：04-24 05:10

#endregion

using System.Collections.Generic;
using System.Linq;

namespace EggFramework.Util.EggCMD
{
    public abstract class AbstractCommandAutoCompleteProvider
    {
        protected CommandInputContext        _context;
        protected Dictionary<string, Helper> _metadata;

        protected AbstractCommandAutoCompleteProvider(CommandInputContext context,
            Dictionary<string, Helper> metadata)
        {
            _context   = context;
            _metadata  = metadata;
        }

        protected abstract void AfterInsertSuggestion();

        public void InsertSuggestion(string replacement)
        {
            if (_context.Tokens.Count <= 0 || _context.TokenFinish) _context.Tokens.Add(replacement);
            else
            {
                _context.Tokens[_context.CurrentTokenIndex] = replacement;
            }

            AfterInsertSuggestion();
        }

        public ( List<string>, List<string>, List<bool> ) ProvideAutoComplete()
        {
            var entries   = new List<string>();
            var contents  = new List<string>();
            var available = new List<bool>();

            ( List<string>, List<string>, List<bool> ) Ret()
            {
                return (entries, contents, available);
            }

            // 情况1：无命令输入或命令未识别，显示所有命令
            if (_context.Tokens.Count == 0 || !_metadata.ContainsKey(_context.Tokens[0]))
            {
                string partial = _context.Tokens.Count > 0 ? _context.Tokens[0] : "";
                foreach (var cmd in _metadata.Keys.Where(c => c.StartsWith(partial)))
                {
                    entries.Add(cmd);
                    contents.Add(cmd);
                    available.Add(true);
                }

                return Ret();
            }

            Helper               helper           = _metadata[_context.Tokens[0]];
            List<string>         parameters       = new List<string>();
            HashSet<string>      enteredOptions   = new HashSet<string>();
            Helper.CommentOption pendingOption    = null;
            int                  i                = 1;
            bool                 alreadyHasOption = false;

            // 解析已输入的参数和选项
            while (i < _context.Tokens.Count)
            {
                string token = _context.Tokens[i];
                if (token.StartsWith("-"))
                {
                    alreadyHasOption = true;
                    var option = helper.Options.FirstOrDefault(o => o.Option == token);
                    if (option != null)
                    {
                        enteredOptions.Add(option.Option);
                        if (!string.IsNullOrEmpty(option.ParamType))
                        {
                            if (i + 1 < _context.Tokens.Count)
                            {
                                i += 2; // 跳过选项及其参数
                            }
                            else
                            {
                                pendingOption = option; // 需要参数
                                i++;
                                break;
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++; // 未知选项
                    }
                }
                else
                {
                    parameters.AddIfNotExist(token);
                    i++;
                }
            }

            // 情况2a：需要输入选项参数
            if (pendingOption != null)
            {
                if (entries.AddIfNotExist($"<{pendingOption.ParamType}>:{pendingOption.Comment}"))
                {
                    contents.Add($"");
                    available.Add(false);
                }
            }
            // 情况2b：补全选项名称
            else if (i < _context.Tokens.Count && _context.Tokens.Last().StartsWith("-"))
            {
                string partialOption = _context.Tokens.Last()[1..]; // 移除开头的"-"
                foreach (var option in helper.Options)
                {
                    if (!enteredOptions.Contains(option.Option) && option.Option.StartsWith(partialOption))
                    {
                        if (entries.AddIfNotExist($"{option.Option}<color=#7A7A7A>:{option.Comment}</color>"))
                        {
                            contents.Add($"{option.Option}");
                            available.Add(true);
                        }
                    }
                }
            }
            // 情况2c：参数或选项提示
            else
            {
                if (!_context.TokenFinish && _context.Tokens.Count > 1 && !alreadyHasOption)
                {
                    var tokenIndex = _context.CurrentTokenIndex;
                    foreach (var group in helper.ParamGroups.Where(g =>
                                 g.Params.Count > tokenIndex - 1 && g.Params[0].ParamType != "Null"))
                    {
                        var param       = group.Params[tokenIndex - 1];
                        var paramString = parameters[tokenIndex - 1];
                        var tokens      = helper.TokenSource?.GetTokenSource(_context.Tokens);
                        if (tokens == null) continue;
                        foreach (var se in tokens
                                     .Where(se => se.StartsWith(paramString)))
                        {
                            entries.AddIfNotExist(se);
                            contents.AddIfNotExist(se.Contains(CommandParser.TOKEN_DELIMITER) ? $"\"{se}\"" : se);
                            available.Add(true);
                        }
                    }
                }

                int paramIndex = parameters.Count;

                bool hasMoreParams = helper.ParamGroups.Any(g => g.Params.Count > paramIndex);

                if (hasMoreParams)
                {
                    if (!_context.TokenFinish) return Ret();
                    foreach (var group in helper.ParamGroups.Where(g =>
                                 g.Params.Count > paramIndex && g.Params[0].ParamType != "Null"))
                    {
                        var param  = group.Params[paramIndex];
                        var tokens = helper.TokenSource?.GetTokenSource(_context.Tokens);
                        if (tokens is { Count: > 0 })
                        {
                            entries.AddRange(tokens);
                            contents.AddRange(tokens.Select(token =>
                                token.Contains(CommandParser.TOKEN_DELIMITER) ? $"\"{token}\"" : token));
                            available.AddRange(Enumerable.Repeat(true, tokens.Count));
                        }
                        else
                        {
                            if (entries.AddIfNotExist($"<{param.ParamType}>:{param.Comment}"))
                            {
                                contents.AddIfNotExist("");
                                available.Add(false);
                            }
                        }
                    }
                    // 显示当前参数位置的所有可能类型
                }

                bool canShowOption = helper.ParamGroups.Any(g => g.Params.Count <= paramIndex);
                // 参数已满，提示选项
                if (canShowOption && _context.TokenFinish)
                    foreach (var option in helper.Options.Where(o => !enteredOptions.Contains(o.Option)))
                    {
                        if (entries.AddIfNotExist($"{option.Option}<color=#7A7A7A>:{option.Comment}</color>"))
                        {
                            contents.Add($"{option.Option}");
                            available.Add(true);
                        }
                    }
            }

            return Ret();
        }
    }
}