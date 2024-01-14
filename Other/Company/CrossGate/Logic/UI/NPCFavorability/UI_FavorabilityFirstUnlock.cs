using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityFirstUnlock : UIBase {
        public Text content;
        
        protected override void OnLoaded() {
            this.content = this.transform.Find("Animator/Image_BG/Text_Title").GetComponent<Text>();
        }

        private CmdFavorabilityFirstUnlockNpcRes res;
        protected override void OnOpen(object arg) {
            res = arg as CmdFavorabilityFirstUnlockNpcRes;
        }

        protected override void OnOpened() {
            var csv = CSVNPCFavorability.Instance.GetConfData(this.res.NpcId);
            if (csv != null) {
                TextHelper.SetText(this.content, csv.UnlockPrompt, this.res.FirstRoleName.ToStringUtf8());
            }
        }
    }
}