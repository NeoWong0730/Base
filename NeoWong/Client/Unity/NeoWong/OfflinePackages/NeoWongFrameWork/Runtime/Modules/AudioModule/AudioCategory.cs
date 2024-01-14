using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace NWFramework
{
    /// <summary>
    /// 音频轨道（类别）
    /// </summary>
    [Serializable]
    public class AudioCategory
    {
        [SerializeField]
        private AudioMixer audioMixer = null;

        public List<AudioAgent> AudioAgents;

        private readonly AudioMixerGroup _audioMixerGroup;

        private AudioGroupConfig _audioGroupConfig;

        private int _maxChannel;

        private bool _enable = true;

        /// <summary>
        /// 音频混响器
        /// </summary>
        public AudioMixer AudioMixer => audioMixer;

        /// <summary>
        /// 音频混响器组
        /// </summary>
        public AudioMixerGroup AudioMixerGroup => _audioMixerGroup;

        /// <summary>
        /// 音频组配置
        /// </summary>
        public AudioGroupConfig AudioGroupConfig => _audioGroupConfig;

        /// <summary>
        /// 实例化根节点
        /// </summary>
        public Transform InstanceRoot { get; private set; }

        public bool Enable
        {
            get => _enable;
            set
            {
                if ( _enable != value)
                {
                    _enable = value;
                    if (!_enable)
                    {
                        foreach (var audioAgent in AudioAgents)
                        {
                            audioAgent?.Stop();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 音频轨道构造函数
        /// </summary>
        /// <param name="maxChannel">最大Channel</param>
        /// <param name="audioMixer">音频混响器</param>
        /// <param name="audioGroupConfig">音频轨道组配置</param>
        public AudioCategory(int maxChannel, AudioMixer audioMixer, AudioGroupConfig audioGroupConfig)
        {
            this.audioMixer = audioMixer;
            _maxChannel = maxChannel;
            _audioGroupConfig = audioGroupConfig;
            AudioMixerGroup[] audioMixerGroups = audioMixer.FindMatchingGroups(Utility.Text.Format("Master/{0}", audioGroupConfig.AudioType.ToString()));
            if (audioMixerGroups.Length > 0)
            {
                _audioMixerGroup = audioMixerGroups[0];
            }
            else
            {
                _audioMixerGroup = audioMixer.FindMatchingGroups("Master")[0];
            }

            AudioAgents = new List<AudioAgent>(32);
            InstanceRoot = new GameObject(Utility.Text.Format("Audio Category - {0}", _audioMixerGroup.name)).transform;
            InstanceRoot.SetParent(GameModule.Audio.InstanceRoot);
            for (int i = 0; i < _maxChannel; i++)
            {
                AudioAgent audioAgent = new AudioAgent();
                audioAgent.Init(this, i);
                AudioAgents.Add(audioAgent);
            }
        }

        /// <summary>
        /// 增加音频
        /// </summary>
        /// <param name="num">Channel数量</param>
        public void AddAudio(int num)
        {
            _maxChannel += num;
            for (int i = 0; i < num; i++)
            {
                AudioAgents.Add(null);
            }
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        /// <param name="path">声音文件路径</param>
        /// <param name="async">是否异步加载</param>
        /// <param name="inPool">是否支持资源池</param>
        /// <returns></returns>
        public AudioAgent Play(string path, bool async, bool inPool = false)
        {
            if (!_enable)
            {
                return null;
            }

            int freeChannel = -1;
            float duration = -1;

            for (int i = 0; i < AudioAgents.Count; i++)
            {
                if (AudioAgents[i].AudioData?.AssetOperationHandle == null || AudioAgents[i].IsFree)
                {
                    freeChannel = i;
                    break;
                }
                else if (AudioAgents[i].Duration > duration)
                {
                    duration = AudioAgents[i].Duration;
                    freeChannel = i;
                }
            }

            if (freeChannel >= 0)
            {
                if (AudioAgents[freeChannel] == null)
                {
                    AudioAgents[freeChannel] = AudioAgent.Create(path, async, this, inPool);
                }
                else
                {
                    AudioAgents[freeChannel].Load(path, async, inPool);
                }

                return AudioAgents[freeChannel];
            }
            else
            {
                Log.Error($"Here is no channel to play audio {path}");
                return null;
            }
        }

        /// <summary>
        /// 暂停音频
        /// </summary>
        /// <param name="fadeout">是否渐出</param>
        public void Stop(bool fadeout)
        {
            for (int i = 0; i < AudioAgents.Count; i++)
            {
                AudioAgents[i]?.Stop(fadeout);
            }
        }

        /// <summary>
        /// 音频轨道轮询
        /// </summary>
        /// <param name="elpaseSeconds">逻辑流逝时间，以秒为单位</param>
        public void Update(float elpaseSeconds)
        {
            for (int i = 0; i < AudioAgents.Count; i++)
            {
                AudioAgents[i]?.Update(elpaseSeconds);
            }
        }
    }
}