using UnityEngine;
using UnityEngine.Playables;

namespace Framework
{
    [System.Serializable]
    public class TimelineAssetCamera : PlayableAsset 
    {
        public LayerMask mask = -1;
        public ExposedReference<Camera> camera;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var playerable = ScriptPlayable<TimelineBehaviourCamera>.Create(graph);
            
            TimelineBehaviourCamera behaviour = (playerable.GetBehaviour() as TimelineBehaviourCamera);
            behaviour.camera = camera.Resolve(graph.GetResolver());
            behaviour.mask = mask;
            return playerable;
        }
    }
}
