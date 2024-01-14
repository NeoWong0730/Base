using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetLifeCircle : PlayableAsset
    {
        public CutSceneArg arg;
        public ExposedReference<TimelineLifeCircle> timelineLifeCycle;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourLifeCircle>.Create(graph);
            TimelineBehaviourLifeCircle t = (playerable.GetBehaviour() as TimelineBehaviourLifeCircle);
            t.SetArg(arg);
            t.timelineLifeCycle = timelineLifeCycle.Resolve(graph.GetResolver());
            return playerable;
        }
    }
}
