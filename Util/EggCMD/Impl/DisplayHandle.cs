#region

//文件创建者：Egg
//创建时间：03-27 08:57

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using EggFramework.Util.Excel;
using EggFramework.Util.Res;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("display")]
    [CommandComment("查看框架内部数据")]
    [EnableOption("-d", "显示细节")]
    [CommandTokenSource(typeof(DisplayTokenSource))]
    public sealed class DisplayHandle : CommandHandle<string>
    {
        protected override void Handle(
            [CommandComment("展示参数")] string p1)
        {
            switch (p1)
            {
                case "excelStruct":
                {
                    var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
                    if (_context.OptionExist("-d"))
                    {
                        CommandLogger.Log(setting.Configs, true);
                    }
                    else
                    {
                        foreach (var excelStructConfig in setting.Configs)
                        {
                            CommandLogger.Log(excelStructConfig.TypeName);
                        }
                    }
                }
                    break;
                case "excelTable":
                {
                    var configs =
                        StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s", new List<ExcelTableConfig>());
                    if (_context.OptionExist("-d"))
                        CommandLogger.Log(configs, true);
                    else
                        foreach (var excelTableConfig in configs)
                        {
                            CommandLogger.Log(excelTableConfig.ConfigName);
                        }
                }
                    break;
                case "prefabGroup":
                {
                    var setting = StorageUtil.LoadFromSettingFile(nameof(ResSetting), new ResSetting());
                    if (_context.OptionExist("-d"))
                    {
                        CommandLogger.Log(setting.PrefabGroups, true);
                    }
                    else
                        foreach (var settingPrefabGroup in setting.PrefabGroups)
                        {
                            CommandLogger.Log(settingPrefabGroup.Name);
                        }
                }
                    break;
                case "resRef":
                {
                    var types = TypeUtil.GetDerivedClassesFromGenericClass(typeof(ResRefData<>));
                    if (_context.OptionExist("-d"))
                    {
                        CommandLogger.LogWarning("Option -d is not available for resRef");
                    }

                    foreach (var type in types)
                    {
                        CommandLogger.Log(type.Name);
                    }
                }
                    break;
                case "audios":
                {
                    var bgms = StorageUtil.LoadFromSettingFile("AudioConstant.BGMIds", new List<string>());
                    var fxs  = StorageUtil.LoadFromSettingFile("AudioConstant.SFXIds",  new List<string>());

                    if (_context.OptionExist("-d"))
                    {
                        CommandLogger.LogWarning("Option -d is not available for audios");
                    }

                    CommandLogger.Log("BGM Keys:");
                    foreach (var bgm in bgms)
                    {
                        CommandLogger.Log("    " + bgm);
                    }

                    CommandLogger.Log("SFX Keys:");
                    foreach (var fx in fxs)
                    {
                        CommandLogger.Log("    " + fx);
                    }
                }
                    break;
            }
        }


        [CommandHandle("display")]
        public sealed class DisplayHandle1 : CommandHandle<string, string>
        {
            protected override void Handle([CommandComment("展示参数1")] string p1, [CommandComment("展示参数2")] string p2)
            {
                switch (p1)
                {
                    case "excelStruct":
                    {
                        var setting = StorageUtil.LoadFromSettingFile(nameof(ExcelSetting), new ExcelSetting());
                        var flag    = false;
                        foreach (var excelStructConfig in setting.Configs)
                        {
                            if (excelStructConfig.TypeName == p2)
                            {
                                CommandLogger.Log(excelStructConfig, true);
                                flag = true;
                            }
                        }

                        if (!flag)
                        {
                            CommandLogger.LogError("没有找到对应的excelStruct");
                        }
                    }
                        break;
                    case "excelTable":
                    {
                        var configs =
                            StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s",
                                new List<ExcelTableConfig>());
                        var flag = false;
                        foreach (var excelTableConfig in configs)
                        {
                            if (excelTableConfig.ConfigName == p2)
                            {
                                CommandLogger.Log(excelTableConfig, true);
                                flag = true;
                            }
                        }

                        if (!flag)
                        {
                            CommandLogger.LogError("没有找到对应的excelTable");
                        }
                    }
                        break;
                }
            }
        }
    }
}
#endif