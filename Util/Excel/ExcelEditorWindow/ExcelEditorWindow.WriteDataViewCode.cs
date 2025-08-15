#region

//文件创建者：Egg
//创建时间：03-22 06:43

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using UnityEditor;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindowView
    {
        private void WriteDataViewCode(ExcelTableConfig tableConfig)
        {
            var code = new RootCode();
            code.Custom("//代码使用工具生成，请勿随意修改");
            code.Custom("#if UNITY_EDITOR");
            foreach (var defaultNameSpace in _defaultNameSpaces)
            {
                code.Using(defaultNameSpace);
            }

            var nameSpaces = new HashSet<string>();
            foreach (var nameSpace in from tableConfigColumn in tableConfig.Columns
                     where tableConfigColumn.IsStruct || tableConfigColumn.IsRes
                     select GetExcelTypeNameSpace(tableConfigColumn.ExcelColType))
            {
                if (!string.IsNullOrEmpty(nameSpace.Trim()) && !_defaultNameSpaces.Contains(nameSpace))
                    nameSpaces.Add(nameSpace);
            }

            foreach (var nameSpace in nameSpaces)
            {
                if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                {
                    code.Custom("#if UNITY_EDITOR");
                }

                code.Using(nameSpace);
                if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                {
                    code.Custom("#endif");
                }
            }

            code.NameSpace(Setting.GenerateDataClassNameSpace, cope =>
            {
                cope.Custom("[Serializable]");
                cope.Class($"{tableConfig.ConfigName}DataView", false, false, true,
                    $"ExcelDataView<{tableConfig.ConfigName}Ref,{tableConfig.ConfigName}>");
            });
            code.Custom("#endif");
            var codeBuilder = new StringBuilder();
            var writer      = new StringWriter(codeBuilder);
            code.Gen(writer);
            DirectoryUtil.MakeSureDirectory($"{Setting.CodePathRoot}/ExcelEntityDataView");
            var path = $"{Setting.CodePathRoot}/ExcelEntityDataView/{tableConfig.ConfigName}DataView.cs";
            File.WriteAllText(path, codeBuilder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif