using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Framework;
using Table;
using Lib.Core;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;

namespace Logic
{
    public class UI_AttrView : UIComponent
    {
        private Text playerName;
        private Text playerGrade;
        private Text comprehensiveGrade;
        private Text level;
        private Text exp;
        private Text energyNum;
        private Text reputationNum;
        private Text reputationTitle;
        private Text reputationLv;
        private Text familyLocaltionName;
        private Text hpNum;
        private Text mpNum;
        private Text captainPoint;
        private Text aidPoint;
        private Text pvpPoint;
        private Text travelersLogPoint;
        private Text tutorTitle;
        private Text tutorPercent;
        private Text rideNum;
        private Text promoteDes;

        private Slider expSlider;
        private Slider energySlider;
        private Slider reputationSlider;
        private Slider hpSlider;
        private Slider mpSlider;
        private Slider tutorSlider;
        private Slider rideSlider;

        private GameObject firstattrGo;
        private GameObject basicattrGo;
        private GameObject seniorattrList;
        private GameObject basicattrView;
        private GameObject seniorattrView;
        private GameObject moreView;
        private GameObject changeHeadRedPoint;
        private GameObject worldLvRedPoint;

        private Button messageBtn;
        private Button lookBtn;
        private Image roleicon;
        private Button rolePosTypeBtn;
        private Button changeHeadBtn;
        private Button useEnergyBtn;
        private Button reputationBtn;
        private Button familyEmpowermentBtn;
        private Button hpAddBtn;
        private Button mpAddBtn;
        private Button pointChangeBtn;
        private Button successViewBtn;
        private Button renameBtn;
        private Button rideBtn;
        private Button promoteBtn;
        private Button outlookingBtn;

        private RawImage rawImage;        
        private Image eventImage;
        private Toggle basicattrtoggle;
        private Toggle seniorattrtoggle;
        private Toggle moretoggle;
        private Toggle hpAutoToggle;
        private Toggle mpAutoToggle;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private List<UI_Attr> elelist = new List<UI_Attr>();
        private List<UI_Attr> basiclist = new List<UI_Attr>();
        private List<UI_Attr> seniorlist = new List<UI_Attr>();
        private List<uint> seniorType = new List<uint>();
        private List<uint> basicIds = new List<uint>();
        private List<uint> eleIds = new List<uint>();

        private GameObject frontPosGo;
        private GameObject BehindPosGo;
        private uint _hpItemId;
        private uint _mpItemId;
        public bool isHpMpShow;
        private CSVCharacterAttribute.Data csvCharacterAttributeDataNextLevel;

        #region Title
        private Text mTitle_text1;
        private Text mTitle_text2;
        private Image mTitle_img2;

        private Image mTitle_img3;
        private Transform mTitle_Fx3parent;
        private GameObject mNo_Title;

        AsyncOperationHandle<GameObject> requestRef;
        private GameObject titleEffect;

        #endregion        

