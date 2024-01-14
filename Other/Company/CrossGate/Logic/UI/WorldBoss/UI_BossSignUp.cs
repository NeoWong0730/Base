using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 报名参赛
    public class UI_BossSignUp : UIBase {
        public Text remainTime;
        public GameObject overLimit;
        public Slider slider;
        public Button btnCancel;

        public CmdWildBossApplyRes signUp;
        private Timer timer;
        private uint RemainTime;

        protected override void OnLoaded() {
            this.btnCancel = this.transform.Find("Animator/Wait/Button_Sure").GetComponent<Button>();
            this.btnCancel.onClick.AddListener(this.OnBtnCancelClicked);

            this.overLimit = this.transform.Find("Animator/Wait/GameObject/Text_Team").gameObject;
            this.remainTime = this.transform.Find("Animator/Wait/GameObject/Text_TeamCout").GetComponent<Text>();
            this.slider = this.transform.Find("Animator/Wait/GameObject/Image_wait").GetComponent<Slider>();
        }
        protected override void OnOpen(object arg) {
            this.signUp = arg as CmdWildBossApplyRes;
        }
        protected override void OnOpened() {
            this.timer?.Cancel();
            RemainTime = this.signUp.TimeStamp + 1;
            this.timer = Timer.Register(RemainTime, () => {
                this.CloseSelf();
            }, (dt) => {
                float fRemain = RemainTime - dt;
                this.RefreshTime(fRemain);
            }, false);
        }
        protected override void OnDestroy() {
            this.timer?.Cancel();
        }
        protected override void OnShow() {
            this.RefreshAll();
        }
        private void RefreshAll() {
            this.RefreshTime(RemainTime);

            bool isCaptain = Sys_Team.Instance.isCaptain();
            btnCancel.gameObject.SetActive(isCaptain);

            uint bossId = signUp.BossId;
            var csv = CSVBOSSInformation.Instance.GetConfData(bossId);
            if (csv != null) {
                bool isNotOverLimit = Sys_WorldBoss.Instance.IsActivityCountValid(csv.playMode_id, out int usedCount, out int limit);
                overLimit.SetActive(!isNotOverLimit);
            }
            else {
                overLimit.SetActive(false);
            }
        }
        public void RefreshTime(float fRemain) {
            int iRemain = Mathf.FloorToInt(fRemain);
            this.remainTime.text = iRemain.ToString();
            slider.value = 1f * fRemain / RemainTime;
        }

        private void OnBtnCancelClicked() {
            this.timer?.Cancel();
            this.CloseSelf();

            ulong teamId = Sys_Team.Instance.teamID;
            ulong roleId = Sys_Role.Instance.RoleId;
            ulong guid = this.signUp.BossGuid;
            Sys_WorldBoss.Instance.ReqSignUp(this.signUp.BossId, teamId, roleId, 1, guid);
        }
    }
}


