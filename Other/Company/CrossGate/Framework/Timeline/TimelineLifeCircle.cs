using System;
using UnityEngine.Playables;
using Lib.Core;
using UnityEngine;

namespace Framework
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimelineLifeCircle : MonoBehaviour
    {
        public static readonly Lib.Core.EventEmitter<ETimelineLifeCircle> eventEmitter = new EventEmitter<ETimelineLifeCircle>();
        public PlayableDirector playableDirector;

        private bool isCrossPlay;
        private bool isCrossPause;

        public Action<PlayableDirector> onPlay;
        public Action<PlayableDirector> onStop;

        public Action<PlayableDirector> onPause;
        public Action<PlayableDirector> onResume;

        private void Awake()
        {
            if (playableDirector == null)
            {
                playableDirector = GetComponent<PlayableDirector>();
            }

            RegistEvent(true);
        }
        private void RegistEvent(bool toRegiset) {
            if (playableDirector != null) {
                if (toRegiset) {
                    playableDirector.played += OnPlayed;
                    playableDirector.paused += OnPaused;
                    playableDirector.stopped += OnStoped;
                }
                else {
                    playableDirector.played -= OnPlayed;
                    playableDirector.paused -= OnPaused;
                    playableDirector.stopped -= OnStoped;
                }
            }
        }
        private void OnDestroy() {
            RegistEvent(false);
        }

        private void OnPlayed(PlayableDirector pd)
        {
            if (Application.isPlaying)
            {
                isCrossPlay = true;

                if (!isCrossPause) {
                    onPlay?.Invoke(playableDirector);
                    eventEmitter.Trigger(ETimelineLifeCircle.OnGraphStart);
                }
                else {
                    onResume?.Invoke(playableDirector);
                    eventEmitter.Trigger(ETimelineLifeCircle.OnGraphResume);
                }
            }
        }
        private void OnPaused(PlayableDirector pd)
        {
            if (Application.isPlaying) {
                if (!isCrossPlay) {
                    return;
                }

                isCrossPause = true;

                onPause?.Invoke(playableDirector);
                eventEmitter.Trigger(ETimelineLifeCircle.OnGraphPause);
            }
        }
        private void OnStoped(PlayableDirector pd)
        {
            if (Application.isPlaying) {
                if (!isCrossPlay) {
                    return;
                }

                isCrossPause = false;
                isCrossPlay = false;

                onStop?.Invoke(playableDirector);
                eventEmitter.Trigger(ETimelineLifeCircle.OnGraphStop);
            }
        }
    }
}
