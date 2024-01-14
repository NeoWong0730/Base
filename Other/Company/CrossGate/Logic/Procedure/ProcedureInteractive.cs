using Logic.Core;
using Net;
using Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;

namespace Logic
{
    /// <summary>
    /// 交互流程///
    /// </summary>
    public class ProcedureInteractive : ProcedureBase
    {
        public override ProcedureManager.EProcedureType ProcedureType
        {
            get
            {
                return ProcedureManager.EProcedureType.Interactive;
            }
        }

        protected internal override void OnInit(IFsm procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected internal override void OnEnter(IFsm procedureOwner)
        {
            base.OnEnter(procedureOwner);

            DebugUtil.LogFormat(ELogType.eProcedure, "OnEnter ProcedureInteractive");
            RegisterEvent((int)EProcedureEvent.EnterNormal, OnEnterNormal);
            RegisterEvent((int)EProcedureEvent.EnterFight, OnEnterFight);
            RegisterEvent((int)EProcedureEvent.EnterCutScene, OnEnterCutScene);

            OnEnterInteractive();
        }

        protected internal override void OnExit(IFsm procedureOwner, bool isShutdown)
        {
            base.OnExit(procedureOwner, isShutdown);

            DebugUtil.LogFormat(ELogType.eProcedure, "OnExit ProcedureInteractive");
            UnRegisterEvent((int)EProcedureEvent.EnterNormal, OnEnterNormal);
            UnRegisterEvent((int)EProcedureEvent.EnterFight, OnEnterFight);
            UnRegisterEvent((int)EProcedureEvent.EnterCutScene, OnEnterCutScene);

            OnExitInteractive();
        }

        void OnEnterNormal(IFsm procedureOwner, object sender, object userData)
        {
            OnExitInteractiveToNormal();
            ChangeState<ProcedureNormal>(procedureOwner);
            OnEnterInteractiveToNormal();
        }

        void OnEnterFight(IFsm procedureOwner, object sender, object userData)
        {
            OnExitInteractiveToFight();
            ChangeState<ProcedureFight>(procedureOwner);
        }

        void OnEnterCutScene(IFsm procedureOwner, object sender, object userData)
        {
            ChangeState<ProcedureCutScene>(procedureOwner);
        }

        void OnExitInteractiveToFight()
        {
            //禁止主场景玩家移动
            GameCenter.AllowMainHeroMove(false);

            //禁止主场景伙伴移动
            //GameCenter.AllowParnterMove(false);

            //禁止主场景队友移动
            GameCenter.AllowTeamPlayerMove(false);
        }

        #region InteractiveToNormal

        /// <summary>
        /// 交互流程到正常流程之前/
        /// </summary>
        void OnExitInteractiveToNormal()
        {
            Sys_Input.Instance.bEnableJoystick = false;
            Sys_Input.Instance.bForbidControl = true;
            Sys_Input.Instance.bForbidTouch = true;
        }

        /// <summary>
        /// 交互流程到正常流程之后
        /// </summary>
        void OnEnterInteractiveToNormal()
        {

        }

        #endregion

        /// <summary>
        /// 进入交互流程一定会触发的逻辑
        /// </summary>
        void OnEnterInteractive()
        {
            ActionCtrl.ActionExecuteLockFlag = true;

            Sys_Input.Instance.bEnableJoystick = false;
            Sys_Input.Instance.bForbidControl = true;
            Sys_Input.Instance.bForbidTouch = true;

            //同步状态
            if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount > 1)
            {
                CmdTeamSyncStat cmdTeamSyncStat = new CmdTeamSyncStat();
                cmdTeamSyncStat.Stat = (uint)ProcedureManager.EProcedureType.Interactive;

                NetClient.Instance.SendMessage((ushort)CmdTeam.SyncStat, cmdTeamSyncStat);
            }

            GameCenter.mCameraController.EnterNPCInteractive();
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnInActiveAllActorHUD);
            DebugUtil.LogFormat(ELogType.eHUD, "ProcedureInteractive.OnInActiveAllActorHUD");

            UIManager.CloseUI(EUIID.UI_Chapter, true);
            UIManager.CloseUI(EUIID.UI_Subtitle, true);
            UIManager.CloseUI(EUIID.UI_PromptBox, true);
            Sys_CutScene.Instance.End(true);
        }

        /// <summary>
        /// 退出交互流程一定会触发的逻辑///
        /// </summary>
        void OnExitInteractive()
        {
            ActionCtrl.ActionExecuteLockFlag = false;

            //将行为控制器中的当前交互行为的标识设成完成
            //行为控制是手动模式
            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.PlayerCtrl)
            {
                InteractiveWithNPCAction interactiveWithNPCAction = ActionCtrl.Instance.currentPlayerCtrlAction as InteractiveWithNPCAction;
                if (interactiveWithNPCAction != null)
                {
                    interactiveWithNPCAction.InteractiveActionCompletedFlag = true;
                }
            }
            //行为控制是自动模式
            else if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
            {
                InteractiveWithNPCAction interactiveWithNPCAction = ActionCtrl.Instance.currentAutoAction as InteractiveWithNPCAction;
                if (interactiveWithNPCAction != null)
                {
                    interactiveWithNPCAction.InteractiveActionCompletedFlag = true;
                }
            }

            //关闭NPC和对话UI
            UIManager.CloseUI(EUIID.UI_NPC);
            UIManager.CloseUI(EUIID.UI_Dialogue);
        }
    }
}