        protected override void Loaded()
        {
            playerName = transform.Find("View_Right/Text_Name").GetComponent<Text>();
            playerGrade = transform.Find("View_Middle/Text_Role_Grade/Text_Num").GetComponent<Text>();
            comprehensiveGrade = transform.Find("View_Middle/Text_Comprehensive_Grade/Text_Num").GetComponent<Text>();
            level = transform.Find("View_Right/Text_Level").GetComponent<Text>();
            exp = transform.Find("View_Right/Text_Percent").GetComponent<Text>();
            energyNum = transform.Find("View_Right/Eg/Text_Percent_Eg").GetComponent<Text>();
            reputationNum = transform.Find("View_Right/Text_Reputation/Text_Percent").GetComponent<Text>();
            reputationTitle = transform.Find("View_Right/Text_Reputation/Text_Name").GetComponent<Text>();
            reputationLv = transform.Find("View_Right/Text_Reputation/Text_Lv").GetComponent<Text>();
            familyLocaltionName = transform.Find("View_Right/Image_Family/Text_FamilyName").GetComponent<Text>();
            hpNum = transform.Find("View_Right3/Image_Hp/Text_Num_HP").GetComponent<Text>();
            mpNum = transform.Find("View_Right3/Image_Mp/Text_Num_Mp").GetComponent<Text>();
            captainPoint = transform.Find("View_Right3/Image_Point/Basic_Attr_Group/Attr_Grid/Item/Text_Number").GetComponent<Text>();
            aidPoint = transform.Find("View_Right3/Image_Point/Basic_Attr_Group/Attr_Grid/Item (1)/Text_Number").GetComponent<Text>();
            pvpPoint = transform.Find("View_Right3/Image_Point/Basic_Attr_Group/Attr_Grid/Item (2)/Text_Number").GetComponent<Text>();
            travelersLogPoint = transform.Find("View_Right3/Image_Point/Basic_Attr_Group/Attr_Grid/Item (3)/Text_Number").GetComponent<Text>();
            tutorTitle = transform.Find("View_Right3/Image_Tutor/Text_Title").GetComponent<Text>();
            tutorPercent = transform.Find("View_Right3/Image_Tutor/Text_Percent").GetComponent<Text>();
            rideNum = transform.Find("View_Right3/Image_Energy/Text_Num_Energy").GetComponent<Text>();
            promoteDes = transform.Find("View_Right/Text_Promote").GetComponent<Text>();

            hpSlider = transform.Find("View_Right3/Image_Hp/Slider_HP").GetComponent<Slider>();
            mpSlider = transform.Find("View_Right3/Image_Mp/Slider_MP").GetComponent<Slider>();
            expSlider = transform.Find("View_Right/Slider_Exp").GetComponent<Slider>();
            energySlider = transform.Find("View_Right/Eg/Slider_Eg").GetComponent<Slider>();
            reputationSlider = transform.Find("View_Right/Text_Reputation/Slider_Reputation").GetComponent<Slider>();
            tutorSlider = transform.Find("View_Right3/Image_Tutor/Slider").GetComponent<Slider>();
            rideSlider = transform.Find("View_Right3/Image_Energy/Slider_Energy").GetComponent<Slider>();

            messageBtn = transform.Find("View_Right/Button_Message").GetComponent<Button>();
            messageBtn.onClick.AddListener(OnmessageBtnClicked);;
            lookBtn = transform.Find("View_Right/Text_Reputation/Button_Look").GetComponent<Button>();
            lookBtn.onClick.AddListener(OnlookBtnClicked);
            useEnergyBtn = transform.Find("View_Right/Eg/Button_Use").GetComponent<Button>();
            useEnergyBtn.onClick.AddListener(OnuseEnergyBtnClicked);
            reputationBtn = transform.Find("View_Right/Text_Reputation/Button_Look").GetComponent<Button>();
            reputationBtn.onClick.AddListener(OnreputationBtnClicked);
            familyEmpowermentBtn = transform.Find("View_Right/Image_Family/Button_Family").GetComponent<Button>();
            familyEmpowermentBtn.onClick.AddListener(OnfamilyEmpowermentBtnClicked);
            renameBtn = transform.Find("View_Right/Button_Rename").GetComponent<Button>();
            renameBtn.onClick.AddListener(OnReNameBtnClicked);
            rideBtn = transform.Find("View_Right3/Image_Energy/Btn_Plus").GetComponent<Button>();
            rideBtn.onClick.AddListener(OnRideBtnClicked);
            promoteBtn = transform.Find("View_Right/Button_Promote").GetComponent<Button>();
            promoteBtn.onClick.AddListener(OnPromoteBtnClicked);

            roleicon = transform.Find("View_Right/Image_Icon").GetComponent<Image>();
            rawImage = transform.Find("View_Middle/Charapter").GetComponent<RawImage>();
            eventImage = transform.Find("View_Middle/EventImage").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);

            basicattrtoggle = transform.Find("Menu/ListItem").GetComponent<Toggle>();
            basicattrtoggle.onValueChanged.AddListener(OnbasicattrtoggleValueChanged);
            seniorattrtoggle = transform.Find("Menu/ListItem (1)").GetComponent<Toggle>();
            seniorattrtoggle.onValueChanged.AddListener(OnseniorattrtoggleValueChanged);
            moretoggle = transform.Find("Menu/ListItem (2)").GetComponent<Toggle>();
            moretoggle.onValueChanged.AddListener(OnmoretoggleValueChanged);
            hpAutoToggle = transform.Find("View_Right3/Image_Hp/Toggle").GetComponent<Toggle>();
            mpAutoToggle = transform.Find("View_Right3/Image_Mp/Toggle").GetComponent<Toggle>();

            mTitle_text1 = transform.Find("View_Middle/Title/Text").GetComponent<Text>();
            mTitle_text2 = transform.Find("View_Middle/Title/Image/Text").GetComponent<Text>();
            mTitle_img2 = transform.Find("View_Middle/Title/Image").GetComponent<Image>();
            mTitle_img3 = transform.Find("View_Middle/Title/Image1").GetComponent<Image>();
            mTitle_Fx3parent = transform.Find("View_Middle/Title/Image1/Fx");
            mNo_Title = transform.Find("View_Middle/Title/Image_None").gameObject;

            rolePosTypeBtn = transform.Find("View_Middle/Button_Stand").GetComponent<Button>();
            changeHeadBtn = transform.Find("View_Right/Button_ChangeHead").GetComponent<Button>();
            hpAddBtn = transform.Find("View_Right3/Image_Hp/Btn_Plus").GetComponent<Button>();
            mpAddBtn = transform.Find("View_Right3/Image_Mp/Btn_Plus").GetComponent<Button>();
            pointChangeBtn = transform.Find("View_Right3/Image_Point/Button_Sure").GetComponent<Button>();
            outlookingBtn = transform.Find("View_Middle/Button_fashion").GetComponent<Button>();

            rolePosTypeBtn.onClick.AddListener(OnrolePosTypeBtnClicked);
            changeHeadBtn.onClick.AddListener(OnchangeHeadBtnClicked);
            hpAddBtn.onClick.AddListener(OnhpAddBtnClicked);
            mpAddBtn.onClick.AddListener(OnmpAddBtnClicked);
            pointChangeBtn.onClick.AddListener(OnpointChangeBtnClicked);
            outlookingBtn.onClick.AddListener(OnoutlookingBtnClicked);
            hpAutoToggle.onValueChanged.AddListener(OnHpAutoToggleValueChanged);
            mpAutoToggle.onValueChanged.AddListener(OnMpAutoToggleValueChanged);

            changeHeadRedPoint = transform.Find("View_Right/Button_ChangeHead/Image_Dot").gameObject;;
            frontPosGo = transform.Find("View_Middle/Button_Stand/Image_Front").gameObject;
            BehindPosGo = transform.Find("View_Middle/Button_Stand/Image_Behind").gameObject;
            firstattrGo = transform.Find("View_Right/Scroll_View_Basic/GameObject/Attr_Basic/Ele_Attr_Group/Attr_Grid/Image_Attr").gameObject;
            basicattrGo = transform.Find("View_Right/Scroll_View_Basic/GameObject/Attr_Basic/Basic_Attr_Group/Attr_Grid/Image_Attr").gameObject;
            seniorattrList = transform.Find("View_Right1/Scroll_View_Senior/GameObject/Attr_Basic/Attr_Group").gameObject;
            basicattrView = transform.Find("View_Right").gameObject;
            seniorattrView = transform.Find("View_Right1").gameObject;
            moreView = transform.Find("View_Right3").gameObject;
            worldLvRedPoint = transform.Find("View_Right/Button_Message/Image_Dot").gameObject;

            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(365);
            if (cSVParamData != null)
            {
                string[] strArr = cSVParamData.str_value.Split('|');
                if (strArr.Length == 2)
                {
                    uint.TryParse(strArr[0], out _hpItemId);
                    uint.TryParse(strArr[1], out _mpItemId);
                }
            }
    }

