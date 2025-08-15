#region

//文件创建者：Egg
//创建时间：07-27 12:48

#endregion

#if UNITY_EDITOR


using EggFramework.Util;
using UnityEditor;
using UnityEngine;

namespace EggFramework.Procedure
{
    public static class ProcedureMenuItem
    {
        [MenuItem("EggFramework/Procedure/打印活动流程递归树", priority = 100)]
        public static void PrintActiveProcessesTree()
        {
            if (!Application.isPlaying)
            {
                Debug.LogWarning("请在运行时调用此功能！");
                return;
            }

            var procedureSystem = TypeUtil.GetFirstActiveArchitecture().GetSystem<IProcedureSystem>();
            var activeProcesses =  ((ProcedureSystem)procedureSystem).GetActiveProcessesTree();
            Debug.Log(activeProcesses);
        }
        
        [MenuItem("EggFramework/Procedure/设置日志输出级别/None", priority = 100)]
        public static void SetNoneLogLevel()
        {
            DisableAll();
        }

        [MenuItem("EggFramework/Procedure/设置日志输出级别/Node", priority = 90)]
        public static void SetNodeLogLevel()
        {
            DisableAll();
            DefineUtil.SetDefineActiveForCurrentTargetGroup("EGG_PROCEDURE_LOG_LEVEL_NODE", true);
        }

        [MenuItem("EggFramework/Procedure/设置日志输出级别/Procedure", priority = 80)]
        public static void SetProcedureLogLevel()
        {
            DisableAll();
            DefineUtil.SetDefineActiveForCurrentTargetGroup("EGG_PROCEDURE_LOG_LEVEL_PROCEDURE", true);
        }

        [MenuItem("EggFramework/Procedure/设置日志输出级别/System", priority = 70)]
        public static void SetSystemLogLevel()
        {
            DisableAll();
            DefineUtil.SetDefineActiveForCurrentTargetGroup("EGG_PROCEDURE_LOG_LEVEL_SYSTEM", true);
        }

        private static void DisableAll()
        {
            DefineUtil.SetDefineActiveForCurrentTargetGroup("EGG_PROCEDURE_LOG_LEVEL_NODE",      false);
            DefineUtil.SetDefineActiveForCurrentTargetGroup("EGG_PROCEDURE_LOG_LEVEL_PROCEDURE", false);
            DefineUtil.SetDefineActiveForCurrentTargetGroup("EGG_PROCEDURE_LOG_LEVEL_SYSTEM",    false);
        }
    }
}
#endif