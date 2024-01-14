using Lib.Core;
using Logic.Core;
using System;
using Table;

namespace Logic
{
    public class UI_MainInterface : UIBase
    {
        private Timer timerDelayBattleUI;
        private  float delayTime;
        private bool isDelayBattleUI=false;
        protected override void ProcessEvents(bool toRegister)
        {
            ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnEnterNormal, OnEnterNormal, toRegister);
            ProcedureManager.eventEmitter.Handle(ProcedureManager.EEvents.OnBeforeEnterFightEffect, OnAfterEnterFightEffect, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, bool>(UIStack.EUIStackEvent.ReadyHideMainCameraChange, OnReadyHideMainCameraChange, toRegister);
        }

        private void OnEnterNormal()
        {
            Refresh(false);
        }

        private void OnReadyHideMainCameraChange(uint stackID, bool arg2)
        {
            if(stackID==(uint)EUIID.UI_JSBattle&& GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                return;
            }
            Refresh(false);
        }

        private void OnAfterEnterFightEffect()
        {
            Refresh(true);
        }

        //isEnterFight 临时处理 区分刚进入战斗的刷新和战斗内的刷新
        private void Refresh(bool isEnterFight)
        {
            if (isOpen)
            {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight || UIManager.ReadyHideMainCamera())
                {    
                    timerDelayBattleUI?.Cancel();
                    isDelayBattleUI = false;
                    UIManager.CloseUI(EUIID.UI_MainBattle, true, false);
                }
                else
                {
                    float.TryParse(CSVParam.Instance.GetConfData(136).str_value, out delayTime);
                    if (isEnterFight)
                    {
                        isDelayBattleUI = true;
                        timerDelayBattleUI?.Cancel();
                        timerDelayBattleUI = Timer.Register(delayTime / 1000, () =>
                        {
                            UIManager.OpenUI(EUIID.UI_MainBattle, false);
                            timerDelayBattleUI.Cancel();
                            isDelayBattleUI = false;
                        }, null, false, true);
                    }
                    else
                    {
                        if (!isDelayBattleUI)
                        {
                            UIManager.OpenUI(EUIID.UI_MainBattle, false);
                        }
                    }
                }

                if (isVisibleAndOpen)
                {
                    //UIManager.OpenUI(EUIID.HUD);
                    Sys_HUD.Instance.OpenHud();

                    if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                    {
                        //                UIManager.CloseUI(EUIID.HUD);
                        UIManager.CloseUI(EUIID.UI_Joystick, true);
                        UIManager.CloseUI(EUIID.UI_Menu, false,false);
                        UIManager.CloseUI(EUIID.UI_FamilyResBattleTop, false);
                        UIManager.CloseUI(EUIID.UI_UseItem, false);
                        //UIManager.CloseUI(EUIID.UI_HornHUD, false);
                        UIManager.CloseUI(EUIID.UI_Mine_Result, true);
                        //UIManager.OpenUI(EUIID.UI_MainBattle);
                        UIManager.CloseUI(EUIID.UI_MainMenu);
                    }
                    else
                    {
                        UIManager.CloseUI(EUIID.UI_FunctionMenu, true);
                        UIManager.CloseUI(EUIID.UI_Element, true);
                        UIManager.CloseUI(EUIID.UI_MainBattle_Skills, true);
                        UIManager.CloseUI(EUIID.UI_MainBattle_Good, true,false);
                        UIManager.CloseUI(EUIID.UI_MainBattle_Pet, true);
                        UIManager.CloseUI(EUIID.UI_MainBattle_PetDetail, true);
                        UIManager.CloseUI(EUIID.UI_MainBattle_SealItem, true);
                        UIManager.CloseUI(EUIID.UI_MainBattle_SparSkills, true);
                        UIManager.CloseUI(EUIID.UI_Buff, true);
                        UIManager.CloseUI(EUIID.UI_Battle_Explain, true);
                        UIManager.CloseUI(EUIID.UI_Sequence, true);
                        UIManager.CloseUI(EUIID.UI_TrialGate_StageTip, true);
                        UIManager.CloseUI(EUIID.UI_Battle_Decide, true);
                        
                        UIManager.OpenUI(EUIID.UI_Joystick, false, null, EUIID.UI_MainInterface);
                        UIManager.OpenUI(EUIID.UI_Menu, false, null, EUIID.UI_MainInterface);
                        UIManager.OpenUI(EUIID.UI_UseItem, false, null, EUIID.UI_MainInterface);


                        if (Sys_Team.Instance.canManualOperate == false)
                        {
                            UIManager.OpenUI(EUIID.UI_Teaming);
                        }
                        //UIManager.OpenUI(EUIID.UI_HornHUD, false, null, EUIID.UI_MainInterface);

                        // 家族资源战的时候，打开特定UI
                        if (Sys_FamilyResBattle.Instance.InFamilyBattle &&
                            (Sys_FamilyResBattle.Instance.stage == Sys_FamilyResBattle.EStage.ReadyBattle || Sys_FamilyResBattle.Instance.stage == Sys_FamilyResBattle.EStage.Battle)) {
                            UIManager.OpenUI(EUIID.UI_FamilyResBattleTop, false, null, EUIID.UI_MainInterface);
                        }
                        else {
                            UIManager.CloseUI(EUIID.UI_FamilyResBattleTop);
                        }
                    }

                    if (UIManager.IsOpen(EUIID.UI_Chat))
                    {
                        //拉到最上层
                        UIManager.OpenUI(EUIID.UI_Chat, false, null, EUIID.UI_MainInterface);

                        //if (UIManager.IsOpen(EUIID.UI_ChatInput))
                        //{
                        //    UIManager.OpenUI(EUIID.UI_ChatInput, false, null, EUIID.UI_Chat);
                        //}
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_ChatSimplify, false, null, EUIID.UI_MainInterface);
                    }

#if GM_PROPAGATE_VERSION && UNITY_STANDALONE_WIN
                    if (Sys_Chat.Instance.isActionHideUI)
                    {
                        UIManager.CloseUI(EUIID.UI_Menu, false, false);
                        UIManager.CloseUI(EUIID.UI_ChatSimplify);
                        Sys_Input.Instance.SetEnableJoystick();
                    }
#endif
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_Qa))
                    //{
                    //    UIManager.CloseUI(EUIID.UI_Qa);
                    //}
                    //
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_ExploreReward))
                    //{
                    //    UIManager.CloseUI(EUIID.UI_ExploreReward);
                    //}
                    //
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_Multi_Info))
                    //{
                    //    UIManager.OpenUI(EUIID.UI_Multi_Info);
                    //}
                    //
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_Message_Box))
                    //{
                    //    UIManager.CloseUI(EUIID.UI_Message_Box);
                    //}
                    //
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_Pet_AddPoint))
                    //{
                    //    UIManager.CloseUI(EUIID.UI_Pet_AddPoint);
                    //}
                    //
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_ScreenLock))
                    //{
                    //    UIManager.OpenUI(EUIID.UI_ScreenLock);
                    //}
                    //
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_TalentExchange))
					//{
                    //    UIManager.OpenUI(EUIID.UI_TalentExchange);
                    //}
                    //
                    ////临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_MapCondition_Tips))
                    //{
                    //    UIManager.OpenUI(EUIID.UI_MapCondition_Tips);
                    //}

                    //临时处理，其他界面层级提高导致后续界面层级变低，无法点击处理
                    //if (UIManager.IsOpen(EUIID.UI_Reputation))
                    //{
                    //    UIManager.CloseUI(EUIID.UI_Reputation);
                    //}
                }
            }
        }

        protected override void OnShow()
        {
            Refresh(false);

            bool hasGotUserPartitionReward = Sys_UserPartition.Instance.CheckUserPartitionGiftIsGet();
            if (!hasGotUserPartitionReward) {
                bool needShowUserPartition = false;
                CSVParam.Data csv = CSVParam.Instance.GetConfData(951u);
                needShowUserPartition = (csv != null && !csv.str_value.Equals("0", StringComparison.CurrentCultureIgnoreCase));
                if (needShowUserPartition) {
                    UIManager.OpenUI(EUIID.UI_UserPartition, false);
                }
            }
        }

        protected override void OnHide()
        {
            //UIManager.CloseUI(EUIID.HUD);
            Sys_HUD.Instance.CloseHud();
            UIManager.CloseUI(EUIID.UI_Uplifted);
            timerDelayBattleUI?.Cancel();
            isDelayBattleUI = false;
        }

        protected override void OnClose()
        {
            UIManager.CloseUI(EUIID.UI_MainBattle, true, false);

            //UIManager.CloseUI(EUIID.UI_HornHUD);
            UIManager.CloseUI(EUIID.UI_Joystick);
            UIManager.CloseUI(EUIID.UI_Menu);
            UIManager.CloseUI(EUIID.UI_FamilyResBattleTop);
            UIManager.CloseUI(EUIID.UI_UseItem);
            UIManager.CloseUI(EUIID.UI_ChatSimplify);
            UIManager.CloseUI(EUIID.UI_Chat);
            UIManager.CloseUI(EUIID.UI_MapTips, true);
            UIManager.CloseUI(EUIID.UI_UserPartition);
            UIManager.CloseUI(EUIID.UI_MainMenu);
            //UIManager.CloseUI(EUIID.HUD);
            Sys_HUD.Instance.CloseHud();
        }
    }
}
