using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;

namespace Logic
{
    /// <summary> 活动累计消费 </summary>
    public class UI_ActivityTotalConsume : UI_OperationalActivityBase
    {
        Text textDes1;
        Text textTime;
        InfinityGrid infinityGrid;

        List<Sys_OperationalActivity.ActivityTotalConsumeCell> dataList;
        List<Sys_OperationalActivity.ActivityTotalConsumeCell> dataList1;
        bool timeDirty;
        protected override void InitBeforOnShow()
        {
            textDes1 = transform.Find("bg/Text2").GetComponent<Text>();
            textTime = transform.Find("bg/Text_Time").GetComponent<Text>();
            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();

            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateActivityTotalConsume, OnUpdateActivityTotalConsume, toRegister);
        }
        public override void Show()
        {
            base.Show();
            Sys_OperationalActivity.Instance.CmdActivityConsumeDataReq();
            timeDirty = true;
            dataList1 = Sys_OperationalActivity.Instance.GetCSVAccrueConsumeDataList(false);
            SetData();
            SetTime();
            RefreshConsumeData();
        }
        protected override void Update()
        {
            base.Update();
            if (timeDirty)
                SetTime();
        }
        private void OnUpdateActivityTotalConsume()
        {
            RefreshConsumeData();
        }
        private void SetData()
        {
            if (dataList1 != null && dataList1.Count > 0)
                textDes1.text = LanguageHelper.GetTextContent(dataList1[0].csvData.text);
        }
        private void SetTime()
        {
            int second = Sys_OperationalActivity.Instance.GetConsumeDiffTime();
            if (second <= 0)
            {
                second = 0;
                timeDirty = false;
            }
            textTime.text = LanguageHelper.GetTextContent(4754, LanguageHelper.TimeToString((uint)(second), LanguageHelper.TimeFormat.Type_4));
        }
        private void RefreshConsumeData()
        {
            dataList = Sys_OperationalActivity.Instance.GetCSVAccrueConsumeDataList();
            if (dataList != null && dataList.Count > 0)
            {
                infinityGrid.CellCount = dataList.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
        }
        private void OnCreateCell(InfinityGridCell cell)
        {
            TotalConsumeItem entry = new TotalConsumeItem();
            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);
        }
        private void OnCellChange(InfinityGridCell cell, int index)
        {
            TotalConsumeItem entry = cell.mUserData as TotalConsumeItem;
            entry.SetData(dataList[index]);//索引数据
        }

        public class TotalConsumeItem
        {
            InfinityGrid infinityGrid;
            Button btnGet;
            GameObject stateGot;
            Image currencyIcon;
            Text currencyNum;
            Slider slider;
            Text text_percent;

            Sys_OperationalActivity.ActivityTotalConsumeCell data;
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
            public void SetData(Sys_OperationalActivity.ActivityTotalConsumeCell data)
            {
                this.data = data;
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(data.csvData.Consume_Condition[0]);
                ImageHelper.SetIcon(currencyIcon, itemData.icon_id);
                currencyNum.text = data.csvData.Consume_Condition[1].ToString();
                slider.maxValue = data.csvData.Consume_Condition[1];
                uint value = Sys_OperationalActivity.Instance.GetCurencyConsumeNum(data.csvData.Consume_Condition[0]);
                slider.value = value;
                text_percent.text = string.Format("{0}/{1}", value, data.csvData.Consume_Condition[1]);
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
                    Sys_OperationalActivity.Instance.CmdActivityConsumeRewardReq(data.id);
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