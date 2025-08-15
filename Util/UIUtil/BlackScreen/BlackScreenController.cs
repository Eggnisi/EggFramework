#region

//文件创建者：Egg
//创建时间：12-03 08:21

#endregion

using DG.Tweening;
using EggFramework.Util;
using UnityEngine;
using UnityEngine.UI;

namespace EggFramework.UIUtil
{
    internal sealed class BlackScreenController : MonoBehaviour
    {
        public Image Canvas;

        private readonly CustomTween _blackTween = new();

        public void Reset()
        {
            Canvas.SetTransparent();
        }

        public void OnBlackScreen(BlackScreenEvent e)
        {
            var black = Color.black;
            var clear = Color.clear;
            _blackTween.CheckAndGenTween(() =>
            {
                Tween tween =
                    DOTween.To(
                        val =>
                        {
                            Canvas.color = e.EnterBlack ? Color.Lerp(clear, black, val) : Color.Lerp(black, clear, val);
                        }, 0, 1, e.Duration);
                tween.SetAutoKill(true);
                return tween;
            }, CustomTween.ETweenCheckMode.Override);
        }
    }
}