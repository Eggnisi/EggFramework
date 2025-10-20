#region

//文件创建者：Egg
//创建时间：10-20 10:16

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework.AudioSystem
{
    [Serializable,HideReferenceObjectPicker]
    public sealed class AudioReviseData
    {
        [LabelText("音频修正项"),HideReferenceObjectPicker]
        public List<AudioReviseItem> ReviseItems = new();
    }

    [Serializable,HideReferenceObjectPicker]
    public sealed class AudioReviseItem
    {
        [LabelText("目标音频")]
        public string TargetAudio;
        [LabelText("起始点"), SuffixLabel("s")]
        public float StartPoint;
        [LabelText("音量修正")]
        public float VolumeRevise;
    }
}