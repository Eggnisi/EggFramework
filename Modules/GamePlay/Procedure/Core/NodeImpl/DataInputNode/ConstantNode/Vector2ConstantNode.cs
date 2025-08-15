#region
// 文件创建者：Egg
// 创建时间：07-19 12:23
#endregion

using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace EggFramework.Procedure
{
    [NodeTitle("Vector2常量")]
    [NodeTint("#52BE80")]
    [CreateNodeMenu("常量节点/Vector2常量")]
    public sealed class Vector2ConstantNode : DataInputNode
    {
        [Output(ShowBackingValue.Always), LabelText("Vector2常量")]
        public Vector2 InputValue = Vector2.zero;

        public override object GetValue(NodePort port) => 
            port.fieldName == nameof(InputValue) ? InputValue : null;
    }
}