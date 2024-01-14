using Logic.Core;
using System;
using UnityEngine;

namespace Logic
{
    public class StateComponent : Logic.Core.Component
    {
        public Action<EStateType, EStateType> StateChange;

        Hero Hero;

        public EStateType CurrentState
        {
            get;
            set;
        }

        protected override void OnConstruct()
        {
            CurrentState = EStateType.Idle;

            Hero = actor as Hero;
        }

        protected override void OnDispose()
        {
            Hero = null;

            base.OnDispose();
        }

        protected virtual bool VaildCheck()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return false;

            return true;
        }

        public void ChangeState(EStateType newState)
        {
            if (CurrentState == newState)
                return;

            StateChange?.Invoke(CurrentState, newState);

            if (Hero != null && Hero.Mount != null)
            {
                if (newState == EStateType.Idle)
                {
                    Hero.Mount.stateComponent?.ChangeState(EStateType.Stand);
                }
                else if (newState == EStateType.Walk || newState == EStateType.Run)
                {
                    Hero.Mount.stateComponent?.ChangeState(EStateType.Sprint);
                }
                else
                {
                    Hero.Mount.stateComponent?.ChangeState(CurrentState);
                }
            }

            CurrentState = newState;
        }
    }
}