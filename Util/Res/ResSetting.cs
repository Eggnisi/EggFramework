#region

//文件创建者：Egg
//创建时间：10-24 01:28

#endregion

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Res
{
    [Serializable]
    public sealed class ResSetting
    {
        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("资源引用数据文件存放路径")]
        public string ResRefDataSavePath = "Assets/Data/ResRefData";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("资源引用数据检查器存放路径")]
        public string ResRefDataViewSavePath = "Assets/Data/ResRefDataView";

        [LabelText("资源引用文件Addressable资源组")] public string ResRefDataAddressableGroupName = "ResRefData";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("自定义资源引用文件生成代码路径")]
        public string CustomResRefCodePath = "Assets/Scripts/Generator/Res";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("预制体组生成代码路径")]
        public string EntityConstantCodePath = "Assets/Scripts/Generator/Prefab";

        [HideInInspector] public bool Setup;

        [LabelText("预制体组信息")] public List<PrefabGroup> PrefabGroups = new();
    }

    [LabelText("预制体组"), Serializable]
    public sealed class PrefabGroup
    {
        [LabelText("预制体组名")] public string Name;

        [LabelText("资源所在文件夹"), ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")]
        public List<string> Folders = new();
    }
}