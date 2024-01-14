using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using UnityEngine.EventSystems;
using Lib.Core;
using Packet;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;

namespace Logic
{
   /* public class UI_Pet_MagicDeeds_Layout 
    {
        public Transform transform;
        public Button closeBtn;
        public GameObject deviceGo;
        public Text boxname;

        public RawImage rawImage;
        public Image eventImage;
        public Button addpetBtn;
        public Button partnericon01;
        public Button partnericon02;

        public Toggle skillToggle;
        public Toggle intensifyToggle;
        public Toggle breakToggle;

        public GameObject skillpageGo;
        public GameObject intensifypageGo;
        public GameObject breakpageGo;

        public GameObject skillGo;
        public Button practiceBtn;
        public Text level;
        public Slider expSlider;
        public Text exp;
        public Text point;

        public GameObject resattrGo;
        public GameObject ameattrGo;
        public Button okBtn;
        public Button addexpBtn;
        public Button resetBtn;
        public Button mageicRuletBtn;
        public GameObject btnGo;

        public GameObject strongGo;
        public GameObject lockedGo;
        public GameObject messageGo;
        public Text unlockname;
        public Text unlockpoint;
        public Button unlockBtn;
        public GameObject skillviewGo;

        public Image activedeviceicon;
        // public GameObject Fx_AddQihe;
        // public GameObject Fx_ui_trail_01;

        public GameObject battleGo;
        public Text grade;
        public Text unlockNumber;
        public Text unlockTip;
        public GameObject attrGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            deviceGo=transform.Find("Scroll_View/Grid/Item01").gameObject;
            boxname= transform.Find("View_Message/View_Left/Image_Namebg/Text_Name").GetComponent<Text>();
            rawImage= transform.Find("View_Message/View_Left/Charapter").GetComponent<RawImage>();
            eventImage= transform.Find("View_Message/View_Left/EventImage").GetComponent<Image>();
            addpetBtn = transform.Find("View_Message/View_Left/Btn_Add").GetComponent<Button>();
            partnericon01 = transform.Find("View_Message/View_Left/Grid_State/StateItem/Button_Icon").GetComponent<Button>();
            partnericon02= transform.Find("View_Message/View_Left/Grid_State/StateItem (1)/Button_Icon").GetComponent<Button>();
            skillToggle= transform.Find("View_Message/View_Right/Menu/ListItem").GetComponent<Toggle>();
            intensifyToggle = transform.Find("View_Message/View_Right/Menu/ListItem (1)").GetComponent<Toggle>();
            breakToggle = transform.Find("View_Message/View_Right/Menu/ListItem (2)").GetComponent<Toggle>();

            skillpageGo = transform.Find("View_Message/View_Right/Page02").gameObject;
            intensifypageGo = transform.Find("View_Message/View_Right/Page01").gameObject;
            breakpageGo = transform.Find("View_Message/View_Right/Page03").gameObject;
            messageGo = transform.Find("View_Message").gameObject;

            skillGo = transform.Find("View_Message/View_Right/Page02/Scroll_View_Skill/Grid/Item").gameObject;
            practiceBtn = transform.Find("View_Message/View_Right/Page02/Btn_01").GetComponent<Button>(); ;
            level= transform.Find("View_Message/View_Right/Page01/Strong/Text_Strong/Text_Num").GetComponent<Text>();
            expSlider= transform.Find("View_Message/View_Right/Page01/Strong/Slider_Exp").GetComponent<Slider>();
            exp= transform.Find("View_Message/View_Right/Page01/Strong/Text_Percent").GetComponent<Text>();
            point= transform.Find("View_Message/View_Right/Page01/Text_Reset/Text_Num").GetComponent<Text>();
            resattrGo = transform.Find("View_Message/View_Right/Page01/Scroll_View/Attr_Grid/Attr01").gameObject;
            //ameattrGo =transform.Find("Animator/View_Messag/View_Right/Page02/Scroll_View/Attr_Grid/Ame_Attr_Group/Attr_Grid/Attr01").gameObject;
            okBtn = transform.Find("View_Message/View_Right/Page01/View_Btn/Button_OK").GetComponent<Button>();
            addexpBtn = transform.Find("View_Message/View_Right/Page01/Strong/Button_Add").GetComponent<Button>();
            resetBtn = transform.Find("View_Message/View_Right/Page01/View_Btn/Btn_01").GetComponent<Button>();
            strongGo = transform.Find("View_Message/View_Right/Page01/Scroll_View_Item").gameObject;
            btnGo = transform.Find("View_Message/View_Right/Page01/View_Btn").gameObject;
            skillviewGo = transform.Find("View_Message/View_Right/Page01/Scroll_View").gameObject;

            lockedGo = transform.Find("View_Lock").gameObject;
            unlockname= transform.Find("View_Lock/Image_Bg (1)/Text_Name").GetComponent<Text>();
            unlockpoint = transform.Find("View_Lock/Text_Name (1)").GetComponent<Text>();
            unlockBtn = transform.Find("View_Lock/Btn_01").GetComponent<Button>();

            activedeviceicon = transform.Find("View_Lock/Item01/Image_Icon").GetComponent<Image>();  
            // Fx_AddQihe = transform.Find("Animator/View_Messag/View_Left/Image_Qihe/Fx_AddQihe").gameObject;
            //Fx_ui_trail_01 = transform.Find("Fx_ui_trail_01").gameObject;
            battleGo = transform.Find("View_Message/View_Left/Image").gameObject;
            grade = transform.Find("View_Message/View_Left/Text_Score").GetComponent<Text>();

            unlockNumber = transform.Find("View_Lock/Text_No").GetComponent<Text>();
            unlockTip = transform.Find("View_Lock/Text_Demand").GetComponent<Text>();
            attrGo = transform.Find("View_Message/View_Left/Score/Image_Bottom").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            addpetBtn.onClick.AddListener(listener.OnaddpetBtnClicked);
            partnericon01.onClick.AddListener(listener.Onpartnericon01Clicked);
            partnericon02.onClick.AddListener(listener.Onpartnericon02Clicked);
            practiceBtn.onClick.AddListener(listener.OnpracticeBtnClicked);
            okBtn.onClick.AddListener(listener.OnokBtnClicked);
            addexpBtn.onClick.AddListener(listener.OnaddexpBtnClicked);
            resetBtn.onClick.AddListener(listener.OnresetBtnClicked);
            unlockBtn.onClick.AddListener(listener.OnunlockBtnClicked);
            skillToggle.onValueChanged.AddListener(listener.OnskillToggleChanged);
            intensifyToggle.onValueChanged.AddListener(listener.OnintensifyToggleChanged);
            breakToggle.onValueChanged.AddListener(listener.OnbreakToggleChanged);
        }

        public interface IListener
        {
            void OnaddBtnClicked();
            void OnresetBtnClicked();
            void Onpartnericon01Clicked();
            void Onpartnericon02Clicked();
            void OnpracticeBtnClicked();
            void OncancelBtnClicked();
            void OnokBtnClicked();
            void OnaddexpBtnClicked();
            void OnaddpetBtnClicked();
            void OnunlockBtnClicked();
            void OnskillToggleChanged(bool arg0);
            void OnintensifyToggleChanged(bool arg0);
            void OnbreakToggleChanged(bool arg0);
        }
    }

    public class UI_Pet_Device : UIComponent
    {
        private bool islock;
        private bool haspet;
        public uint id;
        private uint iconId;
        private ClientPet clientpet;
        private DeviceUnit deviceuint;

        private Text number;
        private Image peticon;
        private Image deviceicon;
        private Button lockBtn;
        private Button boxBtn;
        private GameObject selectGo;

        public UI_Pet_Device(uint _id) : base()
        {
            id = _id;
        }

        protected override void Loaded()
        {
            peticon = transform.Find("Image_Icon").GetComponent<Image>();
            deviceicon = transform.Find("Image_Icon1").GetComponent<Image>();
            number = transform.Find("Image_Level/Text_Level").GetComponent<Text>();
            lockBtn = transform.Find("Image_Lock").GetComponent<Button>();
            lockBtn.onClick.AddListener(OnlockBtnClicked);
            boxBtn = transform.Find("ItemBg").GetComponent<Button>();
            boxBtn.onClick.AddListener(OnboxBtnClicked);
            selectGo = transform.Find("Image_Select01").gameObject;
        }

        public void RefreshItem(uint boxid, uint petid,uint selectboxid,bool islocked)
        {
            if (petid == 0) { haspet = true; }
            else { haspet = false; }
            islock = islocked;
            number.text = LanguageHelper.GetTextContent(2009465, boxid);
            lockBtn.gameObject.SetActive(islock);
            boxBtn.enabled = !islock;
            SetSelectDevice(selectboxid);
            if (!haspet)

            {
                peticon.gameObject.SetActive(true);
                deviceicon.gameObject.SetActive(false);
                ImageHelper.SetIcon(peticon, CSVPet.Instance.GetConfData(petid).icon_id);
            }
            else
            {
                if (!islock)
                {
                    deviceicon.gameObject.SetActive(true);
                    ImageHelper.SetIcon(deviceicon, null, CSVPetDevices.Instance.GetConfData(boxid).icon,false);
                }
                else
                {
                    deviceicon.gameObject.SetActive(false);
                }
                peticon.gameObject.SetActive(false);
            }
        }

        public void SetSelectDevice(uint selectboxid)
        {
            if (id == selectboxid)
            {
                selectGo.SetActive(true);
            }
            else
            {
                selectGo.SetActive(false);
            }
        }

        //已激活
        private void OnboxBtnClicked()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChooseDeviceCell, id);
        }     

        private void OnlockBtnClicked()
        {
            Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnUnLockedDecevice, id);
        }
    }

    public class UI_Pet_MagicDeeds_Skill : UIComponent
    {
        private uint id;
        private Image skillicon;
        private Text message;
        private Button skillbtn;
        private Image quality;
        public GameObject Fx_ui_Select03;
        private CSVPassiveSkillInfo.Data csvPassiveSkillInfo;

        public UI_Pet_MagicDeeds_Skill( uint _id) : base()
        {
            id = _id;
        }

        protected override void Loaded()
        {
            skillicon = transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>();
            quality = transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>();
            message = transform.Find("Text_Tips").GetComponent<Text>();
            skillbtn = transform.Find("PetSkillItem01/Image_Bg").GetComponent<Button>();
            skillbtn.onClick.AddListener(OnskillbtnClicked);
            Fx_ui_Select03 = transform.Find("PetSkillItem01/Image_Skill/Fx_ui_Select03").gameObject;
        }

        private void OnskillbtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, id);
        }

        public void RefreshItem(uint skillld)
        {
            if (!CSVPassiveSkillInfo.Instance.GetDictData().ContainsKey(skillld))
            {
                return;
            }
            csvPassiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillld);
            ImageHelper.SetIcon(skillicon, csvPassiveSkillInfo.icon);
            message.text = LanguageHelper.GetTextContent(csvPassiveSkillInfo.name);
            ImageHelper.GetPetSkillQuality_Frame(quality, (int)csvPassiveSkillInfo.quality);
        }
    }

    public class UI_Pet_Attr_Item : UIComponent
    {
        private uint id;
        private uint num;
        private uint strpoint;
        private uint freepoint;
        public uint addpointnum;
        public uint maxAddPoint;
        public DeviceStrengthen deviceStrengthen;

        private DeviceUnit device;
        private Text attrname;
        private GameObject normalGo;
        private GameObject addpointGo;
        private Text number;
        private Text point;
        private Text addpoint;
        private Button addbtn;
        private Button subbtn;

        public UI_Pet_Attr_Item(uint _id, DeviceUnit _device) : base()
        {
            id = _id;

            device = _device;
        }

        protected override void Loaded()
        {        
            attrname = transform.Find("Text").GetComponent<Text>();
            number = transform.Find("View01/Text_Num").GetComponent<Text>();
            point = transform.Find("View01/Text_Num/Text_Point").GetComponent<Text>();
            addpoint = transform.Find("View02/InputField_Number/Placeholder").GetComponent<Text>();
            normalGo = transform.Find("View01").gameObject;
            addpointGo = transform.Find("View02").gameObject;
            addbtn = transform.Find("View02/Btn_Add").GetComponent<Button>();
            addbtn.onClick.AddListener(OnaddbtnClick);
            subbtn = transform.Find("View02/Btn_Min").GetComponent<Button>();
            subbtn.onClick.AddListener(OnsubbtnClick);
            deviceStrengthen = new DeviceStrengthen();
            deviceStrengthen.AttrId = id;
        }

        private void OnsubbtnClick()
        {
            if (Sys_Pet.Instance.adddevicestrengthpoint <= 0|| addpointnum==0)
            {
                return;
            }
            else
            {
                addpointnum--;
                addpoint.text = addpointnum.ToString();
                deviceStrengthen.Point = addpointnum;
                Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnAddDeviceFreePoint);
                if (CSVAttr.Instance.GetConfData(id).show_type == 1)
                {
                    point.text = LanguageHelper.GetTextContent(2007705, addpointnum);
                }
                else if (CSVAttr.Instance.GetConfData(id).show_type == 2)
                {
                    point.text = LanguageHelper.GetTextContent(2007705, addpointnum / 100);
                }
            }
        }

        private void OnaddbtnClick()
        {
            if (Sys_Pet.Instance.adddevicestrengthpoint >= freepoint)
            {
                return;
            }
            else
            {
                for(int i=0;i< CSVPetDevicesSurmount.Instance.GetConfData(device.BreakCount + 1).add_max_attr_num.Count;++i)
                {
                    if (CSVPetDevicesSurmount.Instance.GetConfData(device.BreakCount + 1).add_max_attr_num[i][0] == id)
                    {
                        maxAddPoint = CSVPetDevicesSurmount.Instance.GetConfData(device.BreakCount + 1).add_max_attr_num[i][1];
                    }
                }
                uint preAdd = addpointnum+1;
                if (preAdd+ strpoint > maxAddPoint)
                {
                    Sys_Hint.Instance.PushContent_Normal( LanguageHelper.GetTextContent(101540));
                }
                else
                {
                    addpointnum++;
                    addpoint.text = addpointnum.ToString();
                    if (CSVAttr.Instance.GetConfData(id).show_type == 1)
                    {
                        point.text = LanguageHelper.GetTextContent(2007705, addpointnum);
                    }
                    else if (CSVAttr.Instance.GetConfData(id).show_type == 2)
                    {
                        point.text = LanguageHelper.GetTextContent(2007704, addpointnum / 100f);
                    }

                    deviceStrengthen.Point = addpointnum;
                    Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnAddDeviceFreePoint);
                }
            }
        }

        public void RefreshItem(uint id, uint renum, uint restrpoint, uint refreepoint)
        {
            num = renum;
            strpoint = restrpoint;
            freepoint = refreepoint;
            Sys_Pet.Instance.adddevicestrengthpoint = 0;
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name);
            normalGo.SetActive(true);
            if (CSVAttr.Instance.GetConfData(id).show_type == 1)
            {
                number.text = renum.ToString();
            }
            else if (CSVAttr.Instance.GetConfData(id).show_type == 2)
            {
                number.text = LanguageHelper.GetTextContent(2009361, (float)(renum + restrpoint) / 100);
            }
            point.text = string.Empty;
            if (refreepoint == 0)
            {
                addpointGo.SetActive(false);
            }
            else
            {
                addpointGo.SetActive(true);
                //addpercent.text = LanguageHelper.GetTextContent(2009361,(float)(renum + restrpoint) / 100);
            }
        }
    }

    public class UI_Pet_MagicDeeds : UIComponent, UI_Pet_MagicDeeds_Layout.IListener
    {
        private UI_Pet_MagicDeeds_Layout layout = new UI_Pet_MagicDeeds_Layout();
        private List<UI_Pet_Device> devicesList = new List<UI_Pet_Device>();
        private Dictionary<uint, Transform> devicetrans = new Dictionary<uint, Transform>();
        private List<UI_Pet_MagicDeeds_Skill> skillsList = new List<UI_Pet_MagicDeeds_Skill>();
        private List<UI_Pet_Attr_Item> resattrList = new List<UI_Pet_Attr_Item>();
        private Dictionary<uint, UI_Pet_Attr_Item> resattrDic = new Dictionary<uint, UI_Pet_Attr_Item>();
        //private List<UI_Pet_Attr_Item> ameattrList = new List<UI_Pet_Attr_Item>();
        private Dictionary<uint, uint> devicestrengthen = new Dictionary<uint, uint>();
       // List<DeviceStrengthen> addpointlist = new List<DeviceStrengthen>();
      
        private ClientPet curclientPet;
        private uint curdeviceid;
        private CSVPet.Data curCsvData;
        //private GameObject model;        
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        //private GameObject clone;
        private GameObject magicroot;
        private GameObject fxbreak;
        private AsyncOperationHandle<GameObject> mHandle;
        private Animator lockanimator;
        private Timer lockedtimer;
        private Animator addanimator;
        private Timer fxtimer;

        protected override void Loaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
          
            lockanimator = layout.lockedGo.GetComponent<Animator>();
            addanimator = layout.addpetBtn.GetComponent<Animator>();
        }

        public void ShowView(uint deviceId=1)
        {
            gameObject.SetActive(true);
            curdeviceid = deviceId;
            AddDevicesList();
          //  layout.Fx_AddQihe.SetActive(false);
            if (Sys_Pet.Instance.devicesdic.ContainsKey(curdeviceid))
            {
                layout.lockedGo.SetActive(false);
                layout.messageGo.SetActive(true);
                layout.skillToggle.isOn = true;
                OnChooseDeviceCell(curdeviceid);
            }
            else
            {
                layout.messageGo.SetActive(false);
                OnUnLockedDecevice(curdeviceid);
            }
            ProcessEventsForEnable(true);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnChooseDeviceCell, OnChooseDeviceCell, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnUnLockedDecevice, OnUnLockedDecevice, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnChangeDeviceStrengthenExp, OnChangeDeviceStrengthenExp, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnAddDeviceFreePoint, OnAddDeviceFreePoint, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnRefreshDevicePoint, OnRefreshDevicePoint, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangePostion, OnChangePostion, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnDeviceResetAddPoint, OnDeviceResetAddPoint, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnActiveDevice, OnActiveDevice, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnChangeDeviceExp, OnChangeDeviceExp, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnDeviceReplaceSkill, OnDeviceReplaceSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnCloseSelectPet, OnCloseSelectPet, toRegister);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(layout.eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
        }

        public override void Hide()
        {
            ProcessEventsForEnable(false);
            DefaultItem();
            DefaultAttrItem();
            curclientPet = null;
            curdeviceid = 0;
            UnloadModel();
            fxtimer?.Cancel();
            lockedtimer?.Cancel();
            CameraManager.mCamera.gameObject.SetActive(true);
            Sys_Pet.Instance.adddevicestrengthpoint = 0;
            for (int i = 0; i < resattrList.Count; ++i) { resattrList[i].addpointnum = 0; }
            gameObject.SetActive(false);
        }

        public override void OnDestroy()
        {
        }

        #region CallBack
        private void OnChooseDeviceCell(uint deviceid)
        {
            layout.messageGo.SetActive(true);
            layout.lockedGo.SetActive(false);
            layout.activedeviceicon.transform.localScale = new Vector3(1, 1, 1);
            curdeviceid = deviceid;
            DeviceUnit deviceunit = Sys_Pet.Instance.devicesdic[deviceid];
            if (Sys_Pet.Instance.device2cilentpet.ContainsKey(deviceunit))
            {
                curclientPet = Sys_Pet.Instance.device2cilentpet[deviceunit];
                layout.addpetBtn.gameObject.SetActive(false);
            }
            else
            {
                curclientPet = null;
                layout.addpetBtn.gameObject.SetActive(true);
            }
            SetModelValue();
            AddSkillItem(curdeviceid);
            GetDeviceStrengthen(curdeviceid);
            SetIntensifyShow(curdeviceid);
            AddResAttr(curdeviceid);
            layout.boxname.text =LanguageHelper.GetTextContent(CSVPetDevices.Instance.GetConfData(curdeviceid).name);
            //layout.breakpoint.text = deviceunit.Exp.ToString();
            layout.skillToggle.isOn = true;
            for (int i=0;i< devicesList.Count;++i)
            {
                devicesList[i].SetSelectDevice(curdeviceid);
            }
            SetPetPkAttr();
        }

        private void OnUnLockedDecevice(uint id)
        {
            CSVPetDevices.Data csvPetDevicesData = CSVPetDevices.Instance.GetConfData(id);
            curdeviceid = id;
            layout.lockedGo.SetActive(false);
            layout.lockedGo.SetActive(true);
            layout.messageGo.SetActive(false);
            layout.unlockname.text= LanguageHelper.GetTextContent(csvPetDevicesData.name);
            layout.unlockpoint.text = LanguageHelper.GetTextContent(2009394, LanguageHelper.GetTextContent(2009395));
            layout.unlockNumber.text = LanguageHelper.GetTextContent(2009465, id);
            layout.unlockTip.text = LanguageHelper.GetTextContent(10678, csvPetDevicesData.level_actication,LanguageHelper.GetTextContent( CSVItem.Instance.GetConfData(csvPetDevicesData.coin_actication[0]).name_id), csvPetDevicesData.coin_actication[1]);
            ImageHelper.SetIcon(layout.activedeviceicon, null, csvPetDevicesData.activation_icon, false);
            for (int i = 0; i < devicesList.Count; ++i)
            {
                devicesList[i].SetSelectDevice(curdeviceid);
            }
        }

        private void OnChangeDeviceStrengthenExp(uint changeid)
        {
            GetDeviceStrengthen(curdeviceid);
            SetIntensifyShow(changeid);
            AddResAttr(changeid);
        }

        private void OnChangePostion()
        {
            DefaultItem();
            AddDevicesList();
            OnChooseDeviceCell(curdeviceid);
        }

        private void OnActiveDevice()
        {
            lockanimator.Play("Activation",-1,0);
            lockedtimer?.Cancel();
            uint targrtdeviceid = curdeviceid;
            lockedtimer = Timer.Register(1.664f, () =>
            {
                PlayFx(targrtdeviceid);
                lockanimator.enabled = false;
                lockedtimer.Cancel();
            },null,false,false);
        }

        private void PlayFx(uint targrtdeviceid)
        {
           UI_Pet_Device device = null;
         //  layout.Fx_ui_trail_01.SetActive(true);
            for (int i = 0; i < devicesList.Count; ++i)
            {
                if (targrtdeviceid == devicesList[i].id)
                {
                    device = devicesList[i];
                }
            }
            float fscale = 0.1782f;
           // DOTween.To(() => layout.Fx_ui_trail_01.transform.position, x => layout.Fx_ui_trail_01.transform.position = x, devicetrans[targrtdeviceid].position, 0.7f);
            DOTween.To(() =>layout.activedeviceicon.transform.position, x => layout.activedeviceicon.transform.position = x, devicetrans[targrtdeviceid].position, 0.7f);
            DOTween.To(() => layout.activedeviceicon.transform.localScale, x => layout.activedeviceicon.transform.localScale = x, new Vector3(fscale, fscale, fscale),0.7f);
            fxtimer?.Cancel();
            fxtimer = Timer.Register(0.7f, () =>
            {
                ActiveDeviceShow(targrtdeviceid);
               // layout.Fx_ui_trail_01.transform.position = new Vector3(100, 100, 0);
              //  layout.Fx_ui_trail_01.SetActive(false);
                lockanimator.enabled = true;
                fxtimer.Cancel();
            },null, false, false);
        }

        private void ActiveDeviceShow(uint showdeviceid)
        {
            for (int i = 0; i < devicesList.Count; ++i)
            {
                if (showdeviceid == devicesList[i].id)
                {
                    devicesList[i].RefreshItem(showdeviceid, 0, curdeviceid, false);
                }
            }
            if (CSVPetDevices.Instance.GetDictData().ContainsKey(showdeviceid + 2))
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.deviceGo, layout.deviceGo.transform.parent);
                go.SetActive(true);
                UI_Pet_Device device = new UI_Pet_Device( showdeviceid + 2);
                device.Init(go.transform);
                device.RefreshItem(showdeviceid + 2, 0, showdeviceid, true);
                devicesList.Add(device);
                devicetrans.Add(showdeviceid + 2, go.transform);
            }
            if(curdeviceid== showdeviceid)
            OnChooseDeviceCell(curdeviceid);
        }

        private void OnRefreshDevicePoint(uint id)
        {
            devicestrengthen.Clear();
            for (int i=0;i< Sys_Pet.Instance.devicestrengthpoints.Count;++i)
            {
                devicestrengthen.Add(Sys_Pet.Instance.devicestrengthpoints[i].AttrId, Sys_Pet.Instance.devicestrengthpoints[i].Point);
            }
            SetIntensifyShow(id);
            AddResAttr(id);
        }

        private void OnAddDeviceFreePoint()
        {
            uint number = 0;
            for (int i=0;i< resattrList.Count; ++i)
            {
                number += resattrList[i].deviceStrengthen.Point;
            }
            Sys_Pet.Instance.adddevicestrengthpoint = number;
            layout.point.text = (Sys_Pet.Instance.devicesdic[curdeviceid].StrengthenFreePoint - number).ToString();
        }

        private void OnDeviceResetAddPoint(uint id)
        {
            Sys_Pet.Instance.adddevicestrengthpoint = 0;
            layout.point.text = Sys_Pet.Instance.devicesdic[id].StrengthenFreePoint.ToString();
            layout.btnGo.SetActive(true);
            layout.skillviewGo.GetComponent<RectTransform>().sizeDelta = new Vector2(455, 244);
            devicestrengthen.Clear();
            AddResAttr(id);
        }

        private void OnChangeDeviceExp()
        {
          // // layout.breakpoint.text = Sys_Pet.Instance.devicesdic[curdeviceid].Exp.ToString();
          ////  if (layout.Fx_AddQihe.activeInHierarchy)
          //  {
          //      layout.Fx_AddQihe.SetActive(false);
          //  }
          //  layout.Fx_AddQihe.SetActive(true);
        }

        private void OnDeviceReplaceSkill()
        {
            AddSkillItem(curdeviceid);
        }

        private void OnCloseSelectPet()
        {
            addanimator.enabled = true;
        }
        #endregion

        #region Function
        private void SetModelValue()
        {
            if (curclientPet == null)
            {
                UnloadModel();                
                _LoadShowScene();
            }
            else
            {
                UnloadModel();
                _LoadShowScene();
                _LoadShowModel(curclientPet);

                curCsvData = CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId);
            }
            if (curclientPet!=null&& Sys_Pet.Instance.fightPet!=null&& Sys_Pet.Instance.fightPet == curclientPet.petUnit)
            {
                layout.battleGo.SetActive(true);
            }
            else
            {
                layout.battleGo.SetActive(false);
            }
        }

        private void SetIntensifyShow(uint id)
        {
            layout.point.text = Sys_Pet.Instance.devicesdic[id].StrengthenFreePoint.ToString();
            layout.level.text = Sys_Pet.Instance.devicesdic[id].StrengthenLv.ToString();
            layout.expSlider.value = (float)Sys_Pet.Instance.devicesdic[id].StrengthenExp / (float)CSVPetDevicesStrengthen.Instance.GetConfData(Sys_Pet.Instance.devicesdic[id].StrengthenLv + 1).exp;
            layout.exp.text = Sys_Pet.Instance.devicesdic[id].StrengthenExp + "/" + CSVPetDevicesStrengthen.Instance.GetConfData(Sys_Pet.Instance.devicesdic[id].StrengthenLv + 1).exp;
            if (Sys_Pet.Instance.devicesdic[id].StrengthenFreePoint == 0)
            {
                layout.btnGo.SetActive(false);
                layout.skillviewGo.GetComponent<RectTransform>().sizeDelta = new Vector2(455, 287);
            }
            else
            {
                layout.btnGo.SetActive(true);
                layout.skillviewGo.GetComponent<RectTransform>().sizeDelta = new Vector2(455, 244);
            }
        }

        private void GetDeviceStrengthen(uint id)
        {
            devicestrengthen.Clear();
            for (int i=0;i< Sys_Pet.Instance.devicesdic[id].StrengthenPoint.Count;++i)
            {
                devicestrengthen.Add(Sys_Pet.Instance.devicesdic[id].StrengthenPoint[i].AttrId, Sys_Pet.Instance.devicesdic[id].StrengthenPoint[i].Point);
            }
        }

        private uint GetDeviceAllStrengthen(uint id)
        {
            uint allpoint = 0;
            for (int i = 0; i < Sys_Pet.Instance.devicesdic[id].StrengthenPoint.Count; ++i)
            {
                allpoint += Sys_Pet.Instance.devicesdic[id].StrengthenPoint[i].Point;
            }
            return allpoint;
        }

        public void SetPetPkAttr()
        {
            for (int i = 0; i <layout.attrGo.transform.parent.childCount; i++)
            {
                if (i >= 1) GameObject.Destroy(layout.attrGo.transform.parent.GetChild(i).gameObject);
            }

            if (null != curclientPet)
            {
                layout.grade.gameObject.SetActive(false);
                layout.attrGo.SetActive(false);
                layout.attrGo.transform.parent.gameObject.SetActive(true);
                for (int i = 0; i < curclientPet.eleAttrs.Count; i++)
                {
                    EPkAttr tempEPk = curclientPet.eleAttrs[i];
                    CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(Sys_Pet.Instance.pkAttrs2Id[tempEPk]);
                    GameObject go = GameObject.Instantiate<GameObject>(layout.attrGo, layout.attrGo.transform.parent);
                    ImageHelper.SetIcon(go.transform.Find("Image_icon").GetComponent<Image>(), cSVAttrData.attr_icon);
                    TextHelper.SetText(go.transform.Find("Text_Num").GetComponent<Text>(), curclientPet.pkAttrs[tempEPk].ToString());
                    go.SetActive(true);
                }
            }
            else
            {
                layout.attrGo.transform.parent.gameObject.SetActive(false);
            }

        }

        private void AddDevicesList()
        {
            devicesList.Clear();
            devicetrans.Clear();
            foreach (var data in Sys_Pet.Instance.devicesdic)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.deviceGo, layout.deviceGo.transform.parent);
                DeviceUnit item = Sys_Pet.Instance.devicesdic[data.Key];
                if (Sys_Pet.Instance.device2cilentpet.ContainsKey(item))
                {
                    ClientPet pet = Sys_Pet.Instance.device2cilentpet[item];
                    UI_Pet_Device device = new UI_Pet_Device( data.Key);
                    device.Init(go.transform);
                    device.RefreshItem(data.Key, pet.petUnit.PetId, curdeviceid, false);
                    devicesList.Add(device);
                }
                else
                {
                    UI_Pet_Device device = new UI_Pet_Device(data.Key);
                    device.Init(go.transform);
                    device.RefreshItem(data.Key, 0, curdeviceid, false);
                    devicesList.Add(device);
                }
                devicetrans.Add(data.Key, go.transform);
            }
            int count = 0;
            if (Sys_Pet.Instance.devicesdic.Count <= CSVPetParameter.Instance.GetConfData(19).value-2)
            {
                count = 2;
            }else if(Sys_Pet.Instance.devicesdic.Count == CSVPetParameter.Instance.GetConfData(19).value - 1)
            {
                count = 1;
            }
            if (Sys_Pet.Instance.devicesdic.Count < CSVPetParameter.Instance.GetConfData(19).value)
            {
                for (int i = 1; i < count + 1; ++i)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.deviceGo, layout.deviceGo.transform.parent);
                    UI_Pet_Device device = new UI_Pet_Device( (uint)(Sys_Pet.Instance.devicesdic.Count + i));
                    device.Init(go.transform);
                    device.RefreshItem((uint)(Sys_Pet.Instance.devicesdic.Count + i), 0, curdeviceid, true);
                    devicesList.Add(device);
                    devicetrans.Add((uint)(Sys_Pet.Instance.devicesdic.Count + i), go.transform);
                }
            }            
            layout.deviceGo.SetActive(false);
        }

        private void AddSkillItem(uint id)
        {
            DefaultSkillItem();
            skillsList.Clear();
            for (int i=0;i< Sys_Pet.Instance.devicesdic[id].SkillList.Count;++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.skillGo, layout.skillGo.transform.parent);
                UI_Pet_MagicDeeds_Skill skill = new UI_Pet_MagicDeeds_Skill( Sys_Pet.Instance.devicesdic[id].SkillList[i]);
                skill.Init(go.transform);
                skill.RefreshItem(Sys_Pet.Instance.devicesdic[id].SkillList[i]);
                skillsList.Add(skill);
            }
            layout.skillGo.SetActive(false);
        }

        private void AddResAttr(uint id)
        {
            DefaultAttrItem();
            resattrList.Clear();
            CSVPetDevices.Data data = CSVPetDevices.Instance.GetConfData(id);
            for (int i=0;i< data.device_attr.Count;++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.resattrGo, layout.resattrGo.transform.parent);
                if(devicestrengthen.ContainsKey(data.device_attr[i][0])&& devicestrengthen[data.device_attr[i][0]] != 0)
                {
                    UI_Pet_Attr_Item item = new UI_Pet_Attr_Item( data.device_attr[i][0], Sys_Pet.Instance.devicesdic[id]);
                    item.Init(go.transform);
                    item.RefreshItem(data.device_attr[i][0], data.device_attr[i][1], devicestrengthen[data.device_attr[i][0]], Sys_Pet.Instance.devicesdic[curdeviceid].StrengthenFreePoint);
                    resattrList.Add(item);
                }
                else
                {
                    UI_Pet_Attr_Item item = new UI_Pet_Attr_Item( data.device_attr[i][0], Sys_Pet.Instance.devicesdic[id]);
                    item.Init(go.transform);
                    item.RefreshItem(data.device_attr[i][0], data.device_attr[i][1], 0, Sys_Pet.Instance.devicesdic[curdeviceid].StrengthenFreePoint);
                    resattrList.Add(item);
                }
            }
            layout.resattrGo.SetActive(false);
        }

        private void DefaultItem()
        {
            layout.deviceGo.SetActive(true);
            for (int i=0;i< devicesList.Count;++i) { devicesList[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.deviceGo.transform.parent.gameObject, layout.deviceGo.transform.name);
        }

        private void DefaultSkillItem()
        {
            layout.skillGo.SetActive(true);
            for (int i = 0; i < skillsList.Count; ++i) { skillsList[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.skillGo.transform.parent.gameObject, layout.skillGo.transform.name);
        }

        private void DefaultAttrItem()
        {
            layout.resattrGo.SetActive(true);
            for (int i = 0; i < resattrList.Count; ++i) { resattrList[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.resattrGo.transform.parent.gameObject, layout.resattrGo.transform.name);
        }
        #endregion

        #region Model
        public void UnloadModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            layout.rawImage.texture = null;
            petDisplay?.Dispose();
            showSceneControl?.Dispose();
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }
            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[1] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            if (curclientPet != null)
            {
                sceneModel.transform.localPosition = new Vector3(10, 0, 0);
            }
            showSceneControl.Parse(sceneModel);
            sceneModel.transform.Find("bg").gameObject.SetActive(false);
            magicroot= sceneModel.transform.Find("Magic").gameObject;
            fxbreak = sceneModel.transform.Find("Fx_ui_Pet_Present").gameObject;
            //设置RenderTexture纹理到RawImage
            layout.rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
            if (CSVPetDevices.Instance.GetConfData(curdeviceid).device_model != "")
            {
                AddressablesUtil.LoadAssetAsync<GameObject>(ref mHandle, CSVPetDevices.Instance.GetConfData(curdeviceid).device_model, MHandle_Completed);
            }
            else
            {
                DebugUtil.LogFormat(ELogType.eNone, "魔契{0}device_model为null",curdeviceid);
            }
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            GameObject go = GameObject.Instantiate(handle.Result);
            go.transform.SetParent(magicroot.transform);
            go.transform.localPosition = Vector3.zero;
        }

        private void _LoadShowModel(ClientPet clientPet)
        {
            string _modelPath = CSVPet.Instance.GetConfData(clientPet.petUnit.PetId).model_show;

            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            showSceneControl.mModelPos.transform.localPosition = new Vector3(showSceneControl.mModelPos.transform.localPosition.x, showSceneControl.mModelPos.transform.localPosition.y+CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId).height, showSceneControl.mModelPos.transform.localPosition.z);
            showSceneControl.mModelPos.transform.localRotation = Quaternion.Euler(CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId).angle1, CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId).angle2, CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId).angle3);
            showSceneControl.mModelPos.transform.localScale = new Vector3((float)CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId).size, (float)CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId).size, (float)CSVPet.Instance.GetConfData(curclientPet.petUnit.PetId).size);
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                uint highId = curclientPet.petUnit.PetId;
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPet.Instance.GetConfData(highId).action_id_show, curCsvData.weapon);
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
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }

        #endregion

        #region ButtonClicked
        public void OnaddBtnClicked()
        {
            Sys_Pet.Instance.selectItemItem.Clear();
            Sys_Pet.Instance.selectItemItem.Add(CSVPetParameter.Instance.GetConfData(37).value);
            Sys_Pet.Instance.selectItemItem.Add(CSVPetParameter.Instance.GetConfData(38).value);
            Sys_Pet.Instance.selectItemItem.Add(CSVPetParameter.Instance.GetConfData(39).value);
            UIManager.OpenUI(EUIID.UI_SelectItem, false, curdeviceid);
        }

        public void OnaddexpBtnClicked()
        {
            Sys_Pet.Instance.selectItemItem.Clear();
            Sys_Pet.Instance.selectItemItem.Add(CSVPetParameter.Instance.GetConfData(36).value);
            Sys_Pet.Instance.selectItemItem.Add(0);
            UIManager.OpenUI(EUIID.UI_SelectItem, false,curdeviceid);
        }

        public void OnbreakBtnClicked()
        {
            if ( Sys_Role.Instance.Role.Level< CSVPetDevicesSurmount.Instance.GetConfData(Sys_Pet.Instance.devicesdic[curdeviceid].BreakCount + 2).need_lv)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009460, CSVPetDevicesSurmount.Instance.GetConfData(Sys_Pet.Instance.devicesdic[curdeviceid].BreakCount + 2).need_lv));
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2009462,LanguageHelper.GetTextContent( CSVPetDevices.Instance.GetConfData(curdeviceid).name), CSVPetDevicesSurmount.Instance.GetConfData(Sys_Pet.Instance.devicesdic[curdeviceid].BreakCount+2).cost_fit_value);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    if (Sys_Pet.Instance.devicesdic[curdeviceid].Exp >= CSVPetDevicesSurmount.Instance.GetConfData(Sys_Pet.Instance.devicesdic[curdeviceid].BreakCount+2).cost_fit_value)
                    {
                        Sys_Hint.Instance.PushEffectInNextFight();
                        Sys_Pet.Instance.OnDeviceSurmountReq(curdeviceid);
                        if (fxbreak.activeInHierarchy)
                            fxbreak.SetActive(false);
                        fxbreak.SetActive(true);
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009461, CSVPetDevicesSurmount.Instance.GetConfData(Sys_Pet.Instance.devicesdic[curdeviceid].BreakCount + 2).cost_fit_value));
                    }
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        public void OncancelBtnClicked()
        {
            if (Sys_Pet.Instance.adddevicestrengthpoint == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4417));
            }
            else
            {
                Sys_Pet.Instance.adddevicestrengthpoint = 0;
                AddResAttr(curdeviceid);
                layout.point.text = Sys_Pet.Instance.devicesdic[curdeviceid].StrengthenFreePoint.ToString();
            }
        }

        private void OpenSelectPet()
        {
            SelectPetParam selectPet = new SelectPetParam();
            selectPet.PetList = Sys_Pet.Instance.GetMagicDeedsSelect();
            selectPet.commonId = curdeviceid;
            selectPet.action = SelectAction;
            UIManager.OpenUI(EUIID.UI_SelectPet, false, selectPet);
        }

        public void OnokBtnClicked()
        {
            addpointlist.Clear();
            for (int i = 0; i < resattrList.Count; ++i)
            {
                if (resattrList[i].deviceStrengthen.Point != 0)
                {
                    addpointlist.Add(resattrList[i].deviceStrengthen);
                }
            }
            if (addpointlist.Count == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4417));
            }
            else
            {
                Sys_Hint.Instance.PushEffectInNextFight();
                Sys_Pet.Instance.OnDeviceAllocPointReq(curdeviceid, addpointlist);
            }
        }

        public void Onpartnericon01Clicked()
        {
        }

        public void Onpartnericon02Clicked()
        {
        }

        public void OnpracticeBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Pet_MagicDeedsPreview,false,curdeviceid);
        }

        public void OnresetBtnClicked()
        {
            PromptBoxParameter.Instance.Clear();
            string[] str = CSVParam.Instance.GetConfData(532).str_value.Split('|');
            uint num = 0;
            uint itemid = 0;
            uint.TryParse(str[0], out itemid);
            uint.TryParse(str[1], out num);
            if (GetDeviceAllStrengthen(curdeviceid) > 0)
            {
                if (Sys_Pet.Instance.devicesdic[curdeviceid].ResetPointCount == 0)
                {
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2009397, LanguageHelper.GetTextContent(CSVPetDevices.Instance.GetConfData(curdeviceid).name));
                }
                else
                {
                    PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Item;
                    PromptBoxParameter.Instance.itemId = itemid;
                    PromptBoxParameter.Instance.itemNum = num;
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2009398, LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(itemid).name_id), LanguageHelper.GetTextContent(CSVPetDevices.Instance.GetConfData(curdeviceid).name));
                }
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    if (Sys_Pet.Instance.devicesdic[curdeviceid].ResetPointCount == 0)
                    {
                        Sys_Pet.Instance.OnDeviceRePointReq(curdeviceid);
                    }
                    else
                    {
                        if (Sys_Bag.Instance.GetItemCount(itemid) >= num)
                        {
                            Sys_Hint.Instance.PushEffectInNextFight();
                            Sys_Pet.Instance.OnDeviceRePointReq(curdeviceid);
                        }
                        else
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4052));
                    }
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009463));
            }
        }

        public void OnaddpetBtnClicked()
        {
            addanimator.enabled = false;
            OpenSelectPet();            
        }

        private void SelectAction(uint petId)
        {
            Sys_Hint.Instance.PushEffectInNextFight();
            Sys_Pet.Instance.OnPetChangePositionReq(petId, curdeviceid);
            UIManager.CloseUI(EUIID.UI_SelectPet);
        }

        public void OnunlockBtnClicked()
        {
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Item;
            PromptBoxParameter.Instance.itemId = CSVPetDevices.Instance.GetConfData(curdeviceid).coin_actication[0];
            PromptBoxParameter.Instance.itemNum = CSVPetDevices.Instance.GetConfData(curdeviceid).coin_actication[1];
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2009399, LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(CSVPetDevices.Instance.GetConfData(curdeviceid).coin_actication[0]).name_id), LanguageHelper.GetTextContent(CSVPetDevices.Instance.GetConfData(curdeviceid).name));
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (Sys_Bag.Instance.GetItemCount(CSVPetDevices.Instance.GetConfData(curdeviceid).coin_actication[0]) >= CSVPetDevices.Instance.GetConfData(curdeviceid).coin_actication[1])
                {
                    Sys_Pet.Instance.OnDeviceActivateReq(curdeviceid);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000934, LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(CSVPetDevices.Instance.GetConfData(curdeviceid).coin_actication[0]).name_id)));
                }           

            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnskillToggleChanged(bool isOn)
        {
            layout.skillpageGo.SetActive(isOn);
            layout.intensifypageGo.SetActive(!isOn);
            layout.breakpageGo.SetActive(!isOn);
        }

        public void OnintensifyToggleChanged(bool isOn)
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(10522, false))
            {
                return;
            }
            layout.intensifypageGo.SetActive(isOn);
            layout.skillpageGo.SetActive(!isOn);
            layout.breakpageGo.SetActive(!isOn);
        }

        public void OnbreakToggleChanged(bool isOn)
        {
            layout.intensifypageGo.SetActive(!isOn);
            layout.skillpageGo.SetActive(!isOn);
            layout.breakpageGo.SetActive(isOn);
        }
        #endregion
    }
    */
}
