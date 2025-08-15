#region

//文件创建者：Egg
//创建时间：11-19 11:49

#endregion

using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace EggFramework.Util.Excel
{
    [Serializable]
    public sealed class ExcelStructColumn
    {
        private readonly ExcelStructConfig       _config;
        private          int                     _index;
        private          ExcelSetting            _setting;
        private          ExcelStructModifyWindow _window;

        [ShowInInspector, HorizontalGroup("Button"), LabelText("结构名")]
        public string Name => _config.TypeName;

        public ExcelStructColumn(int index, ExcelStructConfig config)
        {
            _index   = index;
            _config  = config;
            _setting = StorageUtil.LoadFromSettingFile("ExcelSetting", new ExcelSetting());
        }

        [HorizontalGroup("Button", 50f), Button("删除")]
        public void Delete()
        {
            _setting.Configs.RemoveAt(_index);
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/{_config.TypeName}.ExcelStruct.cs");
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/{_config.TypeName}.ExcelStruct.cs.meta");
            AssetDatabase.Refresh();
            StorageUtil.SaveToSettingFile("ExcelSetting", _setting);
        }

        [HorizontalGroup("Button", 50f), Button("修改")]
        public void Modify()
        {
            _window              = EditorWindow.GetWindow<ExcelStructModifyWindow>();
            _window.titleContent = new GUIContent("修改结构体");
            _window.Config       = _config;
            _window.Host         = this;
            _window.Show();
        }

        public void Save(ExcelStructConfig config)
        {
            var origin = _setting.Configs[_index];
            _setting.Configs[_index] = config;
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/{origin.TypeName}.ExcelStruct.cs");
            FileUtil.DeleteFileOrDirectory($"{_setting.CodePathRoot}/{origin.TypeName}.ExcelStruct.cs.meta");
            new ExcelEditorWindowView(_setting, null)
                .GenerateNewStruct(config);
            StorageUtil.SaveToSettingFile("ExcelSetting", _setting);
            _window.Close();
        }
    }
}
#endif