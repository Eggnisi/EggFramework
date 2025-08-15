#region

//文件创建者：Egg
//创建时间：03-27 10:50

#endregion

using EggFramework.AudioSystem;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("playSFX")]
    [CommandComment("运行时播放音效")]
    [CommandTokenSource(typeof(AudioTokenSource))]
    public sealed class PlaySFXHandle : CommandHandle<string>
    {
        protected override void Handle(
            [CommandComment("SFX名称")] string p1)
        {
            if (!Application.isPlaying)
            {
                CommandLogger.LogError("此命令只能在运行时执行");
                return;
            }

            TypeUtil.GetFirstActiveArchitecture().SendEvent(new PlaySFXEvent(p1));
        }
    }
}