using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetFadeInOut : PlayableAsset {
        public CutSceneArg arg;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
            var playerable = ScriptPlayable<TimelineBehaviourFadeInOut>.Create(graph);
            TimelineBehaviourFadeInOut t = (playerable.GetBehaviour() as TimelineBehaviourFadeInOut);
            t.SetArg(arg);
            return playerable;
        }
    }
}
