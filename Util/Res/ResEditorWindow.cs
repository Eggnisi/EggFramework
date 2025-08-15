#region

//文件创建者：Egg
//创建时间：10-24 01:24

#endregion


#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Object = UnityEngine.Object;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.Util.Res
{
    public sealed partial class ResEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/资源管理面板")]
        public static void OpenWindow()
        {
            var window = GetWindow<ResEditorWindow>();
            window.Show();
            window.titleContent = new GUIContent("资源管理面板");
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree      = new OdinMenuTree { { "控制面板", new ResEditorWindowView(Setting) } };
            var types     = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
            var viewTypes = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefDataView<,>));
            foreach (var type in types)
            {
                var targetType = viewTypes.Find(viewType =>
                    viewType.BaseType!.GenericTypeArguments[1] == type);
                if (targetType == null) continue;
                var view = ResUtil.GetOrCreateAsset($"{Setting.ResRefDataViewSavePath}/{targetType.Name}.asset",
                    targetType);
                targetType.BaseType!.GetMethod("Refresh")!.Invoke(view, null);
                if (!view) continue;
                tree.Add(type.Name[6..], view);
            }

            return tree;
        }

        [LabelText("资源设置")] public ResSetting Setting;

        protected override void OnEnable()
        {
            base.OnEnable();
            Setting = StorageUtil.LoadFromSettingFile(nameof(ResSetting), new ResSetting());
            ClearUnUsedFiles();
        }

        private void ClearUnUsedFiles()
        {
            var pathRoot = Setting.ResRefDataSavePath;
            if (!Directory.Exists(pathRoot)) return;
            var files = Directory.GetFiles(pathRoot);
            var typeNames = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>)).Select(type => type.Name)
                .ToList();
            foreach (var file in files)
            {
                var fileName = DirectoryUtil.ExtractName(file);
                if (typeNames.All(typeName => fileName != typeName && fileName != typeName + ".asset"))
                {
                    FileUtil.DeleteFileOrDirectory(file);
                }
            }

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StorageUtil.SaveToSettingFile(nameof(ResSetting), Setting);
        }
    }

    public class ResEditorWindowView
    {
        [LabelText("资源设置")] public ResSetting Setting;

        public ResEditorWindowView(ResSetting setting)
        {
            Setting = setting;
            if (!setting.Setup)
            {
                setting.Setup = true;
                ResUtil.RefreshResRef();
            }
        }

        [Button("刷新资源文件（资源发生变化后需要点击）", ButtonSizes.Large)]
        public static void Refresh()
        {
            ResUtil.RefreshResRef();
        }

        [Button("刷新预制体组数据（新建预制体后需要点击）", ButtonSizes.Large)]
        public static void RefreshPrefabData()
        {
            EditorWindow.GetWindow<ResEditorWindow>().Close();
            var setting = StorageUtil.LoadFromSettingFile(nameof(ResSetting), new ResSetting());
            foreach (var group in setting.PrefabGroups)
            {
                if (!string.IsNullOrEmpty(group.Name))
                {
                    var prefabs = ResUtil.GetPrefabPaths(group.Folders);
                    ResUtil.AddAssetsToGroup(group.Name, prefabs, true, true);
                }
            }

            WriteEntityConstantCode(setting);
        }

        private static void WriteEntityConstantCode(ResSetting setting)
        {
            var rootCode = new RootCode();
            rootCode.Custom("//代码使用工具生成，请勿手动修改");
            rootCode.Using("System.Collections.Generic");
            rootCode.NameSpace("EggFramework.Generator", ns =>
            {
                ns.Class("EntityConstant", false, true, false, "", cs =>
                {
                    foreach (var group in setting.PrefabGroups)
                    {
                        if (!string.IsNullOrEmpty(group.Name))
                        {
                            cs.Class(group.Name, false, true, false, "", ccs =>
                            {
                                var prefabs = ResUtil.GetPrefabPaths(group.Folders);
                                foreach (var prefab in prefabs)
                                {
                                    var name = DirectoryUtil.ExtractName(prefab);
                                    ccs.Custom(
                                        $"public const string {VariableUtil.PascalCase2BIG_SNAKE_CASE(name)} = \"{name}\";");
                                }

                                ccs.Custom("public static List<string> Entities = new(){");
                                foreach (var prefab in prefabs)
                                {
                                    var name = DirectoryUtil.ExtractName(prefab);
                                    ccs.Custom($"   {VariableUtil.PascalCase2BIG_SNAKE_CASE(name)},");
                                }

                                ccs.Custom("};");
                            });
                        }
                    }
                    cs.Custom("public static List<string> Entities");
                    cs.Custom("{");
                    cs.Custom("    get");
                    cs.Custom("    {");
                    cs.Custom("        var ret = new List<string>();");
                            
                    foreach (var prefabGroup in setting.PrefabGroups)
                    {
                        if (!string.IsNullOrEmpty(prefabGroup.Name))
                        {
                            cs.Custom($"        ret.AddRange({prefabGroup.Name}.Entities);");
                        }
                    }
                    cs.Custom("        return ret;");
                    cs.Custom("    }");
                    cs.Custom("}");
                });
            });
            var buildr = new StringBuilder();
            var writer = new StringWriter(buildr);
            rootCode.Gen(writer);

            DirectoryUtil.MakeSureDirectory(setting.EntityConstantCodePath);
            var path = $"{setting.EntityConstantCodePath}/EntityConstant.cs";
            File.WriteAllText(path, buildr.ToString());
            AssetDatabase.ImportAsset(path);
        }

        [Button("生成自定义资源引用文件代码", ButtonSizes.Large)]
        public void GenerateResRefCode([LabelText("目标样本")] Object targetObject)
        {
            if (targetObject == null) return;
            var type = targetObject.GetType();
            EditorWindow.GetWindow<ResEditorWindow>().Close();

            WriteResRefDataCode(type);
            WriteResRefDataViewCode(type);
            CompilationPipeline.RequestScriptCompilation();
        }

        private void WriteResRefDataCode(Type type)
        {
            var rootCode = new RootCode();
            rootCode.Custom("#if UNITY_EDITOR");
            rootCode.Custom("//代码使用工具生成，请勿随意修改");
            if (!string.IsNullOrEmpty(type.Namespace))
                rootCode.Using(type.Namespace);
            rootCode.NameSpace("EggFramework.Util.Res",
                ns => { ns.Class($"ResRef{type.Name}", false, false, true, $"ResRefData<{type.Name}>", cs => { }); });
            rootCode.Custom("#endif");
            var builder = new StringBuilder();
            var writer  = new StringWriter(builder);
            rootCode.Gen(writer);
            var path = $"{Setting.CustomResRefCodePath}/ResRef{type.Name}.cs";
            DirectoryUtil.MakeSureDirectory(Setting.CustomResRefCodePath);
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }

        private void WriteResRefDataViewCode(Type type)
        {
            var rootCode = new RootCode();
            rootCode.Custom("#if UNITY_EDITOR");
            rootCode.Custom("//代码使用工具生成，请勿随意修改");
            if (!string.IsNullOrEmpty(type.Namespace))
                rootCode.Using(type.Namespace);
            rootCode.NameSpace("EggFramework.Util.Res",
                ns =>
                {
                    ns.Class($"ResRef{type.Name}DataView", false, false, true,
                        $"ResRefDataView<{type.Name},ResRef{type.Name}>", cs => { });
                });
            rootCode.Custom("#endif");
            var builder = new StringBuilder();
            var writer  = new StringWriter(builder);
            rootCode.Gen(writer);
            var path = $"{Setting.CustomResRefCodePath}/ResRef{type.Name}DataView.cs";
            DirectoryUtil.MakeSureDirectory(Setting.CustomResRefCodePath);
            File.WriteAllText(path, builder.ToString());
            AssetDatabase.ImportAsset(path);
            AssetDatabase.Refresh();
        }
    }
}
#endif