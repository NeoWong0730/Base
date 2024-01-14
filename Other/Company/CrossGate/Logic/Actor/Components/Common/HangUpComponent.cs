using Lib.Core;
using Logic.Core;
using Table;

#if false
namespace Logic {    
    public class HangUpComponent : Component, IUpdateCmd {
        private Timer hangupTimer;
        private float hangupDuration;

        private StateComponent stateComponent;

        protected override void OnConstruct() {
            base.OnConstruct();

            float.TryParse(CSVParam.Instance.GetConfData(262).str_value, out this.hangupDuration);
            //stateComponent = World.GetComponent<StateComponent>(this.actor);
            stateComponent = ((SceneActor)actor).stateComponent;
            lastTime = 0;
        }

        protected override void OnDispose() {
            this.hangupTimer?.Cancel();
            this.hangupTimer = null;
            this.hangupDuration = 0f;

            this.stateComponent = null;

            base.OnDispose();
        }

        private float lastTime = 0;
        public void Update() {
            if (UnityEngine.Time.realtimeSinceStartup - this.lastTime < 2f) {
                return;
            }
            this.lastTime = UnityEngine.Time.realtimeSinceStartup;

            if (GameMain.Procedure == null || GameMain.Procedure.CurrentProcedure == null || GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal) {
                return;
            }

            if (this.stateComponent.CurrentState == EStateType.Idle) {
                if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.Hangup) {
                    this.TryHangup();
                }
            }
        }

        private void TryHangup() {
            if (this.hangupTimer != null) {
                return;
            }
            if (!Sys_FunctionOpen.Instance.IsOpen(50701)) {
                return;
            }
            if (!OptionManager.Instance.GetBoolean(OptionManager.EOptionID.AutoHangup)) {
                return;
            }
            var v = Sys_Hangup.Instance.GetTired();
            if (v >= 100 && OptionManager.Instance.GetBoolean(OptionManager.EOptionID.TriedPointProtection)) {
                //DebugUtil.LogError("TriedValue: " + v);
                return;
            }
            if (!Sys_Team.Instance.canManualOperate) {
                return;
            }
            if (Sys_Instance.Instance.IsInInstance) {
                return;
            }
            uint mapId = Sys_Map.Instance.CurMapId;
            CSVMapInfo.Data csv = CSVMapInfo.Instance.GetConfData(mapId);
            if (csv != null) {
                if (csv.AutoHangup) {
                    return;
                }
            }

            this.hangupTimer = Timer.RegisterOrReuse(ref hangupTimer, this.hangupDuration * 60, this.OnCompleted, this.OnTiming, false, false);
        }

        private void TryUnHangup() {
            this.hangupTimer?.Cancel();
            this.hangupTimer = null;
        }

        private void OnCompleted() {
            this.hangupTimer?.Cancel();
            this.hangupTimer = null;

            if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                return;
            }
            if (UIManager.IsOpen(EUIID.UI_NPC)) {
                // npc开启的时候，是normal模式
                UIManager.CloseUI(EUIID.UI_NPC);
            }
            else if (GameMain.Procedure != null && GameMain.Procedure.CurrentProcedure != null && GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Interactive) {
                // 对话开启的时候，是interactive模式
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);

                Sys_NPCFavorability.Instance.CloseAllUI();
            }

            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                void OnConform() {
                    Sys_Hangup.Instance.HangUpOpReq(Packet.HangUpOperator.StartHangUp);

                    this.hangupTimer?.Cancel();
                    this.hangupTimer = null;
                }

                void OnCancel() {
                    // 重新开始计时
                    this.hangupTimer?.Cancel();
                    this.hangupTimer = Timer.Register(this.hangupDuration * 60, this.OnCompleted, this.OnTiming, false, false);
                }

                UIManager.CloseUI(EUIID.UI_PromptBox, true);
                UIManager.UpdateState();

                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.SetCountdown(10f, PromptBoxParameter.ECountdown.Confirm);
                PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2104045).words;
                PromptBoxParameter.Instance.SetConfirm(true, OnConform, 2104046);
                PromptBoxParameter.Instance.SetCancel(true, OnCancel);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        private void OnTiming(float time) {
            void Cancel(bool toReq) {
                this.hangupTimer?.Cancel();
                this.hangupTimer = null;

                if (toReq) {
                    // 结束挂机
                    //if (Sys_Pet.Instance.clientStateId == Sys_Role.EClientState.Hangup) {
                    //    Sys_Hangup.Instance.HangUpOpReq(Packet.HangUpOperator.EndOnlineHangUp);
                    //}
                }
            }

            if (Sys_Instance.Instance.IsInInstance) {
                Cancel(false);
            }
            else if (!Sys_Team.Instance.canManualOperate) {
                Cancel(false);
            }
            else if (this.stateComponent.CurrentState != EStateType.Idle) {
                Cancel(true);
            }
            else if (!OptionManager.Instance.GetBoolean(OptionManager.EOptionID.AutoHangup)) {
                Cancel(false);
            }
            else if (Sys_Hangup.Instance.GetTired() >= 100 && 
                     OptionManager.Instance.GetBoolean(OptionManager.EOptionID.TriedPointProtection)) {
                //DebugUtil.LogError("TriedValue: " + Sys_Hangup.Instance.GetTired());
                Cancel(false);
            }
            else if (GameMain.Procedure != null && GameMain.Procedure.CurrentProcedure != null && GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Interactive) {
                Cancel(false);
            }

            uint mapId = Sys_Map.Instance.CurMapId;
            CSVMapInfo.Data csv = CSVMapInfo.Instance.GetConfData(mapId);
            if (csv != null) {
                if (csv.AutoHangup) {
                    Cancel(false);
                }
            }
        }    
    }
}
#endif
