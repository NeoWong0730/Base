using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Preinstall_Layout
    {
        public Transform transform;
        public Text potency;
        public Button closeBtn;
        public Button resetBtn;
        public Text resetBtntxt;
        public Button okBtn;
        public Button messageBtn;
        public GameObject attrGo;
        public GameObject basicGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            potency = transform.Find("Animator/View_Message/Text_Potency/Text_Number").GetComponent<Text>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Middle/Btn_Close").GetComponent<Button>();
            resetBtn = transform.Find("Animator/View_Message/Button_Reset").GetComponent<Button>();
            resetBtntxt = transform.Find("Animator/View_Message/Button_Reset/Text").GetComponent<Text>();
            okBtn = transform.Find("Animator/View_Message/Button_Ok").GetComponent<Button>();
            messageBtn = transform.Find("Animator/View_Message/Button_Message").GetComponent<Button>();
            attrGo = transform.Find("Animator/View_Message/Add_Attr_Scroll/Viewport/Attr").gameObject;
            basicGo = transform.Find("Animator/Basic_Attr_Scroll/Viewport/Item").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
            resetBtn.onClick.AddListener(listener.OnresetBtnClicked);
            okBtn.onClick.AddListener(listener.OnokBtnClicked);
            messageBtn.onClick.AddListener(listener.OnmessageBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
            void OnresetBtnClicked();
            void OnokBtnClicked();
            void OnmessageBtnClicked();
        }
    }

    public class UI_SliderItem : UIComponent
    {
        private uint id;
        private Text attrname;
        private Text number;
        private Text addpoint;
        private Text subpoint;
        private Button addBtn;
        private Button subBtn;
        private Slider addSlider;
        private int potency;

        public UI_SliderItem(uint _id) : base()
        {
            id = _id;
        }

        protected override void Loaded()
        {
            int.TryParse(CSVParam.Instance.GetConfData(251).str_value,out potency);
            attrname = transform.Find("Text_AttrName").GetComponent<Text>();
            number = transform.Find("Number").GetComponent<Text>();
            addpoint = transform.Find("Number/Add").GetComponent<Text>();
            subpoint = transform.Find("Sub").GetComponent<Text>();
            addBtn = transform.Find("Button_Add").GetComponent<Button>();
            addBtn.onClick.AddListener(OnaddBtnClicked);
            subBtn = transform.Find("Button_Sub").GetComponent<Button>();
            subBtn.onClick.AddListener(OnsubBtnClicked);
            addSlider = transform.Find("Slider").GetComponent<Slider>();
            addSlider.onValueChanged.AddListener(OnsliderValueChanged);
            addSlider.maxValue = potency/2;
            addSlider.wholeNumbers = true;
        }

        private int GetAllusepotency()
        {
            int allusepotency = 0;
            foreach (var v in Sys_Attr.Instance.addbeforeAttrsPreview)
            {
                allusepotency += v.Value;
            }
            return allusepotency;
        }

        private void OnsliderValueChanged(float value)
        {
            int usepotencybefore = GetAllusepotency();
            int addattrbefore = Sys_Attr.Instance.addbeforeAttrsPreview[id];
            Sys_Attr.Instance.addbeforeAttrsPreview[id] = (int)Math.Floor(value);
            int usepotencyafter = GetAllusepotency();
            if (usepotencyafter <= potency&& Sys_Attr.Instance.addbeforeAttrsPreview[id] <= potency / 2)
            {
                if ((value/4 * potency) < Sys_Attr.Instance.beforePoints[id])
                {
                    if ((Sys_Attr.Instance.beforePoints[id] ==Sys_Attr.Instance.addbeforeAttrsPreview[id]))                    
                        addpoint.text = String.Empty;
                    else
                        addpoint.text = "-" + (Sys_Attr.Instance.beforePoints[id] - Sys_Attr.Instance.addbeforeAttrsPreview[id]).ToString();
                }
                else
                {
                    if (Sys_Attr.Instance.addbeforeAttrsPreview[id] == Sys_Attr.Instance.beforePoints[id])
                        addpoint.text = String.Empty;
                    else
                        addpoint.text = "+" + (Sys_Attr.Instance.addbeforeAttrsPreview[id] - Sys_Attr.Instance.beforePoints[id]).ToString();
                }
            }
            else
            {
                if (potency / 2 == usepotencybefore)
                    addpoint.text = String.Empty;
                else
                    addpoint.text = "+" + (potency / 2 - usepotencybefore).ToString();
                Sys_Attr.Instance.addbeforeAttrsPreview[id] = addattrbefore;
                addSlider.value = (float)addattrbefore;
                usepotencyafter = usepotencybefore;
            }
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnBeforeChangePotency, (uint)(potency - usepotencyafter),true);
            Sys_Attr.Instance.eventEmitter.Trigger(Sys_Attr.EEvents.OnPreinstallChangePotency);
        }

        private void OnsubBtnClicked()
        {
            if (Sys_Attr.Instance.addbeforeAttrsPreview[id] > 0)
            {
                addSlider.value -= 1;
            }
        }

        private void OnaddBtnClicked()
        {
            int usepotencybefore = GetAllusepotency();
            if (usepotencybefore < potency)
            {
                if (Sys_Attr.Instance.addbeforeAttrsPreview[id] < potency / 2)
                {
                    addSlider.value += 1;
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

        public void RefreshItem()
        {
            attrname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(id).name).ToString();
            number.text = "+" + Sys_Attr.Instance.beforePoints[id].ToString();
            addpoint.text = string.Empty;
            subpoint.text = string.Empty;
            addSlider.gameObject.SetActive(true);
            addSlider.value = Sys_Attr.Instance.beforePoints[id] ;
        }
    }

    public class UI_Preinstall : UIBase, UI_Preinstall_Layout.IListener
    {
        private UI_Preinstall_Layout layout = new UI_Preinstall_Layout();
        private List<UI_SliderItem> attrsliderlist = new List<UI_SliderItem>();
        private List<UI_BasicAttr> basiclist = new List<UI_BasicAttr>();

        private int usepoint;
        private int point;
        private uint curIndex;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);

        }

        protected override void OnOpen(object index)
        {
            if (index != null)
            {
                curIndex = (uint)index;
            }
        }

        protected override void OnShow()
        {
            SetValue();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {         
            Sys_Attr.Instance.eventEmitter.Handle<uint,bool>(Sys_Attr.EEvents.OnBeforeChangePotency, OnBeforeChangePotency, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle<uint>(Sys_Attr.EEvents.OnUpdateBeforAttr, OnUpdateBeforAttr, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnPreinstallChangePotency, OnPreinstallChangePotency, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle<uint>(Sys_Attr.EEvents.OnGetBeforePoint, OnUpdateBeforAttr, toRegister);
        }

        protected override void OnHide()
        {
            DefaultAttr();
        }

        private void OnUpdateBeforAttr(uint index)
        {
            if (attrsliderlist.Count == 0)
            {
                SetValue();
            }
            else
            {
                for (int i = 0; i < attrsliderlist.Count; ++i)
                {
                    attrsliderlist[i].RefreshItem();
                }
                for (int i = 0; i < basiclist.Count; ++i)
                {
                    basiclist[i].RefreshItem((int)curIndex);
                }
                Getusepoint();
                layout.potency.text = (point - usepoint).ToString();
            }
        }

        private void OnPreinstallChangePotency()
        {
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].OnPreinstallChangePotency();
            }
        }

        private void OnBeforeChangePotency(uint number,bool isPrestall)
        {
            layout.potency.text = number.ToString();
            if(number== point)
            {
                ImageHelper.SetImageGray(layout.resetBtn.GetComponent<Image>(),true);
                layout.resetBtntxt.text = "<color=#573518>" + LanguageHelper.GetTextContent(2009393) + "</color>";
                layout.resetBtn.enabled = false;
            }
            else
            {
                ImageHelper.SetImageGray(layout.resetBtn.GetComponent<Image>(), false);
                layout.resetBtntxt.text = "<color=#345961>" + LanguageHelper.GetTextContent(2009393) + "</color>";
                layout.resetBtn.enabled = true;
            }
            foreach (var item in Sys_Attr.Instance.beforePoints)
            {
                if ( Sys_Attr.Instance.addbeforeAttrsPreview.TryGetValue(item.Key,out int value) && value != item.Value)
                {
                    ImageHelper.SetImageGray(layout.okBtn.GetComponent<Image>(), false);
                    layout.okBtn.enabled = true;
                    return;
                }
            }
            ImageHelper.SetImageGray(layout.okBtn.GetComponent<Image>(), true);
            layout.okBtn.enabled = false;
        }

        private void SetValue()
        {
            if (Sys_Attr.Instance.beforePoints.Count == 0)
            {
                return;
            }
            DefaultAttr();
            AddAttrList();
            AddBasicList();
            Getusepoint();
            int.TryParse(CSVParam.Instance.GetConfData(251).str_value, out point);
            layout.potency.text = (point-usepoint).ToString();
            if (usepoint == 0)
            {
                ImageHelper.SetImageGray(layout.resetBtn.GetComponent<Image>(), true);
                layout.resetBtntxt.text = "<color=#573518>" + LanguageHelper.GetTextContent(2009393) + "</color>";
                layout.resetBtn.enabled = false;
            }
            else
            {
                ImageHelper.SetImageGray(layout.resetBtn.GetComponent<Image>(), false);
                layout.resetBtntxt.text = "<color=#345961>" + LanguageHelper.GetTextContent(2009393) + "</color>";
                layout.resetBtn.enabled = true;
            }
            ImageHelper.SetImageGray(layout.okBtn.GetComponent<Image>(), true);
            layout.okBtn.enabled = false;
        }

        private void Getusepoint()
        {
            usepoint = Sys_Attr.Instance.beforePoints[5] + Sys_Attr.Instance.beforePoints[7] + Sys_Attr.Instance.beforePoints[9] + Sys_Attr.Instance.beforePoints[11] + Sys_Attr.Instance.beforePoints[13];
        }

        private void AddAttrList()
        {
            attrsliderlist.Clear();
            foreach (var item in Sys_Attr.Instance.beforePoints)
            {
                Sys_Attr.Instance.addbeforeAttrsPreview.Add(item.Key, (int)item.Value);
                GameObject go = GameObject.Instantiate<GameObject>(layout.attrGo, layout.attrGo.transform.parent);
                UI_SliderItem slider = new UI_SliderItem(item.Key);
                slider.Init(go.transform);
                slider.RefreshItem();
                attrsliderlist.Add(slider);
            }
            layout.attrGo.SetActive(false);
        }

        private void AddBasicList()
        {
            basiclist.Clear();
            for (int i = 0; i < Sys_Attr.Instance.pkAttrsId.Count; ++i)
            {
                if (CSVAttr.Instance.GetConfData(Sys_Attr.Instance.pkAttrsId[i]).attr_type == 1 && CSVAttr.Instance.GetConfData(Sys_Attr.Instance.pkAttrsId[i]).add_type == 1)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.basicGo, layout.basicGo.transform.parent);
                    UI_BasicAttr basicAttr = new UI_BasicAttr(Sys_Attr.Instance.pkAttrsId[i], true);
                    basicAttr.Init(go.transform);
                    basicAttr.RefreshItem((int)curIndex);
                    basiclist.Add(basicAttr);
                }
            }
            layout.basicGo.SetActive(false);
        }

        private void DefaultAttr()
        {
            Sys_Attr.Instance.addbeforeAttrsPreview.Clear();
            layout.attrGo.SetActive(true);
            layout.basicGo.SetActive(true);
            for (int i = 0; i < attrsliderlist.Count; ++i)
            {
                attrsliderlist[i].OnDestroy();
            }
            for (int i = 0; i < basiclist.Count; ++i)
            {
                basiclist[i].OnDestroy();
            }
            attrsliderlist.Clear();
            basiclist.Clear();
            FrameworkTool.DestroyChildren(layout.attrGo.transform.parent.gameObject,layout.attrGo.name);
            FrameworkTool.DestroyChildren(layout.basicGo.transform.parent.gameObject, layout.basicGo.name);
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Preinstall);
        }

        public void OnmessageBtnClicked()
        {
        }

        public void OnokBtnClicked()
        {
            Sys_Hint.Instance.PushEffectInNextFight();
            Sys_Attr.Instance.AttrSetBeforePointReq(Sys_Attr.Instance.addbeforeAttrsPreview[5], Sys_Attr.Instance.addbeforeAttrsPreview[7],
              Sys_Attr.Instance.addbeforeAttrsPreview[9], Sys_Attr.Instance.addbeforeAttrsPreview[11], Sys_Attr.Instance.addbeforeAttrsPreview[13], curIndex);
            UIManager.CloseUI(EUIID.UI_Preinstall);
        }

        public void OnresetBtnClicked()
        {
            Sys_Attr.Instance.AttrSetBeforePointReq(0,0,0,0,0, curIndex);
        }
    }
}
