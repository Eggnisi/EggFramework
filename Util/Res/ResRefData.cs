#region

//文件创建者：Egg
//创建时间：10-24 01:27

#endregion

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EggFramework.Util.Res
{
    public abstract class ResRefData<T> : ScriptableObject where T : Object
    {
        [Serializable]
        public struct ResRef
        {
            [LabelText("资源名"), LabelWidth(50f), HorizontalGroup("", Width = 400f), Multiline(3)]
            public string Name;

            [HorizontalGroup(""), LabelText("资源"), LabelWidth(50f),
             PreviewField(ObjectFieldAlignment.Center, Height = 100f)]
            public T Data;
        }

        [LabelText("资源引用"), ListDrawerSettings(NumberOfItemsPerPage = 10), ReadOnly]
        public List<ResRef> ResRefs = new();
    }
}
#endif