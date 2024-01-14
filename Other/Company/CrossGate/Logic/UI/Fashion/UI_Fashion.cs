using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using System;
using Packet;

namespace Logic
{
    public partial class UI_Fashion : UIBase
    {
        private Text fashionName;       //当前时装名字
        private GameObject fashionRoot;
        private GameObject labelPage;
        private GameObject wearRoot;
        private GameObject suitRoot;
        private Transform suitGridRoot; //套装 格子(显示多少子部件)节点
        private GameObject countdown;

        #region *************  Attr  *************
        //private Transform fashionAttrRoot;
        //private GameObject suitAttrRoot;
        //private GameObject suitAttrParent;
        //private Transform suitAttrParent2;
        //private GameObject suitAttrChild_fashion;
        //private GameObject suitAttrChild_suit;

        //private GameObject suitChange;
        //private Transform suitChangeParent;
        //private Button suitChangeButton;
        //private Image suitAttrImage;
        #endregion
        private Text text_Titlesuit;
        private Text text_Titlefashion;
        private Text describe;
        private Text fashionPage;
        private Text limitType;         //当前时装期限
        private Button closeBtn;
        private Button backBtn;
        private Button revertToFirst;
        private Transform propRoot;
        private Button unlock_1;
        private Button unlock_2;
        private Button dress;
        private Button rule;
        private List<Button> wearPropButtons = new List<Button>();
        private List<Button> suitGridButtons = new List<Button>();
        private GameObject scrollView_suit;
        private GameObject scrollView_clothes;
        private GameObject scrollView_weapon;
        private GameObject scrollView_acce;
        private GameObject acceRoot;
        private GameObject empty1;
        private GameObject empty2;

        private Button m_FashionPointButton;
        private Text m_FashionPoint;//时装值按钮
        private GameObject m_FashionPointRed;//时装值红点
        private Text m_FashionPoint_Des;//单件时装 描述
        private GameObject m_CustomTag;
        private GameObject m_FreeTag;
        private GameObject m_ActionTag;
        private Button m_LuckyDrawButton;
        private GameObject m_LuckyDrawRedPoint;

        private Dictionary<GameObject, FashionCeilGrid_Suit> m_FashionCeilGrid_Suits = new Dictionary<GameObject, FashionCeilGrid_Suit>();
        private Dictionary<GameObject, FashionCeilGrid_Clothes> m_FashionCeilGrid_Clothes = new Dictionary<GameObject, FashionCeilGrid_Clothes>();
        private Dictionary<GameObject, FashionCeilGrid_Weapon> m_FashionCeilGrid_Weapon = new Dictionary<GameObject, FashionCeilGrid_Weapon>();
        private InfinityGrid m_InfinityGrid_Suit;
        private InfinityGrid m_InfinityGrid_Clothes;
        private InfinityGrid m_InfinityGrid_Weapon;

        private InfinityGrid m_InfinityGrid_Acce;
        private Dictionary<GameObject, FashionCeilGrid_Accessory> fashionCeilGrid_Accessorys = new Dictionary<GameObject, FashionCeilGrid_Accessory>();
        private List<FashionAccessory> fashionAccessories = new List<FashionAccessory>();       //需要刷新的挂饰数据
        private CP_ToggleRegistry CP_ToggleRegistry_Parent;
        private CP_ToggleRegistry CP_ToggleRegistry_Child;
        private Transform m_InfinityParent_Acce;
        private int curAcceSelectIndex;

        private int curParentLable = -1;
        private int CurParentLable
        {
            get { return curParentLable; }
            set
            {
                if (curParentLable != value)
                {
                    curParentLable = value;
                    OnParentLableChanged();
                }
            }
        }
        private bool bacceVaild = false;                        //控制挂饰子页签的显示隐藏
        private bool bAcceVaild
        {
            get { return bacceVaild; }
            set
            {
                if (bacceVaild != value)
                {
                    bacceVaild = value;
                    SetAcceRoot(bacceVaild);
                    curAcceSelectIndex = 0;
                }
            }
        }
        private int curChildLable = 2;
        private int CurChildLable
        {
            get { return curChildLable; }
            set
            {
                if (curChildLable != value)
                {
                    curChildLable = value;
                    OnChildTableChanged();
                }
            }
        }

        private Color unlockColor;
        private Color unlocksuitColor;
        private int viewModel;//0 时装 1 染色
        private uint bagUsefashionId;
        private uint bagUseItemId;

        private FashionClothes curFashionClothes;
        private FashionWeapon curFashionWeapon;
        private FashionSuit curFashionSuit;
        private FashionAccessory curFashionAccessory;
        private int curFashionType = 0;    //记录当前界面时装部件类型  0:套装  1:时装  2:武器  3:挂饰(选中页签类型)
        private int curFashionClotheSelectIndex = 0;
        private int curFashionWeaponSelectIndex = 0;
        private int curFashionSuitSelectIndex = 0;

        private int curFashionJewelry_HeadIndex = 0;//头饰
        private int curFashionJewelry_BackIndex = 0;//背饰
        private int curFashionJewelry_WaistSelectIndex = 0;//腰饰
        private int curFashionJewelry_FaceSelectIndex = 0;//脸饰

        private List<EHeroModelParts> formSuit2OtherChanges = new List<EHeroModelParts>();
        private EHeroModelParts curSelectFashionAcceChildLable;
        private uint curMainPartShowId = 0;
        private uint curWeaponPartShowId = 0;
        private List<uint> curAccePartShowId = new List<uint>();

        private float _clickLimitTime;
        private bool b_MainChanged = true;
        private bool b_WeaponChanged = true;

        #region UILifeCycle
        protected override void OnInit()
        {
            curParentLable = 1;
            bdressMode = false;
            ResetDressData();
            viewModel = 0;
            curdyeMoudle = 0;
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                Tuple<uint, uint> tuple = arg as Tuple<uint, uint>;
                bagUsefashionId = tuple.Item1;
                bagUseItemId = tuple.Item2;
            }
        }

        protected override void OnShow()
        {
            b_MainChanged = true;
            _LoadShowScene();
            _LoadShowModel();
            if (bagUsefashionId != 0)
            {
                OnUseBagItem(bagUsefashionId);
            }
            if (viewModel == 0)
            {
                CP_ToggleRegistry_Parent.SwitchTo(curParentLable);
                RefreshFashionScrollView(curParentLable);
                RefreshWeaponIcon();
                RefreshClothIcon();
                RefreshSuitIcon();
                #region  *******************************    Attr    **********************************
                //UpdateSuitChange(false);
                #endregion
            }
            UpdateFashionPoint();
            timer?.Cancel();
            timer = Timer.Register(1, null, UpdateCameraPos, true);

            OnRefreshFreeDrawState();
            OnRefreshLuckyDrawActiveState();


        }

        private void CheckHudClose()
        {
            if (UIManager.IsOpen(EUIID.HUD))
            {
                UIManager.CloseUI(EUIID.HUD);
            }
        }

        protected override void OnClose()
        {
            showParts.Clear();
        }

        protected override void OnHide()
        {
            _curUpdateAnimationFlagId = 0u;
            _curWeaponFashionModelId = 0u;
            _UnloadShowContent();
            bagUsefashionId = 0;
            bagUseItemId = 0;
            timer?.Cancel();
        }

        protected override void OnLoaded()
        {
            ParseFashionComponent();
            ParseDyeComponent();
            RegisterFashionEvent();
            RegisterDyeEvent();
            ResgisterShowEvent();
            ParseConfig();
            ClearCondition();
            AddCondition();

            SwitchToFashion();
        }

        protected override void OnUpdate()
        {
            UpdateTime();
            //UpdateCameraPos();

            CheckHudClose();
        }

        #endregion

        private void ParseConfig()
        {
            string[] strs = CSVParam.Instance.GetConfData(671).str_value.Split('|');
            unlockColor = new Color(float.Parse(strs[0]) / 255f, float.Parse(strs[1]) / 255f, float.Parse(strs[2]) / 255f, float.Parse(strs[3]) / 255f);
            string[] _strs = CSVParam.Instance.GetConfData(672).str_value.Split('|');
            unlocksuitColor = new Color(float.Parse(_strs[0]) / 255f, float.Parse(_strs[1]) / 255f, float.Parse(_strs[2]) / 255f, float.Parse(_strs[3]) / 255f);
        }

        private void ParseFashionComponent()
        {
            eventImage = transform.Find("Animator/EventImage").GetComponent<Image>();
            closeBtn = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            fashionRoot = transform.Find("Animator/View_Fashion").gameObject;
            revertToFirst = transform.Find("Animator/View_Left/Fashion/Btn_Revert").GetComponent<Button>();
            unlock_1 = fashionRoot.transform.Find("Grid_Btn/Btn_Unlock01").GetComponent<Button>();
            unlock_2 = fashionRoot.transform.Find("Grid_Btn/Btn_Unlock02").GetComponent<Button>();
            rule = transform.Find("Animator/View_Left/Btn_Detail").GetComponent<Button>();
            dress = fashionRoot.transform.Find("Grid_Btn/Btn_Use").GetComponent<Button>();
            labelPage = transform.Find("Animator/View_Fashion/Image_Page").gameObject;
            fashionName = transform.Find("Animator/View_Fashion/Text_Title").GetComponent<Text>();
            text_Titlesuit = transform.Find("Animator/View_Fashion/Text_Title/Text_suit").GetComponent<Text>();
            text_Titlefashion = transform.Find("Animator/View_Fashion/Text_Title/Text_fashion").GetComponent<Text>();
            describe = transform.Find("Animator/View_Fashion/View_Wear/Text_Dis").GetComponent<Text>();
            wearRoot = transform.Find("Animator/View_Fashion/View_Wear").gameObject;
            propRoot = wearRoot.transform.Find("View_Cost/PropItem");
            countdown = wearRoot.transform.Find("Image_Time").gameObject;
            limitType = countdown.transform.Find("Text_Time").GetComponent<Text>();
            suitRoot = transform.Find("Animator/View_Fashion/View_Suit").gameObject;
            m_FashionPointButton = transform.Find("Animator/View_Left/Fashion/Btn_Point").GetComponent<Button>();
            m_FashionPoint = transform.Find("Animator/View_Left/Fashion/Btn_Point/Text_Value").GetComponent<Text>();
            m_FashionPointRed = transform.Find("Animator/View_Left/Fashion/Btn_Point/Image_Dot").gameObject;
            m_FashionPoint_Des = transform.Find("Animator/View_Fashion/View_Wear/Text_Point/Text_Value").GetComponent<Text>();
            m_FreeTag = transform.Find("Animator/View_Fashion/View_Wear/Feature/Item_1").gameObject;
            m_ActionTag = transform.Find("Animator/View_Fashion/View_Wear/Feature/Item_2").gameObject;
            m_CustomTag = transform.Find("Animator/View_Fashion/View_Wear/Feature/Item_3").gameObject;
            m_LuckyDrawButton = transform.Find("Animator/View_Left/Fashion/Btn_LuckDraw").GetComponent<Button>();
            m_LuckyDrawRedPoint = transform.Find("Animator/View_Left/Fashion/Btn_LuckDraw/Image_Dot").gameObject;

            #region ******************* Attr **********************
            //suitChange = transform.Find("Animator/View_Left/Fashion/View_Change").gameObject;
            //suitChangeParent = suitChange.transform.Find("Grid_Attr");
            //suitChangeButton = suitChange.transform.Find("Btn_Change").GetComponent<Button>();
            //fashionAttrRoot = wearRoot.transform.Find("Grid_Attr");
            //suitAttrRoot = suitRoot.transform.Find("UI_Fashion_SuitProperty").gameObject;
            //suitAttrParent = suitAttrRoot.transform.Find("View_Tips/Message/Message_Root/BasicPropRoot").gameObject;
            //suitAttrChild_fashion = suitAttrParent.transform.Find("Basis_PropertyActive").gameObject;
            //suitAttrChild_suit = suitAttrParent.transform.Find("Basis_Suit").gameObject;
            //suitAttrParent2 = suitRoot.transform.Find("Image_SuitProperty/Grid_Attr");
            //suitAttrImage = suitRoot.transform.Find("Image_SuitProperty/Btn_Detail").GetComponent<Image>();
            #endregion

            suitGridRoot = transform.Find("Animator/View_Fashion/View_Suit/Grid").transform;
            scrollView_suit = transform.Find("Animator/View_Fashion/Scroll View01").gameObject;
            scrollView_clothes = transform.Find("Animator/View_Fashion/Scroll View02").gameObject;
            scrollView_weapon = transform.Find("Animator/View_Fashion/Scroll View03").gameObject;
            scrollView_acce = transform.Find("Animator/View_Fashion/Scroll View04").gameObject;
            empty1 = wearRoot.transform.Find("Empty1").gameObject;
            empty2 = wearRoot.transform.Find("Empty2").gameObject;
            CP_ToggleRegistry_Parent = transform.Find("Animator/View_Fashion/List_Menu/TabList01").GetComponent<CP_ToggleRegistry>();
            acceRoot = transform.Find("Animator/View_Fashion/List_Menu/TabList02").gameObject;
            CP_ToggleRegistry_Child = acceRoot.GetComponent<CP_ToggleRegistry>();
            m_InfinityParent_Acce = transform.Find("Animator/View_Fashion/Scroll View04");
            ConstructInfinityGroup();
        }

