using UnityEngine;

// 每个audiolistener上挂载一个，确保至少并且只有一个audiolistener
[DisallowMultipleComponent]
[RequireComponent(typeof(AudioListener))]
public class CP_AudioListenerKeeper : MonoBehaviour {
    private static AudioListener _finalListener = null;

    // default必须不能被 destroy
    private static AudioListener _defaultListener;

    private AudioListener _listener;

    public AudioListener listener {
        get {
            if (this._listener == null) {
                this._listener = this.GetComponent<AudioListener>();
            }

            return this._listener;
        }
    }
    
    public static CP_AudioListenerKeeper Get(GameObject go) {
        if (!go.TryGetComponent(out CP_AudioListenerKeeper keeper)) {
            keeper = go.AddComponent<CP_AudioListenerKeeper>();
        }

        return keeper;
    }

    private void Awake() {

        if (this.listener != null) {
            this.listener.enabled = false;
        }
    }

    // 相机上的AudioListener作为默认的
    public void SetDefault() {
        if (_defaultListener != null) {
            _defaultListener.enabled = false;
        }

        _defaultListener = this.listener;
        _defaultListener.enabled = true;
    }

    // Player上的AudioListener作为最终的
    public void SetFinal(bool toSet) {
        if (toSet) {
            if (_finalListener != null) {
                _finalListener.enabled = false;
            }

            _finalListener = this.listener;

            if (_finalListener != null && _finalListener != _defaultListener) {
                _finalListener.enabled = true;

                if (_defaultListener != null) {
                    _defaultListener.enabled = false;
                }
            }
        }
        else {
            if (_finalListener != null) {
                _finalListener.enabled = false;
            }

            _finalListener = null;
            if (_defaultListener != null) {
                _defaultListener.enabled = true;
            }
        }
    }
    
    // 防止手动在inspector上操作enable导致
    private void OnDisable() {
        if (_finalListener == this.listener && _finalListener != _defaultListener) {
            SetFinal(false);
        }
    }
}