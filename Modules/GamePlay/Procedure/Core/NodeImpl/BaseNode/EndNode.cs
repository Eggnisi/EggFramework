#region

//文件创建者：Egg
//创建时间：07-19 12:18

#endregion

namespace EggFramework.Procedure
{
    [NodeTitle("结束节点")]
    [NodeTint("#F6C342")]
    [CreateNodeMenu("基础节点/结束节点")]
    public sealed class EndNode : SingleFlowNode
    {
        public override bool IsFinished  { get; protected set; }

        public override void OnExecute()
        {
            Log($"流程结束，流程Id：{_handle.ID}，流程名：{_handle.Name}");
            IsFinished = true;
        }
    }
}