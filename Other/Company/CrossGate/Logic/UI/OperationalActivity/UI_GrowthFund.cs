using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Logic.Core;
using Packet;
using UnityEngine.Playables;

namespace Logic
{
    public enum EGrowthFundType
    {
        //需要和成长基金表里的id对应
        LittleFund = 1,
        //BigFund = 2,
    }
    public class UI_GrowthFund : UI_OperationalActivityBase
    {
        CSVGrowthFund.Data fundData;
        private bool isBuy;

        private Button btnBuy;
        private Text txtBuy;
        private GameObject goUnBuy;
        private GameObject goIsBuy;
        private Image imgNPC;
        private Text txtTitle;
        private Timer timer;
        private float countDownTime = 0;

        private List<int> lstFundSortIndex;
        private InfinityGrid infinity;
        private Dictionary<GameObject, UI_GrowthFundCellView> CeilGrids = new Dictionary<GameObject, UI_GrowthFundCellView>();
        /// <summary> 页签 </summary>
        private Dictionary<uint, Toggle> dictPage = new Dictionary<uint, Toggle>();
        private List<Text> listDesc = new List<Text>();

        #region 系统函数
        protected override void Loaded()
        {
        }
        protected override void InitBeforOnShow()
        {
            Parse();
        }
        public override void OnDestroy()
        {
            timer?.Cancel();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            SelectPageOnShow();
        }
        public override void Hide()
        {
            timer?.Cancel();
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateGrowthFundData, OnUpdateGrowthFundData, toRegister);
            SDKManager.eventEmitter.Handle<string>(SDKManager.ESDKLoginStatus.OnSDKPayFailure, OnPayFailure, toRegister);
        }
        #endregion
        #region func
        private void Parse()
        {
            btnBuy = transform.Find("bg/Button").GetComponent<Button>();
            btnBuy.onClick.AddListener(OnBtnBuyClick);
            txtBuy = transform.Find("bg/Button/Text").GetComponent<Text>();
            goUnBuy = transform.Find("bg/Button/Text").gameObject;
            goIsBuy = transform.Find("bg/Button/Text2").gameObject;
            imgNPC = transform.Find("bg/Image2").GetComponent<Image>();
            txtTitle = transform.Find("bg/Image1/Text").GetComponent<Text>();

            infinity = transform.Find("Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
            infinity.onCreateCell += OnCreateCell;
            infinity.onCellChange += OnCellChange;

            Text txt0 = transform.Find("bg/Text").GetComponent<Text>();
            listDesc.Add(txt0);
            Text txt1 = transform.Find("bg/Text1").GetComponent<Text>();
            listDesc.Add(txt1);
            Text txt2 = transform.Find("bg/Text2").GetComponent<Text>();
            listDesc.Add(txt2);

            dictPage.Clear();
            var values = System.Enum.GetValues(typeof(EGrowthFundType));
            for (int i = 0; i < values.Length; i++)
            {
                Toggle toggle = null;
                EGrowthFundType type = (EGrowthFundType)values.GetValue(i);
                switch (type)
                {
                    case EGrowthFundType.LittleFund:
                        {
                            toggle = transform.Find("Content/Toggle0").GetComponent<Toggle>();
                        }
                        break;
                    //case EGrowthFundType.BigFund:
                    //    {
                    //        toggle = transform.Find("Content/Toggle1").GetComponent<Toggle>();
                    //    }
                    //    break;
                }
                toggle.onValueChanged.AddListener((bool value) => OnClickToggle(value, type));
                Text txtDefault = toggle.transform.Find("Text").GetComponent<Text>();
                Text txtSelect = toggle.transform.Find("Text_Select").GetComponent<Text>();
                fundData = CSVGrowthFund.Instance.GetConfData((uint)type);
                txtDefault.text = txtSelect.text = LanguageHelper.GetTextContent(fundData.Title);
                dictPage.Add((uint)type, toggle);
            }
        }
        private void UpdateView(uint actId)
        {
            fundData = CSVGrowthFund.Instance.GetConfData(actId);
            isBuy = Sys_OperationalActivity.Instance.CheckGrowFundIsBuy(fundData.id);
            if(actId == (uint)EGrowthFundType.LittleFund)
            {
                txtTitle.text = LanguageHelper.GetTextContent(4719);//500% <size=36>超值</size>返利！
            }
            //else if(actId == (uint)EGrowthFundType.BigFund)
            //{
            //    txtTitle.text = LanguageHelper.GetTextContent(11758);//300% <size=36>超值</size>返利！
            //}
            goUnBuy.SetActive(!isBuy);
            goIsBuy.SetActive(isBuy);
            if (!isBuy)
            {
                CSVChargeList.Data chargeData = CSVChargeList.Instance.GetConfData(fundData.Charge_Id);
                txtBuy.text = LanguageHelper.GetTextContent(4723, (chargeData.RechargeCurrency / 100).ToString());
            }
            var descIds = fundData.Fun_Des;
            for (int i = 0; i < listDesc.Count; i++)
            {
                if (i < descIds.Count)
                {
                    listDesc[i].gameObject.SetActive(true);
                    listDesc[i].text = LanguageHelper.GetTextContent(descIds[i]);
                }
                else
                {
                    listDesc[i].gameObject.SetActive(false);
                }
            }
            ImageHelper.SetIcon(imgNPC, fundData.Show_Icon);
            lstFundSortIndex = Sys_OperationalActivity.Instance.GetGrowthDataSortIndex(actId);
            infinity.CellCount = lstFundSortIndex.Count;
            infinity.ForceRefreshActiveCell();
            Sys_OperationalActivity.Instance.SetGrowthFundFirstRedPoint(actId);
            UpdateRedPoint();
            UpdateBtnState(false);
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_GrowthFundCellView mCell = new UI_GrowthFundCellView();
            mCell.Init(cell.mRootTransform.transform);
            cell.BindUserData(mCell);
            CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_GrowthFundCellView mCell = cell.mUserData as UI_GrowthFundCellView;
            mCell.UpdateCellView(fundData.id, lstFundSortIndex[index]);
        }
        private void OnClickToggle(bool value, EGrowthFundType type)
        {
            if (value)
            {
                UpdateView((uint)type);
            }
        }
        private void SetPage(EGrowthFundType type)
        {
            Toggle toggle = dictPage[(uint)type];
            if (!toggle.isOn)
            {
                toggle.isOn = true;
            }
            toggle.onValueChanged.Invoke(true);
        }
        private void UpdateRedPoint()
        {
            var values = System.Enum.GetValues(typeof(EGrowthFundType));
            for (int i = 0; i < values.Length; i++)
            {
                EGrowthFundType type = (EGrowthFundType)values.GetValue(i);
                if(dictPage.TryGetValue((uint)type,out Toggle toggle))
                {
                    GameObject goDot = toggle.gameObject.transform.Find("Image_Dot").gameObject;
                    if(goDot != null)
                    {
                        goDot.SetActive(Sys_OperationalActivity.Instance.CheckGrowthFundShowRedPointByActId((uint)type));
                    }
                }
            }
        }
        private void SelectPageOnShow()
        {
            var values = System.Enum.GetValues(typeof(EGrowthFundType));
            bool flag = false;
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EGrowthFundType type = (EGrowthFundType)values.GetValue(i);
                if (Sys_OperationalActivity.Instance.CheckGrowFundIsBuy((uint)type))
                {
                    SetPage(type);
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                SetPage(EGrowthFundType.LittleFund);
            }
        }
        private void UpdateBtnState(bool isGray)
        {
            ImageHelper.SetImageGray(btnBuy.GetComponent<Image>(), isGray, true);
            btnBuy.interactable = !isGray;
        }
        private void StarTimer()
        {
            UpdateBtnState(true);
            countDownTime = uint.Parse(CSVParam.Instance.GetConfData(1061).str_value);
            timer?.Cancel();
            timer = Timer.Register(countDownTime, OnTimerComplete, OnTimerUpdate, false, false);
        }
        #endregion
        #region event
        private void OnBtnBuyClick()
        {
            if (!isBuy)
            {
                StarTimer();
                //跳充值
                Sys_OperationalActivity.Instance.ReportGrowthFundClickEventHitPoint("GotoCharge:" + fundData.Charge_Id);
                Sys_Charge.Instance.OnChargeReq(fundData.Charge_Id);
            }
        }
        private void OnUpdateGrowthFundData()
        {
            if (fundData != null)
            {
                UpdateView(fundData.id);
            }
        }
        private void OnPayFailure(string result)
        {
            if (fundData != null)
            {
                UpdateView(fundData.id);
            }
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            if (fundData != null)
            {
                UpdateView(fundData.id);
            }
        }
        private void OnTimerUpdate(float time)
        {
        }
        #endregion

        public class UI_GrowthFundCellView
        {
            private uint fundId;
            private int index;
            CSVGrowthFund.Data fundData;

            private Transform transform;
            private Text txtBigLv;
            private Text txtRoleLv;
            private Button btnGet;
            private GameObject goIsGet;
            private GameObject goUnActive;

            private List<ItemIdCount> dropItems = new List<ItemIdCount>();
            private InfinityGrid infinity;
            private Dictionary<GameObject, PropItem> CeilGrids = new Dictionary<GameObject, PropItem>();

            public void Init(Transform trans)
            {
                transform = trans;
                txtBigLv = transform.Find("Image_bg/Text_LV").GetComponent<Text>();
                txtRoleLv = transform.Find("Image_bg/Text_LV2").GetComponent<Text>();
                goIsGet = transform.Find("Image_bg/State/Image").gameObject;
                goUnActive = transform.Find("Image_bg/State/Text").gameObject;
                btnGet = transform.Find("Image_bg/State/Btn_01").GetComponent<Button>();
                btnGet.onClick.AddListener(OnBtnGetClick);

                infinity = transform.Find("Image_bg/Scroll View").gameObject.GetNeedComponent<InfinityGrid>();
                infinity.onCreateCell += OnCreateCell;
                infinity.onCellChange += OnCellChange;
            }

            public void UpdateCellView(uint _fundId, int _index)
            {
                index = _index;
                fundId = _fundId;
                fundData = CSVGrowthFund.Instance.GetConfData(fundId);
                var levelList = fundData.level;
                var dropList = fundData.reward_Id;
                if (index < levelList.Count)
                {
                    uint level = levelList[index];
                    txtBigLv.text = level.ToString();
                    txtRoleLv.text = LanguageHelper.GetTextContent(4724, level.ToString());
                    dropItems = CSVDrop.Instance.GetDropItem(dropList[index]);
                    infinity.CellCount = dropItems.Count;
                    infinity.ForceRefreshActiveCell();
                    UpdateBtnState(level);
                }
            }

            private void UpdateBtnState(uint level)
            {
                bool isBuy = Sys_OperationalActivity.Instance.CheckGrowFundIsBuy(fundData.id);
                if (isBuy)
                {
                    goUnActive.SetActive(false);
                    bool canGet = Sys_Role.Instance.Role.Level >= level;
                    if (canGet)
                    {
                        bool isGet = Sys_OperationalActivity.Instance.CheckGrowthFundIsGet(fundId, index);
                        if (isGet)
                        {
                            goIsGet.SetActive(true);
                            btnGet.gameObject.SetActive(false);
                        }
                        else
                        {
                            goIsGet.SetActive(false);
                            btnGet.gameObject.SetActive(true);
                            btnGet.GetComponent<ButtonScaler>().enabled = true;
                            ImageHelper.SetImageGray(btnGet.GetComponent<Image>(), false, true);
                            btnGet.interactable = true;
                        }
                    }
                    else
                    {
                        goIsGet.SetActive(false);
                        btnGet.gameObject.SetActive(true);
                        btnGet.GetComponent<ButtonScaler>().enabled = false;
                        ImageHelper.SetImageGray(btnGet.GetComponent<Image>(), true, true);
                        btnGet.interactable = false;
                    }
                }
                else
                {
                    goUnActive.SetActive(true);
                    goIsGet.SetActive(false);
                    btnGet.gameObject.SetActive(false);
                }
            }

            private void OnBtnGetClick()
            {
                //领取奖励
                Sys_OperationalActivity.Instance.GetGrowFundReward(fundId, (uint)index);
                Sys_OperationalActivity.Instance.ReportGrowthFundClickEventHitPoint("GetReward:" + fundId + "_" + index);
            }

            private void OnCreateCell(InfinityGridCell cell)
            {
                PropItem mCell = new PropItem();
                mCell.BindGameObject(cell.mRootTransform.gameObject);
                cell.BindUserData(mCell);
                CeilGrids.Add(cell.mRootTransform.gameObject, mCell);
            }
            private void OnCellChange(InfinityGridCell cell, int index)
            {
                PropItem mCell = cell.mUserData as PropItem;
                var dropItem = dropItems[index];
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(dropItem.id, dropItem.count, true, false, false, false, false, true);
                mCell.SetData(itemData, EUIID.UI_OperationalActivity);
            }
        }
    }
}
