#region

//文件创建者：Egg
//创建时间：11-10 10:44

#endregion

#if UNITY_EDITOR


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EggFramework.Util.Excel;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EggFramework.Util.Localization
{
    public sealed class LocalizationEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/Excel/本地化")]
        public static void OpenWindow()
        {
            var window = GetWindow<LocalizationEditorWindow>();
            window.Show();
            window.titleContent = new GUIContent("本地化管理面板");
        }

        public  LocalizationSetting    Setting;
        private List<ExcelTableConfig> _excelTableConfigs;
        private LocalizationConfig     _config;

        protected override void OnEnable()
        {
            base.OnEnable();
            Setting = StorageUtil.LoadFromSettingFile(nameof(LocalizationSetting), new LocalizationSetting());
            _excelTableConfigs =
                StorageUtil.LoadFromSettingFile(nameof(ExcelTableConfig) + "s", new List<ExcelTableConfig>());
            _config = StorageUtil.LoadFromSettingFile(nameof(LocalizationConfig), new LocalizationConfig());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StorageUtil.SaveToSettingFile(nameof(LocalizationSetting), Setting);
            StorageUtil.SaveToSettingFile(nameof(LocalizationConfig),  _config);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree
                { { "控制面板", new LocalizationEditorWindowView(Setting, _excelTableConfigs, _config) } };
            var datas = ResUtil.GetAssets(typeof(LocalizationData));
            foreach (var data in datas)
            {
                tree.Add(((Object)data).name[..^17], data);
            }

            return tree;
        }
    }

    public class LocalizationEditorWindowView
    {
        [LabelText("本地化设置")] public LocalizationSetting    Setting;
        private                     List<ExcelTableConfig> _excelTableConfigs;

        public LocalizationEditorWindowView(LocalizationSetting setting, List<ExcelTableConfig> excelTableConfigs,
            LocalizationConfig config)
        {
            Config             = config;
            Setting            = setting;
            _excelTableConfigs = excelTableConfigs;
        }

        private ExcelTableConfig _currentConfig;

        private IEnumerable<string> _colsToLocalization
        {
            get { return _currentConfig.Columns?.Where(col => col.ExcelColType == "String").Select(col => col.Name); }
        }

        private void SetConfig()
        {
            _currentConfig = _excelTableConfigs.Find(config => config.ConfigName == ExcelTableName);
            ColsToBeLocalization.Clear();
        }

        private IEnumerable<string> _excelTableNames => _excelTableConfigs.Select(config => config.ConfigName);

        [InfoBox("需要指定Excel表名哦")]
        [ValueDropdown("_excelTableNames")]
        [LabelText("目标Excel表名")]
        [OnValueChanged("SetConfig")]
        public string ExcelTableName;

        [HideIf("@string.IsNullOrEmpty(ExcelTableName)")]
        [LabelText("需要被本地化的字段名")]
        [ValueDropdown("_colsToLocalization", IsUniqueList = true)]
        public List<string> ColsToBeLocalization = new();

        [LabelText("本地化配置")] public LocalizationConfig Config;

        [Button("生成本地化数据", ButtonSizes.Large)]
        public async void GenerateLocalizationData()
        {
            if (string.IsNullOrEmpty(ExcelTableName)) return;
            var data = Activator.CreateInstance<LocalizationData>();
            data.ExcelTable = ExcelTableName;
            Config.LocalizationTables.AddIfNotExist(ExcelTableName);
            var soData = GetSOData(ExcelTableName);

            var defaultPackage = GenerateDefaultLanguagePackage(soData);
            data.Packages.Add(defaultPackage);

            TranslateUtil.Init(Setting.AppId, Setting.SecretKey);
            var taskList = new List<UniTask>();
            foreach (var language in Setting.SupportedLanguages)
            {
                taskList.Add(AddOnePackage(data.Packages, soData, language));
            }

            await UniTask.WhenAll(taskList);

            async UniTask AddOnePackage(List<LocalizationPackage> packages, Object so,
                SystemLanguage language)
            {
                packages.Add(await GenerateLanguagePackage(so, language));
            }

            DirectoryUtil.MakeSureDirectory(Setting.LocalizationDataSavePathRoot);
            var path = Setting.LocalizationDataSavePathRoot + "/" + $"{ExcelTableName}_LocalizationData.asset";
            AssetDatabase.CreateAsset(data, path);
            ResUtil.AddAssetToGroup("LocalizationData", path, true, true);
            AssetDatabase.Refresh();
        }

        [Button("删除本地化数据，不传表名将会全部删除")]
        public void DeleteLocalizationData(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                FileUtil.DeleteFileOrDirectory(Setting.LocalizationDataSavePathRoot);
                AssetDatabase.Refresh();
                return;
            }

            if (Config.LocalizationTables.Remove(tableName))
            {
                FileUtil.DeleteFileOrDirectory(Setting.LocalizationDataSavePathRoot + "/" +
                                               $"{tableName}_LocalizationData.asset");
                FileUtil.DeleteFileOrDirectory(Setting.LocalizationDataSavePathRoot + "/" +
                                               $"{tableName}_LocalizationData.asset.meta");
                AssetDatabase.Refresh();
            }
        }

        private async UniTask<LocalizationPackage> GenerateLanguagePackage(Object soData, SystemLanguage language)
        {
            async UniTask TranslateOneWord(List<LocalizationItem> localizationItemList, string colName, string value)
            {
                localizationItemList.Add(new LocalizationItem
                {
                    EntryKey = $"{ExcelTableName}_{colName}_{value}",
                    Value    = await TranslateUtil.Translate(value, Setting.DefaultLanguage, language)
                });
            }

            var package = new LocalizationPackage
            {
                Language = language
            };
            foreach (var se in ColsToBeLocalization)
            {
                var list                 = (IList)soData.GetType().GetField("RawDataList").GetValue(soData);
                var elementType          = list.GetType().GenericTypeArguments[0];
                var fields               = elementType.GetSerializeFieldInfos();
                var localizationItemList = new List<LocalizationItem>();
                var keyHashSet           = new HashSet<string>();
                var taskList             = new List<UniTask>();
                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.Name == se)
                    {
                        foreach (var o in list)
                        {
                            var value = (string)o.GetType().GetField(se).GetValue(o);
                            if (keyHashSet.Add(se + value))
                            {
                                taskList.Add(TranslateOneWord(localizationItemList, se, value));
                            }
                        }

                        package.LocalizationCols.Add(new LocalizationCol
                        {
                            LocalizationItems = localizationItemList,
                            ColName           = se
                        });
                        break;
                    }
                }

                await UniTask.WhenAll(taskList);
            }

            return package;
        }

        private LocalizationPackage GenerateDefaultLanguagePackage(Object soData)
        {
            var package = new LocalizationPackage
            {
                Language = Setting.DefaultLanguage
            };
            foreach (var se in ColsToBeLocalization)
            {
                var list                 = (IList)soData.GetType().GetField("RawDataList").GetValue(soData);
                var elementType          = list.GetType().GenericTypeArguments[0];
                var fields               = elementType.GetSerializeFieldInfos();
                var localizationItemList = new List<LocalizationItem>();
                var keyHashSet           = new HashSet<string>();
                foreach (var fieldInfo in fields)
                {
                    if (fieldInfo.Name == se)
                    {
                        foreach (var o in list)
                        {
                            var value = (string)o.GetType().GetField(se).GetValue(o);
                            if (keyHashSet.Add(se + value))
                            {
                                localizationItemList.Add(new LocalizationItem
                                {
                                    EntryKey = $"{ExcelTableName}_{se}_{value}",
                                    Value    = value
                                });
                            }
                        }

                        package.LocalizationCols.Add(new LocalizationCol
                        {
                            LocalizationItems = localizationItemList,
                            ColName           = se
                        });
                        break;
                    }
                }
            }

            return package;
        }

        private Object GetSOData(string excelTableName)
        {
            var soTypes = TypeUtil.GetDerivedClasses(typeof(IExcelEntitySO));
            foreach (var soType in soTypes)
            {
                if (soType.Name == excelTableName + "SO")
                {
                    return ResUtil.GetAsset(soType);
                }
            }

            return null;
        }
    }
}
#endif