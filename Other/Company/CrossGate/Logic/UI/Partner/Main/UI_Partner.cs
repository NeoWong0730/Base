using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using Packet;


namespace Logic
{
    public class PartnerUIParam
    {
        public int tabIndex;
        public uint partnerId;
    }

    public class UI_Partner : UIBase, UI_Partner_Right_Tabs.IListener
    {
        private UI_CurrencyTitle currency;
        private UI_Partner_Right_Tabs rightTabs;
        private Dictionary<EPartnerTabType, UIParseCommon> dictTabPanels = new Dictionary<EPartnerTabType, UIParseCommon>();

        private UI_Partner_List partnerList;
        private UI_Partner_Config partnerConfig;
        private UI_PartnerRuneEquip runeEquipView;
        private UI_PartnerRuneBag runeBagView;
        private UI_PartnerRuneChange runeChangeView;
        private UI_Partner_Fetter partnerFetter;
        private UI_Partner_Property partnerProperty;

        private EPartnerTabType tabType;

        private int tabIndex = 0;
        private uint selecPartnerId = 0;
        private PartnerUIParam uiParam;

        protected override void OnLoaded()
        {            
            //Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Animator/View_Bg01/Image_bg01").gameObject);
            //eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => {
            //    Sys_Partner.Instance.ClearSelectState();
            //});

            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            Button btnClose = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            rightTabs = new UI_Partner_Right_Tabs();
            rightTabs.Init(transform.Find("Animator/Label_Scroll01"));
            rightTabs.Register(this);

            partnerList = new UI_Partner_List();
            partnerList.Init(transform.Find("Animator/View_PartnerList"));
            partnerConfig = new UI_Partner_Config();
            partnerConfig.Init(transform.Find("Animator/View_PartnerConfig"));
            runeEquipView = new UI_PartnerRuneEquip();
            runeEquipView.Init(transform.Find("Animator/View_RuneEquip"));
            runeBagView = new UI_PartnerRuneBag();
            runeBagView.Init(transform.Find("Animator/View_RuneBag"));
            runeChangeView = new UI_PartnerRuneChange();
            runeChangeView.Init(transform.Find("Animator/View_RuneChange"));
            partnerFetter = new UI_Partner_Fetter();
            partnerFetter.Init(transform.Find("Animator/View_Fetter"));
            partnerProperty = new UI_Partner_Property();
            partnerProperty.Init(transform.Find("Animator/View_shuxingqianghua"));

            dictTabPanels.Add(EPartnerTabType.PartnerList, partnerList);
            dictTabPanels.Add(EPartnerTabType.PartnerConfig, partnerConfig);
            dictTabPanels.Add(EPartnerTabType.PartnerProperty, partnerProperty);
            dictTabPanels.Add(EPartnerTabType.PartnerRuneEquip, runeEquipView);
            dictTabPanels.Add(EPartnerTabType.PartnerRuneBag, runeBagView);
            dictTabPanels.Add(EPartnerTabType.PartnerRuneChange, runeChangeView);
            dictTabPanels.Add(EPartnerTabType.PartnerFetter, partnerFetter);
        }

        protected override void OnOpen(object arg)
        {
            uiParam = null;
            tabIndex = 0;
            if (arg != null)
            {
                uiParam = (PartnerUIParam)arg;
                tabIndex = uiParam.tabIndex;
                //OnClickTabType((EPartnerTabType)tabIndex);
            }
        }        

        protected override void OnShow()
        {
            rightTabs.OnTabIndex(tabIndex);
            
            bool hasNew = Sys_Partner.Instance.HasNew();
            this.rightTabs.ActiveRedDot((int)EPartnerTabType.PartnerRuneEquip, hasNew);
            
            CheckRuneChangeActiveRedDot();
            //currency.InitUi();
            currency.SetData(new List<uint>() { 1, 2, 3 },9);
        }

