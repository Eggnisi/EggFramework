#region

//文件创建者：Egg
//创建时间：10-30 10:49

#endregion

#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.Util.Excel
{
    public sealed partial class ExcelEditorWindowView
    {
        [Button("新建结构体", ButtonSizes.Large)]
        public void ShowExcelStructCreateWindow()
        {
            var window = EditorWindow.GetWindow<ExcelStructCreateWindow>();
            window.titleContent = new GUIContent("结构体创建窗口");
            window.Host         = this;
            window.Show();
        }

        public void GenerateNewStruct([LabelText("结构体参数")] ExcelStructConfig config)
        {
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            EditorWindow.GetWindow<ExcelStructCreateWindow>().Close();
            var code = new RootCode();
            code.Custom("//代码使用工具生成，请勿随意修改");
            foreach (var defaultNameSpace in _defaultNameSpaces)
            {
                code.Using(defaultNameSpace);
            }
            var nameSpaces = new HashSet<string>();
            foreach (var nameSpace in from tableConfigColumn in config.Columns
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

            code.NameSpace("EggFramework.Generator", cope =>
            {
                cope.Custom("[Serializable]");
                cope.Struct($"{config.TypeName}", false, "IExcelStruct", (cs) =>
                {
                    foreach (var tableConfigColumn in config.Columns)
                    {
                        var nameSpace = GetExcelTypeNameSpace(tableConfigColumn.ExcelColType);
                        if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                        {
                            cs.Custom("#if UNITY_EDITOR");
                        }

                        var typeName = VariableUtil.BasicTypeName2KeyWord(tableConfigColumn.ExcelColType);
                        cs.Custom(
                            $"[LabelText(\"{(string.IsNullOrEmpty(tableConfigColumn.ChineseName) ? tableConfigColumn.Name : tableConfigColumn.ChineseName)}\")]");
                        if (tableConfigColumn.IsList)
                        {
                            cs.Custom($"[OnValueChanged(\"__OnValueChanged__{tableConfigColumn.Name}\", true)]");
                            if (tableConfigColumn.ExcelColType == "String")
                            {
                                cs.Custom($"[ListDrawerSettings(CustomAddFunction = \"_add__{tableConfigColumn.Name}\")]");
                            }
                            cs.Custom($"public {($"List<{typeName}>")} {tableConfigColumn.Name};\n");
                            cs.Custom("#if UNITY_EDITOR");
                            if (tableConfigColumn.ExcelColType == "String")
                            {
                                cs.Custom($"private string _add__{tableConfigColumn.Name} => \"Empty\";");
                            }

                            cs.Custom($"private Action _action__{tableConfigColumn.Name};");
                            cs.Custom($"public void RegisterOnCollectionChanged__{tableConfigColumn.Name}(Action action)");
                            cs.Custom("{");
                            cs.Custom($"    _action__{tableConfigColumn.Name} ??= action;");
                            cs.Custom("}");
                            cs.Custom($"private void __OnValueChanged__{tableConfigColumn.Name}(List<{typeName}> list)");
                            cs.Custom("{");
                            cs.Custom($"    _action__{tableConfigColumn.Name}?.Invoke();");
                            cs.Custom("}");
                            cs.Custom("#endif");
                        }
                        else cs.Custom($"public {typeName} {tableConfigColumn.Name};\n");
                        if (!string.IsNullOrEmpty(nameSpace) && nameSpace.Contains("Editor"))
                        {
                            cs.Custom("#endif");
                        }
                    }
                });
            });
            var codeBuilder = new StringBuilder();
            var writer      = new StringWriter(codeBuilder);
            code.Gen(writer);
            DirectoryUtil.MakeSureDirectory($"{Setting.CodePathRoot}/ExcelStruct");
            var path = $"{Setting.CodePathRoot}/ExcelStruct/{config.TypeName}.cs";
            File.WriteAllText(path, codeBuilder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}
#endif