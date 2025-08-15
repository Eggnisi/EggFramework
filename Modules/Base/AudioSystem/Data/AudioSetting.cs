#region

//文件创建者：Egg
//创建时间：10-24 04:44

#endregion

using System;
using Sirenix.OdinInspector;

namespace EggFramework.AudioSystem
{
    [Serializable]
    public sealed class AudioSetting
    {
        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] 
        [LabelText("音频生成代码路径")] public string AudioCodePath = "Assets/Scripts/Generator/Audio";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] 
        [LabelText("音频数据路径")] public string AudioDataPath = "Assets/Data/AudioData";

        [LabelText("音频生成代码命名空间")] public string AudioNameSpace = "EggFramework.SimpleAudioSystem.Constant";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("音效文件夹路径")]
        public string AudioFXFolderPath = "Assets";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("背景音乐文件夹路径")]
        public string AudioBGMFolderPath = "Assets";
    }
}