using Logic.Core;
using Table;

namespace Logic
{
    public class RoleActionSystem : LevelSystemBase
    {
        public override void OnUpdate()
        {
            if (GameCenter.mainHero != null)
            {
                Excute(GameCenter.mainHero.roleActionComponent);
            }

            for (int i = 0, len = GameCenter.otherActorList.Count; i < len; ++i)
            {
                Excute(GameCenter.otherActorList[i].roleActionComponent);
            }
        }

        private void Excute(RoleActionComponent roleActionComponent)
        {
            roleActionComponent.Update();
        }
    }
}