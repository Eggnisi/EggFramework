using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework
{
    public class AIBrain : MonoBehaviour
    {
        public                                      List<AIState> States;
        public                                      bool          BrainActive = true;
        public                                      AIState       CurrentState     { get; private set; }
        [ShowInInspector, LabelText("当前状态")] public string        CurrentStateName => CurrentState?.StateName ?? "无状态";

        [ShowInInspector, LabelText("当前状态持续时间")]
        public float TimeInThisState { get; private set; }

        [SerializeField] private bool         _initOnStart = true;
        private                  AIDecision[] _decisions;
        private                  AIAction[]   _actions;

        public Dictionary<string, object> Blackboard { get; } = new();
        
        private void Start()
        {
            if(!_initOnStart) return;
            Init();
        }

        public void Init()
        {
            foreach (AIState state in States)
            {
                state.SetBrain(this);
            }

            _decisions = GetComponents<AIDecision>();
            _actions   = GetComponents<AIAction>();
            
            if (States.Count > 0)
            {
                CurrentState = States[0];
            }

            InitializeDecisions();
            InitializeActions();

            CurrentState.EnterState();
        }

        public void ReInitialize()
        {
            InitializeDecisions();
            InitializeActions();
        }

        private void Update()
        {
            if (!BrainActive || CurrentState == null)
            {
                return;
            }

            CurrentState.PerformActions();
            CurrentState.EvaluateTransitions();

            TimeInThisState += Time.deltaTime;
        }


        public void TransitionToState(string newStateName)
        {
            if (newStateName != CurrentState.StateName)
            {
                CurrentState.ExitState();
                OnExitState();

                CurrentState = FindState(newStateName);
                CurrentState?.EnterState();
            }
        }

        private void OnExitState()
        {
            TimeInThisState = 0f;
        }

        private void InitializeDecisions()
        {
            foreach (var decision in _decisions)
            {
                decision.Initialization();
            }
        }

        private void InitializeActions()
        {
            foreach (var action in _actions)
            {
                action.Initialization();
            }
        }

        private AIState FindState(string stateName)
        {
            foreach (AIState state in States)
            {
                if (state.StateName == stateName)
                {
                    return state;
                }
            }

            return null;
        }
    }
}