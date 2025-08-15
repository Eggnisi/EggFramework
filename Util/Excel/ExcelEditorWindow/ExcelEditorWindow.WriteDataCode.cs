#region

//文件创建者：Egg
//创建时间：10-30 10:44

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using Sirenix.Utilities.Editor;
using UnityEditor;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindowView
    {
        private void WriteTypeCode(ExcelTableConfig tableConfig)
        {
            var code = new RootCode();
            code.Custom("//代码使用工具生成，请勿随意修改");
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
                cope.Class($"{tableConfig.ConfigName}", false, false, true, "IExcelEntity", (cs) =>
                {
                    foreach (var tableConfigColumn in tableConfig.Columns)
                    {
                        var nameSpace = GetExcelTypeNameSpace(tableConfigColumn.ExcelColType);
                        if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                        {
                            cs.Custom("#if UNITY_EDITOR");
                        }

                        var typeName = VariableUtil.BasicTypeName2KeyWord(tableConfigColumn.ExcelColType);
                        cs.Custom(
                            $"[LabelText(\"{(string.IsNullOrEmpty(tableConfigColumn.ChineseName) ? tableConfigColumn.Name : tableConfigColumn.ChineseName)}\")]");
                        cs.Custom(
                            $"public {(tableConfigColumn.IsList ? $"List<{typeName}>" : typeName)} {tableConfigColumn.Name};\n");
                        if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                        {
                            cs.Custom("#endif");
                        }
                    }

                    cs.Custom($"public {tableConfig.ConfigName} Clone(){{");
                    cs.Custom($"    return CloneMapUtil<{tableConfig.ConfigName}>.Clone(this);");
                    cs.Custom("}");
                });
            });
            var codeBuilder = new StringBuilder();
            var writer      = new StringWriter(codeBuilder);
            code.Gen(writer);
            DirectoryUtil.MakeSureDirectory($"{Setting.CodePathRoot}/ExcelEntity");
            var path = $"{Setting.CodePathRoot}/ExcelEntity/{tableConfig.ConfigName}.cs";
            File.WriteAllText(path, codeBuilder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif