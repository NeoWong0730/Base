using UnityEngine.Playables;
using Lib.Core;
using UnityEngine;

namespace Framework
{
    public class TimelineBehaviourShakeCamera : TimelineBehaviourLifeCircle {                                                                                                                              
        public override void SetArg(CutSceneArg target) {
            this.arg.group = "CutScene";
            this.arg.tag = "ShakeCamera";
            this.arg.id = target.id;
        }
    }
}
