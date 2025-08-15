#region

//文件创建者：Egg
//创建时间：03-26 07:35

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    public class CommandSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private Dictionary<string, Helper> _metadata;
        private CommandInputContext        _context;

        public void SetMetaData(Dictionary<string, Helper> data)
        {
            _metadata = data;
        }

        public void SetContext(CommandInputContext context)
        {
            _context = context;
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Commands"), 0));

            // 情况1：无命令输入或命令未识别，显示所有命令
            if (_context.Tokens.Count == 0 || !_metadata.ContainsKey(_context.Tokens[0]))
            {
                string partial = _context.Tokens.Count > 0 ? _context.Tokens[0] : "";
                foreach (var cmd in _metadata.Keys.Where(c => c.StartsWith(partial)))
                {
                    entries.Add(new SearchTreeEntry(new GUIContent(cmd))
                        { level = 1, userData = new CommandSuggestion(cmd) });
                }

                return entries;
            }

            Helper               helper         = _metadata[_context.Tokens[0]];
            List<string>         parameters     = new List<string>();
            HashSet<string>      enteredOptions = new HashSet<string>();
            Helper.CommentOption pendingOption  = null;
            int                  i              = 1;

            // 解析已输入的参数和选项
            while (i < _context.Tokens.Count)
            {
                string token = _context.Tokens[i];
                if (token.StartsWith("-"))
                {
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
                    parameters.Add(token);
                    i++;
                }
            }

            // 情况2a：需要输入选项参数
            if (pendingOption != null)
            {
                entries.Add(new SearchTreeEntry(
                    new GUIContent($"<{pendingOption.ParamType}>: {pendingOption.Comment}"))
                {
                    level    = 1,
                    userData = new OptionParamSuggestion(pendingOption)
                });
            }
            // 情况2b：补全选项名称
            else if (i < _context.Tokens.Count && _context.Tokens.Last().StartsWith("-"))
            {
                string partialOption = _context.Tokens.Last()[1..]; // 移除开头的"-"
                foreach (var option in helper.Options)
                {
                    if (!enteredOptions.Contains(option.Option) && option.Option.StartsWith(partialOption))
                    {
                        entries.Add(new SearchTreeEntry(
                            new GUIContent($"{option.Option}: {option.Comment}"))
                        {
                            level    = 1,
                            userData = new OptionSuggestion(option)
                        });
                    }
                }
            }
            // 情况2c：参数或选项提示
            else
            {
                int  paramIndex    = parameters.Count;
                bool hasMoreParams = helper.ParamGroups.Any(g => g.Params.Count > paramIndex);

                if (hasMoreParams && _context.TokenFinish)
                {
                    // 显示当前参数位置的所有可能类型
                    foreach (var group in helper.ParamGroups.Where(g => g.Params.Count > paramIndex))
                    {
                        var param = group.Params[paramIndex];
                        entries.Add(new SearchTreeEntry(
                            new GUIContent($"<{param.ParamType}>: {param.Comment}"))
                        {
                            level    = 1,
                            userData = new ParamSuggestion(param)
                        });
                    }
                }
                else
                {
                    // 参数已满，提示选项
                    foreach (var option in helper.Options.Where(o => !enteredOptions.Contains(o.Option)))
                    {
                        entries.Add(new SearchTreeEntry(
                            new GUIContent($"{option.Option}: {option.Comment}"))
                        {
                            level    = 1,
                            userData = new OptionSuggestion(option)
                        });
                    }
                }
            }

            return entries;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            if (entry.userData is CommandSuggestion cmdSuggestion)
            {
                ReplaceCurrentToken(cmdSuggestion.Command);
                return true;
            }
            else if (entry.userData is OptionSuggestion optionSuggestion)
            {
                ReplaceCurrentToken($"-{optionSuggestion.Option.Option}");
                return true;
            }
            else if (entry.userData is OptionParamSuggestion paramSuggestion)
            {
                // 可能需要处理参数输入，如自动添加占位符
                return false; // 示例中仅作提示
            }

            return false;
        }

        private string BuildNewInput(string replacement)
        {
            // 逻辑同之前
            if (_context.CurrentTokenIndex >= 0 && _context.CurrentTokenIndex < _context.Tokens.Count)
            {
                _context.Tokens[_context.CurrentTokenIndex] = replacement;
            }
            else
            {
                _context.Tokens.Add(replacement);
            }

            return string.Join(" ", _context.Tokens);
        }

        private void ReplaceCurrentToken(string newToken)
        {
            BuildNewInput(newToken);
        }
    }

// 辅助类用于存储补全建议
    public class CommandSuggestion
    {
        public string Command { get; }
        public CommandSuggestion(string command) => Command = command;
    }

    public class OptionSuggestion
    {
        public Helper.CommentOption Option { get; }
        public OptionSuggestion(Helper.CommentOption option) => Option = option;
    }

    public class OptionParamSuggestion
    {
        public Helper.CommentOption Option { get; }
        public OptionParamSuggestion(Helper.CommentOption option) => Option = option;
    }

    public class ParamSuggestion
    {
        public Helper.CommentParam Param { get; }
        public ParamSuggestion(Helper.CommentParam param) => Param = param;
    }
}
#endif