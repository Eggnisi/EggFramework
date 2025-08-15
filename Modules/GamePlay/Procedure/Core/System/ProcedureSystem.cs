#region

//文件创建者：Egg
//创建时间：07-26 05:52

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.MonoUtil;
using QFramework;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EggFramework.Procedure
{
    public sealed class ProcedureSystem : AbstractSystem, IProcedureSystem
    {
        private const    int                               MAX_RECURSION_DEPTH = 7; // 最大递归深度限制
        private readonly Dictionary<int, ProcedureContext> _contexts           = new();
        private readonly List<ProcedureHandle>             _rootHandles        = new();
        private          MonoUpdater                       _updater;
        private          int                               _nextId = 1;

        private void SetupUpdater()
        {
            _updater = Object.FindObjectOfType<MonoUpdater>();
            if (!_updater)
            {
                var obj = new GameObject("ProcedureManagerUpdater");
                _updater = obj.AddComponent<MonoUpdater>();
                Object.DontDestroyOnLoad(obj);
            }

            _updater.RegisterOnUpdate(UpdateAll);
        }

        // 启动新流程并返回句柄
        public ProcedureHandle SubmitProcedure(string procedureName, FlowNode entryNode, IBlackboard bb,
            ProcedureHandle parent = null)
        {
            if (!entryNode)
                throw new ArgumentNullException(nameof(entryNode));

            // 检查递归深度，防止无限递归
            int depth = CalculateRecursionDepth(parent);
            if (depth > MAX_RECURSION_DEPTH)
            {
                throw new InvalidOperationException(
                    $"流程递归深度超过最大限制({MAX_RECURSION_DEPTH})！" +
                    $"当前深度: {depth}, 流程名称: {procedureName}");
            }

            var id      = _nextId++;
            var context = new ProcedureContext(id, entryNode, this);
            _contexts[id] = context;

            var handle = new ProcedureHandle(id, procedureName, context, bb, parent);
            context.SetProcedureHandle(handle);

            if (parent == null)
            {
                _rootHandles.Add(handle);
            }
#if EGG_PROCEDURE_LOG_LEVEL_SYSTEM
            Log($"流程创建成功，流程id：{handle.ID}，流程名：{handle.Name}，递归深度：{depth}");
#endif
            return handle;
        }

        // 计算递归深度
        private int CalculateRecursionDepth(ProcedureHandle parent)
        {
            int depth = 1; // 当前流程本身算一层

            // 从父流程向上遍历计算深度
            var current = parent;
            while (current != null)
            {
                depth++;

                // 如果深度已经超过最大限制，直接返回
                if (depth > MAX_RECURSION_DEPTH)
                {
                    return depth;
                }

                current = current.Parent;
            }

            return depth;
        }

        // 更新所有运行中的流程
        private void UpdateAll()
        {
            // 创建副本避免在迭代过程中修改集合
            var contexts = _contexts.Values.ToList();

            foreach (var context in contexts)
            {
                context.Update();
            }
        }

        // 移除上下文
        internal void RemoveContext(int id)
        {
            if (_contexts.Remove(id))
            {
                var rootHandle = _rootHandles.FirstOrDefault(h => h.ID == id);
                if (rootHandle != null)
                {
                    _rootHandles.Remove(rootHandle);
                }
            }
        }

        // 停止所有流程
        public void StopAll()
        {
            foreach (var handle in _rootHandles.ToList())
            {
                handle.Stop();
            }

            _rootHandles.Clear();
        }

        protected override void OnInit() => SetupUpdater();

        #region 调试工具

        // 获取所有活动流程的递归树
        public string GetActiveProcessesTree()
        {
            var tree = new System.Text.StringBuilder();
            tree.AppendLine("活动流程树:");

            foreach (var rootHandle in _rootHandles)
            {
                BuildProcessTree(rootHandle, tree, 0);
            }

            return tree.ToString();
        }

        private void BuildProcessTree(ProcedureHandle handle, System.Text.StringBuilder tree, int indent)
        {
            tree.Append(' ', indent * 2);
            tree.AppendLine(
                $"- [{handle.ID}] {handle.Name} (状态: {handle.State}, 深度: {CalculateRecursionDepth(handle.Parent)})");

            // 如果有子流程，递归添加
            foreach (var child in handle.Children)
            {
                BuildProcessTree(child, tree, indent + 1);
            }
        }

        #endregion
    }
}