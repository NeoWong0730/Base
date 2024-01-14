
namespace Framework
{
    public class TimelineBehaviourSubtitleAudio : TimelineBehaviourLifeCircle {
        public override void SetArg(CutSceneArg target) {
            this.arg.group = "CutScene";
            this.arg.tag = "PlaySubtitleAudio";
            this.arg.transform = target.transform;
            this.arg.id = target.id;
            this.arg.offset = target.offset;
            this.arg.value = target.value;
        }
    }
}
