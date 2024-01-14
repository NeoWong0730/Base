using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetQTESlide : TimelineAssetLifeCircle {
        public ExposedReference<TimelineDirector> pauseResumer;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourQTESlide>.Create(graph);
            TimelineBehaviourQTESlide t = (playerable.GetBehaviour() as TimelineBehaviourQTESlide);
            t.SetArg(arg);
            t.pauseResumer = pauseResumer.Resolve(graph.GetResolver());
            return playerable;
        }
    }
}
