using System;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TalentReset : UIBase {
        public UI_CostItem costItem = new UI_CostItem();
        public ItemIdCount idCount = new ItemGuidCount();
        public Button btnSure;
        public Text title;

        protected override void OnLoaded() {
            this.costItem.SetGameObject(this.transform.Find("Animator/Cost_Coin").gameObject);

            this.title = this.transform.Find("Animator/Text_Tip").GetComponent<Text>();

            this.btnSure = this.transform.Find("Animator/Buttons/Button_Sure").GetComponent<Button>();
            this.btnSure.onClick.AddListener(this.OnBtnSureClicked);
        }

        [Flags]
        public enum EReason {
            Nil,
            LackOfItem = 1, // 道具不足
            NoUsedTalent = 2, // 没有使用的天赋点
        }

        public EReason reason;

        public int schemeIndex;
        protected override void OnOpen(object arg) {
            schemeIndex = Convert.ToInt32(arg);
        }

        protected override void OnOpened() {
            if (Sys_Ini.Instance.Get<IniElement_IntArray>(260, out IniElement_IntArray outer) && outer.value != null && outer.value.Length >= 2) {
                uint id = (uint) outer.value[0];
                int count = outer.value[1];

                TextHelper.SetText(this.title, 5521);
                this.idCount.Reset(id, count);
                this.costItem.Refresh(this.idCount, ItemCostLackType.Normal, null, false);

                if (Sys_Talent.Instance.schemes[schemeIndex].usedTalentPoint <= 0) {
                    reason |= EReason.NoUsedTalent;
                }
                if (!this.idCount.Enough) {
                    reason |= EReason.LackOfItem;
                }
                
                bool isEnough = reason == EReason.Nil;
                ImageHelper.SetImageGray(this.btnSure, !isEnough, true);
            }
        }

        private void OnBtnSureClicked() {
            if ((reason & EReason.LackOfItem) == EReason.LackOfItem) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5529));
                return;
            }
            else if((reason & EReason.NoUsedTalent) == EReason.NoUsedTalent) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5530));
                return;
            }
            
            Sys_Talent.Instance.ReqResetTalentPoint((uint)schemeIndex);
            this.CloseSelf();
        }
    }
}