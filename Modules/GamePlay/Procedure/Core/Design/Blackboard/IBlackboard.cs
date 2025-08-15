#region

//文件创建者：Egg
//创建时间：07-28 04:02

#endregion

using System;
using System.Collections.Generic;
using QFramework;
using Object = UnityEngine.Object;

namespace EggFramework.Procedure
{
    ///<summary> An interface for Blackboards</summary>
    public interface IBlackboard
    {
        string                       Identifier                    { get; }
        IBlackboard                  Parent                        { get; }
        Dictionary<string, Variable> Variables                     { get; set; }
        
        Object UnityContextObject { get; set; }
    
        string                       IndependentVariablesFieldName { get; }

        IUnRegister RegisterOnVariableAdded(Action<Variable> onVariableAdded);
        IUnRegister RegisterOnVariableRemoved(Action<Variable> onVariableAdded);
        
        void TryInvokeOnVariableAdded(Variable variable);
        void TryInvokeOnVariableRemoved(Variable variable);
    }

    ///<summary> An interface for Global Blackboards</summary>
    public interface IGlobalBlackboard : IBlackboard
    {
        string Uid { get; }
    }
}