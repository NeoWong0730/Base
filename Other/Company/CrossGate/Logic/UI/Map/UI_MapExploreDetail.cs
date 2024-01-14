using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using System;
using Packet;
using UnityEngine;

namespace Logic {
    /// <summary> 地图探索详情 </summary>
    public class UI_MapExploreDetail : UIBase {
        // 下拉菜单
        public class PetDropDown : UIPopdownItem {
            public virtual void Refresh(uint zoneId, int index) {
                base.Refresh(zoneId, index);

                var csv = CSVPetNew.Instance.GetConfData(zoneId);
                if (csv != null) {
                    TextHelper.SetText(this.text, csv.name);
                    this.optionName = this.text.text;
                }
            }
        }

        public class FoodDropDown : UIPopdownItem {
            public virtual void Refresh(uint zoneId, int index) {
                base.Refresh(zoneId, index);

                var csv = CSVItem.Instance.GetConfData(zoneId);
                if (csv != null) {
                    TextHelper.SetText(this.text, csv.name_id);
                    this.optionName = this.text.text;
                }
            }
        }

        public class PageFood : UIComponent {
            public CP_PopdownList popdownList;
            public UIElementCollector<FoodDropDown> popdownVds = new UIElementCollector<FoodDropDown>();
            public Button btnGoto;
            public Button btnManual;

            public Text foodName;
            public Text foodDesc;

            public uint npcId;

            public CSVNpc.Data csvNpc {
                get { return CSVNpc.Instance.GetConfData(this.npcId); }
            }

            public int currentId = 0;

            protected override void Loaded() {
                popdownList = this.transform.Find("PopupList").GetComponent<CP_PopdownList>();

                btnGoto = this.transform.Find("View01/Btn_02").GetComponent<Button>();
                btnGoto.onClick.AddListener(OnBtnGotoClicked);
                btnManual = this.transform.Find("View01/Btn_01").GetComponent<Button>();
                btnManual.onClick.AddListener(OnBtnManualClicked);

                foodName = this.transform.Find("Text").GetComponent<Text>();
                foodDesc = this.transform.Find("Text1").GetComponent<Text>();
            }

            private void OnBtnGotoClicked() {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(this.npcId);
                UIManager.ClearUntilMain();
            }

            private void OnBtnManualClicked() {
                UIManager.OpenUI(EUIID.UI_Knowledge_RecipeCooking, false, (uint) this.currentId);
            }

            private List<uint> ids = new List<uint>();

            public void Refresh(uint npcId) {
                this.npcId = npcId;

                ids = csvNpc.ResourecePara ?? EmptyList<uint>.Value;
                this.popdownVds.BuildOrRefresh(this.popdownList.optionProto, this.popdownList.optionParent, ids, this.OnRefreshPopDown);

                int index = ids.IndexOf((uint) this.currentId);
                if (index == -1) {
                    if (this.popdownVds.Count > 0) {
                        index = 0;
                        this.currentId = (int) ids[index];
                        this.popdownVds[index].SetSelected(true, true);
                    }
                }
                else {
                    this.popdownVds[index].SetSelected(true, true);
                }
            }

            private void OnRefreshPopDown(FoodDropDown vd, uint id, int index) {
                vd.SetUniqueId((int) id);
                vd.SetSelectedAction((zondId, force) => {
                    this.popdownVds.ForEach((e) => { e.SetHighlight(false); });
                    vd.SetHighlight(true);

                    this.popdownList.Expand(false);
                    this.popdownList.SetSelected(vd.optionName);
                    // 选中
                    int idx = this.ids.IndexOf((uint) zondId);
                    this.popdownList.MoveTo(false, 1f * (idx + 1) / this.ids.Count);

                    this.currentId = zondId;
                    this.RefreshContent();
                });
                vd.Refresh(id, index);
                vd.SetHighlight(false);
            }

            private void RefreshContent() {
                var csvItem = CSVItem.Instance.GetConfData((uint) this.currentId);
                if (csvItem != null) {
                    TextHelper.SetText(this.foodName, csvItem.name_id);
                    TextHelper.SetText(this.foodDesc, csvItem.describe_id);
                }
                else {
                    //
                }
            }
        }

