#region

//文件创建者：Egg
//创建时间：08-16 08:35

#endregion

namespace EggFramework.AudioSystem
{
    public struct PlaySFXEvent
    {
        public readonly string FXName;

        public PlaySFXEvent(string fxName)
        {
            FXName = fxName;
        }
    }
}