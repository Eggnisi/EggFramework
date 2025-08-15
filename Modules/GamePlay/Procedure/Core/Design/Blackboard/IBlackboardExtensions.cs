#region

//文件创建者：Egg
//创建时间：07-28 04:09

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EggFramework.Procedure
{
    public static class IBlackboardExtensions
    {
        ///<summary>Returns the root blackboard upwards the hierarchy</summary>
        public static IBlackboard GetRoot(this IBlackboard blackboard)
        {
            while (true)
            {
                if (blackboard.Parent == null) return blackboard;
                blackboard = blackboard.Parent;
            }
        }

        ///<summary>Returns all parent blackboard (optionaly including self) upwards</summary>
        public static IEnumerable<IBlackboard> GetAllParents(this IBlackboard blackboard, bool includeSelf)
        {
            if (blackboard == null) yield break;

            if (includeSelf) yield return blackboard;

            var current = blackboard.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        ///<summary>Is the blackboard parent of the target child blackboard upward hierarchy or the target itself</summary>
        public static bool IsPartOf(this IBlackboard blackboard, IBlackboard child)
        {
            if (blackboard == null || child == null) return false;

            return blackboard == child || blackboard.IsPartOf(child.Parent);
        }

        ///----------------------------------------------------------------------------------------------
        ///<summary>Adds a new Variable<T> with provided value and returns it.</summary>
        public static Variable<T> AddVariable<T>(this IBlackboard blackboard, string varName, T value)
        {
            var variable = blackboard.AddVariable<T>(varName);
            variable.Value = value;
            return variable;
        }

        ///<summary>Adds a new Variable<T> with default T value and returns it</summary>
        public static Variable<T> AddVariable<T>(this IBlackboard blackboard, string varName)
        {
            return (Variable<T>)blackboard.AddVariable(varName, typeof(T));
        }

        ///<summary>Adds a new Variable in the blackboard</summary>
        public static Variable AddVariable(this IBlackboard blackboard, string varName, object value)
        {
            if (value == null)
            {
                Debug.LogError(
                    "You can't use AddVariable with a null value. Use AddVariable(string, Type) to add the new variable first");
                return null;
            }

            var newVariable                            = blackboard.AddVariable(varName, value.GetType());
            if (newVariable != null) newVariable.Value = value;

            return newVariable;
        }

        ///<summary>Adds a new Variable in the blackboard defining name and type instead of value</summary>
        public static Variable AddVariable(this IBlackboard blackboard, string varName, Type type)
        {
            if (blackboard.Variables.TryGetValue(varName, out var result))
            {
                if (result.CanConvertTo(type))
                {
                    Debug.Log(
                        $"Variable with name '{varName}' already exists in blackboard '{blackboard}'. Returning existing instead of new.");
                    return result;
                }

                Debug.LogError(
                    $"Variable with name '{varName}' already exists in blackboard '{blackboard}', but is of a different type! Returning null instead of new.");
                return null;
            }

            var variableType = typeof(Variable<>).MakeGenericType(type);
            var newVariable  = (Variable)Activator.CreateInstance(variableType);
            newVariable.Name              = varName;
            blackboard.Variables[varName] = newVariable;
            blackboard.TryInvokeOnVariableAdded(newVariable);
            return newVariable;
        }

        ///<summary>Deletes the Variable of name provided regardless of type and returns the deleted Variable object.</summary>
        public static Variable RemoveVariable(this IBlackboard blackboard, string varName)
        {
            if (!blackboard.Variables.Remove(varName, out var variable)) return variable;
            blackboard.TryInvokeOnVariableRemoved(variable);
            variable.OnDestroy();

            return variable;
        }

        ///----------------------------------------------------------------------------------------------
        ///<summary>Gets the Variable value from the blackboard with provided name and type T.</summary>
        public static T GetVariableValue<T>(this IBlackboard blackboard, string varName)
        {
            var variable = GetVariable<T>(blackboard, varName);
            if (variable == null)
            {
                Debug.LogError(
                    $"No Variable of name '{varName}' and type '{typeof(T).Name}' exists on Blackboard '{blackboard}'. Returning default T...");
                return default;
            }

            return variable.Value;
        }

        ///<summary>Set the value of the Variable value defined by its name. If a Variable by that name and type doesnt exist, a new variable is added by that name</summary>
        public static Variable SetVariableValue(this IBlackboard blackboard, string varName, object value)
        {
            if (!blackboard.Variables.TryGetValue(varName, out var variable))
            {
                Debug.Log(
                    $"No Variable of name '{varName}' and type '{(value != null ? value.GetType().Name : "null")}' exists on Blackboard '{blackboard}'. Adding new instead...");
                variable = blackboard.AddVariable(varName, value);
                return variable;
            }

            try
            {
                variable.Value = value;
            }
            catch
            {
                Debug.LogError(
                    $"Can't cast value '{(value != null ? value.ToString() : "null")}' to blackboard variable of name '{varName}' and type '{variable.VarType.Name}'");
                return null;
            }

            return variable;
        }

        ///----------------------------------------------------------------------------------------------
        ///<summary>Initialize Variables data binding for the target (gameobject is used). target ignored for static bindings</summary>
        public static void InitializePropertiesBinding(this IBlackboard blackboard, Component target, bool callSetter)
        {
            if (blackboard.Variables.Count == 0)
            {
                return;
            }

            foreach (var variable in blackboard.Variables.Values)
            {
                variable.InitializePropertyBinding(target?.gameObject, callSetter);
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///<summary>Generic version of GetVariable. Includes parent blackboards upwards the hierarchy.</summary>
        public static Variable<T> GetVariable<T>(this IBlackboard blackboard, string varName)
        {
            return (Variable<T>)blackboard.GetVariable(varName, typeof(T));
        }

        ///<summary>Gets the Variable object of a certain name and optional specified assignable type. Includes parent blackboards upwards the hierarchy.</summary>
        public static Variable GetVariable(this IBlackboard blackboard, string varName, Type ofType = null)
        {
            if (blackboard.Variables != null && varName != null)
            {
                if (blackboard.Variables.TryGetValue(varName, out var variable))
                {
                    if (ofType == null || ofType == typeof(object) || variable.CanConvertTo(ofType))
                    {
                        return variable;
                    }
                }
            }

            if (blackboard.Parent != null)
            {
                var result = blackboard.Parent.GetVariable(varName, ofType);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        ///<summary>Gets the Variable object of a certain ID. Includes parent blackboards upwards the hierarchy.</summary>
        public static Variable GetVariableByID(this IBlackboard blackboard, string ID)
        {
            if (blackboard.Variables != null && ID != null)
            {
                foreach (var pair in blackboard.Variables)
                {
                    if (pair.Value.ID == ID)
                    {
                        return pair.Value;
                    }
                }
            }

            var result = blackboard.Parent?.GetVariableByID(ID);
            return result;
        }

        ///<summary>Get all variables. Includes parent blackboards upwards the hierarchy.</summary>
        public static IEnumerable<Variable> GetVariables(this IBlackboard blackboard, Type ofType = null)
        {
            if (blackboard.Parent != null)
            {
                foreach (var subVariable in blackboard.Parent.GetVariables(ofType))
                {
                    yield return subVariable;
                }
            }

            foreach (var pair in blackboard.Variables)
            {
                if (ofType == null || ofType == typeof(object) || pair.Value.CanConvertTo(ofType))
                {
                    yield return pair.Value;
                }
            }
        }

        ///----------------------------------------------------------------------------------------------
        ///<summary>Change variable type (creates new instance but keeps name and ID)</summary>
        public static Variable ChangeVariableType(this IBlackboard blackboard, Variable target, Type newType)
        {
            var name = target.Name;
            var id   = target.ID;
            blackboard.RemoveVariable(target.Name);
            //do this way instead of AddVariable to keep the same name and id
            var variableType = typeof(Variable<>).MakeGenericType(newType);
            var newVariable  = (Variable)Activator.CreateInstance(variableType, new object[] { name, id });
            blackboard.Variables[target.Name] = newVariable;
            blackboard.TryInvokeOnVariableAdded(newVariable);
            return newVariable;
        }

        ///----------------------------------------------------------------------------------------------
        ///<summary>Overwrite variables from source blackboard into this blackboard</summary>
        public static void OverwriteFrom(this IBlackboard blackboard, IBlackboard sourceBlackboard,
            bool removeMissingVariables = true)
        {
            foreach (var pair in sourceBlackboard.Variables)
            {
                if (blackboard.Variables.ContainsKey(pair.Key))
                {
                    blackboard.SetVariableValue(pair.Key, pair.Value.Value);
                }
                else
                {
                    blackboard.Variables[pair.Key] = pair.Value;
                }
            }

            if (!removeMissingVariables) return;
            var keys = new List<string>(blackboard.Variables.Keys);
            foreach (var key in keys.Where(key => !sourceBlackboard.Variables.ContainsKey(key)))
            {
                blackboard.Variables.Remove(key);
            }
        }
    }
}