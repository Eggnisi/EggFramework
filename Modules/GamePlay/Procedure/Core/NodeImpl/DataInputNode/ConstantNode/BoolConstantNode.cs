#region
// 文件创建者：Egg
// 创建时间：07-19 12:23
#endregion

using Sirenix.OdinInspector;
using XNode;

namespace EggFramework.Procedure
{
    [NodeTitle("布尔常量")]
    [NodeTint("#52BE80")]
    [CreateNodeMenu("常量节点/布尔常量")]
    public sealed class BoolConstantNode : DataInputNode
    {
        [Output(ShowBackingValue.Always), LabelText("布尔值")]
        [ToggleLeft] // Odin特性：左侧切换按钮
        public bool InputValue = false;

        public override object GetValue(NodePort port) => 
            port.fieldName == nameof(InputValue) ? InputValue : null;
    }
}