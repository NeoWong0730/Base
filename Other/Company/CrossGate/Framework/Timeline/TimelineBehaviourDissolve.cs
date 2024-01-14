using UnityEngine.Playables;
using Lib.Core;
using UnityEngine;

namespace Framework
{
    public class TimelineBehaviourDissolve : TimelineBehaviourLifeCircle {                                                                                                                              
        public override void SetArg(CutSceneArg target) {
            this.arg.group = "CutScene";
            this.arg.tag = "Dissolve";
            this.arg.transform = target.transform;
            this.arg.id = target.id;
            this.arg.offset = target.offset;
            this.arg.value = target.value;
            this.arg.boolValue = target.boolValue;
        }
    }
}
