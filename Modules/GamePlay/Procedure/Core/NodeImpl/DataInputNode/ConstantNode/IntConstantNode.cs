#region

//文件创建者：Egg
//创建时间：07-19 12:23

#endregion

using Sirenix.OdinInspector;
using XNode;

namespace EggFramework.Procedure
{
    [NodeTitle("整数常量")]
    [NodeTint("#52BE80")]
    [CreateNodeMenu("常量节点/整数常量")]
    public sealed class IntConstantNode : DataInputNode
    {
        [Output(ShowBackingValue.Always), LabelText("整数常量")]
        public int InputValue;

        public override object GetValue(NodePort port) => port.fieldName == nameof(InputValue) ? InputValue : null;
    }
}