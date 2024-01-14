using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;

namespace Logic
{
    public class BackAssitCeil
    {
        private Text txt_Name;
        private GameObject go_PropItem;
        private GameObject go_Cost;
        private Text txt_Cost;
        private GameObject go_Refresh;
        private Button btn_Exchange;

        private uint m_index;
        private CSVReturnArdentReward.Data m_Data;
        public BackAssitCeil(uint m_index)
        {
            this.m_index = m_index;
            m_Data = CSVReturnArdentReward.Instance.GetConfData(m_index);
        }

        public void Init(Transform trans)
        {
            txt_Name = trans.Find("Text_Name").GetComponent<Text>();
            go_PropItem = trans.Find("ItemGroup/PropItem").gameObject;
            go_Cost= trans.Find("Text_Cost").gameObject;
            txt_Cost= trans.Find("Text_Cost").GetComponent<Text>();
            go_Refresh = trans.Find("Text_Refresh").gameObject;
            btn_Exchange= trans.Find("Btn_Exchange").GetComponent<Button>();
            btn_Exchange.onClick.AddListener(OnExChangeButtonClick);
        }

        public void SetData()
        {
            txt_Name.text = LanguageHelper.GetTextContent(m_Data.pack_name);
            PropItemShow();
            StateShow();
        }
        public void RefreshState()
        {
            StateShow();
        }
        private void PropItemShow()
        {
            DefaultItem();
            var list_drop = CSVDrop.Instance.GetDropItem(m_Data.drop_Id);
            FrameworkTool.CreateChildList(go_PropItem.transform.parent, list_drop.Count);
            for (int i = 0; i < list_drop.Count; i++)
            {
                GameObject _propGo = go_PropItem.transform.parent.GetChild(i).gameObject;
                PropItem _propItem = new PropItem();
                _propItem.BindGameObject(_propGo);
                ItemIdCount itemIdCount = list_drop[i];
                _propItem.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false,_bShowCount: true, _bUseClick: true, _bShowBagCount: false)));
            }

        }
        private void StateShow()
        {//0-fasle 1 2 3 etc.-true
            bool isExChange = Sys_BackAssist.Instance.LoveRewardDictionary[m_index] != 0;
            txt_Cost.text = m_Data.price_pack.ToString();
            go_Cost.SetActive(!isExChange);
            go_Refresh.SetActive(isExChange);
            btn_Exchange.gameObject.SetActive(!isExChange);
            bool isCanBuy = Sys_BackAssist.Instance.TotalLovePoint >= m_Data.price_pack;
            btn_Exchange.enabled = isCanBuy;
            ImageHelper.SetImageGray(btn_Exchange.GetComponent<Image>(),!isCanBuy);//热心值不够置灰

        }
        private void DefaultItem()
        {
            FrameworkTool.DestroyChildren(go_PropItem.transform.parent.gameObject, go_PropItem.transform.name);
        }
        private void OnExChangeButtonClick()
        {
            if (!Sys_BackAssist.Instance.CheckActivityReturnLovePointOpen())
            {
                Sys_Hint.Instance.PushContent_Normal(CSVErrorCode.Instance.GetConfData(107701).words);
                return;
            }
            Sys_BackAssist.Instance.OnActivityReturnLovePointRewardReq(m_index);
        }
    }
    public class UI_BackAssist : UI_OperationalActivityBase
    {
        #region 界面显示
        private GameObject go_Grid;
        private Slider sl_Slider;
        private Text txt_Slider;
        private Button btn_PointList;
        private Button btn_Explain;
        private Text txt_DayLovePoint;
        #endregion
        private List<BackAssitCeil> AwardCeilList = new List<BackAssitCeil>();
        #region 系统函数
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void Show()
        {
            base.Show();
            Sys_BackAssist.Instance.OnActivityReturnLovePointDataReq();
        }
        public override void Hide()
        {
            base.Hide();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_BackAssist.Instance.eventEmitter.Handle(Sys_BackAssist.EEvents.UpdateActivityReturnLovePointData, InitShow, toRegister);
            Sys_BackAssist.Instance.eventEmitter.Handle(Sys_BackAssist.EEvents.ActivityReturnLovePointAwardRes, OnUpdateActivityReturn, toRegister);
        }
        #endregion
        #region Function

        private void Parse()
        {
            go_Grid = transform.Find("Grid").gameObject;
            sl_Slider = transform.Find("rexinzhi/Slider").GetComponent<Slider>();
            txt_Slider = transform.Find("rexinzhi/Text_Value").GetComponent<Text>();
            btn_PointList = transform.Find("rexinzhi/Btn_Details").GetComponent<Button>();
            btn_Explain = transform.Find("Get/Btn_Check").GetComponent<Button>();
            txt_DayLovePoint = transform.Find("Get/Text_Value").GetComponent<Text>();
            btn_PointList.onClick.AddListener(OnButtonPointListClick);
            btn_Explain.onClick.AddListener(OnButtonExplainClick);
        }

        private void InitShow()
        {
            AwardCeilList.Clear();
            for (int i = 0; i < go_Grid.transform.childCount; i++)
            {
                BackAssitCeil _ceil = new BackAssitCeil((uint)i + 1);
                _ceil.Init(go_Grid.transform.GetChild(i));
                _ceil.SetData();
                AwardCeilList.Add(_ceil);
            }
            TotalTotalLovePointShow();
            TodayTotalLovePointShow();
        }
        private void TotalTotalLovePointShow()
        {
            var _pData = CSVReturnParam.Instance.GetConfData(5);//热心值总上限
            txt_Slider.text = LanguageHelper.GetTextContent(2009836, Sys_BackAssist.Instance.TotalLovePoint.ToString(), _pData.str_value);
            float _value = float.Parse(_pData.str_value);
            sl_Slider.value = Sys_BackAssist.Instance.TotalLovePoint / _value;
        }

        private void TodayTotalLovePointShow()
        {
            var _pData = CSVReturnParam.Instance.GetConfData(6);//热心值单日上限
            txt_DayLovePoint.text= LanguageHelper.GetTextContent(2009836, Sys_BackAssist.Instance.TodayLovePoint.ToString(), _pData.str_value);
        }
        private void OnUpdateActivityReturn()
        {
            for (int i=0;i<AwardCeilList.Count;i++)
            {
                AwardCeilList[i].RefreshState();
            }
            TotalTotalLovePointShow();
            TodayTotalLovePointShow();
        }
        private void OnButtonPointListClick()
        {
            UIManager.OpenUI(EUIID.UI_PointGetTips,false,1);
        }
        private void OnButtonExplainClick()
        {
            PointRuleEvt evt = new PointRuleEvt();
            evt.titleLan = 2014917;
            evt.contentLan = 2014916;
            UIManager.OpenUI(EUIID.UI_PointRuleTips, false, evt);
        }
        #endregion

    }

}