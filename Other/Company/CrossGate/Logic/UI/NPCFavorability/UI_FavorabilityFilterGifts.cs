using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityFilterGifts_Layout : UILayoutBase {
        public Button btnExit;
        public Button btnSure;

        public Transform itemGroupProto;

        public void Parse(GameObject root) {
            this.Init(root);

            this.btnExit = this.transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            this.btnSure = this.transform.Find("Animator/Button_All").GetComponent<Button>();
            this.itemGroupProto = this.transform.Find("Animator/Scroll_View/View_Choice/Proto");
        }

        public void RegisterEvents(IListener listener) {
            this.btnExit.onClick.AddListener(listener.OnBtnExitClicked);
            this.btnSure.onClick.AddListener(listener.OnBtnSureClicked);
        }

        public interface IListener {
            void OnBtnExitClicked();
            void OnBtnSureClicked();
        }
    }

    // 过滤礼物
    public class UI_FavorabilityFilterGifts : UIBase, UI_FavorabilityFilterGifts_Layout.IListener {
        public class Item : UIComponent {
            public Text itemName;
            public CP_Toggle toggle;

            public uint giftType;
            public UI_FavorabilityFilterGifts ui;

            protected override void Loaded() {
                this.itemName = this.transform.Find("Text").GetComponent<Text>();
                this.toggle = this.transform.Find("itemProto").GetComponent<CP_Toggle>();

                this.toggle.onValueChanged.AddListener(this.OnValueChanged);
            }

            public void Refresh(UI_FavorabilityFilterGifts ui, uint giftType) {
                this.ui = ui;
                this.giftType = giftType;

                CSVFavorabilityGiftType.Data csvGift = CSVFavorabilityGiftType.Instance.GetConfData(giftType);
                if (csvGift != null) {
                    TextHelper.SetText(this.itemName, csvGift.name);
                }
            }

            private void OnValueChanged(bool selected) {
                this.ui.SetSelected(this.giftType, selected);
            }

            public void SetSelected(bool toSelected) {
                this.toggle.SetSelected(toSelected, true);
            }
        }
        public class ItemGroup : UIComponent {
            public Text itemTypeName;
            public Transform itemProto;

            public COWVd<Item> items = new COWVd<Item>();

            public UI_FavorabilityFilterGifts ui;
            public uint giftGroupId;

            protected override void Loaded() {
                itemTypeName = transform.Find("Image_Title/Text_Title").GetComponent<Text>();
                itemProto = transform.Find("SrollItem/Image");
            }

            public void SetSelected(bool toSelected) {
                for (int i = 0, length = this.items.RealCount; i < length; i++) {
                    this.items[i].SetSelected(toSelected);
                }
            }

            public void Refresh(UI_FavorabilityFilterGifts ui, uint giftGroupId) {
                this.ui = ui;
                this.giftGroupId = giftGroupId;

                var ls = Sys_NPCFavorability.Instance.giftGroup[giftGroupId];
                items.TryBuildOrRefresh(itemProto.gameObject, itemProto.parent, ls.Count, (item, index)=> {
                    item.Refresh(ui, ls[index]);
                });

                uint lanId = 0;
                if (giftGroupId == 1) { lanId = 2010641; }
                else if (giftGroupId == 2) { lanId = 2010642; }
                else if (giftGroupId == 3) { lanId = 2010643; }
                else if (giftGroupId == 4) { lanId = 2010644; }
                TextHelper.SetText(itemTypeName, lanId);
            }
        }

        public UI_FavorabilityFilterGifts_Layout Layout = new UI_FavorabilityFilterGifts_Layout();
        public COWVd<ItemGroup> groups = new COWVd<ItemGroup>();

        public HashSet<uint> selectedTypes = new HashSet<uint>();
        public UI_FavorabilitySendGift.SendGiftArg callback;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.callback = arg as UI_FavorabilitySendGift.SendGiftArg;
        }
        protected override void OnShow() {
            this.groups.TryBuildOrRefresh(this.Layout.itemGroupProto.gameObject, this.Layout.itemGroupProto.parent, Sys_NPCFavorability.Instance.giftGroup.Count, (itemGroup, index) => {
                uint key = Sys_NPCFavorability.Instance.giftGroupList[index];
                itemGroup.Refresh(this, key);
            });

            Lib.Core.FrameworkTool.ForceRebuildLayout(gameObject);
        }

        public void SetAllSelected(bool toSelected) {
            for (int i = 0, length = this.groups.RealCount; i < length; i++) {
                this.groups[i].SetSelected(toSelected);
            }
        }
        public void SetSelected(uint giftType, bool toSelected) {
            if (toSelected && !this.selectedTypes.Contains(giftType)) {
                this.selectedTypes.Add(giftType);
            }
            else if (!toSelected && this.selectedTypes.Contains(giftType)) {
                this.selectedTypes.Remove(giftType);
            }
        }

        public void OnBtnExitClicked() {
            this.SetAllSelected(false);
            this.selectedTypes.Clear();
            this.CloseSelf();
        }
        public void OnBtnSureClicked() {
            this.callback.hash = this.selectedTypes;
            this.callback?.Invoke();
            this.CloseSelf();
        }
    }
}