using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using Net;
using Packet;

namespace Logic
{
    /// <summary>
    /// 普通流程///
    /// </summary>
    public class ProcedureNormal : ProcedureBase
    {
        Timer timer;

        public override ProcedureManager.EProcedureType ProcedureType
        {
            get
            {
                return ProcedureManager.EProcedureType.Normal;
            }
        }

        protected internal override void OnInit(IFsm procedureOwner)
        {
            base.OnInit(procedureOwner);          
        }

        protected internal override void OnEnter(IFsm procedureOwner)
        {
            base.OnEnter(procedureOwner);

            DebugUtil.LogFormat(ELogType.eProcedure, "OnEnter ProcedureNormal");
            RegisterEvent((int)EProcedureEvent.EnterFight, OnEnterFight);
            RegisterEvent((int)EProcedureEvent.EnterInteractive, OnEnterInteractive);
            RegisterEvent((int)EProcedureEvent.EnterCutScene, OnEnterCutScene);

            OnEnterNormal();
        }

        protected internal override void OnExit(IFsm procedureOwner, bool isShutdown)
        {
            base.OnExit(procedureOwner, isShutdown);

            DebugUtil.LogFormat(ELogType.eProcedure, "OnExit ProcedureNormal");
            UnRegisterEvent((int)EProcedureEvent.EnterFight, OnEnterFight);
            UnRegisterEvent((int)EProcedureEvent.EnterInteractive, OnEnterInteractive);
            UnRegisterEvent((int)EProcedureEvent.EnterCutScene, OnEnterCutScene);

            OnExitNormal();
        }

        void OnEnterFight(IFsm procedureOwner, object sender, object userData)
        {
            OnExitNormalToFight();
            ChangeState<ProcedureFight>(procedureOwner);
            OnEnterNormalToFight();
        }

        void OnEnterInteractive(IFsm procedureOwner, object sender, object userData)
        {
            OnExitNormalToInteractive(sender, userData);
            ChangeState<ProcedureInteractive>(procedureOwner);
            OnEnterNormalToInteractive(sender, userData);
        }

        void OnEnterCutScene(IFsm procedureOwner, object sender, object userData)
        {
            OnExitNormalToCutScene();
            ChangeState<ProcedureCutScene>(procedureOwner);
            OnEnterNormalToCutScene();
        }

        #region NormalToFight

        /// <summary>
        /// 正常流程到战斗流程之前///
        /// </summary>
        void OnExitNormalToFight()
        {
            //UI行为
            //UIManager.HideStackUI();

            //重置行为控制器
            ActionCtrl.Instance.Reset();

            //禁止主场景玩家移动
            GameCenter.AllowMainHeroMove(false);

            //禁止主场景伙伴移动
            //GameCenter.AllowParnterMove(false);

            //禁止主场景队友移动
            GameCenter.AllowTeamPlayerMove(false);
        }

        /// <summary>
        /// 正常流程到战斗流程之后///
        /// </summary>
        void OnEnterNormalToFight()
        {
            //相机设置
            //GameCenter.mCameraController.FadeInOut(EnterEffectFunc);
        }

        //void EnterEffectFunc()
        //{
        //    //play bgm
        //    Framework.AudioManager.Instance.SetVolume(Framework.EAudioType.BGM, 1f);
        //    CSVParam.Data paramData = CSVParam.Instance.GetConfData(132);
        //    if (paramData != null)
        //    {
        //        AudioUtil.PlayAudio(System.Convert.ToUInt32(paramData.str_value));
        //    }

        //    ProcedureManager.eventEmitter.Trigger(ProcedureManager.EEvents.OnAfterEnterFightEffect);
        //    UIManager.OpenUI(EUIID.UI_MainBattle);
        //}

        #endregion

        #region NormalToInteractive

        /// <summary>
        /// 正常流程到交互流程之前///
        /// </summary>
        void OnExitNormalToInteractive(object sender, object userData)
        {
            GameCenter.mainHero?.movementComponent?.Stop();
        }

        /// <summary>
        /// 正常流程到交互流程之后///
        /// </summary>
        void OnEnterNormalToInteractive(object sender, object userData)
        {
            InteractiveWithNPCAction interactiveWithNPCAction = sender as InteractiveWithNPCAction;
            if (interactiveWithNPCAction != null)
            {
                interactiveWithNPCAction.InteractiveActionCompletedFlag = false;
            }
        }

        #endregion

        #region NormalToCutScene

        void OnExitNormalToCutScene()
        {
            //重置行为控制器
            ActionCtrl.Instance.Reset();
        }

        void OnEnterNormalToCutScene()
        {

        }

        #endregion

        /// <summary>
        /// 进入正常流程一定会触发的逻辑///
        /// </summary>
        void OnEnterNormal()
        {
            ProcedureManager.eventEmitter.Trigger(ProcedureManager.EEvents.OnEnterNormal);

            //相机设置
            GameCenter.mCameraController?.EnterWorld();

            //HUD设置
            Sys_HUD.Instance?.eventEmitter.Trigger(Sys_HUD.EEvents.OnActiveAllActorHUD);
            DebugUtil.LogFormat(ELogType.eHUD, "ProcedureNormal.OnActiveAllActorHUD");

            //打开遮挡透明效果
            RenderExtensionSetting.bUsageOcclusionTransparent = true;

            timer?.Cancel();
            timer = Timer.Register(0.1f, () =>
            timer = Timer.Register(float.Parse(CSVParam.Instance.GetConfData(117).str_value) / 1000f, () =>
            {
                Sys_Input.Instance.bEnableJoystick = true;
                Sys_Input.Instance.bForbidControl = false;
                Sys_Input.Instance.bForbidTouch = false;
            }, null, false, true));

            //允许主场景玩家移动
            GameCenter.AllowMainHeroMove(true);

            //允许主场景伙伴移动
            //GameCenter.AllowParnterMove(true);

            //允许主场景队友移动
            GameCenter.AllowTeamPlayerMove(true);

            //启用NPC点击
            GameCenter.EnableNpcClick(true);

            //同步状态
            if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount > 1)
            {
                CmdTeamSyncStat cmdTeamSyncStat = new CmdTeamSyncStat();
                cmdTeamSyncStat.Stat = (uint)ProcedureManager.EProcedureType.Normal;

                NetClient.Instance.SendMessage((ushort)CmdTeam.SyncStat, cmdTeamSyncStat);
            }
        }

        /// <summary>
        /// 退出正常流程一定会触发的逻辑///
        /// </summary>
        void OnExitNormal()
        {
            //禁止主场景玩家移动
            //GameCenter.AllowMainHeroMove(false);

            ////禁止主场景伙伴移动
            //GameCenter.AllowParnterMove(false);

            ////禁止主场景队友移动
            //GameCenter.AllowTeamPlayerMove(false);

            //禁止NPC点击
            GameCenter.EnableNpcClick(false);
        }
    }
}