        public class PagePet : UIComponent {
            public CP_PopdownList popdownList;
            public UIElementCollector<PetDropDown> popdownVds = new UIElementCollector<PetDropDown>();
            public Button btnGoto;
            public Button btnManual;

            public Text area;
            public Text firstFind;
            public GameObject coord;

            public Text frequencyDetail;
            public Text coordDetail;

            public uint npcId;

            public int currentId = 0;

            public CSVNpc.Data csvNpc {
                get { return CSVNpc.Instance.GetConfData(this.npcId); }
            }

            protected override void Loaded() {
                popdownList = this.transform.Find("PopupList").GetComponent<CP_PopdownList>();

                btnGoto = this.transform.Find("View01/Btn_01").GetComponent<Button>();
                btnGoto.onClick.AddListener(OnBtnGotoClicked);
                btnManual = this.transform.Find("View01/Btn_02").GetComponent<Button>();
                btnManual.onClick.AddListener(OnBtnManualClicked);

                area = this.transform.Find("Text1/Text").GetComponent<Text>();
                firstFind = this.transform.Find("Text2/Text").GetComponent<Text>();
                coord = this.transform.Find("Text4").gameObject;
                
                frequencyDetail = this.transform.Find("Text3/Text").GetComponent<Text>();
                coordDetail = this.transform.Find("Text4/Text").GetComponent<Text>();
            }

            private void OnBtnGotoClicked() {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(this.npcId);
                UIManager.ClearUntilMain();
            }

            private void OnBtnManualClicked() {
                UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, (uint) this.currentId);
            }

            private List<uint> ids = new List<uint>();

            public void Refresh(uint npcId) {
                this.npcId = npcId;

                ids = csvNpc.ResourecePara ?? ids;
                this.popdownVds.BuildOrRefresh(this.popdownList.optionProto, this.popdownList.optionParent, ids, this.OnRefreshPopDown);

                int index = ids.IndexOf((uint) this.currentId);
                if (index == -1) {
                    if (this.popdownVds.Count > 0) {
                        index = 0;
                        this.currentId = (int) ids[index];
                        this.popdownVds[index].SetSelected(true, true);
                    }
                }
                else {
                    this.popdownVds[index].SetSelected(true, true);
                }
            }

            private void OnRefreshPopDown(PetDropDown vd, uint id, int index) {
                vd.SetUniqueId((int) id);
                vd.SetSelectedAction((zondId, force) => {
                    this.popdownVds.ForEach((e) => { e.SetHighlight(false); });
                    vd.SetHighlight(true);

                    this.popdownList.Expand(false);
                    this.popdownList.SetSelected(vd.optionName);

                    // 选中
                    int idx = this.ids.IndexOf((uint) zondId);
                    this.popdownList.MoveTo(false, 1f * (idx + 1) / this.ids.Count);

                    this.currentId = zondId;
                    this.RefreshContent();
                });
                vd.Refresh(id, index);
                vd.SetHighlight(false);
            }

            private void RefreshContent() {
                var cSVPetSealData = CSVPetNewSeal.Instance.GetConfData((uint) this.currentId);
                if (cSVPetSealData != null) {
                    CmdPetGetHandbookRes.Types.HBSeverData nameData = Sys_Pet.Instance.GetPetBookNameData((uint) this.currentId);
                    bool showMapBtn = false;
                    bool showFindText = false;
                    bool hasSealInfo = null != cSVPetSealData;
                    if (hasSealInfo) {
                        uint sealType = cSVPetSealData.seal_type;
                        if (sealType == 1) {
                            showFindText = true;
                        }
                        else if (sealType == 2) {
                            showMapBtn = true;
                            showFindText = true;
                        }
                        else if (sealType == 3) {
                            showFindText = cSVPetSealData.is_msg;
                        }

                        coord.gameObject.SetActive(sealType != 2 || Sys_Pet.Instance.CanAutoSeal((uint) this.currentId)); //地图-坐标或则信息
                        frequencyDetail.text = LanguageHelper.GetTextContent(cSVPetSealData.frequency); // 频率
                        area.text = LanguageHelper.GetTextContent(cSVPetSealData.seal_area_spe); //地图-发现者    
                        switch (sealType) {
                            case 1:
                                if (cSVPetSealData.AccInfo != 0) {
                                    coordDetail.text = LanguageHelper.GetTextContent(4539);
                                }
                                else {
                                    coordDetail.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString()); //地图-坐标
                                }

                                break;
                            case 2:
                                if (cSVPetSealData.AccInfo != 0) {
                                    if (nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName)) {
                                        coordDetail.text = LanguageHelper.GetTextContent(4539);
                                    }
                                    else {
                                        coordDetail.text = LanguageHelper.GetTextContent(4539); //地图-坐标
                                    }
                                }
                                else {
                                    if (nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName)) {
                                        coordDetail.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString()); //地图-坐标
                                    }
                                    else {
                                        coordDetail.text = LanguageHelper.GetTextContent(4539); //地图-坐标
                                    }
                                }

