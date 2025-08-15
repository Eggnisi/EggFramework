#region

// 文件创建者：Egg
// 创建时间：07-26 05:08

#endregion

using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Procedure
{
    public enum ESubgraphWaitType
    {
        [LabelText("等待子图完成")]   WaitForCompletion,
        [LabelText("直接进入下一节点")] ContinueImmediately
    }

    [NodeTitle("子流程图节点")]
    [NodeTint("#8E44AD")]
    [CreateNodeMenu("控制节点/子流程图节点")]
    public sealed class SubgraphNode : SingleFlowNode
    {
        [LabelText("子流程图")] [Required] public ProcedureGraph Subgraph;

        [LabelText("等待策略")] public ESubgraphWaitType WaitType = ESubgraphWaitType.WaitForCompletion;

        private ProcedureHandle _subgraphHandle;
        private bool            _subgraphStarted;

        protected override void OnEnter()
        {
            _subgraphHandle  = null;
            _subgraphStarted = false;

            if (!Subgraph)
            {
                Debug.LogError("SubgraphNode has no subgraph assigned!");
                IsFinished = true;
                return;
            }

            var startNode = Subgraph.GetStartNode();
            if (!startNode)
            {
                Debug.LogError("Subgraph has no start node!");
                IsFinished = true;
                return;
            }

            // 创建子流程句柄，与当前流程建立父子关系
            _subgraphHandle = _procedureSystem.SubmitProcedure(
                $"{_handle.Name}-Subgraph-{Subgraph.name}",
                startNode,
                _blackboard,
                _handle
            );

            // 根据等待策略决定是否立即完成
            if (WaitType == ESubgraphWaitType.ContinueImmediately)
            {
                IsFinished = true;
            }
        }

        public override bool IsFinished { get; protected set; }

        public override void OnExecute()
        {
            // 确保子流程已启动
            if (!_subgraphStarted && _subgraphHandle != null)
            {
                _subgraphHandle.Start();
                _subgraphStarted = true;
            }

            // 如果选择等待子图完成，则检查子图状态
            if (WaitType == ESubgraphWaitType.WaitForCompletion)
            {
                IsFinished = _subgraphHandle is { IsValid: true, State: EProcedureState.Completed };
            }
        }

        #region 运行时调试信息

        [ShowInInspector, DisplayAsString, FoldoutGroup("运行时状态")]
        [Title("子流程状态")]
        private string _subgraphStatus
        {
            get
            {
                if (_subgraphHandle == null) return "未启动";
                if (!_subgraphHandle.IsValid) return "已结束";

                return
                    $"{_subgraphHandle.State} | 总时间: {_subgraphHandle.ExecutionTime:F1}s | 节点内时间: {_subgraphHandle.TimeInNode:F1}s";
            }
        }

        [ShowInInspector, DisplayAsString, FoldoutGroup("运行时状态")]
        [Title("当前节点")]
        private string _currentNodeInfo
        {
            get
            {
                if (_subgraphHandle is not { IsValid: true }) return "无";
                return _subgraphHandle.CurrentNode?.GetType().Name ?? "未运行";
            }
        }

        #endregion
    }
}