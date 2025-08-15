#region

//文件创建者：Egg
//创建时间：03-26 04:14

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EggFramework.Util.EggCMD
{
    [Serializable]
    public class Helper
    {
        public string                  CommandId;
        public string                  Comment;
        public List<CommentParamGroup> ParamGroups = new();
        public List<CommentOption>     Options     = new();
        public ICommandTokenSource     TokenSource;

        public Helper SetCommandId(string id)
        {
            CommandId = id;
            return this;
        }

        public Helper AddCommentParamGroup(CommentParamGroup group)
        {
            ParamGroups.Add(group);
            return this;
        }

        public Helper AddCommentOption(CommentOption option)
        {
            Options.Add(option);
            return this;
        }

        [Serializable]
        public class CommentParam
        {
            public string       Comment;
            public string       ParamType;
        }

        [Serializable]
        public class CommentParamGroup
        {
            public string             Comment;
            public List<CommentParam> Params = new();

            public CommentParamGroup AddCommentParam(string comment, string paramType )
            {
                Params.Add(new CommentParam
                {
                    Comment     = comment,
                    ParamType   = paramType
                });
                return this;
            }

            public CommentParamGroup AddCommentParam<T>(string comment)
            {
                Params.Add(new CommentParam
                {
                    Comment   = comment,
                    ParamType = typeof(T).Name
                });
                return this;
            }
        }

        [Serializable]
        public class CommentOption
        {
            public string Comment;
            public string Option;
            public string ParamType;
        }
    }

    public static class CommandHelper
    {
        private static readonly Dictionary<string, Helper> _helpers = new();

        private static bool _inited;

        public static Dictionary<string, Helper> Helpers
        {
            get
            {
                MakeSureInited();
                return _helpers.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
        }

        private static void MakeSureInited()
        {
            if (_inited) return;
            _inited = true;
            // 获取所有实现 ICommandHandle 的非抽象类
            var commandTypes = TypeUtil.GetDerivedClasses(typeof(ICommandHandle));
            foreach (var type in commandTypes)
            {
                // 1. 提取 CommandHandleAttribute
                var handleAttr = type.GetCustomAttribute<CommandHandleAttribute>();
                if (handleAttr == null) continue;
                // 2. 初始化或获取对应的 Helper 对象
                if (!_helpers.TryGetValue(handleAttr.TargetCommand, out var helper))
                {
                    helper                             = new Helper().SetCommandId(handleAttr.TargetCommand);
                    _helpers[handleAttr.TargetCommand] = helper;
                }
                var tokenSourceAttribute = type.GetCustomAttribute<CommandTokenSourceAttribute>();
                if (tokenSourceAttribute != null)
                {
                    _helpers[handleAttr.TargetCommand].TokenSource =
                        (ICommandTokenSource)Activator.CreateInstance(tokenSourceAttribute.TokenSourceType);
                }

                var handleCommentAttr = type.GetCustomAttribute<CommandCommentAttribute>();
                if (handleCommentAttr != null)
                {
                    _helpers[handleAttr.TargetCommand].Comment = handleCommentAttr.Comment;
                }

                // 3. 提取 EnableOptionAttribute
                var optionAttrs = type.GetCustomAttributes<EnableOptionAttribute>();
                foreach (var optAttr in optionAttrs)
                {
                    helper.AddCommentOption(new Helper.CommentOption
                    {
                        Option  = optAttr.Option,
                        Comment = optAttr.Comment,
                        ParamType = optAttr.TargetType != null
                            ? optAttr.TargetType.Name
                            : string.Empty // 选项默认无参数，若需要参数可扩展逻辑
                    });
                }

                // 4. 提取参数信息
                var paramGroup = new Helper.CommentParamGroup();

                // 获取泛型参数类型（如 CommandHandle<T1,T2> 中的 T1,T2）
                var genericArgs = type.BaseType?.GetGenericArguments();
                if (genericArgs == null) continue;

                // 获取 Handle 方法参数信息
                var handleMethod = type.GetMethod("Handle",
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    genericArgs.Select(t => t).ToArray(),
                    null
                );

                if (handleMethod != null)
                {
                    // 提取方法注释
                    var methodComment = handleMethod.GetCustomAttribute<CommandCommentAttribute>()?.Comment;
                    paramGroup.Comment = methodComment ?? string.Empty;

                    // 提取参数注释和类型
                    var parameters = handleMethod.GetParameters();
                    foreach (var param in parameters)
                    {
                      

                        var paramComment = param.GetCustomAttribute<CommandCommentAttribute>()?.Comment;
                        paramGroup.AddCommentParam(
                            paramComment ?? string.Empty,
                            genericArgs[param.Position].Name
                        );
                    }
                }

                if (paramGroup.Params.Count <= 0)
                {
                    paramGroup.Params.Add(new Helper.CommentParam
                    {
                        Comment   = string.Empty,
                        ParamType = "Null"
                    });
                }

                helper.AddCommentParamGroup(paramGroup);
            }
        }

        public static Helper GetHelper(string commandId)
        {
            MakeSureInited();
            return _helpers.GetValueOrDefault(commandId, null);
        }

        public static bool CommandExists(string commandId)
        {
            MakeSureInited();
            return _helpers.ContainsKey(commandId);
        }

        public static List<Helper> GetAllHelpers()
        {
            MakeSureInited();
            return _helpers.Select(kv => kv.Value).ToList();
        }

        public static string GetFormatDescription(Helper helper)
        {
            if (helper == null) return string.Empty;
            var builder = new StringBuilder();
            builder.Append(
                $"{helper.CommandId}{(!string.IsNullOrEmpty(helper.Comment) ? $" --{helper.Comment}" : string.Empty)}");
            builder.Append("\n");
            if (helper.ParamGroups.Count > 0)
                builder.Append("\tparams:\n");
            foreach (var paramGroup in helper.ParamGroups)
            {
                builder.Append("\t\t");
                foreach (var param in paramGroup.Params)
                {
                    builder.Append(
                        $"<{param.ParamType}>{(!string.IsNullOrEmpty(param.Comment) ? $" ({param.Comment})" : string.Empty)} ");
                }

                builder.Append(!string.IsNullOrEmpty(paramGroup.Comment) ? $"--{paramGroup.Comment}" : string.Empty);
                builder.Append("\n");
            }

            if (helper.Options.Count > 0)
                builder.Append("\toptions:\n");
            foreach (var commentOption in helper.Options)
            {
                builder.Append("\t\t");
                builder.Append(
                    $"{commentOption.Option}{(!string.IsNullOrEmpty(commentOption.ParamType) ? $" <{commentOption.ParamType}>" : string.Empty)} ");
                builder.Append(!string.IsNullOrEmpty(commentOption.Comment)
                    ? $"--{commentOption.Comment}"
                    : string.Empty);
                builder.Append("\n");
            }

            return builder.ToString();
        }
    }
}