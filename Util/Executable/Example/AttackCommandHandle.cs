#region

//文件创建者：Egg
//创建时间：07-24 03:51

#endregion

using EggFramework.Util.EggCMD;
using UnityEngine;

namespace EggFramework.Executable
{
    [CommandHandle("attack")]
    public sealed class AttackCommandHandle : CommandHandle<int, string>
    {
        protected override void Handle(int p1, string p2)
        {
            Debug.Log($"attack {p1} times with weapon {p2}");
        }
    }
}