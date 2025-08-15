#region

//文件创建者：Egg
//创建时间：09-02 03:35

#endregion

using System;
using DG.Tweening;

namespace EggFramework.Util
{
    public sealed class CustomTween
    {
        public enum ETweenCheckMode
        {
            Normal,
            Override,
            Instant
        }

        private readonly Func<Tween> _tweenFactory;
        private Tween _tween;

        public CustomTween(Func<Tween> tweenFactory = null)
        {
            _tweenFactory = tweenFactory;
        }

        public Tween GetTween()
        {
            return _tween;
        }

        public bool CheckTweenIsFinish(ETweenCheckMode mode)
        {
            if (_tween != null && _tween.IsActive())
            {
                switch (mode)
                {
                    case ETweenCheckMode.Normal:
                    {
                        if (!_tween.IsComplete()) return false;
                        _tween.Kill();
                        _tween = null;
                        return true;
                    }
                    case ETweenCheckMode.Override:
                    {
                        _tween.Complete();
                        _tween.Kill();
                        _tween = null;
                    }
                        return true;
                    case ETweenCheckMode.Instant:
                    {
                        _tween.Kill();
                        _tween = null;
                    }
                        return true;
                }
            }

            return true;
        }

        public void GenTween(Func<Tween> tweenFactory)
        {
            _tween = tweenFactory != null ? tweenFactory.Invoke() : _tweenFactory?.Invoke();
        }

        public void CheckAndGenTween(Func<Tween> tweenFactory = null, ETweenCheckMode mode = ETweenCheckMode.Normal)
        {
            if (CheckTweenIsFinish(mode))
            {
                GenTween(tweenFactory);
            }
        }
    }
}