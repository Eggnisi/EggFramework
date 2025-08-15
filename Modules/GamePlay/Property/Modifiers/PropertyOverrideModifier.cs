#region

//文件创建者：Egg
//创建时间：04-05 11:10

#endregion

using System;

namespace EggFramework
{
    public sealed class PropertyOverrideModifier : IPropertyOverrideModifier<float>
    {
        public PropertyOverrideModifier(Func<float> valueGetter, int priority = 100)
        {
            _valueGetter = valueGetter;
            Priority     = priority;
        }

        public PropertyOverrideModifier(float value, int priority = 100)
        {
            _valueGetter = () => value;
            Priority     = priority;
        }

        private readonly Func<float> _valueGetter;
        public           int         Priority { get; }
        public           float       Value    => _valueGetter?.Invoke() ?? 0;
    }
}