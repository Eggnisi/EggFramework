#region

//文件创建者：Egg
//创建时间：04-05 08:00

#endregion

using System;

namespace EggFramework
{
    public interface IComputeProperty : IProperty
    {
        bool IsInitDynamic { get; }
        void SetValueGetter(Func<float> valueGetter);
    }

    public static class ComputeValuePropertyExtensions
    {
        public static IComputeProperty AddModifier(this IComputeProperty self,
            IPropertyModifier modifier)
        {
            self.AddModifier(modifier);
            return self;
        }

        public static IComputeProperty RemoveModifier(this IComputeProperty self,
            IPropertyModifier modifier)
        {
            self.RemoveModifier(modifier);
            return self;
        }

        public static IComputeProperty Register(this IComputeProperty self,
            PropertyHandle handle)
        {
            handle.RegisterProperty(self);
            return self;
        }
    }
}