#region

// 文件创建者：Egg
// 创建时间：07-26 05:08

#endregion

using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Procedure
{
    public enum ELoopType
    {
        [LabelText("固定次数")] FixedCount,
        [LabelText("无限循环")] Infinite,
        [LabelText("条件循环")] Conditional
    }

    [NodeTitle("循环节点")]
    [NodeTint("#8E44AD")]
    [CreateNodeMenu("控制节点/循环节点")]
    public sealed class LoopNode : SingleFlowNode
    {
        [Output(connectionType = ConnectionType.Override), LabelText("循环体")]
        public ExecutionFlow LoopBody;

        [LabelText("循环类型")] public ELoopType LoopType = ELoopType.FixedCount;

        [LabelText("循环次数")] [ShowIf("LoopType", ELoopType.FixedCount)] [MinValue(1)]
        public int LoopCount = 1;

        [LabelText("循环条件")]
        [ShowIf("LoopType", ELoopType.Conditional)]
        [Input(connectionType = ConnectionType.Override)]
        public bool Condition;

        private FlowNode        _bodyNode;
        private ProcedureHandle _bodyHandle;
        private int             _currentLoopCount;
        private bool            _isBodyCompleted;
        private bool            _shouldContinue = true;

        protected override void OnEnter()
        {
            _currentLoopCount = 0;
            _isBodyCompleted  = false;
            _shouldContinue   = true;

            _bodyNode = GetNextNode(nameof(LoopBody));

            if (!_bodyNode)
            {
                Debug.LogWarning("LoopNode has no body node connected!");
                IsFinished = true;
                return;
            }

            StartNextIteration();
        }

        private void StartNextIteration()
        {
            _currentLoopCount++;
            _isBodyCompleted = false;

            // 提交循环体执行
            _bodyHandle = _procedureSystem.SubmitProcedure($"{_handle.Name}-LoopNode-LoopCount {_currentLoopCount}",
                _bodyNode, _blackboard, _handle);
            _bodyHandle.OnComplete(() => _isBodyCompleted = true);
            _bodyHandle.Start();
        }

        public override bool IsFinished { get; protected set; }

        public override void OnExecute()
        {
            if (IsFinished) return;

            // 等待循环体完成
            if (!_isBodyCompleted) return;

            // 检查是否应该继续循环
            CheckContinueCondition();

            if (_shouldContinue)
            {
                StartNextIteration();
            }
            else
            {
                IsFinished = true;
            }
        }

        private void CheckContinueCondition()
        {
            switch (LoopType)
            {
                case ELoopType.FixedCount:
                    _shouldContinue = _currentLoopCount < LoopCount;
                    break;

                case ELoopType.Infinite:
                    _shouldContinue = true;
                    break;

                case ELoopType.Conditional:
                    _shouldContinue = GetInputValue(nameof(Condition), Condition);
                    break;
            }
        }

        protected override void OnExit()
        {
            // 确保停止循环体执行
            if (_bodyHandle is { IsValid: true })
            {
                _bodyHandle.Stop();
            }
        }
    }
}