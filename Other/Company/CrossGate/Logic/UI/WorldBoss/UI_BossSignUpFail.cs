using Lib.Core;
using Logic.Core;
using Packet;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 报名失败
    public class UI_BossSignUpFail : UIBase {
        public Button btnCancel;

        protected override void OnLoaded() {
            this.btnCancel = this.transform.Find("Animator/Wait/Button_Sure").GetComponent<Button>();
            this.btnCancel.onClick.AddListener(this.OnBtnCancelClicked);
        }
        protected override void OnOpen(object arg) {
            UIManager.CloseUI(EUIID.UI_WorldBossSignUp);
        }
        private void OnBtnCancelClicked() {
            CloseSelf();
        }
    }
}
