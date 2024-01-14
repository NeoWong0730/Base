using Lib.Core;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework {
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelinePauseResume : MonoBehaviour {
        public PlayableDirector playableDirector;

        private void Awake() {
            if (playableDirector == null) {
                playableDirector = GetComponent<PlayableDirector>();
            }
        }
        public void Pause() {
            if (Application.isPlaying) {
                playableDirector.Pause();
            }
        }
        public void Resume() {
            if (Application.isPlaying) {
                playableDirector.Resume();
            }
        }
    }
}
