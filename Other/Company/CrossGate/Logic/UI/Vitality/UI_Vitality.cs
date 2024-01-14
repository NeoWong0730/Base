using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_Vitality_Layout
    {
        public Transform transform;
        public Slider vitalitySlider;
        public Text vitalityPrecent;
        public CP_Toggle getToggle;
        public CP_Toggle changeToggle;
        public Button tipBtn;
        public Button closeBtn;
        public GameObject itemGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            vitalitySlider = transform.Find("Animator/Content/Vitality/Slider_Vitality").GetComponent<Slider>();
            vitalityPrecent = transform.Find("Animator/Content/Vitality/Text_Value").GetComponent<Text>();
            getToggle= transform.Find("Animator/Content/Toggles/Toggle1").GetComponent<CP_Toggle>();
            changeToggle= transform.Find("Animator/Content/Toggles/Toggle2").GetComponent<CP_Toggle>();
            tipBtn = transform.Find("Animator/Content/Vitality/Btn_Help").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            itemGo= transform.Find("Animator/Content/Scroll_View/Viewport/Item").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            getToggle.onValueChanged.AddListener(listener.OngetToggleValueChanged);
            changeToggle.onValueChanged.AddListener(listener.OnchangeToggleValueChanged);
            tipBtn.onClick.AddListener(listener.OntipBtnClicked);
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
        }

        public interface IListener
        {
            void OngetToggleValueChanged(bool isOn);
            void OnchangeToggleValueChanged(bool isOn);
            void OnCloseBtnClicked();
            void OntipBtnClicked();
        }
    }

    public class UI_Vitality_Item : UIComponent
    {
        private CSVVitality.Data csvVitalityData;
        private Image icon;
        private Text title;
        private Text message;
        private Text btnText;
        private Button doBtn;
        private GameObject finishGo;
        private bool isFinish;
        private uint funId;
        private uint type;
        private int vitalityCostChangeCoin;

        private uint id;
        private List<CSVDailyActivity.Data>  dailyActivityDatas = new List<CSVDailyActivity.Data>();
        public UI_Vitality_Item(uint _id,bool _isFinish) : base()
        {
            id = _id;
            isFinish = _isFinish;
        }

        protected override void Loaded()
        {
            icon= transform.Find("Image_BG/Image_Icon").GetComponent<Image>();
            title = transform.Find("Text_Name").GetComponent<Text>();
            message = transform.Find("Text_Description").GetComponent<Text>();
            btnText = transform.Find("Button/Text").GetComponent<Text>();
            finishGo = transform.Find("Finish").gameObject;
            doBtn = transform.Find("Button").GetComponent<Button>();
            doBtn.onClick.AddListener(OndoBtnOnClick);
        }

        public void RefreshItem()
        {
            csvVitalityData = CSVVitality.Instance.GetConfData(id);
            finishGo.SetActive(false);
            doBtn.gameObject.SetActive(true);
            if (csvVitalityData.Way[0] == 1)
            {
                message.text = LanguageHelper.GetTextContent(csvVitalityData.Description, Sys_Daily.Instance.getDailyAcitvity(csvVitalityData.Way[1]).ToString(), CSVDailyActivity.Instance.GetConfData(csvVitalityData.Way[1]).ActivityNumMax.ToString());
                if (isFinish)
                {
                    finishGo.SetActive(true);
                    doBtn.gameObject.SetActive(false);
                }
            }
            else if (csvVitalityData.Way[0] == 2)
            {
                gameObject.SetActive(true);
                message.text = LanguageHelper.GetTextContent(csvVitalityData.Description, CSVItem.Instance.GetConfData(csvVitalityData.Way[1]).fun_value[1].ToString());
            }
            else if(csvVitalityData.Way[0] == 101)
            {
                message.text = string.Empty;
            }
            else if (csvVitalityData.Way[0] == 102)
            {
                int.TryParse(CSVParam.Instance.GetConfData(1069).str_value.Split('|')[0], out vitalityCostChangeCoin);
                message.text = LanguageHelper.GetTextContent(csvVitalityData.Description, CSVParam.Instance.GetConfData(1069).str_value.Split('|')[0], CSVParam.Instance.GetConfData(1069).str_value.Split('|')[1]);
            }
            ImageHelper.SetIcon(icon, csvVitalityData.Icon);
            btnText.text = LanguageHelper.GetTextContent(csvVitalityData.Button);
            title.text = LanguageHelper.GetTextContent(csvVitalityData.Name);
        }

        private void OndoBtnOnClick()
        {
            if (csvVitalityData.Type == 1)
            {
                uint npcid;
                uint.TryParse(csvVitalityData.Prarameter, out npcid);
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(npcid);
                UIManager.CloseUI(EUIID.UI_Vitality);
                UIManager.CloseUI(EUIID.UI_Attribute);
            }
            else if (csvVitalityData.Type == 2)
            {
                int UIid;
                int.TryParse(csvVitalityData.Prarameter, out UIid);
                if (csvVitalityData.Way[0] == 2)
                {
                    MallPrama mall = new MallPrama();
                    mall.itemId = csvVitalityData.Way[1];
                    mall.mallId = csvVitalityData.Shop[0];
                    mall.shopId = csvVitalityData.Shop[1];
                    UIManager.OpenUI(UIid, false,mall);
                }
                else
                {    
                    UIManager.OpenUI(UIid,false, csvVitalityData.Way[1]);
                }
                UIManager.CloseUI(EUIID.UI_Vitality);
                UIManager.CloseUI(EUIID.UI_Attribute);
            }
            else if (csvVitalityData.Type == 3)
            {

                if (Sys_Bag.Instance.GetItemCount(5) < vitalityCostChangeCoin)
                {
                    Sys_Hint.Instance.PushContent_Normal("活力值不足");
                }
                else
                {
                    Sys_Bag.Instance.EnergyExChangeGoldReq();
                }
            }
        }
    }

    public class UI_Vitality : UIBase, UI_Vitality_Layout.IListener
    {
        private  UI_Vitality_Layout layout = new UI_Vitality_Layout();
        private List<UI_Vitality_Item> listGet = new List<UI_Vitality_Item>();
        private List<UI_Vitality_Item> listChange = new List<UI_Vitality_Item>();
        private uint vitalityMax;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
        }

        private void OnRefreshChangeData(int changeType, int curBoxId)
        {
            SetEnergyData();
        }

        protected override void OnShow()
        {
            uint maxAddByAdventureLv = 0;
            if (CSVAdventureLevel.Instance.TryGetValue(Sys_Adventure.Instance.Level, out CSVAdventureLevel.Data csvData))
            {
                if (csvData.addPrivilege != null)
                {
                    for (int i = 0; i < csvData.addPrivilege.Count; ++i)
                    {
                        if (csvData.addPrivilege[i][0] == 1)
                        {
                            maxAddByAdventureLv = csvData.addPrivilege[i][1];
                            break;
                        }
                    }
                }
            }
            vitalityMax = CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level).VitalityLimit + maxAddByAdventureLv;

            SetEnergyData();
            SetVitalityGetItem();
        }

        protected override void OnHide()
        {            
            DefaultItem();
        }

        private void SetVitalityGetItem()
        {
            listGet.Clear();
            foreach (var data in Sys_Vitality.Instance.SetVitalityGetList())
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.itemGo, layout.itemGo.transform.parent);
                UI_Vitality_Item item = new UI_Vitality_Item(data.key,data.isFinish);
                item.Init(go.transform);
                item.RefreshItem();
                listGet.Add(item);
            }
            layout.itemGo.SetActive(false);
        }

        private void SetVitalityChangeItem()
        {
            listChange.Clear();
            foreach (var data in Sys_Vitality.Instance.SetVitalityChangetList())
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.itemGo, layout.itemGo.transform.parent);
                UI_Vitality_Item item = new UI_Vitality_Item(data.key, data.isFinish);
                item.Init(go.transform);
                item.RefreshItem();
                listChange.Add(item);
            }
            layout.itemGo.SetActive(false);
        }

        private void SetEnergyData()
        {
            if (Sys_Bag.Instance.GetItemCount(5) <= vitalityMax)
            {
                layout.vitalitySlider.value = (float)Sys_Bag.Instance.GetItemCount(5) / vitalityMax;
            }
            else
            {
                layout.vitalitySlider.value = 1;
            }
            layout.vitalityPrecent.text = LanguageHelper.GetTextContent(2009377, Sys_Bag.Instance.GetItemCount(5).ToString(), vitalityMax.ToString());
        }

        private void DefaultItem()
        {
            layout.itemGo.SetActive(true);
            for (int i = 0; i < listGet.Count; ++i) { listGet[i].OnDestroy(); }
            for (int i = 0; i < listChange.Count; ++i) { listChange[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.itemGo.transform.parent.gameObject, layout.itemGo.transform.name);
        }


        public void OnchangeToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                DefaultItem();
                SetVitalityChangeItem();
            }
        }

        public void OngetToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                DefaultItem();
                SetVitalityGetItem();
            }
        }

        public void OntipBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Vitality_Tips);
        }

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Vitality);
        }
    }
}
