#region

//文件创建者：Egg
//创建时间：04-05 11:09

#endregion

using System;

namespace EggFramework
{
    public sealed class PropertyMultiplicativeModifier : IPropertyMultiplicativeModifier<float>
    {
        public PropertyMultiplicativeModifier(Func<float> valueGetter, int priority = 100)
        {
            _valueGetter = valueGetter;
            Priority     = priority;
        }

        public PropertyMultiplicativeModifier(float value, int priority = 100)
        {
            _valueGetter = () => value;
            Priority     = priority;
        }

        private readonly Func<float> _valueGetter;
        public           int         Priority { get; }
        public           float       Value    => _valueGetter?.Invoke() ?? 0;
    }
}