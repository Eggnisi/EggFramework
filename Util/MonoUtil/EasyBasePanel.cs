#region

//文件创建者：Egg
//创建时间：11-26 09:01

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EggFramework.MonoUtil
{
    public abstract class EasyBasePanel : MonoBehaviour
    {
        private readonly Dictionary<Type, Dictionary<string, UIBehaviour>> _uiBehaviours = new();

        protected virtual void Awake()
        {
            var uiBehaviours = GetComponentsInChildren<UIBehaviour>(true);

            Clear();

            foreach (var uiBehaviour in uiBehaviours)
            {
                var type = uiBehaviour.GetType();
                _uiBehaviours.TryAdd(type, new Dictionary<string, UIBehaviour>());
                _uiBehaviours[type][uiBehaviour.gameObject.name] = uiBehaviour;
            }
        }

        private void Clear()
        {
            _uiBehaviours.Clear();
        }

        protected T GetElement<T>(string elementName) where T : UIBehaviour
        {
            var type = typeof(T);
            
            foreach (var (key, value) in _uiBehaviours)
            {
                if (type.IsAssignableFrom(key))
                {
                    foreach (var (s, uiBehaviour) in value)
                    {
                        if (s == elementName) return (T)uiBehaviour;
                    }
                }
            }

            return null;
        }
    }
}