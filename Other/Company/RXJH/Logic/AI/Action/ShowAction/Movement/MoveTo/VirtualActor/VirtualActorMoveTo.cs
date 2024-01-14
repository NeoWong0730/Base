using Logic.Core;

namespace Logic
{
    public abstract class VirtualActorMoveTo : MoveTo
    {
        public uint virtualActorUID;

        protected override void SetActor()
        {
            LvPlay lvPlay = LevelManager.mMainLevel as LvPlay;
            if (lvPlay != null)
            {
                lvPlay.mLvPlayData.mActors.TryGetValue(virtualActorUID, out _actor);
            }
        }
    }    
}