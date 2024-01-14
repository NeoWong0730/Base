using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public class TransformCell
    {
        public Transform transform;
        private Image icon;
        private Image quality;
        private GameObject lockGo;
        private GameObject selectGo;
        private Text lockedLv;
        private Text count;
        private Text type;
        private Button button;
        public Action<ulong> action;

        private uint curId;
        private ulong Uuid;
        private bool isTransformShow;
        private bool isBagShow;
        private CSVRaceChangeCard.Data data;

        public void Init(Transform _transform)
        {
            transform = _transform;
            icon = transform.Find("Icon").GetComponent<Image>();
            quality = transform.GetComponent<Image>();
            lockedLv = transform.Find("Lock/Text_Level").GetComponent<Text>();
            count = transform.Find("Text_Value").GetComponent<Text>();
            type = transform.Find("Type/Text").GetComponent<Text>();
            lockGo = transform.Find("Lock").gameObject;
            selectGo = transform.Find("Image_Select").gameObject;
            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        public void SetData(uint _id, bool _isTransformShow, bool _isBagShow=false, ulong _uuid=0)
        {
            curId = _id;
            isTransformShow = _isTransformShow;
            isBagShow = _isBagShow;
            Uuid = _uuid;
            data = CSVRaceChangeCard.Instance.GetConfData(curId);
            RefreshView(); 
        }

        public void RefreshView()
        {
            if (data == null)
            {
                return;
            }
            CSVItem.Data csvItemData = CSVItem.Instance.GetConfData(curId);
            uint petId = CSVTransform.Instance.GetConfData(data.change_id).petid;
            CSVPetNew.Data csvPetData = CSVPetNew.Instance.GetConfData(petId);
            if (csvPetData != null&& csvItemData!=null)
            {
                ImageHelper.SetIcon(icon, csvPetData.bust);
                if (csvItemData.quality == 3)
                {
                    ImageHelper.SetIcon(quality, 6100301);
                }
                else if (csvItemData.quality == 4)
                {
                    ImageHelper.SetIcon(quality, 6100302);
                }
                else if (csvItemData.quality == 5)
                {
                    ImageHelper.SetIcon(quality, 6100303);
                }
            }
            TextHelper.SetText(type,Sys_Transfiguration.Instance.GetTypeLanIdByCareer(data.career));
            ItemData itemData = Sys_Transfiguration.Instance.GetTransfornCardItemData(Uuid);
            uint num = itemData == null ? 0 : itemData.Count;
            if (isTransformShow)
            {
                if (isBagShow)
                {
                    count.text = num.ToString();
                }
                else
                {
                    count.text = Sys_Transfiguration.Instance.GetTransfornCardItemCount(curId).ToString();
                }
                CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(data.need_race_lv);
                CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(data.type);
                uint raceReId = 0;
                ClientShapeShiftPlan clientShapeShiftPlan = Sys_Transfiguration.Instance.clientShapeShiftPlans[(int)Sys_Transfiguration.Instance.curIndex];
                if (clientShapeShiftPlan.shapeShiftRaceSubNodes.ContainsKey(csvGenusData.id))
                {
                    raceReId = clientShapeShiftPlan.shapeShiftRaceSubNodes[csvGenusData.id].Subnodeid;
                }
                else
                {
                    raceReId = csvGenusData.race_change_id;
                }
                CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchDataUnLock = CSVRaceDepartmentResearch.Instance.GetConfData(raceReId);
                if (csvRaceDepartmentResearchData.rank <= csvRaceDepartmentResearchDataUnLock.rank && csvRaceDepartmentResearchData.level <= csvRaceDepartmentResearchDataUnLock.level)
                {
                    lockGo.SetActive(false);
                }
                else
                {
                    lockGo.SetActive(true);
                    TextHelper.SetText(lockedLv, 2013017, LanguageHelper.GetTextContent(csvGenusData.rale_name), csvRaceDepartmentResearchData.rank.ToString(), csvRaceDepartmentResearchData.level.ToString());
                }
            }
            else
            {
                count.text = string.Empty;
                lockGo.SetActive(false);
            }              
        }

        public void SetSelect(ulong selectId)
        {
            if (isBagShow)
            {
                selectGo.SetActive(Uuid == selectId);
            }
            else
            {
                selectGo.SetActive(curId == selectId);
            }
        }

        public void AddAction(Action<ulong> action)
        {
            this.action = action;
        }

        private void OnClicked()
        {
            if (isBagShow)
            {
                action?.Invoke(Uuid);
            }
            else
            {
                action?.Invoke(curId);
            }
        }
    }

    public class TransformAttrAddItem
    {
        public Transform transform;
        private Text attrName;
        private Text attrNumber;
        private Text attrAdd;

        private int attrId;
        private int attrValue;
        private float addNum;

        public void Init(Transform _transform)
        {
            transform = _transform;
            attrName = transform.Find("Text_Name").GetComponent<Text>();
            attrNumber = transform.Find("Text_Value").GetComponent<Text>();
            attrAdd = transform.Find("Text_Add").GetComponent<Text>();
        }

        public void SetData(int _attrId, int _attrValue,float _addNum)
        {
            attrId = _attrId;
            attrValue = _attrValue;
            addNum = _addNum;
            CSVAttr.Data csvAttrData = CSVAttr.Instance.GetConfData((uint)attrId);
            if (csvAttrData != null)
            {
                TextHelper.SetText(attrName,csvAttrData.name);
                float number = attrValue;
                float add = (float)addNum/100;
                if (attrValue>0)
                {
                    if (csvAttrData.show_type == 1)
                    {
                        attrNumber.text = "+" + number.ToString();
                        attrAdd.text = "(+" + (number*add).ToString("0.00") + ")";
                    }
                    else
                    {
                        number = (float)attrValue / 100;
                        attrNumber.text = "+" + number.ToString() + "%";
                        attrAdd.text = "(+" + (number * add).ToString("0.00") + "%)";
                    }
                }
                else
                {
                    if (csvAttrData.show_type == 1)
                    {
                        attrNumber.text = number.ToString();
                        attrAdd.text = "(+" + (number * add).ToString("0.00") + ")";
                    }
                    else
                    {
                        number = (float)attrValue / 100;
                        attrNumber.text = number.ToString() + "%";
                        attrAdd.text = "(+" + (number * add).ToString("0.00") + "%)";
                    }
                }
            }

            attrAdd.gameObject.SetActive(false);
        }

        public void ShowAddAttr(bool isShow)
        {
            attrAdd.gameObject.SetActive(isShow);
        }
    }

    public class TransformCardSelectTab
    {
        public Transform transform;
        public Toggle toggle;
        private Text name;
        private GameObject selectGo;
        private uint Id;

        public void Init(Transform _transform)
        {
            transform = _transform;
            name = transform.Find("Text").GetComponent<Text>();
            selectGo= transform.Find("Image_Select").gameObject;
            toggle = transform.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Transfiguration.Instance.curSortType = Id;
                Sys_Transfiguration.Instance.eventEmitter.Trigger<uint>(Sys_Transfiguration.EEvents.OnSelectCardSort, Id);
            }
        }

        public void SetData(uint _Id)
        {
            Id = _Id;
            TextHelper.SetText(name, Sys_Transfiguration.Instance.GetSortTypeLanIdBySortId(Id));
        }

        public void SetSelectGo(uint selectId)
        {
            selectGo.SetActive(Id== selectId);
        }
    }

    public class UI_Transform : UIComponent
    {
        private Text name;
        private Text type;
        private Text time;
        private Text add;
        private Text count;
        private Text sortType;
        private Text race;

        private Button btn_Study;
        private Button btn_Handbook;
        private Button btn_Handbook_Small;
        private Button btn_Study_Small;
        private Button btn_Arrow;
        private Button btn_Show;
        private Button btn_Delete;
        private Button btn_Added;
        private Button btn_Use;
        private Button btn_AddTips;
        private Toggle toggle_ShowAdd;
        private Toggle toggle_UseShapeShift;
        private RawImage rawImage;
        private Image eventImage;

        private GameObject view_Initial;
        private GameObject view_Transform;
        private GameObject go_SelectToggles;
        private GameObject go_AttrRoot;
        private RectTransform scroll_View;

        private Dictionary<GameObject, TransformCell> itemCeils = new Dictionary<GameObject, TransformCell>();
        private List<TransformCell> transformCellItems = new List<TransformCell>();
        private List<TransformAttrAddItem> attrItems = new List<TransformAttrAddItem>();       
        private List<TransformCardSelectTab> selectTabs = new List<TransformCardSelectTab>();

        private List<ItemData> cardItemList = new List<ItemData>();
        private List<ulong> cardUuIdsList = new List<ulong>();
        private int infinityCountCard;
        private InfinityGrid _infinityGrid;
        private uint curCardId;
        private ulong curUuid;
        private uint curPetId;
        private bool isFirstInit = true;
        private CSVRaceChangeCard.Data curCSVRaceChangeCardData;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private ClientShapeShiftPlan clientShapeShiftPlan;

        protected override void Loaded()
        {
            name = transform.Find("View_Main/View_Left/Text_Name").GetComponent<Text>();
            type = transform.Find("View_Main/View_Left/Text_Type").GetComponent<Text>();
            time = transform.Find("View_Main/View_Right/View_Time/TextValue").GetComponent<Text>();
            add = transform.Find("View_Main/View_Right/View_Addition/TextValue").GetComponent<Text>();
            count = transform.Find("View_Main/View_Right/View_Number/TextValue").GetComponent<Text>();
            sortType= transform.Find("View_Main/View_Left/Scroll View/View_Select01/Text").GetComponent<Text>();
            race = transform.Find("View_Main/View_Left/Race/Text_Race").GetComponent<Text>();
            rawImage = transform.Find("View_Main/View_Left/Charapter").GetComponent<RawImage>();
            rawImage.gameObject.SetActive(true);
            eventImage = transform.Find("View_Main/View_Left/EventImage").GetComponent<Image>();

            btn_Study = transform.Find("View_Initial/Btn_Study").GetComponent<Button>();
            btn_Study.onClick.AddListener(On_BtnStudy_Clicked);
            btn_Handbook = transform.Find("View_Initial/Btn_Handbook").GetComponent<Button>();
            btn_Handbook.onClick.AddListener(On_BtnHandbook_Clicked);
            btn_Handbook_Small = transform.Find("View_Main/View_Left/Btn_Handbook").GetComponent<Button>();
            btn_Handbook_Small.onClick.AddListener(On_BtnHandbook_Clicked);
            btn_Study_Small = transform.Find("View_Main/View_Left/Btn_Study").GetComponent<Button>();
            btn_Study_Small.onClick.AddListener(On_BtnStudy_Clicked);

            btn_Arrow = transform.Find("View_Main/View_Left/Scroll View/View_Select01/Btn_Arrow").GetComponent<Button>();
            btn_Arrow.onClick.AddListener(On_BtnArrow_Clicked);
            btn_Show = transform.Find("View_Main/View_Left/Scroll View/Btn_Arrow").GetComponent<Button>();
            btn_Show.onClick.AddListener(On_BtnShow_Clicked);
            btn_Delete = transform.Find("View_Main/View_Right/View_Number/Btn_Delete").GetComponent<Button>();
            btn_Delete.onClick.AddListener(On_BtnDelete_Clicked);
            btn_Added = transform.Find("View_Main/Button_Added").GetComponent<Button>();
            btn_Added.onClick.AddListener(On_BtnAdded_Clicked);
            btn_Use = transform.Find("View_Main/Button_Use").GetComponent<Button>();
            btn_Use.onClick.AddListener(On_BtnUse_Clicked);
            btn_AddTips = transform.Find("View_Main/View_Right/View_Addition/Image_Title").GetComponent<Button>();
            btn_AddTips.onClick.AddListener(On_BtnAddTips_Clicked);
            toggle_ShowAdd = transform.Find("View_Main/View_Right/View_Addition/Toggle").GetComponent<Toggle>();
            toggle_ShowAdd.onValueChanged.AddListener(OnshowAddValueChanged);
            toggle_UseShapeShift = transform.Find("View_Main/Toggle").GetComponent<Toggle>();
            toggle_UseShapeShift.onValueChanged.AddListener(OnuseShapeShiftValueChanged);

            view_Initial = transform.Find("View_Initial").gameObject;
            view_Transform = transform.Find("View_Main").gameObject;
            go_SelectToggles = transform.Find("View_Main/View_Left/Scroll View/View_Select01/Lab_Select").gameObject;
            go_AttrRoot = transform.Find("View_Main/View_Right/View_Attribute/AttributeGroup").gameObject;
            scroll_View= transform.Find("View_Main/View_Left/Scroll View").GetComponent<RectTransform>();

            _infinityGrid = transform.Find("View_Main/View_Left/Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }

        public override void Show()
        {
            gameObject.SetActive(true);
            ShowCardView();
            TextHelper.SetText(sortType, Sys_Transfiguration.Instance.GetSortTypeLanIdBySortId(Sys_Transfiguration.Instance.curSortType));
            btn_Show.transform.localScale = new Vector3(1, 1, 1);
            scroll_View.sizeDelta = new Vector2(560, 125);
            _infinityGrid.AxisLimit = 1;
            toggle_UseShapeShift.SetIsOnWithoutNotify(OptionManager.Instance.GetBoolean(OptionManager.EOptionID.UseShapeShift, false));
        }

        public  void ProcessEvent(bool toRegister)
        {
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnSelectCardSort, OnSelectCardSort, toRegister);
            Sys_Transfiguration.Instance.eventEmitter.Handle<uint>(Sys_Transfiguration.EEvents.OnUseChangeCard, OnUseChangeCard, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
            OptionManager.Instance.eventEmitter.Handle<int>(OptionManager.EEvents.OptionFinalChange, OptionFinalChange, toRegister);
        }

        private void OnSelectCardSort(uint Id)
        {
            go_SelectToggles.SetActive(false);
            TextHelper.SetText(sortType, Sys_Transfiguration.Instance.GetSortTypeLanIdBySortId(Id));
            Sys_Transfiguration.Instance.SortCardListByTab(Id, cardItemList);
            infinityCountCard = cardItemList.Count;
            _infinityGrid.CellCount = infinityCountCard;
            _infinityGrid.ForceRefreshActiveCell();
            for (int i = 0; i < selectTabs.Count; ++i)
            {
                selectTabs[i].SetSelectGo(Id);
            }
            _infinityGrid.MoveIndexToTop(0);
        }

        private void OnUseChangeCard(uint Id)
        {        
            uint lanID = CSVItem.Instance.GetConfData(Id).name_id;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680001002,LanguageHelper.GetTextContent(lanID)));
        }

        private void OnRefreshChangeData(int changeType, int curBoxId)
        {
            isFirstInit = true;
            ShowCardView();
        }


        private void OptionFinalChange(int optionId)
        {
            if (optionId == (int)OptionManager.EOptionID.UseShapeShift)
            {
                if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.UseShapeShift))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2013306));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2013307));
                }
            }
        }

        public override void Hide()
        {
            OnDestroyModel();
            selectTabs.Clear();
            itemCeils.Clear();
            transformCellItems.Clear();
            attrItems.Clear();
            isFirstInit = true;
            curCardId = 0;
            gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            itemCeils.Clear();
            TransformCell entry = new TransformCell();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.AddAction(OnSelectItem);
            cell.BindUserData(entry);
            itemCeils.Add(go, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            ItemData itemData = cardItemList[index];
            TransformCell item = cell.mUserData as TransformCell;
            item.SetData(itemData.Id, true, true, itemData.Uuid);
            transformCellItems.Add(item);
            if (curUuid == itemData.Uuid)
            {
                OnSelectItem(cardItemList[index].Uuid);
            }
            item.SetSelect(curUuid);
        }

        private void OnSelectItem(ulong uuid)
        {
            ItemData itemData = Sys_Transfiguration.Instance.GetTransfornCardItemData(uuid);
            if (curUuid== uuid && ! isFirstInit)
            {
                RefreshView(itemData.Id, itemData.Count);
                return;
            }              
            isFirstInit = false;
            curUuid = uuid;          
            curCSVRaceChangeCardData = CSVRaceChangeCard.Instance.GetConfData(itemData.Id);        
            if (curCSVRaceChangeCardData == null || !cardUuIdsList.Contains(uuid))
            {
                return;
            }
            CSVTransform.Data csvTransformData = CSVTransform.Instance.GetConfData(curCSVRaceChangeCardData.change_id);
            if (csvTransformData == null)
            {
                return;
            }
            curPetId = csvTransformData.petid;
            OnCreateModel();
            RefreshView(itemData.Id, itemData.Count);
            for (int i = 0; i < transformCellItems.Count; ++i)
            {
                transformCellItems[i].SetSelect(curUuid);
            }
        }

        private void RefreshView(uint cardId, ulong itemCount)
        {
            TextHelper.SetText(name, CSVItem.Instance.GetConfData(cardId).name_id);
            TextHelper.SetText(type, Sys_Transfiguration.Instance.GetTypeLanIdByCareer(curCSVRaceChangeCardData.career));
            TextHelper.SetText(time, 10156, (curCSVRaceChangeCardData.last_time / 60).ToString());
            count.text = itemCount.ToString();
            CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(curCSVRaceChangeCardData.need_race_lv);
            CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(curCSVRaceChangeCardData.type);
            race.text = LanguageHelper.GetTextContent(csvGenusData.rale_name);
            uint raceReId = 0;         
            if (clientShapeShiftPlan.shapeShiftRaceSubNodes.ContainsKey(csvGenusData.id))
            {
                raceReId = clientShapeShiftPlan.shapeShiftRaceSubNodes[csvGenusData.id].Subnodeid;
            }
            else
            {
                raceReId = csvGenusData.race_change_id;
            }
            float addNum = 0;
            if (Sys_Transfiguration.Instance.shapeShiftAddDic.ContainsKey(curCSVRaceChangeCardData.type))
            {
                addNum = Sys_Transfiguration.Instance.shapeShiftAddDic[curCSVRaceChangeCardData.type] / 100.0f;
            }
            TextHelper.SetText(add, 10882, addNum.ToString());
            CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchDataUnLock = CSVRaceDepartmentResearch.Instance.GetConfData(raceReId);
            if (csvRaceDepartmentResearchData.rank <= csvRaceDepartmentResearchDataUnLock.rank && csvRaceDepartmentResearchData.level <= csvRaceDepartmentResearchDataUnLock.level)
            {
                ImageHelper.SetImageGray(btn_Use.GetComponent<Image>(), false, true);
            }
            else
            {
                ImageHelper.SetImageGray(btn_Use.GetComponent<Image>(), true, true);
            }
            ShowAttrView();
        }

        private void ShowCardView()
        {
            cardItemList.Clear();
            cardUuIdsList.Clear();
            cardItemList = Sys_Transfiguration.Instance.GetCardItemDataInBag();
            cardUuIdsList = Sys_Transfiguration.Instance.GetCardsIdInBag(cardItemList);
            clientShapeShiftPlan = Sys_Transfiguration.Instance.clientShapeShiftPlans[(int)Sys_Transfiguration.Instance.curIndex];
            Sys_Transfiguration.Instance.SortCardListByTab(Sys_Transfiguration.Instance.curSortType, cardItemList);
            if (cardItemList.Count == 0)
            {
                view_Initial.SetActive(true);
                view_Transform.SetActive(false);
            }
            else
            {
                curUuid = curUuid == 0||!cardUuIdsList.Contains(curUuid) ? cardUuIdsList[0]: curUuid;
                view_Initial.SetActive(false);
                view_Transform.SetActive(true);
                infinityCountCard = cardItemList.Count;
                _infinityGrid.CellCount = infinityCountCard;
                _infinityGrid.ForceRefreshActiveCell();
                SetCardSortView();
                OnSelectItem(curUuid);
            }
        }

        private void ShowAttrView()
        {
            DefaultAttrItems();
            FrameworkTool.CreateChildList(go_AttrRoot.transform, curCSVRaceChangeCardData.base_attr.Count);
            for (int i = 0; i < go_AttrRoot.transform.childCount; ++i)
            {
                TransformAttrAddItem item = new TransformAttrAddItem();
                if (i != 0)
                {
                    go_AttrRoot.transform.GetChild(i).name = curCSVRaceChangeCardData.base_attr[i][0].ToString();
                }
                item.Init(go_AttrRoot.transform.GetChild(i));
                float addNum =0;
                if (Sys_Transfiguration.Instance.shapeShiftAddDic.ContainsKey(curCSVRaceChangeCardData.type))
                {
                    addNum = Sys_Transfiguration.Instance.shapeShiftAddDic[curCSVRaceChangeCardData.type]/100.0f;
                }
                item.SetData(curCSVRaceChangeCardData.base_attr[i][0], curCSVRaceChangeCardData.base_attr[i][1], addNum);
                attrItems.Add(item);
            }
            toggle_ShowAdd.isOn = Sys_Transfiguration.Instance.isToggleShow;
            if (toggle_ShowAdd.isOn)
            {
                for (int i = 0; i < attrItems.Count; ++i)
                {
                    attrItems[i].ShowAddAttr(toggle_ShowAdd.isOn);
                }
            }
        }

        private void DefaultAttrItems()
        {
            attrItems.Clear();
            FrameworkTool.DestroyChildren(go_AttrRoot, go_AttrRoot.transform.GetChild(0).name);
        }

        private void SetCardSortView()
        {
            selectTabs.Clear();
            go_SelectToggles.SetActive(false);
            for (int i=0;i< go_SelectToggles.transform.childCount; ++i)
            {
                TransformCardSelectTab item = new TransformCardSelectTab();
                item.Init(go_SelectToggles.transform.GetChild(i));
                uint.TryParse(go_SelectToggles.transform.GetChild(i).name, out uint id);
                item.SetData(id);
                selectTabs.Add(item);
                if (id == Sys_Transfiguration.Instance.curSortType)
                {
                    item.toggle.isOn = true;
                }
            }        
        }

        #region  ModelShow
        private void OnCreateModel()
        {
            OnDestroyModel();
            _LoadShowScene();
            _LoadShowModel();
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[1] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3((int)(EUIID.UI_Transfiguration_BookList), 0, 0);
            showSceneControl.Parse(sceneModel);
            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel()
        {
            CSVPetNew.Data petdata = CSVPetNew.Instance.GetConfData(curPetId);
            string _modelPath = petdata.model_show;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + petdata.translation, showSceneControl.mModelPos.transform.localPosition.y + petdata.height, showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(petdata.angle1, petdata.angle2, petdata.angle3);
            showSceneControl.mModelPos.transform.localScale = new Vector3(petdata.size, petdata.size, petdata.size);

        }

        public GameObject modelGo;
        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint weaponId = Constants.UMARMEDID;
                uint highId = curPetId;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, 0, modelGo.transform);
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, weaponId,Constants.PetShowAnimationClipHashSet, go: modelGo);
            }
        }

        private void OnDestroyModel()
        {
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            showSceneControl = null;
            petDisplay = null;
            modelGo = null;
            rawImage.texture = null;
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            Vector3 ilrTemoVector3 = angle;
            if (showSceneControl.mModelPos.transform != null)
            {
                showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
            }
        }
        #endregion

        private void On_BtnArrow_Clicked()
        {
            bool isShow = go_SelectToggles.activeInHierarchy;
            go_SelectToggles.SetActive(!isShow);
        }

        private void On_BtnShow_Clicked()
        {
            if (_infinityGrid.AxisLimit == 1)
            {
                btn_Show.transform.localScale = new Vector3(1, -1, 1);
                scroll_View.sizeDelta = new Vector2(560, 250);
                _infinityGrid.AxisLimit = 2;
            }
            else
            {
                btn_Show.transform.localScale = new Vector3(1,1, 1);
                scroll_View.sizeDelta = new Vector2(560, 125);
                _infinityGrid.AxisLimit = 1;
            }
        }

        private void On_BtnDelete_Clicked()
        {
            ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(curUuid);
            UIManager.OpenUI(EUIID.UI_Decompose, false, itemData);
        }

        private void On_BtnAdded_Clicked()
        {
         
        }

        private void On_BtnUse_Clicked()
        {
            ItemData curItemData = Sys_Bag.Instance.GetItemDataByUuid(curUuid);
            if (!Sys_FunctionOpen.Instance.IsOpen(curItemData.cSVItemData.FunctionOpenId,true))
            {
                return;
            }
            CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(curCSVRaceChangeCardData.need_race_lv);
            CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(curCSVRaceChangeCardData.type);
            uint raceReId = 0;
            if (clientShapeShiftPlan.shapeShiftRaceSubNodes.ContainsKey(csvGenusData.id))
            {
                raceReId = clientShapeShiftPlan.shapeShiftRaceSubNodes[csvGenusData.id].Subnodeid;
            }
            else
            {
                raceReId = csvGenusData.race_change_id;
            }
            CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchDataUnLock = CSVRaceDepartmentResearch.Instance.GetConfData(raceReId);
            if (csvRaceDepartmentResearchData.rank <= csvRaceDepartmentResearchDataUnLock.rank && csvRaceDepartmentResearchData.level <= csvRaceDepartmentResearchDataUnLock.level)
            {               
                if (Sys_Attr.Instance.privilegeBuffDic.ContainsKey(5) && Sys_Attr.Instance.privilegeBuffDic[5].Params.Count!=0)
                {
                    uint usedCardId = Sys_Attr.Instance.privilegeBuffDic[5].Params[0];
                   if ( CSVItem.Instance.TryGetValue(usedCardId, out CSVItem.Data itemData))
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(680001001, LanguageHelper.GetTextContent(itemData.name_id), LanguageHelper.GetTextContent(curItemData.cSVItemData.name_id));
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            Sys_Bag.Instance.UseItemByUuid(curItemData.Uuid, 1);
                        });
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    }
                }
                else
                {
                    Sys_Bag.Instance.UseItemByUuid(curItemData.Uuid, 1);
                }                
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2013017, LanguageHelper.GetTextContent(csvGenusData.rale_name), csvRaceDepartmentResearchData.rank.ToString(), csvRaceDepartmentResearchData.level.ToString()));
            }
        }

        private void On_BtnHandbook_Clicked()
        {
            UIManager.OpenUI(EUIID.UI_Transfiguration_BookList);
        }

        private void On_BtnStudy_Clicked()
        {
            UIManager.OpenUI(EUIID.UI_Transfiguration_Study);

        }

        private void On_BtnAddTips_Clicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = 2013037;
            UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
        }

        private void OnshowAddValueChanged(bool isOn)
        {
            Sys_Transfiguration.Instance.isToggleShow = isOn;
            Sys_Transfiguration.Instance.SaveRecordFile();
            for (int i = 0; i < attrItems.Count; ++i)
            {
                attrItems[i].ShowAddAttr(isOn);
            }
        }

        public void OnuseShapeShiftValueChanged(bool isOn)
        {
            OptionManager.Instance.SetBoolean(OptionManager.EOptionID.UseShapeShift, isOn, false);
        }
    }
}
