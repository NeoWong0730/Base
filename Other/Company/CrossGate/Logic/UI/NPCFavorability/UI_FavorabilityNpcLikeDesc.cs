using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityNpcLikeDesc : UIBase {
        public class Tab : UIComponent {
            public Text desc;

            protected override void Loaded() {
                this.desc = transform.Find("Text").GetComponent<Text>();
            }

            public void Refresh(uint lanId) {
                TextHelper.SetText(this.desc, lanId);
            }
        }

        public GameObject proto;
        public COWVd<Tab> vds = new COWVd<Tab>();

        protected override void OnLoaded() {
            this.proto = this.transform.Find("Animator/Image_BG/Des").gameObject;
        }

        private uint npcId;

        protected override void OnOpen(object arg) {
            npcId = Convert.ToUInt32(arg);
        }


        private List<uint> ids = new List<uint>();

        protected override void OnOpened() {
            var csv = CSVNPCFavorability.Instance.GetConfData(npcId);
            if (csv != null) {
                ids = new List<uint>() {
                    csv.GiftPrompt1,
                    csv.GiftPrompt2,
                    csv.GiftPrompt3,
                };
                vds.TryBuildOrRefresh(this.proto, this.proto.transform.parent, ids.Count, OnRefreshTab);

                FrameworkTool.ForceRebuildLayout(this.gameObject);
            }
        }

        private void OnRefreshTab(Tab vd, int index) {
            vd.Refresh(this.ids[index]);
        }
    }
}