        private void RegisterFashionEvent()
        {
            CP_ToggleRegistry_Parent.onToggleChange = onParentToggleChanged;
            CP_ToggleRegistry_Child.onToggleChange = OnChildToggleChanged;
            closeBtn.onClick.AddListener(() => UIManager.CloseUI(EUIID.UI_Fashion));
            unlock_1.onClick.AddListener(OnUnLock_1ButtonClicked);
            unlock_2.onClick.AddListener(OnUnLock_1ButtonClicked);
            dress.onClick.AddListener(OnDressButtonClicked);
            revertToFirst.onClick.AddListener(OnRevertToFirstClick);
            m_FashionPointButton.onClick.AddListener(OnFashionPointButtonClicked);
            m_LuckyDrawButton.onClick.AddListener(OnLuckyDrawButtonClicked);

            rule.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_LittleGame_Tips, false, (uint)2009547);
            });
            #region *********  Attr  ***********
            //Lib.Core.EventTrigger eventListener0 = Lib.Core.EventTrigger.Get(suitAttrImage);
            //eventListener0.AddEventListener(EventTriggerType.PointerDown, _ => SetSuitAttrDiscribe(true));
            //eventListener0.AddEventListener(EventTriggerType.PointerUp, _ => SetSuitAttrDiscribe(false));

            //suitChangeButton.onClick.AddListener(() =>
            //{
            //    UIManager.OpenUI(EUIID.UI_Fashion_SuitChange);
            //});
            #endregion
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Fashion.Instance.eventEmitter.Handle<uint, EHeroModelParts>(Sys_Fashion.EEvents.OnLoadModelParts, LoadModelParts, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint, EHeroModelParts>(Sys_Fashion.EEvents.OnUnLoadModelParts, UnLoadModelParts, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint, EHeroModelParts>(Sys_Fashion.EEvents.OnDyeModelParts, OnDyeModelParts, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateClothesLockState, OnUpdateClothesLockState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateWeaponLockState, OnUpdateWeaponLockState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateAcceLockState, OnUpdateAcceLockState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateSuitLockState, OnUpdateSuitLockState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateClothesDressState, OnUpdateClothesDressState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateWeaponDressState, OnUpdateWeaponDressState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateAcceDressState, OnUpdateAcceDressState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateSuitDressState, OnUpdateSuitDressState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateSuitAsso, OnUpdateSuitAsso, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateClothesLimitTime, OnUpdateClothesLimitTime, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateWeaponLimitTime, OnUpdateWeaponLimitTime, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnUpdateAcceLimitTime, OnUpdateAcceLimitTime, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<bool>(Sys_Fashion.EEvents.OnUpdateDyeButtonState, UpdateDyeButtonState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnUpdateSuit, OnSetSuitTitle, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnUpdateSuit, OnSetSuitGrid, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnUpdatePropRoot, UpdatePropRoot, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnUpdateUnLockButton, UpdateUnLockButton, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnUpdateDyePropRoot, SetDyePropRoot, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnCheckDyeColorDirty, OnCheckDirty, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnSetColorBtnActive, SetColorBtnActive, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.UpdateCompareLastShowOrHide, UpdateCompareLastShowOrHide, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.RevertToFirstColor, RevertToFirstColor, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.SetToLastColor, SetToLastUseColor, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnUpdateFashionPoint, UpdateFashionPoint, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnUpdateFashionPoint, OnRefreshFashionPointRed, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<int>(Sys_Fashion.EEvents.OnSaveDyeSuccess, OnSaveDyeSuccess, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnRefreshFreeDrawState, OnRefreshFreeDrawState, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnRefreshLuckyDrawActiveState, OnRefreshLuckyDrawActiveState, toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnRefreshLuckyDrawActiveState, toRegister);
            #region  *******************************    Attr    **********************************
            //Sys_Fashion.Instance.eventEmitter.Handle<bool>(Sys_Fashion.EEvents.OnUpdateSuitChange, UpdateSuitChange, toRegister);
            #endregion
        }


        #region RegisterInfinityGrid
        private void ConstructInfinityGroup()
        {
            m_InfinityGrid_Suit = scrollView_suit.GetComponent<InfinityGrid>();
            m_InfinityGrid_Clothes = scrollView_clothes.GetComponent<InfinityGrid>();
            m_InfinityGrid_Weapon = scrollView_weapon.GetComponent<InfinityGrid>();

            m_InfinityGrid_Suit.onCreateCell = OnCreateSuitGrid;
            m_InfinityGrid_Clothes.onCreateCell = OnCreateClothesGrid;
            m_InfinityGrid_Weapon.onCreateCell = OnCreateWeaponGrid;

            m_InfinityGrid_Suit.onCellChange += OnCellChangeSuit;
            m_InfinityGrid_Clothes.onCellChange += OnCellChangeClothes;
            m_InfinityGrid_Weapon.onCellChange += OnCellChangeWeapon;

            m_InfinityGrid_Acce = m_InfinityParent_Acce.gameObject.GetComponent<InfinityGrid>();
            m_InfinityGrid_Acce.onCreateCell = OnCreateAcceGrid;
            m_InfinityGrid_Acce.onCellChange = OnCellChangeAcce;
        }

        private void OnCreateSuitGrid(InfinityGridCell cell)
        {
            FashionCeilGrid_Suit entry = new FashionCeilGrid_Suit();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddEventListener(OnFashionSuitGridSelect);
            cell.BindUserData(entry);
            m_FashionCeilGrid_Suits.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCreateClothesGrid(InfinityGridCell cell)
        {
            FashionCeilGrid_Clothes entry = new FashionCeilGrid_Clothes();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddEventListener(OnFashionClothesGridSelect);
            cell.BindUserData(entry);
            m_FashionCeilGrid_Clothes.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCreateWeaponGrid(InfinityGridCell cell)
        {
            FashionCeilGrid_Weapon entry = new FashionCeilGrid_Weapon();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddEventListener(OnFashionWeaponGridSelect);
            cell.BindUserData(entry);
            m_FashionCeilGrid_Weapon.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCreateAcceGrid(InfinityGridCell cell)
        {
            FashionCeilGrid_Accessory entry = new FashionCeilGrid_Accessory();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddClickListener(OnAcceGridSelected);
            cell.BindUserData(entry);
            fashionCeilGrid_Accessorys.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChangeSuit(InfinityGridCell cell, int index)
        {
            FashionCeilGrid_Suit fashionCeilGrid_Suit = cell.mUserData as FashionCeilGrid_Suit;
            fashionCeilGrid_Suit.SetData(Sys_Fashion.Instance._FashionSuits[index], index);

            if (index != curFashionSuitSelectIndex)
            {
                fashionCeilGrid_Suit.Release();
            }
            else
            {
                fashionCeilGrid_Suit.Select();
            }
        }

        private void OnCellChangeClothes(InfinityGridCell cell, int index)
        {
            FashionCeilGrid_Clothes fashionCeilGrid_Clothes = cell.mUserData as FashionCeilGrid_Clothes;
            fashionCeilGrid_Clothes.SetData(Sys_Fashion.Instance._FashionClothes[index], index);

            if (index != curFashionClotheSelectIndex)
            {
                fashionCeilGrid_Clothes.Release();
            }
            else
            {
                fashionCeilGrid_Clothes.Select();
            }
        }

        private void OnCellChangeWeapon(InfinityGridCell cell, int index)
        {
            FashionCeilGrid_Weapon fashionCeilGrid_Weapon = cell.mUserData as FashionCeilGrid_Weapon;
            fashionCeilGrid_Weapon.SetData(Sys_Fashion.Instance._FashionWeapons[index], index);

            if (index != curFashionWeaponSelectIndex)
            {
                fashionCeilGrid_Weapon.Release();
            }
            else
            {
                fashionCeilGrid_Weapon.Select();
            }
        }

        private void OnCellChangeAcce(InfinityGridCell cell, int index)
        {
            FashionCeilGrid_Accessory fashionCeilGrid_Accessory = cell.mUserData as FashionCeilGrid_Accessory;
            fashionCeilGrid_Accessory.SetData(fashionAccessories[index], index);

            if (index != curAcceSelectIndex)
            {
                fashionCeilGrid_Accessory.Release();
            }
            else
            {
                fashionCeilGrid_Accessory.Select();
            }
        }

        #endregion

        #region DataOperation
        private void OnUseBagItem(uint fashionId)
        {
            EHeroModelParts parts = Sys_Fashion.Instance.parts[fashionId];
            switch (parts)
            {
                case EHeroModelParts.Main:
                    curParentLable = 1;
                    FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == fashionId);
                    if (fashionClothes != null)
                    {
                        curFashionClotheSelectIndex = Sys_Fashion.Instance._FashionClothes.IndexOf(fashionClothes);
                        if (curFashionClotheSelectIndex == -1)
                        {
                            curFashionClotheSelectIndex = 0;
                        }
                        showParts[EHeroModelParts.Main] = fashionId;
                    }
                    break;
                case EHeroModelParts.Weapon:
                    bool result = Sys_Equip.Instance.GetCurWeapon() != Constants.UMARMEDID;
                    if (!result)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009548));
                        break;
                    }
                    curParentLable = 2;
                    FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == fashionId);
                    if (fashionWeapon != null)
                    {
                        curFashionWeaponSelectIndex = Sys_Fashion.Instance._FashionWeapons.IndexOf(fashionWeapon);
                        if (curFashionWeaponSelectIndex == -1)
                        {
                            curFashionWeaponSelectIndex = 0;
                        }
                        showParts[EHeroModelParts.Weapon] = fashionId;
                        curFashionWeapon = fashionWeapon;
                    }
                    break;
                case EHeroModelParts.Jewelry_Head:
                    FashionAccessory fashionAccessory_Head = Sys_Fashion.Instance._FashionAcce_head_2.Find(x => x.Id == fashionId);
                    if (fashionAccessory_Head != null)
                    {
                        curFashionJewelry_HeadIndex = Sys_Fashion.Instance._FashionAcce_head_2.IndexOf(fashionAccessory_Head);
                        if (curFashionJewelry_HeadIndex == -1)
                        {
                            curFashionJewelry_HeadIndex = 0;
                        }
                    }
                    curSelectFashionAcceChildLable = EHeroModelParts.Jewelry_Head;
                    CP_ToggleRegistry_Parent.SwitchTo(3);
                    break;
                case EHeroModelParts.Jewelry_Back:
                    FashionAccessory fashionAccessory_Back = Sys_Fashion.Instance._FashionAcce_back_3.Find(x => x.Id == fashionId);
                    if (fashionAccessory_Back != null)
                    {
                        curFashionJewelry_BackIndex = Sys_Fashion.Instance._FashionAcce_back_3.IndexOf(fashionAccessory_Back);
                        if (curFashionJewelry_BackIndex == -1)
                        {
                            curFashionJewelry_BackIndex = 0;
                        }
                    }
                    curSelectFashionAcceChildLable = EHeroModelParts.Jewelry_Back;
                    CP_ToggleRegistry_Parent.SwitchTo(3);
                    break;
                case EHeroModelParts.Jewelry_Waist:
                    showParts[parts] = fashionId;
                    FashionAccessory fashionAccessory_Waist = Sys_Fashion.Instance._FashionAcce_waist_4.Find(x => x.Id == fashionId);
                    if (fashionAccessory_Waist != null)
                    {
                        curFashionJewelry_WaistSelectIndex = Sys_Fashion.Instance._FashionAcce_waist_4.IndexOf(fashionAccessory_Waist);
                        if (curFashionJewelry_WaistSelectIndex == -1)
                        {
                            curFashionJewelry_WaistSelectIndex = 0;
                        }
                    }
                    curSelectFashionAcceChildLable = EHeroModelParts.Jewelry_Waist;
                    CP_ToggleRegistry_Parent.SwitchTo(3);
                    break;
                case EHeroModelParts.Jewelry_Face:
                    showParts[parts] = fashionId;
                    FashionAccessory fashionAccessory1 = Sys_Fashion.Instance._FashionAcce_face_5.Find(x => x.Id == fashionId);
                    if (fashionAccessory1 != null)
                    {
                        curFashionJewelry_FaceSelectIndex = Sys_Fashion.Instance._FashionAcce_face_5.IndexOf(fashionAccessory1);
                        if (curFashionJewelry_FaceSelectIndex == -1)
                        {
                            curFashionJewelry_FaceSelectIndex = 0;
                        }
                    }
                    curSelectFashionAcceChildLable = EHeroModelParts.Jewelry_Face;
                    CP_ToggleRegistry_Parent.SwitchTo(3);
                    break;
                default:
                    break;
            }

        }

        private void ResetDressData()
        {
            curFashionClotheSelectIndex = Sys_Fashion.Instance.GetDressFashionIndex();
            curFashionWeaponSelectIndex = Sys_Fashion.Instance.GetDressWeaponIndex();
            curFashionSuitSelectIndex = Sys_Fashion.Instance.GetDressSuitIndex();
            curFashionJewelry_HeadIndex = Sys_Fashion.Instance.GetDressAcceIndex(EHeroModelParts.Jewelry_Head);
            curFashionJewelry_WaistSelectIndex = Sys_Fashion.Instance.GetDressAcceIndex(EHeroModelParts.Jewelry_Waist);
            curFashionJewelry_BackIndex = Sys_Fashion.Instance.GetDressAcceIndex(EHeroModelParts.Jewelry_Back);
            curFashionJewelry_FaceSelectIndex = Sys_Fashion.Instance.GetDressAcceIndex(EHeroModelParts.Jewelry_Face);
            curMainPartShowId = GetDressMainPartShowId();
            curWeaponPartShowId = GetDressWeaponPartShowId();
            curAccePartShowId = GetDressAccePartShowId();

            curSelectFashionAcceChildLable = EHeroModelParts.Jewelry_Back;

            showParts[EHeroModelParts.Main] = curMainPartShowId;
            curFashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == curMainPartShowId);
            if (curWeaponPartShowId != 0)
            {
                b_WeaponChanged = !showParts.ContainsKey(EHeroModelParts.Weapon);
                showParts[EHeroModelParts.Weapon] = curWeaponPartShowId;
                curFashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == curWeaponPartShowId);
            }
            if (curAccePartShowId.Count > 0)
            {
                for (int i = 0; i < curAccePartShowId.Count; i++)
                {
                    EHeroModelParts eHeroModelParts = Sys_Fashion.Instance.parts[curAccePartShowId[i]];
                    showParts[eHeroModelParts] = curAccePartShowId[i];
                }
            }
        }

        private void LoadMoreFormSuit2Other(EHeroModelParts filterPart)
        {
            if (formSuit2OtherChanges.Count == 0)
            {
                return;
            }
            for (int i = 0; i < formSuit2OtherChanges.Count; i++)
            {
                if (filterPart == formSuit2OtherChanges[i])
                {
                    continue;
                }
                LoadModelParts(showParts[formSuit2OtherChanges[i]], formSuit2OtherChanges[i]);
            }
            formSuit2OtherChanges.Clear();
        }

        private void UpdateSelectFashionIndex(EHeroModelParts eHeroModelParts)
        {
            if (!showParts.TryGetValue(eHeroModelParts, out uint id))
            {
                return;
            }
            if (eHeroModelParts == EHeroModelParts.Main)
            {
                FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == id);
                curFashionClotheSelectIndex = Sys_Fashion.Instance._FashionClothes.IndexOf(fashionClothes);
                if (curFashionClotheSelectIndex == -1)
                {
                    curFashionClotheSelectIndex = 0;
                }
            }
            else if (eHeroModelParts == EHeroModelParts.Weapon)
            {
                FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == id);
                curFashionWeaponSelectIndex = Sys_Fashion.Instance._FashionWeapons.IndexOf(fashionWeapon);
                if (curFashionWeaponSelectIndex == -1)
                {
                    curFashionWeaponSelectIndex = 0;
                }
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Head)
            {
                FashionAccessory fashionAccessory = Sys_Fashion.Instance._FashionAcce_head_2.Find(x => x.Id == id);
                curFashionJewelry_HeadIndex = Sys_Fashion.Instance._FashionAcce_head_2.IndexOf(fashionAccessory);
                if (curFashionJewelry_HeadIndex == -1)
                {
                    curFashionJewelry_HeadIndex = 0;
                }
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Back)
            {
                FashionAccessory fashionAccessory = Sys_Fashion.Instance._FashionAcce_back_3.Find(x => x.Id == id);
                curFashionJewelry_BackIndex = Sys_Fashion.Instance._FashionAcce_back_3.IndexOf(fashionAccessory);
                if (curFashionJewelry_BackIndex == -1)
                {
                    curFashionJewelry_BackIndex = 0;
                }
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                FashionAccessory fashionAccessory = Sys_Fashion.Instance._FashionAcce_waist_4.Find(x => x.Id == id);
                curFashionJewelry_WaistSelectIndex = Sys_Fashion.Instance._FashionAcce_waist_4.IndexOf(fashionAccessory);
                if (curFashionJewelry_WaistSelectIndex == -1)
                {
                    curFashionJewelry_WaistSelectIndex = 0;
                }
            }
            else if (eHeroModelParts == EHeroModelParts.Jewelry_Face)
            {
                FashionAccessory fashionAccessory = Sys_Fashion.Instance._FashionAcce_face_5.Find(x => x.Id == id);
                curFashionJewelry_FaceSelectIndex = Sys_Fashion.Instance._FashionAcce_face_5.IndexOf(fashionAccessory);
                if (curFashionJewelry_FaceSelectIndex == -1)
                {
                    curFashionJewelry_FaceSelectIndex = 0;
                }
            }
        }

        private void UpdateSelectSuitIndex()
        {
            if (!showParts.TryGetValue(EHeroModelParts.Main, out uint id))
            {
                return;
            }
            curFashionSuitSelectIndex = 0;
            for (int i = 0; i < Sys_Fashion.Instance._FashionSuits.Count; i++)
            {
                FashionSuit fashionSuit = Sys_Fashion.Instance._FashionSuits[i];
                if (fashionSuit.cSVFashionSuitData.FashionId == id)
                {
                    curFashionSuitSelectIndex = Sys_Fashion.Instance._FashionSuits.IndexOf(fashionSuit);
                    break;
                }
            }
            if (curFashionSuitSelectIndex == -1)
            {
                curFashionSuitSelectIndex = 0;
            }
        }

        private void AddCondition()
        {
            //时装主页签
            CP_ToggleRegistry_Parent.AddCondition(2, () =>
            {
                bool result = Sys_Equip.Instance.GetCurWeapon() != Constants.UMARMEDID;
                if (!result)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009548));
                }
                return result;
            });


            //染色主页签
            CP_ToggleRegistry_DyeParent.AddCondition(2, () =>
             {
                 bool result1 = Sys_Equip.Instance.GetCurWeapon() != Constants.UMARMEDID;
                 if (!result1)
                 {
                     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009548));
                 }
                 bool result2 = showParts.ContainsKey(EHeroModelParts.Weapon);
                 if (!result2)
                 {
                     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009551));
                 }
                 return result1 && result2;
             });

            CP_ToggleRegistry_DyeParent.AddCondition(1, () =>
            {
                bool result1 = showParts.ContainsKey(EHeroModelParts.Main);
                bool result2 = true;
                if (!result1)
                {
                    string str = CSVLanguage.Instance.GetConfData(2009552).words;
                    Sys_Hint.Instance.PushContent_Normal(str);
                }
                else
                {
                    //uint id = showParts[EHeroModelParts.Main];
                    //FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == id);
                    //result2 = fashionClothes.UnLock;
                    //if (!result2)
                    //{
                    //    string str = CSVLanguage.Instance.GetConfData(2009552).words;
                    //    Sys_Hint.Instance.PushContent_Normal(str);
                    //}
                }
                return result1 && result2;
            });

            //染色模式切换(定制 色板)
            CP_ToggleRegistry_DyeMoudleSwitch.AddCondition(1, () =>
            {
                bool result = false;
                if (curDyeParentLable == 1)
                {
                    result = curFashionClothes.cSVFashionClothesData.ColorSwitch == 1;
                }
                else if (curDyeParentLable == 2)
                {
                    result = curFashionWeapon.cSVFashionWeaponData.ColorSwitch == 1;
                }
                if (!result)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009576));
                }
                return result;
            });


            CP_ToggleRegistry_Scheme.AddCondition(0, () =>
             {
                 if (m_CurrentScheme == 0)
                 {
                     return true;
                 }
                 //是否有方案一
                 bool result_HasScheme1 = false;
                 if (curDyeParentLable == 1)
                 {
                     result_HasScheme1 = curFashionClothes.schemes[0] != 0;
                 }
                 else if (curDyeParentLable == 2)
                 {
                     result_HasScheme1 = curFashionWeapon.schemes[0] != 0;
                 }
                 else if (curDyeParentLable == 3)
                 {
                     result_HasScheme1 = curFashionAccessory.schemes[0] != 0;
                 }
                 if (!result_HasScheme1)
                 {
                     Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002201));
                     return false;
                 }

                 //是否想要切换到方案一
                 int toScheme = 1 - m_CurrentScheme;
                 string content = LanguageHelper.GetTextContent(590002209, (toScheme + 1).ToString());
                 PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                 () =>
                 {
                     m_CPToggle1.SetToggleIsNotChange(true);
                     CP_ToggleRegistry_Scheme.SwitchTo(toScheme);
                 });
                 return true;
             });

            CP_ToggleRegistry_Scheme.AddCondition(1, () =>
            {
                if (m_CurrentScheme == 1)
                {
                    return true;
                }
                bool result_HasScheme2 = false;
                if (curDyeParentLable == 1)
                {
                    result_HasScheme2 = curFashionClothes.schemes[1] != 0;
                }
                else if (curDyeParentLable == 2)
                {
                    result_HasScheme2 = curFashionWeapon.schemes[1] != 0;
                }
                else if (curDyeParentLable == 3)
                {
                    result_HasScheme2 = curFashionAccessory.schemes[0] != 0;
                }
                if (!result_HasScheme2)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002201));
                    return false;
                }

                //是否想要切换到方案二

                int toScheme = 1 - m_CurrentScheme;
                string content = LanguageHelper.GetTextContent(590002209, (toScheme + 1).ToString());
                PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                () =>
                {
                    m_CPToggle2.SetToggleIsNotChange(true);
                    CP_ToggleRegistry_Scheme.SwitchTo(toScheme);
                });
                return true;
            });
        }

        private void ClearCondition()
        {
            CP_ToggleRegistry_Parent.ClearCondition();
            CP_ToggleRegistry_DyeParent.ClearCondition();
        }

        #endregion

        private void SwitchToDressClothes(bool triggerEvent = false)
        {
            CP_ToggleRegistry_Parent.SwitchTo(1);
            OnUpdateFashionClothesGrid();
        }

        private void SetAcceRoot(bool active)
        {
            acceRoot.SetActive(active);
        }

        private void RefreshFashionScrollView(int lable)
        {
            if (lable == 0)
            {
                scrollView_suit.SetActive(true);
                scrollView_clothes.SetActive(false);
                scrollView_weapon.SetActive(false);
                m_InfinityGrid_Suit.CellCount = Sys_Fashion.Instance._FashionSuits.Count;
                m_InfinityGrid_Suit.ForceRefreshActiveCell();
                OnUpdateFashionSuidGrid();
                m_InfinityGrid_Suit.MoveToIndex(curFashionSuitSelectIndex);
            }
            else if (lable == 1)
            {
                scrollView_suit.SetActive(false);
                scrollView_clothes.SetActive(true);
                scrollView_weapon.SetActive(false);
                m_InfinityGrid_Clothes.CellCount = Sys_Fashion.Instance._FashionClothes.Count;
                m_InfinityGrid_Clothes.ForceRefreshActiveCell();
                OnUpdateFashionClothesGrid();
                m_InfinityGrid_Clothes.MoveToIndex(curFashionClotheSelectIndex);
            }
            else if (lable == 2)
            {
                scrollView_suit.SetActive(false);
                scrollView_clothes.SetActive(false);
                scrollView_weapon.SetActive(true);
                m_InfinityGrid_Weapon.CellCount = Sys_Fashion.Instance._FashionWeapons.Count;
                m_InfinityGrid_Weapon.ForceRefreshActiveCell();
                OnUpdateFashionWeaponGrid();
                m_InfinityGrid_Weapon.MoveToIndex(curFashionWeaponSelectIndex);
            }

            else
            {
                scrollView_suit.SetActive(false);
                scrollView_clothes.SetActive(false);
                scrollView_weapon.SetActive(false);
            }
        }

        #region ToggleSwitch

        private void onParentToggleChanged(int curToggle, int old)
        {
            if (curToggle == old)
                return;
            if (curToggle == 3)
            {
                if (Sys_Fashion.Instance._FashionAccessories.Count == 0)
                {
                    string content = CSVLanguage.Instance.GetConfData(2009549).words;
                    Sys_Hint.Instance.PushContent_Normal(content);
                    return;
                }
                scrollView_acce.SetActive(true);
                bAcceVaild = true;
                UpdateLablePage(null);
            }
            else
            {
                scrollView_acce.SetActive(false);
                bAcceVaild = false;
                if (curToggle == 0)
                {
                    if (Sys_Fashion.Instance._FashionSuits.Count == 0)
                    {
                        string content = CSVLanguage.Instance.GetConfData(2009550).words;
                        Sys_Hint.Instance.PushContent_Normal(content);
                        return;
                    }
                    if (Sys_Fashion.Instance.DressSuit)
                    {
                        Sys_Fashion.Instance.eventEmitter.Trigger<bool>(Sys_Fashion.EEvents.OnUpdateSuitChange, true);
                    }
                }
                else
                {
                    Sys_Fashion.Instance.eventEmitter.Trigger<bool>(Sys_Fashion.EEvents.OnUpdateSuitChange, false);
                }
            }
            CurParentLable = curToggle;
        }

        /// <summary>
        /// 套装 时装 武器 挂饰 大页签切换
        /// </summary>
        private void OnParentLableChanged()
        {
            if (curParentLable == 0)
            {
                suitRoot.SetActive(true);
                wearRoot.SetActive(false);
            }
            else
            {
                formSuit2OtherChanges.Clear();
                suitRoot.SetActive(false);
                wearRoot.SetActive(true);
                //如果是从套装切换到其他部件 此时showParts严格等于对应套装的部件id，如果初始穿戴了其他部件 需要再加回去
                //不包含的情况下 才加回去 包含的情况下 套装部件替换掉原始穿戴
                if (!showParts.ContainsKey(EHeroModelParts.Weapon))
                {
                    uint dressWeaponId = GetDressWeaponPartShowId();
                    if (dressWeaponId != 0)
                    {
                        showParts[EHeroModelParts.Weapon] = dressWeaponId;
                    }
                }
                List<uint> dressAccePartShowId = GetDressAccePartShowId();
                if (!showParts.ContainsKey(EHeroModelParts.Jewelry_Waist))
                {
                    for (int i = 0; i < dressAccePartShowId.Count; i++)
                    {
                        if (dressAccePartShowId[i] != 0)
                        {
                            EHeroModelParts eHeroModelParts = Sys_Fashion.Instance.parts[dressAccePartShowId[i]];
                            if (eHeroModelParts == EHeroModelParts.Jewelry_Waist)
                            {
                                showParts[eHeroModelParts] = dressAccePartShowId[i];
                                formSuit2OtherChanges.Add(eHeroModelParts);
                            }
                        }
                    }
                }
                if (!showParts.ContainsKey(EHeroModelParts.Jewelry_Face))
                {
                    for (int i = 0; i < dressAccePartShowId.Count; i++)
                    {
                        if (dressAccePartShowId[i] != 0)
                        {
                            EHeroModelParts eHeroModelParts = Sys_Fashion.Instance.parts[dressAccePartShowId[i]];
                            if (eHeroModelParts == EHeroModelParts.Jewelry_Face)
                            {
                                showParts[eHeroModelParts] = dressAccePartShowId[i];
                                formSuit2OtherChanges.Add(eHeroModelParts);
                            }
                        }
                    }
                }
            }
            if (curParentLable == 3)
            {
                CP_ToggleRegistry_Child.SwitchTo((int)curSelectFashionAcceChildLable);
                bAcceVaild = true;
            }
            else
            {
                curChildLable = 0;
            }
            RefreshFashionScrollView(curParentLable);
        }

        private void OnChildToggleChanged(int curToggle, int old)
        {
            CurChildLable = curToggle;
        }

        private void OnChildTableChanged()
        {
            if (curChildLable == 3)
            {
                fashionAccessories = Sys_Fashion.Instance._FashionAcce_back_3;
            }
            else if (curChildLable == 4)
            {
                fashionAccessories = Sys_Fashion.Instance._FashionAcce_waist_4;
            }
            else if (curChildLable == 5)
            {
                fashionAccessories = Sys_Fashion.Instance._FashionAcce_face_5;
            }
            m_InfinityGrid_Acce.CellCount = fashionAccessories.Count;
            m_InfinityGrid_Acce.ForceRefreshActiveCell();
            OnSelectFristAcce();
        }

        #endregion

        #region ChangeFashionGridCallback
        private void OnSelectFristAcce()
        {
            if (IsClickLimitTime())
                return;
            if (bagUsefashionId == 0)
            {
                b_MainChanged = false;
                b_WeaponChanged = false;
            }
            if (fashionAccessories.Count == 0)
            {
                string str = string.Empty;
                if (curChildLable == 3)
                {
                    str = CSVLanguage.Instance.GetConfData(2009556).words;
                    Sys_Hint.Instance.PushContent_Normal(str);
                }
                if (curChildLable == 4)
                {
                    str = CSVLanguage.Instance.GetConfData(2009554).words;
                }
                else if (curChildLable == 5)
                {
                    str = CSVLanguage.Instance.GetConfData(2009555).words;
                }
                Sys_Hint.Instance.PushContent_Normal(str);
                return;
            }
            if (curChildLable == (int)EHeroModelParts.Jewelry_Head)
            {
                curAcceSelectIndex = curFashionJewelry_HeadIndex;
            }
            else if (curChildLable == (int)EHeroModelParts.Jewelry_Back)
            {
                curAcceSelectIndex = curFashionJewelry_BackIndex;
            }
            else if (curChildLable == (int)EHeroModelParts.Jewelry_Waist)
            {
                curAcceSelectIndex = curFashionJewelry_WaistSelectIndex;
            }
            else if (curChildLable == (int)EHeroModelParts.Jewelry_Face)
            {
                curAcceSelectIndex = curFashionJewelry_FaceSelectIndex;
            }
            foreach (var item in fashionCeilGrid_Accessorys)
            {
                if (item.Value.dataIndex != curAcceSelectIndex)
                {
                    item.Value.Release();
                    item.Value.dress = false;
                }
                else
                {
                    item.Value.Select();
                    item.Value.dress = true;
                }
            }
            curFashionAccessory = fashionAccessories[curAcceSelectIndex];
            curFashionType = 3;
            UpdateFashionDesPoint(curFashionAccessory.cSVFashionAccessoryData.FashionScore);
            SetFashionName(CSVLanguage.Instance.GetConfData(curFashionAccessory.cSVFashionAccessoryData.AccName).words);
            SetTitle();
            SetWearDescribe(CSVLanguage.Instance.GetConfData(curFashionAccessory.cSVFashionAccessoryData.AccDescribe).words);
            SetColorBtnActive();
            UpdatePropRoot();
            UpdateUnLockButton();
            UpdateTag(false, false, false);
            #region  *****  Attr *******
            //SetFashionAttr();
            #endregion

            EHeroModelParts eHeroModelParts = Sys_Fashion.Instance.parts[curFashionAccessory.Id];
            LoadModelParts(curFashionAccessory.Id, eHeroModelParts);
            LoadMoreFormSuit2Other(eHeroModelParts);
        }

        private void OnAcceGridSelected(FashionCeilGrid_Accessory fashionCeilGrid_Accessory)
        {
            if (IsClickLimitTime())
                return;
            b_MainChanged = false;
            b_WeaponChanged = false;
            curAcceSelectIndex = fashionCeilGrid_Accessory.dataIndex;
            foreach (var item in fashionCeilGrid_Accessorys)
            {
                if (!item.Key.activeSelf)
                    continue;
                if (item.Value.dataIndex == curAcceSelectIndex)
                {
                    item.Value.Select();
                    //刷新当前面板挂饰数据
                    curFashionAccessory = fashionCeilGrid_Accessory.mFashionAccessory;
                    SetFashionName(CSVLanguage.Instance.GetConfData(curFashionAccessory.cSVFashionAccessoryData.AccName).words);
                    SetTitle();
                    SetWearDescribe(CSVLanguage.Instance.GetConfData(curFashionAccessory.cSVFashionAccessoryData.AccDescribe).words);
                }
                else
                {
                    item.Value.Release();
                }
            }
            curFashionType = 3;
            UpdateFashionDesPoint(curFashionAccessory.cSVFashionAccessoryData.FashionScore);
            SetColorBtnActive();
            UpdatePropRoot();
            UpdateUnLockButton();
            UpdateTag(false, false, false);
            #region ***************  Attr  ****************
            //SetFashionAttr();
            #endregion
            EHeroModelParts eHeroModelParts = Sys_Fashion.Instance.parts[curFashionAccessory.Id];
            LoadModelParts(curFashionAccessory.Id, eHeroModelParts);
            LoadMoreFormSuit2Other(eHeroModelParts);
        }

        /// <summary>
        /// 套装格子切换
        /// </summary>
        /// <param name="i"></param>
        private void OnFashionSuitGridSelect(FashionCeilGrid_Suit fashionCeilGrid_Suit)
        {
            if (IsClickLimitTime())
                return;
            if (curFashionSuitSelectIndex == fashionCeilGrid_Suit.dataIndex)
            {
                return;
            }
            curFashionSuitSelectIndex = fashionCeilGrid_Suit.dataIndex;
           
            OnUpdateFashionSuidGrid();
        }

        private void OnUpdateFashionSuidGrid()
        {
            b_MainChanged = true;
            b_WeaponChanged = !showParts.ContainsKey(EHeroModelParts.Weapon);

            foreach (var item in m_FashionCeilGrid_Suits)
            {
                if (item.Value.dataIndex == curFashionSuitSelectIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
            FashionSuit fashionSuit = Sys_Fashion.Instance._FashionSuits[curFashionSuitSelectIndex];
            curFashionSuit = fashionSuit;
            curFashionType = 0;
            UpdateLablePage(string.Format("{0}/{1}", curFashionSuitSelectIndex + 1, Sys_Fashion.Instance._FashionSuits.Count));
            SetFashionName(CSVLanguage.Instance.GetConfData(curFashionSuit.cSVFashionSuitData.SuitName).words);
            //SetSuitTitle(string.Format("{0}/{1}", curFashionSuit.unlockedAssoiated.Count, curFashionSuit.associated.Count));
            OnSetSuitTitle();
            OnSetSuitGrid();
            #region *********** Attr **************
            //SetSuitAttr();
            #endregion
            SetColorBtnActive();
            UpdateUnLockButton();
            UnloadWearParts();
            for (int j = 0; j < curFashionSuit.associated.Count; j++)
            {
                uint id = curFashionSuit.associated[j];
                EHeroModelParts parts = Sys_Fashion.Instance.parts[id];
                LoadModelParts(id, parts);
                UpdateSelectFashionIndex(parts);
            }
        }

        /// <summary>
        /// 时装格子切换
        /// </summary>
        /// <param name="i"></param>
        private void OnFashionClothesGridSelect(FashionCeilGrid_Clothes fashionCeilGrid_Clothes)
        {
            if (IsClickLimitTime())
                return;
            if (curFashionClotheSelectIndex == fashionCeilGrid_Clothes.dataIndex)
            {
                return;
            }
            curFashionClotheSelectIndex = fashionCeilGrid_Clothes.dataIndex;

            OnUpdateFashionClothesGrid();
        }

        private void OnUpdateFashionClothesGrid()
        {
            b_MainChanged = true;
            b_WeaponChanged = !showParts.ContainsKey(EHeroModelParts.Weapon);

            foreach (var item in m_FashionCeilGrid_Clothes)
            {
                if (item.Value.dataIndex == curFashionClotheSelectIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
            FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes[curFashionClotheSelectIndex];
            curFashionClothes = fashionClothes;
            curFashionType = 1;
            UpdateFashionDesPoint(curFashionClothes.cSVFashionClothesData.FashionScore);
            UpdateLablePage(string.Format("{0}/{1}", curFashionClotheSelectIndex + 1, Sys_Fashion.Instance._FashionClothes.Count));
            SetFashionName(CSVLanguage.Instance.GetConfData(curFashionClothes.cSVFashionClothesData.FashionName).words);
            UpdateTag(curFashionClothes.cSVFashionClothesData.Tag[0] == 1, curFashionClothes.cSVFashionClothesData.Tag[1] == 1, curFashionClothes.cSVFashionClothesData.Tag[2] == 1);
            SetTitle();
            SetWearDescribe(CSVLanguage.Instance.GetConfData(curFashionClothes.cSVFashionClothesData.FashionDescribe).words);
            SetColorBtnActive();
            UpdatePropRoot();
            UpdateUnLockButton();
            #region ***************  Attr  ****************
            //SetFashionAttr();
            #endregion
            LoadModelParts(curFashionClothes.Id, EHeroModelParts.Main);
            LoadMoreFormSuit2Other(EHeroModelParts.Main);
            UpdateSelectSuitIndex();
        }

        /// <summary>
        /// 武器格子切换
        /// </summary>
        /// <param name="i"></param>
        private void OnFashionWeaponGridSelect(FashionCeilGrid_Weapon fashionCeilGrid_Weapon)
        {
            if (IsClickLimitTime())
                return;

            if (curFashionWeaponSelectIndex == fashionCeilGrid_Weapon.dataIndex)
            {
                return;
            }
            curFashionWeaponSelectIndex = fashionCeilGrid_Weapon.dataIndex;

            OnUpdateFashionWeaponGrid();
        }

        private void OnUpdateFashionWeaponGrid()
        {
            b_MainChanged = false;
            b_WeaponChanged = true;

            foreach (var item in m_FashionCeilGrid_Weapon)
            {
                if (item.Value.dataIndex == curFashionWeaponSelectIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
            FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons[curFashionWeaponSelectIndex];
            curFashionWeapon = fashionWeapon;
            curFashionType = 2;
            UpdateFashionDesPoint(curFashionWeapon.cSVFashionWeaponData.FashionScore);
            UpdateLablePage(string.Format("{0}/{1}", curFashionWeaponSelectIndex + 1, Sys_Fashion.Instance._FashionWeapons.Count));
            SetFashionName(CSVLanguage.Instance.GetConfData(curFashionWeapon.cSVFashionWeaponData.WeaponName).words);
            UpdateTag(curFashionWeapon.cSVFashionWeaponData.Tag[0] == 1, curFashionWeapon.cSVFashionWeaponData.Tag[1] == 1, curFashionWeapon.cSVFashionWeaponData.Tag[2] == 1);
            SetTitle();
            SetWearDescribe(CSVLanguage.Instance.GetConfData(curFashionWeapon.cSVFashionWeaponData.WeaponDescribe).words);
            SetColorBtnActive();
            UpdatePropRoot();
            UpdateUnLockButton();
            #region ***************  Attr  ****************
            //SetFashionAttr();
            #endregion
            LoadModelParts(curFashionWeapon.Id, EHeroModelParts.Weapon);
            LoadMoreFormSuit2Other(EHeroModelParts.Weapon);
        }

        #endregion

        #region RightView_FashionDes

        private void UpdateLablePage(string str)
        {
            if (string.IsNullOrEmpty(str)) { labelPage.SetActive(false); return; }
            labelPage.SetActive(true);
            labelPage.transform.Find("Text").GetComponent<Text>().text = str;
        }

        private void UpdateTag(bool custom, bool free, bool action)
        {
            m_CustomTag.SetActive(custom);
            m_FreeTag.SetActive(free);
            m_ActionTag.SetActive(action);
        }

        private void SetFashionName(string str)
        {
            fashionName.text = str;
        }

        private void SetTitle()
        {
            uint title = 0;
            countdown.SetActive(false);
            limitType.text = string.Empty;
            if (curFashionType == 1)
            {
                if (curFashionClothes.cSVFashionClothesData.LimitedTime == 0)
                {
                    title = 2009533;
                }
                else
                {
                    if (curFashionClothes.UnLock)
                    {
                        if (curFashionClothes.ownerType == OwnerType.Forever)
                        {
                            title = 2009533;
                        }
                        else if (curFashionClothes.ownerType == OwnerType.Limit)
                        {
                            title = 2009511;
                            countdown.SetActive(true);
                        }
                    }
                    else
                    {
                        title = 2009534;
                    }
                }
                SetTitle(title);
            }
            if (curFashionType == 2)
            {
                if (curFashionWeapon.cSVFashionWeaponData.LimitedTime == 0)
                {
                    title = 2009533;
                }
                else
                {
                    if (curFashionWeapon.UnLock)
                    {
                        if (curFashionWeapon.ownerType == OwnerType.Forever)
                        {
                            title = 2009533;
                        }
                        else if (curFashionWeapon.ownerType == OwnerType.Limit)
                        {
                            title = 2009511;
                            countdown.SetActive(true);
                        }
                    }
                    else
                    {
                        title = 2009534;
                    }
                }
                SetTitle(title);
            }
            if (curFashionType == 3)
            {
                if (curFashionAccessory.cSVFashionAccessoryData.LimitedTime == 0)
                {
                    title = 2009533;
                }
                else
                {
                    if (curFashionAccessory.UnLock)
                    {
                        if (curFashionAccessory.ownerType == OwnerType.Forever)
                        {
                            title = 2009533;
                        }
                        else if (curFashionAccessory.ownerType == OwnerType.Limit)
                        {
                            title = 2009511;
                            countdown.SetActive(true);
                        }
                    }
                    else
                    {
                        title = 2009534;
                    }
                }
                SetTitle(title);
            }
        }

        private void SetTitle(uint content)
        {
            text_Titlefashion.gameObject.SetActive(true);
            text_Titlesuit.gameObject.SetActive(false);
            TextHelper.SetText(text_Titlefashion, content);
        }

        private void OnSetSuitTitle()
        {
            if (curFashionSuit != null)
            {
                SetSuitTitle(string.Format("{0}/{1}", curFashionSuit.unlockedAssoiated.Count, curFashionSuit.associated.Count));
            }
        }

        private void SetSuitTitle(string content)
        {
            text_Titlesuit.gameObject.SetActive(true);
            text_Titlefashion.gameObject.SetActive(false);
            TextHelper.SetText(text_Titlesuit, content);
        }

        private void SetWearDescribe(string des)
        {
            describe.text = des;
        }

        private void UpdateFashionDesPoint(uint value)
        {
            TextHelper.SetText(m_FashionPoint_Des, value.ToString());
        }

        #endregion

        #region PropRoot

        private void UpdatePropRoot()
        {
            foreach (var item in wearPropButtons)
            {
                item.onClick.RemoveAllListeners();
            }
            wearPropButtons.Clear();

            propRoot.gameObject.SetActive(true);
            List<uint> itemIds = new List<uint>();
            if (curFashionType == 1)
            {
                if (curFashionClothes.cSVFashionClothesData.FashionItem != null)
                {
                    itemIds.AddRange(curFashionClothes.cSVFashionClothesData.FashionItem);
                }
                if (curFashionClothes.cSVFashionClothesData.LimitedTime == 0 || curFashionClothes.ownerType == OwnerType.Forever)
                {
                    if (curFashionClothes.cSVFashionClothesData.FashionItem == null)
                    {
                        propRoot.gameObject.SetActive(false);
                        empty2.SetActive(true);
                        return;
                    }
                    else
                    {
                        propRoot.gameObject.SetActive(true);
                        empty2.SetActive(false);
                    }
                }
                if (curFashionClothes.CanLock() || curFashionClothes.UnLock)
                {
                    propRoot.Find("Btn_None").gameObject.SetActive(false);
                }
                else
                {
                    propRoot.Find("Btn_None").gameObject.SetActive(true);
                }

                if (bagUsefashionId == curFashionClothes.cSVFashionClothesData.id && bagUseItemId != 0)
                {
                    itemIds.Clear();
                    itemIds.Add(bagUseItemId);
                    bagUseItemId = 0;
                    bagUsefashionId = 0;
                }
            }
            if (curFashionType == 2)
            {
                if (curFashionWeapon.cSVFashionWeaponData.FashionItem != null)
                {
                    itemIds.AddRange(curFashionWeapon.cSVFashionWeaponData.FashionItem);
                }
                if (curFashionWeapon.cSVFashionWeaponData.LimitedTime == 0 || curFashionWeapon.ownerType == OwnerType.Forever)
                {
                    if (curFashionWeapon.cSVFashionWeaponData.FashionItem == null)
                    {
                        propRoot.gameObject.SetActive(false);
                        empty2.SetActive(true);
                        return;
                    }
                    else
                    {
                        propRoot.gameObject.SetActive(true);
                        empty2.SetActive(false);
                    }
                }
                if (curFashionWeapon.CanLock() || curFashionWeapon.UnLock)
                {
                    propRoot.Find("Btn_None").gameObject.SetActive(false);
                }
                else
                {
                    propRoot.Find("Btn_None").gameObject.SetActive(true);
                }
                if (bagUsefashionId == curFashionWeapon.cSVFashionWeaponData.id && bagUseItemId != 0)
                {
                    itemIds.Clear();
                    itemIds.Add(bagUseItemId);
                    bagUseItemId = 0;
                    bagUsefashionId = 0;
                }
            }
            if (curFashionType == 3)
            {
                if (curFashionAccessory.cSVFashionAccessoryData.AccItem != null)
                {
                    itemIds.AddRange(curFashionAccessory.cSVFashionAccessoryData.AccItem);
                }

                if (curFashionAccessory.cSVFashionAccessoryData.LimitedTime == 0 || curFashionAccessory.ownerType == OwnerType.Forever)
                {
                    if (curFashionAccessory.cSVFashionAccessoryData.AccItem == null)
                    {
                        propRoot.gameObject.SetActive(false);
                        empty2.SetActive(true);
                        return;
                    }
                    else
                    {
                        propRoot.gameObject.SetActive(true);
                        empty2.SetActive(false);
                    }
                }
                if (curFashionAccessory.CanLock() || curFashionAccessory.UnLock)
                {
                    propRoot.Find("Btn_None").gameObject.SetActive(false);
                }
                else
                {
                    propRoot.Find("Btn_None").gameObject.SetActive(true);
                }
                if (bagUsefashionId == curFashionAccessory.cSVFashionAccessoryData.id && bagUseItemId != 0)
                {
                    itemIds.Clear();
                    itemIds.Add(bagUseItemId);
                    bagUseItemId = 0;
                    bagUsefashionId = 0;
                }
            }
            Image image = propRoot.Find("Btn_Item/Image_Icon").GetComponent<Image>();
            if (itemIds.Count > 0)
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemIds[0]);
                ImageHelper.SetIcon(image, CSVItem.Instance.GetConfData(itemIds[0]).icon_id);
                Image mQuality = propRoot.Find("Btn_Item/Image_BG").GetComponent<Image>();
                ImageHelper.GetQualityColor_Frame(mQuality, (int)cSVItemData.quality);
                Button button = propRoot.Find("Btn_Item").GetComponent<Button>();
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemIds[0], 1, false, false, false, false, false, false, true);
                itemData.bShowBtnNo = false;
                button.onClick.AddListener(() =>
                {
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Fashion, itemData));
                });
                wearPropButtons.Add(button);
            }
        }

        #endregion

        #region UnLockButtonState

        private void UpdateUnLockButton()
        {
            if (curFashionType == 1)
            {
                if (curFashionClothes.Dress)
                {
                    if (curFashionClothes.ownerType == OwnerType.Limit)
                    {
                        bool canUnLockForever = false;
                        if (curFashionClothes.CanLock())
                        {
                            uint itemId = curFashionClothes.PropItem;
                            if (itemId == 0)
                            {
                                canUnLockForever = false;
                            }
                            else
                            {
                                canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                            }
                        }
                        _SetUnLockButton(4);
                        SetUnLock_2ButtonActive(canUnLockForever);
                    }
                    else
                    {
                        _SetUnLockButton(3);
                    }
                    TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009519);//已穿戴
                    SetDressButtonInterActable(false);
                    return;
                }
                if (curFashionClothes.ownerType == OwnerType.None)
                {
                    bool canUnLockForever = false;
                    if (curFashionClothes.CanLock())
                    {
                        uint itemId = curFashionClothes.PropItem;
                        if (itemId == 0)
                        {
                            canUnLockForever = false;
                        }
                        else
                        {
                            canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                        }
                        if (canUnLockForever)
                        {
                            _SetUnLockButton(2);
                            SetUnLock_2ButtonActive(true);
                        }
                        else
                        {
                            _SetUnLockButton(1);
                            SetUnLock_1ButtonActive(true);
                        }
                    }
                    else
                    {
                        _SetUnLockButton(1);
                        SetUnLock_1ButtonActive(false);
                    }
                    return;
                }
                else if (curFashionClothes.ownerType == OwnerType.Limit)
                {
                    bool canUnLockForever = false;
                    if (curFashionClothes.CanLock())
                    {
                        uint itemId = curFashionClothes.PropItem;
                        if (itemId == 0)
                        {
                            canUnLockForever = false;
                        }
                        else
                        {
                            canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                        }
                    }
                    _SetUnLockButton(4);
                    SetUnLock_2ButtonActive(canUnLockForever);
                    SetDressButtonInterActable(true);
                }
                else if (curFashionClothes.ownerType == OwnerType.Forever)
                {
                    _SetUnLockButton(3);
                    SetDressButtonInterActable(true);
                }
                TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009517);//穿戴
            }
            if (curFashionType == 2)
            {
                if (curFashionWeapon.Dress)
                {
                    if (curFashionWeapon.ownerType == OwnerType.Limit)
                    {
                        bool canUnLockForever = false;
                        if (curFashionWeapon.CanLock())
                        {
                            uint itemId = curFashionWeapon.PropItem;
                            if (itemId == 0)
                            {
                                canUnLockForever = false;
                            }
                            else
                            {
                                canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                            }
                        }
                        _SetUnLockButton(4);
                        SetUnLock_2ButtonActive(canUnLockForever);
                    }
                    else
                    {
                        _SetUnLockButton(3);
                    }
                    TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009518);//卸下
                    SetDressButtonInterActable(true);
                    return;
                }
                if (curFashionWeapon.ownerType == OwnerType.None)
                {
                    bool canUnLockForever = false;
                    if (curFashionWeapon.CanLock())
                    {
                        uint itemId = curFashionWeapon.PropItem;
                        if (itemId == 0)
                        {
                            canUnLockForever = false;
                        }
                        else
                        {
                            canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                        }
                        if (canUnLockForever)
                        {
                            _SetUnLockButton(2);
                            SetUnLock_2ButtonActive(true);
                        }
                        else
                        {
                            _SetUnLockButton(1);
                            SetUnLock_1ButtonActive(true);
                        }
                    }
                    else
                    {
                        _SetUnLockButton(1);
                        SetUnLock_1ButtonActive(false);
                    }
                    return;
                }
                else if (curFashionWeapon.ownerType == OwnerType.Limit)
                {
                    bool canUnLockForever = false;
                    if (curFashionWeapon.CanLock())
                    {
                        uint itemId = curFashionWeapon.PropItem;
                        if (itemId == 0)
                        {
                            canUnLockForever = false;
                        }
                        else
                        {
                            canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                        }
                    }
                    _SetUnLockButton(4);
                    SetUnLock_2ButtonActive(canUnLockForever);
                    SetDressButtonInterActable(true);
                }
                else if (curFashionWeapon.ownerType == OwnerType.Forever)
                {
                    _SetUnLockButton(3);
                    SetDressButtonInterActable(true);
                }
                TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009517);//穿戴
            }
            if (curFashionType == 3)
            {
                if (curFashionAccessory.Dress)
                {
                    if (curFashionAccessory.ownerType == OwnerType.Limit)
                    {
                        bool canUnLockForever = false;
                        if (curFashionAccessory.CanLock())
                        {
                            uint itemId = curFashionAccessory.PropItem;
                            if (itemId == 0)
                            {
                                canUnLockForever = false;
                            }
                            else
                            {
                                canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                            }
                        }
                        _SetUnLockButton(4);
                        SetUnLock_2ButtonActive(canUnLockForever);
                    }
                    else
                    {
                        _SetUnLockButton(3);
                    }
                    SetDressButtonInterActable(true);
                    TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009518);//卸下
                    return;
                }
                if (curFashionAccessory.ownerType == OwnerType.None)
                {
                    bool canUnLockForever = false;
                    if (curFashionAccessory.CanLock())
                    {
                        uint itemId = curFashionAccessory.PropItem;
                        if (itemId == 0)
                        {
                            canUnLockForever = false;
                        }
                        else
                        {
                            canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                        }
                        if (canUnLockForever)
                        {
                            _SetUnLockButton(2);
                            SetUnLock_2ButtonActive(true);
                        }
                        else
                        {
                            _SetUnLockButton(1);
                            SetUnLock_1ButtonActive(true);
                        }
                    }
                    else
                    {
                        _SetUnLockButton(1);
                        SetUnLock_1ButtonActive(false);
                    }
                    return;
                }
                else if (curFashionAccessory.ownerType == OwnerType.Limit)
                {
                    bool canUnLockForever = false;
                    if (curFashionAccessory.CanLock())
                    {
                        uint itemId = curFashionAccessory.PropItem;
                        if (itemId == 0)
                        {
                            canUnLockForever = false;
                        }
                        else
                        {
                            canUnLockForever = CSVItem.Instance.GetConfData(itemId).fun_value[1] == 0;
                        }
                    }
                    _SetUnLockButton(4);
                    SetUnLock_2ButtonActive(canUnLockForever);
                    SetDressButtonInterActable(true);
                }
                else if (curFashionAccessory.ownerType == OwnerType.Forever)
                {
                    _SetUnLockButton(3);
                    SetDressButtonInterActable(true);
                }
                TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009517);//穿戴
            }
            if (curFashionType == 0)
            {
                _SetUnLockButton(3);
                if (curFashionSuit.Dress)
                {
                    TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009519);//已穿戴
                    SetDressButtonInterActable(false);
                    return;
                }
                if (curFashionSuit.UnLock)
                {
                    TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009517);//穿戴
                    SetDressButtonInterActable(true);
                }
                else
                {
                    SetDressButtonInterActable(false);
                    TextHelper.SetText(dress.transform.Find("Text_01").GetComponent<Text>(), 2009517);//穿戴
                }
            }
        }

        private void SetUnLock_1ButtonActive(bool active)
        {
            ButtonHelper.Enable(unlock_1, active);
        }

        private void SetUnLock_2ButtonActive(bool active)
        {
            ButtonHelper.Enable(unlock_2, active);
        }

        private void SetDressButtonInterActable(bool active)
        {
            ButtonHelper.Enable(dress, active);
        }

        private void _SetUnLockButton(int index)
        {
            if (index == 1)
            {
                unlock_1.gameObject.SetActive(true);
                unlock_2.gameObject.SetActive(false);
                dress.gameObject.SetActive(false);
            }
            if (index == 2)
            {
                unlock_1.gameObject.SetActive(false);
                unlock_2.gameObject.SetActive(true);
                dress.gameObject.SetActive(false);
            }
            if (index == 3)
            {
                unlock_1.gameObject.SetActive(false);
                unlock_2.gameObject.SetActive(false);
                dress.gameObject.SetActive(true);
            }
            if (index == 4)
            {
                unlock_1.gameObject.SetActive(false);
                unlock_2.gameObject.SetActive(true);
                dress.gameObject.SetActive(true);
            }
        }

        #endregion


        private void OnUnLock_1ButtonClicked()
        {
            DyeScheme dyeScheme = new DyeScheme();
            if (curFashionType == 1)
            {
                for (uint i = 0; i < 4; i++)
                {
                    DyeInfo dyeInfo = new DyeInfo();
                    dyeInfo.DyeIndex = i;
                    dyeInfo.Value = ((Color32)curFashionClothes.GetFirstColor((ETintIndex)i)).ToUInt32();
                    dyeScheme.DyeInfo.Add(dyeInfo);
                }

                Sys_Fashion.Instance.UnlockFashionReq(curFashionClothes.Id, curFashionClothes.PropItem, dyeScheme);
            }
            if (curFashionType == 2)
            {
                for (uint i = 0; i < 1; i++)
                {
                    DyeInfo dyeInfo = new DyeInfo();
                    dyeInfo.DyeIndex = i;
                    dyeInfo.Value = ((Color32)curFashionWeapon.GetFirstColor((ETintIndex)i)).ToUInt32();
                    dyeScheme.DyeInfo.Add(dyeInfo);
                }
                Sys_Fashion.Instance.UnlockFashionReq(curFashionWeapon.Id, curFashionWeapon.PropItem, dyeScheme);
            }
            if (curFashionType == 3)
            {
                for (uint i = 0; i < 1; i++)
                {
                    DyeInfo dyeInfo = new DyeInfo();
                    dyeInfo.DyeIndex = i;
                    dyeInfo.Value = ((Color32)curFashionAccessory.GetFirstColor((ETintIndex)i)).ToUInt32();
                    dyeScheme.DyeInfo.Add(dyeInfo);
                }
                Sys_Fashion.Instance.UnlockFashionReq(curFashionAccessory.Id, curFashionAccessory.PropItem, dyeScheme);
            }
        }

        private void OnDressButtonClicked()
        {
            if (curFashionType == 1)
            {
                Sys_Fashion.Instance.DressFashionReq(curFashionClothes.Id);
            }
            if (curFashionType == 2)
            {
                if (curFashionWeapon.Dress)
                {
                    Sys_Fashion.Instance.FashionUnfixReq(curFashionWeapon.Id);
                }
                else
                {
                    Sys_Fashion.Instance.DressFashionReq(curFashionWeapon.Id);
                }
            }
            if (curFashionType == 3)
            {
                if (curFashionAccessory.Dress)
                {
                    Sys_Fashion.Instance.FashionUnfixReq(curFashionAccessory.Id);
                }
                else
                {
                    Sys_Fashion.Instance.DressFashionReq(curFashionAccessory.Id);
                }
            }
            if (curFashionType == 0)
            {
                Sys_Fashion.Instance.SuitSelReqReq(curFashionSuit.Id);
            }
            Sys_Hint.Instance.PushEffectInNextFight();
        }

        private void OnRevertToFirstClick()
        {
            PromptBoxParameter.Instance.Clear();
            string content = CSVLanguage.Instance.GetConfData(2009553).words;
            PromptBoxParameter.Instance.content = content;
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                UnloadWearParts();

                ResetDressData();
                SwitchToDressClothes(true);
                //curMainPartShowId = GetDressMainPartShowId();
                //RevertModelShow();
                _LoadShowModel();
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private void UpdateFashionPoint()
        {
            TextHelper.SetText(m_FashionPoint, Sys_Fashion.Instance.fashionPoint.ToString());
            OnRefreshFashionPointRed();
        }

        private void OnFashionPointButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Fashion_Point);
        }


        private void OnLuckyDrawButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Fashion_LuckyDraw);
        }

        private void OnRefreshFreeDrawState()
        {
            m_LuckyDrawRedPoint.SetActive(Sys_Fashion.Instance.freeDraw);
        }

        private void OnRefreshLuckyDrawActiveState()
        {
            m_LuckyDrawButton.gameObject.SetActive(Sys_Fashion.Instance.activeId > 0 && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(205));
        }

        private void OnRefreshFashionPointRed()
        {
            m_FashionPointRed.SetActive(Sys_Fashion.Instance.HasReward());
        }


        #region SuitGrid

        private void ClearSuitGridButtons()
        {
            foreach (var item in suitGridButtons)
            {
                item.onClick.RemoveAllListeners();
            }
            suitGridButtons.Clear();
        }

        private void SetSuitGrid(int totle, List<uint> owend)
        {
            int owed = owend.Count;
            ClearSuitGridButtons();
            for (int i = 0, count = suitGridRoot.childCount; i < count; i++)
            {
                if (i < totle)
                {
                    uint fashionId = curFashionSuit.associated[i];
                    uint iconId = 0;
                    uint itemId = 0;

                    GameObject child = suitGridRoot.GetChild(i).gameObject;
                    GameObject no = child.transform.Find("Btn_None").gameObject;
                    Button button = child.transform.Find("Btn_Item").GetComponent<Button>();

                    if (!child.activeSelf)
                        child.SetActive(true);

                    no.SetActive(!owend.Contains(fashionId));

                    Image image = child.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();

                    FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == fashionId);
                    if (fashionClothes != null)
                    {
                        iconId = fashionClothes.Id * 10000 + Sys_Role.Instance.HeroId;
                        ImageHelper.SetIcon(image, iconId);
                        itemId = fashionClothes.cSVFashionClothesData.FashionItem[0];
                    }
                    FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == fashionId);
                    if (fashionWeapon != null)
                    {
                        iconId = fashionWeapon.Id * 1000 + (uint)GameCenter.mainHero.careerComponent.CurCarrerType;
                        ImageHelper.SetIcon(image, iconId);
                        itemId = fashionWeapon.cSVFashionWeaponData.FashionItem[0];
                    }
                    FashionAccessory fashionAccessory = Sys_Fashion.Instance._FashionAccessories.Find(x => x.Id == fashionId);
                    if (fashionAccessory != null)
                    {
                        iconId = fashionAccessory.cSVFashionAccessoryData.AccIcon;
                        ImageHelper.SetIcon(image, iconId);
                        itemId = fashionAccessory.cSVFashionAccessoryData.AccItem[0];
                    }
                    button.onClick.AddListener(() =>
                    {
                        UI_Fashion_Tips.FashionTipsData fashionTipsData = new UI_Fashion_Tips.FashionTipsData()
                        {
                            fahsionId = fashionId,
                            iconId = iconId,
                            itemId = itemId,
                        };
                        UIManager.OpenUI(EUIID.UI_Fashion_Tips, false, fashionTipsData);
                    });
                    suitGridButtons.Add(button);
                }
                else
                {
                    suitGridRoot.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        private void OnSetSuitGrid()
        {
            if (curFashionSuit != null)
            {
                SetSuitGrid(curFashionSuit.associated.Count, curFashionSuit.unlockedAssoiated);
            }
        }

        #endregion

        #region RefreshCallback
        private void OnUpdateClothesLockState(uint id)
        {
            if (curFashionType == 1 && curFashionClothes.Id == id)
            {
                SetTitle();
                UpdateUnLockButton();
            }
            FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionClothes.IndexOf(fashionClothes);
            foreach (var item in m_FashionCeilGrid_Clothes)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateLockState();
                    item.Value.Refresh();
                }
            }
        }

        private void OnUpdateWeaponLockState(uint id)
        {
            if (curFashionType == 2 && curFashionWeapon.Id == id)
            {
                SetTitle();
                UpdateUnLockButton();
            }
            FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionWeapons.IndexOf(fashionWeapon);
            foreach (var item in m_FashionCeilGrid_Weapon)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateLockState();
                    item.Value.Refresh();
                }
            }
        }

        private void OnUpdateSuitLockState(uint id)
        {
            if (curFashionType == 0 && curFashionSuit.Id == id)
            {
                UpdateUnLockButton();
            }
            FashionSuit fashionSuit = Sys_Fashion.Instance._FashionSuits.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionSuits.IndexOf(fashionSuit);
            foreach (var item in m_FashionCeilGrid_Suits)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateLockState();
                    item.Value.Refresh();
                }
            }
        }

        private void OnUpdateAcceLockState(uint id)
        {
            if (curFashionType == 3 && curFashionAccessory.Id == id)
            {
                SetTitle();
                UpdateUnLockButton();
            }
            foreach (var item in fashionCeilGrid_Accessorys)
            {
                if (item.Value.mFashionAccessory != null && item.Value.mFashionAccessory.Id == id)
                {
                    item.Value.UpdateLockState();
                    item.Value.Refresh();
                }
            }
        }

        private void OnUpdateClothesDressState(uint id)
        {
            if (curFashionType == 1 && curFashionClothes.Id == id)
            {
                UpdateUnLockButton();
            }
            FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionClothes.IndexOf(fashionClothes);
            foreach (var item in m_FashionCeilGrid_Clothes)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateDressState();
                }
            }
        }

        private void OnUpdateWeaponDressState(uint id)
        {
            if (curFashionType == 2 && curFashionWeapon.Id == id)
            {
                UpdateUnLockButton();
            }
            FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionWeapons.IndexOf(fashionWeapon);
            foreach (var item in m_FashionCeilGrid_Weapon)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateDressState();
                }
            }
        }

        private void OnUpdateAcceDressState(uint id)
        {
            if (curFashionType == 3 && curFashionAccessory.Id == id)
            {
                UpdateUnLockButton();
            }
            foreach (var item in fashionCeilGrid_Accessorys)
            {
                if (item.Value.mFashionAccessory != null && item.Value.mFashionAccessory.Id == id)
                {
                    item.Value.UpdateDressState();
                }
            }
        }

        private void OnUpdateSuitDressState(uint id)
        {
            if (curFashionType == 0 && curFashionSuit.Id == id)
            {
                UpdateUnLockButton();
            }
            FashionSuit fashionSuit = Sys_Fashion.Instance._FashionSuits.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionSuits.IndexOf(fashionSuit);
            foreach (var item in m_FashionCeilGrid_Suits)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateDressState();
                }
            }
        }

        private void OnUpdateSuitAsso(uint id)
        {
            FashionSuit fashionSuit = Sys_Fashion.Instance._FashionSuits.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionSuits.IndexOf(fashionSuit);
            foreach (var item in m_FashionCeilGrid_Suits)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateAssoiated();
                }
            }
        }

        private void UpdateTime()
        {
            if (curFashionType == 1 && curFashionClothes.fashionTimer.bNeedUpdateTime)
            {
                TextHelper.SetText(limitType, curFashionClothes.fashionTimer.GetRemainTimeFormat());
            }
            if (curFashionType == 2 && curFashionWeapon.fashionTimer.bNeedUpdateTime)
            {
                TextHelper.SetText(limitType, curFashionWeapon.fashionTimer.GetRemainTimeFormat());
            }
            if (curFashionType == 3 && curFashionAccessory.fashionTimer.bNeedUpdateTime)
            {
                TextHelper.SetText(limitType, curFashionAccessory.fashionTimer.GetRemainTimeFormat());
            }
        }

        private void OnUpdateClothesLimitTime(uint id)
        {
            FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionClothes.IndexOf(fashionClothes);
            foreach (var item in m_FashionCeilGrid_Clothes)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateTime();
                }
            }
        }

        private void OnUpdateWeaponLimitTime(uint id)
        {
            FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == id);
            int index = Sys_Fashion.Instance._FashionWeapons.IndexOf(fashionWeapon);
            foreach (var item in m_FashionCeilGrid_Weapon)
            {
                if (item.Value.dataIndex == index)
                {
                    item.Value.UpdateTime();
                }
            }
        }

        private void OnUpdateAcceLimitTime(uint id)
        {
            foreach (var item in fashionCeilGrid_Accessorys)
            {
                if (item.Value.mFashionAccessory != null && item.Value.mFashionAccessory.Id == id)
                {
                    item.Value.UpdateTime();
                }
            }
        }

        private void RefreshWeaponIcon()
        {
            foreach (var item in m_FashionCeilGrid_Weapon)
            {
                item.Value.Refresh();
            }
        }

        private void RefreshClothIcon()
        {
            foreach (var item in m_FashionCeilGrid_Clothes)
            {
                item.Value.Refresh();
            }
        }

        private void RefreshSuitIcon()
        {
            foreach (var item in m_FashionCeilGrid_Suits)
            {
                item.Value.Refresh();
            }
        }

        #endregion

        private bool IsClickLimitTime()
        {
            if (Time.realtimeSinceStartup > _clickLimitTime)
            {
                _clickLimitTime = Time.realtimeSinceStartup + 0.3f;
                return false;
            }

            return true;
        }

        #region ***************************************  Attr  ******************************************

        private void SetAttr(uint attr1, uint value1, uint attr2, uint value2, Text attrName1, Text attrValue1, Text attrName2, Text attrValue2)
        {
            if (attrName1 != null)
            {
                CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr1);
                TextHelper.SetText(attrName1, cSVAttrData.name);
                if (cSVAttrData.show_type == 1)
                {
                    attrValue1.text = string.Format("+{0}", value1);
                }
                else
                {
                    attrValue1.text = string.Format("+{0}%", value1 / 100f);
                }
            }
            if (attrName2 != null)
            {

                CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr2);
                TextHelper.SetText(attrName2, cSVAttrData.name);
                if (cSVAttrData.show_type == 1)
                {
                    attrValue2.text = string.Format("+{0}", value2);
                }
                else
                {
                    attrValue2.text = string.Format("+{0}%", value2 / 100f);
                }
            }
        }

        //private void UpdateSuitChange(bool show)
        //{
        //    suitChange.SetActive(show);
        //    if (show)
        //    {
        //        uint curUseSuitAttrid = CSVFashionSuit.Instance.GetConfData(Sys_Fashion.Instance.curUseSuit).attr_id;
        //        CSVFashionAttr.Data cSVFashionAttrData = CSVFashionAttr.Instance.GetConfData(curUseSuitAttrid);
        //        int needCount = cSVFashionAttrData.attr_id.Count;
        //        FrameworkTool.CreateChildList(suitChangeParent, needCount);
        //        if (cSVFashionAttrData != null)
        //        {
        //            for (int i = 0; i < needCount; i++)
        //            {
        //                Transform child = suitChangeParent.GetChild(i);
        //                Text attrName = child.Find("Text").GetComponent<Text>();
        //                Text attrValue = child.Find("Text/Text_Num").GetComponent<Text>();
        //                uint suitAttr1 = cSVFashionAttrData.attr_id[i][0];
        //                uint suitValue1 = cSVFashionAttrData.attr_id[i][1];
        //                SetAttr(suitAttr1, suitValue1, 0, 0, attrName, attrValue, null, null);
        //            }
        //        }
        //        else
        //        {
        //            DebugUtil.LogErrorFormat("当前没有使用中的套装属性 attrid={0}", curUseSuitAttrid);
        //        }
        //        suitChangeButton.gameObject.SetActive(Sys_Fashion.Instance._UnLockedSuits.Count > 1);
        //    }
        //}

        //private void SetSuitAttrDiscribe(bool show)
        //{
        //    if (!show) { suitAttrRoot.SetActive(false); return; }
        //    suitAttrRoot.SetActive(true);
        //    int total = curFashionSuit.associated.Count;
        //    int owned = curFashionSuit.unlockedAssoiated.Count;
        //    int childCount = suitAttrParent.transform.childCount;
        //    if (childCount - 1 >= total)
        //    {
        //        for (int i = total; i < childCount - 1; i++)
        //        {
        //            suitAttrParent.transform.GetChild(i).gameObject.SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        for (int i = childCount - 1; i < total; i++)
        //        {
        //            GameObject gameObject = GameObject.Instantiate<GameObject>(suitAttrChild_fashion, suitAttrParent.transform);
        //            gameObject.transform.SetSiblingIndex(suitAttrParent.transform.childCount - 2);
        //        }
        //    }
        //    LayoutRebuilder.ForceRebuildLayoutImmediate(suitAttrParent.transform as RectTransform);
        //    for (int i = 0; i < total; i++)
        //    {
        //        GameObject go = suitAttrParent.transform.GetChild(i).gameObject;
        //        Color color;
        //        if (i < owned)
        //            color = Color.green;
        //        else
        //            color = unlockColor;

        //        if (!go.activeSelf)
        //            go.SetActive(true);

        //        uint fashionId;
        //        EHeroModelParts eHeroModelParts;

        //        if (i < owned)
        //        {
        //            fashionId = curFashionSuit.unlockedAssoiated[i];
        //            eHeroModelParts = Sys_Fashion.Instance.parts[fashionId];
        //        }
        //        else
        //        {
        //            List<uint> LockedAssociated = curFashionSuit.GetLockedAssociated();
        //            fashionId = LockedAssociated[i - owned];
        //            eHeroModelParts = Sys_Fashion.Instance.parts[fashionId];
        //        }

        //        Text _fashionName = go.transform.Find("Image_Title_Splitline/Text_Parts").GetComponent<Text>();
        //        switch (eHeroModelParts)
        //        {
        //            case EHeroModelParts.Main:
        //                FashionClothes fashionClothes = Sys_Fashion.Instance._FashionClothes.Find(x => x.Id == fashionId);
        //                TextHelper.SetText(_fashionName, fashionClothes.cSVFashionClothesData.FashionName);
        //                if (fashionClothes.cSVFashionClothesData.attr_id == null)
        //                {
        //                    go.SetActive(false);
        //                }
        //                else
        //                {
        //                    int attrCount = fashionClothes.cSVFashionClothesData.attr_id.Count;
        //                    FrameworkTool.CreateChildList(go.transform, attrCount, 1);
        //                    for (int j = 1; j < attrCount + 1; j++)
        //                    {
        //                        GameObject child = go.transform.GetChild(j).gameObject;
        //                        child.SetActive(true);
        //                        uint attrId = fashionClothes.cSVFashionClothesData.attr_id[j - 1][0];
        //                        uint value = fashionClothes.cSVFashionClothesData.attr_id[j - 1][1];
        //                        Text attrName = child.transform.GetComponent<Text>();
        //                        Text attrValue = child.transform.Find("Number01").GetComponent<Text>();
        //                        attrName.color = color;
        //                        attrValue.color = color;
        //                        SetAttr(attrId, value, 0, 0, attrName, attrValue, null, null);
        //                    }
        //                }
        //                break;
        //            case EHeroModelParts.Weapon:
        //                FashionWeapon fashionWeapon = Sys_Fashion.Instance._FashionWeapons.Find(x => x.Id == fashionId);
        //                TextHelper.SetText(_fashionName, fashionWeapon.cSVFashionWeaponData.WeaponName);
        //                if (fashionWeapon.cSVFashionWeaponData.attr_id == null)
        //                {
        //                    go.SetActive(false);
        //                }
        //                else
        //                {
        //                    int _attrCount = fashionWeapon.cSVFashionWeaponData.attr_id.Count;
        //                    FrameworkTool.CreateChildList(go.transform, _attrCount, 1);
        //                    for (int j = 1; j < _attrCount + 1; j++)
        //                    {
        //                        GameObject child = go.transform.GetChild(j).gameObject;
        //                        child.SetActive(true);
        //                        uint attrId = fashionWeapon.cSVFashionWeaponData.attr_id[j - 1][0];
        //                        uint value = fashionWeapon.cSVFashionWeaponData.attr_id[j - 1][1];
        //                        Text attrName = child.transform.GetComponent<Text>();
        //                        Text attrValue = child.transform.Find("Number01").GetComponent<Text>();
        //                        attrName.color = color;
        //                        attrValue.color = color;
        //                        SetAttr(attrId, value, 0, 0, attrName, attrValue, null, null);
        //                    }
        //                }
        //                break;
        //            case EHeroModelParts.Jewelry_Head:
        //            case EHeroModelParts.Jewelry_Back:
        //            case EHeroModelParts.Jewelry_Waist:
        //            case EHeroModelParts.Jewelry_Face:
        //                FashionAccessory fashionAccessory = Sys_Fashion.Instance._FashionAccessories.Find(x => x.Id == fashionId);
        //                TextHelper.SetText(_fashionName, fashionAccessory.cSVFashionAccessoryData.AccName);
        //                if (fashionAccessory.cSVFashionAccessoryData.attr_id == null)
        //                {
        //                    go.SetActive(false);
        //                }
        //                else
        //                {
        //                    int __attrCount = fashionAccessory.cSVFashionAccessoryData.attr_id.Count;
        //                    FrameworkTool.CreateChildList(go.transform, __attrCount, 1);
        //                    for (int j = 1; j < __attrCount + 1; j++)
        //                    {
        //                        GameObject child = go.transform.GetChild(j).gameObject;
        //                        child.SetActive(true);
        //                        uint attrId = fashionAccessory.cSVFashionAccessoryData.attr_id[j - 1][0];
        //                        uint value = fashionAccessory.cSVFashionAccessoryData.attr_id[j - 1][1];
        //                        Text attrName = child.transform.GetComponent<Text>();
        //                        Text attrValue = child.transform.Find("Number01").GetComponent<Text>();
        //                        attrName.color = color;
        //                        attrValue.color = color;
        //                        SetAttr(attrId, value, 0, 0, attrName, attrValue, null, null);
        //                    }
        //                }
        //                break;
        //        }
        //        LayoutRebuilder.ForceRebuildLayoutImmediate(suitAttrParent.transform as RectTransform);
        //    }


        //    Color suitColor;
        //    if (curFashionSuit.UnLock)
        //    {
        //        suitColor = Color.green;
        //    }
        //    else
        //    {
        //        suitColor = unlocksuitColor;
        //    }
        //    CSVFashionAttr.Data cSVFashionAttrData = CSVFashionAttr.Instance.GetConfData(curFashionSuit.cSVFashionSuitData.attr_id);
        //    GameObject suitGo = suitAttrParent.transform.GetChild(suitAttrParent.transform.childCount - 1).gameObject;
        //    for (int i = 1; i < suitGo.transform.childCount; i++)
        //    {
        //        suitGo.transform.GetChild(i).gameObject.SetActive(false);
        //    }
        //    int ___attrCount = cSVFashionAttrData.attr_id.Count;
        //    FrameworkTool.CreateChildList(suitGo.transform, ___attrCount, 1);
        //    for (int j = 1; j < ___attrCount + 1; j++)
        //    {
        //        GameObject child = suitGo.transform.GetChild(j).gameObject;
        //        child.SetActive(true);
        //        uint attrId = cSVFashionAttrData.attr_id[j - 1][0];
        //        uint value = cSVFashionAttrData.attr_id[j - 1][1];
        //        Text attrName = child.transform.GetComponent<Text>();
        //        Text attrValue = child.transform.Find("Number01").GetComponent<Text>();
        //        attrName.color = suitColor;
        //        attrValue.color = suitColor;
        //        SetAttr(attrId, value, 0, 0, attrName, attrValue, null, null);
        //    }
        //}

        //private void SetFashionAttr()
        //{
        //    List<List<uint>> attr_id = new List<List<uint>>();
        //    if (curFashionType == 1)
        //    {
        //        attr_id = curFashionClothes.cSVFashionClothesData.attr_id;
        //    }
        //    else if (curFashionType == 2)
        //    {
        //        attr_id = curFashionWeapon.cSVFashionWeaponData.attr_id;
        //    }
        //    else if (curFashionType == 3)
        //    {
        //        attr_id = curFashionAccessory.cSVFashionAccessoryData.attr_id;
        //    }

        //    int count = 0;
        //    if (attr_id == null)
        //        count = 0;
        //    else
        //        count = attr_id.Count;

        //    empty1.SetActive(count == 0);

        //    for (int i = 0; i < fashionAttrRoot.childCount; i++)
        //    {
        //        GameObject go = fashionAttrRoot.GetChild(i).gameObject;
        //        if (i < count)
        //        {
        //            if (!go.activeSelf)
        //            {
        //                go.SetActive(true);
        //            }
        //            SetAttr(attr_id[i][0], attr_id[i][1], 0, 0, go.transform.Find("Text").GetComponent<Text>(), go.transform.Find("Text_Num").GetComponent<Text>(), null, null);
        //        }
        //        else
        //        {
        //            go.SetActive(false);
        //        }
        //    }
        //}

        //private void SetSuitAttr()
        //{
        //    CSVFashionAttr.Data cSVFashionAttrData = CSVFashionAttr.Instance.GetConfData(curFashionSuit.cSVFashionSuitData.attr_id);
        //    if (cSVFashionAttrData == null)
        //    {
        //        return;
        //    }
        //    if (cSVFashionAttrData.attr_id != null)
        //    {
        //        int needCount = cSVFashionAttrData.attr_id.Count;
        //        FrameworkTool.CreateChildList(suitAttrParent2, needCount);
        //        for (int i = 0; i < needCount; i++)
        //        {
        //            Transform child = suitAttrParent2.GetChild(i);
        //            Text attrName = child.Find("Text").GetComponent<Text>();
        //            Text attrValue = child.Find("Text/Text_Num").GetComponent<Text>();
        //            uint suitAttr1 = cSVFashionAttrData.attr_id[i][0];
        //            uint suitValue1 = cSVFashionAttrData.attr_id[i][1];
        //            SetAttr(suitAttr1, suitValue1, 0, 0, attrName, attrValue, null, null);
        //        }
        //    }
        //}
        #endregion
    }
}

