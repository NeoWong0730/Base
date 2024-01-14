using System;
using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic {
    public class UI_ReadMapTip : UIBase {
        public Text title;
        public Text contentHead;
        public Text content;
        public Text contentTail;
        public Button btnGoto;

        public uint mapId;
        public CSVMapInfo.Data csvMap;

        protected override void OnLoaded() {
            this.title = this.transform.Find("Animator/View_TipsBg/Text_Title").GetComponent<Text>();

            this.contentHead = this.transform.Find("Animator/Text1").GetComponent<Text>();
            this.content = this.transform.Find("Animator/Text").GetComponent<Text>();
            this.contentTail = this.transform.Find("Animator/Text2").GetComponent<Text>();

            this.btnGoto = this.transform.Find("Animator/Btn_01").GetComponent<Button>();
            this.btnGoto.onClick.AddListener(this.OnBtnGotoClicked);
        }

        protected override void OnOpen(object arg) {
            this.mapId = Convert.ToUInt32(arg);
            this.csvMap = CSVMapInfo.Instance.GetConfData(this.mapId);
        }

        protected override void OnOpened() {
            if (this.csvMap != null) {
                string promptName = LanguageHelper.GetTextContent(this.csvMap.PromptName);
                TextHelper.SetText(this.title, 4569, promptName);
                // TextHelper.SetText(this.contentHead, 0);
                TextHelper.SetText(this.content, this.csvMap.PromptContent);
                TextHelper.SetText(this.contentTail, 4570, promptName);

                // 标记已经阅读过
                Sys_Map.Instance.ReqReadMapMail(this.mapId);
            }
            else {
                // error
            }
        }

        private void OnBtnGotoClicked() {
            UIManager.OpenUI(EUIID.UI_Map, false, new Sys_Map.TargetMapParameter(this.mapId));
        }
    }
}