        protected override void OnHide()
        {            
            foreach (var data in dictTabPanels)
            {
                data.Value.Hide();
            }

            rightTabs.Hide();

            //关闭界面的时候, 保存阵容到服务器
            Sys_Partner.Instance.PartnerFormationReq();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Partner.Instance.eventEmitter.Handle<uint>(Sys_Partner.EEvents.OnFetterToRuneEquip, OnFetterToRuneEequip, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<uint, long>(Sys_Bag.EEvents.OnCurrencyChanged, OnRuneFragmentChange, toRegister);
            Sys_Partner.Instance.eventEmitter.Handle<int>(Sys_Partner.EEvents.OnTelPartnerTab, OnTelTab, toRegister);
        }

        protected override void ProcessEvents(bool toRegister) {
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnPartnerRuneStatusChanged, OnPartnerRuneStatusChanged,  toRegister);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnPartnerLevelChanged, OnPartnerLevelChanged,  toRegister);
            Sys_Partner.Instance.eventEmitter.Handle<Partner>(Sys_Partner.EEvents.OnPartnerUnlock, OnPartnerUnlock,  toRegister);
        }

        protected override void OnDestroy()
        {
            foreach (var data in dictTabPanels)
            {
                data.Value.OnDestroy();
            }
            currency?.Dispose();
            Sys_Partner.Instance.ClearPartnerlistIds();            
            
        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void OnPartnerLevelChanged() {
            bool hasNew = Sys_Partner.Instance.HasNew();
            this.rightTabs.ActiveRedDot((int)EPartnerTabType.PartnerRuneEquip, hasNew);
        }

        private void OnPartnerUnlock(Partner partner) {
            bool hasNew = Sys_Partner.Instance.HasNew();
            this.rightTabs.ActiveRedDot((int)EPartnerTabType.PartnerRuneEquip, hasNew);
        }

        private void OnPartnerRuneStatusChanged() {
            if (this.tabType == EPartnerTabType.PartnerRuneEquip) {
                this.runeEquipView?.ReSelectPartner();
            }
            
            bool hasNew = Sys_Partner.Instance.HasNew();
            this.rightTabs.ActiveRedDot((int)EPartnerTabType.PartnerRuneEquip, hasNew);
        }

        private void CheckRuneChangeActiveRedDot()
        {
            bool isEnough = Sys_Partner.Instance.IsEnough();
            this.rightTabs.ActiveRedDot((int)EPartnerTabType.PartnerRuneChange, isEnough);
        }

        private void OnRuneFragmentChange(uint i, long l)
        {
            if((uint)ECurrencyType.Runefragment == i)
            {
                CheckRuneChangeActiveRedDot();
            }
        }
        
        private void OnTelTab(int index)
        {
            if (tabIndex == index)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2006212));
                return;
            }

            rightTabs.OnTabIndex(index);
        }

        private void OnFetterToRuneEequip(uint partnerId)
        {
            selecPartnerId = partnerId;
            rightTabs.OnTabIndex((int)EPartnerTabType.PartnerRuneEquip);
            UIManager.CloseUI(EUIID.UI_Partner_Fetter_Detail);
        }

        public void OnClickTabType(EPartnerTabType _type)
        {
            tabType = _type;
            tabIndex = (int)tabType;

            foreach (var dict in dictTabPanels)
            {
                if (dict.Key == _type)
                {
                    dict.Value.Show();
                    if (_type == EPartnerTabType.PartnerRuneEquip)
                    {
                        if (uiParam != null)
                            selecPartnerId = uiParam.partnerId;

                        ((UI_PartnerRuneEquip)dict.Value).SetData(selecPartnerId);
                        selecPartnerId = 0;
                    }
                    else if (_type == EPartnerTabType.PartnerProperty)
                    {
                        uint tempSelectId = 0;
                        if (uiParam != null)
                            tempSelectId = uiParam.partnerId;

                        ((UI_Partner_Property)dict.Value).UpdateInfo(tempSelectId);
                    }

                    if (_type == EPartnerTabType.PartnerRuneBag || _type == EPartnerTabType.PartnerRuneChange)
                        currency.SetData(new List<uint>() { 9 });
                    else
                        currency.SetData(new List<uint>() { 1,2,3 },9);
                }
                else
                {
                    dict.Value.Hide();
                }
            }

            uiParam = null;
        }
    }
}


