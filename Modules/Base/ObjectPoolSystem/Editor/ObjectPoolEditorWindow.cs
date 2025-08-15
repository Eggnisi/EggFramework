#region

//文件创建者：Egg
//创建时间：04-21 06:37

#endregion

#if UNITY_EDITOR


using EggFramework.AudioSystem;
using EggFramework.Util;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EggFramework.ObjectPool.Editor
{
    public sealed class ObjectPoolEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/GamePlay/对象池配置")]
        public static void OpenWindow()
        {
            var window = GetWindow<ObjectPoolEditorWindow>();
            window.titleContent = new GUIContent("对象池配置");
        }
        
        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree { { "配置信息", GetObjectPoolData() } };
            return tree;
        }

        private static ObjectPoolData GetObjectPoolData()
        {
            const string path = "Assets/Data/ObjectPool/ObjectPoolData.asset";
            var          data = ResUtil.GetOrCreateAsset<ObjectPoolData>(path);
            data.DefaultPoolConfig.IsDefaultGroup = true;
            return data;
        }
    }
}
#endif