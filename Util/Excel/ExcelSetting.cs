#region

//文件创建者：Egg
//创建时间：10-23 11:03

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    [Serializable]
    public sealed class ExcelSetting
    {
        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("Excel数据路径")]
        public string ExcelDataPathRoot = "Assets/Excel/Text";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("源数据路径")]
        public string DataPathRoot = "Assets/Excel/SOData";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("数据视图路径")]
        public string DataViewPathRoot = "Assets/Excel/DataView";

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()")] [LabelText("代码路径")]
        public string CodePathRoot = "Assets/Scripts/Generator/Excel";

        [LabelText("生成数据类代码命名空间")]            public string GenerateDataClassNameSpace = "EggFramework.Generator";
        [LabelText("生成Addressable资源组名")]      public string GenerateAddressableDataGroupName = "ExcelRefData";
        [HideInInspector]                     public bool Inited;
        [HideInInspector]                     public bool IsOnlineMode;
        [LabelText("结构体定义"), HideInInspector] public List<ExcelStructConfig> Configs = new();
#if UNITY_EDITOR
        [ShowInInspector, LabelText("结构体数据"),
         ListDrawerSettings(DraggableItems = false, HideAddButton = true, HideRemoveButton = true,
             DefaultExpandedState = true), JsonIgnore, HideReferenceObjectPicker]
        public List<ExcelStructColumn> Columns
        {
            get
            {
                return Configs?.Select((con, index) => new ExcelStructColumn(index, con)).ToList() ??
                       throw new InvalidOperationException();
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
            }
        }
#endif
    }

    [Serializable]
    public sealed class ExcelBranch
    {
        public string Name;
        public string Token;
        public string Url;
    }

    [Serializable]
    public sealed class ExcelOnlineSetting
    {
        [LabelText("飞书应用Id")] public string AppId;
        [LabelText("飞书应用密钥")] public string AppSecret;
        [LabelText("是否公开访问")] public bool   IsPublic;

        [LabelText("授权手机号"), HideIf("IsPublic")]
        public List<string> Mobiles = new();

        [LabelText("所有分支"), HideInInspector]  public List<ExcelBranch> Branches = new();
        [LabelText("当前分支名"), HideInInspector] public string            CurrentBranchName;

        [JsonIgnore]
        public string SheetToken => Branches.Find(brn => brn.Name == CurrentBranchName).Token;
    }
}