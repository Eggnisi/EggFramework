#region

//文件创建者：Egg
//创建时间：03-27 08:54

#endregion

using System.Collections.Generic;

namespace EggFramework.Util.EggCMD
{
    public sealed class RefreshTokenSource : ICommandTokenSource
    {
        private List<string> _tokens { get; } = new()
        {
            "prefabGroup",
            "res"
        };

        public List<string> GetTokenSource(List<string> tokens)
        {
            if (tokens.Count == 2 && !_tokens.Contains(tokens[1]))
            {
                return _tokens;
            }

            return new List<string>();
        }
    }
}