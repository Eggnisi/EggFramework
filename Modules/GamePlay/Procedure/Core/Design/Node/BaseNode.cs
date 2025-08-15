#region

//文件创建者：Egg
//创建时间：07-19 12:08

#endregion

using EggFramework.Util;
using UnityEngine;
using XNode;

namespace EggFramework.Procedure
{
    public abstract class BaseNode : Node
    {
        public bool IsFlowNode      => this is FlowNode;
        public bool IsDataInputNode => this is DataInputNode;

        protected void Log(object message)
        {
#if UNITY_EDITOR && (EGG_PROCEDURE_LOG_LEVEL_NODE || EGG_PROCEDURE_LOG_LEVEL_PROCEDURE || EGG_PROCEDURE_LOG_LEVEL_NODE)
            Debug.Log($"[ProcedureNode-{GetType().Name}]:{message}");
#endif
        }

        protected void LogWaring(object message)
        {
#if UNITY_EDITOR && (EGG_PROCEDURE_LOG_LEVEL_NODE || EGG_PROCEDURE_LOG_LEVEL_PROCEDURE || EGG_PROCEDURE_LOG_LEVEL_NODE)
            Debug.LogWarning($"[ProcedureNode-{GetType().Name}]:{message}");
#endif
        }

        protected void LogError(object message)
        {
#if UNITY_EDITOR && (EGG_PROCEDURE_LOG_LEVEL_NODE || EGG_PROCEDURE_LOG_LEVEL_PROCEDURE || EGG_PROCEDURE_LOG_LEVEL_NODE)
            Debug.LogError($"[ProcedureNode-{GetType().Name}]:{message}");
#endif
        }
    }
}