#region

//文件创建者：Egg
//创建时间：07-19 12:24

#endregion

using Sirenix.OdinInspector;

namespace EggFramework.Procedure
{
    [NodeTitle("打印节点")]
    [NodeTint("#8E44AD")]
    [CreateNodeMenu("调试节点/打印节点")]
    public sealed class PrintNode : SingleFlowNode
    {
        [Input, LabelText("打印内容")] public string Message;
        public override                   bool   IsFinished { get; protected set; }

        public override void OnExecute()
        {
            var port = GetPort(nameof(Message));
            var msg  = port?.GetInputValue()?.ToString();
            Log(string.IsNullOrEmpty(msg) ? Message : msg);
            IsFinished = true;
        }
    }
}