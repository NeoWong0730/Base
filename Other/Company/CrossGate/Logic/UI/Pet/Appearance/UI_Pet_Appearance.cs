using Framework;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace Logic
{

    public class PetAppearancePetIconCeil
    {
        private Transform transform;
        private Image qualuty;
        private Image icon;
        private Text nameText;
        private GameObject selectGo;
        private CP_Toggle cpToggle;
        public void Init(Transform _transform)
        {
            transform = _transform;
            cpToggle = transform.GetComponent<CP_Toggle>();
            icon = transform.Find("Image_bg/Image_Icon").GetComponent<Image>();
            nameText = transform.Find("Image_Name/Text").GetComponent<Text>();
            selectGo = transform.Find("Select").gameObject;
        }

        public void SetData(uint petId, bool select, int index)
        {
            var petData = CSVPetNew.Instance.GetConfData(petId);
            cpToggle.id = index;
            ImageHelper.SetIcon(icon, petData.icon_id);
            TextHelper.SetText(nameText, petData.name);
            selectGo.gameObject.SetActive(select);
        }
    }

    public class PetAppearanceInfoCeil
    {
        private Transform transform;
        private Image petHeadImage;
        private Text nameText;
        private GameObject selectGo;
        private GameObject lockGo;
        private GameObject newGo;
        private CP_Toggle cpToggle;
        public void Init(Transform _transform)
        {
            transform = _transform;
            cpToggle = transform.GetComponent<CP_Toggle>();
            nameText = transform.Find("Image_Name/Text").GetComponent<Text>();
            selectGo = transform.Find("Image_Select").gameObject;
            lockGo = transform.Find("Image_None").gameObject;
            newGo = transform.Find("Image_New").gameObject;
            petHeadImage = transform.Find("Image_Icon").GetComponent<Image>();
        }

        public void SetData(uint petAppearanId,int index, bool select)
        {
            var petFashionData = CSVPetFashion.Instance.GetConfData(petAppearanId);
            cpToggle.id = index;
            if (null != petFashionData)
            {
                TextHelper.SetText(nameText, petFashionData.FashionName);
                ImageHelper.SetIcon(petHeadImage, null, petFashionData.Icon_Path, true);
                lockGo.SetActive(!Sys_Pet.Instance.IsHasPetFashin(petAppearanId, 0));
                selectGo.SetActive(select);
                newGo.SetActive(petFashionData.Recommend);
            }
        }
    }

    public class PetAppearanceColorInfoCeil
    {
        private Transform transform;
        private Image colorImage;
        private GameObject initColorGo;
        private GameObject selectGo;
        private GameObject lockGo;
        private CP_Toggle cpToggle;
        public void Init(Transform _transform)
        {
            transform = _transform;
            cpToggle = transform.GetComponent<CP_Toggle>();
            selectGo = transform.Find("Image_Select").gameObject;
            lockGo = transform.Find("Image_Lock").gameObject;
            initColorGo = transform.Find("Text").gameObject;
            colorImage = transform.Find("Image_Color").GetComponent<Image>();
        }

        public void SetData(uint petAppearanId, int index, bool select)
        {
            var petFashionData = CSVPetFashion.Instance.GetConfData(petAppearanId);
            cpToggle.id = index;
            if (null != petFashionData)
            {
                initColorGo.SetActive(index == 0);
                if(null != petFashionData.WeaponColour && index >= 0 && index < petFashionData.WeaponColour.Count)
                {
                    var color = new Color(petFashionData.WeaponColour[index][0] / 255f, petFashionData.WeaponColour[index][1] / 255f, petFashionData.WeaponColour[index][2] / 255f);
                    colorImage.color = color;
                }
                lockGo.SetActive(index != 0 && !Sys_Pet.Instance.IsHasPetFashin(petAppearanId, index));
            }
            selectGo.SetActive(select);
        }
    }

    public class UI_Pet_Appearance_Layout
    {
        private Button closeBtn;
        private Dropdown dropdown;
        /// <summary> 宠物无限滚动 </summary>
        private InfinityGrid petIconInfinityGrid;
        /// <summary> 外观无限滚动 </summary>
        private InfinityGrid petAppearanceInfinityGrid;
        /// <summary> 外观无限滚动 </summary>
        private InfinityGrid petAppearanceColorInfinityGrid;
        private CP_ToggleRegistry selectPetToggle;
        private CP_ToggleRegistry selectPetAppearanceToggle;
        private CP_ToggleRegistry selectPetAppearanceColorToggle;
        public Transform rightViewTransfrom;
        public void Init(Transform transform)
        {
            petIconInfinityGrid = transform.Find("Animator/View_Left/List_Pet").GetComponent<InfinityGrid>();
            petAppearanceInfinityGrid = transform.Find("Animator/View_Left/List_Fashion").GetComponent<InfinityGrid>();
            petAppearanceColorInfinityGrid = transform.Find("Animator/View_Right/View_Color/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            dropdown = transform.Find("Animator/View_Left/Dropdown").GetComponent<Dropdown>();
            PopdownListBuild();
            selectPetToggle = transform.Find("Animator/View_Left/List_Pet/Viewport/Content").GetComponent<CP_ToggleRegistry>();
            selectPetAppearanceToggle = transform.Find("Animator/View_Left/List_Fashion/Viewport/Content").GetComponent<CP_ToggleRegistry>();
            selectPetAppearanceColorToggle = transform.Find("Animator/View_Right/View_Color/Scroll View/Viewport/Content").GetComponent<CP_ToggleRegistry>();
            rightViewTransfrom = transform.Find("Animator/View_Right");
        }

        public void SetPetToggleMoveTo(int index, bool sendMessage)
        {
            selectPetToggle.SwitchTo(index, sendMessage: sendMessage);
        }

        public void SetPetAppearanceToggleMoveTo(int index, bool sendMessage)
        {
            selectPetAppearanceToggle.SwitchTo(index, sendMessage: sendMessage);
        }

        public void SetPetAppearanceColorToggleMoveTo(int index, bool sendMessage)
        {
            selectPetAppearanceColorToggle.SwitchTo(index, sendMessage: sendMessage);
        }

        public void SetPetInfinityGridCell(int count)
        {
            petIconInfinityGrid.CellCount = count;
            petIconInfinityGrid.ForceRefreshActiveCell();
        }

        public void SetPetAppearanceInfinityGridCell(int count)
        {
            petAppearanceInfinityGrid.CellCount = count;
            petAppearanceInfinityGrid.ForceRefreshActiveCell();
        }

        public void SetPetAppearanceColorInfinityGridCell(int count)
        {
            petAppearanceColorInfinityGrid.CellCount = count;
            petAppearanceColorInfinityGrid.ForceRefreshActiveCell();
        }

        private void PopdownListBuild()
        {
            dropdown.ClearOptions();
            dropdown.options.Clear();
            var genus = Sys_Pet.Instance.PetAppearanceDropDownList;
            for (uint i = 0; i < genus.Count; ++i)
            {
                var id = genus[(int)i];
                if (id == 0) //all
                {
                    Dropdown.OptionData op = new Dropdown.OptionData();
                    op.text = LanguageHelper.GetTextContent(2006003);
                    dropdown.options.Add(op);
                }
                else
                {
                    CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(id);
                    if (null != cSVGenusData)
                    {
                        Dropdown.OptionData op = new Dropdown.OptionData();
                        op.text = LanguageHelper.GetTextContent(cSVGenusData.rale_name);
                        dropdown.options.Add(op);
                    }
                }
                dropdown.SetValueWithoutNotify(0);
            }
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            petIconInfinityGrid.onCreateCell += listener.OnPetIconCreateCell;
            petIconInfinityGrid.onCellChange += listener.OnPetIconCellChange;
            petAppearanceInfinityGrid.onCreateCell += listener.OnPetAppearanceCreateCell;
            petAppearanceInfinityGrid.onCellChange += listener.OnPetAppearanceCellChange;
            petAppearanceColorInfinityGrid.onCreateCell += listener.OnPetAppearanceColorCreateCell;
            petAppearanceColorInfinityGrid.onCellChange += listener.OnPetAppearanceColorCellChange;
            dropdown.onValueChanged.AddListener(listener.OnValueChanged);
            selectPetToggle.onToggleChange = listener.OnSelectPetChange;
            selectPetAppearanceToggle.onToggleChange = listener.OnSelectPetAppearanceChange;
            selectPetAppearanceColorToggle.onToggleChange = listener.OnSelectPetAppearanceColorChange;
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnPetIconCreateCell(InfinityGridCell cell);
            void OnPetIconCellChange(InfinityGridCell cell, int index);
            void OnPetAppearanceCreateCell(InfinityGridCell cell);
            void OnPetAppearanceCellChange(InfinityGridCell cell, int index);
            void OnPetAppearanceColorCreateCell(InfinityGridCell cell);
            void OnPetAppearanceColorCellChange(InfinityGridCell cell, int index);
            void OnValueChanged(int index);
            void OnSelectPetChange(int curToggle, int old);
            void OnSelectPetAppearanceChange(int curToggle, int old);
            void OnSelectPetAppearanceColorChange(int curToggle, int old);
        }
    }

    public class UI_Pet_AppearanceParam
    {
        /// <summary> 选择的种族下标 </summary>
        public int SelectGenuIndex { get; set; } = -1;
        /// <summary> 选择的宠物下标 </summary>
        public int SelectPetIndex { get; set; } = -1;
        /// <summary> 选中的宠物的外观下标 </summary>
        public int SelectPetAppearanceIndex { get; set; } = -1;
        /// <summary> 选中的外观染色方案下标 </summary>
        public int SelectPetAppearanceColorIndex { get; set; } = -1;
        /// <summary> 种族选项是否变化 </summary>
        public bool SelectGenuChange { get; set; } = false;
        /// <summary> 宠物选项是否变化 </summary>
        public bool SelectPetChange { get; set; } = false;
        /// <summary> 外观选项是否变化 </summary>
        public bool SelectPetAppearanceChange { get; set; } = false;

        /// <summary> 宠物种族字典 </summary>
        private Dictionary<uint, List<uint>> petIdDic = new Dictionary<uint, List<uint>>(9);

        /// <summary> 宠物列表数量 </summary>
        public int PetCount
        {
            get
            {
                return PetIds.Count;
            }
            private set { }
        }
        /// <summary> 宠物列表 </summary>
        public List<uint> PetIds
        {
            get
            {
                var genu = Sys_Pet.Instance.PetAppearanceDropDownList[SelectGenuIndex];
                if(petIdDic.TryGetValue(genu, out List<uint> petIds))
                {
                    return petIds;
                }
                else
                {
                   List<uint> _petIds =  Sys_Pet.Instance.GetPossessAppearancePetIdsByPetGenu(genu);
                    petIdDic.Add(genu, _petIds);
                    return _petIds;
                }
            }
            private set {}
        }

        /// <summary> 宠物外观数量 </summary>
        public int PetAppearanceCount
        {
            get
            {
                return PetAppearanceIds.Count;
            }
            private set { }
        }
        /// <summary> 宠物外观列表 </summary>
        public List<uint> PetAppearanceIds {
            get
            {
                return Sys_Pet.Instance.GetPetAppearanceIdsByPetId(CurrentPetId);
            }
            private set{}
        }

        /// <summary> 宠物外观染色数量 </summary>
        public int PetAppearanceColorCount
        {
            get
            {
                return PetAppearanceColorIds.Count;
            }
            private set { }
        }
        /// <summary> 宠物外观染色列表 </summary>
        public List<uint> PetAppearanceColorIds
        {
            get
            {
                return Sys_Pet.Instance.GetPetAppearanceColorIdsByPetIdAndAppearanceId(CurrentPetId, CurrentPetAppearanceId);
            }
            private set { }
        }

        /// <summary>
        /// 当前选中的宠物id
        /// </summary>
        public uint CurrentPetId
        {
            get
            {
                if(SelectPetIndex >= 0 && SelectPetIndex < PetIds.Count)
                    return PetIds[SelectPetIndex];
                return 0;
            }
            set
            {
                SelectGenuIndex = 0;
                SelectPetAppearanceIndex = 0;
                SelectPetAppearanceColorIndex = 0;
                var _count = PetIds.Count;
                for (int i = 0; i < _count; i++)
                {
                    if(PetIds[i] == value)
                    {
                        SelectPetIndex = i;
                    }
                }
            }
        }

        /// <summary>
        /// 当前选中的宠物外观id
        /// </summary>
        public uint CurrentPetAppearanceId
        {
            get
            {
                if (SelectPetAppearanceIndex >= 0 && SelectPetAppearanceIndex < PetAppearanceIds.Count)
                    return PetAppearanceIds[SelectPetAppearanceIndex];
                return 0;
            }
            private set { }
        }

        /// <summary>
        /// 当前选中的宠物外观染色id
        /// </summary>
        public uint CurrentPetAppearanceColorId
        {
            get
            {
                if (SelectPetAppearanceColorIndex >= 0 && SelectPetAppearanceColorIndex < PetAppearanceColorIds.Count)
                    return PetAppearanceColorIds[SelectPetAppearanceColorIndex];
                return 0;
            }
            private set { }
        }

        public bool isExternalCall = false;

        public bool isFirstOpen = true;
    }

    public class UI_Pet_AppearanceModelShow
    {
        public class AppearanceEntry
        {
            public uint PetId { get; set; }
            public uint PetAppearanceId { get; set; }
            public int ColorIndex { get; set; }
            public CSVPetNew.Data petData
            {
                get
                {
                    return CSVPetNew.Instance.GetConfData(PetId);
                }
                private set { }
            }

            public CSVPetFashion.Data petAppearanceData
            {
                get
                {
                    return CSVPetFashion.Instance.GetConfData(PetAppearanceId);
                }
                private set { }
            }

            public string ModelPath
            {
                get
                {
                    if (null != petAppearanceData)
                    {
                        var _petAppearanceData = petAppearanceData;
                        if (null != _petAppearanceData.model_show && ColorIndex >= 0 && ColorIndex < _petAppearanceData.model_show.Count)
                        {
                            return _petAppearanceData.model_show[ColorIndex];
                        }
                    }
                    else
                    {
                        if (null != petData)
                        {
                            var _petData = petData;
                            if (null != _petData.model_show)
                            {
                                return _petData.model_show;
                            }
                        }
                    }
                    return string.Empty;
                }
            }
        }

        public Image eventImage;
        public GameObject modelGo;
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private AppearanceEntry appearanceEntry = new AppearanceEntry();
        public void Init(Transform transform)
        {
            eventImage = transform.Find("Animator/EventImage").GetComponent<Image>();

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            assetDependencies = transform.GetComponent<AssetDependencies>();
            _LoadShowScene();
        }

        public void OnDrag(BaseEventData eventData)
        {
            if (null != modelGo)
            {
                PointerEventData ped = eventData as PointerEventData;
                Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
                AddEulerAngles(angle);
            }
        }

        public void AddEulerAngles(Vector3 angle)
        {
            Vector3 ilrTemoVector3 = angle;
            if (showSceneControl.mModelPos.transform != null)
            {
                showSceneControl.mModelPos.transform.Rotate(ilrTemoVector3.x, ilrTemoVector3.y, ilrTemoVector3.z);
            }
        }

        public void UnloadModel()
        {
            _UnloadShowContent();
        }

        public void Hide()
        {
            _UnloadShowContent();
        }

        public void Show()
        {
            _LoadShowScene();
        }

        public void ResetModelInfo(uint petId, uint appearanceId, int appearanceIndex)
        {
            appearanceEntry.PetId = petId;
            appearanceEntry.PetAppearanceId = appearanceId;
            appearanceEntry.ColorIndex = appearanceIndex;
            _UnLoadModel();
            _LoadShowModel();
        }

        private void _UnloadShowContent()
        {
            _UnLoadModel();
            showSceneControl?.Dispose();
            showSceneControl = null;
        }

        private void _UnLoadModel()
        {
            if (null != petDisplay && null != petDisplay.mAnimation)
            {
                petDisplay.mAnimation.StopAll();
            }
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            modelGo = null;
            petDisplay = null;
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }
            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(100, 0, 0);
            showSceneControl.Parse(sceneModel);
        }

        private void _LoadShowModel()
        {
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }

            string _modelPath = appearanceEntry.ModelPath;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, appearanceEntry.ModelPath, EPetModelParts.None, null);

            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            var petData = appearanceEntry.petData;
            if(null != petData)
            {
                showSceneControl.mModelPos.transform.eulerAngles = new Vector3(petData.angle1, petData.angle2, petData.angle3);
                showSceneControl.mModelPos.transform.localScale = new Vector3(petData.size, petData.size, petData.size);
                showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + petData.translation, petData.height, showSceneControl.mModelPos.transform.localPosition.z);
            }
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                var petData = appearanceEntry.petData;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                modelGo.SetActive(false);
                petDisplay.mAnimation.UpdateHoldingAnimations(petData.action_id_show, petData.weapon, Constants.PetShowAnimationClipHashSet, go: modelGo);
            }
        }
    }

    public class UI_Pet_Appearance_RightView
    {
        public class AppearanceItemCost : ItemCost
        {
            public override void SetGameObject(GameObject go)
            {
                this.gameObject = go;
                content = go.transform.Find("Text_Value").GetComponent<Text>();
                icon = go.transform.Find("Image_Icon").GetComponent<Image>();
            }
        }
        public class UI_Pet_Appearance_RightViewEntry
        {
            public uint petAppearanId;
            public int index;
        }
        private Text petAppearanceNameText;
        private Transform transform;
        private Transform selfAttrTransform;
        private Transform gemuAttrTransform;
        private Transform allAttrTransform;
        private GameObject buyGo;
        private GameObject equipGo;
        private Button buyBtn;
        private Button colorBuyBtn;
        private Button equipBtn;
        private AppearanceItemCost buyItemCost = new AppearanceItemCost();
        private AppearanceItemCost unLockitemCost = new AppearanceItemCost();
        private UI_Pet_Appearance_RightViewEntry rightEntry = new UI_Pet_Appearance_RightViewEntry();
        public void Init(Transform transform)
        {
            this.transform = transform;
            petAppearanceNameText = transform.Find("Image_Name/Text").GetComponent<Text>();
            selfAttrTransform = transform.Find("View_Addition/Grid_Attr01");
            gemuAttrTransform = transform.Find("View_Addition/Grid_Attr02");
            allAttrTransform = transform.Find("View_Addition/Grid_Attr03");
            buyItemCost.SetGameObject(transform.Find("View_Buy/Consume").gameObject);
            unLockitemCost.SetGameObject(transform.Find("View_Apparel/Consume").gameObject);
            buyGo = transform.Find("View_Buy").gameObject;
            equipGo = transform.Find("View_Apparel").gameObject;

            buyBtn = transform.Find("View_Buy/Btn_Buy").GetComponent<Button>();
            buyBtn.onClick.AddListener(BuyBtnClicked);
            colorBuyBtn = transform.Find("View_Apparel/Btn_Unlock").GetComponent<Button>();
            colorBuyBtn.onClick.AddListener(ColorBuyBtnClicked);
            equipBtn = transform.Find("View_Apparel/Btn_Apparel").GetComponent<Button>();
            equipBtn.onClick.AddListener(EquipBuyBtnClicked);
        }

        private void BuyBtnClicked()
        {
            if (rightEntry.index != 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591001103u));
            }
            else
            {
                var petFashion = CSVPetFashion.Instance.GetConfData(rightEntry.petAppearanId);
                if (null != petFashion)
                {
                    ItemIdCount itemIdCount = new ItemIdCount(1, petFashion.FashionPrice);
                    bool isEnough = itemIdCount.Enough;
                    if (!isEnough)
                    {
                        Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)itemIdCount.id, itemIdCount.count);
                        return;
                    }
                    else
                    {
                        PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(591001102u,
                            LanguageHelper.GetTextContent(petFashion.FashionName)),
                            0,
                            () =>
                            {
                                Sys_Pet.Instance.PetActiveFashionReq(petFashion.id);
                            });
                    }
                }
                
            }
        }

        private void ColorBuyBtnClicked()
        {
            var petFashion = CSVPetFashion.Instance.GetConfData(rightEntry.petAppearanId);
            if (null != petFashion)
            {
                int index = rightEntry.index;
                uint cost = 0;
                if(null != petFashion.itemNeed && index >= 0 && index < petFashion.itemNeed.Count)
                {
                    cost = petFashion.itemNeed[index];
                }
                ItemIdCount itemIdCount = new ItemIdCount(1, cost);
                bool isEnough = itemIdCount.Enough;
                if (!isEnough)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591001101u));
                    return;
                }
                else
                {
                    PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(591001100u, index.ToString()),
                        0,
                        () =>
                        {
                            Sys_Pet.Instance.PetActiveFashionColorReq(petFashion.id, (uint)index);
                        });
                }
            }
        }

        private void EquipBuyBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Pet_FashionApparel, false, (uint)(rightEntry.petAppearanId * 10 + rightEntry.index));
        }

        public void SetAttrView(uint appearanceId)
        {
            rightEntry.petAppearanId = appearanceId;
            var petFashion = CSVPetFashion.Instance.GetConfData(appearanceId);
            if (null != petFashion)
            {
                TextHelper.SetText(petAppearanceNameText, petFashion.FashionName);
                SetAttrInfo(selfAttrTransform, petFashion.attr1_id);
                SetAttrInfo(gemuAttrTransform, petFashion.attr2_id);
                SetAttrInfo(allAttrTransform, petFashion.attr3_id);
                transform.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }

        public void SetActiveInfo(int appearanceIndex)
        {
            rightEntry.index = appearanceIndex;
            bool isBuy = Sys_Pet.Instance.IsHasPetFashin(rightEntry.petAppearanId, 0);//是否购买初始外观
            buyGo.SetActive(!isBuy);
            equipGo.SetActive(isBuy);
            var petFashion = CSVPetFashion.Instance.GetConfData(rightEntry.petAppearanId);
            if (null != petFashion)
            {
                if (isBuy)
                {
                    isBuy = Sys_Pet.Instance.IsHasPetFashin(rightEntry.petAppearanId, appearanceIndex);//是否购买颜色外观 index = 0 默认购买
                    equipBtn.gameObject.SetActive(isBuy);
                    unLockitemCost.gameObject.SetActive(!isBuy);
                    colorBuyBtn.gameObject.SetActive(!isBuy);
                    if (!isBuy)
                    {
                        if (null != petFashion.itemNeed)
                        {
                            int index = appearanceIndex - 1;
                            if (index >= 0 && index < petFashion.itemNeed.Count)
                            {
                                ItemIdCount itemIdCount = new ItemIdCount(1, (long)petFashion.itemNeed[appearanceIndex - 1]);
                                unLockitemCost.Refresh(itemIdCount);
                            }
                        }
                    }
                }
                else
                {
                    ImageHelper.SetImageGray(buyBtn, (appearanceIndex != 0), true);
                    ItemIdCount itemIdCount = new ItemIdCount(1, (long)petFashion.FashionPrice);
                    buyItemCost.Refresh(itemIdCount);
                }
                transform.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
            }
        }

        public void SetAttrInfo(Transform attrTran, List<List<uint>> attrsData)
        {
            int count = null == attrsData ? 0 : attrsData.Count;
            FrameworkTool.CreateChildList(attrTran, count);
            for (int i = 0; i < count; i++)
            {
                Transform attr = attrTran.GetChild(i);
                var nameText = attr.Find("Text").GetComponent<Text>();
                var valueText = attr.Find("Text_Num").GetComponent<Text>();
                CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData((uint)attrsData[i][0]);
                TextHelper.SetText(nameText, attrInfo.name);
                float attr_value = attrsData[i][1];
                if (attr_value >= 0)
                {
                    TextHelper.SetText(valueText, LanguageHelper.GetTextContent(2006142u, (attrInfo.show_type == 1 ? attr_value.ToString("0.#") : (attr_value / 100.0f).ToString("0.#") + "%")));
                }
                else
                {
                    TextHelper.SetText(valueText, (attrInfo.show_type == 1 ? attr_value.ToString("0.#") : (attr_value / 100.0f).ToString("0.#") + "%"));
                }
            }
        }
    }

    public class UI_Pet_Appearance : UIBase, UI_Pet_Appearance_Layout.IListener
    {
        private UI_Pet_Appearance_Layout layout = new UI_Pet_Appearance_Layout();
        private UI_Pet_AppearanceParam param = new UI_Pet_AppearanceParam();
        private UI_Pet_AppearanceModelShow modelShow = new UI_Pet_AppearanceModelShow();
        private UI_Pet_Appearance_RightView rightView = new UI_Pet_Appearance_RightView();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            modelShow.Init(transform);
            rightView.Init(layout.rightViewTransfrom);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnBuyOrActivePetAppearance, OnBuyOrActivePetAppearance, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            if (null != arg)
            {
                var willOpenPetAppearance = Convert.ToUInt32(arg);
                if(willOpenPetAppearance != 0)
                    param.CurrentPetId = willOpenPetAppearance / 10;
            }
        }

        protected override void OnShow()
        {
            if (param.isFirstOpen)
            {
                param.isFirstOpen = false;
                OnValueChanged(param.isExternalCall ? param.SelectGenuIndex : 0);
            }
            else
            {
                /*layout.SetPetInfinityGridCell(param.PetCount);
                layout.SetPetAppearanceInfinityGridCell(param.PetAppearanceCount);
                layout.SetPetAppearanceColorInfinityGridCell(param.PetAppearanceColorCount);*/
                modelShow.Show();
                SetView();
            }
        }

        protected override void OnHide()
        {
            modelShow.Hide();
        }

        protected override void OnDestroy()
        {
        }

        /// <summary>
        /// 宠物
        /// </summary>
        /// <param name="cell"></param>
        public void OnPetIconCreateCell(InfinityGridCell cell)
        {
            PetAppearancePetIconCeil entry = new PetAppearancePetIconCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 宠物
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnPetIconCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= param.PetCount)
                return;
            PetAppearancePetIconCeil entry = cell.mUserData as PetAppearancePetIconCeil;
            var petId = param.PetIds[index];
            entry.SetData(petId, petId == param.CurrentPetId, index);
        }

        /// <summary>
        /// 外观
        /// </summary>
        /// <param name="cell"></param>
        public void OnPetAppearanceCreateCell(InfinityGridCell cell)
        {
            PetAppearanceInfoCeil entry = new PetAppearanceInfoCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 外观
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnPetAppearanceCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= param.PetAppearanceCount)
                return;
            PetAppearanceInfoCeil entry = cell.mUserData as PetAppearanceInfoCeil;
            var petAppearanceId = param.PetAppearanceIds[index];
            entry.SetData(petAppearanceId, index, petAppearanceId == param.CurrentPetAppearanceId);
        }

        /// <summary>
        /// 染色
        /// </summary>
        /// <param name="cell"></param>
        public void OnPetAppearanceColorCreateCell(InfinityGridCell cell)
        {
            PetAppearanceColorInfoCeil entry = new PetAppearanceColorInfoCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 染色
        /// </summary>
        /// <param name="cell"></param>
        public void OnPetAppearanceColorCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= param.PetAppearanceColorCount)
                return;
            PetAppearanceColorInfoCeil entry = cell.mUserData as PetAppearanceColorInfoCeil;
            entry.SetData(param.CurrentPetAppearanceId, index, index == param.CurrentPetAppearanceColorId);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Appearance);
        }

        public void OnValueChanged(int index)
        {
            if (param.SelectGenuIndex == index || index < 0)
                return;
            param.SelectGenuIndex = index;
            var dropDownList = Sys_Pet.Instance.PetAppearanceDropDownList;
            if (index < dropDownList.Count)
            {
                var genu = Sys_Pet.Instance.PetAppearanceDropDownList[index];
                param.SelectGenuChange = true;
                param.SelectPetIndex = param.isExternalCall ? param.SelectPetIndex : 0;
                layout.SetPetInfinityGridCell(param.PetCount);
                OnSelectPetChange(param.SelectPetIndex, -1);
            }
        }

        public void OnSelectPetChange(int curToggle, int old)
        {
            if (!param.SelectGenuChange && curToggle == old)
                return;
            param.SelectPetIndex = curToggle;
            param.SelectGenuChange = false;
            param.SelectPetChange = true;
            layout.SetPetAppearanceInfinityGridCell(param.PetAppearanceCount);
            param.SelectPetAppearanceIndex = param.isExternalCall ? param.SelectPetAppearanceIndex : 0;
            OnSelectPetAppearanceChange(param.SelectPetAppearanceIndex, -1);
        }

        public void OnSelectPetAppearanceChange(int curToggle, int old)
        {
            if (!param.SelectPetChange && curToggle == old)
                return;
            param.SelectPetAppearanceIndex = curToggle;
            rightView.SetAttrView(param.CurrentPetAppearanceId);
            param.SelectPetChange = false;
            param.SelectPetAppearanceChange = true;
            layout.SetPetAppearanceColorInfinityGridCell(param.PetAppearanceColorCount);
            param.SelectPetAppearanceColorIndex = param.isExternalCall ? param.SelectPetAppearanceColorIndex : 0;
            OnSelectPetAppearanceColorChange(param.SelectPetAppearanceColorIndex, -1);
        }

        public void OnSelectPetAppearanceColorChange(int curToggle, int old)
        {
            if (!param.SelectPetAppearanceChange && curToggle == old)
                return;
            param.SelectPetAppearanceColorIndex = curToggle;
            param.SelectPetAppearanceChange = false;
            param.isExternalCall = false;
            SetView();
        }

        private void SetView()
        {
            var petId = param.CurrentPetId;
            var petAppearanceId = param.CurrentPetAppearanceId;
            var index = (int)param.CurrentPetAppearanceColorId;
            modelShow.ResetModelInfo(petId, petAppearanceId, index);
            rightView.SetActiveInfo(index);
        }

        private void OnBuyOrActivePetAppearance(uint type)
        {
            var index = (int)param.CurrentPetAppearanceColorId;
            rightView.SetActiveInfo(index);
            if(type == 2)
            {
                layout.SetPetAppearanceColorInfinityGridCell(param.PetAppearanceColorCount);
            }
            else
            {
                layout.SetPetAppearanceInfinityGridCell(param.PetAppearanceCount);
            }
        }
    }
}
