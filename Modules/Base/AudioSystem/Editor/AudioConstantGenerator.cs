#region

//文件创建者：Egg
//创建时间：10-09 10:51

#endregion

#if UNITY_EDITOR
using System.IO;
using System.Linq;
using System.Text;
using EggFramework.CodeGenKit;
using EggFramework.Util;
using UnityEditor;
using UnityEditor.Compilation;
using StringWriter = EggFramework.CodeGenKit.Writer.StringWriter;

namespace EggFramework.AudioSystem
{
    public static class AudioConstantGenerator
    {
        public static void Generate(string path = "Assets/Scripts/Generator",
            string nameSpace = "EggFramework.SimpleAudioSystem.Constant")
        {
            var data = ResUtil.GetAsset<AudioData>();
            var code = new RootCode();
            code.Custom("//代码使用工具生成，不要随意改动");
            code.Using("System.Collections.Generic");
            code.NameSpace(nameSpace, (ns) =>
            {
                ns.Class("AudioConstant", true, true, false, "", cs =>
                {
                    cs.Class("BGM", false, true, false, "", csBg =>
                    {
                        foreach (var audioClip in data.BGMAudios)
                        {
                            csBg.Custom(
                                $"public const string {VariableUtil.PascalCase2BIG_SNAKE_CASE(audioClip.name)} = \"{audioClip.name}\";");
                        }
                        csBg.Custom("public static List<string> BGMIds = new(){");
                        foreach (var audioClip in data.BGMAudios)
                        {
                            csBg.Custom(
                                $"      {VariableUtil.PascalCase2BIG_SNAKE_CASE(audioClip.name)},");
                        }

                        csBg.Custom("};");
                    });
                    cs.Class("SFX", false, true, false, "", csFX =>
                    {
                        foreach (var audioClip in data.SFXAudios)
                        {
                            csFX.Custom(
                                $"public const string {VariableUtil.PascalCase2BIG_SNAKE_CASE(audioClip.name)} = \"{audioClip.name}\";");
                        }
                        csFX.Custom("public static List<string> SFXIds = new(){");
                        foreach (var audioClip in data.SFXAudios)
                        {
                            csFX.Custom(
                                $"      {VariableUtil.PascalCase2BIG_SNAKE_CASE(audioClip.name)},");
                        }
                        csFX.Custom("};");
                    });
                    cs.Class("Group", false, true, false, "", csGroup =>
                    {
                        foreach (var audioGroup in data.Groups)
                        {
                            csGroup.Custom(
                                $"public const string {VariableUtil.PascalCase2BIG_SNAKE_CASE(audioGroup.GroupName)} = \"{audioGroup.GroupName}\";");
                        }
                        csGroup.Custom("public static List<string> GroupIds = new(){");
                        foreach (var audioGroup in data.Groups)
                        {
                            csGroup.Custom(
                                $"      {VariableUtil.PascalCase2BIG_SNAKE_CASE(audioGroup.GroupName)},");
                        }
                        csGroup.Custom("};");
                    });
                });
            });
            var builder = new StringBuilder();
            var writer  = new StringWriter(builder);
            code.Gen(writer);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fullPath = $"{path}/AudioConstant.cs";
            File.WriteAllText(fullPath, builder.ToString());
            AssetDatabase.ImportAsset(fullPath);
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();
        }
    }
}
#endif