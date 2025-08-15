#region

//文件创建者：Egg
//创建时间：01-24 12:18

#endregion

#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Util.Res
{
    public abstract class ResRefDataView<T, TD> : ScriptableObject where T : Object where TD : ResRefData<T>
    {
        [HideInInspector] public TD Data;

        [ValueDropdown("@EggFramework.Util.DirectoryUtil.GetFilePaths()"), LabelText("路径过滤器"),
         OnValueChanged("Refresh", IncludeChildren = true)]
        public List<string> PathFilters = new();

        [LabelText("资源引用"), ListDrawerSettings(NumberOfItemsPerPage = 10), ReadOnly]
        public List<ResRefData<T>.ResRef> ResRefs = new();

        public void Refresh()
        {
            if (!Data)
            {
                Data = (TD)ResUtil.GetAsset(typeof(TD));
                if(!Data) ResUtil.RefreshResRef();
                Data = (TD)ResUtil.GetAsset(typeof(TD));
            }

            ResRefs = PathFilters.Count <= 0
                ? Data.ResRefs.ToList()
                : Data.ResRefs.Where(res => PathFilters.Any(path => res.Name.Contains(path))).ToList();
        }
    }
}
#endif