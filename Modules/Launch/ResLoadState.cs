#region

//文件创建者：Egg
//创建时间：10-29 09:07

#endregion

using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using QFramework;

namespace EggFramework.Modules.Launch
{
    public sealed class ResLoadState : AbstractState<ELaunchState, LaunchFSM>
    {
        public ResLoadState(FSM<ELaunchState> fsm, LaunchFSM target) : base(fsm, target)
        {
        }

        private readonly List<UniTask> _customTasks = new();

        public void RegisterCustomAsyncTask(UniTask uniTask)
        {
            _customTasks.Add(uniTask);
        }

        protected override async void OnEnter()
        {
            base.OnEnter();
            await UniTask.WhenAll(LaunchFSM.Architecture.HandleAsyncTask(), UniTask.WhenAll(_customTasks));
            mFSM.ChangeState(ELaunchState.CompleteState);
        }
    }
}