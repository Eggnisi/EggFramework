#region

//文件创建者：Egg
//创建时间：07-26 09:28

#endregion

#if UNITY_EDITOR

using Sirenix.OdinInspector;

namespace EggFramework
{
    public sealed partial class BuffHandle
    {
        [Button]
        private void AddBuff(BuffData buff)
        {
            AddBuff(new BuffRunTimeInfo()
            {
                Target = null,
                BuffData = buff
            });
        }
    }
}
#endif