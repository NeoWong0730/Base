using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;

namespace Logic
{
    /// <summary> 活动累计充值 </summary>
    public class UI_ActivityTotalCharge : UI_OperationalActivityBase
    {
        Text textTime;
        InfinityGrid infinityGrid;

        List<Sys_OperationalActivity.ActivityTotalChargeCell> dataList;
        bool timeDirty;
        protected override void InitBeforOnShow()
        {
            textTime = transform.Find("bg/Text_Time").GetComponent<Text>();
            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityTotalCharge, OnUpdateActivityTotalCharge, toRegister);
        }
        public override void Show()
        {
            base.Show();
            Sys_OperationalActivity.Instance.CmdActivityChargeDataReq();
            timeDirty = true;
            SetTime();
            RefreshChargeData();
        }
        protected override void Update()
        {
            base.Update();
            if (timeDirty)
                SetTime();
        }
        private void OnUpdateActivityTotalCharge()
        {
            RefreshChargeData();
        }
        private void SetTime()
        {
            int second = Sys_OperationalActivity.Instance.GetChargeDiffTime();
            if (second <= 0)
            {
                second = 0;
                timeDirty = false;
            }
            textTime.text = LanguageHelper.GetTextContent(4754, LanguageHelper.TimeToString((uint)(second), LanguageHelper.TimeFormat.Type_4));
        }
        private void RefreshChargeData()
        {
            dataList = Sys_OperationalActivity.Instance.GetCSVAccruePayDataList();
            if (dataList != null && dataList.Count > 0)
            {
                infinityGrid.CellCount = dataList.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            TotalChargeItem entry = new TotalChargeItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            TotalChargeItem entry = cell.mUserData as TotalChargeItem;
            entry.SetData(dataList[index]);//索引数据
        }

        public class TotalChargeItem
        {
            InfinityGrid infinityGrid;
            Button btnGet;
            GameObject stateGot;
            Image currencyIcon;
            Text currencyNum;
            Slider slider;
            Text text_percent;

            Sys_OperationalActivity.ActivityTotalChargeCell data;
            List<ItemIdCount> dropItems;
            public void Init(Transform trans)
            {
                infinityGrid = trans.Find("Image_bg/Scroll View").GetComponent<InfinityGrid>();
                btnGet = trans.Find("Image_bg/State/Btn_01").GetComponent<Button>();
                stateGot = trans.Find("Image_bg/State/Image").gameObject;
                currencyIcon = trans.Find("Image_bg/Cost_Coin").GetComponent<Image>();
                currencyNum = trans.Find("Image_bg/Cost_Coin/Text_Cost").GetComponent<Text>();
                slider = trans.Find("Image_bg/Slider").GetComponent<Slider>();
                text_percent = slider.transform.Find("Text_Percent").GetComponent<Text>();

                infinityGrid.onCreateCell += OnCreateCell;
                infinityGrid.onCellChange += OnCellChange;

                btnGet.onClick.AddListener(OnGetReward);

                slider.minValue = 0;
            }
            public void SetData(Sys_OperationalActivity.ActivityTotalChargeCell data)
            {
                this.data = data;
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(1);
                ImageHelper.SetIcon(currencyIcon, itemData.icon_id);
                currencyNum.text = data.csvData.Recharge.ToString();
                slider.maxValue = data.csvData.Recharge;
                uint value = Sys_OperationalActivity.Instance.GetCurencyChargeNum();
                slider.value = value;
                text_percent.text = string.Format("{0}/{1}", value, data.csvData.Recharge);
                if (data.isCanShow)
                {
                    if (data.isGet)
                    {
                        btnGet.gameObject.SetActive(false);
                        stateGot.gameObject.SetActive(true);
                    }
                    else
                    {
                        btnGet.gameObject.SetActive(true);
                        stateGot.gameObject.SetActive(false);
                        btnGet.interactable = true;
                        btnGet.GetComponent<ButtonScaler>().enabled = true;
                        ImageHelper.SetImageGray(btnGet.GetComponent<Image>(), false, true);
                    }
                }
                else
                {
                    btnGet.gameObject.SetActive(true);
                    stateGot.gameObject.SetActive(false);
                    btnGet.interactable = false;
                    btnGet.GetComponent<ButtonScaler>().enabled = false;
                    ImageHelper.SetImageGray(btnGet.GetComponent<Image>(), true, true);
                }
                RefreshReward();
            }
            private void OnGetReward()
            {
                if (data != null)
                    Sys_OperationalActivity.Instance.CmdActivityChargeRewardReq(data.id);
            }
            private void RefreshReward()
            {
                dropItems = CSVDrop.Instance.GetDropItem(data.csvData.reward);
                infinityGrid.CellCount = dropItems.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
            private void OnCreateCell(InfinityGridCell cell)
            {
                PropItem entry = new PropItem();
                entry.BindGameObject(cell.mRootTransform.gameObject);
                cell.BindUserData(entry);
            }
            private void OnCellChange(InfinityGridCell cell, int index)
            {
                PropItem entry = cell.mUserData as PropItem;
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                              (_id: dropItems[index].id,
                              _count: dropItems[index].count,
                              _bUseQuailty: true,
                              _bBind: false,
                              _bNew: false,
                              _bUnLock: false,
                              _bSelected: false,
                              _bShowCount: true,
                              _bShowBagCount: false,
                              _bUseClick: true,
                              _onClick: null,
                              _bshowBtnNo: false);
                entry.SetData(new MessageBoxEvt(EUIID.UI_OperationalActivity, showItem));
            }
        }
    }
}