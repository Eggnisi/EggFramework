#region

//文件创建者：Egg
//创建时间：10-30 10:43

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
        private void WriteDataCode(ExcelTableConfig tableConfig)
        {
            var code = new RootCode();
            code.Custom("//代码使用工具生成，请勿随意修改");
            foreach (var defaultNameSpace in _defaultNameSpaces)
            {
                code.Using(defaultNameSpace);
            }

            var nameSpaces = new HashSet<string>();
            foreach (var nameSpace in from tableConfigColumn in tableConfig.Columns
                     where tableConfigColumn.IsStruct
                     select GetExcelTypeNameSpace(tableConfigColumn.ExcelColType))
            {
                if (!string.IsNullOrEmpty(nameSpace.Trim()) && !_defaultNameSpaces.Contains(nameSpace))
                    nameSpaces.Add(nameSpace);
            }

            foreach (var nameSpace in nameSpaces)
            {
                code.Using(nameSpace);
            }

            code.NameSpace(Setting.GenerateDataClassNameSpace,
                cope =>
                {
                    cope.Class($"{tableConfig.ConfigName}SO", false, false, true, "ScriptableObject , IExcelEntitySO",
                        (cs) =>
                        {
                            cs.Custom("#if UNITY_EDITOR");
                            cs.Custom("[Button(\"读取数据\", ButtonSizes.Large), PropertyOrder(-1)]");
                            cs.Custom("private void ReadData()");
                            cs.Custom("{");
                            cs.Custom($"    ExcelUtil.ReadDataByExcelEntityType(typeof({tableConfig.ConfigName}));");
                            cs.Custom("}");
                            cs.Custom("[Button(\"刷新字符集\", ButtonSizes.Large), PropertyOrder(-2)]");
                            cs.Custom("private void RefreshEncode()");
                            cs.Custom("{");
                            cs.Custom("    PythonUtil.RefreshEncode();");
                            cs.Custom("}");
                            cs.Custom("#endif");
                            cs.Custom("[ReadOnly, LabelText(\"数据库\"), ListDrawerSettings(NumberOfItemsPerPage = 10)]");
                            cs.Custom($"public List<{tableConfig.ConfigName}> RawDataList;\n");
                        });
                });
            var codeBuilder = new StringBuilder();
            var writer      = new StringWriter(codeBuilder);
            code.Gen(writer);
            DirectoryUtil.MakeSureDirectory($"{Setting.CodePathRoot}/ExcelEntitySO");
            var path = $"{Setting.CodePathRoot}/ExcelEntitySO/{tableConfig.ConfigName}SO.cs";
            File.WriteAllText(path, codeBuilder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif