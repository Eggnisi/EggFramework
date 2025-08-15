#region

//文件创建者：Egg
//创建时间：11-19 10:12

#endregion

#if UNITY_EDITOR

using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    [Serializable]
    public sealed class ExcelDataColumn
    {
        private          int                   _index;
        private readonly ExcelTableConfig      _data;
        private          ExcelEditorWindowView _host;
        private          ExcelDataColumnWindow _window;

        public ExcelDataColumn(int index, ExcelTableConfig tableConfig, ExcelEditorWindowView host)
        {
            _index  = index;
            _data   = tableConfig;
            _host   = host;
        }

        [ShowInInspector, HorizontalGroup("Button"), LabelText("表名")]
        public string Name => _data.ConfigName;

        [HorizontalGroup("Button", 50f), Button("删除")]
        public void Delete()
        {
            _host.DeleteTable(_index);
        }

        [HorizontalGroup("Button", 50f), Button("修改")]
        public void Modify()
        {
            _window = EditorWindow.GetWindow<ExcelDataColumnWindow>();
            _window.Config = new ExcelTableConfig
            {
                ConfigName = _data.ConfigName,
                Columns    = _data.Columns.ToList()
            };
            _window.Host         = this;
            _window.titleContent = new GUIContent("表格字段预览窗口");
            _window.Show();
        }

        public void Save(ExcelTableConfig data)
        {
            _host.SaveTable(_index, data);
            _window?.Close();
        }
    }
}
#endif