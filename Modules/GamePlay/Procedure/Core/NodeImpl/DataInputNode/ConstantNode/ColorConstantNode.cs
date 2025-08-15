#region
// 文件创建者：Egg
// 创建时间：07-19 12:23
#endregion

using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace EggFramework.Procedure
{
    [NodeTitle("颜色常量")]
    [NodeTint("#52BE80")]
    [CreateNodeMenu("常量节点/颜色常量")]
    public sealed class ColorConstantNode : DataInputNode
    {
        [Output(ShowBackingValue.Always), LabelText("颜色值")]
        [ColorPalette] // Odin特性：添加颜色选择器
        public Color InputValue = Color.white;

        public override object GetValue(NodePort port) => 
            port.fieldName == nameof(InputValue) ? InputValue : null;
    }
}