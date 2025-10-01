#region

//文件创建者：Egg
//创建时间：08-20 06:36

#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace EggFramework
{
    public abstract class AbstractProperty : IProperty
    {
        protected readonly List<IPropertyModifier> _modifiers = new();
        protected          float _cacheValue;
        protected          bool _isDirty;
        protected          Action _onDirty;
        public             IReadOnlyList<IPropertyModifier> Modifiers => _modifiers;
        public             IPropertyHandle PropertyHandle { get; private set; }
        public             string PropertyId { get; protected set; }
        public             bool IsDynamic => Modifiers.Any(mod => mod is DynamicPropertyModifier);
        public void SetPropertyHandle(IPropertyHandle handle) => PropertyHandle = handle;
        IReadOnlyList<IPropertyModifier> IProperty.Modifiers => _modifiers.AsReadOnly();
        
        public IReadOnlyList<IPropertyModifier> GetModifiersFromSource(IPropertyModifySource source) =>
            _modifiers.Where(mod => mod.Source == source).ToList();
        
        public void AddModifier(IPropertyModifier modifier)
        {
            _modifiers.Add(modifier);
            MakeDirty();
        }
        
        public void RemoveModifier(IPropertyModifier modifier)
        {
            _modifiers.Remove(modifier);
            MakeDirty();
        }
        
        protected void ProcessModifiers(ref float value, EPropertyModifierType type)
        {
            switch (type)
            {
                case EPropertyModifierType.Additive:
                    value += _modifiers.Where(mod => mod.ModifierType == EPropertyModifierType.Additive)
                        .OrderBy(m => m.Priority)
                        .Sum(mod => mod.Value);
                    break;
                case EPropertyModifierType.Multiply:
                    value = _modifiers.Where(mod => mod.ModifierType == EPropertyModifierType.Multiply)
                        .OrderBy(m => m.Priority)
                        .Aggregate(value, (current, mod) => current * mod.Value);
                    break;
                case EPropertyModifierType.Override:
                    var overrideMods = _modifiers.Where(mod => mod.ModifierType == EPropertyModifierType.Override)
                        .OrderBy(m => m.Priority).ToList();
                    if (overrideMods.Any()) value = overrideMods.Last().Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        protected void ApplyModify(ref float ret)
        {
            ProcessModifiers(ref ret, EPropertyModifierType.Additive);
            ProcessModifiers(ref ret, EPropertyModifierType.Multiply);
            ProcessModifiers(ref ret, EPropertyModifierType.Override);
        }
        
        public abstract float GetValue();
        
        public void MakeDirty()
        {
            _isDirty = true;
            _onDirty?.Invoke();
        }
        
        public void OnDirty(Action action) => _onDirty += action;
    }
}