        uint onShowTime;
        public override void Show()
        {
            base.Show();

            basicattrtoggle.isOn =! isHpMpShow;
            seniorattrtoggle.isOn = false;
            moretoggle.isOn = isHpMpShow;
            SetValue();
            OnCreateModel();
            UpdateTitle(Sys_Title.Instance.curShowTitle);
            UIManager.HitPointShow(EUIID.UI_Attribute, ERoleViewType.ViewAttr.ToString());
            onShowTime = Sys_Time.Instance.GetServerTime();
            changeHeadRedPoint.SetActive(Sys_Head.Instance.CheckShowRedPoint());
            if (Sys_FunctionOpen.Instance.IsOpen(50901, false))
            {
                Sys_Rank.Instance.BatUnitDescReq(6, 1, 0, 1, 3);
            }
            outlookingBtn.gameObject.SetActive(Sys_FunctionOpen.Instance.IsOpen(10400, false));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);

            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnAddExp, OnAddExp, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnExtraExp, OnAddExp, toRegister);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnUpdateTitleAttrView, UpdateTitle, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnGradeUpdate, OnGradeUpdate, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnSetPosType, OnSetPosType, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnUsingUpdate, OnUsingUpdate, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnAddUpdate, OnChangeHeadRedPointUpdate, toRegister);
            Sys_Head.Instance.eventEmitter.Handle(Sys_Head.EEvents.OnExpritedUpdate, OnChangeHeadRedPointUpdate, toRegister);
            Sys_Reputation.Instance.eventEmitter.Handle(Sys_Reputation.EEvents.OnReputationUpdate, SetReputationAndFamilyData, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, SetEnergyData, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnHpMpPoolUpdate, OnHpMpPoolUpdate, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnDailyPointUpdate, UpdateDailyPointInfo, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChange, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnTutorInfoUpdate, RefreshTutorInfo, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnReName, OnReNameUpdate, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnEnergyChargeEnd, RefreshRideView, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateOpenServiceDay, OnUpdateOpenServiceDay, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnClickWorldLevel, OnUpdateOpenServiceDay, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnGetPointScheme, OnGetPointScheme, toRegister);
        }

        public override void Hide()
        {
            _UnloadShowContent();
            AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetsLoaded);

            UIManager.HitPointHide(EUIID.UI_Attribute, onShowTime, ERoleViewType.ViewAttr.ToString());
            isHpMpShow = false;
            base.Hide();
        }

        public override void OnDestroy()
        {
          //  mUI_Title.OnDestroy();
        }

        private void OnseniorattrtoggleValueChanged(bool isOn)
        {
            basicattrView.SetActive(!isOn);
            moreView.SetActive(!isOn);
            seniorattrView.SetActive(isOn);
        }

        private void OnbasicattrtoggleValueChanged(bool isOn)
        {
            basicattrView.SetActive(isOn);
            seniorattrView.SetActive(!isOn);
            moreView.SetActive(!isOn);
        }

        private void OnmoretoggleValueChanged(bool isOn)
        {
            basicattrView.SetActive(!isOn);
            seniorattrView.SetActive(!isOn);
            moreView.SetActive(isOn);
        }

        #region ModelShow
        private void OnCreateModel()
        {            
            _LoadShowScene();
            _LoadShowModel((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
            heroLoader.LoadWeaponPart(Sys_Fashion.Instance.GetCurDressedFashionWeapon(), Sys_Equip.Instance.GetCurWeapon());            
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }
        private void _LoadShowModel(uint careerid)
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
                heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
            }

            heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            });            

        }

        private void _UnloadShowContent()
        {
            _uiModelShowManagerEntity?.Dispose();
            _uiModelShowManagerEntity = null;
            rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
            showSceneControl?.Dispose();
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                _uiModelShowManagerEntity?.Dispose();
                _uiModelShowManagerEntity = null;
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
                uint highId = Hero.GetMainHeroHighModelAnimationID();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, Sys_Equip.Instance.GetCurWeapon());

                GameObject mainGo = heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).gameObject;
                mainGo.SetActive(false);
                _uiModelShowManagerEntity = WS_UIModelShowManagerEntity.Start(500000, null, heroLoader.heroDisplay.mAnimation, mainGo);
            }
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = localAngle;
            }
        }

        #endregion

        #region Function
        private void OnUpdateAttr()
        {
            for (int i=0;i< elelist.Count;++i)
            {
                elelist[i].RefreshItem();
            }
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].RefreshItem();
            }
            for (int i = 0; i < seniorlist.Count; ++i)
            {
                seniorlist[i].RefreshItem();
            }
            level.text = Sys_Role.Instance.Role.Level.ToString();
            OnGradeUpdate();
            SetRenameButton();
            OnAdvanceInfoUpdate();
        }

        private void OnAddExp()
        {
            level.text = Sys_Role.Instance.Role.Level.ToString();
            csvCharacterAttributeDataNextLevel = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level + 1);
            OnGradeUpdate();
            OnAdvanceInfoUpdate();
            if (csvCharacterAttributeDataNextLevel != null)
            {
                exp.gameObject.SetActive(true);
                expSlider.gameObject.SetActive(true);
                if (Sys_Role.Instance.Role.ExtraExp == 0)
                {
                    exp.text = LanguageHelper.GetTextContent(2009377, Sys_Role.Instance.Role.Exp.ToString(), csvCharacterAttributeDataNextLevel.upgrade_exp.ToString());
                    expSlider.value = (float)Sys_Role.Instance.Role.Exp / (float)csvCharacterAttributeDataNextLevel.upgrade_exp;
                }
                else
                {
                    exp.text = LanguageHelper.GetTextContent(2009377, "-"+Sys_Role.Instance.Role.ExtraExp.ToString(), csvCharacterAttributeDataNextLevel.upgrade_exp.ToString());
                    expSlider.value = 0;
                }
                uint.TryParse(CSVParam.Instance.GetConfData(968).str_value, out uint compensationExpPercent);
            }
            else
            {
                exp.gameObject.SetActive(false);
                expSlider.gameObject.SetActive(false);
               // maxExpGo.gameObject.SetActive(true);
            }
            uint levelMount = CSVPetNewParam.Instance.GetConfData(65).value;
            ImageHelper.SetImageGray(rideBtn.GetComponent<Image>(), Sys_Role.Instance.Role.Level < levelMount, true);
        }

        private void SetValue()
        {
            playerName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
            OnAddExp();
            Sys_Head.Instance.SetHeadAndFrameData(roleicon);
            DefaultBasicAttr();
            DefaultSeniorAttr();
            AddBasicList();
            AddSeniorList();
            OnSetPosType();
            SetEnergyData();
            SetReputationAndFamilyData();
            Sys_Attr.Instance.TutorInfoReq();
            SetHpMpValue();
            UpdateDailyPointInfo();
            NormalPointInfo();
            RefreshTutorInfo();
            RefreshRideView();
            SetRenameButton();
            OnUpdateOpenServiceDay();
        }

        /// <summary>进阶tip</summary>
        private void OnAdvanceInfoUpdate()
        {
            bool active = false;
            uint levelLimit = Sys_Advance.Instance.GetCurLimiteLevel();
            if (levelLimit == Sys_Role.Instance.Role.Level)
            {
                uint realLv = Sys_Attr.Instance.GetRealLv();
                promoteDes.text = LanguageHelper.GetTextContent(2005055, realLv.ToString());
                active = realLv > Sys_Role.Instance.Role.Level;
            }
            promoteBtn.gameObject.SetActive(active);
            promoteDes.gameObject.SetActive(active);
        }

        private void OnGradeUpdate()
        {
            playerGrade.text = Sys_Attr.Instance.rolePower.ToString();
            comprehensiveGrade.text = Sys_Attr.Instance.power.ToString();
        }

        private void OnSetPosType()
        {
            BehindPosGo.SetActive(Net_Combat.Instance.m_PosType == 0);
            frontPosGo.SetActive(Net_Combat.Instance.m_PosType == 1);
        }

        private void OnUsingUpdate()
        {
            Sys_Head.Instance.SetHeadAndFrameData(roleicon);
        }

        private void OnChangeHeadRedPointUpdate()
        {
            changeHeadRedPoint.SetActive(Sys_Head.Instance.CheckShowRedPoint());
        }

        private void AddSeniorList()
        {
            SetSeniorAttrType();
            seniorlist.Clear();
            for (int i = 0; i < seniorType.Count; ++i)
            {
                if (seniorattrList.transform.parent.transform.Find(seniorType[i].ToString())==null)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(seniorattrList, seniorattrList.transform.parent);        
                    go.transform.name = seniorType[i].ToString();
                    GameObject attrItem = go.transform.Find("Attr_Grid/Image_Attr").gameObject;
                    Text name = go.transform.Find("Title_Tips01/Text_Title").GetComponent<Text>();
                    if (seniorType[i]==21)
                    {
                        name.text =LanguageHelper.GetTextContent(4419);
                    }
                    else if (seniorType[i] == 22)
                    {
                        name.text = LanguageHelper.GetTextContent(4420);
                    }
                    else if (seniorType[i] == 23)
                    {
                        name.text = LanguageHelper.GetTextContent(4421);
                    }
                    else if (seniorType[i] == 24)
                    {
                        name.text = LanguageHelper.GetTextContent(4422);
                    }
                    else if (seniorType[i] == 25)
                    {
                        name.text = LanguageHelper.GetTextContent(4423);
                    }
                    else if (seniorType[i] == 26)
                    {
                        name.text = LanguageHelper.GetTextContent(4425);
                    }
                    for (int j=0; j<Sys_Attr.Instance.pkAttrsId.Count; ++j)
                    {
                        CSVAttr.Data data = CSVAttr.Instance.GetConfData(Sys_Attr.Instance.pkAttrsId[j]);
                        if (data.attr_type == seniorType[i] && data.isShow == 1)
                        {
                            GameObject attrgo = GameObject.Instantiate<GameObject>(attrItem, attrItem.transform.parent);
                            UI_Attr seniorAttr = new UI_Attr(Sys_Attr.Instance.pkAttrsId[j], false);
                            seniorAttr.Init(attrgo.transform);
                            seniorAttr.RefreshItem();
                            seniorlist.Add(seniorAttr);
                        }
                    }
                    attrItem.SetActive(false);
                }
            }
            seniorattrList.SetActive(false);
            for (int i = 0; i < seniorlist.Count; ++i)
            {
                seniorlist[i].ShowBg(i % 2==0);
            }
        }

        private void AddBasicList()
        {
            elelist.Clear();
            basiclist.Clear();
            for (int j = 0; j < Sys_Attr.Instance.pkAttrsId.Count; ++j)
            {
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(Sys_Attr.Instance.pkAttrsId[j]);
                if (data.isShow == 1 && data.attr_type == 1)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(basicattrGo, basicattrGo.transform.parent);
                    UI_Attr basicAttr = new UI_Attr(Sys_Attr.Instance.pkAttrsId[j], false);
                    basicAttr.Init(go.transform);
                    basicAttr.RefreshItem( );
                    basiclist.Add(basicAttr);
                }
                else if (data.attr_type == 4&& data.isShow == 1)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(firstattrGo, firstattrGo.transform.parent);
                    UI_Attr eleAttr = new UI_Attr(Sys_Attr.Instance.pkAttrsId[j], true);
                    eleAttr.Init(go.transform);
                    eleAttr.RefreshItem();
                    elelist.Add(eleAttr);
                }
            }
            for (int i = 0; i < elelist.Count; ++i)
            {
                elelist[i].ShowBg(i%4<2);
            }
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].ShowBg(i % 4 < 2);
            }
            firstattrGo.SetActive(false);
            basicattrGo.SetActive(false);
        }

        private void DefaultBasicAttr()
        {
            firstattrGo.SetActive(true);
            basicattrGo.SetActive(true);
            for(int i=0;i< elelist.Count;++i)
            {
                elelist[i].OnDestroy();
            }
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].OnDestroy();
            }
            FrameworkTool.DestroyChildren(firstattrGo.transform.parent.gameObject, firstattrGo.transform.name);
            FrameworkTool.DestroyChildren(basicattrGo.transform.parent.gameObject, basicattrGo.transform.name);
        }

        private void DefaultSeniorAttr()
        {
            seniorattrList.SetActive(true);
            for (int i = 0; i < seniorlist.Count; ++i)
            {
                seniorlist[i].OnDestroy();
            }
            FrameworkTool.DestroyChildren(seniorattrList.transform.parent.gameObject, seniorattrList.transform.name);
        }

        private void SetSeniorAttrType()
        {
            seniorType.Clear();
            uint type=0;
            for (int j = 0; j < Sys_Attr.Instance.pkAttrsId.Count; ++j)
            {
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(Sys_Attr.Instance.pkAttrsId[j]);
                if (data.isShow==1&& type != data.attr_type && data.attr_type != 1 && data.attr_type != 3 && data.attr_type != 4 && data.attr_type != 0)
                {
                    type = data.attr_type;
                    seniorType.Add(type);
                }
            }
        }

        private void SetReputationAndFamilyData()
        {
            uint csvFameLevel = Sys_Reputation.Instance.danLevel;
            reputationTitle.text = LanguageHelper.GetTextContent(CSVFameRank.Instance.GetConfData(csvFameLevel).name);
            reputationLv.text = LanguageHelper.GetTextContent(2020918, Sys_Reputation.Instance.danLevel.ToString(), Sys_Reputation.Instance.specificLevel.ToString());
            float maxValue = CSVFameRank.Instance.GetConfData(csvFameLevel).lvup_cost;
            ulong curValue = Sys_Reputation.Instance.reputationValue;
            if (csvFameLevel == 10 && Sys_Reputation.Instance.specificLevel >= 100)
            {
                reputationSlider.value = 1;
            }
            else
            {
                reputationSlider.value = curValue / maxValue;
            }
            reputationNum.text = LanguageHelper.GetTextContent(2010321, curValue.ToString(), ((int)maxValue).ToString());
            if (Sys_Family.Instance.familyData.isInFamily)
            {
                if (null == Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info) return;
                familyLocaltionName.text = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info.GName.ToStringUtf8();
            }
            else
            {
                familyLocaltionName.text = LanguageHelper.GetTextContent(5011);
            }
        }

        private void SetEnergyData()
        {
            energySlider.gameObject.SetActive(true);
            uint vitalityMax = Sys_Vitality.Instance.GetMaxVitality();

            long count = Sys_Bag.Instance.GetItemCount(5);
            energyNum.text = LanguageHelper.GetTextContent(2009377, count.ToString(), vitalityMax.ToString());
            if (count <= vitalityMax)
            {
                energySlider.value = (float)count / vitalityMax;
            }
            else
            {
                energySlider.value = 1;
            }
        }

        private void SetHpMpValue()
        {
            float hpMax;
            float mpMax;
            float.TryParse(CSVParam.Instance.GetConfData(363).str_value, out hpMax);
            float.TryParse(CSVParam.Instance.GetConfData(364).str_value, out mpMax);
            ulong hpPool = Sys_Attr.Instance.hpPool;
            ulong mpPool = Sys_Attr.Instance.mpPool;
            hpNum.text = hpPool.ToString();
            mpNum.text = mpPool.ToString();
            hpAutoToggle.isOn = Sys_Attr.Instance.isHpAutoOpen;
            mpAutoToggle.isOn = Sys_Attr.Instance.isMpautoOpen;
            if (hpPool >= hpMax)
            {
                hpSlider.value = 1; 
            }
            else
            {
                hpSlider.value = Sys_Attr.Instance.hpPool / hpMax;
            }
            if (mpPool >= mpMax)
            {
                mpSlider.value = 1;
            }
            else
            {
                mpSlider.value = Sys_Attr.Instance.mpPool / mpMax;
            }
        }

        private void OnHpMpPoolUpdate()
        {
            SetHpMpValue();
        }

        private void UpdateDailyPointInfo()
        {
            UpdateCaptainPointInfo();
            UpdateAidPointInfo();
        }

        private void NormalPointInfo()
        {
            pvpPoint.text = Sys_Bag.Instance.GetItemCount((uint)EPointType.Arena).ToString();
            travelersLogPoint.text = Sys_Bag.Instance.GetItemCount((uint)EPointType.TravelersLog).ToString();
        }

        private void UpdateCaptainPointInfo()
        {
            captainPoint.text = Sys_Bag.Instance.GetItemCount((uint)EPointType.Captain).ToString();
        }

        private void UpdateAidPointInfo()
        {
            aidPoint.text = Sys_Bag.Instance.GetItemCount((uint)EPointType.Aid).ToString();
        }

        private void OnCurrencyChange(uint id, long value)
        {
            NormalPointInfo();
        }

        private void RefreshTutorInfo()
        {
            tutorSlider.minValue = 0;
            CSVTutor.Data data = CSVTutor.Instance.GetConfData(Sys_Attr.Instance.tutorLevel + 1);
            CSVTutor.Data dataNext = CSVTutor.Instance.GetConfData(Sys_Attr.Instance.tutorLevel + 2);
            if (data != null)
            {
                ulong curExp;
                if (dataNext != null)
                    curExp = Sys_Attr.Instance.tutorExp;
                else
                    curExp = Sys_Attr.Instance.tutorExp != 0 ? Sys_Attr.Instance.tutorExp : data.TutorExp;
                ulong nextExp = dataNext != null ? dataNext.TutorExp : data.TutorExp;
                tutorTitle.text = LanguageHelper.GetTextContent(2005038, LanguageHelper.GetTextContent(data.TutorLevelLan));
                tutorPercent.text = LanguageHelper.GetTextContent(11660, curExp.ToString(), nextExp.ToString());
                tutorSlider.maxValue = nextExp;
                tutorSlider.value = curExp;
            }
        }

        private void OnReNameUpdate()
        {
            playerName.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
        }

        private void RefreshRideView()
        {
            uint currentValue = Sys_Pet.Instance.RidingEnergy;
            uint currentMax = CSVPetNewParam.Instance.GetConfData(61).value;
            rideNum.text =currentValue.ToString();
            rideSlider.maxValue = currentMax;
            rideSlider.value = currentValue;
        }

        private void OnUpdateOpenServiceDay()
        {
            worldLvRedPoint.SetActive(Sys_Attr.Instance.CheckWorldLevelChanged());
        }

        private void OnGetPointScheme()
        {
            for (int i = 0; i < elelist.Count; ++i)
            {
                elelist[i].RefreshItem();
            }
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].RefreshItem();
            }
            for (int i = 0; i < seniorlist.Count; ++i)
            {
                seniorlist[i].RefreshItem();
            }
        }

        private void SetRenameButton()
        {
            CSVParam.Data pData = CSVParam.Instance.GetConfData(1328);
            var value = Convert.ToUInt32(pData.str_value);
            if (value!=0)
            {
                renameBtn.gameObject.SetActive(Sys_Role.Instance.Role.Level<=value);
            }
            
        }
        #endregion

        #region Title
        public void UpdateTitle(uint titleId)
        {
            if (titleId==0)
            {
                SetTitleShowType(0);
                return;
            }
            CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(titleId);
            if (cSVTitleData != null)
            {
                if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType(1);
                        TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType(2);
                        TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                    }
                }
                else if (cSVTitleData.id == Sys_Title.Instance.bGroupTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType(1);
                        TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType(2);
                        TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleWarriorGroupName());
                        TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                    }
                }
                else
                {
                    if (cSVTitleData.titleShowLan != 0)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text1, cSVTitleData.titleShow[0], cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon);
                        }
                    }
                    else
                    {
                        SetTitleShowType(3);
                        ImageHelper.SetIcon(mTitle_img3, cSVTitleData.titleShowIcon);
                        uint FxId = cSVTitleData.titleShowEffect;
                        CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                        if (cSVSystemEffectData != null)
                        {
                            LoadTitleEffectAssetAsyn(cSVSystemEffectData.FxPath);
                        }
                    }
                }
            }
        }

        private void LoadTitleEffectAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetsLoaded);
        }

        private void OnAssetsLoaded(AsyncOperationHandle<GameObject> handle)
        {
            titleEffect = handle.Result;
            if (null != titleEffect)
            {
                titleEffect.transform.SetParent(mTitle_Fx3parent);
                RectTransform rectTransform = titleEffect.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }
        

        private void SetTitleShowType(int type)
        {
            if (type==0)
            {
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(false);
                mNo_Title.SetActive(true);
            }
            else if (type == 1)
            {
                mNo_Title.SetActive(false);
                mTitle_text1.gameObject.SetActive(true);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else if (type == 2)
            {
                mNo_Title.SetActive(false);
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(true);
                mTitle_img2.gameObject.SetActive(true);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else
            {
                mNo_Title.SetActive(false);
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(true);
                mTitle_Fx3parent.gameObject.SetActive(true);
            }
        }
        #endregion

        #region ButtonClicked
        private void OnlookBtnClicked()
        {          
            UIManager.OpenUI(EUIID.UI_Reputation); 
        }

        private void OnuseEnergyBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Vitality);
        }

        private void OnreputationBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Reputation);
        }

        private void OnReNameBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_ReName);
        }

        private void OnRideBtnClicked()
        {
            uint level = CSVPetNewParam.Instance.GetConfData(65).value;
            if(Sys_Role.Instance.Role.Level>= level)
            {
                UIManager.OpenUI(EUIID.UI_Pet_MountCharge);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000228, level.ToString()));
            }
        }

        private void OnfamilyEmpowermentBtnClicked()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10109, true))
                return;
            Sys_Experience.Instance.InfoReq();
            UIManager.OpenUI(EUIID.UI_Family_Empowerment);
        }

        private void OnmessageBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_ExpTips);
            if (Sys_Role.Instance.lastClickOpenServiceDay != Sys_Role.Instance.openServiceDay)
            {
                Sys_Role.Instance.OnClickWorldLevelReq();
            }
        }

        private void OnfamilyexpBtnClicked()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10109, true))
                return;
            Sys_Experience.Instance.InfoReq();
            UIManager.OpenUI(EUIID.UI_Family_Empowerment);
        }

        private void OnrolePosTypeBtnClicked()
        {
            if (Net_Combat.Instance.m_PosType == 0)
            {
                Net_Combat.Instance.SetPosTypeReq(1);
            }
            else
            {
                Net_Combat.Instance.SetPosTypeReq(0);
            }
        }

        private void OnchangeHeadBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Head);
        }

        private void OnmpAddBtnClicked()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.itemId = _mpItemId;
            mallPrama.mallId = 301;
            mallPrama.shopId = 3001;
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }

        private void OnhpAddBtnClicked()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.itemId = _hpItemId;
            mallPrama.mallId = 301;
            mallPrama.shopId = 3001;
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }

        private void OnpointChangeBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_PointMall, false, new MallPrama() { mallId = 501 });
        }

        private void OnHpAutoToggleValueChanged(bool isOn)
        {
            if (isOn != Sys_Attr.Instance.isHpAutoOpen)
            {
                hpAutoToggle.isOn = isOn;
                Sys_Attr.Instance.SetAutoHpMpPoolReq(isOn, Sys_Attr.Instance.isMpautoOpen);
            }
        }

        private void OnMpAutoToggleValueChanged(bool isOn)
        {
            if (isOn != Sys_Attr.Instance.isMpautoOpen)
            {
                mpAutoToggle.isOn = isOn;
                Sys_Attr.Instance.SetAutoHpMpPoolReq(Sys_Attr.Instance.isHpAutoOpen, isOn);
            }
        }

        private void OnPromoteBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Advance_Level);
        }


        private void OnoutlookingBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Fashion);
        }
        #endregion
    }
}
