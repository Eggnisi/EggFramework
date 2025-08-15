#region

//文件创建者：Egg
//创建时间：04-20 11:01

#endregion

#if UNITY_EDITOR


using System;
using Sirenix.OdinInspector;
using UnityEditor;

namespace EggFramework.AudioSystem
{
    [Serializable]
    public sealed class AudioOutputModifyColumn
    {
        [HorizontalGroup("Button"), LabelText("输出路径"), ShowInInspector]
        public string OutputPath => _setting.OutputMixerGroupPath;

        private readonly AudioOutputSetting _setting;
        private readonly AudioData          _data;

        public AudioOutputModifyColumn(AudioOutputSetting setting, AudioData data)
        {
            _setting = setting;
            _data    = data;
        }

        [HorizontalGroup("Button", LabelWidth = 50f), Button("修改绑定音频组")]
        private void Modify()
        {
            var window = EditorWindow.GetWindow<AudioOutputModifyWindow>();
            window.Init(_setting, _data);
            window.Show();
        }
    }
}
#endif