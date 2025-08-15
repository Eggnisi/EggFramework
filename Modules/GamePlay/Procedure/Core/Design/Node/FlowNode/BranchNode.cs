#region

//文件创建者：Egg
//创建时间：07-24 11:45

#endregion

using EggFramework.Executable;
using Sirenix.OdinInspector;

namespace EggFramework.Procedure
{
    [NodeTitle("分支节点")]
    [CreateNodeMenu("控制节点/分支节点")]
    public sealed class BranchNode : FlowNode
    {
        [Input(connectionType = ConnectionType.Multiple), LabelText("上一节点")]
        public ExecutionFlow Prev;

        [Input, LabelText("条件输入")] public bool Predicate;
        
        [Output(connectionType = ConnectionType.Override), LabelText("True分支")]
        public ExecutionFlow TrueBranch;

        [Output(connectionType = ConnectionType.Override), LabelText("False分支")]
        public ExecutionFlow FalseBranch;


        public override                    bool IsFinished { get; protected set; }

        public override FlowNode NextNode =>
            GetInputValue(nameof(Predicate), Predicate)
                ? GetNextNode(nameof(TrueBranch))
                : GetNextNode(nameof(FalseBranch));

        public override void OnExecute() => IsFinished = true;
    }
}