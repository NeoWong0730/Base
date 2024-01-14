using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Framework;
using System;
using Packet;
using Table;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.EventSystems;
using Lib.Core;

namespace Logic
{

    public class FriendInfoEvt
    {
        public ulong roleId;
        public uint weaponItemId;
        public uint weaponFashionId;
        public uint power;
        public uint rolePower;
        public uint headId;
        public uint headFrameId;
        public ClientPet clientPet;
        public List<uint> pkAttrsId;
        public Dictionary<uint, long> pkAttrs ;
        public Dictionary<uint, List<dressData>> fashoinDic;
        public Dictionary<uint, int> baseAttrs2AttrAssigh ;
        public RoleBase roleBase;

    }

    public class UI_Friend_Attribute_Layout
    {
        public Transform transform;
        public Button closeBtn;
        public GameObject attrView;
        public GameObject petView; 
        public UI_CurrencyTitle UI_CurrencyTitle;
        public UI_Friend_Attribute_LeftTabs leftTabs;
        public UI_Friend_AttrView viewAttr;
        public UI_Pet_Detail_View viewPetDetail;   

        public void Init(Transform transform)
        {
            this.transform = transform;
            closeBtn = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            petView = transform.Find("Animator/UI_Pet_Details").gameObject;

            leftTabs = new UI_Friend_Attribute_LeftTabs();
            leftTabs.Init(this.transform.Find("Animator/View_Left_Tabs"));

            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            attrView = transform.Find("Animator/View_Attribute").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
        }
    }

    public class UI_Friend_Attribute_LeftTabs : UIComponent
    {
        private List<Toggle> tabList = new List<Toggle>();
        public List<GameObject> tabGoList = new List<GameObject>();
        private int lastIndex = -1;
        private Transform tabParent;

        protected override void Loaded()
        {
            tabList.Clear();
            tabGoList.Clear();
            lastIndex = -1;
            tabParent = transform.Find("Scroll View/TabList");
            for (int i = 0; i < tabParent.childCount; ++i)
            {
                tabGoList.Add(tabParent.GetChild(i).gameObject);
            }
            Toggle[] toggles = transform.GetComponentsInChildren<Toggle>();
            foreach (Toggle toggle in toggles)
            {
                tabList.Add(toggle);
                toggle.onValueChanged.AddListener(var =>
                {
                    if (var)
                    {
                        int index = tabList.IndexOf(toggle);
                        OnSelect(index);
                    }
                });
            }
            toggles[0].isOn = true;
        }

        private void OnSelect(int _index)
        {
            if (lastIndex != _index)
            {
                lastIndex = _index;

                Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnSelectFriendInfoViewType, (EFriendInfoViewType)(_index + 1));
            }
        }


