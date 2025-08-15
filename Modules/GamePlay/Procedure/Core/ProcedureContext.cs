#region

//文件创建者：Egg
//创建时间：07-26 05:47

#endregion

using System;
using UnityEngine;

namespace EggFramework.Procedure
{
    internal class ProcedureContext
    {
        public ProcedureExecutor Executor;
        public EProcedureState   State;
        public float             ExecutionTime;

        public Action OnStart;
        public Action OnPause;
        public Action OnResume;
        public Action OnComplete;

        private readonly int             _id;
        private readonly FlowNode        _entryNode;
        private readonly ProcedureSystem _system;
        private          bool            _stopped;
        private          ProcedureHandle _handle;

        public ProcedureContext(int id, FlowNode entryNode, ProcedureSystem system)
        {
            _id           = id;
            _entryNode    = entryNode;
            _system       = system;
            State         = EProcedureState.Running;
            ExecutionTime = 0f;
        }

        internal void SetProcedureHandle(ProcedureHandle handle)
        {
            _handle = handle;
        }

        public void Start()
        {
            Executor = new ProcedureExecutor();
            Executor.Start(_entryNode, _handle);
            OnStart?.Invoke();
        }

        public void Update()
        {
            if (State != EProcedureState.Running) return;

            ExecutionTime += Time.deltaTime;
            Executor.Update();

            if (!Executor.IsCompleted) return;

            State = EProcedureState.Completed;
            OnComplete?.Invoke();
            _system.RemoveContext(_id);
        }

        public void Pause()
        {
            if (State != EProcedureState.Running) return;
            State = EProcedureState.Paused;
            OnPause?.Invoke();
        }

        public void Resume()
        {
            if (State != EProcedureState.Paused) return;
            State = EProcedureState.Running;
            OnResume?.Invoke();
        }

        public void Stop()
        {
            if (_stopped) return;
            _stopped = true;
            _system.RemoveContext(_id);
        }
    }
}