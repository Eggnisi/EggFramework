#region

//文件创建者：Egg
//创建时间：04-20 10:28

#endregion

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;

#if UNITY_EDITOR
namespace EggFramework.AudioSystem
{
    public sealed class AudioEditorWindowGroupSetting
    {
        private readonly AudioSetting      _setting;
        private readonly AudioData         _data;
        private readonly AudioEditorWindow _host;

        public AudioEditorWindowGroupSetting(AudioEditorWindow host, AudioSetting setting, AudioData data)
        {
            _host    = host;
            _setting = setting;
            _data    = data;
        }

        [ShowInInspector, LabelText("音频组配置"), HideReferenceObjectPicker]
        public List<AudioGroup> Groups
        {
            get => _data.Groups;
            set => _data.Groups = value;
        }

        [Button("保存数据", ButtonSizes.Large)]
        public void Save()
        {
            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif