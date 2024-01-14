using Framework;
using Logic.Core;

namespace Logic {
    public class AudioListenerComponent : Logic.Core.Component {
        public CP_AudioListenerKeeper listener;

        protected override void OnConstruct() {
            Listen(true);
        }

        private void Listen(bool toListen) {
            Sys_Map.Instance.eventEmitter.Handle<uint, uint>(Sys_Map.EEvents.OnChangeMap, this.OnChangeMap, toListen);
        }

        public void Set() {
            var sceneActor = this.actor as SceneActor;
            if (sceneActor != null && sceneActor.gameObject != null) {
                var listener = CP_AudioListenerKeeper.Get(sceneActor.gameObject);
                if (listener != null) {
                    listener.SetFinal(true);
                }

                
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
            sceneActor.gameObject.AddComponent<ReplaceListener>();
            // AudioManager.Instance.SetPHASEParent(sceneActor.transform);
            Apple.PHASE.PHASEEngine.EngineStart();
#endif
            }
        }
        protected override void OnDispose() {
            Listen(false);
            
            if (this.listener != null) {
                this.listener.SetFinal(false);
            }
            
            //AudioManager.Instance.SetPhaseDefaultParent();
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
            Apple.PHASE.PHASEEngine.EngineStop(false);
#endif
        }

        private void OnChangeMap(uint lastMapId, uint curMapId) {
            // apple团队给的建议：重新激活engine
#if UNITY_IPHONE || UNITY_IOS || UNITY_EDITOR_OSX
            Apple.PHASE.PHASEEngine.EngineStop(false);
            Apple.PHASE.PHASEEngine.EngineStart();
#endif
        }
    }
}
