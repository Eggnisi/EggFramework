using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

namespace EggFramework
{
    [Serializable]
    public class AIState
    {
        [LabelText("状态名")]
        public string StateName;
        [LabelText("行为脚本")]
        public List<AIAction>     Actions;
        [LabelText("状态过渡")]
        public List<AITransition> Transitions;

        private AIBrain _brain;

        public void SetBrain(AIBrain brain)
        {
            _brain = brain;
        }

        public void EnterState()
        {
            foreach (var action in Actions)
            {
                action.EnterState();
            }

            foreach (var transition in Transitions.Where(transition => transition.Decision))
            {
                transition.Decision.EnterState();
            }
        }

        public void ExitState()
        {
            foreach (var action in Actions)
            {
                action.ExitState();
            }

            foreach (var transition in Transitions.Where(transition => transition.Decision))
            {
                transition.Decision.ExitState();
            }
        }

        public void PerformActions()
        {
            if (Actions.Count == 0)
            {
                return;
            }

            foreach (var action in Actions.Where(t => t))
            {
                action.PerformAction();
            }
        }

        public void EvaluateTransitions()
        {
            if (Transitions.Count == 0)
            {
                return;
            }

            foreach (var t in Transitions.Where(t => t.Decision))
            {
                if (t.Decision.Decide())
                {
                    if (string.IsNullOrWhiteSpace(t.TrueState)) continue;
                    _brain.TransitionToState(t.TrueState);
                    break;
                }

                if (string.IsNullOrWhiteSpace(t.FalseState)) continue;
                _brain.TransitionToState(t.FalseState);
                break;
            }
        }
    }
}