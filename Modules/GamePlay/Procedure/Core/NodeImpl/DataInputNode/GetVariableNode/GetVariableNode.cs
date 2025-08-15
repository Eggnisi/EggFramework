#region

//文件创建者：Egg
//创建时间：07-29 05:36

#endregion

using Sirenix.OdinInspector;
using XNode;

namespace EggFramework.Procedure
{
    public sealed class GetVariableNode : DataInputNode
    {
        [LabelText("黑板键")] public string VariableName;

        [LabelText("整数输出"),ShowIf(""), Output(connectionType = ConnectionType.Override, backingValue = ShowBackingValue.Never)]
        public int OutputValue;

        public override object GetValue(NodePort port)
        {
            return ((ProcedureGraph)graph).BlackboardNode.BlackboardSource.GetVariable(VariableName);
        }
    }
}