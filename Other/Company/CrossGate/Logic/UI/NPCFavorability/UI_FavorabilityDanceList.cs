using System;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;

namespace Logic {
    // 演奏列表
    public class UI_FavorabilityDanceList : UIBase {
        public class Tab : UIComponent {
            public Button btn;

            protected override void Loaded() {
                btn.onClick.AddListener(OnBtnClicked);
            }

            public void OnBtnClicked() {

            }
        }

        public COWVd<Tab> vds = new COWVd<Tab>();

        public FavorabilityNPC npc;
        public Button btnExit;


        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Animator/View_Title10/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);
        }
        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);
        }
    }
}