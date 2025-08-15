#region

//文件创建者：Egg
//创建时间：04-20 09:28

#endregion

using System;
using System.Diagnostics;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EggFramework.AudioSystem
{
    [InlineProperty, Serializable]
    public struct AudioSystemModifierParam
    {
        [LabelText("操作参数1"), HorizontalGroup] public float Value1;

        [LabelText("操作参数2"), HorizontalGroup, ShowIf("ShowParam2")]
        public float Value2;

#if UNITY_EDITOR
        [HideInInspector] public bool ShowParam2;
#endif
    }

    [Serializable]
    public sealed class AudioSystemModifier
    {
        [LabelText("属性")] public EModifyProperty Property;

        [LabelText("操作类型"), OnValueChanged("RefreshParam")]
        public EModifyType Type;

        [LabelText("参数"), OnValueChanged("RefreshParam")]
        public AudioSystemModifierParam Param;

        [ShowIf("@Type == EModifyType.Add || Type == EModifyType.Multiply"), OnValueChanged("RefreshParam")]
        [LabelText("加算或乘算是否随机值")]
        public bool IsRandom;
#if UNITY_EDITOR
        
        private void RefreshParam()
        {
            if (IsRandom && Type is EModifyType.Add or EModifyType.Multiply ||
                Type is EModifyType.Override or EModifyType.Clamp)
            {
                Param.ShowParam2 = true;
            }
            else Param.ShowParam2 = false;
        }
#endif

        public enum EModifyProperty
        {
            Volume,
            Pitch
        }

        public enum EModifyType
        {
            [LabelText("加")]  Add,
            [LabelText("乘")]  Multiply,
            [LabelText("限制")] Clamp,
            [LabelText("覆盖")] Override
        }

        public void Modify(AudioSource source)
        {
            switch (Type)
            {
                case EModifyType.Add:
                    switch (Property)
                    {
                        case EModifyProperty.Volume:
                            source.volume += IsRandom ? Random.Range(Param.Value1, Param.Value2) : Param.Value1;
                            break;
                        case EModifyProperty.Pitch:
                            source.pitch += IsRandom ? Random.Range(Param.Value1, Param.Value2) : Param.Value1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EModifyType.Multiply:
                    switch (Property)
                    {
                        case EModifyProperty.Volume:
                            source.volume *= IsRandom ? Random.Range(Param.Value1, Param.Value2) : Param.Value1;
                            break;
                        case EModifyProperty.Pitch:
                            source.pitch *= IsRandom ? Random.Range(Param.Value1, Param.Value2) : Param.Value1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EModifyType.Clamp:
                    switch (Property)
                    {
                        case EModifyProperty.Volume:
                            source.volume = Mathf.Clamp(source.volume, Param.Value1, Param.Value2);
                            break;
                        case EModifyProperty.Pitch:
                            source.pitch = Mathf.Clamp(source.volume, Param.Value1, Param.Value2);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                case EModifyType.Override:
                    switch (Property)
                    {
                        case EModifyProperty.Volume:
                            source.volume = Param.Value1;
                            break;
                        case EModifyProperty.Pitch:
                            source.pitch = Param.Value1;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}