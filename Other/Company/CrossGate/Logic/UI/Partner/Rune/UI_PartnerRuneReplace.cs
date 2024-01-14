using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_RuneReplaceLeft : UIComponent {
        private InfinityGrid infinity;
        private readonly Dictionary<GameObject, UI_Rune_Item> runeDic = new Dictionary<GameObject, UI_Rune_Item>();
        private readonly List<UI_Rune_Item> runeItemList = new List<UI_Rune_Item>();

        private List<UIRuneResultParam> runeList;
        private CSVRuneSlot.Data slotData;
        public uint runeId;
        public uint partnerId;

        public int selectIndex = -1;
        private IListener listener;

        protected override void Loaded() {
            this.infinity = this.transform.Find("Scroll View").GetNeedComponent<InfinityGrid>();

            this.infinity.onCreateCell += this.OnCreateCell;
            this.infinity.onCellChange += this.OnCellChange;
        }
        private void OnCreateCell(InfinityGridCell cell) {
            UI_Rune_Item entry = new UI_Rune_Item();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddListtener(this.OnRuneSelect);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index) {
            var runeItem = cell.mUserData as UI_Rune_Item;
            runeItem.SetRuneData(this.runeList[index], index);
            runeItem.SetSelectStateAndDontSendMessage(this.selectIndex);
        }
        
        private void OnRuneSelect(UI_Rune_Item runeItemSc) {
            this.selectIndex = runeItemSc.index;
            this.listener?.OnSelectRune(runeItemSc.uI_RuneCellBase.runeResultParam.RuneId);
        }

        public override void SetData(params object[] arg) {
            this.selectIndex = -1;
            this.runeId = Convert.ToUInt32(arg[0]);
            this.slotData = arg[1] as CSVRuneSlot.Data;
            this.partnerId = Convert.ToUInt32(arg[2]);

            if (null != this.slotData) {
                this.runeList = Sys_Partner.Instance.GetRuneSureList(this.slotData.slot_type, this.slotData.slot_level, this.partnerId);
                this.infinity.CellCount = this.runeList.Count;
                this.infinity.ForceRefreshActiveCell();
                this.infinity.MoveToIndex(0);
            }
        }

        public void Register(IListener _listener) {
            this.listener = _listener;
        }

        public interface IListener {
            void OnSelectRune(uint runeId);
        }
    }

    public class UI_RuneReplaceRight : UIComponent, UI_RuneReplaceLeft.IListener {
        public class UI_RunePage : UIComponent {
            private Image iconImage;
            private Image runeLevelImage;
            private Text nameText;

            private Transform attrParentTrans;
            private Transform skillParentTrans;
            private GameObject attrTextGo;
            private GameObject skillTextGo;

            public uint runId;

            protected override void Loaded() {
                this.iconImage = this.transform.Find("Skill_Icon").GetComponent<Image>();
                this.nameText = this.transform.Find("Text_Name").GetComponent<Text>();
                this.attrParentTrans = this.transform.Find("Basic_Attr_Group/Attr_Grid");
                this.skillParentTrans = this.transform.Find("Basic_Skill_Group/Attr_Grid");
                this.attrTextGo = this.transform.Find("Basic_Attr_Group/Attr").gameObject;
                this.skillTextGo = this.transform.Find("Basic_Skill_Group/Attr").gameObject;
                this.runeLevelImage = this.transform.Find("Image_RuneRank")?.GetComponent<Image>();
            }

            public override void SetData(params object[] arg) {
                this.runId = Convert.ToUInt32(arg[0]);
                bool hasSkill = false;
                CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData(this.runId);
                if (null != runeInfo) {
                    ImageHelper.SetIcon(this.iconImage, runeInfo.icon);
                    ImageHelper.SetIcon(this.runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(runeInfo.rune_lvl));
                    this.runeLevelImage.gameObject.SetActive(true);
                    TextHelper.SetText(this.nameText, runeInfo.rune_name);
                    FrameworkTool.DestroyChildren(this.attrParentTrans.gameObject);
                    if (runeInfo.rune_attribute.Count >= 2) {
                        //防止符文属性配置为多属性
                        int num = runeInfo.rune_attribute.Count / 2;
                        for (int k = 0; k < num; k++) {
                            uint attrId = runeInfo.rune_attribute[k];
                            uint attrValue = runeInfo.rune_attribute[k + 1];
                            CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(attrId);
                            if (null != attrInfo) {
                                GameObject go = GameObject.Instantiate<GameObject>(this.attrTextGo, this.attrParentTrans);
                                TextHelper.SetText(go.GetComponent<Text>(), attrInfo.name);
                                TextHelper.SetText(go.transform.Find("Text_Number").GetComponent<Text>(), attrInfo.show_type == 1 ? attrValue.ToString() : attrValue / 100.0f + "%");
                                go.SetActive(true);
                            }
                        }
                        this.attrParentTrans.parent.gameObject.SetActive(num > 0);
                    }
                    FrameworkTool.DestroyChildren(this.skillParentTrans.gameObject);
                    if (runeInfo.rune_passiveskillID != 0) {
                        CSVPassiveSkillInfo.Data passInfo = CSVPassiveSkillInfo.Instance.GetConfData(runeInfo.rune_passiveskillID);
                        if (null != passInfo) {
                            if (!hasSkill)
                                hasSkill = true;
                            GameObject go = GameObject.Instantiate<GameObject>(this.skillTextGo, this.skillParentTrans);
                            TextHelper.SetText(go.GetComponent<Text>(), passInfo.desc);
                            go.SetActive(true);
                        }
                    }
                    this.skillParentTrans.transform.parent.gameObject.SetActive(runeInfo.rune_passiveskillID != 0);
                    this.gameObject.SetActive(true);
                }
                else {
                    this.gameObject.SetActive(false);
                }
            }
        }

        private Button btnSure;

        private GameObject leftNoneGo;
        private GameObject rightNoneGo;

        private GameObject allNoneGo;
        private GameObject tipGo;

        private GameObject arrowGo;

        private UI_RunePage leftRunePage;
        private UI_RunePage rightRunePage;
        private uint leftRuneId = 0;
        private uint rightRuneId = 0;
        private uint partnerId;

        private CSVRuneSlot.Data csvLeftSlot;

        protected override void Loaded() {
            this.btnSure = this.transform.Find("BtnSure").GetComponent<Button>();
            this.btnSure.onClick.AddListener(this.OnBtnSureClicked);

            this.leftRunePage = new UI_RunePage();
            this.leftRunePage.Init(this.transform.Find("Rune01"));
            this.rightRunePage = new UI_RunePage();
            this.rightRunePage.Init(this.transform.Find("Rune02"));

            this.leftNoneGo = this.transform.Find("Text_LeftNone").gameObject;
            this.rightNoneGo = this.transform.Find("Text_RightNone").gameObject;
            this.allNoneGo = this.transform.Find("Text_AllNone").gameObject;
            this.tipGo = this.transform.Find("Image_Tips").gameObject;
            this.arrowGo = this.transform.Find("Image_Arrow").gameObject;
        }
        public override void OnDestroy() {
            this.leftRunePage?.OnDestroy();
            this.rightRunePage?.OnDestroy();
            base.OnDestroy();
        }

        // 左侧选中一个rune
        public void OnSelectRune(uint runeId) {
            this.rightRuneId = runeId;
            this.Refresh();
        }

        private void OnBtnSureClicked() {
            Sys_Partner.Instance.PartnerRuneDressReq(this.partnerId, this.rightRuneId, this.csvLeftSlot.slot_category - 1, this.csvLeftSlot.slot_sequence - 1);
        }

        public override void SetData(params object[] arg) {
            // reset
            this.leftRuneId = 0;
            this.rightRuneId = 0;

            this.leftRuneId = Convert.ToUInt32(arg[0]);
            this.csvLeftSlot = arg[1] as CSVRuneSlot.Data;
            this.partnerId = Convert.ToUInt32(arg[2]);

            this.Refresh();
        }

        private new void Refresh() {
            if (null != this.csvLeftSlot) {
                this.tipGo.SetActive(this.csvLeftSlot.slot_type == 9);
            }

            this.NoneStateCtrl();
            this.leftRunePage.SetData(this.leftRuneId);
            this.rightRunePage.SetData(this.rightRuneId);

            ButtonHelper.Enable(this.btnSure, this.rightRuneId != 0);
        }

        private void NoneStateCtrl() {
            this.leftNoneGo.SetActive(this.leftRuneId == 0 && !(this.leftRuneId == 0 && this.rightRuneId == 0));
            this.rightNoneGo.SetActive(this.rightRuneId == 0 && !(this.leftRuneId == 0 && this.rightRuneId == 0));
            this.allNoneGo.SetActive(this.leftRuneId == 0 && this.rightRuneId == 0);
            this.arrowGo.SetActive(this.leftRuneId != 0 || this.rightRuneId != 0);
        }
    }

    public class UI_PartnerRuneReplace : UIBase {
        private Button btnClose;

        private UI_RuneReplaceLeft leftAttr;
        private UI_RuneReplaceRight rightAttr;

        private uint partnerId;
        private uint leftRuneId;
        private CSVRuneSlot.Data csvRuneSlot;

        protected override void OnLoaded() {
            this.btnClose = this.transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            this.btnClose.onClick.AddListener(this.OnCloseBtnClicked);

            this.rightAttr = new UI_RuneReplaceRight();
            this.rightAttr.Init(this.transform.Find("Animator/View_RuneEquip/View_Right"));

            this.leftAttr = new UI_RuneReplaceLeft();
            this.leftAttr.Init(this.transform.Find("Animator/View_RuneEquip/View_Left"));
            this.leftAttr.Register(this.rightAttr);
        }

        protected override void OnDestroy() {
            this.leftAttr?.OnDestroy();
            this.rightAttr?.OnDestroy();            
        }

        protected override void OnOpen(object arg) {
            Tuple<uint, uint, CSVRuneSlot.Data> tp = arg as Tuple<uint, uint, CSVRuneSlot.Data>;
            if (tp != null) {
                this.partnerId = Convert.ToUInt32(tp.Item1);
                this.leftRuneId = Convert.ToUInt32(tp.Item2);
                this.csvRuneSlot = tp.Item3 as CSVRuneSlot.Data;
            }
        }

        protected override void OnShow() {
            this.leftAttr.SetData(this.leftRuneId, this.csvRuneSlot, this.partnerId);
            this.rightAttr.SetData(this.leftRuneId, this.csvRuneSlot, this.partnerId);
        }

        private void OnCloseBtnClicked() {
            this.CloseSelf();
        }

        #region 事件处理
        protected override void ProcessEvents(bool toRegister) {
            Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnRuneDressCallBack, this.OnRuneDressCallBack, toRegister);
        }

        // 装载成功
        private void OnRuneDressCallBack(uint pos) {
            this.CloseSelf();
        }
        #endregion

        #region 接口实现
        #endregion
    }
}

