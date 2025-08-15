using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.Util;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace EggFramework
{
    [CreateAssetMenu(fileName = "BuffData", menuName = "BuffManager/BuffData", order = 1)]
    public class BuffData : ScriptableObject
    {
        [FoldoutGroup("通用")] [ReadOnly, LabelText("BuffId"), InfoBox("BuffId自动指定")]
        public string Id;

        [FoldoutGroup("通用")] [ReadOnly, LabelText("Buff名称"), InfoBox("Buff名称为资产名")]
        public string BuffName;

        [HorizontalGroup("通用/信息")] [TextArea(5, 200), LabelText("Buff描述")]
        public string Desc;

        [HorizontalGroup("通用/信息")] [LabelText("Buff图标"), PreviewField(100f)]
        public Sprite Icon;

        [FoldoutGroup("叠层与时效")] [LabelText("最大堆叠上限")]
        public int MaxStack = 1;

        [FoldoutGroup("叠层与时效")] [LabelText("是否为永久Buff")]
        public bool IsForever;

        [FoldoutGroup("叠层与时效")] [HideIf("@IsForever"), LabelText("持续时间")]
        public float Duration;

        [FoldoutGroup("回调参数")] [LabelText("Tick回调执行间隔")]
        public float TickTime = -1;

        [HorizontalGroup("回调参数/Enum")] [LabelText("Buff更新方式")]
        public BuffUpdateEnum BuffUpdateEnum;

        [HorizontalGroup("回调参数/Enum")] [LabelText("Buff移除方式")]
        public BuffRemoveStackEnum BuffRemoveStackEnum;

        [FoldoutGroup("回调参数")] [LabelText("新增Buff时触发Tick回调")]
        public bool TriggerTickOnCreate;

        [FoldoutGroup("回调与标签")] [LabelText("Buff回调")]
        public List<CustomBuffModule> BuffModules = new();

        [FoldoutGroup("回调与标签")] [LabelText("标签"), ValueDropdown("GetCustomTags", IsUniqueList = true)]
        public List<string> Tags = new();

        public bool ExistBuffModule(string callbackName)
        {
            return BuffModules.Exists(buff => buff.CallbackName == callbackName);
        }

        public bool ExistTag(string tag)
        {
            return Tags.Contains(tag);
        }

        public void TriggerBuffModule(string callbackName, BuffRunTimeInfo runTimeInfo, params object[] paramList)
        {
            runTimeInfo.TriggerBuffAction(callbackName);

            foreach (var buffModule in BuffModules)
            {
                if (buffModule.CallbackName == callbackName)
                {
                    buffModule.Callback?.Apply(runTimeInfo, paramList);
                }
            }
        }

        private static List<string> GetCustomTags()
        {
            return StorageUtil.LoadFromSettingFile(nameof(BuffConfig), new BuffConfig()).CustomTags;
        }

        public static readonly IReadOnlyList<string> InnerBuffTriggers = new List<string>()
        {
            "OnCreate",
            "OnRemove",
            "OnAddStack",
            "OnReduceStack",
            "OnTick"
        };

#if UNITY_EDITOR
        private void OnEnable()
        {
            BuffName = name;
            while (!BuffIdIsValid(Id))
            {
                Id = UuidUtil.GetRandomUuid();
            }
        }

        private bool BuffIdIsValid(string buffId)
        {
            var buffs = ResUtil.GetCustomAssets<BuffData>();
            return buffs.All(buff => buff == this || buff.Id != buffId) && !string.IsNullOrEmpty(buffId);
        }
#endif
    }

    [Serializable]
    public struct CustomBuffModule
    {
        private static List<string> GetCustomTriggers()
        {
            var custom = StorageUtil.LoadFromSettingFile(nameof(BuffConfig), new BuffConfig()).CustomTriggers;
            custom.AddRange(BuffData.InnerBuffTriggers);
            return custom;
        }

        [LabelText("回调模块")] public BuffSerializeCallback Callback;

        [LabelText("回调时机"), ValueDropdown("GetCustomTriggers")]
        public string CallbackName;
    }
}