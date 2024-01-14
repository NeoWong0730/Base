using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_HangupTip : UIBase {
        public Text content;
        public Text leftBtnText;
        public Text rightBtnText;

        public GameObject firstTip;
        public Text fitstTipContent;
        public Timer timer;

        public float duration;

        protected override void OnLoaded() {
            this.content = this.transform.Find("Animator/Text_Tip").GetComponent<Text>();

            Button btn = this.transform.Find("Animator/Buttons/Button_Cancel").GetComponent<Button>();
            btn.onClick.AddListener(this.OnLeftBtnClicked);
            btn = this.transform.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            btn.onClick.AddListener(this.OnRightBtnClicked);
            this.leftBtnText = this.transform.Find("Animator/Buttons/Button_Cancel/Text").GetComponent<Text>();
            this.rightBtnText = this.transform.Find("Animator/Buttons/Button_Sure/Text").GetComponent<Text>();

            this.firstTip = this.transform.Find("Animator/View_Toggle").gameObject;
            Toggle tg = this.transform.Find("Animator/View_Toggle/Toggle_Read").GetComponent<Toggle>();
            tg.onValueChanged.AddListener(this.OnValueChanged);
            this.fitstTipContent = this.transform.Find("Animator/View_Toggle/Label").GetComponent<Text>();

            this.timer?.Cancel();
            float.TryParse(CSVHangupParam.Instance.GetConfData(14).str_value, out duration);
            this.timer = Timer.RegisterOrReuse(ref this.timer, duration, this.OnLeftBtnClicked, this.OnTiming);
        }

        protected override void OnOpened() {
            TextHelper.SetText(this.content, 2104048, Sys_Role.Instance.Role.Name.ToStringUtf8(), Sys_Hangup.Instance?.cmdHangUpDataNtf.WorkingHourPoint.ToString());

            if (Sys_Hangup.Instance.firstAD) {
                this.firstTip.SetActive(true);
                TextHelper.SetText(this.fitstTipContent, 2104049);
            }
            else {
                this.firstTip.SetActive(false);
            }

            TextHelper.SetText(this.rightBtnText, 2104050);
        }
        protected override void OnDestroy() {
            this.timer?.Cancel();
        }

        private void OnLeftBtnClicked() {
            this.CloseSelf();
        }

        private void OnRightBtnClicked() {
            Sys_Hangup.Instance.SendHangUpWorkingHourOpReq(true);
            this.CloseSelf();
        }
        private void OnValueChanged(bool flag) {
            Sys_Hangup.Instance.firstAD = !flag;
        }
        private void OnTiming(float dt) {
            int t = Mathf.RoundToInt(duration - dt);
            TextHelper.SetText(this.leftBtnText, 2104051, t.ToString());
        }
    }
}