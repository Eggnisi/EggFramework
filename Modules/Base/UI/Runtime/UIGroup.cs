#region

//文件创建者：Egg
//创建时间：10-31 08:06

#endregion

using System.Collections.Generic;
using System.Linq;

namespace EggFramework.Modules.UI
{
    public class UIGroup
    {
        protected readonly LinkedList<UIFormInfo> _uiFormInfos = new();

        protected IUISystem _uiSystem;

        public int UIFormCount => _uiFormInfos.Count;

        public IUIForm CurrentUIForm => _uiFormInfos.First()?.UIForm;

        protected sealed class UIFormInfo
        {
            public IUIForm UIForm;
            public bool    Paused;
            public bool    Covered;
        }

        public UIGroup(IUISystem uiSystem, string name, int depth, bool pause)
        {
            _uiSystem = uiSystem;
            Name      = name;
            Depth     = depth;
            Pause     = pause;
        }

        public string Name { get; }

        public int Depth { get; set; }

        public bool Pause { get; set; }

        public IEnumerable<IUIForm> GetUIForms()
        {
            return _uiFormInfos.Select(formInfo => formInfo.UIForm);
        }

        public IUIForm GetUIForm(string formName)
        {
            var current = _uiFormInfos.First;

            while (current is { Value: not null })
            {
                var next = current.Next;
                if (current.Value.UIForm.Name == formName) return current.Value.UIForm;
                current = next;
            }

            return null;
        }

        public void OnUpdate()
        {
            var current = _uiFormInfos.First;

            while (current is { Value: not null })
            {
                var next = current.Next;
                current.Value.UIForm.OnUpdate();
                current = next;
            }
        }
    }
}