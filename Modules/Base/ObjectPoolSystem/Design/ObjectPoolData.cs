#region

//文件创建者：Egg
//创建时间：04-21 09:23

#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.ObjectPool
{
    [CreateAssetMenu(menuName = "Data/ObjectPoolData")]
    public sealed class ObjectPoolData : ScriptableObject
    {
        [LabelText("默认池配置信息"), Tooltip("如果不采用特殊配置，默认使用这个")] [InfoBox("默认配置信息实体名字段无效")]
        public ObjectPoolConfig DefaultPoolConfig;

        [LabelText("池配置信息")] public List<ObjectPoolConfig> PoolConfigs = new();
    }
}