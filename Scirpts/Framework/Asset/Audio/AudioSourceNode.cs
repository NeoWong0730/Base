#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
using Apple.PHASE;
#endif

using UnityEngine;

namespace Framework
{
    [RequireComponent(typeof(AudioSource))]
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
    [RequireComponent(typeof(PHASESource))]
#endif
    [DisallowMultipleComponent]
    public class AudioSourceNode : MonoBehaviour
    {
        public uint audioType = 5;

        private AudioSource _audioSource;
        public AudioSource audioSource
        {
            get
            {
                _audioSource = gameObject.GetOrAddComponent<AudioSource>();
                return _audioSource;
            }
        }

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
        private PHASESource _phaseAudioSource;
        public PHASESource phaseAudioSource
        {
            get
            {
                _phaseAudioSource = gameObject.GetOrAddComponent<PHASESource>();
                return _phaseAudioSource;
            }
        }
#endif

        private AudioCollectorComponent _collector;
        public AudioCollectorComponent collector
        {
            get
            {
                if (_collector == null)
                    _collector = AudioManager.Instance.environmentAudioCollector;

                return _collector;
            }
        }

        private void Awake()
        {
            if (AudioManager.Instance.use3dSound)
            {
                if (audioSource != null)
                {
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
                    audioSource.enabled = !AudioManager.Instance.usePHASEInIOS;
                    phaseAudioSource.enabled = AudioManager.Instance.usePHASEInIOS;
#else
                    audioSource.enabled = true;
#endif
                }
            }
            else
            {
                if (audioSource != null)
                {
                    audioSource.enabled = false;
                }
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
                phaseAudioSource.enabled = false;
#endif
            }
        }

        private void Start()
        {
            if (collector != null && collector.onGetVoloume != null)
            {
                float volume = collector.onGetVoloume.Invoke(audioType);
                collector.SetVolume(audioType, volume);
            }
        }

        private void OnEnable()
        {
            collector?.Register(this, true);
        }

        private void OnDisable()
        {
            collector?.Register(this, false);
        }
    }
}