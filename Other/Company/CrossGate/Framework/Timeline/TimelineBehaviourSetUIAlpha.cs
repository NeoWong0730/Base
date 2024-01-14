using UnityEngine.Playables;
using Lib.Core;
using UnityEngine;

namespace Framework
{
    public class TimelineBehaviourSetUIAlpha : TimelineBehaviourLifeCircle {
        public override void SetArg(CutSceneArg target) {
            this.arg.group = "CutScene";
            this.arg.tag = "SetUIAlpha";
            this.arg.transform = target.transform;
            this.arg.id = target.id;
            this.arg.offset = target.offset;
            this.arg.value = target.value;
        }
        
        public override void OnPlayableBehaviourPlay(Playable playable, FrameData info) {
            TimelineLifeCircle.eventEmitter.Trigger<CutSceneArg>(ETimelineLifeCircle.SetUIAlpha, arg);
        }
    }
}
