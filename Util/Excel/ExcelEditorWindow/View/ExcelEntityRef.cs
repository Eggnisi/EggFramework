#region

//文件创建者：Egg
//创建时间：03-22 03:23

#endregion

#if UNITY_EDITOR


using System.Collections.Generic;
using UnityEngine;

namespace EggFramework.Util.Excel
{
    public abstract class ExcelEntityRef<T> where T : IExcelEntity
    {
        [HideInInspector] public List<string> RawData = new();
        public abstract          int          MaxRowIndex { get; }

        protected string GetRawData(int index, string defaultValue = "")
        {
            if (index >= 0 && index < RawData.Count) return RawData[index];
            return defaultValue;
        }

        protected void SetRawData(int index, string value)
        {
            while (RawData.Count <= MaxRowIndex)
            {
                RawData.Add(string.Empty);
            }

            if (index >= 0 && index < RawData.Count)
            {
                RawData[index] = value;
            }
        }
    }
}
#endif