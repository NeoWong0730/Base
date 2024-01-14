using Logic.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    public class PetControlSystem : LevelSystemBase
    {
        float lastTriggerTime = 0;
        float cd = 5f;

        public override void OnUpdate()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            if (Time.unscaledTime < lastTriggerTime)
                return;

            lastTriggerTime = Time.unscaledTime + cd;

            PetShow(GameCenter.mainHero);
            PetFollow(GameCenter.mainHero);
            for (int index = 0, len = GameCenter.otherActorList.Count; index < len; index++)
            {
                PetShow(GameCenter.otherActorList[index]);
                PetFollow(GameCenter.otherActorList[index]);
            }          
        }

        void PetShow(Hero hero)
        {
            if (hero == null)
                return;

            if (hero.Pet == null)
                return;

            if (hero.Pet.movementComponent == null)
                return;

            if (hero.Pet.movementComponent.mNavMeshAgent == null)
                return;

            hero.Pet.movementComponent.mNavMeshAgent.enabled = false;
            hero.Pet.movementComponent.mNavMeshAgent.enabled = true;

            if (Vector3.Distance(hero.Pet.transform.position, hero.Pet.HandlerHero.transform.position) > 20)
            {
                hero.Pet.movementComponent.TransformToPosImmediately(hero.Pet.HandlerHero.transform.position);
            }
        }

        void PetFollow(Hero hero)
        {
            if (hero == null)
                return;

            if (hero.Pet == null)
                return;

            if (hero.Pet.stateComponent.CurrentState == EStateType.Idle)
            {
                hero.Pet.petShowInMainWorldComponent.count++;
            }
            else
            {
                hero.Pet.petShowInMainWorldComponent.count = 0;
            }

            if (hero.Pet.petShowInMainWorldComponent.count >= 2)
            {
                hero.Pet.animationComponent.Play("action_collection2", () =>
                {
                    hero.Pet.animationComponent.Play("action_idle");
                    hero.Pet.petShowInMainWorldComponent.count = 1;
                });
            }
        }
    }

    public class PetShowInMainWorldComponent
    {
        public Pet Pet
        {
            get;
            set;
        }
        
        public int count = 0;

        public void Dispose()
        {
            Pet = null;
            count = 0;
        }

        public void Update()
        {

            if (Pet.stateComponent.CurrentState == EStateType.Idle)
            {
                count++;
            }
            else
            {
                count = 0;
            }

            if (count >= 2)
            {
                Pet.animationComponent.Play("action_collection2", () =>
                {
                    Pet.animationComponent.Play("action_idle");
                    count = 1;
                });
            }
        }

        protected virtual bool VaildCheck()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return false;

            return true;
        }
    }
}
