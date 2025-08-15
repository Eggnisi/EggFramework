#region

//文件创建者：Egg
//创建时间：04-05 10:03

#endregion

namespace EggFramework
{
    public interface IPropertyOverrideModifier<out T> : IBasePropertyModifier, IComputePropertyModifier
    {
        T Value { get; }
    }
}