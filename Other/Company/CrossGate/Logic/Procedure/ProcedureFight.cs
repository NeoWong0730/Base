using UnityEngine;
using Logic.Core;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 战斗流程///
    /// </summary>
    public class ProcedureFight : ProcedureBase
    {
        public override ProcedureManager.EProcedureType ProcedureType
        {
            get
            {
                return ProcedureManager.EProcedureType.Fight ;
            }
        }

        protected internal override void OnInit(IFsm procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected internal override void OnEnter(IFsm procedureOwner)
        {
            base.OnEnter(procedureOwner);

            //Debug.Log("OnEnter ProcedureFight");
            RegisterEvent((int)EProcedureEvent.EnterNormal, OnEnterNormal);
            RegisterEvent((int)EProcedureEvent.EnterInteractive, OnEnterInteractive);
            RegisterEvent((int)EProcedureEvent.EnterCutScene, OnEnterCutScene);

            OnEnterFight();
        }

        protected internal override void OnExit(IFsm procedureOwner, bool isShutdown)
        {
            base.OnExit(procedureOwner, isShutdown);

            //Debug.Log("OnExit ProcedureFight");
            UnRegisterEvent((int)EProcedureEvent.EnterNormal, OnEnterNormal);
            UnRegisterEvent((int)EProcedureEvent.EnterInteractive, OnEnterInteractive);
            UnRegisterEvent((int)EProcedureEvent.EnterCutScene, OnEnterCutScene);

            OnExitFight();
        }

        void OnEnterNormal(IFsm procedureOwner, object sender, object userData)
        {
            ChangeState<ProcedureNormal>(procedureOwner);
        }

        void OnEnterInteractive(IFsm procedureOwner, object sender, object userData)
        {
            ChangeState<ProcedureInteractive>(procedureOwner);
        }

        void OnEnterCutScene(IFsm procedureOwner, object sender, object userData)
        {
            ChangeState<ProcedureCutScene>(procedureOwner);
        }

        void OnExitFight()
        {
            if (GameCenter.mainHero != null && GameCenter.mainHero.Mount != null)
            {
                if (GameCenter.mainHero.stateComponent.CurrentState == EStateType.Idle)
                {
                    GameCenter.mainHero.Mount.stateComponent.ChangeState(EStateType.Stand);
                }
                else if (GameCenter.mainHero.stateComponent.CurrentState == EStateType.Walk || GameCenter.mainHero.stateComponent.CurrentState == EStateType.Run)
                {
                    GameCenter.mainHero.Mount.stateComponent.ChangeState(EStateType.Sprint);
                }
                else
                {
                    GameCenter.mainHero.Mount.stateComponent.ChangeState(GameCenter.mainHero.stateComponent.CurrentState);
                }

                if (GameCenter.mainHero.stateComponent.CurrentState == EStateType.Idle)
                {
                    if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                        GameCenter.mainHero.animationComponent?.CrossFade((uint)EStateType.mount_1_idle, Constants.CORSSFADETIME);
                    else
                        GameCenter.mainHero.animationComponent?.CrossFade((uint)EStateType.mount_2_idle, Constants.CORSSFADETIME);
                }
                else if (GameCenter.mainHero.stateComponent.CurrentState == EStateType.Run)
                {
                    if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                        GameCenter.mainHero.animationComponent?.CrossFade((uint)EStateType.mount_1_run, Constants.CORSSFADETIME);
                    else
                        GameCenter.mainHero.animationComponent?.CrossFade((uint)EStateType.mount_2_run, Constants.CORSSFADETIME);
                }
                else if (GameCenter.mainHero.stateComponent.CurrentState == EStateType.Walk)
                {
                    if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                        GameCenter.mainHero.animationComponent?.CrossFade((uint)EStateType.mount_1_walk, Constants.CORSSFADETIME);
                    else
                        GameCenter.mainHero.animationComponent?.CrossFade((uint)EStateType.mount_2_walk, Constants.CORSSFADETIME);
                }
            }
        }

        /// <summary>
        /// 进入战斗流程一定会触发的逻辑///
        /// </summary>
        void OnEnterFight()
        {
            //生产战斗控制器
            Sys_Fight.Instance.CreateControl();

            //HUD设置
            //Sys_HUD.Instance.Construct();
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnInActiveAllActorHUD);
            DebugUtil.LogFormat(ELogType.eHUD, "ProcedureFight.OnInActiveAllActorHUD");
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnClearNpcBubbles);

            //关闭遮挡透明效果 战斗中不需要
            RenderExtensionSetting.bUsageOcclusionTransparent = false;

            //UIManager.HideStackUI();
            ProcedureManager.eventEmitter.Trigger(ProcedureManager.EEvents.OnBeforeEnterFightEffect);

            CSVBattleType.Data cSVBattleTypeData = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            if (cSVBattleTypeData == null)
            {
                EnterEffectFunc();
            }
            else
            {
                if ((cSVBattleTypeData.enter_battle_effect == 0u || !UIManager.IsVisible(EUIID.UI_MainInterface)) &&
                    !Net_Combat.Instance.m_IsVideo)
                {
                    EnterEffectFunc();
                }
                else
                {
                    CSVParam.Data paramData = CSVParam.Instance.GetConfData(133);
                    if (paramData != null)
                    {
                        AudioUtil.PlayAudio(Convert.ToUInt32(paramData.str_value));
                    }
                    //相机设置
                    uint enterEffectType;
                    if (Net_Combat.Instance.m_IsVideo)
                        enterEffectType = OptionManager.Instance.GetBoolean((int)OptionManager.EOptionID.UsePcStyleEnterFight) ?
                                            1u : 3u;
                    else
                    {
                        enterEffectType = OptionManager.Instance.GetBoolean((int)OptionManager.EOptionID.UsePcStyleEnterFight) ?
                                            cSVBattleTypeData.enter_battle_effectON : cSVBattleTypeData.enter_battle_effectOFF;
                    }
                    GameCenter.mCameraController.FadeInOut(enterEffectType, EnterEffectFunc);
                }          
            }

            UIManager.CloseUI(EUIID.UI_NPC);
        }

        void EnterEffectFunc()
        {
            CSVBattleType.Data cSVBattleTypeData = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            if (cSVBattleTypeData != null) {
                AudioUtil.PlayAudio(cSVBattleTypeData.battle_bgm);
            }

            //Sys_Fight.Instance.eventEmitter.Trigger(Sys_Fight.EEvents.Reconnected);

            ProcedureManager.eventEmitter.Trigger(ProcedureManager.EEvents.OnAfterEnterFightEffect);
            //UIManager.OpenUI(EUIID.UI_MainBattle);
            DebugUtil.LogFormat(ELogType.eCombat, "UI_MainBattle is Open！！！");
        }
    }
}