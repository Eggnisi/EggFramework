#region

//文件创建者：Egg
//创建时间：10-20 10:16

#endregion

using System;
using System.Collections.Generic;
using EggFramework.Util;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.AudioSystem
{
    [Serializable, HideReferenceObjectPicker]
    public sealed class AudioReviseData
    {
        [LabelText("全局音量映射")]
        public Vector2 GlobalVolumeMapping = new(0f, 1f);
        [LabelText("音频修正项"), HideReferenceObjectPicker]
        public List<AudioReviseItem> ReviseItems = new();
    }

    [Serializable, HideReferenceObjectPicker]
    public sealed class AudioReviseItem
    {
        [LabelText("目标音频"), ValueDropdown("GetAudioNames")]
        public string TargetAudio;

        [LabelText("起始点"), SuffixLabel("s")] public float StartPoint;
        [LabelText("音量修正")]                  public float VolumeRevise;
#if UNITY_EDITOR
        private IEnumerable<string> GetAudioNames()
        {
            var ret = StorageUtil.LoadFromSettingFile("AudioConstant.BGMIds",    new List<string>());
            ret.AddRange(StorageUtil.LoadFromSettingFile("AudioConstant.SFXIds", new List<string>()));
            return ret;
        }
#endif
    }
}