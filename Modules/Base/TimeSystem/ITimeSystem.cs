#region

//文件创建者：Egg
//创建时间：10-30 10:10

#endregion

using System;
using QFramework;

namespace EggFramework.TimeSystem
{
    public interface ITimeSystem : ISystem
    {
        int RegisterDelayTask(Action action, float delay, bool ignoreTimeScale = false);
        int RegisterDelayTask(Action action, string taskName, float delay, bool ignoreTimeScale = false);
        int RegisterLoopTask(Action action, float loopTime, int loops = -1, bool ignoreTimeScale = false);
        int RegisterLoopTask(Action action, string taskName, float loopTime, int loops = -1,
            bool ignoreTimeScale = false);

        DelayTaskState GetDelayTaskState(int taskId);
        LoopTaskState GetLoopTaskState(int taskId);
        DelayTaskState GetDelayTaskState(string taskName);
        LoopTaskState GetLoopTaskState(string taskName);

        void CancelTask(int taskId);
        void CancelTask(string taskName);
        void PauseTask(int taskId);
        void PauseTask(string taskName);
        void RecoverTask(int taskId);
        void RecoverTask(string taskName);
        
        void ModifyDelayTaskState(int taskId, ModifyDelayTaskParam delayTaskParam);
        void ModifyLoopTaskState(int taskId, ModifyLoopTaskParam loopTaskParam);
        void ChangeTimeScale(float targetScale, float duration);
        void ResetTimeScale();
        void Pause();
        void Recover();
        
        bool IsPaused { get; }

        IUnRegister RegisterOnPause(Action action);
        IUnRegister RegisterOnRecover(Action action);
    }

    public class DelayTaskState : TimeSystemTaskState
    {
        public float RawDelay;
    }

    public class LoopTaskState : TimeSystemTaskState
    {
        public float RawLoopTime;
        public int   RawLoops;
        public int   PastLoops;
    }

    public abstract class TimeSystemTaskState
    {
        public int        TaskId;
        public string     TaskName;
        public bool       IgnoreTimeScale;
        public float      TimeRemainUtilNextTick;
        public ETaskState State;
    }

    public class ModifyDelayTaskParam : ModifyTimeSystemTaskParam
    {
        public float RawDelay;
    }

    public abstract class ModifyTimeSystemTaskParam
    {
        public bool  IgnoreTimeScale = false;
        public float TimeRemainUtilNextTick;
    }

    public class ModifyLoopTaskParam : ModifyTimeSystemTaskParam
    {
        public float RawLoopTime;
        public int   RawLoops;
    }

    public enum ETaskState
    {
        BeforeStart,
        OnGoing,
        Finish,
        Cancelled
    }
}