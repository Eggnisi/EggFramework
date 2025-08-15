#region

//文件创建者：Egg
//创建时间：07-26 06:02

#endregion

using QFramework;

namespace EggFramework.Procedure
{
    public interface IProcedureSystem : ISystem
    {
        ProcedureHandle SubmitProcedure(string procedureName, FlowNode entryNode, IBlackboard bb,
            ProcedureHandle parent = null);

        void StopAll();
    }
}