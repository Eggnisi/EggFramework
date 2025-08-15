using System;
using Sirenix.OdinInspector;

namespace EggFramework
{
    [Serializable]
    public class AITransition 
    {
        [LabelText("判断脚本")]
        public AIDecision Decision;
        [LabelText("True目标状态")]
        public string TrueState;
        [LabelText("False目标状态")]
        public string FalseState;
    }
}