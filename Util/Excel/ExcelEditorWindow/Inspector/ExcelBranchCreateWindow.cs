#region

//文件创建者：Egg
//创建时间：11-19 10:53

#endregion

#if UNITY_EDITOR


using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public sealed class ExcelBranchCreateWindow : OdinEditorWindow
    {
        [HideInInspector] public ExcelOnlineView Host;

        [LabelText("分支名称")] public string Name;

        [Button("新建分支", ButtonSizes.Large)]
        private void CreateBranch()
        {
            if (string.IsNullOrEmpty(Name))
            {
                Debug.LogError("分支名不可以为空");
                return;
            }
            if (!EditorUtility.DisplayDialog("新建分支", $"是否创建分支{Name}?", "确定")) return;
            Close();
            Host.CreateBranch(Name);
        }
    }
}
#endif