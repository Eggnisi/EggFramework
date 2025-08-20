#region

//文件创建者：Egg
//创建时间：04-05 10:25

#endregion

namespace EggFramework
{
    public sealed class BaseProperty : AbstractProperty, IBaseProperty
    {
        public BaseProperty(float baseValue, string propertyId)
        {
            ((IBaseProperty)this).SetBaseValue(baseValue);
            PropertyId = propertyId;
        }

        private float _baseValue;

        public override float GetValue()
        {
            if (IsDynamic)
            {
                var ret = _baseValue;
                ApplyModify(ref ret);
                return ret;
            }
            {
                if (!_isDirty) return _cacheValue;
                var ret = _baseValue;
                ApplyModify(ref ret);
                _cacheValue = ret;
                _isDirty    = false;
                return _cacheValue;
            }
        }

        void IBaseProperty.SetBaseValue(float value)
        {
            _baseValue = value;
            MakeDirty();
        }
    }
}