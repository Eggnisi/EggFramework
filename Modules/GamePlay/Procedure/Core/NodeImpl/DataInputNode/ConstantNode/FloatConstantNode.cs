#region

//文件创建者：Egg
//创建时间：07-19 12:23

#endregion

using Sirenix.OdinInspector;
using XNode;

namespace EggFramework.Procedure
{
    [NodeTitle("浮点常量")]
    [NodeTint("#52BE80")]
    [CreateNodeMenu("常量节点/浮点常量")]
    public sealed class FloatConstantNode : DataInputNode
    {
        [Output(ShowBackingValue.Always), LabelText("浮点常量")]
        public float InputValue;

        public override object GetValue(NodePort port) => port.fieldName == nameof(InputValue) ? InputValue : null;
    }
}