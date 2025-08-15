#region

//文件创建者：Egg
//创建时间：04-20 09:47

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.AudioSystem
{
    [Serializable]
    public sealed class AudioGroup
    {
        public enum EAudioGroupPlayMode
        {
            [LabelText("只播放第一个")] First,
            [LabelText("随机播放")]   Random,
            [LabelText("循环播放")]   Loop,
            [LabelText("同时播放全部")] All
        }

        [FoldoutGroup("@GroupName"), LabelText("组名称"), ValidateInput("Validate")]
        public string GroupName;

        [FoldoutGroup("@GroupName"), LabelText("播放模式"), ValidateInput("ValidatePlayMode")]
        public EAudioGroupPlayMode PlayMode;

        [FoldoutGroup("@GroupName"), LabelText("组内音频"), ValidateInput("ValidateAudio")]
        public List<AudioClip> Clips = new();

        [LabelText("修改器组"), HideReferenceObjectPicker, FoldoutGroup("@GroupName")]
        public List<AudioSystemModifier> Modifiers = new();

        [FoldoutGroup("@GroupName"), LabelText("是否为背景音乐组"), ValidateInput("ValidateBGMGroup")]
        public bool IsBGMGroup;


#if UNITY_EDITOR
        private bool Validate(string name, ref string errMsg)
        {
            if (!string.IsNullOrEmpty(name)) return true;
            errMsg = "组名不能为空";
            return false;
        }

        private bool ValidatePlayMode(EAudioGroupPlayMode mode, ref string errMsg)
        {
            if (mode == EAudioGroupPlayMode.All && IsBGMGroup)
            {
                errMsg = "BGM默认只有一个实例，无法播放多个";
                return false;
            }

            return true;
        }

        private bool ValidateBGMGroup(bool value, ref string errMsg)
        {
            if (PlayMode == EAudioGroupPlayMode.All && value)
            {
                errMsg = "BGM默认只有一个实例，无法播放多个";
                return false;
            }

            return true;
        }

        private bool ValidateAudio(List<AudioClip> audioClips, ref string errMsg)
        {
            if (audioClips.Count <= 0)
            {
                errMsg = "音频组至少需要有一个音频";
                return false;
            }

            return true;
        }
#endif
    }
}