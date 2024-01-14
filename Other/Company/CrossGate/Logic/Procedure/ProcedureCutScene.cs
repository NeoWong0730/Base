using Logic.Core;
using Net;
using Packet;
using UnityEngine;
using Lib.Core;

namespace Logic
{
    /// <summary>
    /// 过场流程///
    /// </summary>
    public class ProcedureCutScene : ProcedureBase
    {
        public override ProcedureManager.EProcedureType ProcedureType
        {
            get
            {
                return ProcedureManager.EProcedureType.CutScene;
            }
        }

        protected internal override void OnInit(IFsm procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        protected internal override void OnEnter(IFsm procedureOwner)
        {
            base.OnEnter(procedureOwner);

            DebugUtil.LogFormat(ELogType.eProcedure, "OnEnter ProcedureCutScene");
            RegisterEvent((int)EProcedureEvent.EnterFight, OnEnterFight);
            RegisterEvent((int)EProcedureEvent.EnterInteractive, OnEnterInteractive);
            RegisterEvent((int)EProcedureEvent.EnterNormal, OnEnterNormal);

            OnEnterCutScene();
        }

        protected internal override void OnExit(IFsm procedureOwner, bool isShutdown)
        {
            base.OnExit(procedureOwner, isShutdown);

            DebugUtil.LogFormat(ELogType.eProcedure, "OnExit ProcedureCutScene");
            UnRegisterEvent((int)EProcedureEvent.EnterFight, OnEnterFight);
            UnRegisterEvent((int)EProcedureEvent.EnterInteractive, OnEnterInteractive);
            UnRegisterEvent((int)EProcedureEvent.EnterNormal, OnEnterNormal);

            OnExitCutScene();
        }

        /// <summary>
        /// 进入过场流程一定会触发的逻辑///
        /// </summary>
        void OnEnterCutScene()
        {
            //同步状态
            if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount > 1)
            {
                CmdTeamSyncStat cmdTeamSyncStat = new CmdTeamSyncStat();
                cmdTeamSyncStat.Stat = (uint)ProcedureManager.EProcedureType.CutScene;

                NetClient.Instance.SendMessage((ushort)CmdTeam.SyncStat, cmdTeamSyncStat);
            }
            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnInActiveAllActorHUD);
            DebugUtil.LogFormat(ELogType.eHUD, "ProcedureCutScene.OnInActiveAllActorHUD");
            ActionCtrl.ActionExecuteLockFlag = true;
            UIManager.CloseUI(EUIID.UI_Dialogue, true);
        }

        void OnEnterFight(IFsm procedureOwner, object sender, object userData)
        {
            OnExitCutSceneToFight();
            ChangeState<ProcedureFight>(procedureOwner);
        }

        void OnExitCutSceneToFight()
        {
            //禁止主场景玩家移动
            GameCenter.AllowMainHeroMove(false);

            //禁止主场景伙伴移动
            //GameCenter.AllowParnterMove(false);

            //禁止主场景队友移动
            GameCenter.AllowTeamPlayerMove(false);
        }

        void OnEnterInteractive(IFsm procedureOwner, object sender, object userData)
        {
            ChangeState<ProcedureInteractive>(procedureOwner);
        }

        void OnEnterNormal(IFsm procedureOwner, object sender, object userData)
        {
            ChangeState<ProcedureNormal>(procedureOwner);
            OnEnterCutSceneToNormal();
        }

        void OnEnterCutSceneToNormal()
        {
            //如果在自动任务状态，继续任务
            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
            {
                Sys_Task.Instance.TryContinueDoCurrentTask();
            }
        }

        void OnExitCutScene()
        {
            ActionCtrl.ActionExecuteLockFlag = false;

            UIManager.CloseUI(EUIID.UI_Subtitle);
            UIManager.CloseUI(EUIID.UI_Chapter);
        }
    }
}
