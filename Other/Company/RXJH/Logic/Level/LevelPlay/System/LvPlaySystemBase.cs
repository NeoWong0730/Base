using Logic.Core;

namespace Logic
{
    public class LvPlaySystemBase : LevelSystemBase
    {
        protected LvPlayData mData;
        public override void OnPreCreate()
        {
            mData = (mLevelBase as LvPlay).mLvPlayData;
        }
    }
}