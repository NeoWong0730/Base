using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class StateSystem : LevelSystemBase
    {
        public override void OnUpdate()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            Hero hero = GameCenter.mainHero;
            Excute(hero, hero.stateComponent, hero.animationComponent);
        }

        private void Excute(Hero hero, StateComponent stateComponent, AnimationComponent animationComponent)
        {
            if (stateComponent.CurrentState == EStateType.Collection)
                return;

            if (hero.Mount != null)
                return;

            ClientPet mount = Sys_Pet.Instance.GetMountPet();
            if (mount == null)
                return;

            hero.OnMount(mount.GetFollowPetInfo(), Sys_Pet.Instance.mountPetUid, Sys_Pet.Instance.GetMountPetSuitFashionId(), (uint)Sys_Pet.Instance.GetMountPerfectRemakeCount(), Sys_Pet.Instance.IsMountPetShowDemonSpiritFx(), false);
        }
    }
}