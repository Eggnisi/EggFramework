#region

//文件创建者：Egg
//创建时间：08-16 08:34

#endregion

namespace EggFramework.AudioSystem
{
    public struct PlayBGMEvent
    {
        public readonly string BGMName;

        public PlayBGMEvent(string bgmName)
        {
            BGMName = bgmName;
        }
    }
}