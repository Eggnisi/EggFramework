#region

//文件创建者：Egg
//创建时间：09-11 06:42

#endregion

using EggFramework.AudioSystem;
using QFramework;

namespace EggFramework
{
    public static class SoundExtension
    {
        public static void PlaySFX(this ICanSendEvent self, string soundName)
        {
            self.SendEvent(new PlaySFXEvent(soundName));
        }

        public static void PlayBGM(this ICanSendEvent self, string soundName)
        {
            self.SendEvent(new PlayBGMEvent(soundName));
        }
        
        public static void PlayAudioGroup(this ICanSendEvent self, string soundName)
        {
            self.SendEvent(new PlayAudioGroupEvent(soundName));
        }
    }
}