#region

//文件创建者：Egg
//创建时间：07-28 05:06

#endregion

using System;
using System.Collections.Generic;
using QFramework;
using Object = UnityEngine.Object;

namespace EggFramework.Procedure
{
    [Serializable]
    public class BlackboardSource : IBlackboard
    {
        private Dictionary<string, Variable> _variables = new();
        public string                 Identifier => "Graph";

        public Dictionary<string, Variable> Variables
        {
            get => _variables;
            set => _variables = value;
        }

        public Object UnityContextObject { get; set; }

        public IBlackboard        Parent                        { get; set; }
        string IBlackboard.       IndependentVariablesFieldName => null;
        
        private Action<Variable> _onVariableAdded;
        private Action<Variable> _onVariableRemoved;
        
        public IUnRegister RegisterOnVariableAdded(Action<Variable> onVariableAdded)
        {
            _onVariableAdded += onVariableAdded;
            return new CustomUnRegister(() => _onVariableAdded -= onVariableAdded);
        }

        public IUnRegister RegisterOnVariableRemoved(Action<Variable> onVariableRemoved)
        {
            _onVariableRemoved += onVariableRemoved;
            return new CustomUnRegister(() => _onVariableRemoved -= onVariableRemoved);
        }

        void IBlackboard.TryInvokeOnVariableAdded(Variable variable) => _onVariableAdded?.Invoke(variable);
        void IBlackboard.TryInvokeOnVariableRemoved(Variable variable) => _onVariableRemoved?.Invoke(variable);

        //required
        public BlackboardSource()
        {
            
        }

        public override string ToString() => Identifier;
    }
}