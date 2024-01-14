using System;
using System.Collections.Generic;
using Lib.Core;
using UnityEngine;
//#if USE_ADDRESSABLE_ASSET
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework {
    [System.Serializable]
    public class AudioEntry {
        public readonly AudioSource mAudioSource;

        public uint eAudioType { get; private set; }
        private bool isMutli;
        public string sAudioPath { get; private set; }
        private AsyncOperationHandle<AudioClip> mHandle;
        public const float VOLUME = 1f;
        public float fVolumeFade = VOLUME;

        // 默认的bgm音量缩放
        public static float DEFAULT_BGMVOLUE_SCALE = 0.5f;

        private readonly float FADE_LENGTH = 0.6f;

        private Timer fadeTimer;
        private Timer timer;

        private bool _isPause;

        public bool isPause {
            get { return this._isPause; }
            set {
                this._isPause = value;
                if (this._isPause != value) {
                    if (this._isPause) {
                        this.mAudioSource.Pause();
                        this.timer?.Pause();
                    }
                    else {
                        this.mAudioSource.UnPause();
                        this.timer?.Resume();
                    }
                }
            }
        }

        public AudioEntry(AudioSource audioSource) {
            this.mAudioSource = audioSource;
            this.mAudioSource.playOnAwake = false;
        }

        // AudioInfo复用设计
        public void Play(string audioPath, bool loop, uint audioType, bool isMutli) {
            //如果只能存在单个的音源 如BGM
            if (isMutli == false) {
                if (string.Equals(this.sAudioPath, audioPath, StringComparison.Ordinal)) {
                    return; // bgm不重新播放
                }
                else {
                    if (this.sAudioPath != null) {
                        // 第一次直接播放,否则需要fade
                        this.fadeTimer?.Cancel();
                        this.fadeTimer = Timer.Register(this.FADE_LENGTH * 2, null, (dt) => {
                            if (dt <= this.FADE_LENGTH) {
                                // 前半段 VOLUME - 0.1
                                float rate = dt / this.FADE_LENGTH;
                                this.fVolumeFade = Mathf.Lerp(VOLUME * DEFAULT_BGMVOLUE_SCALE, 0.1f, rate);

                                //fVolumeFade = dt / FADE_LENGTH * 0.9f + 0.1f;
                            }
                            else {
                                // 后半段  0.1 - VOLUME
                                if (!string.Equals(this.sAudioPath, audioPath, StringComparison.Ordinal)) {
                                    this.TryPlay(audioPath, loop, audioType, isMutli);
                                }

                                float rate = (dt - this.FADE_LENGTH) / this.FADE_LENGTH;
                                this.fVolumeFade = Mathf.Lerp(0.1f, VOLUME * DEFAULT_BGMVOLUE_SCALE, rate);

                                //fVolumeFade = (1 - (dt - FADE_LENGTH) / FADE_LENGTH) * 0.9f + 0.1f;
                            }

                            this.RefreshVolume();
                        });

                        return;
                    }
                }
            }

            this.TryPlay(audioPath, loop, audioType, isMutli);
        }

        private void TryPlay(string audioPath, bool loop, uint audioType, bool isMutli) {
            if (!string.Equals(this.sAudioPath, audioPath, StringComparison.Ordinal)) {
                if (this.mHandle.IsValid()) {
                    AddressablesUtil.Release<AudioClip>(ref this.mHandle, this.MHandle_Completed);
                }

                this.mAudioSource.clip = null;
            }
            else {
                if (isMutli == false) {
                    return; // bgm不重新播放
                }
            }

            this.sAudioPath = audioPath;
            this.eAudioType = audioType;
            this.isMutli = isMutli;
            this.mAudioSource.Stop();
            this.mAudioSource.loop = loop;

            this.RefreshVolume();

            if (this.mAudioSource.clip == null) {
                if (!this.mHandle.IsValid()) {
                    AddressablesUtil.LoadAssetAsync<AudioClip>(ref this.mHandle, this.sAudioPath, this.MHandle_Completed);
                }
            }
            else {
                this.DoPlay();
            }
        }

        private void MHandle_Completed(AsyncOperationHandle<AudioClip> handle) {
            if(handle.Status == AsyncOperationStatus.Succeeded) {
                this.mAudioSource.clip = handle.Result;
                this.DoPlay();
            }
        }

        private void DoPlay() {
            if (this.mAudioSource.clip != null) {
                // 规避异步加载完成的时候刚好在暂停的情况
                // stop和mute不需要担心
                if (!this.isPause) {
                    this.mAudioSource.Play();
                }

                if (!this.mAudioSource.loop) {
                    this.timer?.Cancel();
                    this.timer = Timer.Register(this.mAudioSource.clip.length, this.Stop);
                }
            }
            else {
                this.Stop();
            }
        }

        public void RefreshVolume() {
            this.mAudioSource.volume = this.fVolumeFade * AudioManager.Instance.GetVolume(this.eAudioType);
        }

        public void Pause(bool toPause) {
            this.isPause = toPause;
        }

        public void Stop() {
            this.sAudioPath = null;
            this.mAudioSource.Stop();
            this.mAudioSource.clip = null;

            this.timer?.Cancel();
            //this.fadeTimer?.Cancel();

            AddressablesUtil.Release<AudioClip>(ref this.mHandle, this.MHandle_Completed);

            if (this.isMutli) {
                AudioManager.Instance.Recovery(this);
            }
        }
    }

    public class AudioManager : TSingleton<AudioManager> {
        public const int gAudioMax = 32;

        private Transform mRoot;

        // Normal音源的音效管理
        public List<AudioEntry> mAudioEntries = new List<AudioEntry>(gAudioMax);
        public Queue<AudioEntry> mAudioEntriesPool = new Queue<AudioEntry>(gAudioMax);
        // 其他音源管理
        private readonly Dictionary<uint, AudioEntry> mAudioEntryMap = new Dictionary<uint, AudioEntry>();
        //音源音量管理
        private readonly Dictionary<uint, float> mAudioVolumeMap = new Dictionary<uint, float>();

        public CP_AudioCollector environmentAudioCollector;
        
        // ios平台环境音使用phase还是unity内置的
        public bool usePHASEInIOS = true;
        public bool use3dSound = true;
        
        public Transform phaseListener;
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
        public Apple.PHASE.PHASEEngine phaseEngine;
#endif

        public void Init() {
            this.mRoot = new GameObject("Root_Audio").transform;
            if (this.mRoot != null) {
                GameObject.DontDestroyOnLoad(this.mRoot);
                
                //TODO AudioListener 一般放置于相机上
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
                phaseEngine = mRoot.gameObject.AddComponent<Apple.PHASE.PHASEEngine>();
                mRoot.gameObject.AddComponent<ReplaceListener>();
                // phaseListener = TryCreatePHASEListener();
#endif
                
                var keeper = this.mRoot.gameObject.AddComponent<CP_AudioListenerKeeper>();
                if (keeper != null) {
                    keeper.SetDefault();
                }
                this.environmentAudioCollector = this.mRoot.gameObject.AddComponent<CP_AudioCollector>();
                this.environmentAudioCollector.onGetVolume = this.GetVolume;
#if UNITY_EDITOR
                this.mRoot.gameObject.AddComponent<AudioChecker>();
#endif
            }
        }

        public void UnInit() { }

        public Transform TryCreatePHASEListener() {
            if (phaseListener == null) {
                // 首次创建 或者 被销毁创建
                var proto = Resources.Load<GameObject>("PHASEListener");
                phaseListener = GameObject.Instantiate(proto, mRoot.gameObject.transform).transform;
                phaseListener.localPosition = Vector3.zero;
                proto = null;
            }

            return phaseListener;
        }

        public void SetPHASEParent(Transform parent) {
            DebugUtil.LogError($"ChangeParent: {parent}");
            if (phaseListener != null) {
                phaseListener.SetParent(parent, false);
                phaseListener.localPosition = Vector3.zero;
            }
        }

        public void SetPhaseDefaultParent() {
            SetPHASEParent(mRoot.gameObject.transform);
        }

        private AudioEntry CreateAudioEntry() {
            AudioEntry entry = null;
            if (this.mAudioEntriesPool.Count > 0) {
                entry = this.mAudioEntriesPool.Dequeue();
            }
            else {
                entry = new AudioEntry(this.mRoot.gameObject.AddComponent<AudioSource>());
            }
            return entry;
        }

        public void StopSingle(uint audioType) {
            if (this.mAudioEntryMap.TryGetValue(audioType, out var entry)) {
                entry.Stop();
            }
        }
        public void PauseSingle(uint audioType, bool toPause) {
            if (this.mAudioEntryMap.TryGetValue(audioType, out var entry)) {
                entry.Pause(toPause);
            }
        }
        // 返回值有可能为null
        public AudioEntry Play(string audioPath, uint audioType, bool isMulti, bool loop = false, float volumeScale = 1f) {
            if (string.IsNullOrEmpty(audioPath))
                return null;

            AudioEntry entry = null;
            if (isMulti) {
                if (this.mAudioEntries.Count < gAudioMax) {
                    entry = this.CreateAudioEntry();
                    this.mAudioEntries.Add(entry);
                }
            }
            else {
                if (!this.mAudioEntryMap.TryGetValue(audioType, out entry)) {
                    entry = this.CreateAudioEntry();
                    this.mAudioEntryMap[audioType] = entry;
                }
            }

            // 超过最大音源限制，为null
            if (entry != null) {
                entry.fVolumeFade = AudioEntry.VOLUME * volumeScale;
                entry.Play(audioPath, loop, audioType, isMulti);
            }

            return entry;
        }

        public void SetVolume(uint audioType, float volume) {
            if (this.mAudioVolumeMap.TryGetValue(audioType, out float rlt) && rlt == volume) {
                return;
            }

            this.mAudioVolumeMap[audioType] = volume;
            // Debug.LogError(audioType + "  " + volume);

            if (this.mAudioEntryMap.TryGetValue(audioType, out AudioEntry entry)) {
                entry.RefreshVolume();
            }
            else {
                for (int i = 0; i < this.mAudioEntries.Count; ++i) {
                    this.mAudioEntries[i].RefreshVolume();
                }
            }

            // 环境音因为不是AudioEntry管理的，所以只能单独管理
            // AudioUtil.EAudioType.SceneSound = 5
            if (audioType == 5 || audioType == 6) {
                float v = this.GetVolume(audioType);
                this.environmentAudioCollector?.SetVolume(audioType, v);

                if(audioType == 6)
                {
                    VideoManager.SetVolume(v);
                }
            }
        }

        public float GetVolume(uint audioType) {
            this.mAudioVolumeMap.TryGetValue(audioType, out float rlt);
            return rlt;
        }

        public void Pause(AudioEntry audioInfo, bool toPause) {
            if (audioInfo != null) {
                audioInfo.isPause = toPause;
            }
        }

        public void Stop(AudioEntry audioInfo) {
            audioInfo?.Stop();
        }

        public void StopAll() {
            foreach (var kvp in this.mAudioEntryMap) {
                kvp.Value.Stop();
            }
            for (int i = this.mAudioEntries.Count - 1; i >= 0; i--) {
                this.mAudioEntries[i].Stop();
            }
        }

        internal void Recovery(AudioEntry audioEntry) {
            int index = this.mAudioEntries.IndexOf(audioEntry);
            if (index >= 0) {
                this.mAudioEntries.RemoveAt(index);
                if (this.mAudioEntriesPool.Count < gAudioMax) {
                    this.mAudioEntriesPool.Enqueue(audioEntry);
                }
            }
            else {
                // Debug.LogError("回收音源出现问题");
                // Debug.Break();
            }
        }

        public void AudioSetting(int iSpeakerMode, int iDspBufferSize, int iSampleRate, int iNumRealVoices, int iNumVirtualVoices) {
            AudioSettings.Reset(new AudioConfiguration() {
                speakerMode = (AudioSpeakerMode)(iSpeakerMode),
                dspBufferSize = iDspBufferSize,
                sampleRate = iSampleRate,
                numRealVoices = iNumRealVoices,
                numVirtualVoices = iNumVirtualVoices,
            });
        }

        public void OnGUI(UnityEngine.Vector2 size, bool hideNormal) {
#if DEBUG_MODE
            foreach (var kvp in mAudioVolumeMap) {
                uint audioType = kvp.Key;
                float volume = kvp.Value;

                UnityEngine.GUILayout.BeginHorizontal();
                UnityEngine.GUILayout.Label(audioType.ToString(), UnityEngine.GUILayout.Width(size.x * 0.4f));
                UnityEngine.GUILayout.Label(volume.ToString(), UnityEngine.GUILayout.Width(size.x * 0.1f));
                UnityEngine.GUILayout.EndHorizontal();
            }
#endif
        }
    }
}