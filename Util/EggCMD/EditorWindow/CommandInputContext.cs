#region

//文件创建者：Egg
//创建时间：03-26 07:37

#endregion


using System.Collections.Generic;

namespace EggFramework.Util.EggCMD
{
    public sealed class CommandInputContext
    {
        public List<string> Tokens;
        public int          CurrentTokenIndex;
        public bool         TokenFinish;
    }
}
