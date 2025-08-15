#region

//文件创建者：Egg
//创建时间：04-05 08:00

#endregion

using System;

namespace EggFramework
{
    public interface IComputeValueProperty<T> : IProperty<T>
    {
        void SetValueGetter(Func<T> valueGetter);
    }

    public static class ComputeValuePropertyExtensions
    {
        public static IComputeValueProperty<T> AddModifier<T>(this IComputeValueProperty<T> self,
            IComputePropertyModifier modifier)
        {
            self.AddModifier(modifier);
            return self;
        }

        public static IComputeValueProperty<T> RemoveModifier<T>(this IComputeValueProperty<T> self,
            IComputePropertyModifier modifier)
        {
            self.RemoveModifier(modifier);
            return self;
        }

        public static IComputeValueProperty<float> Register(this IComputeValueProperty<float> self,
            PropertyManager manager)
        {
            manager.RegisterProperty(self.PropertyId, self);
            return self;
        }
    }
}