        public void OnDefaultSelect(int defaultIndex)
        {
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnSelectFriendInfoViewType, (EFriendInfoViewType)defaultIndex);
            tabList[defaultIndex - 1].isOn = true;
            lastIndex = defaultIndex - 1;
        }
    }

    public class UI_Friend_Attr : UIComponent
    {
        private uint id;
        private Text attrname;
        private Text number;
        private Text addpoint;
        private Image icon;
        private Button button;
        private Image bg;
        private bool isBasicAttr;

        public UI_Friend_Attr(uint _id, bool _isBasicAttr)
            : base()
        {
            id = _id;
            isBasicAttr = _isBasicAttr;
        }

        protected override void Loaded()
        {
            attrname = transform.Find("Attr").GetComponent<Text>();
            number = transform.Find("Attr/Text/Text_Number").GetComponent<Text>();
            addpoint = transform.Find("Attr/Text").GetComponent<Text>();
            bg = transform.Find("Image_Attr (2)").GetComponent<Image>();
            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = CSVAttr.Instance.GetConfData(id).desc;
            UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
        }

        public void RefreshItem(long num, int assignNum, bool isShowBg = true)
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            bg.enabled = isShowBg;
            if (!isBasicAttr)
            {
                if (CSVAttr.Instance.GetConfData(id).show_type == 1)
                    number.text = num.ToString();
                else
                    number.text = ((float)num / 100).ToString() + "%";
                addpoint.text = string.Empty;
            }
            else
            {
                number.text = num.ToString();
                int origion = (int)num - assignNum;
                addpoint.text = LanguageHelper.GetTextContent(4418, assignNum.ToString(), origion.ToString());
            }
        }
    }

    public class UI_Friend_AttrView : UIComponent
    {
        private Text playerName;
        private Text playerGrade;
        private Text comprehensiveGrade;
        private Text level;
        private Text exp;
        private Slider expSlider;
        private Slider compensationExpSlider;
        private GameObject firstattrGo;
        private GameObject basicattrGo;
        private GameObject seniorattrList;
        private GameObject basicattrView;
        private GameObject seniorattrView;
        private Image roleicon;
        private RawImage rawImage;
        private Image eventImage;
        private Toggle basicattrtoggle;
        private Toggle seniorattrtoggle;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;
        private WS_UIModelShowManagerEntity _uiModelShowManagerEntity;

        private List<UI_Friend_Attr> elelist = new List<UI_Friend_Attr>();
        private List<UI_Friend_Attr> basiclist = new List<UI_Friend_Attr>();
        private List<UI_Friend_Attr> seniorlist = new List<UI_Friend_Attr>();
        private List<uint> seniorType = new List<uint>();

        private GameObject mView_middle;
        private GameObject mView_right;
        private GameObject maxExpGo;
        private GameObject frontPosGo;
        private GameObject BehindPosGo;

        private CSVCharacterAttribute.Data csvCharacterAttributeDataNextLevel;
        private FriendInfoEvt evt;

        protected override void Loaded()
        {
            playerName = transform.Find("View_Right/Text_Name").GetComponent<Text>();
            playerGrade = transform.Find("View_Middle/Text_Role_Grade/Text_Num").GetComponent<Text>();
            comprehensiveGrade = transform.Find("View_Middle/Text_Comprehensive_Grade/Text_Num").GetComponent<Text>();
            level = transform.Find("View_Right/Text_Level").GetComponent<Text>();
            exp = transform.Find("View_Right/Text_Percent").GetComponent<Text>();
            expSlider = transform.Find("View_Right/Slider_Exp").GetComponent<Slider>();
            compensationExpSlider = transform.Find("View_Right/Slider_Exp/Slider_WelfareExp").GetComponent<Slider>();
            firstattrGo = transform.Find("View_Right/Scroll_View_Basic/GameObject/Attr_Basic/Ele_Attr_Group/Attr_Grid/Image_Attr").gameObject;
            basicattrGo = transform.Find("View_Right/Scroll_View_Basic/GameObject/Attr_Basic/Basic_Attr_Group/Attr_Grid/Image_Attr").gameObject;
            seniorattrList = transform.Find("View_Right/Scroll_View_Senior/GameObject/Attr_Basic/Attr_Group").gameObject;
            basicattrView = transform.Find("View_Right/Scroll_View_Basic").gameObject;
            seniorattrView = transform.Find("View_Right/Scroll_View_Senior").gameObject;
            mView_middle = transform.Find("View_Middle").gameObject;
            mView_right = transform.Find("View_Right").gameObject;
            roleicon = transform.Find("View_Right/Image_Icon").GetComponent<Image>();
            rawImage = transform.Find("View_Middle/Charapter").GetComponent<RawImage>();
            eventImage = transform.Find("View_Middle/EventImage").GetComponent<Image>();

            basicattrtoggle = transform.Find("View_Right/Menu/ListItem").GetComponent<Toggle>();
            basicattrtoggle.onValueChanged.AddListener(OnbasicattrtoggleValueChanged);
            seniorattrtoggle = transform.Find("View_Right/Menu/ListItem (1)").GetComponent<Toggle>();
            seniorattrtoggle.onValueChanged.AddListener(OnseniorattrtoggleValueChanged);

            frontPosGo = transform.Find("View_Middle/Button_Stand/Image_Front").gameObject;
            BehindPosGo = transform.Find("View_Middle/Button_Stand/Image_Behind").gameObject;

            seniorattrView.SetActive(false);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            assetDependencies = transform.GetComponent<AssetDependencies>();
        }

        public UI_Friend_AttrView(FriendInfoEvt _evt): base()
        {
            evt = _evt;
        }

        public override void Show()
        {
            base.Show();
            basicattrtoggle.isOn = true;
            SetValue();
        }

        public override void Hide()
        {
            base.Hide();
            _UnloadShowContent();
   
        }  

        private void OnseniorattrtoggleValueChanged(bool isOn)
        {
            basicattrView.SetActive(!isOn);
            seniorattrView.SetActive(isOn);
        }

        private void OnbasicattrtoggleValueChanged(bool isOn)
        {
            basicattrView.SetActive(isOn);
            seniorattrView.SetActive(!isOn);
        }

        #region ModelShow
        private void OnCreateModel()
        {
            _LoadShowScene();
            _LoadShowModel(evt.roleBase.Career);
            heroLoader.LoadWeaponPart(evt.weaponFashionId, evt.weaponItemId);
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
            heroLoader.LoadHero(evt.roleBase.HeroId, evt.weaponItemId, ELayerMask.ModelShow, evt.fashoinDic, (go) =>
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
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(evt.roleBase.Career);
                uint highId = Hero.GetOtherHeroHighModelAnimationID(evt.roleBase.HeroId,evt.fashoinDic);
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId,evt.weaponItemId);

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


        private void OnAddExp()
        {
            level.text =evt.roleBase.Level.ToString();
            csvCharacterAttributeDataNextLevel = CSVCharacterAttribute.Instance.GetConfData(evt.roleBase.Level + 1);
            OnGradeUpdate();
            if (csvCharacterAttributeDataNextLevel != null)
            {
                exp.gameObject.SetActive(true);
                expSlider.gameObject.SetActive(true);
                exp.text = LanguageHelper.GetTextContent(2009377, evt.roleBase.Exp.ToString(), csvCharacterAttributeDataNextLevel.upgrade_exp.ToString());
                expSlider.value = (float)evt.roleBase.Exp / (float)csvCharacterAttributeDataNextLevel.upgrade_exp;
                compensationExpSlider.gameObject.SetActive(false);       
            }
            else
            {
                exp.gameObject.SetActive(false);
                expSlider.gameObject.SetActive(false);
            }
        }

        public void SetValue()
        {
            playerName.text = evt.roleBase.Name.ToStringUtf8();
            OnAddExp();
            CharacterHelper.SetHeadAndFrameData(roleicon,evt.roleBase.HeroId,evt.headId,evt.headFrameId);
            DefaultBasicAttr();
            DefaultSeniorAttr();
            AddBasicList();
            AddSeniorList();
            OnCreateModel();          
        }

        private void OnGradeUpdate()
        {
            playerGrade.text =evt.rolePower.ToString();
            comprehensiveGrade.text = evt.power.ToString();
        }

        private void AddSeniorList()
        {
            SetSeniorAttrType();
            seniorlist.Clear();
            for (int i = 0; i < seniorType.Count; ++i)
            {
                if (seniorattrList.transform.parent.transform.Find(seniorType[i].ToString()) == null)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(seniorattrList, seniorattrList.transform.parent);
                    go.transform.name = seniorType[i].ToString();
                    GameObject attrItem = go.transform.Find("Attr_Grid/Image_Attr").gameObject;
                    Text name = go.transform.Find("Title_Tips01/Text_Title").GetComponent<Text>();
                    if (seniorType[i] == 21)
                    {
                        name.text = LanguageHelper.GetTextContent(4419);
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
                    for (int j = 0; j < evt.pkAttrsId.Count; ++j)
                    {
                        uint id = evt.pkAttrsId[j];
                        CSVAttr.Data data = CSVAttr.Instance.GetConfData(id);
                        if (data.attr_type == seniorType[i] && data.isShow == 1)
                        {
                            GameObject attrgo = GameObject.Instantiate<GameObject>(attrItem, attrItem.transform.parent);
                            UI_Friend_Attr seniorAttr = new UI_Friend_Attr(id, false);
                            seniorAttr.Init(attrgo.transform);
                            seniorAttr.RefreshItem(evt.pkAttrs[id],0, j % 2 == 1);
                            seniorlist.Add(seniorAttr);
                        }
                    }
                    attrItem.SetActive(false);
                }
            }
            seniorattrList.SetActive(false);
        }

        private void AddBasicList()
        {
            elelist.Clear();
            basiclist.Clear();
            for (int j = 0; j < evt.pkAttrsId.Count; ++j)
            {
                uint id = evt.pkAttrsId[j];
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(id);              
                if (data.isShow == 1 && data.attr_type == 1)
                {          
                    GameObject go = GameObject.Instantiate<GameObject>(basicattrGo, basicattrGo.transform.parent);
                    UI_Friend_Attr basicAttr = new UI_Friend_Attr(id, false);
                    basicAttr.Init(go.transform);
                    basicAttr.RefreshItem(evt.pkAttrs[id],0, j % 2 == 1);
                    basiclist.Add(basicAttr);
                }
                else if (data.attr_type == 4 && data.isShow == 1)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(firstattrGo, firstattrGo.transform.parent);
                    UI_Friend_Attr eleAttr = new UI_Friend_Attr(id, true);
                    eleAttr.Init(go.transform);
                    eleAttr.RefreshItem(evt.pkAttrs[id], evt.baseAttrs2AttrAssigh[id], j % 2 == 0);
                    elelist.Add(eleAttr);
                }
            }

            firstattrGo.SetActive(false);
            basicattrGo.SetActive(false);
        }

        private void DefaultBasicAttr()
        {
            firstattrGo.SetActive(true);
            basicattrGo.SetActive(true);
            for (int i = 0; i < elelist.Count; ++i)
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
            uint type = 0;
            for (int j = 0; j < evt.pkAttrsId.Count; ++j)
            {
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(evt.pkAttrsId[j]);
                if (data.isShow == 1 && type != data.attr_type && data.attr_type != 1 && data.attr_type != 3 && data.attr_type != 4 && data.attr_type != 0)
                {
                    type = data.attr_type;
                    seniorType.Add(type);
                }
            }
        }

        #endregion
    }

    public class UI_Friend_Attribute : UIBase, UI_Friend_Attribute_Layout.IListener
    {
        private UI_Friend_Attribute_Layout layout = new UI_Friend_Attribute_Layout();

        private Dictionary<EFriendInfoViewType, UIComponent> dictOpPanel;
        private int defaultType;
        private int curERoleViewType;
        private FriendInfoEvt evt;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            layout.viewAttr = new UI_Friend_AttrView(evt);
            layout.viewAttr.Init(layout.attrView.transform);
            dictOpPanel = new Dictionary<EFriendInfoViewType, UIComponent>();
            dictOpPanel.Add(EFriendInfoViewType.ViewRoleAttr, layout.viewAttr);

            if (evt.clientPet != null)
            {
                layout.viewPetDetail = new UI_Pet_Detail_View(evt.clientPet);
                layout.viewPetDetail.Init(layout.petView.transform);
                dictOpPanel.Add(EFriendInfoViewType.ViewPetInfo, layout.viewPetDetail);
                layout.leftTabs.tabGoList[1].SetActive(true);
            }
            else
            {
                layout.leftTabs.tabGoList[1].SetActive(false);
            }
        }

        protected override void OnOpen(object arg)
        {
            if (arg == null)
            {
                return;
            }
            else
            {
                evt = (FriendInfoEvt)arg;
            }
            defaultType = 1;
        }
        protected override void OnShow()
        {            
            if (curERoleViewType != 0)
            {
                defaultType = curERoleViewType;
            }
            layout.leftTabs.OnDefaultSelect(defaultType);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Attr.Instance.eventEmitter.Handle<EFriendInfoViewType>(Sys_Attr.EEvents.OnSelectFriendInfoViewType, OnSelectViewType, toRegister);
        }

        protected override void OnHide()
        {
            foreach (var data in dictOpPanel)
            {
                data.Value.Hide();             
            }
        }

        protected override void OnDestroy()
        {
            layout.viewPetDetail?.OnDestroy();
        }

        private void OnSelectViewType(EFriendInfoViewType _type)
        {
            foreach (var data in dictOpPanel)
            {
                if (data.Key == _type)
                {
                    data.Value.Show();
                }
                else
                {
                    data.Value.Hide();
                }
            }
            curERoleViewType = (int)_type;
        }

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Friend_Attribute);
        }
    }
}
