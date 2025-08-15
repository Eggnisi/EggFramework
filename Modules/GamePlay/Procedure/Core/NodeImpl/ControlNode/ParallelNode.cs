#region

//文件创建者：Egg
//创建时间：07-26 05:08

#endregion

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace EggFramework.Procedure
{
    public enum EParallelNodeWaitType
    {
        [LabelText("不等待")]    None, // 直接执行不等待
        [LabelText("等待全部完成")] All,  // 等待所有子节点都完成
        [LabelText("等待任意完成")] Any   // 等待任意一个子节点完成
    }

    [NodeTitle("并行节点")]
    [NodeTint("#8E44AD")]
    [CreateNodeMenu("控制节点/并行节点")]
    public sealed class ParallelNode : SingleFlowNode
    {
        [Output(connectionType = ConnectionType.Override, dynamicPortList = true)]
        public List<ExecutionFlow> Branches = new();

        private readonly List<ProcedureHandle> _handles = new();

        [LabelText("等待类型")] public EParallelNodeWaitType WaitType;

        protected override void OnEnter()
        {
            _handles.Clear();
            var index = 0;
            while (HasPort($"{nameof(Branches)} {index}"))
            {
                var startNode = GetNextNode($"{nameof(Branches)} {index}");
                if (startNode)
                {
                    var handle = _procedureSystem.SubmitProcedure($"{_handle.Name}-ParallelNodeChild-Branch {index}",
                        startNode, _blackboard, _handle);
                    _handles.Add(handle);
                }

                index++;
            }

            foreach (var handle in _handles)
            {
                handle.Start();
            }
        }

        public override bool IsFinished { get; protected set; }

        public override void OnExecute()
        {
            IsFinished = WaitType switch
            {
                EParallelNodeWaitType.None => true,
                EParallelNodeWaitType.Any  => _handles.Any(handle => handle.IsCompleted),
                EParallelNodeWaitType.All  => _handles.All(handle => handle.IsCompleted),
                _                          => IsFinished
            };
        }
    }
}