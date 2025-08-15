#region

//文件创建者：Egg
//创建时间：04-05 10:01

#endregion

namespace EggFramework
{
    public interface IPropertyAdditiveModifier<out T> : IBasePropertyModifier
    {
        T Value { get; }
    }
}