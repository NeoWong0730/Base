using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Linq;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_AddView : UIComponent
    {
        public Text potency;
        public Text useBtnText;
        public Button messageBtn;
        public Button preinstallBtn;
        public Button resetBtn;
        public Button addBtn;
        public Button recomandBtn;
        public Button useBtn;
        public GameObject basicattrGo; 
        public GameObject attrsliderGo;
        public Transform planView;
        public uint carrerId;
        private uint freeResetMaxLevel;
        private uint curIndex;

        private List<UI_BasicAttr> basiclist = new List<UI_BasicAttr>();
        private List<UI_AttrSlider> attrsliderlist = new List<UI_AttrSlider>();
        private UI_AddPoint_Plan addPointPlan = new UI_AddPoint_Plan();

        protected override void Loaded()
        {
            potency = transform.Find("Text_Potency/Text_Number").GetComponent<Text>();
            useBtnText = transform.Find("Button_use/Text_01").GetComponent<Text>(); ;
            messageBtn = transform.Find("Button_Message").GetComponent<Button>();
            preinstallBtn = transform.Find("Button_Preinstall").GetComponent<Button>();
            resetBtn = transform.Find("Button_Reset").GetComponent<Button>();
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            recomandBtn = transform.Find("Button_Command").GetComponent<Button>();
            useBtn = transform.Find("Button_use").GetComponent<Button>();
            messageBtn.onClick.AddListener(OnmessageBtnClicked);
            addBtn.onClick.AddListener(OnaddBtnClicked);
            preinstallBtn.onClick.AddListener(OnpreinstallBtnClicked);
            resetBtn.onClick.AddListener(OnresetBtnClicked);
            recomandBtn.onClick.AddListener(OnrecomandBtnBtnClicked);
            useBtn.onClick.AddListener(OnUseBtnClicked);
            basicattrGo = transform.Find("Basic_Attr_Scroll/Viewport/Item").gameObject;
            attrsliderGo = transform.Find("Add_Attr_Scroll/Viewport/Attr").gameObject;
            planView = transform.Find("View_Lable").transform;

            uint.TryParse(CSVParam.Instance.GetConfData(299).str_value, out freeResetMaxLevel);
            addPointPlan.Init(planView);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForAwake(toRegister);
            Sys_Attr.Instance.eventEmitter.Handle<long>(Sys_Attr.EEvents.OnChangePotency, OnChangePotency, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle<int>(Sys_Attr.EEvents.OnSelectAddPointPlan, OnSelectAddPointPlan, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnSchemeUpdateAttr, OnSchemeUpdateAttr, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnGetPointScheme, OnGetPointScheme, toRegister);
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.OnChangePlanSuccess, OnChangePlanSuccess, toRegister);
            addPointPlan.PocessEvent(toRegister);
        }

        uint onShowTime;
        public override void Show()
        {
            base.Show();
            curIndex = Sys_Attr.Instance.curIndex;
            carrerId = (uint)GameCenter.mainHero.careerComponent.CurCarrerType;
            recomandBtn.gameObject.SetActive(carrerId != 0);
            SetValue();
            UIManager.HitPointShow(EUIID.UI_Attribute, ERoleViewType.ViewAdd.ToString());
            onShowTime = Sys_Time.Instance.GetServerTime();
            addPointPlan.Refresh();
            if (curIndex == Sys_Attr.Instance.curIndex)
            {
                ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), true);
                TextHelper.SetText(useBtnText, 8311);
                useBtn.enabled = false;
            }
            else
            {
                ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), false);
                TextHelper.SetText(useBtnText, 5127);
                useBtn.enabled = true;
            }
        }

        public override void Hide()
        {
            UIManager.HitPointHide(EUIID.UI_Attribute, onShowTime, ERoleViewType.ViewAdd.ToString());
            DefaultBasicAttr();
            DefaultSliderAttr();
            base.Hide();
        }

        #region Event Callback
        private void OnUpdateAttr()
        {
            for (int i=0;i<basiclist.Count;++i)
            {
                basiclist[i].RefreshItem((int)curIndex);
            }
            for (int i = 0; i < attrsliderlist.Count; ++i)
            {
                attrsliderlist[i].RefreshItem(GetMaxSliderValue(), GetSurplusMostAddPoint((int)attrsliderlist[i].id));
            }
            potency.text = Sys_Attr.Instance.GetAssighPointByAttrId(0, curIndex).ToString();
            ImageHelper.SetImageGray(addBtn.GetComponent<Image>(), true);
        }


        private void OnSelectAddPointPlan(int index)
        {
            if (index == curIndex)
            {
                return;
            }     
            curIndex = (uint)index;
            DefaultSliderAttr();
            AddAttrList();
            potency.text = Sys_Attr.Instance.listBasePlans[(int)curIndex].SurplusPoint.ToString();
            for(int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].RefreshItem((int)curIndex);
            }
            if (curIndex == Sys_Attr.Instance.curIndex)
            {
                ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), true);
                TextHelper.SetText(useBtnText, 8311);
                useBtn.enabled = false;
            }
            else
            {
                ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), false);
                TextHelper.SetText(useBtnText, 5127);
                useBtn.enabled = true;
            }
        }

        private void OnSchemeUpdateAttr()
        {
            DefaultSliderAttr();
            AddAttrList();
            potency.text = Sys_Attr.Instance.listBasePlans[(int)curIndex].SurplusPoint.ToString();
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].RefreshItem((int)curIndex);
            }
        }

        private void OnGetPointScheme()
        {
            SetValue();
        }

        private void OnChangePlanSuccess(uint planType, uint index)
        {
            if (planType == (uint)Sys_Plan.EPlanType.RoleAttribute)
            {
                if (curIndex == Sys_Attr.Instance.curIndex)
                {
                    ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), true);
                    TextHelper.SetText(useBtnText, 8311);
                    useBtn.enabled = false;
                }
                else
                {
                    ImageHelper.SetImageGray(useBtn.GetComponent<Image>(), false);
                    TextHelper.SetText(useBtnText, 5127);
                    useBtn.enabled = true;
                }
            }
        }

        private void OnChangePotency(long value)
        {
            if (Sys_Attr.Instance.GetAssighPointByAttrId(0, curIndex) == value)
            {
                ImageHelper.SetImageGray(addBtn.GetComponent<Image>(), true);
            }
            else
            {
                ImageHelper.SetImageGray(addBtn.GetComponent<Image>(), false);
            }
            potency.text = value.ToString();
            for(int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].OnChangePotency(value);
            }
        }

        #endregion

        #region Function
        private void SetValue()
        {
            if (Sys_Attr.Instance.listBasePlans.Count < curIndex)
            {
                return;
            }
            potency.text = Sys_Attr.Instance.listBasePlans[(int)curIndex].SurplusPoint.ToString();
            AddBasicList();
            AddAttrList();
            ImageHelper.SetImageGray(addBtn.GetComponent<Image>(), true);
        }

        private void AddAttrList()
        {
            attrsliderlist.Clear();
            Sys_Attr.Instance.addAttrsPreview.Clear();
            for (int i = 0; i < Sys_Attr.Instance.pkAttrsId.Count; ++i)
            {
                uint attrId = Sys_Attr.Instance.pkAttrsId[i];
                CSVAttr.Data data = CSVAttr.Instance.GetConfData(attrId);
                if (data.attr_type ==4 && data.show_type == 1 && data.isShow == 1)
                {
                    Sys_Attr.Instance.addAttrsPreview.Add(attrId, 0);
                    GameObject go = GameObject.Instantiate<GameObject>(attrsliderGo, attrsliderGo.transform.parent);
                    UI_AttrSlider attrslider = new UI_AttrSlider(attrId,curIndex);
                    attrslider.Init(go.transform);
                    attrsliderlist.Add(attrslider);
                    int assighPoint = Sys_Attr.Instance.GetAssighPointByAttrId(attrId,curIndex);
                    attrslider.RefreshItem(GetMaxSliderValue(), GetSurplusMostAddPoint(assighPoint));
                }
            }
            attrsliderGo.SetActive(false);
        }

        private long GetSurplusMostAddPoint(int assighPoint)
        {
            long.TryParse(CSVParam.Instance.GetConfData(251).str_value, out long peraddpotency);
            long totalMaxAddPoint = Sys_Role.Instance.Role.Level * peraddpotency / 2;
            if (totalMaxAddPoint<= assighPoint)
            {
                return 0;
            }
            else
            {
                return totalMaxAddPoint - assighPoint;
            }
          
        }

        private long GetMaxSliderValue()
        {
            int vitAssign = Sys_Attr.Instance.listBasePlans[(int)curIndex].VitAssign;
            int snhAssign = Sys_Attr.Instance.listBasePlans[(int)curIndex].SnhAssign;
            int intenAssign = Sys_Attr.Instance.listBasePlans[(int)curIndex].IntenAssign;
            int speedAssign = Sys_Attr.Instance.listBasePlans[(int)curIndex].SpeedAssign;
            int magicAssign = Sys_Attr.Instance.listBasePlans[(int)curIndex].MagicAssign;

            long VitMax = vitAssign + GetSurplusMostAddPoint(vitAssign);
            long MagicMax = magicAssign + GetSurplusMostAddPoint(magicAssign);
            long IntenMax = intenAssign + GetSurplusMostAddPoint(intenAssign);
            long SpeedMax = speedAssign + GetSurplusMostAddPoint(speedAssign);
            long SnhMax = snhAssign + GetSurplusMostAddPoint(snhAssign);
            long[] numbers = { VitMax, MagicMax, IntenMax, SpeedMax, SnhMax };
             return numbers.Max();
        }

        private void AddBasicList()
        {
            basiclist.Clear();
            for (int i = 0; i < Sys_Attr.Instance.pkAttrsId.Count; ++i)
            {
                if (CSVAttr.Instance.GetConfData(Sys_Attr.Instance.pkAttrsId[i]).attr_type == 1&& CSVAttr.Instance.GetConfData(Sys_Attr.Instance.pkAttrsId[i]).add_type==1)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(basicattrGo, basicattrGo.transform.parent);
                    UI_BasicAttr basicAttr = new UI_BasicAttr(Sys_Attr.Instance.pkAttrsId[i], false);
                    basicAttr.Init(go.transform);

                    basicAttr.RefreshItem((int)curIndex);
                    basiclist.Add(basicAttr);
                }
            }
            basicattrGo.SetActive(false);
        }

        private void DefaultBasicAttr()
        {
            basicattrGo.SetActive(true);
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].OnDestroy();
            }
            basiclist.Clear();
            FrameworkTool.DestroyChildren(basicattrGo.transform.parent.gameObject, basicattrGo.transform.name);   
        }

        private void DefaultSliderAttr()
        {
            attrsliderGo.SetActive(true);
            for (int i = 0; i < attrsliderlist.Count; ++i)
            {
                attrsliderlist[i].OnDestroy();
            }
            attrsliderlist.Clear();
            FrameworkTool.DestroyChildren(attrsliderGo.transform.parent.gameObject, attrsliderGo.transform.name);
        }

        #endregion

        #region ButtonClicked
        private void OnresetBtnClicked()
        {
            if (Sys_Role.Instance.Role.Level <= freeResetMaxLevel)
            {
                int num = Sys_Attr.Instance.listBasePlans[(int)curIndex].IntenAssign + Sys_Attr.Instance.listBasePlans[(int)curIndex].MagicAssign + Sys_Attr.Instance.listBasePlans[(int)curIndex].SnhAssign
    + Sys_Attr.Instance.listBasePlans[(int)curIndex].SpeedAssign + Sys_Attr.Instance.listBasePlans[(int)curIndex].VitAssign;
                if (num == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(104308));
                }
                else
                {
                    Sys_Attr.Instance.RePointReq(0,curIndex);
                }
            }
            else
            {
                UIManager.OpenUI(EUIID.UI_Reset,false,curIndex);
            }
        }

        private void OnpreinstallBtnClicked()
        {
            Sys_Attr.Instance.AttrGetBeforePointReq(curIndex);
            UIManager.OpenUI(EUIID.UI_Preinstall,false, curIndex);
        }

        private void OnaddBtnClicked()
        {
            if ((int)Sys_Attr.Instance.addAttrsPreview[5] == 0 && (int)Sys_Attr.Instance.addAttrsPreview[7] == 0 
                && (int)Sys_Attr.Instance.addAttrsPreview[9] == 0 && (int)Sys_Attr.Instance.addAttrsPreview[11] == 0
                && (int)Sys_Attr.Instance.addAttrsPreview[13] == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4417));
            }
            else
            {
                Sys_Hint.Instance.PushEffectInNextFight();
                Sys_Attr.Instance.AttrAllocPointReq((int)Sys_Attr.Instance.addAttrsPreview[5], (int)Sys_Attr.Instance.addAttrsPreview[7],
                    (int)Sys_Attr.Instance.addAttrsPreview[9], (int)Sys_Attr.Instance.addAttrsPreview[11], (int)Sys_Attr.Instance.addAttrsPreview[13],curIndex);
                Sys_Attr.Instance.addAttrsPreview[5] = 0;
                Sys_Attr.Instance.addAttrsPreview[7] = 0;
                Sys_Attr.Instance.addAttrsPreview[9] = 0;
                Sys_Attr.Instance.addAttrsPreview[11] = 0;
                Sys_Attr.Instance.addAttrsPreview[13] = 0;
            }
        }

        private void OnmessageBtnClicked()
        {
            UIRuleParam param = new UIRuleParam();
            param.StrContent =LanguageHelper.GetTextContent(4351);
            param.Pos = CameraManager.mUICamera.WorldToScreenPoint(messageBtn.GetComponent<RectTransform>().position);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void OnrecomandBtnBtnClicked()
        {
            UIRuleParam param = new UIRuleParam();
            param.StrContent = LanguageHelper.GetTextContent(CSVCareer.Instance.GetConfData(carrerId).recommend) ;
            param.Pos = CameraManager.mUICamera.WorldToScreenPoint(recomandBtn.GetComponent<RectTransform>().position);
            UIManager.OpenUI(EUIID.UI_Rule, false, param);
        }

        private void OnUseBtnClicked()
        {
            Sys_Attr.Instance.SwitchPointSchemeReq(curIndex);
        }
        #endregion
    }

    public class UI_BasicAttr : UIComponent
    {
        public Transform trans;
        private Text attrname;
        private Text number;
        private Image icon;
        private Text addpoint;
        private Button button;

        private uint id;
        private bool isPreinstall;
        private int preinstallNumber;

        public UI_BasicAttr(uint _id,  bool _isPreinstall) : base()
        {
            id = _id;
            isPreinstall = _isPreinstall;
        }

        protected override void Loaded()
        {        
            attrname = transform.Find("Text_Attr").GetComponent<Text>();
            number = transform.Find("Text_Add/Text_Number").GetComponent<Text>();
            icon = transform.Find("Icon").GetComponent<Image>();
            addpoint = transform.Find("Text_Add").GetComponent<Text>();
            button = transform.Find("BG").GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        private void OnClicked()
        {
            AttributeTip tip = new AttributeTip();
            tip.tipLan = CSVAttr.Instance.GetConfData(id).desc;
            UIManager.OpenUI(EUIID.UI_Attribute_Tips, false, tip);
        }

        public void OnChangePotency(long value)
        {
            float num = 0;
            uint level = Sys_Role.Instance.Role.Level;
            for (int i = 0; i < CSVAttrConvert.Instance.GetConfData(level).vtl_convert.Count; ++i)
            {
                if (id == CSVAttrConvert.Instance.GetConfData(level).vtl_convert[i][0])
                {
                    num += Sys_Attr.Instance.addAttrsPreview[5] * (CSVAttrConvert.Instance.GetConfData(level).vtl_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < CSVAttrConvert.Instance.GetConfData(level).str_convert.Count; ++i)
            {
                if (id == CSVAttrConvert.Instance.GetConfData(level).str_convert[i][0])
                {
                    num +=Sys_Attr.Instance.addAttrsPreview[7] * (CSVAttrConvert.Instance.GetConfData(level).str_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < CSVAttrConvert.Instance.GetConfData(level).tgh_convert.Count; ++i)
            {
                if (id == CSVAttrConvert.Instance.GetConfData(level).tgh_convert[i][0])
                {
                    num += Sys_Attr.Instance.addAttrsPreview[9] * (CSVAttrConvert.Instance.GetConfData(level).tgh_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < CSVAttrConvert.Instance.GetConfData(level).qui_convert.Count; ++i)
            {
                if (id == CSVAttrConvert.Instance.GetConfData(level).qui_convert[i][0])
                {
                    num += Sys_Attr.Instance.addAttrsPreview[11] * (CSVAttrConvert.Instance.GetConfData(level).qui_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < CSVAttrConvert.Instance.GetConfData(level).mgc_convert.Count; ++i)
            {
                if (id == CSVAttrConvert.Instance.GetConfData(level).mgc_convert[i][0])
                {
                    num += Sys_Attr.Instance.addAttrsPreview[13] *(CSVAttrConvert.Instance.GetConfData(level).mgc_convert[i][1] / 10000.0f);
                    break;
                }
            }
            if (num == 0)
            {
                addpoint.enabled = false;
                addpoint.text = string.Empty;
            }
            else
            {
                addpoint.enabled = true;
                addpoint.text = "+" + ((int)num).ToString();
            }
        }

        private float CaculatePreinstallPoint()
        {
            float num = 0;
            uint level = Sys_Role.Instance.Role.Level;
            CSVAttrConvert.Data cSVAttrConvertData = CSVAttrConvert.Instance.GetConfData(level);
            for (int i = 0; i < cSVAttrConvertData.vtl_convert.Count; ++i)
            {
                if (id == cSVAttrConvertData.vtl_convert[i][0] && Sys_Attr.Instance.addbeforeAttrsPreview.TryGetValue(5,out int value))
                {
                    num += value * (cSVAttrConvertData.vtl_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < cSVAttrConvertData.str_convert.Count; ++i)
            {
                if (id == cSVAttrConvertData.str_convert[i][0] && Sys_Attr.Instance.addbeforeAttrsPreview.TryGetValue(7,out int value))
                {
                    num += value * (cSVAttrConvertData.str_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < cSVAttrConvertData.tgh_convert.Count; ++i)
            {
                if (id == cSVAttrConvertData.tgh_convert[i][0] && Sys_Attr.Instance.addbeforeAttrsPreview.TryGetValue(9, out int value))
                {
                    num += value * (cSVAttrConvertData.tgh_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < cSVAttrConvertData.qui_convert.Count; ++i)
            {
                if (id == cSVAttrConvertData.qui_convert[i][0] && Sys_Attr.Instance.addbeforeAttrsPreview.TryGetValue(11, out int value))
                {
                    num += value * (cSVAttrConvertData.qui_convert[i][1] / 10000.0f);
                    break;
                }
            }
            for (int i = 0; i < cSVAttrConvertData.mgc_convert.Count; ++i)
            {
                if (id == cSVAttrConvertData.mgc_convert[i][0] && Sys_Attr.Instance.addbeforeAttrsPreview.TryGetValue(13, out int value))
                {
                    num += value * (cSVAttrConvertData.mgc_convert[i][1] / 10000.0f);
                    break;
                }
            }
            return num;
        }

        public void OnPreinstallChangePotency()
        {
            if (!isPreinstall)
                return;
            int curNumber = (int)CaculatePreinstallPoint();

            number.text = string.Empty;
            addpoint.enabled = true;
            addpoint.text = "+" + curNumber.ToString();
        }

        public void RefreshItem(int index)
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            if (isPreinstall)
            {
                preinstallNumber =(int) CaculatePreinstallPoint();
                number.text =preinstallNumber.ToString();
            }
            else
            {
                if (index == Sys_Attr.Instance.curIndex)
                {
                    number.text = Sys_Attr.Instance.pkAttrs[id].ToString();
                }
                else
                {
                    number.text = Sys_Attr.Instance.GetPkAttrByIndex(index, id).ToString();
                }
            }
            ImageHelper.SetIcon(icon, CSVAttr.Instance.GetConfData(id).attr_icon);
            addpoint.text = string.Empty;
            addpoint.enabled = false;
        }
    }

    public class UI_AttrSlider : UIComponent
    {
        public uint id;
        private Text attrname;
        private Text number;
        private Text addpoint;
        private Text subpoint;
        private Button addBtn;
        private Button subBtn;
        private Slider addSlider;
        private Slider fixpointshow;
        private uint potency;
        private int fixnumber;
        private bool flag = false;
  
        private bool isBtncontrol = false;
        private float fixpoint;
        private uint peraddpotency;
        private long surplusmostAddPoint;
        private long sliderMaxValue;
        private long baseAttrsafterPoint;
        private uint index;

        public UI_AttrSlider(uint _id,uint _index)
            : base()
        {
            id = _id;
            index = _index;
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
            addSlider = transform.Find("Slider").GetComponent<Slider>();
            addSlider.onValueChanged.AddListener(OnsliderValueChanged);
            fixpointshow = transform.Find("Slider_Fill").GetComponent<Slider>();
            uint.TryParse(CSVParam.Instance.GetConfData(251).str_value, out peraddpotency);
        }

        private long GetAllusepotency(uint id=0,long usepoint=0)
        {
            long allusepotency = 0;
            foreach (var v in Sys_Attr.Instance.addAttrsPreview)
            {
                if (v.Key == id)
                {
                    allusepotency += usepoint;
                }
                else
                {
                    allusepotency += v.Value;
                }
            }
            return allusepotency;
        }


        private void OnsliderValueChanged(float value)
        {
            long addattrbefore = Sys_Attr.Instance.addAttrsPreview[id];
            if (flag)
            {
                if (isBtncontrol)
                {
                    Sys_Attr.Instance.addAttrsPreview[id] = baseAttrsafterPoint - fixnumber;
                    isBtncontrol = false;
                }
                else
                {
                    Sys_Attr.Instance.addAttrsPreview[id] = (uint)Mathf.Round(addSlider.value * sliderMaxValue- fixnumber);
                    baseAttrsafterPoint = fixnumber + Sys_Attr.Instance.addAttrsPreview[id];
                }
                long usepotencyafter = GetAllusepotency();
                long addattrAfter = Sys_Attr.Instance.addAttrsPreview[id];
                if (usepotencyafter <= potency && addSlider.value >= fixpoint)
                {
                    if (addattrAfter <= surplusmostAddPoint)
                    {
                        if (addattrAfter != 0)
                        {
                            addpoint.text = "+"+((int)addattrAfter).ToString();
                        }
                        else
                        {
                            addpoint.text = string.Empty;
                        }
                        Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnChangePotency, potency - usepotencyafter);
                    }
                    else
                    {
                        Sys_Attr.Instance.addAttrsPreview[id] = surplusmostAddPoint;
                        addSlider.value = (float)(Sys_Attr.Instance.addAttrsPreview[id] + fixnumber) / sliderMaxValue;
                    }
                }
                else if (usepotencyafter > potency)
                {
                    addpoint.text = string.Empty;        
                    if (addSlider.value< fixpoint)
                    {
                        addSlider.value = fixpoint;
                        Sys_Attr.Instance.addAttrsPreview[id]= 0;
                    }
                    else if (GetAllusepotency(id, surplusmostAddPoint) > potency)
                    {
                        long point = potency - GetAllusepotency(id, 0);
                        addSlider.value = (float)(point + fixnumber) / sliderMaxValue;
                        Sys_Attr.Instance.addAttrsPreview[id] = point;
                    }
                    else
                    {
                        addSlider.value = (float)(surplusmostAddPoint + fixnumber) / sliderMaxValue;
                        Sys_Attr.Instance.addAttrsPreview[id] = surplusmostAddPoint;
                    }
                }
                else
                {
                    addpoint.text = string.Empty;
                    Sys_Attr.Instance.addAttrsPreview[id] = addattrbefore;
                    addSlider.value = fixpoint;
                }
            }
            else { flag = true; }
        }

        private void OnsubBtnClicked()
        {
            if (Sys_Attr.Instance.addAttrsPreview[id] > 0)
            {
                isBtncontrol = true;
                baseAttrsafterPoint -= 1;
                addSlider.value = (float)baseAttrsafterPoint / sliderMaxValue;
            }
        }

        private void OnaddBtnClicked()
        {
            long usepotencybefore = GetAllusepotency();
            if (usepotencybefore < potency)
            {
                if (Sys_Attr.Instance.addAttrsPreview[id] < surplusmostAddPoint)
                {
                    isBtncontrol = true;
                    baseAttrsafterPoint += 1;
                    addSlider.value = (float)baseAttrsafterPoint / sliderMaxValue;
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4410));
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4410));
            }
        }


        public void RefreshItem(long sliderMax,long surplusmost)
        {
            potency = (uint)Sys_Attr.Instance.listBasePlans[(int)index].SurplusPoint;
            surplusmostAddPoint = surplusmost;
            sliderMaxValue = sliderMax;
            fixnumber = Sys_Attr.Instance.GetAssighPointByAttrId(id, index);
            baseAttrsafterPoint = (uint)fixnumber;
            fixpoint = fixnumber / (float)sliderMaxValue;
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name);
            number.text = "+"+ fixnumber.ToString(); 
            if (fixpoint > 0.05f)
            {
                fixpointshow.value = fixpoint - 0.05f;
            }
            else
            {
                fixpointshow.value = fixpoint;
            }
            addpoint.text = string.Empty;
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

    public class UI_Point_Plan_Item
    {
        private Transform transform;
        public CP_Toggle toggle;
        private Button addBtn;
        private Button reNameBtn;
        private Text nameLight;
        private Text nameDark;
        public int index;
        public Action<int> addAction;
        public Action<int> renameAction;

        public void Init(Transform _transform)
        {
            transform = _transform;
            toggle = transform.Find("TabItem").GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(onValueChanged);
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnAddBtnClicked);
            reNameBtn = transform.Find("TabItem/Button_Rename").GetComponent<Button>();
            reNameBtn.onClick.AddListener(OnReNameBtnClicked);
            nameLight = transform.Find("TabItem/Image_Menu_Light/Text_Menu_Dark").GetComponent<Text>();
            nameDark = transform.Find("TabItem/Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
        }

        public void Refersh(int _index, string _name, bool _isAdd,bool isShow=true)
        {
            index = _index;
            if (_name == "")
            {
                nameLight.text = LanguageHelper.GetTextContent(10013503, (index + 1).ToString());
                nameDark.text = LanguageHelper.GetTextContent(10013503, (index + 1).ToString());
            }
            else
            {
                nameLight.text = _name;
                nameDark.text = _name;
            }
            addBtn.gameObject.SetActive(_isAdd);
            toggle.gameObject.SetActive(!_isAdd);
            transform.gameObject.SetActive(isShow);
        }

        public void Destroy()
        {
            addAction = null;
            renameAction = null;
        }

        private void OnAddBtnClicked()
        {
            addAction?.Invoke(index);
        }

        private void OnReNameBtnClicked()
        {
            renameAction?.Invoke(index);
        }

        private void onValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Attr.Instance.eventEmitter.Trigger<int>(Sys_Attr.EEvents.OnSelectAddPointPlan, index);
            }
        }
    }

    public class UI_AddPoint_Plan
    {
        private Transform transform;
        private Transform item;
        private List<UI_Point_Plan_Item> cells = new List<UI_Point_Plan_Item>();

        public void Init(Transform _transform)
        {
            transform = _transform;
            item = transform.Find("Label_Scroll01/TabList/Item");
        }

        public void PocessEvent(bool isRegister)
        {
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnGetPointScheme, OnGetPointScheme, isRegister);          
            Sys_Attr.Instance.eventEmitter.Handle<uint>(Sys_Attr.EEvents.OnRenamePointScheme, OnRenamePointScheme, isRegister);
        }

        private void OnGetPointScheme()
        {
            Default();
            AddItems();
        }

        private void OnRenamePointScheme(uint index)
        {
            uint.TryParse(CSVParam.Instance.GetConfData(1561).str_value, out uint openLv);
            for (int i = 0; i < cells.Count; ++i)
            {
                if (cells[i].index == index)
                {
                    bool isShow = (Sys_Role.Instance.Role.Level >= openLv && index == 1) || (index != 1);
                    cells[i].Refersh((int)index, Sys_Attr.Instance.listPlansName[i], i == (cells.Count - 1), isShow);
                }
            }
        }

        public void Refresh()
        {
            Default();
            AddItems();
        }

        private void AddItems()
        {
            cells.Clear();
            uint.TryParse(CSVParam.Instance.GetConfData(1561).str_value,out uint openLv);
            FrameworkTool.CreateChildList(item.parent.transform, Sys_Attr.Instance.listBasePlans.Count + 1);
            for (int i = 0; i <= Sys_Attr.Instance.listBasePlans.Count; ++i)
            {
                if (i != Sys_Attr.Instance.listBasePlans.Count)
                {
                    item.parent.transform.GetChild(i).name = (i + 1).ToString();
                }
                else
                {
                    item.parent.transform.GetChild(i).name = "Add";
                }
                UI_Point_Plan_Item cell = new UI_Point_Plan_Item();
                cell.Init(item.parent.transform.GetChild(i));
                bool isShow = (Sys_Role.Instance.Role.Level >= openLv && i == 1) || (i != 1);
                if (i == Sys_Attr.Instance.listBasePlans.Count)
                {
                    cell.Refersh(i, string.Empty, true);
                }
                else
                {
                    cell.Refersh(i, Sys_Attr.Instance.listPlansName[i], false, isShow);
                }
                cell.addAction = OnAddAction;
                cell.renameAction = OnRenamection;
                cells.Add(cell);
                cell.toggle.SetSelected(i == Sys_Attr.Instance.curIndex, true);
            }
        }

        private void Default()
        {
            for (int i = 0; i < cells.Count; ++i)
            {
                cells[i].Destroy();
            }
            if (item != null)
            {
                string name = item.parent.transform.GetChild(0).name;
                FrameworkTool.DestroyChildren(item.parent.gameObject, name);
            }
        }

        private void OnRenamection(int index)
        {
            void OnRename(int schIndex, int __, string newName)
            {
                Sys_Attr.Instance.RenamePointSchemeReq((uint)schIndex, newName);
            }

            UI_ChangeSchemeName.ChangeNameArgs arg = new UI_ChangeSchemeName.ChangeNameArgs()
            {
                arg1 = index,
                arg2 = 0,
                oldName = Sys_Attr.Instance.listPlansName[index],
                onYes = OnRename
            };
            UIManager.OpenUI(EUIID.UI_ChangeSchemeName, false, arg);
        }

        private void OnAddAction(int index)
        {
             bool valid = (Sys_Ini.Instance.Get<IniElement_IntArray>(1433, out var rlt) && rlt.value.Length >= 3);
            int limit = rlt.value[1];
            uint lan = 10013808;
            void OnConform()
            { 
                if (Sys_Bag.Instance.GetItemCount(2) < rlt.value[2])
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5602));
                }
                else
                {
                    // 请求server新增方案，同时给新方案设置空的数据
                    Sys_Attr.Instance.AddPointSchemeReq();
                }
            }

            bool isOpen = CSVCheckseq.Instance.GetConfData(12102).IsValid();
            if (isOpen)
            {
                if (Sys_Attr.Instance.listBasePlans.Count >= limit)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013703));
                    return;
                }
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(lan, rlt.value[2].ToString());
                PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013901));
            }
        }
    }
}
