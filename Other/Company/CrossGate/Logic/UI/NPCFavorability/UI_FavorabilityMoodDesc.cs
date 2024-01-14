using System;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityMoodDesc : UIBase {
        public Text desc;
        public RectTransform node;

        private string content;
        private Vector3 pos;

        protected override void OnLoaded() {
            this.node = this.transform.Find("Animator/ZoneHealth").GetComponent<RectTransform>();
            
            this.desc = this.transform.Find("Animator/ZoneHealth/View_Health/Text_Des").GetComponent<Text>();
            Button btn = this.transform.Find("Close").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnExitClicked);
        }

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
        protected override void OnOpen(object arg) {
            var tp = arg as Tuple<string, Vector3>;
            if (tp != null) {
                this.content = tp.Item1;
                this.pos = tp.Item2;
            }
        }
        protected override void OnShow() {
            this.RefreshAll();
        }
        public void RefreshAll() {
            TextHelper.SetText(this.desc, this.content);
            node.anchoredPosition = this.pos;
            
            Lib.Core.FrameworkTool.ForceRebuildLayout(this.gameObject);
        }
    }
}