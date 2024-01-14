using UnityEngine;
using UnityEngine.Playables;

namespace Framework {
    [System.Serializable]
    public class TimelineAssetDissolve : TimelineAssetLifeCircle {
        public ExposedReference<Transform> trans;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
            var playerable = ScriptPlayable<TimelineBehaviourDissolve>.Create(graph);

            TimelineBehaviourDissolve behaviour = (playerable.GetBehaviour() as TimelineBehaviourDissolve);
            behaviour.SetArg(this.arg);
            behaviour.arg.transform = this.trans.Resolve(graph.GetResolver());
            return playerable;
        }
    }
}
