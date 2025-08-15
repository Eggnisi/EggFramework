#region

//文件创建者：Egg
//创建时间：04-05 10:44

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EggFramework
{
    public sealed class ComputeValueProperty : IComputeValueProperty<float>
    {
        public ComputeValueProperty(Func<float> valueGetter, string propertyId)
        {
            ((IComputeValueProperty<float>)this).SetValueGetter(valueGetter);
            PropertyId = propertyId;
        }

        public PropertyManager                            PropertyManager { get; set; }
        IReadOnlyList<IPropertyModifier> IProperty<float>.Modifiers       => _modifiers;

        public IReadOnlyList<IComputePropertyModifier> Modifiers =>
            _modifiers.OfType<IComputePropertyModifier>().ToList();

        public void AddModifier(IComputePropertyModifier modifier)
        {
            ((IProperty<float>)this).AddModifier(modifier);
        }

        void IProperty<float>.AddModifier(IPropertyModifier modifier)
        {
            _modifiers.Add(modifier);
            MakeDirty();
        }

        public void RemoveModifier(IComputePropertyModifier modifier)
        {
            ((IProperty<float>)this).RemoveModifier(modifier);
        }

        void IProperty<float>.RemoveModifier(IPropertyModifier modifier)
        {
            _modifiers.Remove(modifier);
            MakeDirty();
        }

        public string PropertyId { get; }

        private readonly List<IPropertyModifier> _modifiers = new();

        private Func<float> _valueGetter;
        private float       _cacheValue;
        private bool        _isDirty;
        private Action      _onDirty;

        private void ProcessModifiers(ref float value, EModifierMergeType type)
        {
            switch (type)
            {
                case EModifierMergeType.Override:
                    var overrideMods = _modifiers.OfType<IPropertyOverrideModifier<float>>()
                        .OrderBy(m => m.Priority).ToList();
                    if (overrideMods.Any()) value = overrideMods.Last().Value; // 若Override类型以最后一个为准
                    break;
                case EModifierMergeType.Clamp:
                    value = _modifiers.OfType<IPropertyClampModifier<float>>()
                        .OrderBy(m => m.Priority)
                        .Aggregate(value, (current, mod) => Mathf.Clamp(current, mod.Min, mod.Max));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public float GetValue()
        {
            if (!_isDirty) return _cacheValue;
            var ret = _valueGetter?.Invoke() ?? 0;
            ApplyModify(ref ret);
            _cacheValue = ret;
            _isDirty    = false;
            return _cacheValue;
        }

        private void ApplyModify(ref float ret)
        {
            ProcessModifiers(ref ret, EModifierMergeType.Clamp);
            ProcessModifiers(ref ret, EModifierMergeType.Override);
        }

        public void MakeDirty()
        {
            _isDirty = true;
            _onDirty?.Invoke();
        }

        public void OnDirty(Action action)
        {
            _onDirty += action;
        }

        void IComputeValueProperty<float>.SetValueGetter(Func<float> valueGetter)
        {
            _valueGetter = valueGetter;
            MakeDirty();
        }
    }
}