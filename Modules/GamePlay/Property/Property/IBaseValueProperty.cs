#region

//文件创建者：Egg
//创建时间：04-05 07:55

#endregion

namespace EggFramework
{
    public interface IBaseValueProperty<T> : IProperty<T>
    {
        void SetBaseValue(T value);
    }

    public static class BaseValuePropertyExtensions
    {
        public static IBaseValueProperty<T> AddModifier<T>(this IBaseValueProperty<T> self,
            IBasePropertyModifier modifier)
        {
            self.AddModifier(modifier);
            return self;
        }
        
        public static IBaseValueProperty<T> RemoveModifier<T>(this IBaseValueProperty<T> self,
            IBasePropertyModifier modifier)
        {
            self.RemoveModifier(modifier);
            return self;
        }
        
        public static IBaseValueProperty<float> Register(this IBaseValueProperty<float> self,
            PropertyManager manager)
        {
            manager.RegisterProperty(self.PropertyId, self);
            return self;
        }
    }
}