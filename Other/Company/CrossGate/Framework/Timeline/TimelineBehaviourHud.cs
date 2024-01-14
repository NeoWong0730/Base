using UnityEngine.Playables;
using Lib.Core;
using UnityEngine;

namespace Framework
{
    public class TimelineBehaviourHud : TimelineBehaviourLifeCircle {                                                                                                                              
        public override void SetArg(CutSceneArg target) {
            this.arg.group = "CutScene";
            this.arg.tag = "PlayHud";
            this.arg.transform = target.transform;
            this.arg.id = target.id;
            this.arg.offset = target.offset;
            this.arg.value = target.value;
        }
    }
}
