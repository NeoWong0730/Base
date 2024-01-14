using UnityEngine;
using System.Collections.Generic;

namespace Apple.PHASE
{
    [DisallowMultipleComponent]
    public class PHASEEngine : MonoBehaviour
    {
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
        private void Awake() {
            if (!SDKManager.GetiOSPhaseCanPlay())
            {
                enabled = false;
                return;
            }

            EngineStart();
        }
        
        private void OnDestroy() {
            EngineStop();
        }

        private void LateUpdate() {
            PHASESource.UpdateSources();
            Helpers.PHASEUpdate();
        }
        
        public static bool IsEngineActive = false;

        public static void EngineStart() {
            if (!SDKManager.GetiOSPhaseCanPlay())
            {
                return;
            }
            if (IsEngineActive) {
                return;
            }

            IsEngineActive = Helpers.PHASEStart();
            if (IsEngineActive == false) {
                Debug.LogError("Failed to start PHASE Engine");
            }
        }

        public static void EngineStop(bool clearSoundEvents = true) {
            if (!SDKManager.GetiOSPhaseCanPlay())
            {
                return;
            }
            if (clearSoundEvents) {
                PHASESoundEventNodeGraph.UnregisterAll();
            }
            
            Helpers.PHASEStop();
            IsEngineActive = false;
        }
#endif
    }
}
