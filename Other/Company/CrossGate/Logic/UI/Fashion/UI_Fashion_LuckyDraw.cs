using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public class UI_Fashion_LuckyDraw : UIBase
    {
        private Text m_ActivityName;

        private Image m_DiamondsIcon;
        private Text m_DiamondsCount;
        private Button m_DiamondsAddButton;
        private Image m_FashionCoinIcon;
        private Text m_FashionCoinCount;
        private Button m_FashionCoinAddButton;

        private Button m_ButtonDetail;
        private Text m_ValidTime;
        private Text m_RemainingTime;
        private Button m_CloseButton;

        private Button m_LuckyDraw_1;
        private Button m_LuckyDraw_2;
        private Text m_LuckyDrawCount_1;
        private Text m_LuckyDrawCount_2;
        private Image m_LuckyDrawIcon_1;
        private Image m_LuckyDrawIcon_2;
        private Text m_NextFreeDrawRemainingTime;
        private GameObject m_TextFree;

        private Button m_ExchangeShopButton;
        private Toggle m_AutoBuy;

        private GameObject m_CustomGo;  //定制
        private GameObject m_ActionGo;  //专属动作
        private GameObject m_FreeDyeGo; //自由

        private Text m_TotalFashionPointText;
        private Image m_FashionPointIcon;
        private Button m_FashionPointTipsButton;

        private Image m_EventImage;

        private uint m_ActivityId;
        private List<uint> m_ShowFashionIds = new List<uint>();

        private ShowSceneControl showSceneControl;
        private HeroDisplayControl heroDisplay;
        private AssetDependencies assetDependencies;

        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;
        private uint _curUpdateAnimationFlagId;
        private uint _curWeaponFashionModelId;

        private CSVFashionActivity.Data m_CSVFashionActivityData;
        private CSVItem.Data m_CSVItemData_FashionCoin;

        private DateTime startTime;
        private DateTime endTime;
        private bool m_IsSameDay5;
        private bool isSameDay5
        {
            get
            {
                return m_IsSameDay5;
            }
            set
            {
                if (m_IsSameDay5 != value)
                {
                    m_IsSameDay5 = value;
                    UpdateButtonText();
                }
            }
        }

        private bool b_FreeFlag;
        private Text m_LuckDraw1Text;

        private GameObject m_TipsFashionCoinRoot;
        private Text m_TipsFashionCoin;
        private GameObject m_TipsFashionPointRoot;
        //private GameObject m_ViewRuleRoot;
        private Text m_TipsFashionPoint;
        private Image m_TipsFashionCoinClickClose;
        private Image m_TipsFashionPointClickClose;
        private GameObject m_FreeRedPoint;

        private uint m_ToSilverCoinRate;
        
        private GameObject m_SceneModel;
        private Transform m_TimeLineParent;
        private Transform m_LuckyDrawParent;
        private Transform m_CurChild;
        private PlayableDirector m_Draw;
        private Transform m_AnimatorRoot;

        private uint m_DrawTime;

        protected override void OnInit()
        {
            m_CSVFashionActivityData = CSVFashionActivity.Instance.GetConfData(Sys_Fashion.Instance.activeId);
            for (int i = 0; i < m_CSVFashionActivityData.Show.Count; i++)
            {
                if (i == (int)EHeroModelParts.Weapon)
                {
                    if (Sys_Equip.Instance.GetCurWeapon() == Constants.UMARMEDID)
                    {
                        m_ShowFashionIds.Add(0);
                    }
                    else
                    {
                        m_ShowFashionIds.Add(m_CSVFashionActivityData.Show[i]);
                    }
                }
                else
                {
                    m_ShowFashionIds.Add(m_CSVFashionActivityData.Show[i]);
                }
            }
            m_CSVItemData_FashionCoin = CSVItem.Instance.GetConfData(m_CSVFashionActivityData.FashionCoin);
            m_ToSilverCoinRate = uint.Parse(CSVParam.Instance.GetConfData(1298).str_value);
        }

        protected override void OnLoaded()
        {
            assetDependencies = transform.GetComponent<AssetDependencies>();
            m_AnimatorRoot = transform.Find("Animator");
            m_ActivityName = transform.Find("Animator/View_Left/Image_NameBg/Text_1").GetComponent<Text>();
            m_CloseButton = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            m_ButtonDetail = transform.Find("Animator/View_Right/Text_Details/Btn_Details").GetComponent<Button>();
            m_LuckyDraw_1 = transform.Find("Animator/View_Right/Btn_LuckyDraw_1").GetComponent<Button>();
            m_LuckyDraw_2 = transform.Find("Animator/View_Right/Btn_LuckyDraw_10").GetComponent<Button>();
            m_LuckyDrawCount_1 = transform.Find("Animator/View_Right/Cost_1/Text_Value").GetComponent<Text>();
            m_LuckyDrawCount_2 = transform.Find("Animator/View_Right/Cost_10/Text_Value").GetComponent<Text>();
            m_RemainingTime = transform.Find("Animator/View_Left/TimeRemaining/Text_Value").GetComponent<Text>();
            m_LuckDraw1Text = transform.Find("Animator/View_Right/Btn_LuckyDraw_1/Text_01").GetComponent<Text>();
            m_NextFreeDrawRemainingTime = transform.Find("Animator/View_Right/TimeRemaining").GetComponent<Text>();
            m_TextFree = transform.Find("Animator/View_Right/Text_Free").gameObject;
            m_LuckyDrawIcon_1 = transform.Find("Animator/View_Right/Cost_1/Image_Icon").GetComponent<Image>();
            m_LuckyDrawIcon_2 = transform.Find("Animator/View_Right/Cost_10/Image_Icon").GetComponent<Image>();
            m_ExchangeShopButton = transform.Find("Animator/View_Left/Btn_Mall").GetComponent<Button>();
            m_FashionPointTipsButton = transform.Find("Animator/View_Left/Point/bg/Btn_Details").GetComponent<Button>();
            m_AutoBuy = transform.Find("Animator/View_Right/Toggle_AutoBuy").GetComponent<Toggle>();
            m_TotalFashionPointText = transform.Find("Animator/View_Left/Point/bg/Text_Value").GetComponent<Text>();
            m_FashionPointIcon = transform.Find("Animator/View_Left/Point/bg/Image_Icon").GetComponent<Image>();
            m_CustomGo = transform.Find("Animator/View_Left/Feature/Item_3").gameObject;
            m_ActionGo = transform.Find("Animator/View_Left/Feature/Item_2").gameObject;
            m_FreeDyeGo = transform.Find("Animator/View_Left/Feature/Item_1").gameObject;
            m_FreeRedPoint = transform.Find("Animator/View_Right/Btn_LuckyDraw_1/Image_Dot").gameObject;
            m_DiamondsIcon = transform.Find("Animator/UI_Property/Image_Property (1)/Image_Icon").GetComponent<Image>();
            m_DiamondsAddButton = transform.Find("Animator/UI_Property/Image_Property (1)/Button_Add").GetComponent<Button>();
            m_DiamondsCount = transform.Find("Animator/UI_Property/Image_Property (1)/Text_Number").GetComponent<Text>();
            m_FashionCoinIcon = transform.Find("Animator/UI_Property/Image_Property/Image_Icon").GetComponent<Image>();
            m_FashionCoinAddButton = transform.Find("Animator/UI_Property/Image_Property/Button_Add").GetComponent<Button>();
            m_FashionCoinCount = transform.Find("Animator/UI_Property/Image_Property/Text_Number").GetComponent<Text>();
            m_TipsFashionCoinRoot = transform.Find("Animator/LuckyDraw_Tips1").gameObject;
            m_TipsFashionPointRoot = transform.Find("Animator/LuckyDraw_Tips2").gameObject;
            m_TipsFashionCoinClickClose = transform.Find("Animator/LuckyDraw_Tips1/Close").GetComponent<Image>();
            m_TipsFashionPointClickClose = transform.Find("Animator/LuckyDraw_Tips2/Close").GetComponent<Image>();
            //m_ViewRuleRoot = transform.Find("Animator/LuckyDraw_Tips2/View_Rule").gameObject;
            m_TipsFashionCoin = transform.Find("Animator/LuckyDraw_Tips1/View_Rule/Text_Tips").GetComponent<Text>();
            m_TipsFashionPoint = transform.Find("Animator/LuckyDraw_Tips2/View_Rule/Text_Tips").GetComponent<Text>();
            m_ValidTime = transform.Find("Animator/View_Right/Text_Time/Text_Value").GetComponent<Text>();
            m_EventImage = transform.Find("Animator/EventImage").GetComponent<Image>();

            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_ButtonDetail.onClick.AddListener(OnButtonDetailClicked);
            m_LuckyDraw_1.onClick.AddListener(OnLuckyDraw_1ButtonClicked);
            m_LuckyDraw_2.onClick.AddListener(OnLuckDraw_2ButtonClicked);
            m_ExchangeShopButton.onClick.AddListener(OnExchangeShopButtonClicked);
            m_FashionPointTipsButton.onClick.AddListener(OnFashionPointTipsButtonClicked);
            m_DiamondsAddButton.onClick.AddListener(OnDiamondsAddButtonClicked);
            m_FashionCoinAddButton.onClick.AddListener(OnFashionCoinButtonClicked);

            m_AutoBuy.onValueChanged.AddListener(OnToggleChanged);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_EventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnModleDrag);

            Lib.Core.EventTrigger eventTrigger = Lib.Core.EventTrigger.Get(m_TipsFashionCoinClickClose);
            eventTrigger.AddEventListener(EventTriggerType.PointerClick, OnFashionCoinClick);

            Lib.Core.EventTrigger eventTrigger1 = Lib.Core.EventTrigger.Get(m_TipsFashionPointClickClose);
            eventTrigger1.AddEventListener(EventTriggerType.PointerClick, OnFashionPointClick);
            
            // _LoadShowScene();
            // _LoadShowModel();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnItemCountChanged, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnDrawLucky, OnDrawLucky, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle(Sys_Fashion.EEvents.OnExchangeDraw, OnExchangeDraw, toRegister);
            Sys_Fashion.Instance.eventEmitter.Handle<uint>(Sys_Fashion.EEvents.OnStartLuckyDrawFromRes, PlayFx, toRegister);
        }

        protected override void OnShow()
        {
            UpdateInfo();
            //ResetAnimation();

            _LoadShowScene();
            _LoadShowModel();
            
            if (!Sys_Fashion.Instance.fashionFreeRedInfo.played)
            {
                Sys_Fashion.Instance.fashionFreeRedInfo.played = true;
                Sys_Fashion.Instance.SaveMemory();
            }
            
            if (Sys_Fashion.Instance.drawFromRes)
            {
                PlayFx(Sys_Fashion.Instance.lastDrawTimes);
                Sys_Fashion.Instance.drawFromRes = false;
            }
        }

        protected override void OnHide()
        {
            m_AnimatorRoot.gameObject.SetActive(true);
            _UnloadShowContent();
        }

        protected override void OnClose()
        {
            //_UnloadShowContent();
        }
        
        protected override void OnUpdate()
        {
            UpdateButtonState();
            UpdateRemainingTime();
        }

        #region ModelShow
        private void _UnloadShowContent()
        {
            if (_uiModelShowManagerEntity != null)
            {
                _uiModelShowManagerEntity.Dispose();
                _uiModelShowManagerEntity = null;
            }

            //rawImage.texture = null;
            //heroDisplay.Dispose();
            HeroDisplayControl.Destory(ref heroDisplay);
            showSceneControl.Dispose();
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }
            m_SceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            m_SceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(m_SceneModel);

            UpdateTimeLine();

            if (heroDisplay == null)
            {
                heroDisplay = HeroDisplayControl.Create(true);
                heroDisplay.onLoaded = OnShowModelLoaded;
                heroDisplay.eLayerMask = ELayerMask.ModelShow;
            }
            showSceneControl.mModelPos.transform.localPosition = new Vector3(m_CSVFashionActivityData.Pos[0] / 100f, m_CSVFashionActivityData.Pos[1] / 100f, m_CSVFashionActivityData.Pos[2] / 100f);
        }

        private void _LoadShowModel()
        {
            uint mainPartId = m_ShowFashionIds[(int)EHeroModelParts.Main];
            if (mainPartId != 0)
            {
                LoadModelParts(mainPartId, EHeroModelParts.Main);
            }

            uint weaponPartId = m_ShowFashionIds[(int)EHeroModelParts.Weapon];
            if (weaponPartId != 0)
            {
                LoadModelParts(weaponPartId, EHeroModelParts.Weapon);
            }

            uint Jewelry_WaistPartId = m_ShowFashionIds[(int)EHeroModelParts.Jewelry_Waist];
            if (Jewelry_WaistPartId != 0)
            {
                LoadModelParts(Jewelry_WaistPartId, EHeroModelParts.Jewelry_Waist);
            }

            uint Jewelry_FacePartId = m_ShowFashionIds[(int)EHeroModelParts.Jewelry_Face];
            if (Jewelry_FacePartId != 0)
            {

                LoadModelParts(Jewelry_FacePartId, EHeroModelParts.Jewelry_Face);
            }
        }

        private void LoadModelParts(uint id, EHeroModelParts eHeroModelParts)
        {
            if (eHeroModelParts == EHeroModelParts.Main)
            {
                uint clothesfashionModelId = id * 10000 + Sys_Role.Instance.HeroId;
                CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(clothesfashionModelId);
                if (cSVFashionModelData != null)
                {
                    string modelPath = cSVFashionModelData.model_show;
                    heroDisplay.LoadMainModel(EHeroModelParts.Main, modelPath, EHeroModelParts.None, null);
                    heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
                }
            }
            if (eHeroModelParts == EHeroModelParts.Weapon)
            {
                var csvFashtionWeaponData = Sys_Fashion.Instance.GetWeaponModelData(id, Sys_Equip.Instance.GetCurWeapon());
                if (csvFashtionWeaponData != null)
                {
                    heroDisplay.LoadWeaponModel(csvFashtionWeaponData, true);
                }
            }
            if (eHeroModelParts == EHeroModelParts.Jewelry_Back || eHeroModelParts == EHeroModelParts.Jewelry_Face
                || eHeroModelParts == EHeroModelParts.Jewelry_Head || eHeroModelParts == EHeroModelParts.Jewelry_Waist)
            {
                CSVFashionAccessory.Data cSVFashionAccessoryData = CSVFashionAccessory.Instance.GetConfData(id);
                if (cSVFashionAccessoryData != null)
                {
                    string modelPath = cSVFashionAccessoryData.model_show;
                    string socketName = cSVFashionAccessoryData.AccSlot;
                    heroDisplay.LoadMainModel(eHeroModelParts, modelPath, EHeroModelParts.Main, socketName);
                }
            }
        }

        private void OnShowModelLoaded(int obj)
        {
            EHeroModelParts eHeroModelParts = (EHeroModelParts)obj;
            uint fashionId = m_ShowFashionIds[obj];

            UIUpdateAnimation(eHeroModelParts, fashionId);
        }

        protected void UIUpdateAnimation(EHeroModelParts eHeroModelParts, uint id)
        {
            //DebugUtil.Log(ELogType.eBag, $"<Color=yellow>_processWeaponType  :{_processWeaponType}  eHeroModelParts:  {eHeroModelParts}</Color>");

            VirtualGameObject virtualGameObject = heroDisplay.GetPart(0);
            if (virtualGameObject != null)
            {
                GameObject go = virtualGameObject.gameObject;
                if (go != null)
                {
                    uint _charId;
                    if (eHeroModelParts == EHeroModelParts.Main)
                    {
                        _charId = id * 10000u + Sys_Role.Instance.HeroId;
                    }
                    else
                    {
                        uint mainId = m_ShowFashionIds[(int)EHeroModelParts.Main];
                        if (mainId > 0)
                        {
                            _charId = mainId * 10000u + Sys_Role.Instance.HeroId;
                        }
                        else
                        {
                            _charId = 0;
                        }
                    }

                    if (_charId > 0u)
                    {
                        go.SetActive(false);

                        CSVFashionModel.Data cSVFashionModelData = CSVFashionModel.Instance.GetConfData(_charId);
                        if (cSVFashionModelData != null)
                        {
                            uint isUpdateAnimtionsFlagId = cSVFashionModelData.action_show_id * 10u + cSVFashionModelData.show_label;
                            _charId = cSVFashionModelData.action_show_id;
                            uint weaponfashionModeId = 0u;
                            uint equidId;
                            uint fashionWeaponId = m_ShowFashionIds[(int)EHeroModelParts.Weapon];

                            if (fashionWeaponId > 0)
                            {
                                CSVEquipment.Data cSVEquipmentData = CSVEquipment.Instance.GetConfData(Sys_Equip.Instance.GetCurWeapon());
                                weaponfashionModeId = fashionWeaponId * 10 + cSVEquipmentData.equipment_type;
                                equidId = CSVFashionWeaponModel.Instance.GetConfData(weaponfashionModeId).equip_id;

                                if (_uiModelShowManagerEntity != null)
                                    _uiModelShowManagerEntity.DoClearData(_curWeaponFashionModelId != weaponfashionModeId);

                                DebugUtil.Log(ELogType.eUIModelShowWorkStream, $"<Color=yellow>更新动画:_curUpdateAnimationFlagId:{_curUpdateAnimationFlagId.ToString()}  _charId:{_charId.ToString()}    _curWeaponFashionModelId:{_curWeaponFashionModelId.ToString()}  weaponfashionModeId:{weaponfashionModeId.ToString()}</Color>");
                                heroDisplay.mAnimation.UpdateHoldingAnimations(_charId, equidId, Constants.UIModelShowAnimationClipHashSet, EStateType.Idle, go);
                            }
                            else
                            {
                                equidId = Constants.UMARMEDID;
                                weaponfashionModeId = equidId;

                                if (_uiModelShowManagerEntity != null)
                                    _uiModelShowManagerEntity.DoClearData(_curWeaponFashionModelId != weaponfashionModeId);

                                DebugUtil.Log(ELogType.eUIModelShowWorkStream, $"<Color=yellow>更新动画:_curUpdateAnimationFlagId:{_curUpdateAnimationFlagId.ToString()}  _charId:{_charId.ToString()}    _curWeaponFashionModelId:{_curWeaponFashionModelId.ToString()}  weaponfashionModeId:{weaponfashionModeId.ToString()}</Color>");
                                heroDisplay.mAnimation.UpdateHoldingAnimations(_charId, Constants.UMARMEDID, Constants.UIModelShowAnimationClipHashSet, EStateType.Idle, go);
                            }

                            _curUpdateAnimationFlagId = isUpdateAnimtionsFlagId;
                            _curWeaponFashionModelId = weaponfashionModeId;

                            GameObject weaponGo = null;
                            VirtualGameObject weaponVGO = heroDisplay.GetPart(1);
                            if (weaponVGO != null)
                                weaponGo = weaponVGO.gameObject;

                            VirtualGameObject weapon02VGO = heroDisplay.GetOtherWeapon();

                            SetPosEulerAngles(0f);
                            if (_uiModelShowManagerEntity == null)
                                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(cSVFashionModelData.ui_show_workID, null, heroDisplay.mAnimation, go, equidId, weaponGo, weapon02VGO);
                            else
                                _uiModelShowManagerEntity = _uiModelShowManagerEntity.StartWork(cSVFashionModelData.ui_show_workID, null, heroDisplay.mAnimation, go, equidId, weaponGo, weapon02VGO);
                        }
                        else
                        {
                            DebugUtil.LogErrorFormat($"找不到时装模型配置{_charId}");
                        }
                    }
                }
            }

        }

        public void OnModleDrag(BaseEventData eventData)
        {
            if (_uiModelShowManagerEntity != null &&
                !_uiModelShowManagerEntity.IsCanControlUIOperation(WS_UIModelShowManagerEntity.ControlUIOperationEnum.OnRotateModel))
                return;

            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }

        public void SetPosEulerAngles(float eulerAnglesY)
        {
            Transform posTrans = showSceneControl.mModelPos.transform;
            if (posTrans != null)
            {
                Vector3 localAngle = posTrans.localEulerAngles;
                posTrans.localEulerAngles = new Vector3(localAngle.x, eulerAnglesY, localAngle.z);
            }
        }

        private void UpdateTimeLine()
        {
            m_TimeLineParent = m_SceneModel.transform.Find("Timeline");

            for (int i = 0; i < m_TimeLineParent.childCount; i++)
            {
                Transform child = m_TimeLineParent.GetChild(i);

                bool equals = child.name == m_CSVFashionActivityData.id.ToString();

                child.gameObject.SetActive(equals);

                if (equals)
                {
                    m_CurChild = child;
                }
            }

            m_LuckyDrawParent = m_SceneModel.transform.Find("LuckyDraw");

            for (int i = 0; i < m_LuckyDrawParent.childCount; i++)
            {
                Transform child = m_LuckyDrawParent.GetChild(i);

                bool equals = child.name == m_CSVFashionActivityData.id.ToString();

                child.gameObject.SetActive(equals);
            }

            m_Draw = m_CurChild.Find("Draw").GetComponent<PlayableDirector>();

            m_Draw.stopped -= OnPlayableEnd;
            m_Draw.stopped += OnPlayableEnd;
        }

        #endregion

        private void UpdateInfo()
        {
            //activityName
            TextHelper.SetText(m_ActivityName, m_CSVFashionActivityData.Name);
            //tag
            m_CustomGo.SetActive(m_CSVFashionActivityData.Tag[0] == 1);
            m_FreeDyeGo.SetActive(m_CSVFashionActivityData.Tag[1] == 1);
            m_ActionGo.SetActive(m_CSVFashionActivityData.Tag[2] == 1);

            //fashionPoint
            CSVItem.Data cSVItem_FashionPoint = CSVItem.Instance.GetConfData((uint)ECurrencyType.FashionPoint);
            if (cSVItem_FashionPoint != null)
            {
                ImageHelper.SetIcon(m_FashionPointIcon, cSVItem_FashionPoint.small_icon_id);
                UpdateFashionLuckyPoint();
            }

            //title
            ImageHelper.SetIcon(m_DiamondsIcon, CSVItem.Instance.GetConfData((uint)ECurrencyType.Diamonds).icon_id);
            UpdateDiamondCount();

            if (m_CSVItemData_FashionCoin != null)
            {
                ImageHelper.SetIcon(m_FashionCoinIcon, m_CSVItemData_FashionCoin.icon_id);
                UpdateFashionCoinCount();
            }

            //validTime
            startTime = TimeManager.GetDateTime(Sys_Fashion.Instance.startTime);
            endTime = TimeManager.GetDateTime(Sys_Fashion.Instance.endTime);
            TextHelper.SetText(m_ValidTime, LanguageHelper.GetTextContent(590002117, startTime.Year.ToString(), startTime.Month.ToString(), startTime.Day.ToString(),
                endTime.Year.ToString(), endTime.Month.ToString(), endTime.Day.ToString()));

            //buttonState
            UpdateItemCount();
            //UpdateButtonText();
            ImageHelper.SetIcon(m_LuckyDrawIcon_1, m_CSVItemData_FashionCoin.icon_id);
            ImageHelper.SetIcon(m_LuckyDrawIcon_2, m_CSVItemData_FashionCoin.icon_id);

            //toggleState
            m_AutoBuy.isOn = Sys_Fashion.Instance.autoBuyDraw;
        }

        private void UpdateFashionLuckyPoint()
        {
            long currencyCount = Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.FashionPoint);
            TextHelper.SetText(m_TotalFashionPointText, currencyCount.ToString());
        }

        private void UpdateRemainingTime()
        {
            uint endTime = Sys_Fashion.Instance.endTime;
            uint now = Sys_Time.Instance.GetServerTime();
            if (endTime < now)
            {
                UIManager.CloseUI(EUIID.UI_Fashion_LuckyDraw);
                return;
            }
            if (endTime >= now)
            {
                string time = LanguageHelper.TimeToString(endTime - now, LanguageHelper.TimeFormat.Type_9);
                TextHelper.SetText(m_RemainingTime, time);
            }
        }

        private void UpdateButtonState()
        {
            b_FreeFlag = !m_CSVFashionActivityData.Free;

            if (b_FreeFlag)
            {
                isSameDay5 = Sys_Time.IsServerSameDay5(Sys_Time.Instance.GetServerTime(), Sys_Fashion.Instance.lastFreeDrawTime);
                if (m_IsSameDay5)
                {
                    m_NextFreeDrawRemainingTime.gameObject.SetActive(true);
                    uint nowRelative = Sys_Time.Instance.GetServerTime() - 3600 * 5;
                    uint nowRelative_ZeroStamp = nowRelative - nowRelative % 86400;
                    uint refresh_ZeroStamp = nowRelative_ZeroStamp + 86400;

                    uint remainTime = refresh_ZeroStamp - Sys_Time.Instance.GetServerTime() + 3600 * 5;
                    TextHelper.SetText(m_NextFreeDrawRemainingTime, LanguageHelper.TimeToString(remainTime, LanguageHelper.TimeFormat.Type_10));
                }
                else
                {
                    m_NextFreeDrawRemainingTime.gameObject.SetActive(false);
                }
            }
            else
            {
                isSameDay5 = true;
                m_NextFreeDrawRemainingTime.gameObject.SetActive(false);
            }
        }

        //更新货币栏魔币数量
        private void UpdateDiamondCount()
        {
            long diamondCount = Sys_Bag.Instance.GetItemCount((uint)ECurrencyType.Diamonds);
            TextHelper.SetText(m_DiamondsCount, Sys_Bag.Instance.GetValueFormat(diamondCount));
        }

        private void OnItemCountChanged(int a, int b)
        {
            UpdateFashionCoinCount();
            UpdateDiamondCount();
        }

        //更新货币栏道具数量
        private void UpdateFashionCoinCount()
        {
            long fashionCoinCount = Sys_Bag.Instance.GetItemCount(m_CSVFashionActivityData.FashionCoin);
            TextHelper.SetText(m_FashionCoinCount, fashionCoinCount.ToString());
        }

        //更新抽奖按钮上方道具数量
        private void UpdateItemCount()
        {
            long fashionPointCount = Sys_Bag.Instance.GetItemCount(m_CSVFashionActivityData.FashionCoin);
            if (fashionPointCount < 1)
            {
                TextHelper.SetText(m_LuckyDrawCount_1, 2007202, 1.ToString());
                TextHelper.SetText(m_LuckyDrawCount_2, 2007202, 10.ToString());
            }
            else if (fashionPointCount < 10)
            {
                TextHelper.SetText(m_LuckyDrawCount_1, 2011419, 1.ToString());
                TextHelper.SetText(m_LuckyDrawCount_2, 2007202, 10.ToString());
            }
            else
            {
                TextHelper.SetText(m_LuckyDrawCount_1, 2011419, 1.ToString());
                TextHelper.SetText(m_LuckyDrawCount_2, 2011419, 10.ToString());
            }
            UpdateButtonText();
        }

        private void UpdateButtonText()
        {
            UpdateButtonState();
            if (b_FreeFlag)
            {
                if (m_IsSameDay5)
                {
                    m_LuckyDrawIcon_1.gameObject.SetActive(true);
                    m_TextFree.SetActive(false);
                    m_FreeRedPoint.SetActive(false);
                }
                else
                {
                    m_LuckyDrawIcon_1.gameObject.SetActive(false);
                    TextHelper.SetText(m_LuckyDrawCount_1, string.Empty);
                    m_TextFree.SetActive(true);
                    m_FreeRedPoint.SetActive(true);
                }
            }
            else
            {
                m_LuckyDrawIcon_1.gameObject.SetActive(true);
                m_TextFree.SetActive(false);
                m_FreeRedPoint.SetActive(false);
            }
        }

        private void OnDrawLucky()
        {
            UpdateFashionLuckyPoint();
            UpdateItemCount();
            UpdateFashionCoinCount();
            UpdateDiamondCount();
            UpdateButtonText();
        }

        private void OnExchangeDraw()
        {
            UpdateFashionCoinCount();
            UpdateDiamondCount();
            UpdateItemCount();
        }

        #region LuckyDrawButtonClick
        //抽1次
        private void OnLuckyDraw_1ButtonClicked()
        {
            if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(205))
            {
                string content = LanguageHelper.GetTextContent(2009583);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }

            if (!m_IsSameDay5)
            {
                PlayFx(1);
                //Sys_Fashion.Instance.FashionDrawReq(1);
                return;
            }
            long fashionPointCount = Sys_Bag.Instance.GetItemCount(m_CSVFashionActivityData.FashionCoin);
            if (fashionPointCount >= 1)     //是否抽一次
            {
                //“该操作将消耗{0}个{活动道具}，进行{0}次奖池抽取。”
                string content = LanguageHelper.GetTextContent(590002100, 1.ToString(),
                    LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id), 1.ToString());
                PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                () =>
                {
                    PlayFx(1);
                    //Sys_Fashion.Instance.FashionDrawReq(1);
                });
            }
            else if (fashionPointCount < 1)  //是否用魔币兑换抽奖道具
            {
                uint needDiamondCount = m_CSVFashionActivityData.Value;
                if (!m_AutoBuy.isOn)//没有勾选 就直接弹是否用魔币兑换道具界面
                {
                    //“您的{活动道具}不足，是否使用{0}魔币购买{1}个{活动道具}”
                    string content = LanguageHelper.GetTextContent(590002101, LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                   needDiamondCount.ToString(), 1.ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id));
                    PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                    () =>
                    {
                        if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                        }
                        else
                        {
                            Sys_Fashion.Instance.FashionExchangeDrawItemReq(1);//魔币兑换道具 
                        }
                    });
                }
                else
                {
                    //“该操作将消耗{0}个{活动道具}、{1}魔币 进行{2}次奖池抽取。”
                    string content = LanguageHelper.GetTextContent(590002102, 0.ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                 needDiamondCount.ToString(), 1.ToString());
                    PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                   () =>
                   {
                       if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                       {
                           Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                       }
                       else
                       {
                           PlayFx(1);
                           //Sys_Fashion.Instance.FashionDrawReq(1);//兑换并且自动购买
                       }
                   });
                }
            }
        }

        //抽10次
        private void OnLuckDraw_2ButtonClicked()
        {
            if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(205))
            {
                string content = LanguageHelper.GetTextContent(2009583);
                Sys_Hint.Instance.PushContent_Normal(content);
                return;
            }

            long fashionPointCount = Sys_Bag.Instance.GetItemCount(m_CSVFashionActivityData.FashionCoin);
            if (fashionPointCount >= 10)     //是否抽10次
            {
                //“该操作将消耗{0}个{活动道具}，进行{0}次奖池抽取。”
                string content = LanguageHelper.GetTextContent(590002100, 10.ToString(),
                    LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id), 10.ToString());
                PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                () =>
                {
                    PlayFx(10);
                    //Sys_Fashion.Instance.FashionDrawReq(10);
                });
            }
            else if (fashionPointCount < 10)  //是否用魔币兑换抽奖道具
            {
                uint needDiamondCount = (uint)(10 - fashionPointCount) * m_CSVFashionActivityData.Value;
                if (!m_AutoBuy.isOn)//没有勾选 就直接弹是否用魔币兑换道具界面
                {
                    //“您的{活动道具}不足，是否使用{0}魔币购买{1}个{活动道具}”
                    string content = LanguageHelper.GetTextContent(590002101, LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                  needDiamondCount.ToString(), (10 - fashionPointCount).ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id));
                    PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                    () =>
                    {
                        if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                        }
                        else
                        {
                            Sys_Fashion.Instance.FashionExchangeDrawItemReq(10);///魔币兑换道具 
                        }
                    });
                }
                else
                {
                    //“该操作将消耗{0}个{活动道具}、{1}魔币 进行{2}次奖池抽取。”
                    string content = LanguageHelper.GetTextContent(590002102, fashionPointCount.ToString(), LanguageHelper.GetTextContent(m_CSVItemData_FashionCoin.name_id),
                 needDiamondCount.ToString(), 10.ToString());
                    PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                   () =>
                   {
                       if (needDiamondCount > Sys_Bag.Instance.GetItemCount(1))
                       {
                           Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(590002121));//魔币不足
                       }
                       else
                       {
                           PlayFx(10);
                           //Sys_Fashion.Instance.FashionDrawReq(10);//兑换并且自动购买
                       }
                   });
                }
            }
        }


        private void PlayFx(uint _drawTime)
        {
            m_DrawTime = _drawTime;
            
            m_AnimatorRoot.gameObject.SetActive(false);

            if (m_Draw)
                m_Draw.Play();
        }

        private void OnPlayableEnd(PlayableDirector playableDirector)
        {
            Sys_Fashion.Instance.FashionDrawReq(m_DrawTime);
        }

        #endregion


        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Fashion_LuckyDraw);
        }

        private void OnFashionCoinClick(BaseEventData baseEventData)
        {
            m_TipsFashionCoinRoot.SetActive(false);
        }

        private void OnFashionPointClick(BaseEventData baseEventData)
        {
            m_TipsFashionPointRoot.SetActive(false);
        }

        private void OnButtonDetailClicked()
        {
            UIRuleParam param = new UIRuleParam();
            param.TitlelanId = 590002106;
            param.StrContent = LanguageHelper.GetTextContent(m_CSVFashionActivityData.Rules);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void OnExchangeShopButtonClicked()
        {
            MallPrama param = new MallPrama();
            param.mallId = m_CSVFashionActivityData.PtStore[0];
            param.shopId = m_CSVFashionActivityData.PtStore[1];
            param.isCharge = false;
            UIManager.OpenUI(EUIID.UI_Mall, false, param);
        }

        private void OnFashionPointTipsButtonClicked()
        {
            //590002113
            m_TipsFashionPointRoot.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_TipsFashionPointRoot.transform as RectTransform);
            //FrameworkTool.ForceRebuildLayout(m_TipsFashionPointRoot);
            string content = LanguageHelper.GetTextContent(590002113, m_CSVFashionActivityData.Reward.ToString(), endTime.Year.ToString(),
                endTime.Month.ToString(), endTime.Day.ToString(), m_ToSilverCoinRate.ToString());
            TextHelper.SetText(m_TipsFashionPoint, content);
        }

        private void OnDiamondsAddButtonClicked()
        {
            Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.Diamonds, 0);
        }

        private void OnFashionCoinButtonClicked()
        {
            //590002112
            m_TipsFashionCoinRoot.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_TipsFashionCoinRoot.transform as RectTransform);
            TextHelper.SetText(m_TipsFashionCoin, 590002112);
        }

        private void OnToggleChanged(bool arg)
        {
            if (arg != Sys_Fashion.Instance.autoBuyDraw)
            {
                Sys_Fashion.Instance.AutoBuyDrawReq(arg ? 1 : 0);
            }
        }
    }
}


