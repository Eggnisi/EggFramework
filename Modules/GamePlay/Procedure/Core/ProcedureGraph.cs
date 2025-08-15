#region

//文件创建者：Egg
//创建时间：07-19 12:10

#endregion

using System.Linq;
using EggFramework.Util;
using UnityEngine;
using UnityEngine.Serialization;
using XNode;

namespace EggFramework.Procedure
{
    [CreateAssetMenu(menuName = "Data/ProcedureGraph", fileName = "ProcedureGraph")]
    [RequireNode(typeof(StartNode), typeof(EndNode), typeof(BlackboardNode))]
    public sealed class ProcedureGraph : NodeGraph
    { 
        [SerializeField] public BlackboardNode BlackboardNode;
        private void OnEnable()
        {
            foreach (var node in nodes.ToList())
            {
                if (!node)
                {
                    nodes.Remove(node);
                    continue;
                }

                var type = node.GetType();
                node.name = type.GetAttribute<NodeTitleAttribute>()?.Title ?? type.Name;
            }
            BlackboardNode = this.GetNode<BlackboardNode>();
        }
    }
}