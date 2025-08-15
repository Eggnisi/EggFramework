#region

//文件创建者：Egg
//创建时间：03-25 09:29

#endregion

using System.Collections.Generic;

namespace EggFramework.Util.EggCMD
{
    public sealed class RawCommand
    {
        public string CommandId;

        public readonly List<string> Params = new();

        public readonly Dictionary<string, List<string>> OptionDic = new();
    }
}