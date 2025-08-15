#region

//文件创建者：Egg
//创建时间：10-29 09:11

#endregion

using System;
using Cysharp.Threading.Tasks;
using QFramework;

namespace EggFramework.Modules.Launch
{
    public sealed class CompleteState : AbstractState<ELaunchState, LaunchFSM>
    {
        private Action _onComplete;
        public CompleteState(FSM<ELaunchState> fsm, LaunchFSM target) : base(fsm, target)
        {
            UniTask.CompletedTask.Forget();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            _onComplete?.Invoke();
        }

        public void OnComplete(Action action)
        {
            _onComplete += action;
        }
    }
}