#region

//文件创建者：Egg
//创建时间：05-20 07:50

#endregion

#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EggFramework.Util.Excel
{
    [Serializable]
    public sealed class ExcelOnlineView
    {
        [LabelText("设置")] public ExcelOnlineSetting Setting;

        public ExcelOnlineView(ExcelOnlineSetting setting)
        {
            Setting = setting;
            Columns = Setting.Branches.Select(brn => new ExcelBranchColumn(this, brn)).ToList();
        }

        [LabelText("当前分支"), ShowInInspector] public string CurrentBranch => Setting.CurrentBranchName;

        [LabelText("当前分支线上表网址"), ShowInInspector]
        public string CurrentUrl => Setting.Branches.Find(brn => brn.Name == Setting.CurrentBranchName)?.Url;

        [LabelText("分支列表"), ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        public List<ExcelBranchColumn> Columns;

        public void Switch(string branchName)
        {
            if (branchName == CurrentBranch) return;

            var branch = Setting.Branches.Find(brn => brn.Name == branchName);
            if (branch != null)
            {
                Setting.CurrentBranchName = branch.Name;
            }
        }

        [Button("新建分支", ButtonSizes.Large)]
        public void ShowBranchCreateWindow()
        {
            var window = EditorWindow.GetWindow<ExcelBranchCreateWindow>();
            window.Host = this;
            window.Show();
        }

        [ShowIf("@!string.IsNullOrEmpty(CurrentBranch)")]
        [Button("@\"拉取\" + CurrentBranch + \"分支数据\"", ButtonSizes.Large)]
        public void PullBranchData()
        {
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            PythonUtil.PullSheet(Setting.AppId, Setting.AppSecret, Setting.SheetToken);
        }

        [ShowIf("@!string.IsNullOrEmpty(CurrentBranch)")]
        [Button("@\"更新\" + CurrentBranch + \"分支表头\"", ButtonSizes.Large)]
        private void UpdateRemoteSheet() => UpdateRemoteSheet(CurrentBranch);

        [ShowIf("@!string.IsNullOrEmpty(CurrentBranch)")]
        [Button("@\"更新\" + CurrentBranch + \"分支权限\"", ButtonSizes.Large)]
        private void UpdateRemoteAuthority()
        {
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            PythonUtil.UpdateSheetAuthority(Setting.AppId, Setting.AppSecret, Setting.SheetToken);
        }

        private void UpdateRemoteSheet(string name)
        {
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            PythonUtil.UpdateRemoteSheet(Setting.AppId, Setting.AppSecret, name);
        }

        public void CreateBranch(string branchName)
        {
            if (Setting.Branches.Any(brn => brn.Name == branchName))
            {
                Debug.LogError("不允许重复的分支名");
                return;
            }
            var branch = new ExcelBranch
            {
                Name  = branchName,
                Token = null,
                Url   = null
            };
            if (Setting.Branches.Count == 0)
            {
                Setting.CurrentBranchName = branchName;
            }

            Setting.Branches.Add(branch);
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
            UpdateRemoteSheet(branchName);
        }

        public void Delete(string branchName)
        {
            var branch = Setting.Branches.Find(brn => brn.Name == branchName);
            if (branch == null) return;
            if (!EditorUtility.DisplayDialog("删除分支", $"确定要删除分支{branchName}么？", "朕意已决")) return;
            Setting.Branches.Remove(branch);
            EditorWindow.GetWindow<ExcelEditorWindow>().Close();
        }
    }
}
#endif