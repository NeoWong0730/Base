using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace NWFramework
{
    /// <summary>
    /// 音频模块接口
    /// </summary>
    public interface IAudioModule
    {
        /// <summary>
        /// 总音量控制
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// 总开关
        /// </summary>
        bool Enable { get; set; }

        /// <summary>
        /// 音乐音量
        /// </summary>
        float MusicVolume { get; set; }

        /// <summary>
        /// 音效音量
        /// </summary>
        float SoundVolume { get; set; }

        /// <summary>
        /// UI音效音量
        /// </summary>
        float UISoundVolume { get; set; }

        /// <summary>
        /// 语音音量
        /// </summary>
        float VoiceVolume { get; set; }

        /// <summary>
        /// 音乐开关
        /// </summary>
        bool MusicEnable { get; set; }

        /// <summary>
        /// 音效开关
        /// </summary>
        bool SoundEnable { get; set; }

        /// <summary>
        /// UI音效开关
        /// </summary>
        bool UISoundEnable { get; set; }

        /// <summary>
        /// 语音开关
        /// </summary>
        bool VoiceEnable { get; set; }

        /// <summary>
        /// 初始化音频模块
        /// </summary>
        /// <param name="audioGroupConfigs">音频轨道组配置</param>
        /// <param name="instanceRoot">实例化根节点</param>
        /// <param name="audioMixer">音频混响器</param>
        void Initialize(AudioGroupConfig[] audioGroupConfigs, Transform instanceRoot = null, AudioMixer audioMixer = null);

        /// <summary>
        /// 重启音频模块
        /// </summary>
        void Restart();

        /// <summary>
        /// 播放音频接口
        /// </summary>
        /// <remarks>如果超过最大发声数采用fadeout的方式复用最久播放的AudioSource。</remarks>
        /// <param name="type">声音类型</param>
        /// <param name="path">声音文件路径</param>
        /// <param name="loop">是否循环播放</param>
        /// <param name="volume">音量（0-1.0）</param>
        /// <param name="async">是否异步加载</param>
        /// <param name="inPool">是否支持资源池</param>
        /// <returns></returns>
        AudioAgent Play(AudioType type, string path, bool loop = false, float volume = 1.0f, bool async = false, bool inPool = false);

        /// <summary>
        /// 停止某类声音播放
        /// </summary>
        /// <param name="type">声音类型</param>
        /// <param name="fadeout">是否渐消</param>
        void Stop(AudioType type, bool fadeout);

        /// <summary>
        /// 停止所有声音
        /// </summary>
        /// <param name="fadeout">是否渐消</param>
        void StopAll(bool fadeout);

        /// <summary>
        /// 预先加载AudioClip，并放入对象池
        /// </summary>
        /// <param name="list">AudioClip的AssetPath集合</param>
        void PutInAudioPool(List<string> list);

        /// <summary>
        /// 将部分AudioClip从对象池移出
        /// </summary>
        /// <param name="list">AudioClip的AssetPath集合</param>
        void RemoveClipFromPool(List<string> list);

        /// <summary>
        /// 清空AudioClip的对象池
        /// </summary>
        void ClearSoundPool();
    }
}