using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;

namespace Logic
{

    public class UI_Energyspar : UIBase, UI_Energyspar_Right_Tabs.IListener
    {
        private int tabIndex = 0;
        private UI_CurrencyTitle currency;
        private UI_Energyspar_Right_Tabs rightTabs;
        private Dictionary<EEnergysparTabType, UIComponent> dictTabPanels = new Dictionary<EEnergysparTabType, UIComponent>();
        private UI_Energyspar_Shop energysparShopPanel;
        private UI_Energyspar_Main energysparMainPanel;
        private UI_Energyspar_Right_Tabs energysparTab;
        private MallPrama param;
        protected override void OnLoaded()
        {
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            
            rightTabs = new UI_Energyspar_Right_Tabs();
            rightTabs.Init(transform.Find("Animator/View_Left_Tabs"));
            rightTabs.Register(this);

            energysparMainPanel = AddComponent<UI_Energyspar_Main>(transform.Find("Animator/View_Energyspar"));
            energysparShopPanel = AddComponent<UI_Energyspar_Shop>(transform.Find("Animator/View_Exchange"));

            dictTabPanels.Add(EEnergysparTabType.Main, energysparMainPanel);
            dictTabPanels.Add(EEnergysparTabType.Shop, energysparShopPanel);
            
            Button btnClose = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Energyspar);
            });
        }

        protected override void OnOpen(object arg)
        {
            if (null != arg)
            {
                param = arg as MallPrama;
                tabIndex = (int)param.mallId - 1;
            }
            else
            {
                tabIndex = 0;
            }
                
        }

        protected override void OnShow()
        {            
            rightTabs.OnTabIndex(tabIndex);

            currency.SetData(new List<uint>() { 1, 2, 6 });
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_StoneSkill.Instance.eventEmitter.Handle<uint>(Sys_StoneSkill.EEvents.EnergysparSpecilEvent, EnergysparSpecilEvent, toRegister);
        }

        private void EnergysparSpecilEvent(uint id)
        {
            if(null == param)
            {
                param = new MallPrama();
                param.itemId = id;
            }
            rightTabs.OnTabIndex(1);
        }

        protected override void OnHide()
        {
            //TODO
            tabIndex = 0;
            List<UIComponent> dataList = new List<UIComponent>(dictTabPanels.Values);
            for (int i = 0; i < dataList.Count; i++)
            {
                dataList[i].Hide();
            }            
            rightTabs.Hide();
        }

        protected override void OnDestroy()
        {
            currency.Dispose();
        }        

        uint pageShowTime;
        public void OnClickTabType(EEnergysparTabType _type)
        {
            if(isOpen)
            {
                //TODO
                tabIndex = (int)_type;
                List<EEnergysparTabType> keyList = new List<EEnergysparTabType>(dictTabPanels.Keys);
                for (int i = 0; i < keyList.Count; i++)
                {
                    EEnergysparTabType key = keyList[i];
                    UIComponent value = dictTabPanels[key];
                    if (key == _type)
                    {
                        
                        if (key == EEnergysparTabType.Shop)
                        {
                            if (null == param)
                            {
                                param = new MallPrama();
                                param.itemId = 0;
                            }
                            energysparShopPanel.ShowEx(param.itemId);
                            param = null;
                        }
                        else
                        {
                            param = null;
                            value.Show();
                        }

                        pageShowTime = Sys_Time.Instance.GetServerTime();
                    }
                    else
                    {
                        value.Hide();
                    }
                }                
            }            
        }

    }
}

