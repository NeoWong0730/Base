using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetSetUIAlpha : TimelineAssetLifeCircle {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourSetUIAlpha>.Create(graph);

            TimelineBehaviourSetUIAlpha behaviour = (playerable.GetBehaviour() as TimelineBehaviourSetUIAlpha);
            behaviour.SetArg(arg);
            return playerable;
        }
    }
}
