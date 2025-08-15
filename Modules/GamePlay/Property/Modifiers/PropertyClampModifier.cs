#region

//文件创建者：Egg
//创建时间：04-05 11:10

#endregion

using System;
using UnityEngine;

namespace EggFramework
{
    public sealed class PropertyClampModifier : IPropertyClampModifier<float>
    {
        public PropertyClampModifier(Func<Vector2> valueGetter, int priority = 100)
        {
            _valueGetter = valueGetter;
            Priority     = priority;
        }

        public PropertyClampModifier(float minValue, float maxValue, int priority = 100)
        {
            _valueGetter = () => new Vector2(minValue, maxValue);
            Priority     = priority;
        }

        private readonly Func<Vector2> _valueGetter;
        public           int           Priority { get; }
        public           float         Min      => _valueGetter?.Invoke().x ?? 0;
        public           float         Max      => _valueGetter?.Invoke().y ?? 0;
    }
}