#region

//文件创建者：Egg
//创建时间：02-21 11:16

#endregion

#if UNITY_EDITOR
using System.Linq;
using EggFramework.Util;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EggFramework
{
    public sealed class BuffEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/GamePlay/Buff管理面板 #&b")]
        private static void OpenWindow()
        {
            GetWindow<BuffEditorWindow>("Buff管理面板");
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Config = StorageUtil.LoadFromSettingFile(nameof(BuffConfig), new BuffConfig());
        }

        protected override void OnDisable()
        {
            StorageUtil.SaveToSettingFile(nameof(BuffConfig), Config);
            base.OnDisable();
        }

        [LabelText("配置信息")] public BuffConfig Config;


        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree { { "控制面板", new BuffEditorWindowView(Config) } };

            var buffDatas = ResUtil.GetCustomAssets<BuffData>();
            foreach (var tag in Config.CustomTags)
            {
                foreach (var buffData in buffDatas.Where(buff => buff.Tags.Contains(tag)))
                {
                    tree.Add($"BuffData/{tag}/{buffData.name}", buffData);
                }
            }

            foreach (var buffData in buffDatas.Where(buff => buff.Tags.Count <= 0))
            {
                tree.Add("BuffData/NoTag/" + buffData.name, buffData);
            }

            return tree;
        }
    }
}
#endif