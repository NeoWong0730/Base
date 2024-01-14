using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Framework;
using System.Linq;
using Lib.Core;
using UnityEngine.EventSystems;
using System;
using Packet;

namespace Logic
{
    public class UI_Pet_AddPoint_Layout 
    {
        public Transform transform;
        public Text potency;
        public Text level;
        public Text petname;
        public Text useBtnText;
        public Button presentBtn;
        public Button closeBtn;
        public Button resetBtn;
        public Button addBtn;
        public Button recomandBtn;
        public Button preinstallBtn;
        public Button useBtn;

        public GameObject basicattrGo;
        public GameObject attrsliderGo;
        public RawImage rawImage;
        public Image eventImage;
        public GameObject redPointGo;
        public Transform planView;

        public void Init(Transform transform)
        {
            this.transform = transform;
            potency = transform.Find("Animator/View_Right/Text_Potency/Text_Number").GetComponent<Text>();
            redPointGo = transform.Find("Animator/View_Right/Text_Potency/Image_Dot").gameObject;
            level = transform.Find("Animator/View_Left/Image_Namebg/Text_Label/Text_Level").GetComponent<Text>();
            petname = transform.Find("Animator/View_Left/Image_Namebg/Text_Name").GetComponent<Text>();
            useBtnText = transform.Find("Animator/View_Right/Button_use/Text_01").GetComponent<Text>();
            addBtn = transform.Find("Animator/View_Right/Button_Add").GetComponent<Button>();
            presentBtn = transform.Find("Animator/View_Right/Button_Present").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            resetBtn = transform.Find("Animator/View_Right/Button_Reset").GetComponent<Button>();
            recomandBtn = transform.Find("Animator/View_Right/Button_Command").GetComponent<Button>();
            preinstallBtn = transform.Find("Animator/View_Right/Button_Preinstall").GetComponent<Button>();
            useBtn = transform.Find("Animator/View_Right/Button_use").GetComponent<Button>();

            basicattrGo = transform.Find("Animator/View_Left/Image_Namebg/Scroll View/Viewport/Content/Text_Attr01").gameObject;
            attrsliderGo = transform.Find("Animator/View_Right/Add_Attr_Scroll/Viewport/Attr").gameObject;
            rawImage = transform.Find("Animator/View_Left/Charapter").GetComponent<RawImage>();
            eventImage = transform.Find("Animator/View_Left/EventImage").GetComponent<Image>();
            planView = transform.Find("Animator/View_Lable/Label_Scroll01");
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            addBtn.onClick.AddListener(listener.OnaddBtnClicked);
            presentBtn.onClick.AddListener(listener.OnpresentBtnClicked);
            resetBtn.onClick.AddListener(listener.OnresetBtnClicked);
            recomandBtn.onClick.AddListener(listener.OnrecomandBtnClicked);
            preinstallBtn.onClick.AddListener(listener.OnpreinstallBtnClicked);
            useBtn.onClick.AddListener(listener.OnuseBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnaddBtnClicked();
            void OnpresentBtnClicked();
            void OnresetBtnClicked();
            void OnrecomandBtnClicked();
            void OnpreinstallBtnClicked();
            void OnuseBtnClicked();
        }
    }

    public class UI_Pet_BasicAttr : UIComponent
    {
        public uint id;
        public uint index;
        private Text attrname;
        private Text number;
        public Text addpoint;
        private ClientPet clientpet;
        private Button button;

        public UI_Pet_BasicAttr( uint _id, uint _index, ClientPet _clientpet) : base()
        {
            id = _id;
            index = _index;
            clientpet = _clientpet;
        }

        protected override void Loaded()
        {
            attrname = transform.GetComponent<Text>();
            number = transform.Find("Text_Add/Text_Number").GetComponent<Text>();
            addpoint = transform.Find("Text_Add").GetComponent<Text>();
            button = transform.Find("Button_Attr").GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        public void RefreshItem()
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            long realNum = clientpet.GetAttrValueByAttrId((int)id);
            int curIndex = (int)clientpet.petUnit.PetPointPlanData.CurrentPlanIndex;
            if (curIndex == index)
            {
                number.text = "+" + realNum.ToString();
            }
            else
            {
                clientpet.pointPlanAttrs.TryGetValue(index, out PetPkAttr pkAttr);
                if (pkAttr != null)
                {
                    for (int i = 0; i < pkAttr.Attr.Count; ++i)
                    {
                        if (id == pkAttr.Attr[i].AttrId)
                        {
                            number.text = "+" + pkAttr.Attr[i].AttrValue.ToString();
                        }
                    }
                }
            }
            addpoint.text = string.Empty;
        }

