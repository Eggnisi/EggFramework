#region

//文件创建者：Egg
//创建时间：07-24 03:52

#endregion

using EggFramework.Util.EggCMD;
using UnityEngine;

namespace EggFramework.Executable
{
    [CommandHandle("heal")]
    public sealed class HealCommandHandle : CommandHandle<int, string>
    {
        protected override void Handle(int p1, string p2)
        {
            Debug.Log($"heal {p1} HP with item {p2}, 这是两个参数的重载版本");
        }
    }
    
    [CommandHandle("heal")]
    public sealed class HealCommandHandle1 : CommandHandle<int>
    {
        protected override void Handle(int p1)
        {
            Debug.Log($"heal {p1} HP , 这是一个参数的重载版本");
        }
    }
}