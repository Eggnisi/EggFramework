#region

//文件创建者：Egg
//创建时间：04-20 10:14

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EggFramework.AudioSystem
{
    [Serializable]
    public sealed class AudioOutputSetting
    {
        [LabelText("输出总线路径")]
        public string OutputMixerGroupPath;

        [LabelText("关联音频组")]
        public List<string> AudioGroups = new();
    }
}