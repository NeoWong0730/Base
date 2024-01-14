using UnityEngine.Playables;
using Lib.Core;
using UnityEngine;

namespace Framework
{
    public class TimelineBehaviourFadeInOut : TimelineBehaviourLifeCircle {
        public override void SetArg(CutSceneArg target) {
            this.arg.group = "CutScene";
            this.arg.tag = "Fadeinout";
            this.arg.transform = target.transform;
            this.arg.id = target.id;
            this.arg.offset = target.offset;
            this.arg.value = target.value;
        }
        
        public override void OnPlayableBehaviourPlay(Playable playable, FrameData info) {
            TimelineLifeCircle.eventEmitter.Trigger<CutSceneArg>(ETimelineLifeCircle.Fadeinout, arg);
        }
    }
}
