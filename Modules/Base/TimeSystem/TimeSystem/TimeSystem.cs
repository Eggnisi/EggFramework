#region

//文件创建者：Egg
//创建时间：10-30 10:15

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using EggFramework.MonoUtil;
using QFramework;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace EggFramework.TimeSystem
{
    public sealed class TimeSystem : AbstractModule, ITimeSystem
    {
        private MonoUpdater _updater;

        private readonly Dictionary<int, Task> _taskDic = new();

        private const float TIME_REMAIN_BEFORE_DESTROY = 60f;

        private class Task
        {
            public int        TaskId;
            public string     TaskName;
            public ETaskState State;
            public Action     Handle;
            public bool       IsPause;
            public bool       IgnoreTimeScale;
            public float      TimeRemainUtilNextTick;

            public int   RawLoops;
            public float RawLoopTime;
            public int   PastLoops;

            public float TimeRemainUtilDestroy;
        }

        protected override void OnInit()
        {
            _updater = Object.FindObjectOfType<MonoUpdater>();
            if (!_updater)
            {
                var obj = new GameObject("TimeSystemUpdater");
                _updater = obj.AddComponent<MonoUpdater>();
                Object.DontDestroyOnLoad(obj);
            }

            _updater.RegisterOnUpdate(OnUpdate);
        }

        private void OnUpdate()
        {
            UpdateTimeTask();

            UpdateTimeScale();
        }

        private void UpdateTimeTask()
        {
            var deleteList = new List<int>();
            var keys       = _taskDic.Keys.ToList(); // 创建键的副本

            foreach (var key in keys)
            {
                var value = _taskDic[key];
                switch (value.State)
                {
                    case ETaskState.Finish or ETaskState.Cancelled:
                        value.TimeRemainUtilDestroy -= Time.unscaledDeltaTime;
                        if (value.TimeRemainUtilDestroy < 0) deleteList.Add(key);
                        continue;
                    case ETaskState.BeforeStart:
                        value.State                  = ETaskState.OnGoing;
                        value.TimeRemainUtilNextTick = value.RawLoopTime;
                        break;
                }

                if (!value.IsPause)
                    value.TimeRemainUtilNextTick -= value.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

                if (value.TimeRemainUtilNextTick < 0)
                {
                    value.Handle?.Invoke();
                    value.PastLoops++;
                    if (value.PastLoops >= value.RawLoops && value.RawLoops != -1)
                    {
                        value.TimeRemainUtilDestroy = TIME_REMAIN_BEFORE_DESTROY;
                        value.State                 = ETaskState.Finish;
                    }
                    else
                    {
                        value.TimeRemainUtilNextTick = value.RawLoopTime;
                    }
                }
            }

            foreach (var i in deleteList)
            {
                _taskDic.Remove(i);
            }
        }

        private int GenerateRandomTaskId()
        {
            var taskId = Random.Range(0, int.MaxValue);
            while (_taskDic.ContainsKey(taskId))
            {
                taskId = Random.Range(0, int.MaxValue);
            }

            return taskId;
        }

        public int RegisterDelayTask(Action action, float delay, bool ignoreTimeScale = false)
        {
            var taskId = GenerateRandomTaskId();
            var task = new Task
            {
                TaskId                 = taskId,
                TaskName               = "",
                Handle                 = action,
                RawLoopTime            = delay,
                RawLoops               = 1,
                IgnoreTimeScale        = ignoreTimeScale,
                IsPause                = false,
                PastLoops              = 0,
                TimeRemainUtilNextTick = 0,
                TimeRemainUtilDestroy  = 0
            };
            _taskDic.Add(taskId, task);
            return taskId;
        }

        public int RegisterDelayTask(Action action, string taskName, float delay, bool ignoreTimeScale = false)
        {
            var taskId = GenerateRandomTaskId();
            var task = new Task
            {
                TaskId                 = taskId,
                TaskName               = taskName,
                Handle                 = action,
                RawLoopTime            = delay,
                RawLoops               = 1,
                IgnoreTimeScale        = ignoreTimeScale,
                IsPause                = false,
                PastLoops              = 0,
                TimeRemainUtilNextTick = 0,
                TimeRemainUtilDestroy  = 0
            };
            _taskDic.Add(taskId, task);
            return taskId;
        }

        public int RegisterLoopTask(Action action, float loopTime, int loops = -1, bool ignoreTimeScale = false)
        {
            if (loops == 1)
            {
                Debug.LogError("不能创建循环次数为一的LoopTask，请考虑使用DelayTask");
                return 0;
            }

            var taskId = GenerateRandomTaskId();
            var task = new Task
            {
                TaskId                 = taskId,
                TaskName               = "",
                Handle                 = action,
                RawLoopTime            = loopTime,
                RawLoops               = loops,
                IgnoreTimeScale        = ignoreTimeScale,
                IsPause                = false,
                PastLoops              = 0,
                TimeRemainUtilNextTick = 0,
                TimeRemainUtilDestroy  = 0
            };
            _taskDic.Add(taskId, task);
            return taskId;
        }

        public int RegisterLoopTask(Action action, string taskName, float loopTime, int loops = -1,
            bool ignoreTimeScale = false)
        {
            if (loops == 1)
            {
                Debug.LogError("不能创建循环次数为一的LoopTask，请考虑使用DelayTask");
                return 0;
            }

            var taskId = GenerateRandomTaskId();
            var task = new Task
            {
                TaskId                 = taskId,
                TaskName               = taskName,
                Handle                 = action,
                RawLoopTime            = loopTime,
                RawLoops               = loops,
                IgnoreTimeScale        = ignoreTimeScale,
                IsPause                = false,
                PastLoops              = 0,
                TimeRemainUtilNextTick = 0,
                TimeRemainUtilDestroy  = 0
            };
            _taskDic.Add(taskId, task);
            return taskId;
        }

        public DelayTaskState GetDelayTaskState(int taskId)
        {
            var task = GetTaskById(taskId);
            if (task == null) return null;
            if (task.RawLoops != 1)
            {
                Debug.LogError($"id为{taskId}的task不是DelayTask");
                return null;
            }

            var taskState = new DelayTaskState
            {
                TaskId                 = task.TaskId,
                TaskName               = task.TaskName,
                IgnoreTimeScale        = task.IgnoreTimeScale,
                TimeRemainUtilNextTick = task.TimeRemainUtilNextTick,
                State                  = task.State,
                RawDelay               = task.RawLoopTime
            };
            return taskState;
        }

        public LoopTaskState GetLoopTaskState(int taskId)
        {
            var task = GetTaskById(taskId);
            if (task == null) return null;

            if (task.RawLoops == 1)
            {
                Debug.LogError($"id为{taskId}的task不是LoopTask");
                return null;
            }

            var taskState = new LoopTaskState
            {
                TaskId                 = task.TaskId,
                TaskName               = task.TaskName,
                IgnoreTimeScale        = task.IgnoreTimeScale,
                TimeRemainUtilNextTick = task.TimeRemainUtilNextTick,
                State                  = task.State,
                RawLoopTime            = task.RawLoopTime,
                RawLoops               = task.RawLoops,
                PastLoops              = task.PastLoops
            };
            return taskState;
        }

        public DelayTaskState GetDelayTaskState(string taskName)
        {
            var task = GetTaskByName(taskName);
            if (task == null) return null;

            if (task.RawLoops != 1)
            {
                Debug.LogError($"name为{taskName}的task不是DelayTask");
                return null;
            }

            var taskState = new DelayTaskState
            {
                TaskId                 = task.TaskId,
                TaskName               = task.TaskName,
                IgnoreTimeScale        = task.IgnoreTimeScale,
                TimeRemainUtilNextTick = task.TimeRemainUtilNextTick,
                State                  = task.State,
                RawDelay               = task.RawLoopTime
            };
            return taskState;
        }

        private Task GetTaskById(int taskId)
        {
            if (_taskDic.TryGetValue(taskId, out var task))
            {
                return task;
            }

            Debug.Log($"没有找到对应的Task,id为{taskId}");
            return null;
        }

        private Task GetTaskByName(string taskName)
        {
            Task task = null;
            foreach (var (key, value) in _taskDic)
            {
                if (value.TaskName != taskName) continue;
                task = value;
                break;
            }

            if (task == null) Debug.Log($"没有找到name为{taskName}的task");

            return task;
        }

        public LoopTaskState GetLoopTaskState(string taskName)
        {
            var task = GetTaskByName(taskName);
            if (task == null) return null;

            if (task.RawLoops == 1)
            {
                Debug.Log($"name为{taskName}的task不是LoopTask");
                return null;
            }

            var taskState = new LoopTaskState
            {
                TaskId                 = task.TaskId,
                TaskName               = task.TaskName,
                IgnoreTimeScale        = task.IgnoreTimeScale,
                TimeRemainUtilNextTick = task.TimeRemainUtilNextTick,
                State                  = task.State,
                RawLoopTime            = task.RawLoopTime,
                RawLoops               = task.RawLoops,
                PastLoops              = task.PastLoops,
            };
            return taskState;
        }


        public void CancelTask(int taskId)
        {
            var task = GetTaskById(taskId);
            if (task != null)
            {
                task.TimeRemainUtilDestroy = TIME_REMAIN_BEFORE_DESTROY;
                task.State                 = ETaskState.Cancelled;
            }
        }

        public void CancelTask(string taskName)
        {
            var task = GetTaskByName(taskName);
            if (task != null)
            {
                task.TimeRemainUtilDestroy = TIME_REMAIN_BEFORE_DESTROY;
                task.State                 = ETaskState.Cancelled;
            }
        }

        public void PauseTask(int taskId)
        {
            var task = GetTaskById(taskId);
            if (task is { State: ETaskState.OnGoing })
            {
                task.IsPause = true;
            }
        }

        public void PauseTask(string taskName)
        {
            var task = GetTaskByName(taskName);
            if (task is { State: ETaskState.OnGoing })
            {
                task.IsPause = true;
            }
        }

        public void RecoverTask(int taskId)
        {
            var task = GetTaskById(taskId);
            if (task is { State: ETaskState.OnGoing })
            {
                task.IsPause = false;
            }
        }

        public void RecoverTask(string taskName)
        {
            var task = GetTaskByName(taskName);
            if (task is { State: ETaskState.OnGoing })
            {
                task.IsPause = false;
            }
        }

        public void ModifyDelayTaskState(int taskId, ModifyDelayTaskParam delayTaskParam)
        {
            var task = GetTaskById(taskId);
            if (task != null && task.RawLoops != 1)
            {
                Debug.Log($"Id为{taskId}的task不是DelayTask");
                return;
            }

            if (task != null && delayTaskParam != null)
            {
                task.IgnoreTimeScale        = delayTaskParam.IgnoreTimeScale;
                task.TimeRemainUtilNextTick = delayTaskParam.TimeRemainUtilNextTick;
                task.RawLoopTime            = delayTaskParam.RawDelay;
            }
        }

        public void ModifyLoopTaskState(int taskId, ModifyLoopTaskParam loopTaskParam)
        {
            var task = GetTaskById(taskId);
            if (task is { RawLoops: 1 })
            {
                Debug.Log($"Id为{taskId}的task不是LoopTask");
                return;
            }

            if (task != null && loopTaskParam != null)
            {
                task.IgnoreTimeScale        = loopTaskParam.IgnoreTimeScale;
                task.TimeRemainUtilNextTick = loopTaskParam.TimeRemainUtilNextTick;
                task.RawLoopTime            = loopTaskParam.RawLoopTime;
                task.RawLoops               = loopTaskParam.RawLoops;
            }
        }

        private class TimeScaleRequest
        {
            public float Duration;
            public float TargetScale;
            public float TimeRemain;
        }

        private readonly Stack<TimeScaleRequest> _requests = new();
        private          float                   _preservedTimeScale;

        private void UpdateTimeScale()
        {
            if (_requests.Count <= 0 || IsPaused) return;
            var request = _requests.Peek();
            request.TimeRemain -= Time.unscaledDeltaTime;
            Time.timeScale     =  request.TargetScale;
            if (request.TimeRemain < 0)
            {
                _requests.Pop();
                if (_requests.Count > 0)
                {
                    Time.timeScale = _requests.Peek().TargetScale;
                }
                else
                {
                    Time.timeScale = 1f;
                }
            }
        }

        public void ChangeTimeScale(float targetScale, float duration)
        {
            _requests.Push(new TimeScaleRequest
            {
                Duration    = duration,
                TargetScale = targetScale,
                TimeRemain  = duration
            });
        }

        public void ResetTimeScale()
        {
            _requests.Clear();
            Time.timeScale = _preservedTimeScale = 1f;
        }

        public void Pause()
        {
            if (IsPaused) return;
            IsPaused       = true;
            _preservedTimeScale = Time.timeScale;
            Time.timeScale = 0;
            _onPause?.Invoke();
        }

        public void Recover()
        {
            if (!IsPaused) return;
            IsPaused       = false;
            Time.timeScale = _preservedTimeScale;
            _onRecover?.Invoke();
        }

        public bool IsPaused { get; private set; }

        private Action _onPause;
        private Action _onRecover;

        public IUnRegister RegisterOnPause(Action action)
        {
            _onPause += action;
            return new CustomUnRegister(() => _onPause -= action);
        }

        public IUnRegister RegisterOnRecover(Action action)
        {
            _onRecover += action;
            return new CustomUnRegister(() => _onRecover -= action);
        }
    }
}