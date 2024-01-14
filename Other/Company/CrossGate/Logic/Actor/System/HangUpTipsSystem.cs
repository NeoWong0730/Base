using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class HangUpTipsSystem : LevelSystemBase
    {
        private float GAP = 1f;
        private Timer timer;

        public override void OnCreate()
        {
            float.TryParse(CSVHangupParam.Instance.GetConfData(13).str_value, out this.GAP);
            //this.GAP = 3f;
            this.ProcessEvents(true);
        }

        public override void OnDestroy()
        {         
            this.timer?.Cancel();
            this.ProcessEvents(false);
        }

        private void ProcessEvents(bool toRegist)
        {
            Sys_Hangup.Instance.eventEmitter.Handle(Sys_Hangup.EEvents.OnHangupEnter, this.OnHangupEnter, toRegist);
            Sys_Hangup.Instance.eventEmitter.Handle(Sys_Hangup.EEvents.OnHangupExit, this.OnHangupExit, toRegist);
        }

        private void OnHangupEnter()
        {
            this.EnterHangup();
        }
        private void OnHangupExit()
        {
            //Sys_Hangup.Instance.firstAD = true;
            this.ExitHangup();
        }

        public void EnterHangup()
        {
            this.TryTip();
        }

        public void ExitHangup()
        {
            this.timer?.Cancel();
        }

        private void TryTip()
        {
            if (!Sys_Hangup.Instance.firstAD)
            {
                return;
            }

            var cmdHangUpDataNtf = Sys_Hangup.Instance.cmdHangUpDataNtf;
            if (cmdHangUpDataNtf == null || cmdHangUpDataNtf.WorkingHourOpened || cmdHangUpDataNtf.WorkingHourPoint <= 0)
            {
                return;
            }

            this.timer?.Cancel();
            // 进入挂机状态之后，条件后面才满足，不处理！！！
            this.timer = Timer.RegisterOrReuse(ref this.timer, this.GAP, this.OnCompleted, null, false, false);
        }

        private void OnCompleted()
        {
            this.timer?.Cancel();
            UIManager.OpenUI(EUIID.UI_HangupTip);
        }
    }
}