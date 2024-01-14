using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetSubtitleAudio : TimelineAssetLifeCircle {
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourSubtitleAudio>.Create(graph);
            (playerable.GetBehaviour() as TimelineBehaviourSubtitleAudio).SetArg(arg);
            return playerable;
        }
    }
}
