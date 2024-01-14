using UnityEngine;
using UnityEngine.Playables;

namespace Framework {
    [System.Serializable]
    public class CutSceneArg {
        public string group;
        public string tag;
        [HideInInspector]
        public Transform transform;
        [Header("如果是字幕音效，表示字幕id；" +
            "如果是气泡，表示气泡id,如果是qte,表示开锁表的Id；" +
            "如果是打开UI,则是UIId；" +
            "如果是fadeinout,是fadetype")]
        public uint id;
        [Header("如果是气泡，表示气泡相对于trans的偏移")]
        public Vector3 offset;
        [Header("如果是fadeinout,表示fadetime")]
        public float value;
        [Header("如果是dissolve,表示开关")]
        public bool boolValue;
    }

    [System.Serializable]
    public class TimelineAssetHud : TimelineAssetLifeCircle {
        public ExposedReference<Transform> trans;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject go) {
            var playerable = ScriptPlayable<TimelineBehaviourHud>.Create(graph);

            TimelineBehaviourHud behaviour = (playerable.GetBehaviour() as TimelineBehaviourHud);
            behaviour.SetArg(this.arg);
            behaviour.arg.transform = this.trans.Resolve(graph.GetResolver());
            return playerable;
        }
    }
}
