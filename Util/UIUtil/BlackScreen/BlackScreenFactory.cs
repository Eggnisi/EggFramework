#region

//文件创建者：Egg
//创建时间：12-03 08:37

#endregion

using UnityEngine;
using UnityEngine.UI;

namespace EggFramework.UIUtil
{
    internal static class BlackScreenFactory
    {
        public static BlackScreenController Create()
        {
            var target = new GameObject("BlackScreen");
            Object.DontDestroyOnLoad(target);
            var canvasScaler = target.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            var canvas = target.GetOrAddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;
            var ctrl  = target.AddComponent<BlackScreenController>();
            var child = new GameObject("Image");
            child.transform.SetParent(target.transform);
            var image = child.GetOrAddComponent<Image>();
            image.color = Color.clear;
            var rect = image.GetComponent<RectTransform>();
            rect.anchoredPosition = Vector2.zero;
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.currentResolution.width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   Screen.currentResolution.height);
            ctrl.Canvas = image;
            return ctrl;
        }
    }
}