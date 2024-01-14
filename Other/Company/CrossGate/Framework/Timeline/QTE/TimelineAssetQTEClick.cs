using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetQTEClick : TimelineAssetLifeCircle {
        public ExposedReference<TimelineDirector> pauseResumer;
        public ExposedReference<Transform> trans;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourQTEClick>.Create(graph);
            TimelineBehaviourQTEClick t = (playerable.GetBehaviour() as TimelineBehaviourQTEClick);
            t.SetArg(arg);
            t.arg.transform = this.trans.Resolve(graph.GetResolver());
            t.pauseResumer = pauseResumer.Resolve(graph.GetResolver());
            return playerable;
        }
    }
}
