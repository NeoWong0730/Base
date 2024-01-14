using UnityEngine.Playables;
using UnityEngine;

namespace Framework
{
    public class TimelineBehaviourLifeCircle : PlayableBehaviour
    {
        public CutSceneArg arg = new CutSceneArg();

        public TimelineLifeCircle timelineLifeCycle;
        private bool hasRegisted = false;

        private bool isCrossPlay;
        private bool isCrossPause;

        public virtual void SetArg(CutSceneArg target) {
            this.arg.group = "CutScene";
            this.arg.tag = "LifeCycle";
            this.arg.transform = target.transform;
            this.arg.id = target.id;
            this.arg.offset = target.offset;
            this.arg.value = target.value;
        }

        private void TryRegist() {
            if (!hasRegisted) { 
                hasRegisted = true;
                if (timelineLifeCycle != null) {
                    timelineLifeCycle.onPlay += (director) => {
                        isCrossPlay = false;
                        isCrossPause = false;
                    };
                }
            }
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info) {
            if (Application.isPlaying) {
                TryRegist();
                isCrossPlay = true;

                if (!isCrossPause) {
                    OnPlayableBehaviourPlay(playable, info);
                }
                else {
                    OnPlayableBehaviourResume(playable, info);
                }
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info) {
            if (Application.isPlaying) {
                if (!isCrossPlay) {
                    return;
                }

                isCrossPause = true;
                OnPlayableBehaviourPause(playable, info);
            }
        }

        public virtual void OnPlayableBehaviourPlay(Playable playable, FrameData info) {
            TimelineLifeCircle.eventEmitter.Trigger<CutSceneArg>(ETimelineLifeCircle.OnBehaviourPlay, arg);
        }
        public virtual void OnPlayableBehaviourResume(Playable playable, FrameData info) {
            TimelineLifeCircle.eventEmitter.Trigger<CutSceneArg>(ETimelineLifeCircle.OnBehaviourResume, arg);
        }
        public virtual void OnPlayableBehaviourPause(Playable playable, FrameData info) {
            TimelineLifeCircle.eventEmitter.Trigger<CutSceneArg>(ETimelineLifeCircle.OnBehaviourPause, arg);
        }
    }
}
