#region

// 作者： Egg
// 时间： 2025 07.19 09:16

#endregion

namespace EggFramework
{
    public static class FuncParser
    {
        public static RawFunc ParseRawFunc(string func)
        {
            var ret = new RawFunc();
            var tokens = func.Split(" ");
            var tokenIndex = 0;
            ret.FuncId = tokens[tokenIndex];
            tokenIndex++;
            while (tokenIndex < tokens.Length)
            {
                ret.Params.Add(tokens[tokenIndex]);
                tokenIndex++;
            }
            return ret;
        }
    }
}