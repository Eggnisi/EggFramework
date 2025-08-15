#region

//文件创建者：Egg
//创建时间：07-19 12:16

#endregion

namespace EggFramework.Procedure
{
    [NodeTitle("开始节点")]
    [NodeTint("#D05F2B")]
    [CreateNodeMenu("基础节点/开始节点")]
    public sealed class StartNode : SingleFlowNode
    {
        public override bool IsFinished { get; protected set; }

        public override void OnExecute()
        {
            Log($"流程开始，流程Id：{_handle.ID}，流程名：{_handle.Name}");
            IsFinished = true;
        }
    }
}