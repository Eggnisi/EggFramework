#region

//文件创建者：Egg
//创建时间：07-20 12:42

#endregion

using EggFramework.Util;

namespace EggFramework.Procedure
{
    public abstract class FlowNode : BaseNode
    {
        public virtual FlowNode NextNode => GetNextNode("Next");

        /// <summary>
        /// 标志节点是否执行完毕，设置为 true 时，表示该节点的逻辑已经完成，外部会执行下一个节点。
        /// </summary>
        public abstract bool IsFinished { get; protected set; }

        protected IProcedureSystem _procedureSystem;
        protected FlowNode GetNextNode(string fieldName) => GetPort(fieldName)?.Connection?.node as FlowNode;
        public abstract void OnExecute();
        protected ProcedureHandle _handle     { get; private set; }
        protected IBlackboard     _blackboard { get; private set; }

        public void Initialize(ProcedureHandle procedureHandle, IBlackboard bb)
        {
            _handle          = procedureHandle;
            _blackboard      = bb;
            _procedureSystem = TypeUtil.GetFirstActiveArchitecture().GetSystem<IProcedureSystem>();
            OnInitialize();
        }

        public void Enter()
        {
            IsFinished = false;
            OnEnter();
        }

        public void Exit() => OnExit();

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnExit()
        {
        }
    }
}