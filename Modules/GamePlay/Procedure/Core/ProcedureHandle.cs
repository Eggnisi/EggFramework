#region

//文件创建者：Egg
//创建时间：07-26 05:51

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEngine;

namespace EggFramework.Procedure
{
    public class ProcedureHandle
    {
        public readonly int             ID;
        public readonly string          Name;
        public          EProcedureState State         => _context?.State ?? EProcedureState.None;
        public          FlowNode        CurrentNode   => _context?.Executor?.CurrentNode;
        public          float           ExecutionTime => _context?.ExecutionTime ?? 0f;
        public          float           TimeInNode    => _context?.Executor?.TimeInNode ?? 0f;

        public ProcedureHandle Parent { get; private set; }

        private readonly List<ProcedureHandle> _children = new();
        
        public IReadOnlyList<ProcedureHandle> Children => _children;

        private ProcedureContext _context;

        internal ProcedureHandle(int id, string name, ProcedureContext context,IBlackboard blackboard ,ProcedureHandle parent = null)
        {
            ID                 = id;
            Name               = name;
            _context           = context;
            Parent             = parent;
            Blackboard         = blackboard;
            ((BlackboardSource)Blackboard).Parent = parent?.Blackboard;
            parent?.AddChild(this);
        }

        public void Start() => _context?.Start();

        private void AddChild(ProcedureHandle child)
        {
            if (child != null && !_children.Contains(child))
            {
                _children.Add(child);
            }
        }

        // 移除子流程
        private void RemoveChild(ProcedureHandle child)
        {
            _children.Remove(child);
        }

        public void Pause()
        {
            if (_context == null) return;
            _context.Pause();
            PropagateStateToChildren(EProcedureState.Paused);
#if EGG_PROCEDURE_LOG_LEVEL_PROCEDURE || EGG_PROCEDURE_LOG_LEVEL_SYSTEM
            Debug.Log("[ProcedureHandle] 流程暂停: " + Name);
#endif
        }

        public void Resume()
        {
            if (_context == null) return;
            _context.Resume();
            PropagateStateToChildren(EProcedureState.Running);
#if EGG_PROCEDURE_LOG_LEVEL_PROCEDURE || EGG_PROCEDURE_LOG_LEVEL_SYSTEM
            Debug.Log("[ProcedureHandle] 流程恢复: " + Name);
#endif
        }

        public void Stop()
        {
            if (_context == null) return;

            // 先停止所有子流程
            PropagateStateToChildren(EProcedureState.Completed, true);

            _context.Stop();
            _context = null; // 标记为无效

            // 从父流程移除
            Parent?.RemoveChild(this);
            Debug.Log("[ProcedureHandle] 流程终止: " + Name);
        }

        // 将状态传播给所有子流程
        private void PropagateStateToChildren(EProcedureState state, bool stop = false)
        {
            foreach (var child in _children.ToList().Where(child => child.IsValid))
            {
                if (stop)
                {
                    child.Stop();
                }
                else
                {
                    switch (state)
                    {
                        case EProcedureState.Paused:
                            child.Pause();
                            break;
                        case EProcedureState.Running:
                            child.Resume();
                            break;
                    }
                }
            }
        }


        public bool        IsValid     => _context != null;
        public bool        IsCompleted => _context?.State == EProcedureState.Completed;
        public IBlackboard Blackboard  { get; set; }

        public IUnRegister OnStart(Action action)
        {
            if (_context == null) return null;
            _context.OnStart += action;
            return new CustomUnRegister(() => _context.OnStart -= action);
        }

        public IUnRegister OnPause(Action action)
        {
            if (_context == null) return null;
            _context.OnPause += action;
            return new CustomUnRegister(() => _context.OnPause -= action);
        }

        public IUnRegister OnResume(Action action)
        {
            if (_context == null) return null;
            _context.OnResume += action;
            return new CustomUnRegister(() => _context.OnResume -= action);
        }

        public IUnRegister OnComplete(Action action)
        {
            if (_context == null) return null;
            _context.OnComplete += action;
            return new CustomUnRegister(() => _context.OnComplete -= action);
        }
    }
}