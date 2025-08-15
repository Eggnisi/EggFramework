#region

//文件创建者：Egg
//创建时间：02-17 09:33

#endregion

using TMPro;

namespace EggFramework.UIUtil
{
    #region

//文件创建者：Egg
//创建时间：12-03 08:37

    #endregion

    using UnityEngine;
    using UnityEngine.UI;

    namespace EggFramework.UIUtil
    {
        public static class QuickTipFactory
        {
            public static QuickTipController Create()
            {
                var target = new GameObject("BlackScreen");
                Object.DontDestroyOnLoad(target);
                var canvasScaler = target.AddComponent<CanvasScaler>();
                canvasScaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                canvasScaler.referenceResolution = new Vector2(1920, 1080);
                var canvas = target.GetOrAddComponent<Canvas>();
                canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;
                
                var ctrl  = target.AddComponent<QuickTipController>();
                var child = new GameObject("Tip");
                child.transform.SetParent(target.transform);
                
                var tipText = child.GetOrAddComponent<TextMeshProUGUI>();
                tipText.color    = Color.black;
                tipText.fontSize = 100f;
                var rect = tipText.GetComponent<RectTransform>();
                rect.anchoredPosition = Vector2.zero;
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.currentResolution.width);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   Screen.currentResolution.height);

                ctrl.QuickTip = tipText;
                return ctrl;
            }
        }
    }
}