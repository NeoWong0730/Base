using Logic.Core;

namespace Logic
{
    public abstract class PlayerMoveTo : MoveTo
    {
        protected override void SetActor()
        {
            LvPlay lvPlay = LevelManager.mMainLevel as LvPlay;
            if (lvPlay != null)
            {
                _actor = lvPlay.mLvPlayData.mMainActor;
            }
        }
    }
}
