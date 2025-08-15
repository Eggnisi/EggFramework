#region

//文件创建者：Egg
//创建时间：04-20 10:28

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

#if UNITY_EDITOR
namespace EggFramework.AudioSystem
{
    public sealed class AudioEditorWindowOutputSetting
    {
        private readonly AudioSetting      _setting;
        private readonly AudioData         _data;
        private readonly AudioEditorWindow _host;

        public AudioEditorWindowOutputSetting(AudioEditorWindow host, AudioSetting setting, AudioData data)
        {
            _host    = host;
            _setting = setting;
            _data    = data;
            RefreshMixerData();
        }

        [ShowInInspector, LabelText("混音器")]
        public AudioMixer Mixer
        {
            get => _data.Mixer;
            set => _data.Mixer = value;
        }

        public List<AudioOutputModifyColumn> Columns;

        [Button("刷新总线输出组", ButtonSizes.Large)]
        public void RefreshMixerData()
        {
            if (!Mixer)
            {
                Debug.Log("[AudioSystem]::没有指定Audio Mixer，无法使用Group功能");
                return;
            }

            var paths = GetOutputPaths();
            ClearPath(paths);
            PushNewPath(paths);
            Columns = _data.Settings.Select(setting => new AudioOutputModifyColumn(setting, _data)).ToList();
        }

        private void PushNewPath(List<string> paths)
        {
            var settings = _data.Settings;
            foreach (var path in paths.Where(path => settings.All(setting => setting.OutputMixerGroupPath != path)))
            {
                settings.Add(new AudioOutputSetting
                {
                    OutputMixerGroupPath = path,
                    AudioGroups          = new List<string>()
                });
            }
        }


        private void ClearPath(List<string> paths)
        {
            var deleteList = new List<AudioOutputSetting>();
            var settings   = _data.Settings;
            foreach (var setting in settings)
            {
                if (!paths.Contains(setting.OutputMixerGroupPath))
                {
                    deleteList.Add(setting);
                }
            }

            foreach (var audioOutputSetting in deleteList)
            {
                settings.Remove(audioOutputSetting);
            }
        }

        private List<string> GetOutputPaths()
        {
            var paths = new List<string>();

            var allGroups = Mixer.FindMatchingGroups("");
            if (allGroups.Length == 0)
            {
                Debug.LogError("未找到任何Group");
                return paths;
            }

            AddPath(allGroups[0], "Master", paths);
            return paths;
        }

        private void AddPath(AudioMixerGroup group, string currentPath, List<string> paths)
        {
            var children = (AudioMixerGroup[])group.GetType().GetProperty("children").GetValue(group);
            paths.Add(currentPath);
            foreach (var audioMixerGroup in children)
            {
                AddPath(audioMixerGroup, currentPath + "/" + audioMixerGroup.name, paths);
            }
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