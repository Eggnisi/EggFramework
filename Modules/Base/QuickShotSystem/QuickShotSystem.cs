#region

//文件创建者：Egg
//创建时间：09-08 09:15

#endregion

using System.Collections.Generic;
using System.Linq;
using EggFramework.ObjectPool;
using QFramework;
using UnityEngine;

namespace EggFramework
{
    public sealed class QuickShotSystem : AbstractModule, IQuickShotSystem
    {
        //所有快照数据
        private readonly List<QuickShot> _quickShots = new();

        //下一个快照Id
        private static int _nextQuickShotId;

        //使用栈来存储快照Id
        private readonly Stack<int> _quickShotStack = new();

        //受到管理的类
        private readonly HashSet<ICanQuickShot> _quickShotClasses = new();

        //临时数据
        private readonly Dictionary<ICanQuickShot, Dictionary<string, object>> _tempCustomQuickShotData = new();
        private readonly Dictionary<ICanQuickShot, Dictionary<string, object>> _tempQuickShotData       = new();

        private IObjectPoolSystem _objectPoolSystem;

        protected override void OnInit()
        {
            _objectPoolSystem = this.GetSystem<IObjectPoolSystem>();
            _objectPoolSystem?.RegisterCallbacks("QuickShotData",
                o => ((Dictionary<ICanQuickShot, Dictionary<string, object>>)o).Clear(), null);
        }

        public IReadOnlyList<QuickShot> GetDatas() => _quickShots;
        public int CurrentQuickShotCount => _quickShotStack.Count;
        public int HistoryQuickShotCount { get; private set; } = 0;

        public void RegisterQuickShotClass(ICanQuickShot canQuickShot) => _quickShotClasses.Add(canQuickShot);
        public void UnregisterQuickShotClass(ICanQuickShot canQuickShot) => _quickShotClasses.Remove(canQuickShot);

        public void Clear()
        {
            _quickShots.Clear();
            _quickShotStack.Clear();
            _quickShotClasses.Clear();
            _nextQuickShotId      = 0;
            HistoryQuickShotCount = 0;
        }

        public void DeleteQuickShot(int quickShotId)
        {
            var quickShot = _quickShots.Find(qs => qs.Id == quickShotId);
            if (quickShot == null) return;
            _objectPoolSystem?.ReturnObj("QuickShotData", quickShot.QuickShotData);
            _quickShots.Remove(quickShot);
        }

        public int TakeRandomQuickShot()
        {
            foreach (var quickShotClass in _quickShotClasses)
            {
                quickShotClass.BeforeUpload();
            }

            _tempCustomQuickShotData.Clear();

            foreach (var abstractQuickShotClass in _quickShotClasses)
            {
                var data = abstractQuickShotClass.UploadQuickShotData();
                if (!_tempCustomQuickShotData.TryAdd(abstractQuickShotClass, data))
                {
                    _tempCustomQuickShotData[abstractQuickShotClass] = data;
                }
            }
            
            var quickShotData =
                _objectPoolSystem?.GetObj<Dictionary<ICanQuickShot, Dictionary<string, object>>>("QuickShotData") ??
                new Dictionary<ICanQuickShot, Dictionary<string, object>>();
            foreach (var keyValuePair in _tempCustomQuickShotData)
            {
                quickShotData.Add(keyValuePair.Key, keyValuePair.Value.ToDictionary(dic => dic.Key, dic => dic.Value));
            }

            var randomId = GetRandomID();

            _quickShots.Add(new QuickShot
            {
                Id            = randomId,
                QuickShotData = quickShotData,
            });
            HistoryQuickShotCount++;

            return randomId;

            int GetRandomID()
            {
                var id = Random.Range(int.MaxValue / 2, int.MaxValue);
                while (_quickShots.Any(qs => qs.Id == id))
                {
                    id = Random.Range(int.MaxValue / 2, int.MaxValue);
                }

                return id;
            }
        }

        public int PushQuickShot()
        {
            CollectQuickShotData();
            PushQuickShotDataInStack();
            return _nextQuickShotId - 1;
        }

        public void CollectQuickShotData()
        {
            foreach (var quickShotClass in _quickShotClasses) quickShotClass.BeforeUpload();

            _tempQuickShotData.Clear();

            foreach (var abstractQuickShotClass in _quickShotClasses)
            {
                var data = abstractQuickShotClass.UploadQuickShotData();
                if (!_tempQuickShotData.TryAdd(abstractQuickShotClass, data))
                {
                    _tempQuickShotData[abstractQuickShotClass] = data;
                }
            }
        }

        public void PushQuickShotDataInStack()
        {
            var quickShotData = _tempQuickShotData.ToDictionary(keyValuePair => keyValuePair.Key,
                keyValuePair => keyValuePair.Value.ToDictionary(dic => dic.Key, dic => dic.Value));

            _quickShots.Add(new QuickShot
            {
                Id            = _nextQuickShotId++,
                QuickShotData = quickShotData,
            });
            HistoryQuickShotCount++;
            _quickShotStack.Push(_nextQuickShotId - 1);
        }

        public void PopQuickShot()
        {
            if (_quickShotStack.Count <= 0) return;
            this.SendEvent(new RecoverQuickShotEvent());
            PopQuickShot(_quickShotStack.Pop());
        }

        public void PopQuickShot(int id) => InnerRecoverQuickShot(id, true);

        public void PeekQuickShot()
        {
            if (_quickShotStack.Count <= 0) return;
            this.SendEvent(new RecoverQuickShotEvent());
            PeekQuickShot(_quickShotStack.Peek());
        }

        public void PeekQuickShot(int id) => InnerRecoverQuickShot(id, false);

        private void InnerRecoverQuickShot(int id, bool delete)
        {
            var quickShot = _quickShots.Find(quickShot => quickShot.Id == id);
            if (quickShot == null) return;

            foreach (var abstractQuickShotClass in _quickShotClasses.Where(abstractQuickShotClass =>
                         quickShot.QuickShotData.TryGetValue(abstractQuickShotClass, out _)))
            {
                abstractQuickShotClass.DownloadQuickShot(
                    quickShot.QuickShotData.GetValueOrDefault(abstractQuickShotClass));
            }

            if (delete)
            {
                _objectPoolSystem?.ReturnObj("QuickShotData", quickShot.QuickShotData);
                _quickShots.Remove(quickShot);
            }

            foreach (var quickShotClass in _quickShotClasses)
            {
                quickShotClass.AfterDownload();
            }
        }
    }
}