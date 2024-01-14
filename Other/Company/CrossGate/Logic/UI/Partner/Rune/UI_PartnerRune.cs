//using Lib.Core;
//using Logic.Core;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Table;
//using UnityEngine;
//using UnityEngine.UI;

//namespace Logic
//{
//    public enum ERuneUIType
//    {
//        Euqip,
//        Bag,
//        Exchange,
//    }

//    public class UI_PartnerRune_Layout
//    {
//        public Button closeBtn;
//        public Transform transform;
//        public GameObject runeEquipGo;
//        public GameObject runeBagGo;
//        public GameObject runeChangeGo;
//        public Text resAddText;

//        public CP_Toggle runeEquipToggle;
//        public CP_Toggle runeBagToggle;
//        public CP_Toggle runeExchangeToggle;
//        public void Init(Transform transform)
//        {
//            this.transform = transform;
//            closeBtn = transform.Find("Animator/View_Title08/Btn_Close").GetComponent<Button>();
//            resAddText = transform.Find("Animator/Image_Have/Text_Add").GetComponent<Text>();
//            runeEquipGo = transform.Find("Animator/View_RuneEquip").gameObject;
//            runeBagGo = transform.Find("Animator/View_RuneBag").gameObject;
//            runeChangeGo = transform.Find("Animator/View_RuneChange").gameObject;
//            runeEquipToggle = transform.Find("Animator/Label_Scroll01/TabList/TabItem0").GetComponent<CP_Toggle>();
//            runeBagToggle = transform.Find("Animator/Label_Scroll01/TabList/TabItem1").GetComponent<CP_Toggle>();
//            runeExchangeToggle = transform.Find("Animator/Label_Scroll01/TabList/TabItem2").GetComponent<CP_Toggle>();
//        }

//        public void RegisterEvents(IListener listener)
//        {
//            closeBtn.onClick.AddListener(listener.OncloseBtnClicked);
//            runeEquipToggle.onValueChanged.AddListener(listener.OnEquipToggleClicked);
//            runeBagToggle.onValueChanged.AddListener(listener.OnBagToggleClicked);
//            runeExchangeToggle.onValueChanged.AddListener(listener.OnExchangeToggleClicked);
//        }

//        public interface IListener
//        {
//            void OncloseBtnClicked();
//            void OnEquipToggleClicked(bool isOn);
//            void OnBagToggleClicked(bool isOn);
//            void OnExchangeToggleClicked(bool isOn);
//        }
//    }

//    public class UI_PartnerRune : UIBase, UI_PartnerRune_Layout.IListener, UI_PartnerRuneBag.IListener
//    {
//        private UI_PartnerRune_Layout layout = new UI_PartnerRune_Layout();
//        private UI_PartnerRuneEquip runeEquipView;
//        private UI_PartnerRuneBag runeBagView;
//        private UI_PartnerRuneChange runeChangeView;
//        private UI_CurrencyTitle currency;
//        private ERuneUIType currentIndex = ERuneUIType.Euqip;
//        private uint currentPartnerId;

//        protected override void OnLoaded()
//        {
//            layout.Init(transform);
//            layout.RegisterEvents(this);
//            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property")?.gameObject);
//            runeEquipView = AddComponent<UI_PartnerRuneEquip>(layout.runeEquipGo.transform);
//            runeBagView = AddComponent<UI_PartnerRuneBag>(layout.runeBagGo.transform);
//            runeBagView.Register(this);
//            runeChangeView = AddComponent<UI_PartnerRuneChange>(layout.runeChangeGo.transform);
//        }

//        protected override void OnOpen(object arg)
//        {
//            if (null != arg)
//                currentPartnerId = Convert.ToUInt32(arg);
//            else
//                currentPartnerId = 1501;
//        }

//        protected override void ProcessEventsForEnable(bool toRegister)
//        {            
//            //Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnRuneDecomposeCallBack, OnRuneDecomposeCallBack, toRegister);
//            //Sys_Bag.Instance.eventEmitter.Handle<uint,long>(Sys_Bag.EEvents.OnCurrencyChanged, OnCurrencyChange, toRegister);
//        }

//        private void InitView()
//        {
//            layout.resAddText.transform.parent.gameObject.SetActive(false);

//        }

//        public void RefreshExpState(bool isAddShow, long num)
//        {
//            layout.resAddText.transform.parent.gameObject.SetActive(isAddShow);
//            if (isAddShow)
//            {
//                TextHelper.SetText(layout.resAddText, LanguageHelper.GetTextContent(4062, num.ToString()));
//            }
//        }

//        protected override void OnOpened()
//        {
//        }

//        protected override void OnHide()
//        {
//            runeChangeView.Hide();
//            runeBagView.Hide();
//            runeEquipView.Hide();
//        }

//        public void OncloseBtnClicked()
//        {
//            currentIndex = ERuneUIType.Euqip;
//            currentPartnerId = 0;
//            runeEquipView.CloseView();
//            CloseSelf();
//        }

//        public List<uint> currencyList = new List<uint>() { 9 };
//        protected override void OnShow()
//        {
//            if (currentIndex == ERuneUIType.Euqip)
//            {
//                currency?.SetData(UI_CurrencyTitle.DefaultCurrencies);

//                layout.runeEquipToggle.SetSelected(true, false);
//                runeEquipView.SetData(currentPartnerId);
//                runeEquipView.Show();
//                runeBagView.Hide();
//            }
//            else if (currentIndex == ERuneUIType.Bag)
//            {
//                currency?.SetData(currencyList);

//                layout.runeBagToggle.SetSelected(true, false);
//                runeBagView.Show();
//                runeEquipView.Hide();
//            }
//            else if (currentIndex == ERuneUIType.Exchange)
//            {
//                currency?.SetData(currencyList);
//                RefreshExpState(false, 0);
//                layout.runeExchangeToggle.SetSelected(true, false);
//            }
//            InitView();
//        }

//        protected override void OnUpdate()
//        {
//            runeBagView.ExecUpdate();
//        }

//        protected override void OnDestroy()
//        {
//            runeBagView.OnDestroy();
//            runeEquipView?.OnDestroy();
//            currency?.Dispose();
//        }

//        public void OnEquipToggleClicked(bool isOn)
//        {

//            if (isOn)
//            {
//                currency?.SetData(UI_CurrencyTitle.DefaultCurrencies);

//                currentIndex = ERuneUIType.Euqip;
//                runeEquipView.SetData(currentPartnerId);
//                runeEquipView.Show();
//                runeBagView.Hide();
//                runeChangeView.Hide();
//            }

//        }

//        public void OnBagToggleClicked(bool isOn)
//        {
//            if (isOn)
//            {
//                currency?.SetData(currencyList);

//                currentIndex = ERuneUIType.Bag;
//                runeBagView.Show();
//                runeEquipView.Hide();
//                runeChangeView.Hide();
//            }
//        }

//        public void OnExchangeToggleClicked(bool isOn)
//        {
//            if (isOn)
//            {
//                currency?.SetData(currencyList);
//                RefreshExpState(false, 0);
//                currentIndex = ERuneUIType.Exchange;
//                runeEquipView.Hide();
//                runeChangeView.Show(nSortingOrder);
//                runeBagView.Hide();
//            }
//        }
//    }
//}

