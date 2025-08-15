#region

//文件创建者：Egg
//创建时间：04-13 02:41

#endregion

using System;
using UnityEngine;

namespace EggFramework.MonoUtil
{
    public sealed class MonoUpdater : MonoBehaviour
    {
        private Action _action;
        private void Update()
        {
            _action?.Invoke();
        }

        public void RegisterOnUpdate(Action action)
        {
            _action += action;
        }
    }
}