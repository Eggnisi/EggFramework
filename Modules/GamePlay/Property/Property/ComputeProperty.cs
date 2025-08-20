#region

//文件创建者：Egg
//创建时间：04-05 10:44

#endregion

using System;

namespace EggFramework
{
    public sealed class ComputeProperty : AbstractProperty, IComputeProperty
    {
        public ComputeProperty(Func<float> valueGetter, string propertyId, bool isInitDynamic = false)
        {
            ((IComputeProperty)this).SetValueGetter(valueGetter);
            PropertyId    = propertyId;
            IsInitDynamic = isInitDynamic;
        }
        
        private Func<float> _valueGetter;
        
        public override float GetValue()
        {
            if (IsInitDynamic || IsDynamic)
            {
                var ret = _valueGetter?.Invoke() ?? 0;
                ApplyModify(ref ret);
                return ret;
            }
            
            {
                if (!_isDirty) return _cacheValue;
                var ret = _valueGetter?.Invoke() ?? 0;
                ApplyModify(ref ret);
                _cacheValue = ret;
                _isDirty    = false;
                return _cacheValue;
            }
        }
        
        public bool IsInitDynamic { get; }
        
        void IComputeProperty.SetValueGetter(Func<float> valueGetter)
        {
            _valueGetter = valueGetter;
            MakeDirty();
        }
    }
}