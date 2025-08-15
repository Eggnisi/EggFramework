#region

//文件创建者：Egg
//创建时间：07-24 12:43

#endregion

using Sirenix.OdinInspector;

namespace EggFramework.Procedure
{
    public abstract class SingleFlowNode : FlowNode
    {
        [Input(connectionType = ConnectionType.Multiple), HideIf("@GetType().Name == \"StartNode\""), LabelText("上一节点")]
        public ExecutionFlow Prev;

        [Output(connectionType = ConnectionType.Override), HideIf("@GetType().Name == \"EndNode\""),
         LabelText("下一节点")]
        public ExecutionFlow Next;
    }
}