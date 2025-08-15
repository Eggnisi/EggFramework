#region

//文件创建者：Egg
//创建时间：04-20 11:00

#endregion
#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;



namespace EggFramework.AudioSystem
{
    public sealed class AudioOutputModifyWindow : OdinEditorWindow
    {
        private AudioOutputSetting _setting;
        private AudioData          _data;

        public void Init(AudioOutputSetting setting, AudioData data)
        {
            _setting = setting;
            _data    = data;
            Groups   = _setting.AudioGroups.ToList();
        }

        private List<string> GetFreeAudioGroup()
        {
            return (from audioGroup in _data.Groups
                where _data.Settings.All(setting => !setting.AudioGroups.Contains(audioGroup.GroupName))
                select audioGroup.GroupName).ToList();
        }

        [ShowInInspector, LabelText("输出路径"), ReadOnly]
        public string OutputPath => _setting.OutputMixerGroupPath;

        [LabelText("绑定音频组"), ValueDropdown("GetFreeAudioGroup", IsUniqueList = true)]
        public List<string> Groups = new();

        [Button("保存", ButtonSizes.Large)]
        private void Save()
        {
            _setting.AudioGroups = Groups;
            Close();
            EditorUtility.SetDirty(_data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif