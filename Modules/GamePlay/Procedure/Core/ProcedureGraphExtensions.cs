#region

//文件创建者：Egg
//创建时间：07-19 12:51

#endregion

using System;

namespace EggFramework.Procedure
{
    public static class ProcedureGraphExtensions
    {
        public static TNode GetNode<TNode>(this ProcedureGraph graph) where TNode : BaseNode
        {
            if (!graph)
            {
                throw new ArgumentNullException(nameof(graph), "ProcedureGraph cannot be null");
            }

            foreach (var node in graph.nodes)
            {
                if (node is TNode tNode)
                {
                    return tNode;
                }
            }

            throw new Exception($"No node of type {typeof(TNode).Name} found in the ProcedureGraph");
        }

        public static StartNode GetStartNode(this ProcedureGraph graph) => graph.GetNode<StartNode>();
        public static EndNode GetEndNode(this ProcedureGraph graph) => graph.GetNode<EndNode>();
    }
}