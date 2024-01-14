using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_FamilySale_Record_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/Scroll_Rank").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_FamilySaleRecordCeil
    {
        private FamilySaleCoin dealCoin;
        private Text activeGetAway;
        private Text itemNameAndCout;
        private Text timeText;
        private Text dealTypeText;
        private GameObject noneGo;
        public void Init(Transform transform)
        {
            dealCoin = new FamilySaleCoin();
            dealCoin.Init(transform.Find("Price1/Image_Coin"));
            dealTypeText = transform.Find("Price1/Text_Type").GetComponent<Text>();
            noneGo = transform.Find("Price2").gameObject;
            timeText = transform.Find("Text_Time").GetComponent<Text>();

            activeGetAway = transform.Find("Text_Place").GetComponent<Text>();
            itemNameAndCout = transform.Find("Text_Thing").GetComponent<Text>();
        }

        public void SetInfo(GuildAuctionRecord data)
        {
            if(null != data)
            {
                CSVFamilyAuctionAct.Data actData = CSVFamilyAuctionAct.Instance.GetConfData(data.ActiveId);
                if(null != actData)
                {
                    TextHelper.SetText(activeGetAway, actData.subordinatelangid);
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"Not Find id = {data.ActiveId} in CSVFamilyAuctionAct");
                }
                CSVItem.Data configIntem = CSVItem.Instance.GetConfData(data.ItemInfoId);
                if(null != configIntem)
                {

                    itemNameAndCout.text = LanguageHelper.GetTextContent(11904, LanguageHelper.GetTextContent(configIntem.name_id), data.Count.ToString());
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"Not Find id = {data.ItemInfoId} in CSVItemData");
                }
                bool isNone = data.DealType == 0;
                noneGo.gameObject.SetActive(isNone);
                dealTypeText.transform.parent.gameObject.SetActive(!isNone);
                if (!isNone)
                {
                    dealCoin.SetCoin(data.CurrencyType, data.DealPrice);
                    TextHelper.SetText(dealTypeText, data.DealType == 1 ? 11906u : 11905u); 
                }
                timeText.text = LanguageHelper.TimeToString((uint)data.DealTime, LanguageHelper.TimeFormat.Type_8);
            }
        }
    }

    public class UI_FamilySale_Record : UIBase, UI_FamilySale_Record_Layout.IListener
    {
        private UI_FamilySale_Record_Layout layout = new UI_FamilySale_Record_Layout();
        private List<GuildAuctionRecord> auctionRecord;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnAuctionReocordAckEnd, RefreshView, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            
        }

        protected override void OnShow()
        {
            Sys_Family.Instance.GuildAuctionRecordReq();
            //RefreshView();
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_FamilySaleRecordCeil entry = new UI_FamilySaleRecordCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FamilySaleRecordCeil entry = cell.mUserData as UI_FamilySaleRecordCeil;
            if (index < 0 || index >= auctionRecord.Count)
            {
                return;
            }
            entry.SetInfo(auctionRecord[index]);
        }

        private void RefreshView()
        {
            auctionRecord = Sys_Family.Instance.familyData.familyAuctionInfo.auctionRecord;
            if(null != auctionRecord)
            {
                layout.SetInfinityGridCell(auctionRecord.Count);
            }
            else
            {
                layout.SetInfinityGridCell(0);
            }
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale_My, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilySale_Record);
        }

    }
}