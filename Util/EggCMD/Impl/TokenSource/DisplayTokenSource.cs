#region

//文件创建者：Egg
//创建时间：03-27 08:57

#endregion

using System.Collections.Generic;
using System.Linq;
using EggFramework.Util.Excel;

namespace EggFramework.Util.EggCMD
{
    public sealed class DisplayTokenSource : ICommandTokenSource
    {
        private List<string> _tokens { get; } = new()
        {
            "excelStruct",
            "excelTable",
            "prefabGroup",
            "resRef",
            "audios",
        };

        public List<string> GetTokenSource(List<string> tokens)
        {
            if (tokens.Count == 2 && !_tokens.Contains(tokens[1]) || tokens.Count == 1)
            {
                return _tokens;
            }
            if (_tokens.Count == 3 && !_tokens.Contains(tokens[2]) || tokens.Count == 2)
            {
                switch (tokens[1])
                {
                    case "excelStruct" :
                        var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
                        return setting.Configs.Select(con => con.TypeName).ToList();
                    case "excelTable" :
                        var configs =
                            StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s", new List<ExcelTableConfig>());
                        return configs.Select(con => con.ConfigName).ToList();
                }
            }

            return new List<string>();
        }
    }
}