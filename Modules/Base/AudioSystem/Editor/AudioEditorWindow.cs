#region

//文件创建者：Egg
//创建时间：10-12 11:01

#endregion

#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using EggFramework.Util;
using QFramework;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace EggFramework.AudioSystem
{
    public sealed class AudioEditorWindow : OdinMenuEditorWindow
    {
        [MenuItem("EggFramework/音频管理面板")]
        public static void OpenWindow()
        {
            var window = GetWindow<AudioEditorWindow>();
            window.titleContent = new GUIContent("音频管理面板");
        }

        [LabelText("音频数据")] public AudioData Data;

        protected override void OnEnable()
        {
            base.OnEnable();
            Setting = StorageUtil.LoadFromSettingFile(nameof(AudioSetting), new AudioSetting());
            Data    = ResUtil.GetAsset<AudioData>();
            if (Data != null) return;
            GenerateAudioData();
        }

        [Button("重新生成音频数据", ButtonSizes.Large)]
        public void GenerateAudioData()
        {
            GetWindow<AudioEditorWindow>().Close();
            var path = Setting.AudioDataPath + "/AudioData.asset";
            Data = ResUtil.GetOrCreateAsset<AudioData>(path);
            var fxs = ResUtil.GetAssetsInFolder<AudioClip>(Setting.AudioFXFolderPath);
            if (fxs != null)
            {
                Data.SFXAudios.Clear();
                Data.SFXAudios.AddRange(fxs);
            }

            var bgms = ResUtil.GetAssetsInFolder<AudioClip>(Setting.AudioBGMFolderPath);
            if (bgms != null)
            {
                Data.BGMAudios.Clear();
                Data.BGMAudios.AddRange(bgms);
            }

            StorageUtil.SaveToSettingFile("AudioConstant.BGMIds", Data.BGMAudios.Select(au => au.name).ToList());
            StorageUtil.SaveToSettingFile("AudioConstant.SFXIds", Data.SFXAudios.Select(au => au.name).ToList());

            ResUtil.AddAssetToGroup("AudioData", path, true, true);
            EditorUtility.SetDirty(Data);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GenerateAudioConstant();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            StorageUtil.SaveToSettingFile(nameof(AudioSetting), Setting);
        }

        [LabelText("音频设置")] public AudioSetting Setting;

        public void GenerateAudioConstant()
        {
            AudioConstantGenerator.Generate(Setting.AudioCodePath, Setting.AudioNameSpace);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree
            {
                { "配置面板", new AudioEditorWindowView(this, Setting, Data) },
                { "资源查看", new AudioEditorWindowPreviewClip(this, Setting, Data) },
                { "音频组设置", new AudioEditorWindowGroupSetting(this, Setting, Data) },
                { "输出设置", new AudioEditorWindowOutputSetting(this, Setting, Data) },
            };
            return tree;
        }
    }
}
#endif