#region

//文件创建者：Egg
//创建时间：04-05 07:54

#endregion

using System;
using System.Collections.Generic;

namespace EggFramework
{
    public interface IProperty
    {
        PropertyHandle PropertyHandle { get; }
        void SetPropertyManager(PropertyHandle handle);
        IReadOnlyList<IPropertyModifier> Modifiers { get; }
        IReadOnlyList<IPropertyModifier> GetModifiersFromSource(IPropertyModifySource source);
        void AddModifier(IPropertyModifier modifier);
        void RemoveModifier(IPropertyModifier modifier);
        string PropertyId { get; }
        /// <summary>
        /// 是否是动态计算的属性，动态计算不会缓存值，每次获取值都会重新计算。
        /// </summary>
        bool IsDynamic { get; }
        Func<float> GetValueGetter() => GetValue;
        float GetValue();
        void MakeDirty();
        void OnDirty(Action action);
    }

    public static class PropertyExtensions
    {
        public static IProperty OnDirty(this IProperty self, Action action)
        {
            self.OnDirty(action);
            return self;
        }

        public static IProperty NotifyParentDirty(this IProperty self, string parentId)
        {
            self.OnDirty(() => self.PropertyHandle.GetProperty(parentId).MakeDirty());
            return self;
        }
    }
}