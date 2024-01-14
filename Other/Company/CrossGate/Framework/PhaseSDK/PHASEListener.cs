using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Apple.PHASE {
    /// <summary>
    /// Represents a listener in the PHASE engine.
    /// </summary>
    /// <remarks> Only one listener can exist at a time. </remarks>
    [DisallowMultipleComponent]
    public class PHASEListener : MonoBehaviour {
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
        public static PHASEListener instance;
        
        // Transform to store.
        private Transform _transform;

        // List of mixers.
        private List<PHASEMixer> _mixers = new List<PHASEMixer>();

        /// <summary>
        /// Global default reverb setting.
        /// </summary>
        [SerializeField] private Helpers.ReverbPresets _reverbPreset = Helpers.ReverbPresets.MediumRoom;

        private Helpers.ReverbPresets _lastReverb;

        // Is this listener registered with PHASE?
        private bool _registered = false;
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
        // Awake is called before the scene starts.
        void Awake()
        {
            if (!SDKManager.GetiOSPhaseCanPlay())
            {
                enabled = false;
                return;
            }

            instance = this;

            CreateListener();

            // Store the transform object (transform itself gets updated).
            _transform = GetComponent<Transform>();

            SetupReverb();
        }

        void CreateListener()
        {
            // Create the PHASE Listener.
            _registered = Helpers.PHASECreateListener();
            if (_registered == false)
            {
                Debug.LogError("Failed to create PHASE Listener");
            }
        }

        // Update is called once per frame.
        void LateUpdate()
        {
            if (_registered)
            {
                if (_transform != null)
                {
                    // Transpose position to a row matrix and convert the matrix to right-handed.
                    Matrix4x4 phaseTransform = Helpers.GetPhaseTransform(_transform);
                    bool result = Helpers.PHASESetListenerTransform(phaseTransform);
                    if (result == false)
                    {
                        Debug.LogError("Failed to set transform on listener");
                    }
                }
            }
        }

        // Stop is called when the object stops.
        void OnDestroy()
        {
            if (!SDKManager.GetiOSPhaseCanPlay())
            {
                return;
            }

            if (_registered)
            {
                bool result = Helpers.PHASEDestroyListener();
                if (result == false)
                {
                    Debug.LogError("Failed to destroy PHASE Listener");
                }
                else
                {
                    _registered = false;
                }
            }

            instance = null;
        }

        /// <summary>
        /// Give the listener access to a list of mixers.
        /// </summary>
        /// <param name="mixers"> A <c>List</c> of <c>PHASEMixer</c>es.</param>
        /// <remarks> This method is used for listener directivity visualization. </remarks>
        public void AddMixers(List<PHASEMixer> mixers)
        {
            if (mixers != null)
            {
                _mixers.AddRange(mixers);
            }
        }

        private void OnDisable()
        {
            if (!SDKManager.GetiOSPhaseCanPlay())
            {
                return;
            }

            OnDestroy();
        }

        private void OnEnable()
        {
            if (!SDKManager.GetiOSPhaseCanPlay())
            {
                return;
            }

            if (_registered == false)
            {
                CreateListener();
            }
        }

        void SetupReverb()
        {
            // Set the reverb preset in the scene.
            _lastReverb = _reverbPreset;
            Helpers.PHASESetSceneReverbPreset((int)_reverbPreset);
        }

        /// <summary>
        /// Set the global default reverb to the given preset.
        /// </summary>
        /// <param name="preset"> The value of the new global reverb preset. </param>
        public void SetReverbPreset(Helpers.ReverbPresets preset)
        {
            _reverbPreset = preset;
            if (_lastReverb != _reverbPreset)
            {
                _lastReverb = _reverbPreset;
                Helpers.PHASESetSceneReverbPreset((int)_reverbPreset);
            }
        }
#endif
#endif
    }
}