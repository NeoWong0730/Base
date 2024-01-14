using System;
using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic {
    // –‘∏ÒΩÈ…‹
    public class UI_FavorabilityNPCCharacterDesc : UIBase {
        public FavorabilityNPC npc;

        public Button btnExit;
        public Text characterDesc;
        public Text npcDesc;

        public void OnBtnExitClicked() {
            this.CloseSelf();
        }

        protected override void OnLoaded() {
            this.btnExit = this.transform.Find("Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.characterDesc = this.transform.Find("ZoneCharacter/View_Character/Character/Text_Des").GetComponent<Text>();
            this.npcDesc = this.transform.Find("ZoneCharacter/View_Character/Background/Text_Des").GetComponent<Text>();
        }
        protected override void OnOpen(object arg) {
            uint id = Convert.ToUInt32(arg);
            Sys_NPCFavorability.Instance.TryGetNpc(id, out this.npc, false);
        }

        protected override void OnShow() {
            CSVNPCCharacter.Data csvCharacter = CSVNPCCharacter.Instance.GetConfData(this.npc.csvNPCFavorability.Character);
            if (csvCharacter != null) {
                TextHelper.SetText(this.characterDesc, csvCharacter.Des, (1f * csvCharacter.FavorabilityRatio / 10000f).ToString("P0"));
                TextHelper.SetText(this.npcDesc, npc.csvNPCFavorability.BackStory);
            }

            Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
        }
    }
}