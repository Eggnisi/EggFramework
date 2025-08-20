#region

//文件创建者：Egg
//创建时间：04-05 07:55

#endregion

namespace EggFramework
{
    public interface IBaseProperty : IProperty
    {
        void SetBaseValue(float value);
    }

    public static class BaseValuePropertyExtensions
    {
        public static IBaseProperty AddModifier(this IBaseProperty self,
            IPropertyModifier modifier)
        {
            self.AddModifier(modifier);
            return self;
        }
        
        public static IBaseProperty RemoveModifier(this IBaseProperty self,
            IPropertyModifier modifier)
        {
            self.RemoveModifier(modifier);
            return self;
        }
        
        public static IBaseProperty Register(this IBaseProperty self,
            PropertyHandle handle)
        {
            handle.RegisterProperty(self);
            return self;
        }
    }
}