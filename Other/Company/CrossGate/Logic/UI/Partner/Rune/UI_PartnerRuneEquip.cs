using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic {
    public class UI_Rune_Item {
        public Transform transform;
        public UI_RuneCellBase uI_RuneCellBase;


        private CP_Toggle toggle;
        public Action<UI_Rune_Item> action;
        public int index;
        public void BindGameObject(GameObject go) {
            this.transform = go.transform;
            this.uI_RuneCellBase = new UI_RuneCellBase();
            this.uI_RuneCellBase.BindGameObject(go);
            this.toggle = this.transform.GetComponent<CP_Toggle>();
            this.toggle.onValueChanged.AddListener(this.OnToggleClicked);
        }

        private void OnToggleClicked(bool isOn) {
            if (isOn)
                this.action?.Invoke(this);
        }

        public void SetRuneData(UIRuneResultParam runeResultParam, int index) {
            this.uI_RuneCellBase.SetRuneData(runeResultParam);
            this.index = index;
        }

        public void AddListtener(Action<UI_Rune_Item> action) {
            this.action = action;
        }

        public void Refresh() {
            this.uI_RuneCellBase.Refresh();
        }

        public void SetSelectStateAndDontSendMessage(int index) {
            bool isSelect = this.index == index;
            this.toggle.SetSelected(isSelect, false);
        }
    }

    // 伙伴列表
    public class UIPartnerHeadList : UIComponent {
        private class PartnerHead : UIComponent {
            private CP_Toggle _toggle;
            private Image _imgIcon;
            private Image _imgFight;
            private GameObject _redDotGo;

            private uint _partnerId;
            private Action<uint> _action;

            protected override void Loaded() {
                this._toggle = this.transform.GetComponent<CP_Toggle>();
                this._toggle.onValueChanged.AddListener(this.OnClickToggle);

                this._redDotGo = this.transform.Find("Image_Dot").gameObject;
                
                this._imgIcon = this.transform.Find("Icon").GetComponent<Image>();
                this._imgFight = this.transform.Find("Imag_Fight").GetComponent<Image>();
            }

            private void OnClickToggle(bool isOn) {
                if (isOn) {
                    this._action?.Invoke(this._partnerId);
                }
            }

            public void Register(Action<uint> action) {
                this._action = action;
            }

            public void UpdateInfo(uint partnerId) {
                this._partnerId = partnerId;

                CSVPartner.Data data = CSVPartner.Instance.GetConfData(partnerId);
                ImageHelper.SetIcon(this._imgIcon, data.battle_headID);

                bool has = Sys_Partner.Instance.HasNew(this._partnerId);
                this._redDotGo.SetActive(has);

                this._imgFight.gameObject.SetActive(Sys_Partner.Instance.IsInForm(this._partnerId));
            }
            public void SetSelected(bool toSelected) {
                this._toggle.SetSelected(toSelected, true);
            }
        }

        //public InfinityGridLayoutGroup gridGroup;
        private readonly Dictionary<GameObject, PartnerHead> dicCells = new Dictionary<GameObject, PartnerHead>();
        //private int visualGridCount;
        public InfinityGrid _infinityGrid;

        private IListener _listener;
        private uint selectedId = 0;
        public List<uint> listIds = new List<uint>();

        protected override void Loaded()
        {
            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }
        public override void OnDestroy() {
            foreach (var item in this.dicCells) {
                item.Value.OnDestroy();
            }
            base.OnDestroy();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            PartnerHead entry = new PartnerHead();
            entry.Init(cell.mRootTransform);
            entry.Register(OnSelectParnter);

            cell.BindUserData(entry);

            dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PartnerHead entry = cell.mUserData as PartnerHead;
            entry.UpdateInfo(listIds[index]);
            entry.SetSelected(this.listIds[index] == this.selectedId);
        }
        
        private void OnSelectParnter(uint partnerId) {
            this._listener?.OnSelectParnter(partnerId);
        }

        public void Register(IListener listener) {
            this._listener = listener;
        }

        public void UpdateInfo(uint infoId) {
            
            //this.visualGridCount = this.listIds.Count;
            //this.gridGroup.SetAmount(this.visualGridCount);
            
        }

        public void UpdateInfo(List<uint> list, uint selectedId)
        {
            Sys_Partner.Instance.SelectPartnerId = this.selectedId = selectedId;
            this.listIds = list;
            _infinityGrid.CellCount = listIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }

        public void ForceRefreshActiveCell(uint selectedId) {
            Sys_Partner.Instance.SelectPartnerId = this.selectedId = selectedId;
            
            if (this._infinityGrid) {
                _infinityGrid.ForceRefreshActiveCell();
            }
        }

        public void MoveTo(uint id) {
            int index = this.listIds.IndexOf(id);
            if (index != -1) {
                _infinityGrid.MoveToIndex(index);
            }
        }

        public interface IListener {
            void OnSelectParnter(uint newId);
        }
    }

    // 符文树中的单个槽
    public class UIRuneSlot : UIComponent {
        private readonly List<GameObject> bgGoList = new List<GameObject>();
        private readonly List<Image> levelGoList = new List<Image>();
        public GameObject selectImageGo;
        private GameObject lineGo;
        private GameObject addGo;
        private GameObject lock2;
        private Image runeImage;
        private Image runeLevelImage;
        private Text LevelText;
        private Image skillImage;
        private Image sliderIcon;
        private GameObject redDotGo;

        private Text seqText;

        public CSVRuneSlot.Data csvRuneSlot;
        public Action<UIRuneSlot> action;
        public uint partnerId;
        public uint runeId;
        // 807  red
        // 808  blue

        protected override void Loaded() {
            this.seqText = this.transform.Find("Text").GetComponent<Text>();

            this.bgGoList.Add(this.transform.Find("Image_Bg_Yellow").gameObject);
            this.bgGoList.Add(this.transform.Find("Image_Bg_Red").gameObject);
            this.bgGoList.Add(this.transform.Find("Image_Bg_Blue").gameObject);
            this.bgGoList.Add(this.transform.Find("Image_Bg_Purple").gameObject);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(this.transform.Find("Image_Bg_Red").GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerClick, (BaseEventData eventData) => {
                this.action?.Invoke(this);
            });

            eventListener = Lib.Core.EventTrigger.Get(this.transform.Find("Image_Bg_Blue").GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerClick, (BaseEventData eventData) => {
                this.action?.Invoke(this);
            });

            eventListener = Lib.Core.EventTrigger.Get(this.transform.Find("Image_Bg_Yellow").GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerClick, (BaseEventData eventData) => {
                this.action?.Invoke(this);
            });

            eventListener = Lib.Core.EventTrigger.Get(this.transform.Find("Image_Bg_Purple").GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerClick, (BaseEventData eventData) => {
                this.action?.Invoke(this);
            });

            this.levelGoList.Add(this.transform.Find("Image_Write04").GetComponent<Image>());
            this.levelGoList.Add(this.transform.Find("Image_Write01").GetComponent<Image>());
            this.levelGoList.Add(this.transform.Find("Image_Write02").GetComponent<Image>());
            this.levelGoList.Add(this.transform.Find("Image_Write03").GetComponent<Image>());

            this.redDotGo = this.transform.Find("Image_Dot")?.gameObject;
            
            this.selectImageGo = this.transform.Find("Image_Select")?.gameObject;
            this.lineGo = this.transform.Find("Image_Line01")?.gameObject;
            this.runeImage = this.transform.Find("Image_Rune").GetComponent<Image>();
            this.runeLevelImage = this.transform.Find("Image_RuneRank")?.GetComponent<Image>();

            this.LevelText = this.transform.Find("Image_Lock/GameObject").GetComponent<Text>();
            this.sliderIcon = this.transform.Find("Image_Slider").GetComponent<Image>();

            this.addGo = this.transform.Find("Image_Skill/Image_Add")?.gameObject;
            this.lock2 = this.transform.Find("Image_Skill/Image_Lock01")?.gameObject;
            this.skillImage = this.transform.Find("Image_Skill").GetComponent<Image>();
            this.skillImage.GetComponent<Button>().onClick.AddListener(this.OnClick);
        }
        
        public void Refresh(CSVRuneSlot.Data slotData, uint partnerId, uint runeId) {
            this.partnerId = partnerId;
            this.csvRuneSlot = slotData;
            this.runeId = runeId;
            //this.runeId = 422101u;

            if (null != slotData) {
                bool isRune = slotData.slot_type != 9;
                this.runeImage.gameObject.SetActive(isRune && this.runeId != 0);
                this.runeLevelImage?.gameObject.SetActive(isRune && this.runeId != 0);
                this.lineGo?.SetActive(this.runeId != 0);
                this.addGo?.gameObject.SetActive(false);
                this.redDotGo.gameObject.SetActive(false);

                this.skillImage.gameObject.SetActive(!isRune);
                this.sliderIcon.gameObject.SetActive(!isRune);

                if (!isRune) {
                    List<uint> data = Sys_Partner.Instance.GetRuneSkillData(partnerId, (int)slotData.slot_category - 1, (int)slotData.slot_sequence);
                    if (data.Count >= 3) {
                        CSVPassiveSkillInfo.Data skillData = CSVPassiveSkillInfo.Instance.GetConfData(data[0]);
                        if (null != skillData) {
                            ImageHelper.SetIcon(this.skillImage, skillData.icon);
                        }

                        // 前置符文是否镶嵌满
                        bool isType = data[1] == 1;// 1 额外符文 0 前置镶嵌满即可
                        uint skillRuneId = Sys_Partner.Instance.GetRuneId(this.partnerId, (int)this.csvRuneSlot.slot_category - 1, (int)this.csvRuneSlot.slot_sequence - 1);
                        // 解锁
                        bool isUnlock = Sys_Partner.Instance.GetRuneSkillUnlock(this.partnerId, (ERuneType)this.csvRuneSlot.slot_category - 1, this.csvRuneSlot.slot_unlocknum);
                        // 激活
                        bool isActive = (!isType) || (isType && runeId != 0);
                        // data[1] == 0表示解锁就激活
                        // data[1] == 1表示解锁之后还需要镶嵌一个符文才能激活

                        ImageHelper.SetImageGray(this.skillImage, !isActive, true);
                        this.lock2?.gameObject.SetActive(!isUnlock);

                        int equipedCount = Sys_Partner.Instance.GetEquipedRuneCount(this.partnerId, (ERuneType)this.csvRuneSlot.slot_category - 1);
                        this.sliderIcon.fillAmount = 1f * equipedCount / this.csvRuneSlot.slot_unlocknum;

                        if (!isUnlock) {
                        }
                        else {
                            if (!isActive) {
                                // 激活状态下， 还需要runeId != 0才能让按钮显示出来
                                // bool activeAndHasValidRune = isType && runeId != 0;
                            }
                            else {

                            }
                        }
                    }
                }
                else {
                    if (this.runeId != 0) {
                        CSVRuneInfo.Data showRuneData = CSVRuneInfo.Instance.GetConfData(this.runeId);
                        if (null != showRuneData) {
                            ImageHelper.SetIcon(this.runeImage, showRuneData.icon);
                            ImageHelper.SetIcon(this.runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(showRuneData.rune_lvl));
                            this.runeLevelImage.gameObject.SetActive(true);
                        }
                    }
                    else {
                        bool has = Sys_Partner.Instance.HasNew(partnerId, slotData.slot_category - 1, slotData.slot_sequence - 1);
                        this.redDotGo.gameObject.SetActive(has);
                    }
                }

                for (int i = 0; i < this.bgGoList.Count; i++) {
                    if (isRune) {
                        this.bgGoList[i].gameObject.SetActive(i == (slotData.slot_type));
                    }
                    else {
                        this.bgGoList[i].gameObject.SetActive(false);
                    }
                }

                for (int i = 0; i < this.levelGoList.Count; i++) {
                    //if (i == 0) {
                    //    bool isShow = (slotData.slot_type == 0 && i == 0);
                    //    this.levelGoList[i].gameObject.SetActive(isShow);
                    //    if (isShow) {
                    //        this.levelGoList[i].color = Sys_Partner.Instance.SetLineColor((int)slotData.slot_type);
                    //    }
                    //}
                    //else 
                    {
                        bool isShow = (slotData.slot_level == i/* && slotData.slot_type != 0*/);
                        this.levelGoList[i].gameObject.SetActive(isShow);
                        if (isShow) {
                            this.levelGoList[i].color = Sys_Partner.Instance.SetLineColor((int)slotData.slot_type);
                        }
                    }
                }

                TextHelper.SetText(this.LevelText, LanguageHelper.GetTextContent(2006112, slotData.slot_unlocklevel.ToString()));
                Partner partner = Sys_Partner.Instance.GetPartnerInfo(partnerId);
                if (null != partner) {
                    this.LevelText.transform.parent.gameObject.SetActive(partner.Level < slotData.slot_unlocklevel);
                }
                else {
                    this.LevelText.transform.parent.gameObject.SetActive(true);
                }
                this.selectImageGo?.SetActive(false);
            }
        }

        public void AddListenter(Action<UIRuneSlot> action) {
            this.action = action;
        }

        public void SelectState(bool select) {
            this.selectImageGo?.SetActive(select);
        }

        private void OnClick() {
            this.action?.Invoke(this);
        }
    }

    // 符文树
    public class UIRuneSlotTree : UIComponent {
        // 不能clear
        private Dictionary<int, List<CSVRuneSlot.Data>> dicLineSlot = new Dictionary<int, List<CSVRuneSlot.Data>>();

        // index:vd
        private readonly Dictionary<int, UIRuneSlot> ceilDic = new Dictionary<int, UIRuneSlot>();
        readonly List<UIRuneSlot> ceilList = new List<UIRuneSlot>();

        private List<PartnerRunePos> partnerRunePos;
        private uint partnerId;
        private ERuneType viewType = ERuneType.Battle;
        private IListener listener;

        private GameObject runeSelectFx;
        private GameObject runeDressFx;
        private GameObject skillDressFx;

        protected override void Loaded() {
            Transform ceilTrans = this.transform.Find("Group_Item");

            this.runeSelectFx = ceilTrans.Find("Fx_ui_item04").gameObject;
            this.runeDressFx = ceilTrans.Find("Fx_ui_item03").gameObject;
            this.skillDressFx = ceilTrans.Find("Fx_ui_item03_big").gameObject;

            this.InitLeftSlot();
            for (int i = 0; i < ceilTrans.childCount; i++) {
                GameObject go = ceilTrans.Find($"Item{i}")?.gameObject;
                if (null != go) {
                    UIRuneSlot ceil = new UIRuneSlot();
                    ceil.Init(go.transform);
                    ceil.AddListenter(this.OnSlotRuneClick);
                    this.ceilList.Add(ceil);
                    this.ceilDic.Add(i, ceil);
                }
            }
        }

        private void InitLeftSlot() {
            this.dicLineSlot = Sys_Partner.Instance.dicLineSlot;
        }

        private void OnSlotRuneClick(UIRuneSlot ceilData) {
            Partner partner = Sys_Partner.Instance.GetPartnerInfo(this.partnerId);
            bool isUnlock = null != partner && partner.Level >= ceilData.csvRuneSlot.slot_unlocklevel;
            if (ceilData.csvRuneSlot.slot_type != 9) {
                List<int> keyList = new List<int>(this.ceilDic.Keys);
                for (int i = 0; i < keyList.Count; i++) {
                    int key = keyList[i];
                    UIRuneSlot value = this.ceilDic[key];
                    bool showSelect = (int)ceilData.csvRuneSlot.slot_sequence - 1 == key && isUnlock;
                    if (showSelect) {
                        this.FxPostionSet(value.selectImageGo.transform, true);
                    }
                    value.SelectState(showSelect);
                }
            }
            else {
                for (int i = 0; i < this.ceilList.Count; i++) {
                    this.ceilList[i].SelectState(false);
                }
            }

            this.listener?.OnClickRuneCeil(ceilData);
        }

        private Timer skillDressFxTimer;
        private Timer runeDressFxTimer;
        public void SetDrPostion(int pos) {
            if (pos <= this.dicLineSlot[(int)this.viewType].Count) {
                CSVRuneSlot.Data cSVRuneSlotData = this.dicLineSlot[(int)this.viewType][pos];
                if (cSVRuneSlotData.slot_type == 9) {
                    this.skillDressFx.gameObject.SetActive(false);
                    this.skillDressFx.transform.localPosition = this.ceilDic[pos].transform.localPosition;
                    this.skillDressFx.gameObject.SetActive(true);

                    this.skillDressFxTimer?.Cancel();
                    this.skillDressFxTimer = Timer.Register(0.5f, () => {
                        this.skillDressFx.gameObject.SetActive(false);
                    });
                }
                else {
                    this.runeDressFx.gameObject.SetActive(false);
                    this.runeDressFx.transform.localPosition = this.ceilDic[pos].transform.localPosition;
                    this.runeDressFx.gameObject.SetActive(true);

                    this.runeDressFxTimer?.Cancel();
                    this.runeDressFxTimer = Timer.Register(0.5f, () => {
                        this.runeDressFx.gameObject.SetActive(false);
                    });
                }
            }
        }

        public void ClearSelectState() {
            for (int i = 0; i < this.ceilList.Count; i++) {
                this.ceilList[i].SelectState(false);
            }
        }

        private void FxPostionSet(Transform parent, bool isRune) {
            if (isRune) {
                this.runeSelectFx.transform.SetParent(parent);
                this.runeSelectFx.transform.localPosition = new Vector3(-parent.localPosition.x, -parent.localPosition.y, 0);
                this.runeSelectFx.gameObject.SetActive(true);
            }
        }

        public override void SetData(params object[] arg) {
            this.partnerId = Convert.ToUInt32(arg[0]);
            this.partnerRunePos = Sys_Partner.Instance.GetPartnerRunePosByPartnerId(this.partnerId);
            this.SlotReset(Convert.ToUInt32(arg[1]));
        }

        public void SlotReset(uint viewType) {
            //重置格子与线条
            this.viewType = (ERuneType)viewType;
            if (this.partnerRunePos.Count >= 2) {
                for (int i = 0; i < this.partnerRunePos[(int)this.viewType].RuneId.Count; i++) {
                    if (null != this.dicLineSlot[(int)this.viewType] && null != this.ceilDic[i]) {
                        this.ceilDic[i].Refresh(this.dicLineSlot[(int)this.viewType][i], this.partnerId, this.partnerRunePos[(int)this.viewType].RuneId[i]);
                    }
                }
            }
            else {
                for (int i = 0; i < this.dicLineSlot[(int)this.viewType].Count; i++) {
                    if (null != this.dicLineSlot[(int)this.viewType] && null != this.ceilDic[i]) {
                        this.ceilDic[i].Refresh(this.dicLineSlot[(int)this.viewType][i], this.partnerId, 0);
                    }
                }
            }
        }

        public void Register(IListener _listener) {
            this.listener = _listener;
        }

        public override void OnDestroy() {
            this.skillDressFxTimer?.Cancel();
            this.runeDressFxTimer?.Cancel();

            base.OnDestroy();
        }

        public interface IListener {
            void OnClickRuneCeil(UIRuneSlot ceilData);
        }
    }

    // 全部符文信息
    public class UI_PartnerAllRuneAttr : UIComponent {
        private Transform attrParentTrans;
        private Transform skillParentTrans;
        private GameObject attrTextGo;
        private GameObject skillTextGo;
        private GameObject noneRuneGo;

        private Button btnUnloadAll;
        private Button btnLoadAll;
        private IListener listener;

        public uint partnerId;

        public interface IListener {
            void OnBtnUnloadAllRuneClicked();
            void OnBtnLoadAllRuneClicked();
        }

        public void Register(IListener listener) {
            this.listener = listener;
        }

        protected override void Loaded() {
            this.attrParentTrans = this.transform.Find("Basic_Attr_Group/Attr_Grid");
            this.skillParentTrans = this.transform.Find("Basic_Skill_Group/Skill_Grid");
            this.attrTextGo = this.transform.Find("Basic_Attr_Group/Attr").gameObject;
            this.skillTextGo = this.transform.Find("Basic_Skill_Group/Attr").gameObject;

            this.noneRuneGo = this.transform.Find("Text_AllNone").gameObject;

            this.btnUnloadAll = this.transform.Find("UnloadAll").GetComponent<Button>();
            this.btnUnloadAll.onClick.AddListener(() => {
                if (this.partnerId != 0)
                    this.listener?.OnBtnUnloadAllRuneClicked();
            });
            
            this.btnLoadAll = this.transform.Find("Btn_InlayAll").GetComponent<Button>();
            this.btnLoadAll.onClick.AddListener(() => {
                if (this.partnerId != 0)
                    this.listener?.OnBtnLoadAllRuneClicked();
            });
        }

        public void SetData(uint id) {
            this.partnerId = id;
            
            this.btnUnloadAll.enabled = false;
            this.btnLoadAll.enabled = false;

            if (this.partnerId != 0) {
                Dictionary<uint, uint> runeAttrDic = Sys_Partner.Instance.GetPartnerRuneAttrByPartnerId(this.partnerId);
                FrameworkTool.DestroyChildren(this.skillParentTrans.gameObject);
                FrameworkTool.DestroyChildren(this.attrParentTrans.gameObject);
                
                bool canUnload = (runeAttrDic.Count > 0);
                ButtonHelper.Enable(this.btnUnloadAll, canUnload);
                
                // 20个符文槽
                List<CmdPartnerRuneDressOneKeyReq.Types.RuneList> ls = new List<CmdPartnerRuneDressOneKeyReq.Types.RuneList>(2);
                Sys_Partner.Instance.GetUnloadedRunes(this.partnerId, ls, null);
                bool canLoad = (ls[0].Runes.Count > 0 || ls[1].Runes.Count > 0);
                ButtonHelper.Enable(this.btnLoadAll, canLoad);

                bool hasSkill = false;
                List<uint> keyList = new List<uint>(runeAttrDic.Keys);
                for (int i = 0; i < keyList.Count; i++) {
                    uint key = keyList[i];
                    uint value = runeAttrDic[key];
                    if (key > 3000000) {
                        CSVPassiveSkillInfo.Data passInfo = CSVPassiveSkillInfo.Instance.GetConfData(key);
                        if (null != passInfo) {
                            if (!hasSkill)
                                hasSkill = true;
                            GameObject go = GameObject.Instantiate<GameObject>(this.skillTextGo, this.skillParentTrans);
                            TextHelper.SetText(go.GetComponent<Text>(), passInfo.desc);
                            go.SetActive(true);
                        }
                    }
                    else {
                        CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(key);
                        if (null != attrInfo) {
                            GameObject go = GameObject.Instantiate<GameObject>(this.attrTextGo, this.attrParentTrans);
                            TextHelper.SetText(go.GetComponent<Text>(), attrInfo.name);
                            TextHelper.SetText(go.transform.Find("Text_Number").GetComponent<Text>(), LanguageHelper.GetTextContent(2006142u, attrInfo.show_type == 1 ? value.ToString() : value / 100.0f + "%"));
                            go.SetActive(true);
                        }
                    }
                }

                this.noneRuneGo.SetActive(runeAttrDic.Count < 1);
                this.skillParentTrans.parent.gameObject.SetActive(hasSkill);
            }
        }
    }

    // 空槽符文信息
    public class UI_PartnerEmptyRuneAttr : UIComponent, UI_RuneReplaceLeft.IListener {
        private UI_RuneReplaceLeft replaceList;
        public Button btnSure;

        private IListener listener;
        private uint selectedRuneId;

        protected override void Loaded() {
            this.replaceList = new UI_RuneReplaceLeft();
            this.replaceList.Init(this.transform);
            this.replaceList.Register(this);

            this.btnSure = this.transform.Find("BtnSure").GetComponent<Button>();
            this.btnSure.onClick.AddListener(this.OnBtnSureClicked);

            Button btnBack = this.transform.Find("Btn_Back").GetComponent<Button>();
            btnBack.onClick.AddListener(this.OnBtnBackClicked);
        }
        public override void OnDestroy() {
            this.replaceList.OnDestroy();
            base.OnDestroy();
        }

        private void OnBtnSureClicked() {
            this.listener?.OnBtnSureChangeClicked(this.selectedRuneId);
        }
        private void OnBtnBackClicked() {
            this.listener?.OnBtnBackClicked();
        }

        public override void SetData(params object[] arg) {
            this.btnSure.gameObject.SetActive(false);
            this.selectedRuneId = 0;

            this.replaceList.SetData(arg);
        }

        public void Register(IListener listener) {
            this.listener = listener;
        }

        public void OnSelectRune(uint runeId) {
            this.selectedRuneId = runeId;
            this.btnSure.gameObject.SetActive(this.replaceList.selectIndex != -1 && runeId != 0);
        }

        public interface IListener {
            void OnBtnSureChangeClicked(uint runeId);
            void OnBtnBackClicked();
        }
    }

    // 已经装配槽符文信息
    public class UI_PartnerEquipRuneAttr : UIComponent {
        public class UIRuneAttr : UIComponent {
            public Text name;
            public Text number;

            protected override void Loaded() {
                this.name = this.gameObject.GetComponent<Text>();
                this.number = this.transform.Find("Text_Number").GetComponent<Text>();
            }
            public void Refresh(uint attrId, uint attrValue) {
                CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(attrId);
                if (null != attrInfo) {
                    TextHelper.SetText(this.name, attrInfo.name);
                    string content = LanguageHelper.GetTextContent(2006142u, attrInfo.show_type == 1 ? attrValue.ToString() : attrValue / 100.0f + "%");
                    TextHelper.SetText(this.number, content);
                }
            }
            public override void OnDestroy() {
                base.OnDestroy();
            }
        }
        private Image iconImg;
        private Image levelImg;
        private Text nameText;

        private readonly COWVd<UIRuneAttr> runeAttrGroup = new COWVd<UIRuneAttr>();
        private readonly COWComponent<Text> skillAttrGroup = new COWComponent<Text>();

        private Button btnReplace;
        private Button btnUnload;

        private Transform attrParentTrans;
        private Transform skillParentTrans;

        private GameObject attrTextGo;
        private GameObject skillTextGo;

        public uint partnerId;
        public CSVRuneSlot.Data csvRuneSlot;
        public CSVRuneInfo.Data csvRuneInfo;
        public CSVPassiveSkillInfo.Data csvPassInfo;

        private IListener listener;

        protected override void Loaded() {
            this.iconImg = this.transform.Find("RuneIcon").GetComponent<Image>();
            this.levelImg = this.transform.Find("Image_RuneRank")?.GetComponent<Image>();
            this.nameText = this.transform.Find("Text_Name").GetComponent<Text>();

            this.btnUnload = this.transform.Find("BtnUnload").GetComponent<Button>();
            this.btnUnload.onClick.AddListener(() => {
                this.listener?.OnBtnUnloadClicked(this.csvRuneSlot);
            });

            this.btnReplace = this.transform.Find("BtnReplace").GetComponent<Button>();
            this.btnReplace.onClick.AddListener(() => {
                this.listener?.OnBtnReplaceClicked(this.csvRuneSlot);
            });

            Button btn = this.transform.Find("Btn_Back").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnBackClicked);

            this.attrParentTrans = this.transform.Find("Property/Basic_Attr_Group/Attr_Grid");
            this.skillParentTrans = this.transform.Find("Property/Basic_Skill_Group/Skill_Grid");

            this.attrTextGo = this.transform.Find("Property/Basic_Attr_Group/Attr").gameObject;
            this.skillTextGo = this.transform.Find("Property/Basic_Skill_Group/Attr").gameObject;
        }

        public override void OnDestroy() {
            this.runeAttrGroup.ForEach((ele) => { ele.OnDestroy(); });
            this.runeAttrGroup.Clear();

            this.skillAttrGroup.Clear();
            base.OnDestroy();
        }

        public override void SetData(params object[] arg) {
            if (null != arg && arg.Length > 0) {
                this.partnerId = Convert.ToUInt32(arg[0]);
                this.csvRuneSlot = arg[1] as CSVRuneSlot.Data;
                this.SetAttr();
            }
        }

        private void SetAttr() {
            if (null != this.csvRuneSlot && this.partnerId != 0) {
                bool isPassView = this.csvRuneSlot.slot_type == 9;
                if (!isPassView) {
                    uint runeId = Sys_Partner.Instance.GetRuneId(this.partnerId, (int)this.csvRuneSlot.slot_category - 1, (int)this.csvRuneSlot.slot_sequence - 1);
                    this.csvRuneInfo = CSVRuneInfo.Instance.GetConfData(runeId);

                    this.skillParentTrans.parent.gameObject.SetActive(false);
                    this.attrParentTrans.parent.gameObject.SetActive(false);

                    if (null != this.csvRuneInfo) {
                        ImageHelper.SetIcon(this.iconImg, this.csvRuneInfo.icon);
                        ImageHelper.SetIcon(this.levelImg, Sys_Partner.Instance.GetRuneLevelImageId(this.csvRuneInfo.rune_lvl));
                        TextHelper.SetText(this.nameText, this.csvRuneInfo.rune_name);

                        // 符文属性处理
                        if (this.csvRuneInfo.rune_attribute.Count >= 2) {
                            //防止符文属性配置为多属性
                            int num = this.csvRuneInfo.rune_attribute.Count / 2;
                            this.runeAttrGroup.TryBuildOrRefresh(this.attrTextGo, this.attrParentTrans, num, this.OnRuneAttrRefresh);
                            this.attrParentTrans.parent.gameObject.SetActive(num > 0);
                        }

                        // 技能属性处理
                        if (this.csvRuneInfo.rune_passiveskillID != 0) {
                            this.csvPassInfo = CSVPassiveSkillInfo.Instance.GetConfData(this.csvRuneInfo.rune_passiveskillID);
                            if (null != this.csvPassInfo) {
                                this.skillParentTrans.parent.gameObject.SetActive(true);
                                this.skillAttrGroup.TryBuildOrRefresh(this.skillTextGo, this.skillParentTrans, 1, this.OnSkillAttrRefresh);
                            }
                        }
                    }
                }
            }
        }

        private void OnRuneAttrRefresh(UIRuneAttr vd, int index) {
            uint attrId = this.csvRuneInfo.rune_attribute[index];
            uint attrValue = this.csvRuneInfo.rune_attribute[index + 1];
            vd.Refresh(attrId, attrValue);
        }
        private void OnSkillAttrRefresh(Text text, int index) {
            TextHelper.SetText(text, this.csvPassInfo.desc);
        }
        private void OnBtnBackClicked() {
            this.listener?.OnBtnBackClicked();
        }

        public void Register(IListener listener) {
            this.listener = listener;
        }

        public interface IListener {
            void OnBtnReplaceClicked(CSVRuneSlot.Data slotData);
            void OnBtnUnloadClicked(CSVRuneSlot.Data slotData);
            void OnBtnBackClicked();
        }
    }

    public class PropItemLayout {
        public Transform transform;

        public Image icon;
        public GameObject imgLock;
        public Image imgSelect;
        public Image imgEquiped;
        public Image imgAdd;
        public Text txtNumber;
        public GameObject txtNew;
        public Text txtBind;
        public Text txtName;
        public GameObject addGo;
        public GameObject gotGo;
        public GameObject clock; // 禁售期
        public Image runeLevelImage;

        public Text idText;
        public Button btnNone;
        public Button btnBg;

        public UI_LongPressButton longPressButton;

        public void Parse(GameObject root) {
            this.transform = root.transform;

            this.imgLock = this.transform.Find("Image_Lock").gameObject;
            this.imgSelect = this.transform.Find("Image_Select").GetComponent<Image>();
            this.imgEquiped = this.transform.Find("Image_Equiped").GetComponent<Image>();
            this.imgAdd = this.transform.Find("Image_Add").GetComponent<Image>();
            this.txtNumber = this.transform.Find("Text_Number").GetComponent<Text>();
            this.txtNew = this.transform.Find("Text_New").gameObject;
            this.txtBind = this.transform.Find("Text_Bound").GetComponent<Text>();
            this.txtName = this.transform.Find("Text_Name").GetComponent<Text>();
            this.idText = this.transform.Find("Id")?.GetComponent<Text>();
            this.icon = this.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
            this.runeLevelImage = this.transform.Find("Image_RuneRank")?.GetComponent<Image>();

            this.gotGo = this.transform.Find("Btn_Get").gameObject;
            this.clock = this.transform.Find("Image_Clock").gameObject;
            this.addGo = this.transform.Find("Image_Add").gameObject;

            this.btnNone = this.transform.Find("Btn_None").GetComponent<Button>();
            this.btnBg = this.transform.Find("Btn_Item").GetComponent<Button>();
            if (!this.btnBg.gameObject.TryGetComponent(out this.longPressButton)) {
                this.longPressButton = this.btnBg.gameObject.AddComponent<UI_LongPressButton>();
            }
        }

        public void RegisterEvents(IListener listener) {
            this.btnBg.onClick.AddListener(listener.OnBtnBgClicked);
            this.btnNone.onClick.AddListener(listener.OnBtnNoneClicked);
            this.longPressButton.onStartPress.AddListener(listener.OnLongPressed);
        }

        private Action onClickBg;
        private Action onClickNone;
        private Action onLongPress;

        // 配置
        public void Setup(bool useClickBg, bool useClickNone, bool useLongPress, Action onClickBg = null, Action onClickNone = null, Action onLongPress = null) {
            this.btnBg.enabled = useClickBg;
            this.btnNone.enabled = useClickNone;
            this.longPressButton.enabled = useLongPress;

            this.onClickBg = onClickBg;
            this.onClickNone = onClickNone;
            this.onLongPress = onLongPress;
        }

        public void CleanUp() {
            this.onClickBg = null;
            this.onClickNone = null;
            this.onLongPress = null;
        }

        public interface IListener {
            void OnBtnBgClicked();
            void OnBtnNoneClicked();
            void OnLongPressed();
        }
    }
    public class UIRunePropItem : UIComponent, PropItemLayout.IListener {
        private readonly PropItemLayout layout = new PropItemLayout();

        private uint partnerId;
        private uint runeId;
        private CSVRuneSlot.Data csvRuneSlot;

        protected override void Loaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }
        public override void OnDestroy() {
            this.layout.CleanUp();

            base.OnDestroy();
        }

        public void Refresh(uint partnerId, uint runeId, CSVRuneSlot.Data csvRuneSlot) {
            this.partnerId = partnerId;
            this.runeId = runeId;
            this.csvRuneSlot = csvRuneSlot;

#if UNITY_EDITOR
            this.layout.idText.text = runeId.ToString();
#endif
            if (runeId == 0) {
                this.layout.btnNone.gameObject.SetActive(true);
                this.layout.icon.gameObject.SetActive(false);
                this.layout.runeLevelImage.gameObject.SetActive(false);
                this.layout.Setup(false, true, false, null, this.OnBtnNoneClicked);
            }
            else {
                this.layout.btnNone.gameObject.SetActive(false);
                this.layout.icon.gameObject.SetActive(true);
                this.layout.runeLevelImage.gameObject.SetActive(true);
                this.layout.Setup(true, false, false, this.OnBtnBgClicked);
                CSVRuneInfo.Data csvRune = CSVRuneInfo.Instance.GetConfData(runeId);
                if (null != csvRune) {
                    ImageHelper.SetIcon(this.layout.icon, csvRune.icon);
                    ImageHelper.SetIcon(this.layout.runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(csvRune.rune_lvl));
                }
            }
        }

        #region 实现的接口
        public void OnBtnBgClicked() {
            if (this.runeId != 0) {
                UIManager.OpenUI(EUIID.UI_PartnerRune_Tips, false, this.runeId);
            }
        }

        public void OnBtnNoneClicked() {
            UIManager.OpenUI(EUIID.UI_PartnerRuneReplace, false, new Tuple<uint, uint, CSVRuneSlot.Data>(this.partnerId, this.runeId, this.csvRuneSlot));
        }

        public void OnLongPressed() {
        }
        #endregion
    }

    // 技能信息
    public class UI_PartnerSkillAttr : UIComponent {
        public enum EUnlockType {
            Locked,        // 未解锁
            UnlockUnActive,  // 已解锁 未激活
            UnlockActive,  // 已解锁 已激活
        }

        private Image skillIcon;
        private Text skillName;
        private Text skillDesc;

        public GameObject lockGo;
        public GameObject unlockGo;
        public GameObject unActiveGo;
        public GameObject activeGo;

        private UIRunePropItem propItemVd;

        private Slider slider;
        private Text sliderText;

        private Button btnReplace;
        private Button btnUnload;

        public uint partnerId;
        public CSVRuneSlot.Data csvRuneSlot;

        private IListener listener;

        protected override void Loaded() {
            this.skillDesc = this.transform.Find("Text_Dis").GetComponent<Text>();
            this.skillIcon = this.transform.Find("Skill_Icon").GetComponent<Image>();
            this.skillName = this.transform.Find("Text_Name").GetComponent<Text>();

            // 未解锁
            this.lockGo = this.transform.Find("Node/Node/Lock").gameObject;
            // 已解锁
            this.unlockGo = this.transform.Find("Node/Node/Unlock").gameObject;
            // 已解锁 未激活
            this.unActiveGo = this.transform.Find("Node/Node/Unlock/UnlockUnActive").gameObject;
            // 已解锁 已激活
            this.activeGo = this.transform.Find("Node/Node/Unlock/UnlockActive").gameObject;

            this.propItemVd = new UIRunePropItem();
            this.propItemVd.Init(this.transform.Find("Node/Node/Unlock/PropItem"));

            this.slider = this.transform.Find("Node/Node/Lock/Slider_Exp").GetComponent<Slider>();
            this.sliderText = this.transform.Find("Node/Node/Lock/Slider_Exp/Text_Value").GetComponent<Text>();

            this.btnUnload = this.transform.Find("Node/Node/Unlock/UnlockActive/BtnUnload").GetComponent<Button>();
            this.btnUnload.onClick.AddListener(() => {
                this.listener?.OnBtnUnloadClicked(this.csvRuneSlot);
            });

            this.btnReplace = this.transform.Find("Node/Node/Unlock/UnlockActive/BtnReplace").GetComponent<Button>();
            this.btnReplace.onClick.AddListener(() => {
                this.listener?.OnBtnReplaceClicked(this.csvRuneSlot);
            });

            Button btn = this.transform.Find("Btn_Back").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnBackClicked);
        }

        public override void OnDestroy() {
            this.propItemVd.OnDestroy();

            base.OnDestroy();
        }

        private void OnBtnBackClicked() {
            this.listener?.OnBtnBackClicked();
        }

        public override void SetData(params object[] arg) {
            if (null != arg && arg.Length > 0) {
                this.partnerId = Convert.ToUInt32(arg[0]);
                this.csvRuneSlot = arg[1] as CSVRuneSlot.Data;
                this.Refresh();
            }
        }

        private new void Refresh() {
            if (null != this.csvRuneSlot && this.partnerId != 0) {
                bool isPassView = this.csvRuneSlot.slot_type == 9;
                if (isPassView) {
                    List<uint> data = Sys_Partner.Instance.GetRuneSkillData(this.partnerId, (int)this.csvRuneSlot.slot_category - 1, (int)this.csvRuneSlot.slot_sequence);
                    if (data.Count >= 3) {
                        CSVPassiveSkillInfo.Data skillData = CSVPassiveSkillInfo.Instance.GetConfData(data[0]);
                        if (null != skillData) {
                            ImageHelper.SetIcon(this.skillIcon, skillData.icon);
                            TextHelper.SetText(this.skillName, skillData.name);
                            TextHelper.SetText(this.skillDesc, skillData.desc);
                        }

                        bool isType = data[1] == 1;// 1 额外符文 0 前置镶嵌满即可
                        uint runeId = Sys_Partner.Instance.GetRuneId(this.partnerId, (int)this.csvRuneSlot.slot_category - 1, (int)this.csvRuneSlot.slot_sequence - 1);
                        // 解锁
                        bool isUnLock = Sys_Partner.Instance.GetRuneSkillUnlock(this.partnerId, (ERuneType)this.csvRuneSlot.slot_category - 1, this.csvRuneSlot.slot_unlocknum);
                        // 激活
                        bool isActive = (!isType) || (isType && runeId != 0);

                        this.lockGo.SetActive(!isUnLock);
                        this.unlockGo.SetActive(isUnLock);
                        this.unActiveGo.SetActive(isUnLock && !isActive);
                        this.activeGo.SetActive(isUnLock && isActive);

                        if (!isUnLock) {
                            int equipedCount = Sys_Partner.Instance.GetEquipedRuneCount(this.partnerId, (ERuneType)this.csvRuneSlot.slot_category - 1);
                            this.slider.value = 1f * equipedCount / this.csvRuneSlot.slot_unlocknum;
                            this.sliderText.text = string.Format("{0}/{1}", equipedCount, this.csvRuneSlot.slot_unlocknum);
                        }
                        else {
                            this.propItemVd.Refresh(this.partnerId, runeId, this.csvRuneSlot);
                            if (isActive) {
                                // 激活状态下， 还需要runeId != 0才能让按钮显示出来
                                bool activeAndHasValidRune = isType && runeId != 0;
                                this.btnReplace.gameObject.SetActive(activeAndHasValidRune);
                                this.btnUnload.gameObject.SetActive(activeAndHasValidRune);
                            }
                        }
                    }
                }
            }
        }

        public void Register(IListener listener) {
            this.listener = listener;
        }

        public interface IListener {
            void OnBtnReplaceClicked(CSVRuneSlot.Data slotData);
            void OnBtnUnloadClicked(CSVRuneSlot.Data slotData);
            void OnBtnBackClicked();
        }
    }

    public class UI_PartnerRuneCtrlRight : UIComponent, UI_PartnerAllRuneAttr.IListener, UI_PartnerEmptyRuneAttr.IListener, UI_PartnerEquipRuneAttr.IListener, UI_PartnerSkillAttr.IListener {
        public enum EMode {
            AllRune, // 全符文
            EquipedRune,  // 镶嵌符文信息
            EmptyRune,    // 未装载符文信息展示
            Skill,    // 技能信息(不管是空技能还是装配技能右侧都有信息展示，只不过一个是显示解锁条件，一个显示进度)
        }

        public UI_PartnerAllRuneAttr allRuneAttr;
        public UI_PartnerEmptyRuneAttr emptyRuneAttr;
        public UI_PartnerEquipRuneAttr equipRuneAttr;

        public UI_PartnerSkillAttr skillAttr;

        private UI_PartnerRuneEquip ui;
        private uint partnerId;
        private CSVRuneSlot.Data csvRuneSlot;
        private uint runeId;

        protected override void Loaded() {
        }

        public void UpdateInfo(UI_PartnerRuneEquip ui, EMode mode, uint partnerId, CSVRuneSlot.Data csvRuneSlot, uint runeId) {
            this.ui = ui;
            this.partnerId = partnerId;
            this.csvRuneSlot = csvRuneSlot;
            this.runeId = runeId;

            this.allRuneAttr?.ShowHide(mode == EMode.AllRune);
            this.equipRuneAttr?.ShowHide(mode == EMode.EquipedRune);
            this.emptyRuneAttr?.ShowHide(mode == EMode.EmptyRune);
            this.skillAttr?.ShowHide(mode == EMode.Skill);

            if (mode == EMode.AllRune) {
                if (this.allRuneAttr == null) {
                    this.allRuneAttr = new UI_PartnerAllRuneAttr();
                    this.allRuneAttr.Init(this.transform.Find("AllRune"));
                    this.allRuneAttr.Register(this);
                }

                this.allRuneAttr.Show();
                this.allRuneAttr.SetData(partnerId);
            }
            else if (mode == EMode.EmptyRune) {
                if (this.emptyRuneAttr == null) {
                    this.emptyRuneAttr = new UI_PartnerEmptyRuneAttr();
                    this.emptyRuneAttr.Init(this.transform.Find("EmptyRune"));
                    this.emptyRuneAttr.Register(this);
                }
                this.emptyRuneAttr.Show();
                this.emptyRuneAttr.SetData(runeId, csvRuneSlot, partnerId);
            }
            else if (mode == EMode.EquipedRune) {
                if (this.equipRuneAttr == null) {
                    this.equipRuneAttr = new UI_PartnerEquipRuneAttr();
                    this.equipRuneAttr.Init(this.transform.Find("EquipRune"));
                    this.equipRuneAttr.Register(this);
                }

                this.equipRuneAttr.Show();
                this.equipRuneAttr.SetData(partnerId, csvRuneSlot);
            }
            else if (mode == EMode.Skill) {
                if (this.skillAttr == null) {
                    this.skillAttr = new UI_PartnerSkillAttr();
                    this.skillAttr.Init(this.transform.Find("Skill"));
                    this.skillAttr.Register(this);
                }

                this.skillAttr.Show();
                this.skillAttr.SetData(partnerId, csvRuneSlot);
            }
        }

        public override void OnDestroy() {
            this.allRuneAttr?.OnDestroy();
            this.allRuneAttr = null;
            this.equipRuneAttr?.OnDestroy();
            this.equipRuneAttr = null;
            this.emptyRuneAttr?.OnDestroy();
            this.emptyRuneAttr = null;
            this.skillAttr?.OnDestroy();
            this.skillAttr = null;

            base.OnDestroy();
        }

        #region 实现的接口
        // 全符文 -- 卸载所有
        public void OnBtnUnloadAllRuneClicked() {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(2006107u).words;
            PromptBoxParameter.Instance.SetConfirm(true, () => {
                Sys_Partner.Instance.PartnerRuneUnloadAllReq(this.partnerId);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        
        // 全符文 -- 装载所有
        public void OnBtnLoadAllRuneClicked() {
            Sys_Partner.Instance.PartnerRuneLoadAllReq(this.partnerId);
        }

        // 空符文 -- 装载
        public void OnBtnSureChangeClicked(uint targetRuneId) {
            Sys_Partner.Instance.PartnerRuneDressReq(this.partnerId, targetRuneId, this.csvRuneSlot.slot_category - 1, this.csvRuneSlot.slot_sequence - 1);
        }

        // 已装配符文 -- 替换
        public void OnBtnReplaceClicked(CSVRuneSlot.Data slotData) {
            uint runeId = Sys_Partner.Instance.GetRuneId(this.partnerId, (int)slotData.slot_category - 1, (int)slotData.slot_sequence - 1);
            UIManager.OpenUI(EUIID.UI_PartnerRuneReplace, false, new System.Tuple<uint, uint, CSVRuneSlot.Data>(this.partnerId, runeId, slotData));
        }
        // 已装配符文 -- 卸载
        public void OnBtnUnloadClicked(CSVRuneSlot.Data slotData) {
            Sys_Partner.Instance.PartnerRuneUnloadReq(this.partnerId, slotData.slot_category - 1, slotData.slot_sequence - 1);
        }

        public void OnBtnBackClicked() {
            this.ui.SetAllRuneAttr();
        }
        #endregion
    }

    public class UI_PartnerRuneEquip : UIParseCommon, UIRuneSlotTree.IListener, UIPartnerHeadList.IListener {
        private CP_Toggle bttleToggle;
        private CP_Toggle lifeToggle;

        private GameObject battleRedDotGo;
        private GameObject lifeRedDotGo;

        private uint curPartnerId = 0;
        private ERuneType curViewType = ERuneType.Battle;
        private UI_PartnerRuneCtrlRight.EMode curRightMode = UI_PartnerRuneCtrlRight.EMode.AllRune;

        private readonly Dictionary<int, List<CSVRuneSlot.Data>> dicLineSlot = new Dictionary<int, List<CSVRuneSlot.Data>>();
        private List<PartnerRunePos> partnerRunePos;

        public UIPartnerHeadList headList { get; private set; } = null;
        private UIRuneSlotTree leftSlotTree;
        private UI_PartnerRuneCtrlRight rightRuneCtrl;

        private readonly UI_RuneReplaceRight changeLeftView;
        private readonly UI_RuneReplaceLeft changeRightView;

        private Text _textName;
        private GameObject _goFormation;

        public uint selectedId { get; set; } = 0;

        protected override void Parse()
        {
            this.headList = new UIPartnerHeadList();
            this.headList.Init(this.transform.Find("Scroll_View_Partner"));
            this.headList.Register(this);

            this.leftSlotTree = new UIRuneSlotTree();
            this.leftSlotTree.Init(this.transform.Find("View_Left"));
            this.leftSlotTree.Register(this);

            this.battleRedDotGo = this.transform.Find("View_Left/List_Menu/TabList/ListItem/Image_Dot").gameObject;
            this.lifeRedDotGo = this.transform.Find("View_Left/List_Menu/TabList/ListItem (1)/Image_Dot").gameObject;
                
            this.bttleToggle = this.transform.Find("View_Left/List_Menu/TabList/ListItem").GetComponent<CP_Toggle>();
            this.bttleToggle.onValueChanged.AddListener((bool isOn) => {
                if (isOn)
                {
                    this.SetRuneSlotView(ERuneType.Battle);
                }
            });
            this.lifeToggle = this.transform.Find("View_Left/List_Menu/TabList/ListItem (1)").GetComponent<CP_Toggle>();
            this.lifeToggle.onValueChanged.AddListener((bool isOn) => {
                if (isOn)
                {
                    this.SetRuneSlotView(ERuneType.Life);
                }
            });

            this.rightRuneCtrl = new UI_PartnerRuneCtrlRight();
            this.rightRuneCtrl.Init(this.transform.Find("View_Right"));

            _textName = transform.Find("View_Left/Text_Name").GetComponent<Text>();
            _goFormation = transform.Find("View_Left/Grid/Team").gameObject;
        }

        public void SetData(uint select = 0) 
        {
            uint partnerId = select;

            List<uint> tempList = Sys_Partner.Instance.GetRuneEquipPartnerList();
            if (partnerId != 0)
            {
                selectedId = 0;
                //tempList.Remove(partnerId);
                //tempList.Insert(0, partnerId);
            }
            else
            {
                partnerId = tempList[0];
            }

            selectedId = selectedId == 0 ? partnerId : selectedId;

            // 移动headList的位置并且选中
            this.headList.UpdateInfo(tempList, selectedId);
            this.headList.MoveTo(selectedId);

            this.RefreshAll(selectedId);
        }

        public void ReSelectPartner() {
            this.headList.ForceRefreshActiveCell(this.selectedId);
            //this.headList.MoveTo(this.selectedId);
            this.RefreshAll(this.selectedId);
        }

        public void RefreshAll(uint partnerId) {
            this.curPartnerId = partnerId;
            this.partnerRunePos = Sys_Partner.Instance.GetPartnerRunePosByPartnerId(this.curPartnerId);

            this.Show();

            //伙伴名称
            CSVPartner.Data infoData = CSVPartner.Instance.GetConfData(this.curPartnerId);
            if (infoData != null)
                _textName.text = LanguageHelper.GetTextContent(infoData.name);
            //阵容显示
            FrameworkTool.DestroyChildren(_goFormation.transform.parent.gameObject, _goFormation.name);
            List<int> formations = Sys_Partner.Instance.BelongFormations(this.curPartnerId);

            //Debug.LogErrorFormat("Id {0}  count = {1}", formations.Count, this.curPartnerId);
            for (int i = 0; i < formations.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(_goFormation, _goFormation.transform.parent);
                go.SetActive(true);
                Text text = go.transform.Find("Text").GetComponent<Text>();
                text.text = LanguageHelper.GetTextContent((uint)formations[i] + 2006006);
            }
        }

        public void TryRefreshRight(UI_PartnerRuneCtrlRight.EMode mode, uint partnerId, CSVRuneSlot.Data csvRuneSlot, uint runeId) {
            this.curRightMode = mode;
            this.rightRuneCtrl.UpdateInfo(this, mode, partnerId, csvRuneSlot, runeId);
        }

        private void ShowInitView() {
            this.leftSlotTree.ClearSelectState();
            this.leftSlotTree.SetData(this.curPartnerId, (uint)this.curViewType);

            this.SetAllRuneAttr();
        }
        
        public override void Show() {
            base.Show();

            // 左右切页的红点
            bool has = Sys_Partner.Instance.HasNew(this.curPartnerId, (int) ERuneType.Battle);
            battleRedDotGo.SetActive(has);
            has = Sys_Partner.Instance.HasNew(this.curPartnerId, (int) ERuneType.Life);
            lifeRedDotGo.SetActive(has);
            
            if (this.curViewType == ERuneType.Battle) {
                this.bttleToggle.SetSelected(true, false);
                this.lifeToggle.SetSelected(false, false);
            }
            else if (this.curViewType == ERuneType.Life) {
                this.bttleToggle.SetSelected(false, false);
                this.lifeToggle.SetSelected(true, false);
            }

            this.leftSlotTree.SetData(this.curPartnerId, (uint)this.curViewType);

            if (this.curRightMode == UI_PartnerRuneCtrlRight.EMode.AllRune) {
                this.SetAllRuneAttr();
            }

            ProcessEventsForEnable(false);
            ProcessEventsForEnable(true);
        }

        public override void Hide()
        {
            base.Hide();
            this.curRightMode = UI_PartnerRuneCtrlRight.EMode.AllRune;
            ProcessEventsForEnable(false);
        }

        private void SetRuneSlotView(ERuneType type) {
            if (this.curViewType != type) {
                bool hasNew = Sys_Partner.Instance.HasNew(this.selectedId, (uint)this.curViewType);
                if (hasNew) {
                    Sys_Partner.Instance.ClearStatus(this.curPartnerId, (uint)this.curViewType);
                }

                this.curViewType = type;
                this.leftSlotTree.SetData(this.curPartnerId, (uint)this.curViewType);

                this.SetAllRuneAttr();
            }
        }

        public void SetAllRuneAttr() {
            this.leftSlotTree.ClearSelectState();
            this.TryRefreshRight(UI_PartnerRuneCtrlRight.EMode.AllRune, this.curPartnerId, null, 0);
        }

        public void OnClickRuneCeil(UIRuneSlot ceilData) {
            Partner partner = Sys_Partner.Instance.GetPartnerInfo(this.curPartnerId);
            if (null != partner && partner.Level >= ceilData.csvRuneSlot.slot_unlocklevel) {
                var runeId = Sys_Partner.Instance.GetRuneId(this.curPartnerId, (int)ceilData.csvRuneSlot.slot_category - 1, (int)ceilData.csvRuneSlot.slot_sequence - 1);
                if (ceilData.csvRuneSlot.slot_type == 9) {
                    // 技能点击
                    this.TryRefreshRight(UI_PartnerRuneCtrlRight.EMode.Skill, this.curPartnerId, ceilData.csvRuneSlot, runeId);
                }
                else {
                    if (0 != runeId) {
                        // 非空槽点击
                        this.TryRefreshRight(UI_PartnerRuneCtrlRight.EMode.EquipedRune, this.curPartnerId, ceilData.csvRuneSlot, runeId);
                    }
                    else {
                        this.TryRefreshRight(UI_PartnerRuneCtrlRight.EMode.EmptyRune, this.curPartnerId, ceilData.csvRuneSlot, runeId);
                    }
                }
            }
            
            bool hasNew = Sys_Partner.Instance.HasNew(this.selectedId, (uint)this.curViewType, ceilData.csvRuneSlot.slot_sequence - 1);
            if (hasNew) {
                Sys_Partner.Instance.ClearStatus(this.curPartnerId, (uint)this.curViewType, (int)ceilData.csvRuneSlot.slot_sequence - 1);
            }
        }

        // cuibinbin 这里需要被调用
        public override void OnDestroy() {
            this.headList.OnDestroy();
            this.headList = null;
            this.leftSlotTree.OnDestroy();
            this.leftSlotTree = null;
            this.rightRuneCtrl.OnDestroy();
            this.rightRuneCtrl = null;

            this.curPartnerId = 0;
            this.selectedId = 0;

            base.OnDestroy();
        }

        #region 实现的接口
        // 左侧列表选中伙伴点击
        public void OnSelectParnter(uint newId) {
            if (newId != this.selectedId) {
                bool hasNew = Sys_Partner.Instance.HasNew(this.selectedId);
                if (hasNew) {
                    Sys_Partner.Instance.ClearStatus(this.selectedId);
                }

                this.selectedId = newId;
                this.curRightMode = UI_PartnerRuneCtrlRight.EMode.AllRune;
            }
            
            this.ReSelectPartner();
        }
        #endregion

        #region 网络响应 全部回到全符文状态
        private void ProcessEventsForEnable(bool toRegister) {
            Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnRuneDressCallBack, this.OnRuneDressCallBack, toRegister);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnRuneUnLoadCallBack, this.OnRuneUnLoadCallBack, toRegister);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnRuneLoadALlCallBack, this.OnRuneLoadALlCallBack, toRegister);
        }

        // 符文替换/装载
        private void OnRuneDressCallBack(uint pos) {
            this.curRightMode = UI_PartnerRuneCtrlRight.EMode.AllRune;
            this.ReSelectPartner();
            
            // 特效展示
            this.leftSlotTree.SetDrPostion((int)pos);
        }

        // 符文卸载[包括卸载所有，卸载单个]
        private void OnRuneUnLoadCallBack() {
            this.ShowInitView();
        }

        // 一键装载所有
        private void OnRuneLoadALlCallBack() {
            this.curRightMode = UI_PartnerRuneCtrlRight.EMode.AllRune;
            this.ReSelectPartner();
        }

        #endregion
    }
}
