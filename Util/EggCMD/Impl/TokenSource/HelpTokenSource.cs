#region

//文件创建者：Egg
//创建时间：03-28 12:00

#endregion

using System.Collections.Generic;
using System.Linq;

namespace EggFramework.Util.EggCMD
{
    public sealed class HelpTokenSource : ICommandTokenSource
    {
        public List<string> GetTokenSource(List<string> tokens)
        {
            return CommandHelper.Helpers.Select(helper => helper.Key).ToList();
        }
    }
}