#region

//文件创建者：Egg
//创建时间：09-08 10:42

#endregion

using System;
using System.Collections.Generic;
using QFramework;

namespace EggFramework.AudioSystem
{
    public interface IAudioSystem : ISystem
    {
        void PlayBGM(string bgm);
        void StopBGM();
        void PlaySFX(string sfx);
        //统一设置零到一的值
        void SetBGMVolume(float strength);

        //统一设置零到一的值
        void SetSFXVolume(float strength);
        void BindBGMVolume(BindableProperty<float> bgm);
        void BindSFXVolume(BindableProperty<float> sfx);

        //播放预定义的SFX组，具体行为可以在设置面板修改
        //策略模式，随机播放、循环播放、全部播放 -> 定义播放什么
        //修改器模式，VolumeModifier，PitchModifier -> 定义怎么播放,策略模式，+ * 随机 clamp override
        void PlayGroup(string groupName);

        void TriggerOnLoadedOrAfter(Action action);
        void AddModifier(string groupName, AudioSystemModifier modifier);
        void RemoveModifier(string groupName, AudioSystemModifier modifier);
        void ClearModifier(string groupName);
        IReadOnlyList<AudioSystemModifier> GetModifiers(string groupName);
    }
}