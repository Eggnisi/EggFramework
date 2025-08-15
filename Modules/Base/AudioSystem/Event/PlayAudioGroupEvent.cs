#region

//文件创建者：Egg
//创建时间：08-16 08:35

#endregion

namespace EggFramework.AudioSystem
{
    public struct PlayAudioGroupEvent
    {
        public readonly string GroupName;

        public PlayAudioGroupEvent(string groupName)
        {
            GroupName = groupName;
        }
    }
}