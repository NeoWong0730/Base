using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetOpenUI : TimelineAssetLifeCircle {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourOpenUI>.Create(graph);

            TimelineBehaviourOpenUI behaviour = (playerable.GetBehaviour() as TimelineBehaviourOpenUI);
            behaviour.SetArg(arg);
            return playerable;
        }
    }
}
