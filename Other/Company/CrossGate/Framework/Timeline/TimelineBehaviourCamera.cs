using UnityEngine.Playables;
using Lib.Core;
using UnityEngine;
using UnityEngine;

namespace Framework
{
    public class TimelineBehaviourCamera : PlayableBehaviour {
        public LayerMask mask = -1;
        public Camera camera;

        public override void OnBehaviourPlay(Playable playable, FrameData info) {
            camera.cullingMask = mask.value;
        }
    }
}
