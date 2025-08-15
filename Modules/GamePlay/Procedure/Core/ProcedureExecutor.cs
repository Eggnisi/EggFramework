#region

//文件创建者：Egg
//创建时间：07-26 05:21

#endregion

using UnityEngine;

namespace EggFramework.Procedure
{
    internal class ProcedureExecutor
    {
        public FlowNode CurrentNode { get; private set; }
        public float    TimeInNode  { get; private set; }
        public bool     IsCompleted { get; private set; }
        /// <summary>
        /// 操作句柄
        /// </summary>
        private ProcedureHandle _handle;

        public void Start(FlowNode startNode, ProcedureHandle handle)
        {
            if (!startNode)
                throw new System.ArgumentNullException(nameof(startNode), "Start node cannot be null.");
            _handle     = handle;
            CurrentNode = startNode;
            TimeInNode  = 0f;
            IsCompleted = false;
            CurrentNode.Initialize(handle, _handle.Blackboard);
            CurrentNode.Enter();
        }

        public void Update()
        {
            if (IsCompleted || CurrentNode == null) return;

            TimeInNode += Time.deltaTime;
            CurrentNode.OnExecute();

            if (!CurrentNode.IsFinished) return;

            CurrentNode.Exit();
            var nextNode = CurrentNode.NextNode;

            if (!nextNode)
            {
                IsCompleted = true;
                CurrentNode = null;
                return;
            }

            Start(nextNode, _handle);
        }
    }
}