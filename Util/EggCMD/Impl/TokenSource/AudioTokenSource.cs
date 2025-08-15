#region

//文件创建者：Egg
//创建时间：03-27 10:30

#endregion

using System.Collections.Generic;

namespace EggFramework.Util.EggCMD
{
    public sealed class AudioTokenSource : ICommandTokenSource
    {
        public List<string> GetTokenSource(List<string> tokens)
        {
            if (tokens.Count <= 0) return new List<string>();
            if (tokens[0] == "playBGM")
            {
                return StorageUtil.LoadFromSettingFile("AudioConstant.BGMIds", new List<string>());
            }

            if (tokens[0] == "playSFX")
            {
                return StorageUtil.LoadFromSettingFile("AudioConstant.SFXIds", new List<string>());
            }

            CommandLogger.LogError(
                $"Invalid token source:{tokens[0]} 初始化错误， AudioTokenSource只能用于 playBGM 或 playAudio 命令");
            return new List<string>();
        }
    }
}