#region

//文件创建者：Egg
//创建时间：02-21 11:16

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace EggFramework
{
    public sealed class BuffDataRef : ScriptableObject
    {
        public List<BuffData> BuffDatas = new();
    }
}