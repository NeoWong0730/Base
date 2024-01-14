using UnityEngine;
using UnityEngine.Playables;

namespace Framework {
    [System.Serializable]
    public class TimelineAssetShakeCamera : TimelineAssetLifeCircle {
        public ExposedReference<Transform> trans;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
            var playerable = ScriptPlayable<TimelineBehaviourShakeCamera>.Create(graph);

            TimelineBehaviourShakeCamera behaviour = (playerable.GetBehaviour() as TimelineBehaviourShakeCamera);
            behaviour.SetArg(this.arg);
            behaviour.arg.transform = this.trans.Resolve(graph.GetResolver());

            return playerable;
        }
    }
}
