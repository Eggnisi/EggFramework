#region

//文件创建者：Egg
//创建时间：04-05 07:57

#endregion

namespace EggFramework
{
    public enum EPropertyModifierType
    {
        Additive,
        Multiply,
        Override,
    }
    
    public interface IPropertyModifier
    {
        EPropertyModifierType ModifierType { get; }
        int                   Priority     { get; }
        bool                  IsDynamic    { get; }
        float                 Value        { get; }
        IPropertyModifySource Source       { get; }
    }
}