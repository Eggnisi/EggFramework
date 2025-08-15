#region

//文件创建者：Egg
//创建时间：03-06 02:27

#endregion

using System;
using QFramework;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EggFramework.MonoUtil
{
    public sealed class PointerTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
        IPointerDownHandler, IPointerClickHandler, IDragHandler
    {
        private Action<PointerEventData> _onPointerEnter;
        private Action<PointerEventData> _onPointerExit;
        private Action<PointerEventData> _onPointerUp;
        private Action<PointerEventData> _onPointerDown;
        private Action<PointerEventData> _onPointerClick;
        private Action<PointerEventData> _onDrag;

        public IUnRegister RegisterOnPointerEnter(Action<PointerEventData> action)
        {
            _onPointerEnter += action;
            return new CustomUnRegister(() => _onPointerEnter -= action);
        }

        public IUnRegister RegisterOnPointerExit(Action<PointerEventData> action)
        {
            _onPointerExit += action;
            return new CustomUnRegister(() => _onPointerExit -= action);
        }

        public IUnRegister RegisterOnPointerUp(Action<PointerEventData> action)
        {
            _onPointerUp += action;
            return new CustomUnRegister(() => _onPointerUp -= action);
        }

        public IUnRegister RegisterOnPointerDown(Action<PointerEventData> action)
        {
            _onPointerDown += action;
            return new CustomUnRegister(() =>
            {
                _onPointerDown -= action;
            });
        }

        public IUnRegister RegisterOnPointerClick(Action<PointerEventData> action)
        {
            _onPointerClick += action;
            return new CustomUnRegister(() =>
            {
                _onPointerClick -= action;
            });
        }
        
        public IUnRegister RegisterOnDrag(Action<PointerEventData> action)
        {
            _onDrag += action;
            return new CustomUnRegister(() =>
            {
                _onDrag -= action;
            });
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            _onPointerEnter?.Invoke(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            _onPointerExit?.Invoke(eventData);
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _onPointerUp?.Invoke(eventData);
        }

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _onPointerDown?.Invoke(eventData);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            _onPointerClick?.Invoke(eventData);
        }

        void IDragHandler.OnDrag(PointerEventData eventData)
        {
            _onDrag?.Invoke(eventData);
        }
    }
}