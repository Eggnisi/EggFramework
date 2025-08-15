#region

//文件创建者：Egg
//创建时间：05-20 07:52

#endregion

#if UNITY_EDITOR

using System;
using Sirenix.OdinInspector;

namespace EggFramework.Util.Excel
{
    [Serializable]
    public sealed class ExcelBranchColumn
    {
        private ExcelOnlineView _host;
        private ExcelBranch     _branch;

        public ExcelBranchColumn(ExcelOnlineView host, ExcelBranch branch)
        {
            _host   = host;
            _branch = branch;
        }

        [ShowInInspector, HorizontalGroup("Button"), LabelText("分支名")]
        public string Name => _branch.Name;

        [HorizontalGroup("Button", 100f), Button("切换到此分支")]
        public void Switch() =>  _host.Switch(_branch.Name);
        
        [HorizontalGroup("Button", 75f), Button("删除分支")]
        public void Delete() =>  _host.Delete(_branch.Name);
    }
}
#endif