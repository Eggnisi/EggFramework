#region
// 文件创建者：Egg
// 创建时间：07-19 12:23
#endregion

using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace EggFramework.Procedure
{
    [NodeTitle("Vector3常量")]
    [NodeTint("#52BE80")]
    [CreateNodeMenu("常量节点/Vector3常量")]
    public sealed class Vector3ConstantNode : DataInputNode
    {
        [Output(ShowBackingValue.Always), LabelText("Vector3常量")]
        public Vector3 InputValue = Vector3.zero;

        public override object GetValue(NodePort port) => 
            port.fieldName == nameof(InputValue) ? InputValue : null;
    }
}