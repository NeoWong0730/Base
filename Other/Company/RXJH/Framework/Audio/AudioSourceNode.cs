#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
using Apple.PHASE;
#endif

using Framework;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
[RequireComponent(typeof(PHASESource))]
#endif
[DisallowMultipleComponent]
public class AudioSourceNode : MonoBehaviour {
    public uint audioType = 5;
    private AudioSource _ausioSource;

    public AudioSource audioSource {
        get {
            if (this._ausioSource == null) {
                if (!this.gameObject.TryGetComponent(out this._ausioSource)) {
                    this._ausioSource = this.gameObject.AddComponent<AudioSource>();
                }
            }

            return this._ausioSource;
        }
    }

#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
    private PHASESource _phaseAusioSource;

    public PHASESource phaseAudioSource {
        get {
            if (this._phaseAusioSource == null) {
                if (!this.gameObject.TryGetComponent(out this._phaseAusioSource)) {
                    this._phaseAusioSource = this.gameObject.AddComponent<PHASESource>();
                }
            }

            return this._phaseAusioSource;
        }
    }
#endif

    private CP_AudioCollector _collector;

    public CP_AudioCollector collector {
        get {
            if (this._collector == null) {
                this._collector = AudioManager.Instance.environmentAudioCollector;
            }

            return this._collector;
        }
    }
    
    private void Awake() {
        if (AudioManager.Instance.use3dSound) {
            if (audioSource != null) {
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
            audioSource.enabled = !AudioManager.Instance.usePHASEInIOS;
            phaseAudioSource.enabled = AudioManager.Instance.usePHASEInIOS;
#else
			audioSource.enabled = true;
#endif
            }
        }
        else {
            if (audioSource != null) {
                audioSource.enabled = false;
            }
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
            phaseAudioSource.enabled = false;
#endif
        }
    }

    private void Start() {
        if (this.collector != null && this.collector.onGetVolume != null) {
            float volume = this.collector.onGetVolume.Invoke(audioType);
            this.collector.SetVolume(audioType, volume);
        }
    }

    private void OnEnable() {
        if (this.collector != null) {
            this.collector.Register(this, true);
        }
    }

    private void OnDisable() {
        if (this.collector != null) {
            this.collector.Register(this, false);
        }
    }
}