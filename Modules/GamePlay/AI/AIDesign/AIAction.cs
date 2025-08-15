using UnityEngine;


namespace EggFramework
{
    public abstract class AIAction : MonoBehaviour
    {
        public string Label;
        public abstract void PerformAction();
        public bool ActionInProgress { get; set; }
        
        protected AIBrain _brain;
        
        public void Initialization()
        {
            _brain = GetComponent<AIBrain>();
            OnInit();
        }

        public void EnterState()
        {
            ActionInProgress = true;
            OnEnterState();
        }
        public void ExitState()
        {
            ActionInProgress = false;
            OnExitState();
        }
        protected virtual void OnInit(){}
        protected virtual void OnEnterState(){}
        protected virtual void OnExitState(){}
    }
}