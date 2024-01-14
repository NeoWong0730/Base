using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetQTELongPress : TimelineAssetLifeCircle {
        public ExposedReference<TimelineDirector> pauseResumer;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourQTELongPress>.Create(graph);
            TimelineBehaviourQTELongPress t = (playerable.GetBehaviour() as TimelineBehaviourQTELongPress);
            t.SetArg(arg);
            t.pauseResumer = pauseResumer.Resolve(graph.GetResolver());
            return playerable;
        }
    }
}
