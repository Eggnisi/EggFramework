#region

//文件创建者：Egg
//创建时间：10-31 07:53

#endregion

using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EggFramework.MonoUtil;
using EggFramework.ObjectPool;
using EggFramework.TimeSystem;
using QFramework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace EggFramework.Modules.UI
{
    public sealed class UISystem : AbstractModule, IUISystem
    {
        private readonly LinkedList<UIGroup> _uiGroups = new();

        private IObjectPoolSystem _objSystem;

        public static GameObject UIRoot
        {
            get
            {
                if (_uiRootInst) return _uiRootInst;
                _uiRootInst = new GameObject("UIRoot");
                Object.DontDestroyOnLoad(_uiRootInst);
                return _uiRootInst;
            }
        }

        private static GameObject _uiRootInst;

        private UIConfig _uiConfig;

        protected override void OnInit()
        {
            UIRoot.AddComponent<MonoUpdater>().RegisterOnUpdate(Update);
        }

        protected override async UniTask OnAsyncInit()
        {
            _uiConfig  = await Addressables.LoadAssetAsync<UIConfig>(nameof(UIConfig)).ToUniTask();
            _objSystem = this.GetSystem<IObjectPoolSystem>();
            var taskList = Enumerable.Select(_uiConfig.UIGroupConfigs,
                    group => _objSystem.RegisterPrefabs(group.UIFormConfigs.Select(uiConfig => uiConfig.AssetName)))
                .ToList();
            await UniTask.WhenAll(taskList);
        }

        public GameObject RequestUIInst(string assetName)
        {
            return _objSystem.GetGameObject(assetName);
        }

        public IUIForm OpenUIForm(string formName)
        {
            var current = _uiGroups.First;
            while (current is { Value: not null })
            {
                var next = current.Next;

                var uiform = current.Value.GetUIForm(formName);
                if (uiform != null)
                {
                    uiform.OnOpen();
                }

                current = next;
            }

            return null;
        }

        public IUIForm CloseUIForm(string formName)
        {
            return null;
        }

        public IUIForm GetUIForm(string formName)
        {
            var current = _uiGroups.First;
            while (current is { Value: not null })
            {
                var next   = current.Next;
                var uiForm = current.Value.GetUIForm(formName);
                if (uiForm != null) return uiForm;
                current = next;
            }

            return null;
        }

        public UIGroup GetUIGroup(string groupName)
        {
            return null;
        }

        public bool HasUIGroup(string groupName)
        {
            return false;
        }

        public IEnumerable<UIGroup> GetUIGroups()
        {
            return null;
        }

        public void RefocusUIForm(IUIForm uiForm)
        {
        }

        private void Update()
        {
            var current = _uiGroups.First;
            while (current is { Value: not null })
            {
                var next = current.Next;
                current.Value.OnUpdate();
                current = next;
            }
        }
    }
}