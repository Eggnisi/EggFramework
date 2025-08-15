#region

//文件创建者：Egg
//创建时间：03-27 10:43

#endregion

using EggFramework.AudioSystem;
using UnityEngine;

namespace EggFramework.Util.EggCMD
{
    [CommandHandle("playBGM")]
    [CommandComment("运行时切换背景音乐")]
    [CommandTokenSource(typeof(AudioTokenSource))]
    public sealed class PlayBGMHandle : CommandHandle<string>
    {
        protected override void Handle(
            [CommandComment("BGM名称")] string p1)
        {
            if (!Application.isPlaying)
            {
                CommandLogger.LogError("此命令只能在运行时执行");
                return;
            }

            TypeUtil.GetFirstActiveArchitecture().SendEvent(new PlayBGMEvent(p1));
        }
    }
}