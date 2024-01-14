using System.Collections.Generic;
using UnityEngine;
using System;
using Lib;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Framework
{
    public class AudioManager : TSingleton<AudioManager>
    {
        public const int gAudtioMax = 32;

        private Transform mRoot;

        public List<AudioEntry> mAudioEntries = new List<AudioEntry>(gAudtioMax);
        public Queue<AudioEntry> mAudioEntriesPool = new Queue<AudioEntry>(gAudtioMax);

        private readonly Dictionary<uint, AudioEntry> mAudioEntryMap = new Dictionary<uint, AudioEntry>();

        private readonly Dictionary<uint, float> mAudioVolumeMap = new Dictionary<uint, float>();

        public AudioCollectorComponent environmentAudioCollector;

        public bool usePHASEInIOS = true;
        public bool use3dSound = true;

        public Transform phaseListener;
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
        public Apple.PHASE.PHASEEngine phaseEngine;
#endif

        public void Init()
        {
            mRoot = new GameObject("Root_Audio").transform;
            if (mRoot != null)
            {
                GameObject.DontDestroyOnLoad(mRoot);

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
                phaseEngine = mRoot.gameObject.AddComponent<Apple.PHASE.PHASEEngine>();
                mRoot.gameObject.AddComponent<ReplaceListener>();
#endif

                var keeper = mRoot.gameObject.AddComponent<AudioListenerKeeperComponent>();
                if (keeper != null)
                    keeper.SetDefault();

                environmentAudioCollector = mRoot.gameObject.AddComponent<AudioCollectorComponent>();
                environmentAudioCollector.onGetVoloume = GetVolume;
#if UNITY_EDITOR
                mRoot.gameObject.AddComponent<AudioChecker>();
            }
#endif
        }

        public void UnInit() { }

        public Transform TryCreatePHASEListener()
        {
            if (phaseListener == null)
            {
                var proto = Resources.Load<GameObject>("PHASEListener");
                phaseListener = GameObject.Instantiate(proto, mRoot.gameObject.transform).transform;
                phaseListener.localPosition = Vector3.zero;
                proto = null;
            }

            return phaseListener;
        }

        public void SetPHASEParent(Transform parent)
        {
            DebugUtil.LogError($"ChangerParent: {parent}");
            if (phaseListener != null)
            {
                phaseListener.SetParent(parent, false);
                phaseListener.localPosition = Vector3.zero;
            }
        }

        public void SetPhaseDefaultParent()
        {
            SetPHASEParent(mRoot.gameObject.transform);
        }

        private AudioEntry CreateAudioEntry()
        {
            AudioEntry entry = null;
            if (mAudioEntriesPool.Count > 0)
            {
                entry = mAudioEntriesPool.Dequeue();
            }
            else
            {
                entry = new AudioEntry(mRoot.gameObject.AddComponent<AudioSource>());
            }

            return entry;
        }

        public void StopSingle(uint audioType)
        {
            if (mAudioEntryMap.TryGetValue(audioType, out var entry))
            {
                entry.Stop();
            }
        }

        public void PauseSingle(uint audioType, bool toPause)
        {
            if (mAudioEntryMap.TryGetValue(audioType, out var entry))
            {
                entry.Pause(toPause);
            }
        }

        public AudioEntry Play(string audioPath, uint audioType, bool isMulti, bool loop = false, float volumeScale = 1f)
        {
            if (string.IsNullOrEmpty(audioPath))
                return null;

            AudioEntry entry = null;
            if (isMulti)
            {
                if (mAudioEntries.Count < gAudtioMax)
                {
                    entry = CreateAudioEntry();
                    mAudioEntries.Add(entry);
                }
            }
            else
            {
                if (!mAudioEntryMap.TryGetValue(audioType, out entry))
                {
                    entry = CreateAudioEntry();
                    mAudioEntryMap[audioType] = entry;
                }
            }

            if (entry != null)
            {
                entry.fVolumeFade = AudioEntry.VOLUME * volumeScale;
                entry.Play(audioPath, loop, audioType, isMulti);
            }

            return entry;
        }

        public void SetVolume(uint audioType, float volume)
        {
            if (mAudioVolumeMap.TryGetValue(audioType, out float rlt) && rlt == volume)
                return;

            mAudioVolumeMap[audioType] = volume;

            if (mAudioEntryMap.TryGetValue(audioType, out AudioEntry entry))
                entry.RefreshVolume();
            else
            {
                for (int i = 0; i < mAudioEntries.Count; ++i)
                {
                    mAudioEntries[i].RefreshVolume();
                }
            }

            if (audioType == 5 || audioType == 6)
            {
                float v = GetVolume(audioType);
                environmentAudioCollector?.SetVolume(audioType, v);

                if (audioType == 6)
                {
                    VideoManager.SetVolume(v);
                }
            }
        }

        public float GetVolume(uint audioType)
        {
            mAudioVolumeMap.TryGetValue(audioType, out float rlt);
            return rlt;
        }

        public void Pause(AudioEntry audioInfo, bool toPause)
        {
            if (audioInfo != null)
            {
                audioInfo.isPause = toPause;
            }
        }

        public void Stop(AudioEntry audioInfo)
        {
            audioInfo?.Stop();
        }

        public void StopAll()
        {
            foreach (var kvp in mAudioEntryMap)
            {
                kvp.Value.Stop();
            }
            for (int i = mAudioEntries.Count - 1; i >= 0; i--)
            {
                mAudioEntries[i].Stop();
            }
        }

        internal void Recovery(AudioEntry audioEntry)
        {
            int index = mAudioEntries.IndexOf(audioEntry);
            if (index >= 0)
            {
                mAudioEntries.RemoveAt(index);
                if (mAudioEntriesPool.Count < gAudtioMax)
                {
                    mAudioEntriesPool.Enqueue(audioEntry);
                }
            }
            else
            {

            }
        }

        public void AudioSetting(int iSpeakMode, int iDspBufferSize, int iSampleRate, int iNumRealVoices, int iNumVirtualVoices)
        {
            AudioSettings.Reset(new AudioConfiguration()
            {
                speakerMode = (AudioSpeakerMode)iSpeakMode,
                dspBufferSize = iDspBufferSize,
                sampleRate = iSampleRate,
                numRealVoices = iNumRealVoices,
                numVirtualVoices = iNumVirtualVoices,
            });
        }

        public void OnGUI(Vector2 size, bool hideNormal)
        {
#if DEBUG_MODE
            foreach (var kvp in mAudioVolumeMap)
            {
                uint audioType = kvp.Key;
                float volume = kvp.Value;

                GUILayout.BeginHorizontal();
                GUILayout.Label(audioType.ToString(), GUILayout.Width(size.x * 0.4f));
                GUILayout.Label(volume.ToString(), GUILayout.Width(size.x * 0.1f));
                GUILayout.EndHorizontal();
            }
#endif
        }
    }
}
