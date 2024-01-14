using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
namespace Logic
{
    public class UI_Pet_MountCharge_Layout
    {
        private Button closeBtn;
        private Button onceBtn;
        private Button tenceBtn;
        private PropItem item;
        private Slider slider;
        private Text sliderText;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();
            onceBtn =  transform.Find("Animator/View_Info/Btn_Charge_1").GetComponent<Button>();
            tenceBtn = transform.Find("Animator/View_Info/Btn_Charge_10").GetComponent<Button>();
            item = new PropItem();
            item.BindGameObject(transform.Find("Animator/View_Info/PropItem").gameObject);
            slider = transform.Find("Animator/View_Info/Slider_Energy").GetComponent<Slider>();
            sliderText = transform.Find("Animator/View_Info/Slider_Energy/Text_Percent").GetComponent<Text>();
        }

        public void SetItemInfo()
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(Sys_Pet.Instance.MountEnergyItemId, 1, true, false, false, false, false, true, true, true);
            CSVItem.Data itemConfigData = CSVItem.Instance.GetConfData(Sys_Pet.Instance.MountEnergyItemId);
            TextHelper.SetText(item.txtName, itemConfigData.name_id);
            item.txtName.gameObject.SetActive(true);
            item.SetData(itemData, EUIID.UI_Pet_MountCharge);
        }

        public void RefreshSliderView()
        {
            var currentValue = Sys_Pet.Instance.RidingEnergy;
            var currentMax = CSVPetNewParam.Instance.GetConfData(61).value;
            slider.value = (currentValue + 0f) / currentMax;
            sliderText.text = currentValue.ToString(); //string.Format("{0}/{1}", currentValue.ToString(), currentMax.ToString());
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            onceBtn.onClick.AddListener(listener.OnceBtnClicked);
            tenceBtn.onClick.AddListener(listener.TenceBtnClicked);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnceBtnClicked();
            void TenceBtnClicked();
        }
    }

    public class UI_Pet_MountCharge : UIBase, UI_Pet_MountCharge_Layout.IListener
    {
        private UI_Pet_MountCharge_Layout layout = new UI_Pet_MountCharge_Layout();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnEnergyChargeEnd, RefreshView, toRegister);
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            layout.SetItemInfo();
            layout.RefreshSliderView();
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_MountCharge);
        }

        public void OnceBtnClicked()
        {
            AddEnergy(1);
        }

        public void TenceBtnClicked()
        {
            AddEnergy(2);
        }

        public void AddEnergy(uint type)
        {
            //if(Sys_Pet.Instance.RidingEnergy >= Sys_Pet.Instance.EnergyLimit)
            //{
                ItemIdCount itemIdCount = new ItemIdCount(Sys_Pet.Instance.MountEnergyItemId, 1);
                if (itemIdCount.Enough)
                {
                    Sys_Pet.Instance.OnPetRidingSkillAddEnergyReq(type);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000221u));
                }
            //}
        }
    }
}