#region

//文件创建者：Egg
//创建时间：09-08 10:43

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using EggFramework.MonoUtil;
using EggFramework.Util;
using QFramework;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace EggFramework.AudioSystem
{
    public sealed class AudioSystem : AbstractModule, IAudioSystem
    {
        private readonly Dictionary<AudioGroup, int>                       _groupIndexDic      = new();
        private readonly Dictionary<AudioGroup, List<AudioSystemModifier>> _runtimeModifierDic = new();
        private          bool                                              _useMixer;
        
        // 音频修正管理器
        private AudioReviseManager _reviseManager;

        protected override void OnInit()
        {
            _audioObj = new GameObject("AudioSystemUpdater");
            Object.DontDestroyOnLoad(_audioObj);
            _audioObj.AddComponent<MonoUpdater>()
                .RegisterOnUpdate(Update);
            _bgmSource        = _audioObj.AddComponent<AudioSource>();
            _bgmSource.loop   = true;
            _bgmSource.volume = _bgmVolume;
            this.RegisterEvent<PlayBGMEvent>(e => PlayBGM(e.BGMName));
            this.RegisterEvent<PlaySFXEvent>(e => PlaySFX(e.FXName));
            this.RegisterEvent<PlayAudioGroupEvent>(e => PlayGroup(e.GroupName));
            
            // 初始化音频修正管理器
            _reviseManager = new AudioReviseManager();
        }

        private AudioData _data;

        protected override async UniTask OnAsyncInit()
        {
            SetAudioData(await ResUtil.LoadAssetAsync<AudioData>(nameof(AudioData)));
        }

        private          AudioSource       _bgmSource;
        private readonly List<AudioSource> _sfxSourcePool   = new();
        private readonly List<AudioSource> _sfxSourceInPlay = new();
        private          float             _sfxVolume       = 1;
        private          float             _bgmVolume       = 1;

        private GameObject _audioObj;

        private void Update()
        {
            var deleteList = _sfxSourceInPlay.Where(audioSource => !audioSource.isPlaying).ToList();
            foreach (var audioSource in deleteList)
            {
                _sfxSourcePool.Add(audioSource);
                _sfxSourceInPlay.Remove(audioSource);
            }
        }

        private AudioSource InnerPlaySFX(AudioClip clip)
        {
            AudioSource sfxSource;
            if (_sfxSourcePool.Count <= 0) sfxSource = ExpandSFXPool();
            else
            {
                sfxSource = _sfxSourcePool.First();
                _sfxSourcePool.Remove(sfxSource);
            }

            _sfxSourceInPlay.Add(sfxSource);
            
            // 使用修正管理器获取修正设置
            var reviseSettings = _reviseManager.GetReviseSettings(clip.name, _sfxVolume, _data?.ReviseData);
            sfxSource.volume = reviseSettings.FinalVolume;
            sfxSource.clip   = clip;
            sfxSource.loop   = false;
            sfxSource.Play();
            sfxSource.time = reviseSettings.StartPoint;
            return sfxSource;
        }

        private AudioSource ExpandSFXPool()
        {
            var fxSource = _audioObj.AddComponent<AudioSource>();
            fxSource.loop        = false;
            fxSource.playOnAwake = false;
            return fxSource;
        }

        private void SetAudioData(AudioData data)
        {
            _data = data;
            _onLoaded?.Invoke();
            _onLoaded = null;
            if (_data.Mixer) _useMixer = true;

            try
            {
                var clip = _data.BGMAudios.First();
                
                // 使用修正管理器获取修正设置
                var reviseSettings = _reviseManager.GetReviseSettings(clip.name, _bgmVolume, _data.ReviseData);
                _bgmSource.clip   = clip;
                _bgmSource.volume = reviseSettings.FinalVolume;
                _bgmSource.Play();
                _bgmSource.time = reviseSettings.StartPoint;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private readonly CustomTween _bgmSwitchTween = new();

        private AudioSource InnerPlayBGM(AudioClip clip)
        {
            _bgmSwitchTween.CheckAndGenTween(() =>
            {
                // 使用修正管理器获取修正设置
                var reviseSettings = _reviseManager.GetReviseSettings(clip.name, _bgmVolume, _data?.ReviseData);
                var rawVolume = reviseSettings.FinalVolume;
                
                return DOTween.Sequence()
                    .Append(DOTween.To(val => { _bgmSource.volume = rawVolume * (1 - val); }, 0, 1, 0.5f))
                    .AppendCallback(() =>
                    {
                        _bgmSource.clip = clip;
                        _bgmSource.Play();
                        _bgmSource.time = reviseSettings.StartPoint;
                    }).Append(DOTween.To(val => { _bgmSource.volume = rawVolume * val; }, 0, 1, 0.5f));
            }, CustomTween.ETweenCheckMode.Override);
            return _bgmSource;
        }

        public void PlayBGM(string bgm)
        {
            this.ExecuteInstantOrDont(() =>
            {
                var clip = _data.BGMAudios.Find(au => au.name == bgm);
                if (!clip)
                {
                    Debug.LogError($"找不到名为{bgm}的背景音乐");
                    return;
                }

                InnerPlayBGM(clip);
            });
        }

        public void StopBGM()
        {
            _bgmSource.Stop();
        }

        public void PlaySFX(string sfx)
        {
            this.ExecuteInstantOrDont(() =>
            {
                var clip = _data.SFXAudios.Find(au => au.name == sfx);
                if (!clip)
                {
                    Debug.LogError($"找不到名为{sfx}的音效");
                    return;
                }

                InnerPlaySFX(clip);
            });
        }

        public void SetBGMVolume(float strength)
        {
            strength   = Mathf.Clamp01(strength);
            _bgmVolume = strength;
            if (_bgmSource && _bgmSource.clip)
            {
                // 使用修正管理器获取修正设置
                var reviseSettings = _reviseManager.GetReviseSettings(_bgmSource.clip.name, _bgmVolume, _data?.ReviseData);
                _bgmSource.volume = reviseSettings.FinalVolume;
            }
        }

        public void SetSFXVolume(float strength)
        {
            strength   = Mathf.Clamp01(strength);
            _sfxVolume = strength;
        }

        public void BindBGMVolume(BindableProperty<float> bgm) => bgm.RegisterWithInitValue(SetBGMVolume);
        public void BindSFXVolume(BindableProperty<float> sfx) => sfx.RegisterWithInitValue(SetSFXVolume);

        public void PlayGroup(string groupName)
        {
            this.ExecuteInstantOrDont(() =>
            {
                if (GetGroupByName(groupName, out var group)) return;
                if (group.IsBGMGroup) PlayBGMByGroup(group);
                else PlaySFXByGroup(group);
            });
        }

        private Action _onLoaded;

        public void TriggerOnLoadedOrAfter(Action action)
        {
            if (!_data)
            {
                _onLoaded += action;
            }
            else action?.Invoke();
        }

        private void PlaySFXByGroup(AudioGroup group)
        {
            switch (group.PlayMode)
            {
                case AudioGroup.EAudioGroupPlayMode.First:
                {
                    var source = InnerPlaySFX(group.Clips[0]);
                    ApplyAudioModify(group, source);
                    AssignOutputWire(group, source);
                }
                    break;
                case AudioGroup.EAudioGroupPlayMode.Random:
                {
                    var source = InnerPlaySFX(group.Clips[Random.Range(0, group.Clips.Count)]);
                    ApplyAudioModify(group, source);
                    AssignOutputWire(group, source);
                }
                    break;
                case AudioGroup.EAudioGroupPlayMode.Loop:
                {
                    if (_groupIndexDic.TryGetValue(group, out var index))
                    {
                    }
                    else _groupIndexDic[group] = index;

                    index                 = (index + 1) % group.Clips.Count;
                    _groupIndexDic[group] = index;

                    var source = InnerPlaySFX(group.Clips[index]);
                    ApplyAudioModify(group, source);
                    AssignOutputWire(group, source);
                }
                    break;
                case AudioGroup.EAudioGroupPlayMode.All:
                {
                    foreach (var source in group.Clips.Select(InnerPlaySFX))
                    {
                        ApplyAudioModify(group, source);
                        AssignOutputWire(group, source);
                    }
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool GetGroupByName(string groupName, out AudioGroup group)
        {
            group = _data.Groups.Find(group => group.GroupName == groupName);
            if (group.Clips.Count <= 0)
            {
                Debug.LogError("音频组必须存在一个音频");
                return true;
            }

            return false;
        }

        private void PlayBGMByGroup(AudioGroup group)
        {
            switch (group.PlayMode)
            {
                case AudioGroup.EAudioGroupPlayMode.First:
                {
                    var source = InnerPlayBGM(group.Clips[0]);
                    ApplyAudioModify(group, source);
                    AssignOutputWire(group, source);
                }
                    break;
                case AudioGroup.EAudioGroupPlayMode.Random:
                {
                    var source = InnerPlayBGM(group.Clips[Random.Range(0, group.Clips.Count)]);
                    ApplyAudioModify(group, source);
                    AssignOutputWire(group, source);
                }
                    break;
                case AudioGroup.EAudioGroupPlayMode.Loop:
                {
                    if (_groupIndexDic.TryGetValue(group, out var index))
                    {
                    }
                    else _groupIndexDic[group] = index;

                    index                 = (index + 1) % group.Clips.Count;
                    _groupIndexDic[group] = index;

                    var source = InnerPlayBGM(group.Clips[index]);
                    ApplyAudioModify(group, source);
                    AssignOutputWire(group, source);
                }
                    break;
                case AudioGroup.EAudioGroupPlayMode.All:
                {
                    Debug.LogError("背景音乐默认只能有一个实例");
                }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void AssignOutputWire(AudioGroup group, AudioSource source)
        {
            if (!_useMixer) return;
            foreach (var setting in _data.Settings.Where(setting => setting.AudioGroups.Contains(group.GroupName)))
            {
                source.outputAudioMixerGroup = _data.Mixer.FindMatchingGroups(setting.OutputMixerGroupPath)[0];
            }
        }

        private void ApplyAudioModify(AudioGroup group, AudioSource source)
        {
            foreach (var audioSystemModifier in group.Modifiers)
            {
                audioSystemModifier.Modify(source);
            }

            if (_runtimeModifierDic.TryGetValue(group, out var list))
            {
                foreach (var audioSystemModifier in list)
                {
                    audioSystemModifier.Modify(source);
                }
            }
        }

        public void AddModifier(string groupName, AudioSystemModifier modifier)
        {
            if (GetGroupByName(groupName, out var group))
            {
                Debug.LogError($"没有找到音频组{groupName}");
                return;
            }

            if (_runtimeModifierDic.TryGetValue(group, out var list))
            {
                list.Add(modifier);
            }
            else _runtimeModifierDic[group] = new List<AudioSystemModifier> { modifier };
        }

        public void RemoveModifier(string groupName, AudioSystemModifier modifier)
        {
            if (GetGroupByName(groupName, out var group))
            {
                Debug.LogError($"没有找到音频组{groupName}");
                return;
            }

            if (_runtimeModifierDic.TryGetValue(group, out var list))
            {
                list.Remove(modifier);
            }
        }

        public void ClearModifier(string groupName)
        {
            if (GetGroupByName(groupName, out var group))
            {
                Debug.LogError($"没有找到音频组{groupName}");
                return;
            }

            if (_runtimeModifierDic.TryGetValue(group, out var list))
            {
                list.Clear();
            }
        }

        public IReadOnlyList<AudioSystemModifier> GetModifiers(string groupName)
        {
            if (!GetGroupByName(groupName, out var group))
                return _runtimeModifierDic.GetValueOrDefault(group, new List<AudioSystemModifier>());
            Debug.LogError($"没有找到音频组{groupName}");
            return new List<AudioSystemModifier>();
        }
    }

    /// <summary>
    /// 音频修正管理器 - 专门处理音频修正逻辑
    /// </summary>
    public class AudioReviseManager
    {
        /// <summary>
        /// 获取音频修正设置
        /// </summary>
        /// <param name="clipName">音频剪辑名称</param>
        /// <param name="baseVolume">基础音量</param>
        /// <param name="reviseData">修正数据</param>
        /// <returns>修正后的设置</returns>
        public ReviseSettings GetReviseSettings(string clipName, float baseVolume, AudioReviseData reviseData)
        {
            var reviseItem = GetReviseItem(clipName, reviseData);
            var finalVolume = CalculateFinalVolume(baseVolume, reviseItem, reviseData);
            
            return new ReviseSettings
            {
                StartPoint = reviseItem.StartPoint,
                FinalVolume = finalVolume
            };
        }

        /// <summary>
        /// 获取修正项
        /// </summary>
        private AudioReviseItem GetReviseItem(string clipName, AudioReviseData reviseData)
        {
            if (reviseData == null)
            {
                return new AudioReviseItem
                {
                    TargetAudio = clipName,
                    StartPoint = 0f,
                    VolumeRevise = 0f
                };
            }

            return reviseData.ReviseItems.Find(item => item.TargetAudio == clipName) ??
                   new AudioReviseItem
                   {
                       TargetAudio = clipName,
                       StartPoint = 0f,
                       VolumeRevise = 0f
                   };
        }

        /// <summary>
        /// 计算最终音量
        /// </summary>
        private float CalculateFinalVolume(float baseVolume, AudioReviseItem reviseItem, AudioReviseData reviseData)
        {
            if (reviseData == null)
            {
                return baseVolume + reviseItem.VolumeRevise;
            }

            var globalAdjustedVolume = Mathf.Lerp(
                reviseData.GlobalVolumeMapping.x,
                reviseData.GlobalVolumeMapping.y,
                baseVolume
            );
            
            return globalAdjustedVolume + reviseItem.VolumeRevise;
        }
    }

    /// <summary>
    /// 修正设置结果
    /// </summary>
    public struct ReviseSettings
    {
        public float StartPoint;
        public float FinalVolume;
    }
}