        private void OnClicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = CSVAttr.Instance.GetConfData(id).desc;
            UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
        }
    }

    public class UI_Pet_AttrSlider : UIComponent
    {
        private uint id;
        private ClientPet clientpet;
        private Text attrname;
        private Text number;
        private Text addpoint;
        private Text subpoint;
        private Button addBtn;
        private Button subBtn;
        private Button tipBtn;
        private Button tipGradeAddBtn;
        private Slider addSlider;
        private Slider fixpointshow;

        private uint potency;
        private uint fixnumber;
        private float fixpoint;
        private long surplusmostAddPoint;
        private float gradePerent;
        private long addNum;
        private bool flag = false;
        private bool isBtncontrol = false;
        private EBaseAttr attr;
        private string petName;
        private UI_Pet_AddPoint ui_Pet_Addpoint;
        private CSVPetNewlv.Data csvPetNewlvData;
        private uint index;

        public UI_Pet_AttrSlider(uint _id, EBaseAttr _attr,ClientPet _clientpet) : base()
        {
            id = _id;
            attr = _attr;
            clientpet = _clientpet;
        }

        protected override void Loaded()
        {
            attrname = transform.Find("Text_AttrName").GetComponent<Text>();
            number = transform.Find("Text_AttrName/Number").GetComponent<Text>();
            addpoint = transform.Find("Text_AttrName/Number/Add").GetComponent<Text>();
            subpoint = transform.Find("Text_AttrName/Sub").GetComponent<Text>();
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnaddBtnClicked);  
            subBtn = transform.Find("Button_Sub").GetComponent<Button>();
            subBtn.onClick.AddListener(OnsubBtnClicked);
            tipBtn = transform.Find("Button_Attr").GetComponent<Button>();
            tipBtn.onClick.AddListener(OntipBtnClicked);
            tipGradeAddBtn = transform.Find("Text_AttrName/Button_Tips").GetComponent<Button>();
            tipGradeAddBtn.onClick.AddListener(OntipGradeAddBtnClicked);
            addSlider = transform.Find("Slider").GetComponent<Slider>();
            addSlider.onValueChanged.AddListener(OnsliderValueChanged);
            fixpointshow = transform.Find("Slider_Fill").GetComponent<Slider>();
        }

        public override void OnDestroy()
        {
            addBtn.onClick.RemoveListener(OnaddBtnClicked);
            subBtn.onClick.RemoveListener(OnsubBtnClicked);
            tipBtn.onClick.RemoveListener(OntipBtnClicked);
            tipGradeAddBtn.onClick.RemoveListener(OntipGradeAddBtnClicked);
            addSlider.onValueChanged.RemoveListener(OnsliderValueChanged);
        }

        private void OntipGradeAddBtnClicked()
        {
            TipsContent tip = new TipsContent();
            long Init = clientpet.GetBasicAttrPointByAttrId(id);
            long add = clientpet.GetAssignAttrPointByAttrId(id,index);
            long grade = clientpet.GetGradeAddAttrPointByAttrId(id, gradePerent,index);
            tip.InitPoint = Init;
            tip.addGradePrecent = gradePerent;
            tip.addPoint= add;
            tip.addGradePoint = grade;
            tip.remouldAddPoint = clientpet.GetAttrValueByAttrId((int)attr) - Init - add- grade;
            tip.attrName =LanguageHelper.GetTextContent( CSVAttr.Instance.GetConfData(id).name);
  
            UIManager.OpenUI(EUIID.UI_Tips_Attribute,false,tip);
        }

