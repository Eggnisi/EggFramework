#region

//文件创建者：Egg
//创建时间：03-26 01:35

#endregion

#if UNITY_EDITOR


using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace EggFramework.Util.EggCMD
{
    public sealed class CommandEditorWindow : EditorWindow
    {
        public VisualTreeAsset CommandLineAsset;
        public VisualTreeAsset VisualTreeAsset;
        public VisualTreeAsset CommandPopupAsset;

        private VisualElement _scrollContainer;
        private ScrollView _scrollView;
        private TextField _textField;
        private ListView _listView;
        private readonly Vector2 _popupOffset = new(0, 0);

        private int _listViewIndex;

        public static bool Enable { get; private set; }

        public void OnEnable()
        {
            Enable = true;
        }

        public void OnDisable()
        {
            Enable = false;
        }

        [MenuItem("EggFramework/命令行窗口 #&c")]
        private static void OpenWindow()
        {
            var window = GetWindow<CommandEditorWindow>();
            window.titleContent = new GUIContent("命令行工具");
            window.Show();
        }

        public void OutputString(string value, bool userInput)
        {
            var labelElement = CommandLineAsset.CloneTree();
            var label = labelElement.Q<Label>("Content");
            label.text = userInput ? CommandOutputModifier.UserPrefix() + value : value;
            _scrollContainer.Add(label);

            _scrollView.schedule.Execute(() =>
            {
                // 滚动到底部
                _scrollView.scrollOffset = new Vector2(0, _scrollView.contentContainer.layout.height);
            }).StartingIn(0);
        }

        public void ClearString()
        {
            _scrollContainer.Clear();
        }

        private readonly List<string> _cachedDisplayTokens = new();
        private readonly List<string> _cachedContentTokens = new();
        private readonly List<bool> _cachedAvailable = new();

        private void CreateGUI()
        {
            VisualTreeAsset = ResUtil.GetAsset<VisualTreeAsset>("CommandEditorUxml");
            CommandLineAsset = ResUtil.GetAsset<VisualTreeAsset>("CommandLineUxml");
            CommandPopupAsset = ResUtil.GetAsset<VisualTreeAsset>("CommandPopupUxml");
            _listView = CommandPopupAsset.CloneTree().Q<ListView>();
            _listView.style.display = DisplayStyle.None;
            var tree = VisualTreeAsset.CloneTree();

            _scrollContainer = tree.Q<VisualElement>("ScrollContainer");
            _scrollView = tree.Q<ScrollView>("ScrollView");
            _textField = tree.Q<TextField>("InputField");
            _textField.RegisterCallback<KeyDownEvent>(e =>
            {
                switch (e.keyCode)
                {
                    case KeyCode.Return or KeyCode.KeypadEnter when _listView.style.display == DisplayStyle.None:
                    {
                        var command = _textField.text;
                        OutputString(_textField.text, true);
                        _textField.SetValueWithoutNotify(string.Empty);
                        _textField.Focus();
                        CommandManager.DoCommand(command);
                        break;
                    }
                    case KeyCode.Return or KeyCode.KeypadEnter when _listView.style.display == DisplayStyle.Flex:
                    {
                        if (!string.IsNullOrEmpty(_cachedContentTokens[_listView.selectedIndex]))
                        {
                            _provider.InsertSuggestion(_cachedContentTokens[_listView.selectedIndex]);
                            _listView.schedule.Execute(() => { _listView.style.display = DisplayStyle.None; })
                                .StartingIn(0);
                        }

                        break;
                    }
                    case KeyCode.UpArrow when _listView.style.display == DisplayStyle.Flex:
                    {
                        _listViewIndex = _listView.selectedIndex =
                            Mathf.Clamp(_listViewIndex - 1, 0, _cachedDisplayTokens.Count - 1);
                        break;
                    }
                    case KeyCode.DownArrow when _listView.style.display == DisplayStyle.Flex:
                    {
                        _listViewIndex = _listView.selectedIndex =
                            Mathf.Clamp(_listViewIndex + 1, 0, _cachedDisplayTokens.Count - 1);
                        break;
                    }
                }
            });

            _listView.itemsSource = _cachedDisplayTokens;
            _listView.makeItem = () => new Label();
            _listView.bindItem = (e, i) =>
            {
                var label = e.Q<Label>();
                label.style.color = _cachedAvailable[i] ? Color.white : Color.gray;
                label.text = _cachedDisplayTokens[i];
            };
            _listView.pickingMode = PickingMode.Ignore;

            _textField.RegisterCallback<FocusInEvent>(OnFocusIn);
            _textField.RegisterCallback<FocusOutEvent>(OnFocusOut);
            _textField.RegisterValueChangedCallback(OnTextChanged);

            rootVisualElement.Add(tree);
            rootVisualElement.Add(_listView);
        }

        private void OnFocusIn(FocusInEvent evt)
        {
            _listView.style.display = DisplayStyle.Flex;
            UpdateListPosition();
            RefreshListView();
        }

        private void OnFocusOut(FocusOutEvent evt)
        {
            _listView.style.display = DisplayStyle.None;
        }

        private void OnTextChanged(ChangeEvent<string> evt)
        {
            UpdateListPosition();
            RefreshListView();
        }

        private CommandAutoCompleteProvider _provider;

        private void RefreshListView()
        {
            var context = new CommandInputContext
            {
                CurrentTokenIndex = CommandParser.ParseTokenIndex(_textField.value, _textField.cursorIndex),
                Tokens = TokenSplitter.ParseTokens(_textField.value, CommandParser.TOKEN_DELIMITER),
                TokenFinish = _textField.value.EndsWith(CommandParser.TOKEN_DELIMITER)
            };
            RefreshCache(context);

            _listView.Rebuild();
            _listView.selectedIndex = _listViewIndex = 0;

            _listView.style.display = _cachedDisplayTokens.Count != 0 ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void RefreshCache(CommandInputContext context)
        {
            _provider = new CommandAutoCompleteProvider(context, _textField, CommandHelper.Helpers);

            var (t1, t2, t3) = _provider.ProvideAutoComplete();

            _cachedDisplayTokens.Clear();
            _cachedDisplayTokens.AddRange(t1);
            _cachedContentTokens.Clear();
            _cachedContentTokens.AddRange(t2);
            _cachedAvailable.Clear();
            _cachedAvailable.AddRange(t3);
        }

        private void UpdateListPosition()
        {
            // 获取光标索引
            int cursorIndex = _textField.cursorIndex;
            cursorIndex = Mathf.Clamp(cursorIndex, 0, _textField.text.Length);

            // 通过文本选择器获取光标位置
            Vector2 cursorPos = _textField.MeasureTextSize(
                _textField.text[..cursorIndex],
                _textField.resolvedStyle.width,
                VisualElement.MeasureMode.Undefined,
                _textField.resolvedStyle.height,
                VisualElement.MeasureMode.Exactly
            );

            // 转换为全局坐标
            Vector2 worldPos = _textField.LocalToWorld(cursorPos);
            Vector2 localPos = _listView.parent.WorldToLocal(worldPos);

            // 设置ListView位置（偏移量可调整）
            _listView.style.top = localPos.y + _popupOffset.y;
            _listView.style.left = localPos.x + _popupOffset.x;
        }
    }
}
#endif