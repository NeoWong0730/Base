using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public class FollowSystem : LevelSystemBase
    {
        public override void OnUpdate()
        {
            Hero hero;

            if (GameCenter.mainHero != null)
            {
                hero = GameCenter.mainHero;
                Excute(hero.followComponent);

                if (hero.Pet != null)
                {
                    Excute(hero.Pet.followComponent);
                }
            }

            for (int i = 0, len = GameCenter.otherActorList.Count; i < len; ++i)
            {
                hero = GameCenter.otherActorList[i];
                Excute(hero.followComponent);

                if (hero.Pet != null)
                {
                    Excute(hero.Pet.followComponent);
                }
            }

            foreach (var vNpc in Sys_Escort.Instance.escortVirtualNpcs.Values)
            {
                if (vNpc.followComponent != null)
                {
                    Excute(vNpc.followComponent);
                }
            }

            foreach (var vNpc in Sys_Track.Instance.trackVirtualNpcs.Values)
            {
                if (vNpc.followComponent != null)
                {
                    Excute(vNpc.followComponent);
                }
            }

            foreach (var vNpc in Sys_NpcFollow.Instance.npcFollowVirtualNpcs.Values)
            {
                if (vNpc.followComponent != null)
                {
                    Excute(vNpc.followComponent);
                }
            }
        }

        private void Excute(FollowComponent followComponent)
        {
            followComponent.Update();
        }
    }
}