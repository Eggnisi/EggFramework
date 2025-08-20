#region

//文件创建者：Egg
//创建时间：04-06 10:30

#endregion

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EggFramework
{
    /// <summary>
    /// 属性管理器默认实现
    /// </summary>
    public sealed class PropertyHandle : IPropertyHandle
    {
        private readonly Dictionary<string, IProperty> _properties = new();
        public           GameObject                    Owner { get; set; }

        public void RegisterProperty(IProperty property) =>
            _properties.TryAdd(property.PropertyId, property);

        public void ClearProperty() => _properties.Clear();
        public IReadOnlyList<IProperty> Properties => _properties.Values.ToList();

        public IProperty GetProperty(string propertyId) =>
            _properties.GetValueOrDefault(propertyId);

        public IBaseProperty GetBaseValueProperty(string propertyId) =>
            (IBaseProperty)_properties.GetValueOrDefault(propertyId);

        public IComputeProperty GetComputeValueProperty(string propertyId) =>
            (IComputeProperty)_properties.GetValueOrDefault(propertyId);

        public IProperty this[string id] => GetProperty(id);
    }
}