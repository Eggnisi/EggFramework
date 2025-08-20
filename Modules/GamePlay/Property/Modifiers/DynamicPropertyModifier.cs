#region

//文件创建者：Egg
//创建时间：08-20 07:26

#endregion

using System;

namespace EggFramework
{
    public sealed class DynamicPropertyModifier : IPropertyModifier
    {
        public DynamicPropertyModifier(EPropertyModifierType modifierType, Func<float> valueProvider, int priority,
            IPropertyModifySource source = null)
        {
            ModifierType   = modifierType;
            _valueProvider = valueProvider;
            Priority       = priority;
            Source         = source;
        }
        
        public EPropertyModifierType ModifierType { get; }
        public int                   Priority     { get; }
        public bool                  IsDynamic   => true;
        public IPropertyModifySource Source       { get; }
        public float                 Value        => _valueProvider?.Invoke() ?? 0f;
        
        private readonly Func<float> _valueProvider;
    }
}