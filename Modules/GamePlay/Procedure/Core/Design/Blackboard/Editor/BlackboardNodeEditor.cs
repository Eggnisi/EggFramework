#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.Procedure;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(BlackboardNode))]
public sealed class BlackboardNodeEditor : NodeEditor
{
    // UI状态变量
    private string  newVariableName   = "";
    private int     selectedTypeIndex = 0;
    private Vector2 scrollPosition;

    // 支持的变量类型列表
    private static readonly Type[] supportedTypes = new Type[]
    {
        typeof(int),
        typeof(bool),
        typeof(string),
        typeof(float),
        typeof(Vector2),
        typeof(Vector3)
    };

    public override void OnBodyGUI()
    {
        base.OnBodyGUI(); // 绘制默认字段

        serializedObject.Update();
        var node = target as BlackboardNode;

        if (node == null || node.BlackboardSource == null)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("黑板变量管理", EditorStyles.boldLabel);

        // 1. 添加新变量区域
        DrawAddVariableSection(node);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("现有变量", EditorStyles.boldLabel);

        // 2. 显示现有变量列表
        DrawExistingVariables(node);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawAddVariableSection(BlackboardNode node)
    {
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField("添加新变量");
        EditorGUILayout.BeginHorizontal();

        // 变量名称输入
        newVariableName = EditorGUILayout.TextField(newVariableName);

        // 类型选择下拉菜单
        selectedTypeIndex = EditorGUILayout.Popup(
            selectedTypeIndex,
            supportedTypes.Select(t => t.Name).ToArray()
        );

        EditorGUILayout.EndHorizontal();

        // 添加按钮
        if (GUILayout.Button("添加变量"))
        {
            AddNewVariable(node);
        }

        EditorGUILayout.EndVertical();
    }

    private void AddNewVariable(BlackboardNode node)
    {
        if (string.IsNullOrWhiteSpace(newVariableName))
        {
            Debug.LogWarning("变量名不能为空!");
            return;
        }

        // 检查变量名是否已存在
        if (node.BlackboardSource.Variables.Keys.Any(v => v == newVariableName))
        {
            Debug.LogWarning($"变量 '{newVariableName}' 已存在!");
            return;
        }

        // 获取选择的类型
        var selectedType = supportedTypes[selectedTypeIndex];
        // 添加变量到黑板
        Undo.RecordObject(node, "Add Variable");
        node.BlackboardSource.AddVariable(newVariableName, selectedType);
        newVariableName = ""; // 清空输入框

        // 刷新编辑器
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DrawExistingVariables(BlackboardNode node)
    {
        if (node.BlackboardSource.Variables.Count == 0)
        {
            EditorGUILayout.HelpBox("黑板中没有变量", MessageType.Info);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(GetWidth()));

        foreach (var variable in node.BlackboardSource.Variables.ToList())
        {
            EditorGUILayout.BeginHorizontal(GUI.skin.box);

            // 显示变量信息
            EditorGUILayout.LabelField($"{variable.Key} : {variable.Value.VarType.Name}",
                GUILayout.Width(GetWidth() / 3f));
            variable.Value.Value = DrawPropertyByType(variable.Key, variable.Value.VarType, variable.Value.Value);
            // 删除按钮
            if (GUILayout.Button("删除", GUILayout.Width(60)))
            {
                Undo.RecordObject(node, "Remove Variable");
                node.BlackboardSource.RemoveVariable(variable.Key);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private object DrawPropertyByType(string name, Type valueVarType, object value)
    {
        if (valueVarType == typeof(int))
        {
            return EditorGUILayout.IntField((int)value, GUILayout.Width(GetWidth() / 3f));
        }

        if (valueVarType == typeof(float))
        {
            return EditorGUILayout.FloatField((float)value, GUILayout.Width(GetWidth() / 3f));
        }

        if (valueVarType == typeof(bool))
        {
            return EditorGUILayout.Toggle((bool)value, GUILayout.Width(GetWidth() / 3f));
        }

        if (valueVarType == typeof(string))
        {
            return EditorGUILayout.TextField((string)value, GUILayout.Width(GetWidth() / 3f));
        }

        if (valueVarType == typeof(Vector2))
        {
            return EditorGUILayout.Vector2Field(name, (Vector2)value, GUILayout.Width(GetWidth() / 3f));
        }
        
        if (valueVarType == typeof(Vector3))
        {
            return EditorGUILayout.Vector3Field(name, (Vector3)value, GUILayout.Width(GetWidth() / 3f));
        }


        return null;
    }
}
#endif