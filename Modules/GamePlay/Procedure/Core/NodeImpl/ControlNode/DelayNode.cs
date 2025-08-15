#region

//文件创建者：Egg
//创建时间：07-19 12:54

#endregion

using Sirenix.OdinInspector;
using UnityEngine;

namespace EggFramework.Procedure
{
    [NodeTitle("延时节点")]
    [NodeTint("#8E44AD")]
    [CreateNodeMenu("控制节点/延时节点")]
    public sealed class DelayNode : SingleFlowNode
    {
        [Input(connectionType = ConnectionType.Override), LabelText("延时时间")]
        public float DelayTime = 1f; // 延时的时间，单位为秒

        public override bool IsFinished { get; protected set; }

        private float _timer;

        public override void OnExecute()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                Log("延时结束");
                IsFinished = true;
            }
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            DelayTime = GetInputValue(nameof(DelayTime), DelayTime);
            _timer    = DelayTime;
            Log($"延时开始，等待 {DelayTime} 秒");
            if (_timer <= 0)
            {
                LogWaring("延时时间小于等于0，立即结束延时");
                IsFinished = true;
            }
        }
    }
}