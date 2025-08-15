#region

//文件创建者：Egg
//创建时间：10-29 09:06

#endregion

using Cysharp.Threading.Tasks;
using EggFramework.Util;
using QFramework;

namespace EggFramework.Modules.Launch
{
    public sealed class ConfigLoadState : AbstractState<ELaunchState, LaunchFSM>
    {
        public ConfigLoadState(FSM<ELaunchState> fsm, LaunchFSM target) : base(fsm, target)
        {
        }

        protected override async void OnEnter()
        {
            base.OnEnter();
            await ResUtil.LoadSetting();
            mFSM.ChangeState(ELaunchState.ResLoad);
        }
    }
}