        private void OntipBtnClicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = CSVAttr.Instance.GetConfData(id).desc;
            UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
        }

        private uint GetAllusepotency(uint selfId=0)
        {
            uint allusepotency = 0;
            foreach (var v in clientpet.addAttrsPreview)
            {
                if(v.Key!=selfId)
                allusepotency += v.Value;
            }
            return allusepotency;
        }

        //计算剩余最多加点数
        private void GetSurplusMostAddPoint()
        {
            if (surplusmostAddPoint != 0)
            {
                return;
            }
            long vit = clientpet.GetAttrValueByIndex((int)EBaseAttr.Vit, (int)index,gradePerent);
            long magic = clientpet.GetAttrValueByIndex((int)EBaseAttr.Magic, (int)index, gradePerent);
            long inten = clientpet.GetAttrValueByIndex((int)EBaseAttr.Inten, (int)index, gradePerent);
            long snh = clientpet.GetAttrValueByIndex((int)EBaseAttr.Snh, (int)index, gradePerent);
            long speed = clientpet.GetAttrValueByIndex((int)EBaseAttr.Speed, (int)index, gradePerent);

            long num = (vit + magic + inten + snh + speed + potency) / 2;
            long VitMax = num  - vit;
            long MagicMax = num - magic;
            long IntenMax = num - inten; 
            long SpeedMax = num - speed;
            long SnhMax = num - snh;
            long[] numbers = { VitMax, MagicMax, IntenMax, SpeedMax, SnhMax };
            long maxNum= numbers.Max(); 
            surplusmostAddPoint = (uint)(maxNum > 0 && maxNum < potency ? maxNum : potency);
            DebugUtil.Log(ELogType.eAttr, $"属性:{LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString()} 剩余最多加点数 : {surplusmostAddPoint.ToString()}");
        }

        //进度条长度（每个属性进度条长度相同）
        private uint GetSliderMaxValue()
        {
            long vit = clientpet.GetAttrValueByIndex((int)EBaseAttr.Vit, (int)index, gradePerent);
            long magic = clientpet.GetAttrValueByIndex((int)EBaseAttr.Magic, (int)index, gradePerent);
            long inten = clientpet.GetAttrValueByIndex((int)EBaseAttr.Inten, (int)index, gradePerent);
            long snh = clientpet.GetAttrValueByIndex((int)EBaseAttr.Snh, (int)index, gradePerent);
            long speed = clientpet.GetAttrValueByIndex((int)EBaseAttr.Speed, (int)index, gradePerent);

            long VitMax = vit + surplusmostAddPoint;
            long MagicMax = magic + surplusmostAddPoint;
            long IntenMax = inten + surplusmostAddPoint;
            long SpeedMax = speed + surplusmostAddPoint;
            long SnhMax = snh  + surplusmostAddPoint;
            long[] numbers = { VitMax, MagicMax, IntenMax, SpeedMax, SnhMax };
            return (uint)numbers.Max();
        }

        private void OnsliderValueChanged(float value)
        {
            uint usepotencybefore = GetAllusepotency();
            uint addattrbefore = clientpet.addAttrsPreview[id];
            if (flag)
            {
                if (isBtncontrol)
                {
                    clientpet.addAttrsPreview[id] = (uint)(clientpet.baseAttrsafter[(int)attr] - clientpet.GetAttrValueByIndex((int)attr, (int)index, gradePerent));
                    isBtncontrol = false;
                }
                else
                {
                    if (value <= fixpoint)
                    {
                        clientpet.addAttrsPreview[id] = 0;
                    }
                    else
                    {
                        clientpet.addAttrsPreview[id] = (uint)Mathf.CeilToInt((value - fixpoint) * GetSliderMaxValue());
                    }
                    clientpet.baseAttrsafter[(int)attr] = fixnumber + clientpet.addAttrsPreview[id];
                }
                uint usepotencyafter = GetAllusepotency();
                if (usepotencyafter <= potency && value >= fixpoint)
                {
                    if (clientpet.addAttrsPreview[id] <= surplusmostAddPoint)
                    {
                        if (clientpet.addAttrsPreview[id] != 0)
                        {
                            addpoint.text = "+" + (clientpet.addAttrsPreview[id]).ToString();
                        }
                        else
                        {
                            addpoint.text = string.Empty;
                        }
                        Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangePotency, potency - usepotencyafter); 
                    }
                    else
                    {
                        clientpet.addAttrsPreview[id] = (uint)surplusmostAddPoint;
                        addSlider.value = (float)(clientpet.addAttrsPreview[id] + fixnumber) / GetSliderMaxValue();
                    }
                }
                else if (usepotencyafter > potency)
                {
                    uint numtemp = potency - GetAllusepotency(id);
                    addpoint.text = "+" + numtemp.ToString(); 
                    clientpet.addAttrsPreview[id] = numtemp;
                    addSlider.value = (float)(numtemp + fixnumber) / GetSliderMaxValue();
                    Sys_Pet.Instance.eventEmitter.Trigger(Sys_Pet.EEvents.OnChangePotency, (uint)0);
                }
                else
                {
                    addpoint.text = string.Empty;
                    clientpet.addAttrsPreview[id] = addattrbefore;
                    addSlider.value = fixpoint;
                }
            }
            else { flag = true; }
        }

        private void OnsubBtnClicked()
        {
            if (clientpet.addAttrsPreview[id] > 0)
            {
                isBtncontrol = true;
                clientpet.baseAttrsafter[(int)attr] -= 1;
                addSlider.value = (float)clientpet.baseAttrsafter[(int)attr] / GetSliderMaxValue();
            }
        }

        private void OnaddBtnClicked()
        {
            uint usepotencybefore = GetAllusepotency();
            if (clientpet.addAttrsPreview[id] < surplusmostAddPoint && usepotencybefore < potency)
            {
                isBtncontrol = true;
                clientpet.baseAttrsafter[(int)attr] += 1;
                addSlider.value = (float)clientpet.baseAttrsafter[(int)attr] / GetSliderMaxValue();
            }
            else
            {
                if(usepotencybefore >= potency)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10954));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10955));
                }
            }
        }

        public void RefreshItem(uint _index)
        {
            index = _index;
            uint total = clientpet.petUnit.PetPointPlanData.TotalPoint;
            uint use = clientpet.petUnit.PetPointPlanData.Plans[(int)index].UsePoint;
            potency = total - use;
            uint gradeNum = clientpet.GetPetMaxGradeCount() - clientpet.GetPetGradeCount();
            gradePerent = Sys_Pet.Instance.GetPetGradeNum((int)gradeNum);
            GetSurplusMostAddPoint();
            long attrNum = clientpet.GetAttrValueByIndex((int)attr, (int)index, gradePerent);
            number.text = "+" + attrNum.ToString();
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            if (attrNum / (float)GetSliderMaxValue() > 0.05f)
            {
                fixpointshow.value = attrNum / (float)GetSliderMaxValue() - 0.05f;
            }
            else
            {
                fixpointshow.value = attrNum / (float)GetSliderMaxValue();
            }
            fixpoint = attrNum / (float)GetSliderMaxValue();
            addpoint.text = string.Empty;
            fixnumber = (uint)attrNum;
            if (potency == 0)
            {
                addSlider.gameObject.SetActive(false);
            }
            else
            {
                addSlider.gameObject.SetActive(true);
                addSlider.value = fixpoint;
            }

       
        }
    }

    public class UI_Pet_AddPoint : UIBase,UI_Pet_AddPoint_Layout.IListener
    {
        private UI_Pet_AddPoint_Layout layout = new UI_Pet_AddPoint_Layout();
        private List<UI_Pet_BasicAttr> basiclist = new List<UI_Pet_BasicAttr>();
        private List<UI_Pet_AttrSlider> attrsliderlist = new List<UI_Pet_AttrSlider>();
        private UI_PetPoint_Plan petPointPlan = new UI_PetPoint_Plan();
        private List<uint> pkAttrIds = new List<uint>();
        private ClientPet clientpet;
        private string petName;
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private uint curIndex;

        //private UI_PetAdd_RedPoint redPoint;

        protected override void OnOpen(object arg)
        {
            clientpet = (ClientPet)arg;
        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            assetDependencies = transform.GetComponent<AssetDependencies>();
            petPointPlan.Init(layout.planView);
            curIndex = 0;
        }

        protected override void OnShow()
        {
            Sys_Pet.Instance.GetPointPlanAttrReq(clientpet.petUnit.Uid);
            SetValue();
            OnCreateModel();
            petPointPlan.Refresh(clientpet.petUnit,Sys_Plan.EPlanType.PetAttribute);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnChangePotency, OnChangePotency, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateAttr, OnUpdateAttr, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateExp, OnUpdateExp, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdateAllPlayerAllocPoint, OnUpdateAllPlayerAllocPoint, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnUpdatePetInfo, OnUpdatePetInfo, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<int, int>(Sys_Pet.EEvents.OnSelectAddPointPlan, OnSelectAddPointPlan, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnGetPointPlanAttr, OnGetPointPlanAttr, toRegister);

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(layout.eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);
            petPointPlan.PocessEvent(toRegister);
        }

        protected override void OnHide()
        {
            OnDestroyModel();
        }

        #region  ModelShow
        private void OnCreateModel()
        {
            _LoadShowScene();
            _LoadShowModel(clientpet.petUnit.SimpleInfo.PetId);
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

            layout.rawImage.gameObject.SetActive(true);
            layout.rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);

            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
                petDisplay.onLoaded = OnShowModelLoaded;
            }
        }

        private void _LoadShowModel(uint petid)
        {
            CSVPetNew.Data petdata = CSVPetNew.Instance.GetConfData(petid);
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
                uint highId = clientpet.petUnit.SimpleInfo.PetId;
                modelGo = petDisplay.GetPart(EPetModelParts.Main).gameObject;
                SceneActor.PetWearSet(Constants.PETWEAR_EQUIP, clientpet.GetPetSuitFashionId(), modelGo.transform);
                petDisplay.mAnimation.UpdateHoldingAnimations(CSVPetNew.Instance.GetConfData(highId).action_id_show, weaponId);
            }
        }

        private void OnDestroyModel()
        {
            _UnloadShowContent();
        }

        private void _UnloadShowContent()
        {
            layout.rawImage.gameObject.SetActive(false);
            layout.rawImage.texture = null;
            //petDisplay.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
            showSceneControl.Dispose();
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
                //localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                Vector3 newAngle = new Vector3(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = newAngle;
            }
        }

        #endregion

        #region CallBack

        private void OnUpdateAttr()
        {
            for (int i=0;i< basiclist.Count;++i) { basiclist[i].RefreshItem(); }
            for (int i = 0; i < attrsliderlist.Count;++i) { attrsliderlist[i].RefreshItem(curIndex); }
            uint total = clientpet.petUnit.PetPointPlanData.TotalPoint;
            uint use = clientpet.petUnit.PetPointPlanData.Plans[(int)curIndex].UsePoint;
            layout.potency.text = (total- use).ToString();
            ImageHelper.SetImageGray(layout.addBtn.GetComponent<Image>(), true,true);
            ImageHelper.SetImageGray(layout.presentBtn.GetComponent<Image>(), true,true);
            if (curIndex == clientpet.petUnit.PetPointPlanData.CurrentPlanIndex)
            {
                layout.redPointGo.SetActive(null != clientpet && Sys_Pet.Instance.fightPet.IsSamePet(clientpet.petUnit) && Sys_Pet.Instance.IsHasFightPetPointNotUse());
            }
            else
            {
                layout.redPointGo.SetActive(false);
            }
        }

        private void OnUpdateExp()
        {
            for (int i=0;i< Sys_Pet.Instance.petsList.Count;++i)
            {
                if (Sys_Pet.Instance.petsList[i] == clientpet)
                {
                    layout.level.text = LanguageHelper.GetTextContent(2009330, Sys_Pet.Instance.petsList[i].petUnit.SimpleInfo.Level.ToString());
                }
            }
            if (curIndex == clientpet.petUnit.PetPointPlanData.CurrentPlanIndex)
            {
                layout.redPointGo.SetActive(null != clientpet && Sys_Pet.Instance.fightPet.IsSamePet(clientpet.petUnit) && Sys_Pet.Instance.IsHasFightPetPointNotUse());
            }
            else
            {
                layout.redPointGo.SetActive(false);
            }
        }

        private void OnChangePotency(uint value)
        {
            uint total = clientpet.petUnit.PetPointPlanData.TotalPoint;
            uint use = clientpet.petUnit.PetPointPlanData.Plans[(int)curIndex].UsePoint;
            uint point = total - use;
            if (point == value)
            {
                ImageHelper.SetImageGray(layout.addBtn.GetComponent<Image>(), true,true);
                ImageHelper.SetImageGray(layout.presentBtn.GetComponent<Image>(), true,true);
            }
            else
            {
                ImageHelper.SetImageGray(layout.addBtn.GetComponent<Image>(), false,true);
                ImageHelper.SetImageGray(layout.presentBtn.GetComponent<Image>(), false, true);
            }

            layout.potency.text = value.ToString();
            CSVAttrConvert.Data csvattrData = CSVAttrConvert.Instance.GetConfData(clientpet.petUnit.SimpleInfo.Level);
            for (int i=0;i<basiclist.Count;++i) 
            {
                float num = 0;
                for (int j =0;j< csvattrData.vtl_convert.Count; ++j)
                {
                    if (basiclist[i].id == csvattrData.vtl_convert[j][0])
                    {
                        num +=clientpet.addAttrsPreview[5] * (csvattrData.vtl_convert[j][1] / 10000.0f);
                        break;
                    }
                }
                for (int j = 0; j < csvattrData.str_convert.Count; ++j)
                {
                    if (basiclist[i].id == csvattrData.str_convert[j][0])
                    {
                        num += clientpet.addAttrsPreview[7] * (csvattrData.str_convert[j][1] / 10000.0f);
                        break;
                    }
                }
                for (int j = 0; j < csvattrData.tgh_convert.Count; ++j)
                {
                    if (basiclist[i].id == csvattrData.tgh_convert[j][0])
                    {
                        num += clientpet.addAttrsPreview[9] * (csvattrData.tgh_convert[j][1] / 10000.0f);
                        break;
                    }
                }
                for (int j = 0; j < csvattrData.qui_convert.Count; ++j)
                {
                    if (basiclist[i].id == csvattrData.qui_convert[j][0])
                    {
                        num += clientpet.addAttrsPreview[11] * (csvattrData.qui_convert[j][1] / 10000.0f);
                        break;
                    }
                }
                for (int j = 0; j < csvattrData.mgc_convert.Count; ++j)
                {
                    if (basiclist[i].id == csvattrData.mgc_convert[j][0])
                    {
                        num += clientpet.addAttrsPreview[13] * (csvattrData.mgc_convert[j][1] / 10000.0f);
                        break;
                    }
                }
                if (num <0.0001)
                {
                    basiclist[i].addpoint.text = string.Empty;
                    basiclist[i].addpoint.enabled = false;
                }
                else
                {
                    basiclist[i].addpoint.enabled = true;
                    basiclist[i].addpoint.text = "+" + Mathf.CeilToInt(num).ToString();
                }
            }
            if (curIndex == clientpet.petUnit.PetPointPlanData.CurrentPlanIndex)
            {
                layout.redPointGo.SetActive(null != clientpet && Sys_Pet.Instance.fightPet.IsSamePet(clientpet.petUnit) && Sys_Pet.Instance.IsHasFightPetPointNotUse());
            }
            else
            {
                layout.redPointGo.SetActive(false);
            }
        }

        private void OnUpdateAllPlayerAllocPoint()
        {
            SetRecomandRule(clientpet.petData.id);
        }

        private void OnUpdatePetInfo()
        {
            SetValue();
        }

        private void OnSelectAddPointPlan(int index, int type)
        {
            if (type == (int)Sys_Plan.EPlanType.PetAttribute)
            {
                curIndex = (uint)index;
                SetValue();
            }
        }

        private void OnGetPointPlanAttr()
        {
            DefaultAttr();
            AddBasicList();
            AddAttrList();
        }
        #endregion

        #region Function
        private void SetValue()
        {
            if (clientpet == null)
            {
                clientpet = Sys_Pet.Instance.petsList[0];
            }
            else
            {
                for (int i = 0; i < Sys_Pet.Instance.petsList.Count; ++i)
                {
                    if (Sys_Pet.Instance.petsList[i].petUnit.Uid == clientpet.petUnit.Uid)
                    {
                        clientpet = Sys_Pet.Instance.petsList[i];
                    }
                }
            }
            uint total = clientpet.petUnit.PetPointPlanData.TotalPoint;
            uint use= clientpet.petUnit.PetPointPlanData.Plans[(int)curIndex].UsePoint;
            layout.potency.text = (total - use).ToString();
            DefaultAttr();
            AddBasicList();
            AddAttrList();
            ImageHelper.SetImageGray(layout.addBtn.GetComponent<Image>(), true, true);
            ImageHelper.SetImageGray(layout.presentBtn.GetComponent<Image>(), true, true);

            if (clientpet.petUnit.SimpleInfo.Name.IsEmpty)
            {
                petName = LanguageHelper.GetTextContent(CSVPetNew.Instance.GetConfData(clientpet.petUnit.SimpleInfo.PetId).name);
            }
            else
            {
                petName = clientpet.petUnit.SimpleInfo.Name.ToStringUtf8();
            }
            layout.petname.text = petName;
            layout.level.text = LanguageHelper.GetTextContent(2009330, clientpet.petUnit.SimpleInfo.Level.ToString());
            layout.rawImage.gameObject.SetActive(true);
            if (curIndex == clientpet.petUnit.PetPointPlanData.CurrentPlanIndex)
            {
                layout.redPointGo.SetActive(null != clientpet && Sys_Pet.Instance.fightPet.IsSamePet(clientpet.petUnit) && Sys_Pet.Instance.IsHasFightPetPointNotUse());
                ImageHelper.SetImageGray(layout.useBtn.GetComponent<Image>(), true);
                TextHelper.SetText(layout.useBtnText, 8311);
                layout.useBtn.enabled = false;
            }
            else
            {
                layout.redPointGo.SetActive(false);
                ImageHelper.SetImageGray(layout.useBtn.GetComponent<Image>(), false);
                TextHelper.SetText(layout.useBtnText, 5127);
                layout.useBtn.enabled = true;
            }
        }

        private void AddAttrList()
        {
            attrsliderlist.Clear();
            clientpet.addAttrsPreview.Clear();
            foreach (var item in Sys_Pet.Instance.baseAttrs2Id)
            {
                clientpet.addAttrsPreview.Add(item.Value, 0);
                GameObject go = GameObject.Instantiate<GameObject>(layout.attrsliderGo, layout.attrsliderGo.transform.parent);
                UI_Pet_AttrSlider attrslider = new UI_Pet_AttrSlider(item.Value, item.Key, clientpet);
                attrslider.Init(go.transform);
                attrslider.RefreshItem(curIndex);
                attrsliderlist.Add(attrslider);
            }
            layout.attrsliderGo.SetActive(false);
        }

        private void AddBasicList()
        {
            basiclist.Clear();
            pkAttrIds.Clear();
            foreach (var item in clientpet.pkAttrs)
            {
                if (CSVAttr.Instance.TryGetValue((uint)item.Key, out CSVAttr.Data data) && data.attr_type == 1 && data.add_type == 1)
                {
                    pkAttrIds.Add((uint)item.Key);
                }
            }
            pkAttrIds.Sort((dataA, dataB) =>
            {
                return CSVAttr.Instance.GetConfData(dataA).order_list.CompareTo(CSVAttr.Instance.GetConfData(dataB).order_list);
            });
            for (int i = 0; i < pkAttrIds.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.basicattrGo, layout.basicattrGo.transform.parent);
                UI_Pet_BasicAttr basicAttr = new UI_Pet_BasicAttr(pkAttrIds[i], curIndex, clientpet);
                basicAttr.Init(go.transform);
                basicAttr.RefreshItem();
                basiclist.Add(basicAttr);
            }
            layout.basicattrGo.SetActive(false);
        }

        private void DefaultAttr()
        {
            layout.basicattrGo.SetActive(true);
            layout.attrsliderGo.SetActive(true);
            for (int i=0;i< basiclist.Count;++i) { basiclist[i].OnDestroy(); }
            for (int i = 0; i < attrsliderlist.Count; ++i)  { attrsliderlist[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.basicattrGo.transform.parent.gameObject, layout.basicattrGo.transform.name);
            FrameworkTool.DestroyChildren(layout.attrsliderGo.transform.parent.gameObject, layout.attrsliderGo.transform.name);
        }

        private void SetRecomandRule(uint petId)
        {
            UIRuleParam param = new UIRuleParam();
            int vit = Sys_Pet.Instance.GetAttrAllocPlayerPer(petId, 0);
            int stren = Sys_Pet.Instance.GetAttrAllocPlayerPer(petId, 1);
            int inten = Sys_Pet.Instance.GetAttrAllocPlayerPer(petId, 2);
            int speed = Sys_Pet.Instance.GetAttrAllocPlayerPer(petId, 3);
            int magic = 100 - vit - stren - inten - speed;

            param.StrContent = LanguageHelper.GetTextContent(10771, petName, vit.ToString(), stren.ToString(), inten.ToString(), speed.ToString(), magic.ToString());
            param.Pos = CameraManager.mUICamera.WorldToScreenPoint(layout.recomandBtn.GetComponent<RectTransform>().position);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        #endregion

        #region ButtonClick
        public void OnaddBtnClicked()
        {
            if ((int)clientpet.addAttrsPreview[5] == 0 && (int)clientpet.addAttrsPreview[7] == 0 &&
    (int)clientpet.addAttrsPreview[9] == 0 && (int)clientpet.addAttrsPreview[11] == 0 && (int)clientpet.addAttrsPreview[13] == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4417));
            }
            else
            {
                Sys_Hint.Instance.PushEffectInNextFight();
                Sys_Pet.Instance.OnAllocPointReq(clientpet.petUnit.Uid,clientpet.addAttrsPreview[5], clientpet.addAttrsPreview[7],
                   clientpet.addAttrsPreview[9], clientpet.addAttrsPreview[11], clientpet.addAttrsPreview[13],curIndex);
                clientpet.addAttrsPreview[5] = 0;
                clientpet.addAttrsPreview[7] = 0;
                clientpet.addAttrsPreview[9] = 0;
                clientpet.addAttrsPreview[11] = 0;
                clientpet.addAttrsPreview[13] = 0;
            }
        }

        public void OnpresentBtnClicked()
        {
            clientpet.addAttrsPreview[5] = 0;
            clientpet.addAttrsPreview[7] = 0;
            clientpet.addAttrsPreview[9] = 0;
            clientpet.addAttrsPreview[11] = 0;
            clientpet.addAttrsPreview[13] = 0;
            OnUpdateAttr();
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_AddPoint);
        }

        public void OnresetBtnClicked()
        {
            PromptBoxParameter.Instance.Clear();
            string[] str = CSVPetNewParam.Instance.GetConfData(18).str_value.Split('&');
            uint num = 0;
            uint itemid = 0;
            uint.TryParse(str[0], out itemid);
            uint.TryParse(str[1], out num);
            if (clientpet.petUnit.PetPointPlanData.Plans[(int)curIndex].ResetCount == 0)
            {
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2009337, Sys_Pet.Instance.GetPetName(clientpet));
            }
            else
            {
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Item;
                PromptBoxParameter.Instance.itemId = itemid;
                PromptBoxParameter.Instance.itemNum = num;
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2009338, LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(itemid).name_id), Sys_Pet.Instance.GetPetName(clientpet));
            }
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (Sys_Bag.Instance.GetItemCount(itemid) >= num|| clientpet.petUnit.PetPointPlanData.Plans[(int)curIndex].ResetCount == 0)
                {
                    if (clientpet.petUnit.PetPointPlanData.Plans[(int)curIndex].UsePoint== 0){
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2009464));
                    }
                    else {
                        if (clientpet != null)
                        {
                            Sys_Hint.Instance.PushEffectInNextFight();
                            Sys_Pet.Instance.OnResetPointReq(clientpet.petUnit.Uid, curIndex);
                        }
                        clientpet.addAttrsPreview[5] = 0;
                        clientpet.addAttrsPreview[7] = 0;
                        clientpet.addAttrsPreview[9] = 0;
                        clientpet.addAttrsPreview[11] = 0;
                        clientpet.addAttrsPreview[13] = 0;
                    }
                }
                else
                {
                    ExchangeCoinParm exchangeCoinParm = new ExchangeCoinParm();
                    exchangeCoinParm.ExchangeType = itemid;
                    exchangeCoinParm.needCount = 0;
                    UIManager.OpenUI(EUIID.UI_Exchange_Coin, false, exchangeCoinParm);
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void OnrecomandBtnClicked()
        {
            uint petId = clientpet.petData.id;

            if (Sys_Pet.Instance.CheckAllPlayerAllocInfoIsValid(clientpet.petUnit.SimpleInfo.PetId))
            {
                SetRecomandRule(petId);
            }
            else
            {
                Sys_Pet.Instance.AllPlayerAllocInfoReq(petId);
            }
        }

        public void OnpreinstallBtnClicked()
        {
            PetAutoOpenEvt evt = new PetAutoOpenEvt();
            evt.clientPet = clientpet;
            evt.index = curIndex;
            UIManager.OpenUI(EUIID.UI_Pet_Automatical,false, evt);
        }

        public void OnuseBtnClicked()
        {
            Sys_Pet.Instance.AllocPointPlanUseReq(clientpet.petUnit.Uid, curIndex);
        }
        #endregion
    }
}

