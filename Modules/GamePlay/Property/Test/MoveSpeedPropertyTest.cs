#region

//文件创建者：Egg
//创建时间：04-05 11:44

#endregion

using System.Collections;
using UnityEngine;

namespace EggFramework
{
    public sealed class MoveSpeedPropertyTest : MonoBehaviour
    {
        private PropertyManager _propertyManager;

        private void Awake()
        {
            _propertyManager = GetComponent<PropertyManager>();
            new BaseValueProperty(100f, "MoveSpeed-Value-Config").Register(_propertyManager)
                .NotifyParentDirty("MoveSpeed-Value");
            new BaseValueProperty(0, "MoveSpeed-Value-Buff").Register(_propertyManager)
                .NotifyParentDirty("MoveSpeed-Value");
            new BaseValueProperty(0, "MoveSpeed-Value-Other").Register(_propertyManager)
                .NotifyParentDirty("MoveSpeed-Value").AddModifier(new PropertyClampModifier(-999, 50));
            new ComputeValueProperty(() => _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Config").GetValue() +
                                           _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Buff").GetValue() +
                                           _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Other").GetValue(),
                "MoveSpeed-Value").Register(_propertyManager).NotifyParentDirty("MoveSpeed").AddModifier(
                new PropertyClampModifier(
                    () => new Vector2(0,
                        _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Config").GetValue() * 2)));

            new BaseValueProperty(0, "MoveSpeed-Mul-Buff").Register(_propertyManager)
                .NotifyParentDirty("MoveSpeed-Mul").AddModifier(new PropertyClampModifier(-1, 0.5f));
            new BaseValueProperty(1, "MoveSpeed-Mul-Other").Register(_propertyManager)
                .NotifyParentDirty("MoveSpeed-Mul").AddModifier(new PropertyClampModifier(0, 1.5f));
            new ComputeValueProperty(() =>
                    (1 + _propertyManager.GetBaseValueProperty("MoveSpeed-Mul-Buff").GetValue()) *
                    _propertyManager.GetBaseValueProperty("MoveSpeed-Mul-Other").GetValue(),
                "MoveSpeed-Mul").Register(_propertyManager).NotifyParentDirty("MoveSpeed");
            new ComputeValueProperty(() => _propertyManager.GetComputeValueProperty("MoveSpeed-Mul").GetValue() *
                                           _propertyManager.GetComputeValueProperty("MoveSpeed-Value").GetValue(),
                "MoveSpeed").Register(_propertyManager);
        }

        private void TakeDrug(string drugId, float duration)
        {
            if (drugId == "加速药水")
            {
                var modifier = new PropertyOverrideModifier(20, 100);
                _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Buff").AddModifier(modifier);
                StartCoroutine(RemoveModifierAfter(duration, "MoveSpeed-Value-Buff", modifier));
            }

            if (drugId == "高级加速药水")
            {
                var modifier = new PropertyOverrideModifier(30, 200);
                _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Buff").AddModifier(modifier);
                StartCoroutine(RemoveModifierAfter(duration, "MoveSpeed-Value-Buff", modifier));
            }
        }

        private IEnumerator RemoveModifierAfter(float duration, string propertyId, IPropertyModifier modifier)
        {
            yield return new WaitForSeconds(duration);
            _propertyManager.GetProperty(propertyId).RemoveModifier(modifier);
        }

        //其他乱七八糟的效果
        private void AddEffectValue(string propName, float duration, float value)
        {
            if (propName == "MoveSpeed")
            {
                var modifier = new PropertyAdditiveModifier(value);
                _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Other").AddModifier(modifier);
                StartCoroutine(RemoveModifierAfter(duration, "MoveSpeed-Value-Other", modifier));
            }
        }
        
        private void AddEffectMul(string propName, float duration, float value)
        {
            if (propName == "MoveSpeed")
            {
                var modifier = new PropertyMultiplicativeModifier(value);
                _propertyManager.GetBaseValueProperty("MoveSpeed-Mul-Other").AddModifier(modifier);
                StartCoroutine(RemoveModifierAfter(duration, "MoveSpeed-Mul-Other", modifier));
            }
        }

        private IPropertyModifier _modifier;

        private void Equip(string equipId)
        {
            if (equipId == "小飞鞋" && _modifier == null)
            {
                _modifier = new PropertyOverrideModifier(() =>
                    2 * _propertyManager.GetBaseValueProperty("MoveSpeed-Value-Config").GetValue());
                _propertyManager.GetComputeValueProperty("MoveSpeed").AddModifier(_modifier);
            }
        }

        private void UnEquip(string equipId)
        {
            if (equipId == "小飞鞋" && _modifier != null)
            {
                _propertyManager.GetComputeValueProperty("MoveSpeed").RemoveModifier(_modifier);
            }
        }
    }
}