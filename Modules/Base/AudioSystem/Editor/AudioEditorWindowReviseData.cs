#region

//文件创建者：Egg
//创建时间：04-20 10:28

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
namespace EggFramework.AudioSystem
{
    public sealed class AudioEditorWindowReviseData
    {
        private readonly AudioSetting      _setting;
        private readonly AudioData         _data;
        private readonly AudioEditorWindow _host;

        public AudioEditorWindowReviseData(AudioEditorWindow host, AudioSetting setting, AudioData data)
        {
            _host    = host;
            _setting = setting;
            _data    = data;
        }
        
        [ShowInInspector, LabelText("修正数据")]
        public AudioReviseData ReviseData
        {
            get => _data.ReviseData;
            set => _data.ReviseData = value;
        }

        [Button("保存数据", ButtonSizes.Large)]
        public void Save()
        {
            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif