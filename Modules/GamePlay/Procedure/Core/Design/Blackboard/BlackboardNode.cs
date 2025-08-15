#region

//文件创建者：Egg
//创建时间：07-28 05:21

#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace EggFramework.Procedure
{
    [NodeTitle("黑板节点")]
    [NodeWidth(500)]
    [NodeTint("#4A90E2")]
    [CreateNodeMenu("数据/黑板节点")]
    public sealed class BlackboardNode : BaseNode, ISerializationCallbackReceiver
    {
        public BlackboardSource BlackboardSource = new();
        void ISerializationCallbackReceiver.OnBeforeSerialize() => SelfSerialize();
        void ISerializationCallbackReceiver.OnAfterDeserialize() => SelfDeserialize();

        [SerializeField, HideInInspector] private string _serializedBlackboardSource;

        private void SelfSerialize()
        {
            if (BlackboardSource == null) return;

            var serializedData = new SerializedBlackboardData
            {
                Variables = new List<SerializedVariable>()
            };

            foreach (var kv in BlackboardSource.Variables)
            {
                var variable = kv.Value;
                var serializedVar = new SerializedVariable
                {
                    Name            = variable.Name,
                    ID              = variable.ID,
                    IsPublic        = variable.IsExposedPublic,
                    DebugBoundValue = variable.DebugBoundValue,
                    PropertyPath    = variable.PropertyPath,
                    TypeName        = variable.VarType?.AssemblyQualifiedName
                };

                // 序列化值
                Type value = variable.VarType;
                if (value == typeof(int) || value == typeof(float) || value == typeof(bool) || value == typeof(string))
                {
                    // 对于基本类型，直接序列化为字符串
                    serializedVar.ValueJson = variable.Value.ToString();
                }
                else if (value == typeof(Vector2) || value == typeof(Vector3) || value == typeof(GameObject))
                {
                    // 对于Unity类型，使用JsonUtility序列化
                    serializedVar.ValueJson = JsonUtility.ToJson(variable.Value);
                }
                
                serializedData.Variables.Add(serializedVar);
            }

            _serializedBlackboardSource = JsonUtility.ToJson(serializedData);
        }

        private void SelfDeserialize()
        {
            if (string.IsNullOrEmpty(_serializedBlackboardSource)) return;
            if (BlackboardSource == null) BlackboardSource = new BlackboardSource();

            try
            {
                var serializedData = JsonUtility.FromJson<SerializedBlackboardData>(_serializedBlackboardSource);
                if (serializedData?.Variables == null) return;

                BlackboardSource.Variables.Clear();

                foreach (var sv in serializedData.Variables)
                {
                    if (string.IsNullOrEmpty(sv.TypeName)) continue;

                    var varType = Type.GetType(sv.TypeName);
                    if (varType == null) continue;

                    // 创建Variable实例
                    var variableType = typeof(Variable<>).MakeGenericType(varType);
                    var variable     = (Variable)Activator.CreateInstance(variableType);

                    // 设置属性
                    variable.Name            = sv.Name;
                    variable.IsExposedPublic = sv.IsPublic;
                    variable.DebugBoundValue = sv.DebugBoundValue;
                    variable.PropertyPath    = sv.PropertyPath;

                    // 设置ID (通过反射)
                    var idField = typeof(Variable).GetField("_id",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    idField?.SetValue(variable, sv.ID);

                    // 反序列化值
                    if (!string.IsNullOrEmpty(sv.ValueJson))
                    {
                        if(varType == typeof(int) || varType == typeof(float) || varType == typeof(bool) || varType == typeof(string))
                        {
                            // 对于基本类型，直接解析字符串
                            variable.Value = Convert.ChangeType(sv.ValueJson, varType);
                        }
                        else if (varType == typeof(Vector2))
                        {
                            // 对于Unity类型，使用JsonUtility反序列化
                            variable.Value  = JsonUtility.FromJson<Vector2>(sv.ValueJson);
                        }else if (varType == typeof(Vector3))
                        {
                            // 对于Unity类型，使用JsonUtility反序列化
                            variable.Value  = JsonUtility.FromJson<Vector3>(sv.ValueJson);
                        }
                    }

                    BlackboardSource.Variables.Add(variable.Name, variable);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Blackboard deserialization failed: {e.Message}");
            }
        }

        // 序列化数据结构
        [Serializable]
        private class SerializedBlackboardData
        {
            public List<SerializedVariable> Variables;
        }

        [Serializable]
        private class SerializedVariable
        {
            public string Name;
            public string ID;
            public bool   IsPublic;
            public bool   DebugBoundValue;
            public string PropertyPath;
            public string TypeName;
            public string ValueJson;
        }
    }
}