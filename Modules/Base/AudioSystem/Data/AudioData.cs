#region

//文件创建者：Egg
//创建时间：08-16 08:29

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Serialization;

namespace EggFramework.AudioSystem
{
    [CreateAssetMenu(menuName = "Data/AudioData")]
    public sealed class AudioData : ScriptableObject
    {
        [LabelText("背景音乐引用")] public List<AudioClip>          BGMAudios = new();
        [LabelText("音效引用")]   public List<AudioClip>          SFXAudios = new();
        [LabelText("混音器")]    public AudioMixer               Mixer;
        [LabelText("音频组")]    public List<AudioGroup>         Groups     = new();
        [LabelText("输出设置")]   public List<AudioOutputSetting> Settings   = new();
        [LabelText("音频修正数据")] public AudioReviseData          ReviseData = new();
    }
}