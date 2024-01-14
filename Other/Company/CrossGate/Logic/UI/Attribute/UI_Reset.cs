using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System;

namespace Logic
{
    public class UI_Reset_Layout
    {
        public Transform transform;
        public Text potency;
        public Button closeBtn;
        public GameObject attrGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            potency = transform.Find("Animator/View_Message/Image_Potency/Text_Number").GetComponent<Text>();
            closeBtn = transform.Find("Animator/View_TipsBg01_Middle/Btn_Close").GetComponent<Button>();
            attrGo = transform.Find("Animator/View_Message/Scroll_View/Viewport/Item").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
        }

        public interface IListener
        {
            void OncloseBtnClicked();
        }
    }

    public class UI_Reset_Item : UIComponent
    {
        private Text itemname;
        private Text attrget;
        private Text attrreserPer;
        private Text resetpoint;
        private Button useBtn;
        private GameObject prop;

        private uint id;
        private uint attrid;
        private uint index;
        private long resetpointnum;
        private PropItem propItem;

        public UI_Reset_Item(uint _id, uint _index) : base()
        {
            id = _id;
            index = _index;
        }

        protected override void Loaded()
        {
            prop = transform.Find("Attr_Icon/PropItem").gameObject;
            itemname = transform.Find("Text_Name").GetComponent<Text>();
            attrget=transform.Find("Text_Name/Text_Number").GetComponent<Text>();
            attrreserPer = transform.Find("Text_Name/Text_Number/Text_Per").GetComponent<Text>();
            resetpoint = transform.Find("Text_Point").GetComponent<Text>();
            useBtn = transform.Find("Button_Use").GetComponent<Button>();
            useBtn.onClick.AddListener(OnuseBtnClicked);
            ProcessEventsForEnable(true);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        private void OnuseBtnClicked()
        {
            if (Sys_Bag.Instance.GetItemCount(id) == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101444));
            }
            else
            {
                if (resetpointnum == 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101106));
                }
                else
                { 
                    Sys_Hint.Instance.PushEffectInNextFight();
                    Sys_Attr.Instance.RePointReq(id, index);
                }
            }
        }

        public void RefreshItem()
        {
            List<uint> fun = CSVItem.Instance.GetConfData(id).fun_value;
            if (fun[0] == 0)
            {
                itemname.text = LanguageHelper.GetTextContent(4404);
                attrget.gameObject.SetActive(false);
                int num = Sys_Attr.Instance.listBasePlans[(int)index].IntenAssign + Sys_Attr.Instance.listBasePlans[(int)index].MagicAssign + Sys_Attr.Instance.listBasePlans[(int)index].SnhAssign
                    + Sys_Attr.Instance.listBasePlans[(int)index].SpeedAssign + Sys_Attr.Instance.listBasePlans[(int)index].VitAssign;
                resetpoint.text = num.ToString();
                resetpointnum = (long)num;
            }
            else
            {
                attrget.gameObject.SetActive(true);
                attrreserPer.text = fun[1].ToString();

                foreach (var v in Sys_Attr.Instance.pkAttrs)
                {
                    if (CSVAttr.Instance.GetConfData(v.Key).attr_type==4&& CSVAttr.Instance.GetConfData(v.Key).show_type==1&&v.Key == fun[0])
                    {
                        attrid = v.Key;
                        long attrnum = Sys_Attr.Instance.pkAttrs[attrid];
                        attrget.text = attrnum.ToString();
                        resetpointnum = Sys_Attr.Instance.GetAssighPointByAttrId(attrid, index);
                        resetpoint.text = resetpointnum.ToString();
                        itemname.text = LanguageHelper.GetTextContent(CSVAttr.Instance.GetConfData(attrid).name);
                        break;
                    }
                }
            }
            if (Sys_Bag.Instance.GetItemCount(id) == 0|| resetpointnum==0)
            {
                ImageHelper.SetImageGray(useBtn.gameObject.GetComponent<Image>(),true);
            }
            else
            {
                ImageHelper.SetImageGray(useBtn.gameObject.GetComponent<Image>(), false);
            }

            propItem = new PropItem();
            propItem.BindGameObject(prop);
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Pet_Develop, new PropIconLoader.ShowItemData(id, 1, true, false, false, false, false,
                _bShowCount: true, _bShowBagCount: true, _bUseClick: true)));
        }
    }

    public class UI_Reset : UIBase, UI_Reset_Layout.IListener
    {
        private UI_Reset_Layout layout = new UI_Reset_Layout();
        private List<UI_Reset_Item> itemlist = new List<UI_Reset_Item>();
        private uint index;

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                index = (uint)arg;
            }
            else
            {
                index = Sys_Attr.Instance.curIndex;
            }
        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            AddAttrList();
        }

        protected override void OnShow()
        {
            OnUpdateAttr();
            for (int i=0;i< itemlist.Count;++i)
            {
                itemlist[i].RefreshItem();
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateAttr, OnUpdateAttr, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateNumber, OnUpdateAttr, toRegister);
        }

        private void OnUpdateAttr()
        {
            layout.potency.text = Sys_Attr.Instance.GetAssighPointByAttrId(0, index).ToString();
            for (int i = 0; i < itemlist.Count; ++i)
            {
                itemlist[i].RefreshItem();
            }
        }

        private void AddAttrList()
        {
            itemlist.Clear();
            List<uint> list = new List<uint>();
            for (uint id = 200001; id < 200007; id++)
            {
                list.Add(id);             
            }
            uint lastone = list[5];
            list.Remove(lastone);
            list.Insert(0, lastone);
            for (int i = 0; i < list.Count; ++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.attrGo, layout.attrGo.transform.parent);
                UI_Reset_Item attrItem = new UI_Reset_Item(list[i], index);
                attrItem.Init(go.transform);
                itemlist.Add(attrItem);
            }
            layout.attrGo.SetActive(false);
        }

        public void OncloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Reset);
        }
    }
}
