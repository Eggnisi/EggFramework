#region

//文件创建者：Egg
//创建时间：04-06 10:30

#endregion

using System.Collections.Generic;
using UnityEngine;

namespace EggFramework
{
    public sealed class PropertyManager : MonoBehaviour
    {
        private readonly Dictionary<string, IProperty<float>> _properties = new();

        public void RegisterProperty(string propertyId, IProperty<float> property) =>
            _properties.TryAdd(propertyId, property);

        public void ClearProperty() => _properties.Clear();

        public IProperty<float> GetProperty(string propertyId) =>
            _properties.GetValueOrDefault(propertyId);

        public IBaseValueProperty<float> GetBaseValueProperty(string propertyId) =>
            (IBaseValueProperty<float>)_properties.GetValueOrDefault(propertyId);

        public IComputeValueProperty<float> GetComputeValueProperty(string propertyId) =>
            (IComputeValueProperty<float>)_properties.GetValueOrDefault(propertyId);
    }
}