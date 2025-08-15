#region

//文件创建者：Egg
//创建时间：04-21 10:03

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EggFramework.Util;
using EggFramework.Util.Res;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.ObjectPool
{
    [Serializable]
    public sealed class ObjectPoolConfig
    {
        [LabelText("实体Id"), HideIf("IsDefaultGroup"), ValueDropdown("GetPrefabIds")]
        public string EntityId;

        [LabelText("低水位"), Tooltip("初始化的预加载数量")]
        public int PreloadCount = 10;

        [LabelText("高水位"), Tooltip("对象池的最大数量上限")]
        public int Capacity = 100;

        [LabelText("扩容模式")] public EObjectPoolExpandMode  ExpandMode  = EObjectPoolExpandMode.PredictiveExpand;
        [LabelText("释放模式")] public EObjectPoolReleaseMode ReleaseMode = EObjectPoolReleaseMode.PredictiveRelease;
        [LabelText("到达容量上限后是否强制回收")] public bool ForceRetrieve;
        
        [LabelText("@ReleaseMode == EObjectPoolReleaseMode.UsePoolTimeout ? \"池静置释放时间\" : \"对象静置释放时间\""),
         ShowIf(
             "@ReleaseMode == EObjectPoolReleaseMode.UsePoolTimeout || ReleaseMode == EObjectPoolReleaseMode.UseObjectTimeout")]
        public float TimeOut;
#if UNITY_EDITOR
        [HideInInspector] public bool IsDefaultGroup;

        private IEnumerable<string> GetPrefabIds()
        {
            var resSetting = StorageUtil.LoadFromSettingFile(nameof(ResSetting), new ResSetting());
            var ret        = new List<string>();
            foreach (var group in resSetting.PrefabGroups)
            {
                if (!string.IsNullOrEmpty(group.Name))
                {
                    var prefabs = ResUtil.GetPrefabPaths(group.Folders);
                    ret.AddRange(prefabs.Select(Path.GetFileNameWithoutExtension));
                }
            }

            return ret;
        }
#endif
    }
}