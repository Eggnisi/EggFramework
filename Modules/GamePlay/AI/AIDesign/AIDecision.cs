using UnityEngine;

namespace EggFramework
{
    public abstract class AIDecision : MonoBehaviour
    {
        public abstract bool Decide();

        public string Label;
        public bool DecisionInProgress { get; set; }
        
        protected AIBrain _brain;
        
        public void Initialization()
        {
            _brain = GetComponent<AIBrain>();
            OnInit();
        }

        public void EnterState()
        {
            DecisionInProgress = true;
            OnEnterState();
        }
        public void ExitState()
        {
            DecisionInProgress = false;
            OnExitState();
        }
        protected virtual void OnInit(){}
        protected virtual void OnEnterState(){}
        protected virtual void OnExitState(){}
    }
}