#region

//文件创建者：Egg
//创建时间：04-18 04:06

#endregion

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EggFramework.MonoUtil
{
    public sealed class DragHandle : MonoBehaviour, IDragHandler, IBeginDragHandler
    {
        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
        }

        private Vector2       _startPoint;
        private RectTransform _rect;

        public void OnDrag(PointerEventData eventData)
        {
            _startPoint            += eventData.delta;
            _rect.anchoredPosition =  _startPoint;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _startPoint = _rect.anchoredPosition;
        }
    }
}