                                break;
                            case 3:
                                if (cSVPetSealData.AccInfo != 0) {
                                    if (!Sys_Pet.Instance.CanAutoSeal((uint) this.currentId)) {
                                        if (cSVPetSealData.is_unloce && nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName)) {
                                            coordDetail.text = LanguageHelper.GetTextContent(4539);
                                        }
                                        else {
                                            coordDetail.text = LanguageHelper.GetTextContent(4539);
                                        }
                                    }
                                    else {
                                        coordDetail.text = LanguageHelper.GetTextContent(4539);
                                    }
                                }
                                else {
                                    if (!Sys_Pet.Instance.CanAutoSeal((uint) this.currentId)) {
                                        if (cSVPetSealData.is_unloce && nameData != null && !string.IsNullOrEmpty(nameData.DiscovererName)) {
                                            coordDetail.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString()); //地图-坐标
                                        }
                                        else {
                                            coordDetail.text = LanguageHelper.GetTextContent(4539);
                                        }
                                    }
                                    else {
                                        coordDetail.text = LanguageHelper.GetTextContent(11003, Mathf.Abs(cSVPetSealData.coordinate[0] / 100f).ToString(), Mathf.Abs(cSVPetSealData.coordinate[1] / 100f).ToString()); //地图-坐标
                                    }
                                }

                                break;
                        }

                        if (null != nameData) {
                            if (string.IsNullOrEmpty(nameData.DiscovererName)) {
                                this.firstFind.text = LanguageHelper.GetTextContent(4536);
                            }
                            else {
                                if (null != cSVPetSealData.coordinate && cSVPetSealData.coordinate.Count >= 2) {
                                    this.firstFind.text = LanguageHelper.GetTextContent(4537, nameData.DiscovererName);
                                }
                            }
                        }
                        else {
                            this.firstFind.text = LanguageHelper.GetTextContent(4536);
                        }
                    }
                    else {
                        area.text = LanguageHelper.GetTextContent(11782); //无法封印
                    }

                    frequencyDetail.transform.parent.gameObject.SetActive(hasSealInfo);
                    this.firstFind.gameObject.SetActive(showFindText);
                    coord.gameObject.SetActive(hasSealInfo);
                }
            }
        }

        #region 界面组件

        private Transform tr_Node;
        private Button button_TelNpc1;
        private Button button_TelNpc2;

        private GameObject petGo;
        private GameObject foodGo;

        private PageFood pageFood;
        private PagePet pagePet;

        #endregion

        #region 数据定义

        private uint npcId;
        private uint markType;

        private uint MoveTargetNpcID = 0;

        #endregion

        #region 系统函数

        protected override void OnLoaded() {
            petGo = transform.Find("Animator/View_ExploreDetail/Type_Pet").gameObject;
            foodGo = transform.Find("Animator/View_ExploreDetail/Type_Food").gameObject;

            tr_Node = transform.Find("Animator/View_ExploreDetail").transform;
            button_TelNpc1 = transform.Find("Animator/View_ExploreDetail/Type2/View02/Btn_01").GetComponent<Button>();
            button_TelNpc2 = transform.Find("Animator/View_ExploreDetail/Type2/View02/Btn_02").GetComponent<Button>();

            transform.Find("Animator/View_ExploreDetail/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_CloseSelectMark);
            button_TelNpc1.onClick.AddListener(() => { OnClick_TelNpc(this.npcId); });
            button_TelNpc2.onClick.AddListener(() => { OnClick_TelNpc(this.npcId); });
            transform.Find("Animator/View_ExploreDetail/Type1/View01/Btn_01").GetComponent<Button>().onClick.AddListener(() => { OnClick_TelNpc(this.npcId); });
        }

        protected override void OnOpen(object arg) {
            Tuple<uint, uint> tp = arg as Tuple<uint, uint>;
            if (tp != null) {
                this.npcId = tp.Item1;
                markType = tp.Item2;
            }
        }

        protected override void OnShow() {
            RefreshView();
        }

        protected override void OnDestroy() {
            this.pageFood?.OnDestroy();
            this.pagePet?.OnDestroy();
        }

        #endregion

        #region 初始化

        private void CreateItemList(Transform node, Transform child, int number) {
            while (node.childCount < number) {
                GameObject.Instantiate(child, node);
            }
        }

        #endregion

        #region 界面显示

        private void RefreshView() {
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(this.npcId);
            if (null == cSVNpcData) {
                tr_Node.gameObject.SetActive(false);
                return;
            }

            uint markId = Sys_Npc.Instance.GetMarkId(cSVNpcData);
            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(markId);
            if (null == cSVMapExplorationMarkData) {
                tr_Node.gameObject.SetActive(false);
                return;
            }

            Image image_Icon = tr_Node.Find("Image_Title/Image_Mark").GetComponent<Image>();
            bool isMark = Sys_Npc.Instance.IsActivatedNpc(cSVNpcData.id);
            ImageHelper.SetIcon(image_Icon, isMark ? cSVMapExplorationMarkData.resource_icon2 : cSVMapExplorationMarkData.resource_icon1);
            Text text_Name = tr_Node.Find("Image_Title/Text").GetComponent<Text>();
            text_Name.text = LanguageHelper.GetNpcTextContent(cSVNpcData.name);

            this.petGo.SetActive(false);
            this.foodGo.SetActive(false);
            ENPCMarkType mkType = (ENPCMarkType) (markType);
            if (mkType == ENPCMarkType.Food || mkType == ENPCMarkType.Food_Fishing || mkType == ENPCMarkType.Food_Hunting || mkType == ENPCMarkType.Food_LianDan || mkType == ENPCMarkType.Food_Water) {
                // 食材
                this.foodGo.SetActive(true);
                if (this.pageFood == null) {
                    this.pageFood = new PageFood();
                    this.pageFood.Init(this.foodGo.transform);
                }

                pageFood.Refresh(this.npcId);
            }
            else if (mkType == ENPCMarkType.CatchPet) {
                // 抓宠
                this.petGo.SetActive(true);
                if (this.pagePet == null) {
                    this.pagePet = new PagePet();
                    this.pagePet.Init(this.petGo.transform);
                }

                pagePet.Refresh(this.npcId);
            }
            else {
                // 其他原始逻辑
                SetExploreDetailView(tr_Node, cSVNpcData, cSVMapExplorationMarkData);
            }
        }

        private void SetExploreDetailView(Transform tr, CSVNpc.Data cSVNpcData, CSVMapExplorationMark.Data cSVMapExplorationMarkData) {
            uint taskId = Sys_Npc.Instance.GetTaskId(cSVNpcData);
            tr.gameObject.SetActive(true);

            /// <summary> 名称 </summary>
            Text text_Name = tr.Find("Image_Title/Text").GetComponent<Text>();
            text_Name.text = string.Empty;
            /// <summary> 说明 </summary>
            Text text_Explain = tr.Find("Text").GetComponent<Text>();
            text_Explain.text = string.Empty;
            /// <summary> 任务 </summary>
            GameObject go_Task = tr.Find("Type1").gameObject;
            if (Sys_Npc.Instance.IsTaskNpc(cSVNpcData)) {
                go_Task.SetActive(true);
                GameObject go_View01 = tr.Find("Type1/View01").gameObject;
                GameObject go_View02 = tr.Find("Type1/View02").gameObject;

                bool isFinish = Sys_Task.Instance.IsSubmited(taskId);
                CSVTask.Data taskData = CSVTask.Instance.GetConfData(taskId);
                text_Name.text = LanguageHelper.GetTaskTextContent(taskData.taskName);
                text_Explain.text = LanguageHelper.GetTaskTextContent(taskData.taskDescribe);
                go_View01.SetActive(!isFinish);
                go_View02.SetActive(isFinish);
                if (!isFinish && null != taskData) {
                    Text text_lv = tr.Find("Type1/View01/Grid/Image_Lv/Image_Frame1/Text_Num").GetComponent<Text>();

                    uint lanId = taskData.ExecuteLvLowerLimit <= Sys_Role.Instance.Role.Level ? 4566u : 4567u;
                    string require = LanguageHelper.GetTextContent(lanId, taskData.ExecuteLvLowerLimit.ToString());
                    TextHelper.SetText(text_lv, require);

                    GameObject childExp = tr.Find("Type1/View01/Grid/Image_Frame").gameObject;
                    Text text_Exp = tr.Find("Type1/View01/Grid/Image_Frame/Image_Frame1/Text_Num").GetComponent<Text>();
                    Image image_Exp = tr.Find("Type1/View01/Grid/Image_Frame/Image_Frame1/Text_Num/ICON").GetComponent<Image>();
                    if (null != taskData.RewardExp && taskData.RewardExp.Count >= 2) {
                        ItemData exp = new ItemData(0, 0, taskData.RewardExp[0], taskData.RewardExp[1], 0, false, false, null, null, 0);

                        childExp.SetActive(true);
                        text_Exp.text = exp.Count.ToString();
                        ImageHelper.SetIcon(image_Exp, exp.cSVItemData.icon_id);
                    }
                    else {
                        childExp.SetActive(false);
                    }

                    GameObject childCoin = tr.Find("Type1/View01/Grid/Image_Frame (1)").gameObject;
                    Text text_Gold = tr.Find("Type1/View01/Grid/Image_Frame (1)/Image_Frame1/Text_Num").GetComponent<Text>();
                    Image image_Gold = tr.Find("Type1/View01/Grid/Image_Frame (1)/Image_Frame1/Text_Num/ICON").GetComponent<Image>();
                    if (null != taskData.RewardGold && taskData.RewardGold.Count >= 2) {
                        childCoin.SetActive(true);
                        ItemData gold = new ItemData(0, 0, taskData.RewardGold[0], taskData.RewardGold[1], 0, false, false, null, null, 0);
                        text_Gold.text = gold.Count.ToString();
                        ImageHelper.SetIcon(image_Gold, gold.cSVItemData.icon_id);
                    }
                    else {
                        childCoin.SetActive(false);
                    }

                    Transform tr_ItemParent = tr.Find("Type1/View01/Scroll_View_Item/TabList");
                    Transform tr_Item = tr.Find("Type1/View01/Scroll_View_Item/TabList/PropItem");
                    if (null != taskData.DropId) {
                        List<ItemData> items = new List<ItemData>();
                        int length = taskData.DropId.Count;
                        for (int i = 0; i < length; i++) {
                            List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(taskData.DropId[i]);
                            if (dropItems != null) {
                                int len = dropItems.Count;
                                for (int j = 0; j < len; j++) {
                                    ItemData item = new ItemData(0, 0, dropItems[j].id, (uint) dropItems[j].count, 0, false, false, null, null, 0);
                                    items.Add(item);
                                }
                            }
                        }

                        CreateItemList(tr_ItemParent, tr_Item, items.Count);
                        for (int i = 0; i < tr_ItemParent.childCount; i++) {
                            PropItem propItem = new PropItem();
                            propItem.BindGameObject(tr_ItemParent.GetChild(i).gameObject);
                            ItemData itemData = items.Count > i ? items[i] : null;
                            if (itemData == null) {
                                propItem.transform.gameObject.SetActive(false);
                            }
                            else {
                                propItem.transform.gameObject.SetActive(true);
                                PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(itemData.Id, itemData.Count, true, false, false, false, false, true);
                                propItem.SetData(new MessageBoxEvt(EUIID.UI_Map, showItemData));
                            }
                        }
                    }
                }
            }
            else {
                go_Task.SetActive(false);
            }

            /// <summary> 传送点 </summary>
            GameObject go_Transmit = tr.Find("Type2").gameObject;
            if (Sys_Npc.Instance.IsTransmitNpc(cSVNpcData)) {
                text_Name.text = LanguageHelper.GetNpcTextContent(cSVNpcData.name);
                text_Explain.text = LanguageHelper.GetTextContent(cSVNpcData.mark_des);
                go_Transmit.SetActive(true);
                bool isActivatedNpc = Sys_Npc.Instance.IsActivatedNpc(cSVNpcData.id);
                GameObject go_View01 = tr.Find("Type2/View01").gameObject;
                GameObject go_View02 = tr.Find("Type2/View02").gameObject;
                go_View01.SetActive(!isActivatedNpc);
                go_View02.SetActive(isActivatedNpc);

                button_TelNpc1.gameObject.SetActive(true);
                button_TelNpc2.gameObject.SetActive(false);
            }
            else if (Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Collection) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Lumbering) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Fishing) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Mining) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Hunting) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.ResourcesNew) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.MiningNew) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.LumberingNew) ||
                     Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.CollectionNew)) {
                text_Name.text = LanguageHelper.GetNpcTextContent(cSVNpcData.name);
                text_Explain.text = LanguageHelper.GetTextContent(cSVNpcData.mark_des);
                go_Transmit.SetActive(true);
                GameObject go_View01 = tr.Find("Type2/View01").gameObject;
                GameObject go_View02 = tr.Find("Type2/View02").gameObject;
                go_View01.SetActive(false);
                go_View02.SetActive(true);

                button_TelNpc1.gameObject.SetActive(false);
                button_TelNpc2.gameObject.SetActive(true);
            }
            else {
                go_Transmit.SetActive(false);
            }
        }

        #endregion

        #region 响应事件

        public void OnClick_Close() {
            CloseSelf(true);
        }

        public void OnClick_CloseSelectMark() {
            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnCloseSelectMark);
            OnClick_Close();
        }

        public void OnClick_TelNpc(uint npcId) {
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(npcId);
            if (null == cSVNpcData)
                return;

            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(cSVNpcData.mapId);
            if (null == cSVMapInfoData)
                return;

            if (Sys_Fight.Instance.IsFight()) {
                Sys_Hint.Instance.PushForbidOprationInFight();
                return;
            }

            if (Sys_Npc.Instance.IsTaskNpc(cSVNpcData)) {
                //ActionCtrl.Instance.MoveToTargetNPC(npcId);
                //UIManager.CloseUI(EUIID.UI_MapExploreDetail);
                //UIManager.CloseUI(EUIID.UI_Map);

                MoveToTragetNpc(npcId);
            }
            else if (Sys_Npc.Instance.IsTransmitNpc(cSVNpcData)) {
                if (Sys_Npc.Instance.IsActivatedNpc(npcId)) {
                    MoveToTragetNpc(npcId);
                }
            }
            else if (isMarkNpc(cSVNpcData)) {
                MoveToTragetNpc(npcId);
            }

            Sys_Adventure.Instance.eventEmitter.Trigger(Sys_Adventure.EEvents.OnCLoseAdventureView);
        }

        #endregion

        #region 提供功能

        private bool isMarkNpc(CSVNpc.Data cSVNpcData) {
            return Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Collection) ||
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Lumbering) ||
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Fishing) ||
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Mining) ||
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.Hunting) ||
                   
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.MiningNew) ||
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.LumberingNew) ||
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.CollectionNew) ||
                   Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.ResourcesNew);
        }

        private void MoveToTragetNpc(uint npcid) {
            MoveTargetNpcID = npcid;


            if (Sys_SurvivalPvp.Instance.isSurvivalPvpMap(Sys_Map.Instance.CurMapId)) {
                Sys_SurvivalPvp.Instance.OpenTipsDialog(DoMoveToTarget);
            }
            else {
                DoMoveToTarget();
            }
        }

        private void DoMoveToTarget() {
            if (MoveTargetNpcID == 0) {
                return;
            }

            ActionCtrl.Instance.MoveToTargetNPC(MoveTargetNpcID);
            UIManager.CloseUI(EUIID.UI_MapExploreDetail);
            UIManager.CloseUI(EUIID.UI_Map);

            MoveTargetNpcID = 0;
        }

        #endregion
    }
}