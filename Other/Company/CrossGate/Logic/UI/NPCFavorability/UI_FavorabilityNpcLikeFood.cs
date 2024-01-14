using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityNpcLikeFood : UIBase {
        public class Tab : UISelectableElement {
            public CP_Toggle toggle;
            public Text tabNameLight;
            public Text tabNameDark;

            public uint tabId = 0;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.tabNameLight = this.transform.Find("Object_Selected/Text").GetComponent<Text>();
                this.tabNameDark = this.transform.Find("Object/Text").GetComponent<Text>();
            }
            public void Refresh(uint tabId) {
                this.tabId = tabId;
                var csv = CSVFavorabilityBanquet.Instance.GetConfData(tabId);
                if (csv != null) {
                    TextHelper.SetText(this.tabNameLight, csv.Name);
                    TextHelper.SetText(this.tabNameDark, csv.Name);
                }
            }
            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke((int)this.tabId, true);
                }
            }
            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }
        
        public Text desc;
        public Transform rewardParent;
        public UI_RewardList rewards;

        private GameObject tabProto;
        private Transform tabProtoParent;
        
        
        protected override void OnLoaded() {
            this.desc = this.transform.Find("ZoneLike/View_Content/Image/Text_Des").GetComponent<Text>();
            this.rewardParent = this.transform.Find("ZoneLike/View_Content/Image/Scroll_View/Viewport");
            
            this.tabProtoParent = this.transform.Find("ZoneLike/View_Content/ScrollView_Menu/List");
            this.tabProto = this.transform.Find("ZoneLike/View_Content/ScrollView_Menu/List/Toggle").gameObject;
        }

        private uint npcId;
        public FavorabilityNPC npc;
        
        public int currentTabId = 0;
        public List<uint> tabIds;
        public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();
        
        protected override void OnOpen(object arg) {
            this.npcId = Convert.ToUInt32(arg);
            
            Sys_NPCFavorability.Instance.TryGetNpc(npcId, out this.npc, false);

            this.tabIds = npc.csvNPCFavorability.BanqueID;

            // 重置数据
            this.currentTabId = 0;
        }
        
        protected override void OnOpened() {
            this.tabVds.BuildOrRefresh<uint>(this.tabProto, this.tabProtoParent, this.tabIds, this.OnRefresgTab);

            // 默认选中Tab
            if (this.tabVds.CorrectId(ref this.currentTabId, this.tabIds)) {
                if (this.tabVds.TryGetVdById(this.currentTabId, out var vd)) {
                    vd.SetSelected(true, true);
                }
            }
        }

        public void OnRefresgTab(Tab vd, uint id, int index) {
            vd.SetUniqueId((int)id);
            vd.SetSelectedAction((innerId, force) => {
                this.currentTabId = innerId;

                RefreshRight();

            });
            vd.Refresh(id);
        }

        private void RefreshRight() {
            if (this.rewards == null) {
                this.rewards = new UI_RewardList(this.rewardParent, EUIID.UI_FavorabilityNpcLikeFood);
            }
            var ls = FavorabilityNPC.GetFoods((uint)this.currentTabId);
            this.rewards.SetRewardList(ls);
            this.rewards.Build(true, false, false, false, false, false, false, true, PropItem.OnClickPropItem, false);
            
            TextHelper.SetText(this.desc, "");
            var csvBanquet = CSVFavorabilityBanquet.Instance.GetConfData((uint)this.currentTabId);
            if (csvBanquet != null) {
                TextHelper.SetText(this.desc, csvBanquet.Des);
            }
        }
    }
}