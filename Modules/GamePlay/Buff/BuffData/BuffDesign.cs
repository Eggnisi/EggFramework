using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace EggFramework
{
    public enum BuffUpdateEnum
    {
        [LabelText("增加持续时间")]      AddTime,
        [LabelText("刷新持续时间并增加层数")] ReplaceAndAddStack,
        [LabelText("保留持续时间并增加层数")] KeepAndAddStack
    }

    public enum BuffRemoveStackEnum
    {
        [LabelText("全部清除")] Clear,
        [LabelText("减少层数")] Reduce
    }

    [Serializable]
    public class BuffRunTimeInfo : IBuffTicker, IBuffAction
    {
        public BuffData   BuffData;
        
        public GameObject Creator;
        public GameObject Target;
        
        float IBuffTicker.DurationTimer { get; set; }
        float IBuffTicker.TickTimer     { get; set; }
        int IBuffTicker.  CurStack      { get; set; } = 1;
        private Dictionary<string, Action> _buffActions { get; } = new();
        public void RegisterBuffAction(string callbackName, Action action)
        {
            if (!_buffActions.TryAdd(callbackName, action))
            {
                _buffActions[callbackName] += action;
            }
        }
        public void TriggerBuffAction(string callbackName)
        {
            _buffActions.TryGetValue(callbackName, out var action);
            action?.Invoke();
        }
        
    }

    public interface IBuffAction
    {
        void RegisterBuffAction(string callbackName, Action action);

        void TriggerBuffAction(string callbackName);
    }

    public interface IBuffTicker
    {
        float DurationTimer { get; set; }
        float TickTimer     { get; set; }
        int   CurStack      { get; set; }
    }
}