using UnityEngine;

namespace Framework
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioListener))]
    public class AudioListenerKeeperComponent : MonoBehaviour
    {
        private static AudioListener _finalListener = null;

        private static AudioListener _defaultListener;

        private AudioListener _listener;

        public AudioListener listener
        {
            get
            {
                if (_listener == null)
                    _listener = GetComponent<AudioListener>();

                return _listener;
            }
        }

        public static AudioListenerKeeperComponent Get(GameObject go)
        {
            AudioListenerKeeperComponent keeper = go.GetOrAddComponent<AudioListenerKeeperComponent>();
            return keeper;
        }

        private void Awake()
        {
            if (listener != null)
                listener.enabled = false;
        }

        public void SetDefault()
        {
            if (_defaultListener != null)
                _defaultListener.enabled = false;

            _defaultListener = listener;
            _defaultListener.enabled = true;
        }

        public void SetFinal(bool toSet)
        {
            if (toSet)
            {
                if (_finalListener != null)
                    _finalListener.enabled = false;

                _finalListener = listener;

                if (_finalListener != null && _finalListener != _defaultListener)
                {
                    _finalListener.enabled = true;

                    if (_defaultListener != null)
                    {
                        _finalListener.enabled = false;
                    }
                }
            }
            else
            {
                if (_finalListener != null)
                    _finalListener.enabled = false;

                _finalListener = null;
                if (_defaultListener != null)
                    _defaultListener.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (_finalListener == listener && _finalListener != _defaultListener)
            {
                SetFinal(false);
            }
        }
    }
}
