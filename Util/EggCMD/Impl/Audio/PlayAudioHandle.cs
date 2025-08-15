#region

//文件创建者：Egg
//创建时间：03-26 05:32

#endregion

using EggFramework.AudioSystem;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("playAudio")]
    [CommandComment("运行时播放音频")]
    [EnableOption("-sfx -s", "指定为sfx,否则为bgm")]
    public sealed class PlayAudioHandle : CommandHandle<string>
    {
        [CommandComment(                               "播放音频")]
        protected override void Handle([CommandComment("音频名称")] string p1)
        {
            if (!Application.isPlaying)
            {
                CommandLogger.LogError("此命令只能在运行时执行");
                return;
            }

            if (_context.OptionExist("-sfx", "-s"))
                TypeUtil.GetFirstActiveArchitecture().SendEvent(new PlaySFXEvent(p1));
            else
            {
                TypeUtil.GetFirstActiveArchitecture().SendEvent(new PlayBGMEvent(p1));
            }
        }
    }
}