using System;
using QFramework;
using UnityEngine;

namespace EggFramework.MonoUtil
{
    public sealed class Trigger2D : MonoBehaviour
    {
        private Action<Collider2D> _onEnterAction;
        private Action<Collider2D> _onExitAction;
        private Action<Collider2D> _onStayAction;

        private void OnTriggerEnter2D(Collider2D other)
        {
            _onEnterAction?.Invoke(other);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            _onExitAction?.Invoke(other);
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            _onStayAction?.Invoke(other);
        }

        public IUnRegister RegisterOnEnter(Action<Collider2D> action)
        {
            _onEnterAction += action;
            return new CustomUnRegister(()=> _onEnterAction -= action);
        }

        public IUnRegister RegisterOnExit(Action<Collider2D> action)
        {
            _onExitAction += action;
            return new CustomUnRegister(()=> _onExitAction -= action);
        }

        public IUnRegister RegisterOnStay(Action<Collider2D> action)
        {
            _onStayAction += action;
            return new CustomUnRegister(()=> _onStayAction -= action);
        }
    }
}