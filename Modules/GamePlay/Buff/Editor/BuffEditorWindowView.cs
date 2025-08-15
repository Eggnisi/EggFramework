#region

//文件创建者：Egg
//创建时间：02-21 01:51

#endregion

#if UNITY_EDITOR

using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using EggFramework.Util;
using Sirenix.OdinInspector;
using UnityEditor;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework
{
    public sealed class BuffEditorWindowView
    {
        [LabelText("配置信息")] public BuffConfig Config;

        public BuffEditorWindowView(BuffConfig config)
        {
            Config = config;
        }

        [Button("保存配置信息", ButtonSizes.Large),
         InfoBox("一般退出后自动保存即可，若新增Trigger没有显示，则手动点击")]
        public void SaveConfig()
        {
            StorageUtil.SaveToSettingFile(nameof(BuffConfig), Config);
        }

        [Button("更新Buff数据", ButtonSizes.Large)]
        public void CollectBuff()
        {
            EditorWindow.GetWindow<BuffEditorWindow>().Close();
            var buffDatas = ResUtil.GetAssets<BuffData>().ToList();
            DirectoryUtil.MakeSureDirectory(Config.BuffRefFilePath);
            var path    = $"{Config.BuffRefFilePath}/BuffDataRef.asset";
            var buffRef = ResUtil.GetOrCreateAsset<BuffDataRef>(path);
            buffRef.BuffDatas = buffDatas;
            ResUtil.AddAssetToGroup("BuffData", path, true, true);
            EditorUtility.SetDirty(buffRef);

            WriteBuffCode(buffRef);

            AssetDatabase.SaveAssets();
        }

        private void WriteBuffCode(BuffDataRef buffRef)
        {
            var rootCode = new RootCode();
            rootCode.Custom("//代码使用工具生成，请勿手动修改");
            rootCode.Using("EggFramework");
            rootCode.Using("System");
            rootCode.Using("System.Linq");
            rootCode.Using("System.Collections.Generic");
            rootCode.Using("System.Collections");
            rootCode.NameSpace("EggFramework.Generator", ns =>
            {
                ns.Class("BuffConstant", false, true, false, "", cs =>
                {
                    cs.Class("Trigger", false, true, false, "", tcs =>
                    {
                        var triggers = Config.CustomTriggers.ToList();
                        triggers.AddRange(BuffData.InnerBuffTriggers);
                        foreach (var configCustomTrigger in triggers.Where(trigger => !string.IsNullOrEmpty(trigger)))
                        {
                            tcs.Custom(
                                $"public const string {VariableUtil.PascalCase2BIG_SNAKE_CASE(configCustomTrigger)} = \"{configCustomTrigger}\";");
                        }
                    });
                    cs.Class("Tag", false, true, false, "", tcs =>
                    {
                        var tags = Config.CustomTags.ToList();
                        foreach (var tag in tags.Where(trigger => !string.IsNullOrEmpty(trigger)))
                        {
                            tcs.Custom(
                                $"public const string {VariableUtil.PascalCase2BIG_SNAKE_CASE(tag)} = \"{tag}\";");
                        }
                    });
                    cs.Class("BuffData", false, true, false, "", bcs =>
                    {
                        foreach (var data in buffRef.BuffDatas.Where(data => !string.IsNullOrEmpty(data.name)))
                        {
                            bcs.Custom(
                                $"public const string {VariableUtil.PascalCase2BIG_SNAKE_CASE(data.name)} = \"{data.name}\";");
                        }
                    });
                });
            });
            var stringBuilder = new StringBuilder();
            var writer        = new StringWriter(stringBuilder);
            rootCode.Gen(writer);

            DirectoryUtil.MakeSureDirectory(Config.BuffConstantCodePath);
            var path = $"{Config.BuffConstantCodePath}/BuffConstant.cs";
            File.WriteAllText(path, stringBuilder.ToString());
            AssetDatabase.ImportAsset(path);
        }
    }
}

#endif