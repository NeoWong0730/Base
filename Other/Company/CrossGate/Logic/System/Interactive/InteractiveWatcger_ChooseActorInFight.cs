using Lib.Core;
using Logic.Core;
using UnityEngine;
using Table;
using Packet;

namespace Logic
{
    [InteractiveWatcher(EInteractiveAimType.ChooseActorInFight)]

    public class InteractiveWatcger_ChooseActorInFight : IInteractiveWatcher
    {
        private Timer time;
        private  Timer delayTime;
        private bool isDoubleClicked;
        public void OnAreaCheckExecute(InteractiveEvtData data)
        {
        }

        public void OnClickExecute(InteractiveEvtData data)
        {
            delayTime?.Cancel();
            delayTime = Timer.Register(GameCenter.fightControl.doubleTime, () =>
            {
                if (!isDoubleClicked)
                {
                    DoOneClicked(data);
                }
                delayTime.Cancel();
                isDoubleClicked = false;
            }, null, false, true);
        }

        private void DoOneClicked(InteractiveEvtData data)
        {
            DebugUtil.Log(ELogType.eCombat, $"InteractiveWatcher_ChooseActorInFight.OnClickExecute() uid: {data.sceneActor.UID}");
            MobEntity mob = MobManager.Instance.GetMob((uint)data.sceneActor.UID);
            if (GameCenter.fightControl == null)
            {
                return;
            }
            if (!GameCenter.fightControl.m_SelectList.Contains(mob))
            {
                if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstChoose || GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondChoose)
                {
                    GameCenter.fightControl.DoSelectedForbitMob(mob);
                }
                else if (Net_Combat.Instance._canSendBattleUnitInfoReq && !GameCenter.fightControl.isCommanding)
                {
                    UIManager.OpenUI(EUIID.UI_Buff, true, mob.m_Go);
                    Net_Combat.Instance._canSendBattleUnitInfoReq = false;

                    time?.Cancel();
                    time = Timer.Register(Constants.BUFFSHOWCLICKTIME, () =>
                    {
                        Net_Combat.Instance._canSendBattleUnitInfoReq = true;
                        time.Cancel();
                    }, (time) =>
                    {
                        Net_Combat.Instance._canSendBattleUnitInfoReq = false;
                    }, false, false);
                }

                return;
            }
            if (!GameCenter.fightControl.isCommanding)
            {
                GameCenter.fightControl.DoSelectedMob(mob);
            }
        }

        public void OnDoubleClickExecute(InteractiveEvtData data)
        {
            isDoubleClicked = true;
            MobEntity mob = MobManager.Instance.GetMob((uint)data.sceneActor.UID);
            if (mob == null || GameCenter.fightControl == null|| Sys_Fight.Instance.AutoFightData.AutoState)
            {
                return;
            }
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForFirstOperation || GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation)
            {
                GameCenter.fightControl.DoubleClickedOp(mob);
            }
        }

        public void OnLongPressExecute(InteractiveEvtData data)
        {

        }

        public void OnDistanceCheckExecute(InteractiveEvtData data)
        {

        }

        public void OnUIButtonExecute(InteractiveEvtData data)
        {

        }
    }
}
