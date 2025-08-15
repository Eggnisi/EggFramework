#region

//文件创建者：Egg
//创建时间：04-05 07:54

#endregion

using System;
using System.Collections.Generic;

namespace EggFramework
{
    public interface IProperty<out T>
    {
        PropertyManager                  PropertyManager { get; }
        IReadOnlyList<IPropertyModifier> Modifiers       { get; }
        void AddModifier(IPropertyModifier modifier);
        void RemoveModifier(IPropertyModifier modifier);
        string PropertyId { get; }

        Func<T> GetValueGetter() => GetValue;
        T GetValue();
        void MakeDirty();
        void OnDirty(Action action);
    }

    public static class PropertyExtensions
    {
        public static IProperty<float> OnDirty(this IProperty<float> self, Action action)
        {
            self.OnDirty(action);
            return self;
        }

        public static IProperty<float> NotifyParentDirty(this IProperty<float> self, string parentId)
        {
            self.OnDirty(() => { self.PropertyManager.GetProperty(parentId).MakeDirty(); });
            return self;
        }
    }
}