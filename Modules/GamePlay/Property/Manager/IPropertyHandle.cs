#region

//文件创建者：Egg
//创建时间：08-20 06:58

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace EggFramework
{
    /// <summary>
    /// 属性管理器接口
    /// </summary>
    public interface IPropertyHandle
    {
        GameObject Owner { get; }
        void RegisterProperty(IProperty property);
        void ClearProperty();
        IReadOnlyList<IProperty> Properties { get; }
        IProperty GetProperty(string propertyId);
        IBaseProperty GetBaseValueProperty(string propertyId);
        IComputeProperty GetComputeValueProperty(string propertyId);
    }
}