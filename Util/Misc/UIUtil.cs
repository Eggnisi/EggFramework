#region

//文件创建者：Egg
//创建时间：09-13 12:55

#endregion

using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EggFramework.Util
{
    public static class UIUtil
    {
        private static readonly Dictionary<GameObject, CustomTween> _tweens = new();

        public static void FadeGameObjectByCanvasGroup(this GameObject self, float alpha, float fadeTime = .5f)
        {
            if (_tweens.TryGetValue(self, out var tween))
            {
                tween.CheckAndGenTween(() => GenFadeTween(self, alpha, fadeTime), CustomTween.ETweenCheckMode.Override);
            }
            else
            {
                _tweens[self] = new CustomTween();
                _tweens[self].CheckAndGenTween(() => GenFadeTween(self, alpha, fadeTime),
                    CustomTween.ETweenCheckMode.Override);
            }
        }

        public static void ResetTransparent(this SpriteRenderer self)
        {
            self.color = self.color.ResetTransparent();
        }

        public static void ResetTransparent(this Image self)
        {
            self.color = self.color.ResetTransparent();
        }

        public static void SetTransparent(this SpriteRenderer self)
        {
            self.color = self.color.SetTransparent();
        }

        public static void ResetTransparent(this TMP_Text self)
        {
            self.color = self.color.ResetTransparent();
        }

        public static void SetTransparent(this TMP_Text self)
        {
            self.color = self.color.SetTransparent();
        }

        public static void SetTransparent(this Text self)
        {
            self.color = self.color.SetTransparent();
        }

        public static void SetTransparent(this Image self)
        {
            self.color = self.color.SetTransparent();
        }

        public static void SetAlpha(this TMP_Text self, float alpha)
        {
            self.color = self.color.SetAlpha(alpha);
        }

        public static void SetAlpha(this SpriteRenderer self, float alpha)
        {
            self.color = self.color.SetAlpha(alpha);
        }

        public static void SetAlpha(this Text self, float alpha)
        {
            self.color = self.color.SetAlpha(alpha);
        }

        public static void SetAlpha(this Image self, float alpha)
        {
            self.color = self.color.SetAlpha(alpha);
        }
        
        public static void SetAlpha(this RawImage self, float alpha)
        {
            self.color = self.color.SetAlpha(alpha);
        }

        private static Tween GenFadeTween(GameObject target, float alpha, float fadeTime)
        {
            var cg = target.GetComponent<CanvasGroup>();
            return DOTween.To(val => target.SetAlphaByCanvasGroup(Mathf.Lerp(cg.alpha, alpha, val)), 0, 1, fadeTime);
        }

        public static void SetAlphaByCanvasGroup(this GameObject self, float alpha)
        {
            var cg = self.GetComponent<CanvasGroup>();
            if (!cg) return;
            cg.alpha = Mathf.Clamp01(alpha);
        }

        public static void SetAlphaByCanvasGroupInChildren(this GameObject self, float alpha)
        {
            var cg = self.GetComponentsInChildren<CanvasGroup>();
            if (cg.Length <= 0) return;
            foreach (var canvasGroup in cg)
            {
                canvasGroup.alpha = Mathf.Clamp01(alpha);
            }
        }
    }
}