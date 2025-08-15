#region

//文件创建者：Egg
//创建时间：07-19 12:20

#endregion

using Sirenix.OdinInspector;
using XNode;

namespace EggFramework.Procedure
{
    [NodeTitle("字符串常量")]
    [NodeTint("#52BE80")]
    [CreateNodeMenu("常量节点/字符串常量")]
    public sealed class StringConstantNode : DataInputNode
    {
        [Output(ShowBackingValue.Always), LabelText("字符串常量")]
        public string InputValue;

        public override object GetValue(NodePort port) => port.fieldName == nameof(InputValue) ? InputValue : null;
    }
}