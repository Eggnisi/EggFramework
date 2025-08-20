#region

//文件创建者：Egg
//创建时间：08-20 07:01

#endregion

namespace EggFramework
{
    public sealed class PropertyModifier : IPropertyModifier
    {
        public PropertyModifier(EPropertyModifierType modifierType, int priority, float value, IPropertyModifySource source = null)
        {
            ModifierType = modifierType;
            Priority     = priority;
            Value        = value;
            Source       = source;
        }
        public EPropertyModifierType ModifierType { get; }
        public int                   Priority     { get; }
        public bool                  IsDynamic    => false;
        public float                 Value        { get; }
        public IPropertyModifySource Source       { get; }
    }
}