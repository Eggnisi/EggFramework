#region

//文件创建者：Egg
//创建时间：04-20 10:28

#endregion

#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.AudioSystem
{
    public sealed class AudioEditorWindowPreviewClip
    {
        private readonly AudioSetting      _setting;
        private readonly AudioData         _data;
        private readonly AudioEditorWindow _host;

        public AudioEditorWindowPreviewClip(AudioEditorWindow host, AudioSetting setting, AudioData data)
        {
            _host    = host;
            _setting = setting;
            _data    = data;
        }

        [ShowInInspector, LabelText("背景音乐")] public List<AudioClip> BGMClips => _data.BGMAudios;
        [ShowInInspector, LabelText("音效")]   public List<AudioClip> SFXClips => _data.SFXAudios;

        [Button("重新生成音频数据", ButtonSizes.Large)]
        private void Refresh()
        {
            _host.GenerateAudioData();
        }
    }
}
#endif