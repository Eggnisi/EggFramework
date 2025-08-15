#region

//文件创建者：Egg
//创建时间：04-20 10:24

#endregion

#if UNITY_EDITOR


using Sirenix.OdinInspector;

namespace EggFramework.AudioSystem
{
    public sealed class AudioEditorWindowView
    {
        [LabelText("配置信息")] public AudioSetting      Setting;
        [LabelText("音频数据")] public AudioData         Data;
        private readonly           AudioEditorWindow _host;

        public AudioEditorWindowView(AudioEditorWindow host, AudioSetting setting, AudioData data)
        {
            _host   = host;
            Setting = setting;
            Data    = data;
        }
        
        [Button("重新生成音频数据", ButtonSizes.Large)]
        private void Refresh()
        {
            _host.GenerateAudioData();
        }
    }
}
#endif