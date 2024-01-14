using Logic.Core;

namespace Logic
{
    public class PlayerMoveToTargetVirtualActorPosition : PlayerMoveTo
    {
        public uint targetVirtualActorUID;

        public override string OnDrawNodeText()
        {
            NodeData.Comment = "����ƶ��������ɫλ��";
            return base.OnDrawNodeText();
        }

        protected override void SetTargetPosition()
        {
            LvPlay lvPlay = LevelManager.mMainLevel as LvPlay;
            if (lvPlay != null)
            {
                Actor actor;
                if (lvPlay.mLvPlayData.mActors.TryGetValue(targetVirtualActorUID, out actor))
                {
                    _targetPosition = actor.mTransform.position;
                }
            }
        }
    }
}