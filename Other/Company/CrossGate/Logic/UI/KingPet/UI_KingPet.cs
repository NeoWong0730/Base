using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Logic;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{

    public enum EKingPetView
    {
        Draw, //随机兑换金宠
        Compose  //合成金宠
    }

    public class UI_KingPetDraw : UIComponent
    {
        private Button m_KingPetPreView;

        private PropItem m_PropItem;

        private Button m_ExchangeButton;
        private Button m_DrawItemButton;

        private Text m_DrawCount;
        //private Image m_SpecialPetIcon;

        private GameObject m_DrawGo;
        private PropItem m_DrawPropItem;

        private uint m_NeedItemId;
        private uint m_NeedItemCount;

        private Animator m_Animator;
        private Timer m_AnimatorTimer;
        private float m_AnimatorTime;

        private uint m_GuaranteedCount;
        private uint m_CurDrawCount;
        private uint m_SpecialPetId;
        private CSVSpecialGoldPetExchange.Data m_CsvSpecialGoldPetExchange;
        private uint m_ItemId;

        protected override void Loaded()
        {
            Sys_Ini.Instance.Get(1451, out IniElement_IntArray array);
            m_NeedItemId = (uint)array.value[0];
            m_NeedItemCount = (uint)array.value[1];

            Sys_Ini.Instance.Get(1452, out IniElement_IntArray array_1);
            m_GuaranteedCount = (uint)array_1.value[0];

            Sys_Ini.Instance.Get(1453, out IniElement_IntArray array_2);
            m_SpecialPetId = (uint)array_2.value[0];
            m_CsvSpecialGoldPetExchange = CSVSpecialGoldPetExchange.Instance.GetConfData(m_SpecialPetId);
            if (m_CsvSpecialGoldPetExchange != null)
            {
                m_ItemId = m_CsvSpecialGoldPetExchange.Pet_id;
            }

            m_AnimatorTime = 1.6f;

            //m_SpecialPetIcon = transform.Find("Animator/View_01/MouseKing/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            m_DrawItemButton = transform.Find("View_01/MouseKing/PropItem/Btn_Item").GetComponent<Button>();
            m_DrawCount = transform.Find("View_01/MouseKing/PropItem/Text_Number").GetComponent<Text>();
            m_ExchangeButton = transform.Find("View_01/Btn_01").GetComponent<Button>();
            
            m_KingPetPreView = transform.Find("Btn_Peiyang").GetComponent<Button>();
            m_Animator = transform.GetComponent<Animator>();
            m_PropItem = new PropItem();
            m_PropItem.BindGameObject(transform.Find("View_01/PropItem").gameObject);

            m_DrawGo = transform.Find("View_01/MouseKing/PropItem").gameObject;
            m_DrawPropItem = new PropItem();
            m_DrawPropItem.BindGameObject(m_DrawGo);

            m_ExchangeButton.onClick.AddListener(OnExchangeButtonClicked);
            m_KingPetPreView.onClick.AddListener(OnKingPetPreViewClicked);
            m_DrawItemButton.onClick.AddListener(OnDrawItemButtonClicked);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, UpdateInfoUI, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSpecialPetExDraw, OnSpecialPetExDraw, toRegister);
        }

        public override void Show()
        {
            base.Show();
            UpdateInfoUI(0, 0);
        }

        public override void Hide()
        {
            base.Hide();
            m_AnimatorTimer?.Cancel();
        }

        private void UpdateInfoUI(int tab, int boxId)
        {
            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
            (_id: m_NeedItemId,
                _count: m_NeedItemCount,
                _bUseQuailty: true,
                _bBind: false,
                _bNew: false,
                _bUnLock: false,
                _bSelected: false,
                _bShowCount: true,
                _bShowBagCount: true,
                _bUseClick: true,
                _onClick: null,
                _bshowBtnNo: false);
            m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, showItem));

            uint itemCount = (uint)Sys_Bag.Instance.GetItemCount(m_NeedItemId);
            bool enough = itemCount >= m_NeedItemCount;

            ButtonHelper.Enable(m_ExchangeButton, true);

            if (!enough)
            {
                ImageHelper.SetImageGray(m_ExchangeButton.image, true, true);
            }
            PropIconLoader.ShowItemData showItem_1 = new PropIconLoader.ShowItemData
            (
                _id: m_ItemId,
                _count: 0,
                _bUseQuailty: true,
                _bBind: false,
                _bNew: false,
                _bUnLock: false,
                _bSelected: false,
                _bShowCount: false,
                _bShowBagCount: false,
                _bUseClick: true,
                _onClick: null,
                _bshowBtnNo: false);
            m_DrawPropItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, showItem_1));

            UpdateDrawCount();
            //ImageHelper.SetIcon(m_SpecialPetIcon, CSVItem.Instance.GetConfData(m_ItemId).icon_id);
        }

        private void OnSpecialPetExDraw()
        {
            UpdateDrawCount();
        }

        private void UpdateDrawCount()
        {
            if (!m_DrawCount.gameObject.activeSelf)
            {
                m_DrawCount.gameObject.SetActive(true);
            }

            TextHelper.SetText(m_DrawCount, string.Format($"{Sys_Pet.Instance.specialPetExCount}/{m_GuaranteedCount}"));
        }

        private void OnExchangeButtonClicked()
        {
            uint itemCount = (uint)Sys_Bag.Instance.GetItemCount(m_NeedItemId);
            bool enough = itemCount >= m_NeedItemCount;
            if (!enough)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2027151));
            }
            else
            {
                ButtonHelper.Enable(m_ExchangeButton, false);
                PromptBoxParameter.Instance.OpenPromptBox(LanguageHelper.GetTextContent(2027150), 0,
                    () =>
                    {
                        m_Animator.Play("Exchange", -1, 0);
                        m_AnimatorTimer?.Cancel();
                        m_AnimatorTimer = Timer.Register(m_AnimatorTime, ExReq);
                    }, () => { ButtonHelper.Enable(m_ExchangeButton, true); });
            }
        }

        private void OnKingPetPreViewClicked()
        {
            UIManager.OpenUI(EUIID.UI_KingPetReview);
        }

        private void OnDrawItemButtonClicked()
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(m_ItemId, 0, true, false, false, false, false, true);
            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_KingPet, itemData));
        }

        private void ExReq()
        {
            Sys_Pet.Instance.ExchangeSpecialGoldPetReq();
        }
    }

    public class UI_KingPetCompose : UIComponent
    {
        public class KingPetIcon
        {
            private Button btn;
            private GameObject selectGo;
            private Image petIcomImage;
            private Action<int> action;
            private int index;
            public void Init(Transform transform)
            {
                selectGo = transform.Find("Image_Select01").gameObject;
                petIcomImage = transform.Find("Pet01/Image_Icon").GetComponent<Image>();
                btn = transform.Find("ItemBg").GetComponent<Button>();
                btn.onClick.AddListener(PetIconBeClicked);
            }

            public void SetAction(Action<int> action)
            {
                this.action = action;
            }

            public void SetView(int index, int selectIndex)
            {
                this.index = index;
                var iconData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(index);
                if (null != iconData)
                {
                    CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(iconData.pet_id);
                    if(null != petData)
                    {
                        ImageHelper.SetIcon(petIcomImage, petData.icon_id);
                    }
                }
                SetSelectState(selectIndex);
            }

            public void SetSelectState(int selectIndex)
            {
                selectGo.SetActive(index == selectIndex);
            }

            private void PetIconBeClicked()
            {
                action?.Invoke(index);
            }
        }
        public class UI_KingPetComposeItem : UI_Compose_Item
        {
            private GameObject fxGo;
            protected override void Loaded()
            {
                base.Loaded();
                fxGo = transform.Find("Fx_ui_KingPet_item").gameObject;
            }

            public void HideFx()
            {
                fxGo.SetActive(false);
            }
        }
        private Image icon;
        private Text itemName;
        private Text itemNum;
        private Button itemBtn;
        //宠物名称等级
        private Text petName;
        //品质相关
        private Image cardQuality;
        private Text cardName;
        private Image cardLevel;
        private Text cardFamilyName;

        private Button composeBtn;
        private Button bookBtn;
        private Button showFashionBtn;
        private RawImage rawImage;
        private Animator targetAni;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;

        private InfinityGrid _infinityGrid;
        private List<KingPetIcon> kingPetIcons = new List<KingPetIcon>();
        /// <summary>
        /// 右侧合成单品需求展示
        /// </summary>
        private GameObject composeSubGo;
        private Transform undoParent;
        private List<List<uint>> undos = new List<List<uint>>();
        private List<UI_KingPetComposeItem> composeItems = new List<UI_KingPetComposeItem>(4);
        private Timer aniTimer;
        private int selectIndex = 0;
        private bool isShowFashionPreview = false;
        private int infinityCount = 0;
        public int Count
        {
            get
            {
                return 1;
            }
        }
        private float animatorLength;
        public float AnimatorLength
        {
            get
            {
                if(null != targetAni && animatorLength == 0)
                {
                    for (int i = 0; i < targetAni.runtimeAnimatorController.animationClips.Length; i++)
                    {
                        var animationClip = targetAni.runtimeAnimatorController.animationClips[i];
                        if (string.Equals(animationClip.name, "UI_KingPet_Animator_Circle2_Exchange2_Open"))
                        {
                            animatorLength = animationClip.averageDuration;
                        }
                    }
                }
                return animatorLength;
            }
        }

        protected override void Loaded()
        {
            icon = transform.Find("View_01/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            itemNum = transform.Find("View_01/PropItem/Text_Number").GetComponent<Text>();
            itemName = transform.Find("View_01/Text").GetComponent<Text>();
            itemBtn = transform.Find("View_01/PropItem/Btn_Item").GetComponent<Button>();
            itemBtn.onClick.AddListener(() =>
            {
                if (selectIndex >= 0 && selectIndex < infinityCount)
                {
                    var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                    if (composeData.pet_id != 0)
                    {
                        PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(composeData.pet_id, 0, false, false, false, false, false, false, false);
                        itemData.bShowBtnNo = false;
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_KingPet, itemData));
                    }
                }
            });
            petName = transform.Find("View_01/Name/Text_Name").GetComponent<Text>();
            cardQuality = transform.Find("View_01/Name/Image_Card").GetComponent<Image>();
            cardName = transform.Find("View_01/Name/Image_Card/Text_CardName").GetComponent<Text>();
            cardLevel = transform.Find("View_01/Name/Image_Card/Text_CardLevel").GetComponent<Image>();
            cardFamilyName = transform.Find("View_01/Name/Text_Type").GetComponent<Text>();
            rawImage = transform.Find("Image_Circle02").GetComponent<RawImage>();
            undoParent = transform.Find("Cost01/Viewport");
            composeSubGo = transform.Find("Cost01/Viewport/View_Item01").gameObject;
            composeSubGo.gameObject.SetActive(false);
            for (int i = 0; i < undoParent.childCount; i++)
            {
                UI_KingPetComposeItem item = AddComponent<UI_KingPetComposeItem>(transform.Find($"Cost01/Viewport/View_Item0{i + 1}").transform);
                item.SetItemResUIID(EUIID.UI_KingPet);
                composeItems.Add(item);
            }
            bookBtn = transform.Find("Btn_Peiyang").GetComponent<Button>();
            bookBtn.onClick.AddListener(OnBookBtnClicked);
            showFashionBtn = transform.Find("Btn_Yuanhe").GetComponent<Button>();
            showFashionBtn.onClick.AddListener(OnShowFashionBtnClicked);
            composeBtn = transform.Find("View_01/Btn_01").GetComponent<Button>();
            composeBtn.onClick.AddListener(Undo);
            targetAni = transform.GetComponent<Animator>();

            _infinityGrid = transform.Find("View_01/Image_Scroll/Scroll_View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            infinityCount = CSVSpecialGoldPetAppointExchange.Instance.Count;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            KingPetIcon entry = new KingPetIcon();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            entry.SetAction(OnSkillSelect);
            kingPetIcons.Add(entry);
            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= infinityCount)
                return;
            KingPetIcon entry = cell.mUserData as KingPetIcon;
            entry.SetView(index,selectIndex);
        }

        private void OnSkillSelect(int index)
        {
            if (selectIndex == index)
                return;
            selectIndex = index;
            SetPetIconSelectState();
            RefreshView();
        }

        private void SetPetIconSelectState()
        {
            for (int i = 0; i < kingPetIcons.Count; i++)
            {
                kingPetIcons[i].SetSelectState(selectIndex);
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, UpdateInfoUI, toRegister);
        }

        public override void Hide()
        {
            aniTimer?.Cancel();
            targetAni.Play("Close", -1, 0);
            UnloadModel();
            base.Hide();
        }

        public override void Show()
        {
            _infinityGrid.CellCount = infinityCount;
            _infinityGrid.ForceRefreshActiveCell();
            aniTimer?.Cancel();
            targetAni.Play("Close", -1, 0);
            RefreshView();
            base.Show();
        }

        private void RefreshView()
        {
            isShowFashionPreview = false;
            showFashionBtn.transform.Find("Image_Select").gameObject.SetActive(isShowFashionPreview);
            UnloadModel();
            _LoadShowScene();
            _LoadShowModel();
            ResetComposeItems();
        }

        private void UpdateInfoUI(int tab, int boxId)
        {
            ResetComposeItems();
        }

        private void ResetComposeItems()
        {
            icon.enabled = true;
            itemNum.gameObject.SetActive(true);
            itemName.gameObject.SetActive(true);
            if (selectIndex >= 0 && selectIndex < infinityCount)
            {
                var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                CSVItem.Data composeItemData = CSVItem.Instance.GetConfData(composeData.pet_id);
                if(null != composeItemData)
                {
                    ImageHelper.SetIcon(icon, composeItemData.icon_id);
                    TextHelper.SetText(itemName, composeItemData.name_id);
                }
                itemNum.text = Count.ToString();
                CSVPetNew.Data currentPetData = CSVPetNew.Instance.GetConfData(composeData.pet_id);
                if(null != currentPetData)
                {
                    ImageHelper.GetPetCardLevel(cardLevel, currentPetData.card_lv);
                    cardName.text = Sys_Pet.Instance.CardName(currentPetData.card_type);
                    ImageHelper.SetIcon(cardQuality, Sys_Pet.Instance.SetPetQuality(currentPetData.card_type));
                    petName.text = LanguageHelper.GetTextContent(currentPetData.name);
                    showFashionBtn.gameObject.SetActive(currentPetData.show_appearance);
                    CSVGenus.Data cSVGenusData = CSVGenus.Instance.GetConfData(currentPetData.race);
                    if (null != cSVGenusData)
                    {
                        //ImageHelper.SetIcon(cardFamily, cSVGenusData.rale_icon);
                        cardFamilyName.text = LanguageHelper.GetTextContent(cSVGenusData.rale_name);
                    }
                }

            }
            undos = GetComposeSubItem();
            UpdateButtonState();
            for (int i = 0; i < undos.Count; i++)
            {
                if (composeItems.Count <= i)
                {
                    GameObject go = GameObject.Instantiate(composeSubGo, undoParent);
                    UI_KingPetComposeItem item = AddComponent<UI_KingPetComposeItem>(go.transform);
                    item.SetItemResUIID(EUIID.UI_KingPet);
                    item.SetData(undos[i][0], undos[i][1] * Count);
                    item.SetLineState(!(i == undos.Count - 1));
                    composeItems.Add(item);
                    go.SetActive(true);
                }
                else
                {
                    composeItems[i].SetData(undos[i][0], undos[i][1] * Count);
                    composeItems[i].SetLineState(!(i == undos.Count - 1));
                    composeItems[i].gameObject.SetActive(true);
                }
            }

            for (int i = undos.Count; i < composeItems.Count; i++)
            {
                composeItems[i].gameObject.SetActive(false);
            }
        }

        private uint MaxInputCount;

        private List<List<uint>> GetComposeSubItem()
        {
            List<List<uint>> temp = new List<List<uint>>();
            if (selectIndex >= 0 && selectIndex < infinityCount)
            {
                var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                if (null != composeData)
                {
                    MaxInputCount = 1;
                    if (null != composeData.RecyclingItem1)
                    {
                        MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem1[0])) / (composeData.RecyclingItem1[1])), MaxInputCount);
                        temp.Add(composeData.RecyclingItem1);
                        if (null != composeData.RecyclingItem2)
                        {
                            MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem2[0])) / (composeData.RecyclingItem2[1])), MaxInputCount);
                            temp.Add(composeData.RecyclingItem2);
                            if (null != composeData.RecyclingItem3)
                            {
                                MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem3[0])) / (composeData.RecyclingItem3[1])), MaxInputCount);
                                temp.Add(composeData.RecyclingItem3);
                                if (null != composeData.RecyclingItem4)
                                {
                                    MaxInputCount = (uint)Math.Min((Math.Floor((double)Sys_Bag.Instance.GetItemCount(composeData.RecyclingItem4[0])) / (composeData.RecyclingItem4[1])), MaxInputCount);
                                    temp.Add(composeData.RecyclingItem4);
                                }
                            }
                        }
                    }
                }
            }
            return temp;
        }

        private void UpdateButtonState()
        {
            composeBtn.enabled = true;
            ImageHelper.SetImageGray(composeBtn, !(MaxInputCount > 0), true);
        }

        public GameObject modelGo;

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                if (selectIndex >= 0 && selectIndex < infinityCount)
                {
                    var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                    CSVPetNew.Data CurrentPetData = CSVPetNew.Instance.GetConfData(composeData.pet_id);
                    modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                    modelGo.SetActive(false);
                    ResetFashion();
                    petDisplay.mAnimation.UpdateHoldingAnimations(CurrentPetData.action_id_show, CurrentPetData.weapon, Constants.PetShowAnimationClipHashSet, go: modelGo);
                }
            }
        }

        public void UnloadModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            if (null != petDisplay && null != petDisplay.mAnimation)
            {
                petDisplay.mAnimation.StopAll();
            }
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl?.Dispose();
            modelGo = null;
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(1000, 0, 0);

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
            if (selectIndex >= 0 && selectIndex < infinityCount)
            {
                var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                CSVPetNew.Data CurrentPetData = CSVPetNew.Instance.GetConfData(composeData.pet_id);
                string _modelPath = CurrentPetData.model_show;
                petDisplay.eLayerMask = ELayerMask.ModelShow;
                petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
                petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                showSceneControl.mModelPos.transform.Rotate(new Vector3(CurrentPetData.angle1, CurrentPetData.angle2, CurrentPetData.angle3));
                showSceneControl.mModelPos.transform.localScale = new Vector3(CurrentPetData.size, CurrentPetData.size, CurrentPetData.size);
                showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x + CurrentPetData.translation, CurrentPetData.height, showSceneControl.mModelPos.transform.localPosition.z);
            }
        }

        public void ResetFashion()
        {
            uint fashionId = 0;
            if (isShowFashionPreview)
            {
                var configs = CSVPetEquipSuitAppearance.Instance.GetAll();
                for (int i = 0, len = configs.Count; i < len; i++)
                {
                    var config = configs[i];
                    if (selectIndex >= 0 && selectIndex < infinityCount)
                    {
                        var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                        if (config.pet_id.Contains(composeData.pet_id))
                        {
                            fashionId = config.show_id;
                            break;
                        }
                    }
                }
            }
            SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, fashionId, modelGo.transform);
        }

        private void OnBookBtnClicked()
        {
            if (selectIndex >= 0 && selectIndex < infinityCount)
            {
                var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                PetBookListPar petBookListPar = new PetBookListPar();
                petBookListPar.petId = composeData.pet_id;
                petBookListPar.showChangeBtn = false;
                petBookListPar.ePetReviewPageType = EPetBookPageType.Seal;
                UIManager.OpenUI(EUIID.UI_Pet_BookReview, false, petBookListPar);
            }
        }

        private void OnShowFashionBtnClicked()
        {
            isShowFashionPreview = !isShowFashionPreview;
            showFashionBtn.transform.Find("Image_Select").gameObject.SetActive(isShowFashionPreview);
            ResetFashion();
        }

        private void Undo()
        {
            if (selectIndex >= 0 && selectIndex < infinityCount)
            {
                var composeData = CSVSpecialGoldPetAppointExchange.Instance.GetByIndex(selectIndex);
                if (composeData.pet_id != 0)
                {
                    if (Sys_Pet.Instance.IsUniquePet(composeData.pet_id) && Sys_Pet.Instance.HasUniquePet(composeData.pet_id))
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12470));
                        return;
                    }
                }

                if(MaxInputCount > 0)
                {
                    CSVPetNew.Data petData = CSVPetNew.Instance.GetConfData(composeData.pet_id);
                    string petNameTips = string.Empty;
                    if (null != petData)
                    {
                        petNameTips = LanguageHelper.GetTextContent(2045051, LanguageHelper.GetTextContent(petData.name));
                    }
                    ButtonHelper.Enable(composeBtn, false);
                    PromptBoxParameter.Instance.OpenPromptBox(petNameTips, 0, () => {
                        ButtonHelper.Enable(composeBtn, false);
                        targetAni.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                        targetAni.Play("Exchange2", -1, 0);
                        aniTimer = Timer.Register(AnimatorLength, () =>
                        {
                            Sys_Pet.Instance.OnPetExchangTargetGoldPetReq(composeData.id);
                        }, null, false, false);
                    },()=> {
                        UpdateButtonState();
                    });
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2045052));
                }
            }
        }
    }

    public class UI_KingPet_Tabs : UIComponent
    {
        public class TabType
        {
            private Transform transform;
            private CP_Toggle _toggle;
            private Text tabName1;
            private Text tabName2;
            private int _tabIndex;
            private System.Action<int> _action;
            public void Init(Transform trans)
            {
                transform = trans;
                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClickToggle);
                tabName1 = transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
                tabName2 = transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(_tabIndex);
                }
            }

            public void SetType(int index)
            {
                _tabIndex = index;
            }

            public void Register(System.Action<int> action)
            {
                _action = action;
            }

            public void OnSelect(bool isOn)
            {
                _toggle.SetSelected(isOn, true);
            }
        }

        private List<TabType> listTabs = new List<TabType>();
        private IListener listener;

        protected override void Loaded()
        {
            int length = Enum.GetValues(typeof(EKingPetView)).Length;

            for (int i = 0; i < length; ++i)
            {
                TabType tab = new TabType();
                tab.Init(transform.Find($"TabItem0{i + 1}"));
                tab.SetType(i);
                tab.Register(OnTabSelect);
                listTabs.Add(tab);
            }
        }

        public override void Hide()
        {
            for (int i = 0; i < listTabs.Count; ++i)
                listTabs[i].OnSelect(false);
        }

        private void OnTabSelect(int index)
        {
            listener?.OnClickTabType((EKingPetView)index);
        }

        public void OnTabIndex(int tabIndex)
        {
            listTabs[tabIndex].OnSelect(true);
        }

        public void Register(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnClickTabType(EKingPetView _type);
        }
    }

    public class UI_KingPet : UIBase, UI_KingPet_Tabs.IListener
    {
        private Button closeBtn;
        private UI_KingPet_Tabs tabs;
        private Dictionary<EKingPetView, UIComponent> viewDic = new Dictionary<EKingPetView, UIComponent>();

        private EKingPetView viewType = EKingPetView.Draw;
        
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseButtonClicked);
            UI_KingPetDraw kingPetDraw = AddComponent<UI_KingPetDraw>(transform.Find("Animator/Circle1"));
            viewDic.Add(EKingPetView.Draw, kingPetDraw);
            UI_KingPetCompose kingPetCompose = AddComponent<UI_KingPetCompose>(transform.Find("Animator/Circle2"));
            kingPetCompose.assetDependencies = transform.GetComponent<AssetDependencies>();
            viewDic.Add(EKingPetView.Compose, kingPetCompose);
            tabs = new UI_KingPet_Tabs();
            tabs.Init(transform.Find("Animator/Tab"));
            tabs.Register(this);
        }

        protected override void OnOpen(object arg)
        {
            if(null != arg)
            {
                viewType = (EKingPetView)Convert.ToUInt32(arg);
            }
        }

        protected override void OnShow()
        {
            //viewDic[EKingPetView.Draw].Show();
            tabs.OnTabIndex((int)viewType);
        }

        protected override void OnHide()
        {
            foreach (var dict in viewDic)
            {
                if (null != dict.Value)
                    dict.Value.Hide();
            }
            tabs.Hide();
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_KingPet);
        }

        public void OnClickTabType(EKingPetView _type)
        {
            viewType = _type;

            foreach (var dict in viewDic)
            {
                if (dict.Key == _type)
                {
                    if(null != dict.Value)
                    {
                        dict.Value.Show();
                    }
                }
                else
                {
                    if (null != dict.Value)
                        dict.Value.Hide();
                }
            }
        }
    }
}