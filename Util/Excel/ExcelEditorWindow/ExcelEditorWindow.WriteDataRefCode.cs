#region

//文件创建者：Egg
//创建时间：03-22 06:42

#endregion

#if UNITY_EDITOR


using System;
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
        private static readonly List<string> _defaultNameSpaces = new()
        {
            "System",
            "System.Collections.Generic",
            "System.Linq",
            "EggFramework",
            "EggFramework.Util.Excel",
            "EggFramework.Util",
            "UnityEngine",
            "Sirenix.OdinInspector",
        };
        private void WriteDataRefCode(ExcelTableConfig tableConfig)
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
                cope.Class($"{tableConfig.ConfigName}Ref", false, false, true,
                    $"ExcelEntityRef<{tableConfig.ConfigName}>", (cs) =>
                    {
                        var colIndex = 0;
                        for (var index = 0; index < tableConfig.Columns.Count; index++)
                        {
                            var tableConfigColumn = tableConfig.Columns[index];
                            var nameSpace         = GetExcelTypeNameSpace(tableConfigColumn.ExcelColType);
                            if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                            {
                                cs.Custom("#if UNITY_EDITOR");
                            }

                            if (!tableConfigColumn.IsStruct)
                            {
                                var typeName = VariableUtil.BasicTypeName2KeyWord(tableConfigColumn.ExcelColType);
                                cs.Custom(
                                    $"[ShowInInspector, HideLabel, VerticalGroup(\"{(string.IsNullOrEmpty(tableConfigColumn.ChineseName) ? tableConfigColumn.Name : tableConfigColumn.ChineseName)}\"){(typeName is "Sprite" or "Texture" or "GameObject" ? ", PreviewField" : "")}{(tableConfigColumn.IsList ? $", OnValueChanged(\"__OnValueChanged__{tableConfigColumn.Name}\",true)" : "")}{(tableConfigColumn.ExcelColType == "GameObject" ? ", AssetsOnly" : "")}]");
                                cs.Custom(
                                    $"public {(tableConfigColumn.IsList ? $"List<{typeName}>" : typeName)} {tableConfigColumn.Name}");
                                cs.Custom("{");
                                cs.Custom(
                                    $"    get => ExcelUtil.ParseData<{(tableConfigColumn.IsList ? $"List<{typeName}>" : typeName)}>(GetRawData({colIndex}));");
                                cs.Custom($"    set => SetRawData({colIndex}, ExcelUtil.SerializeData(value));");
                                cs.Custom("}\n");

                                if (tableConfigColumn.IsList)
                                {
                                    cs.Custom("#if UNITY_EDITOR");
                                    cs.Custom(
                                        $"private void __OnValueChanged__{tableConfigColumn.Name}(List<{typeName}> value)");
                                    cs.Custom("{");
                                    cs.Custom($"    SetRawData({colIndex}, ExcelUtil.SerializeData(value));");
                                    cs.Custom("}");
                                    cs.Custom("#endif");
                                }

                                colIndex++;
                            }
                            else
                            {
                                var config = ExcelUtil.GetExcelStructConfigByName(tableConfigColumn.ExcelColType);
                                cs.Custom(
                                    $"[ShowInInspector, HideLabel, VerticalGroup(\"{(!string.IsNullOrEmpty(tableConfigColumn.ChineseName) ? tableConfigColumn.ChineseName : tableConfigColumn.Name)}\"), OnValueChanged(\"__OnValueChanged__{tableConfigColumn.Name}\", true)]");
                                cs.Custom($"public {tableConfigColumn.ExcelColType} {tableConfigColumn.Name}");
                                cs.Custom("{");
                                cs.Custom("    get");
                                cs.Custom("    {");
                                cs.Custom($"        var ret = new {tableConfigColumn.ExcelColType}");
                                cs.Custom("        {");
                                for (var i = 0; i < config.Columns.Count; i++)
                                {
                                    var excelColConfig = config.Columns[i];
                                    if (excelColConfig.IsStruct) throw new Exception("暂时不支持结构体嵌套");
                                    cs.Custom(
                                        $"            {excelColConfig.Name} = ExcelUtil.ParseData<{(excelColConfig.IsList ? $"List<{excelColConfig.ExcelColType}>" : excelColConfig.ExcelColType)}>(GetRawData({colIndex + i})),");
                                }

                                cs.Custom("        };");
                                for (var i = 0; i < config.Columns.Count; i++)
                                {
                                    var excelColConfig = config.Columns[i];
                                    if (excelColConfig.IsList)
                                    {
                                        cs.Custom(
                                            $"        ret.RegisterOnCollectionChanged__{excelColConfig.Name}(() =>");
                                        cs.Custom("        {");
                                        cs.Custom(
                                            $"             {tableConfigColumn.Name} = CloneMapUtil<{tableConfigColumn.ExcelColType}>.Clone(ret);");
                                        cs.Custom("        });");
                                    }
                                }

                                cs.Custom("        return ret;");
                                cs.Custom("    }");
                                cs.Custom("    set");
                                cs.Custom("    {");
                                for (var i = 0; i < config.Columns.Count; i++)
                                {
                                    var excelColConfig = config.Columns[i];
                                    cs.Custom(
                                        $"         SetRawData({colIndex + i}, ExcelUtil.SerializeData(value.{excelColConfig.Name}));");
                                }

                                cs.Custom("    }");
                                cs.Custom("}");

                                cs.Custom(
                                    $"private void __OnValueChanged__{tableConfigColumn.Name}({tableConfigColumn.ExcelColType} val)");
                                cs.Custom("{");
                                for (var i = 0; i < config.Columns.Count; i++)
                                {
                                    var excelColConfig = config.Columns[i];
                                    cs.Custom(
                                        $"    SetRawData({colIndex + i}, ExcelUtil.SerializeData(val.{excelColConfig.Name}));");
                                }

                                cs.Custom("}");
                                colIndex += config.Columns.Count;
                            }

                            if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                            {
                                cs.Custom("#endif");
                            }
                        }

                        cs.Custom($"public override int MaxRowIndex => {colIndex};");
                    });
            });
            code.Custom("#endif");
            var codeBuilder = new StringBuilder();
            var writer      = new StringWriter(codeBuilder);
            code.Gen(writer);
            DirectoryUtil.MakeSureDirectory($"{Setting.CodePathRoot}/ExcelEntityRef");
            var path = $"{Setting.CodePathRoot}/ExcelEntityRef/{tableConfig.ConfigName}Ref.cs";
            File.WriteAllText(path, codeBuilder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif