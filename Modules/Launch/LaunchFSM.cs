#region

//文件创建者：Egg
//创建时间：10-29 09:02

#endregion

using System;
using Cysharp.Threading.Tasks;
using QFramework;

namespace EggFramework.Modules.Launch
{
    public sealed class LaunchFSM
    {
        private readonly FSM<ELaunchState> _innerFSM = new();

        private readonly CompleteState _completeState;
        private readonly ResLoadState  _resLoadState;
        public static    IArchitecture Architecture { get; private set; }
        private readonly bool          _started;
        
        public static bool Finished { get; private set; }

        public LaunchFSM(IArchitecture architecture)
        {
            if (Architecture != null)
            {
                _started       = true;
                _completeState = new CompleteState(_innerFSM, this);
                _innerFSM.AddState(ELaunchState.CompleteState, _completeState);
                return;
            }

            Architecture = architecture;
            _innerFSM.AddState(ELaunchState.ConfigLoad, new ConfigLoadState(_innerFSM, this));
            _resLoadState = new ResLoadState(_innerFSM, this);
            _innerFSM.AddState(ELaunchState.ResLoad, _resLoadState);
            _completeState = new CompleteState(_innerFSM, this);
            _innerFSM.AddState(ELaunchState.CompleteState, _completeState);
        }

        public void RegisterCustomAsyncTask(UniTask task)
        {
            _resLoadState?.RegisterCustomAsyncTask(task);
        }

        public void OnLaunchComplete(Action action)
        {
            _completeState?.OnComplete(action);
            Finished = true;
        }

        public void Start()
        {
            _innerFSM.StartState(!_started ? ELaunchState.ConfigLoad : ELaunchState.CompleteState);
        }
    }

    public enum ELaunchState
    {
        ConfigLoad,
        ResLoad,
        CompleteState
    }
}