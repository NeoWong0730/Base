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
    public class UI_FamilySale_My_Layout
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

    public class MyFamilySaleCoin: FamilySaleCoin
    {
        public override void Init(Transform transform)
        {
            coinImage = transform.Find("Image_Coin").GetComponent<Image>();
            coinText = transform.GetComponent<Text>();
        }

        public void SetCoin(uint id, long num, bool max)
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(id);
            ImageHelper.SetIcon(coinImage, itemData.small_icon_id);
            uint styleId = max ? 74u : 75u;
            string str = LanguageHelper.GetTextContent(max ? 12454u : 12455u, num.ToString());
            TextHelper.SetText(coinText, str, CSVWordStyle.Instance.GetConfData(styleId));
        }
    }

    public class UI_MyFamilySaleCeil
    {
        private PropItem item;
        private MyFamilySaleCoin currentCoin;
        private MyFamilySaleCoin myCoin;
        private Button saleBtn;
        private Text timeText;
        private Timer limitTime;
        private GuildAuctionMyInfo itemData;
        public void Init(Transform transform)
        {
            item = new PropItem();
            item.BindGameObject(transform.Find("ListItem").gameObject);
            currentCoin = new MyFamilySaleCoin();
            currentCoin.Init(transform.Find("Present"));
            saleBtn = transform.Find("Btn_Price").GetComponent<Button>();
            saleBtn.onClick.AddListener(OnSaleBtnClick);
            myCoin = new MyFamilySaleCoin();
            myCoin.Init(transform.Find("My"));
            timeText = transform.Find("Text_Time").GetComponent<Text>();
        }

        public void SetInfo(GuildAuctionMyInfo data)
        {
            itemData = data;
            CSVFamilyAuction.Data awardData = CSVFamilyAuction.Instance.GetConfData(data.InfoId);
            if (null != awardData)
            {
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(awardData.ItemId, data.Count, false, false, false, false, false, true, false, true);
                item.SetData(itemN, EUIID.UI_FamilySale_My);
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(awardData.ItemId);
                bool hasItem = null != itemData;
                if (hasItem)
                {
                    TextHelper.SetText(item.txtName, itemData.name_id);
                }
                item.txtName.gameObject.SetActive(hasItem);
                currentCoin.SetCoin(awardData.AuctionCurrency, data.Price);
                myCoin.SetCoin(awardData.AuctionCurrency, data.MyPrice, data.Owned);
                limitTime?.Cancel();
                limitTime = Timer.Register(1, null, (t) =>
                {
                    long time = (long)data.EndTime - (long)Sys_Time.Instance.GetServerTime();
                    if (time >= 0)
                    {
                        timeText.gameObject.SetActive(true);
                        timeText.text = LanguageHelper.TimeToString((uint)time, LanguageHelper.TimeFormat.Type_1);
                    }
                    else
                    {
                        timeText.text = "";
                        limitTime?.Cancel();
                    }
                }, true);
            }
        }

        public void OnHide()
        {
            limitTime?.Cancel();
        }

        private void OnSaleBtnClick()
        {
            UIManager.HitButton(EUIID.UI_FamilySale_My, "OnSaleBtnClick");
            if (null != itemData)
            {
                if (Sys_Family.Instance.familyData.familyAuctionInfo.GetAuctionIsEnd(itemData.ActiveId))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11916)); //当前拍卖已结束，请下次再来
                    return;
                }
                UIManager.OpenUI(EUIID.UI_FamilySale_Detail, false, new Tuple<uint, uint>(itemData.ActiveId, itemData.Id));
            }
        }
    }

    public class UI_FamilySale_My : UIBase, UI_FamilySale_My_Layout.IListener
    {
        private UI_FamilySale_My_Layout layout = new UI_FamilySale_My_Layout();
        private List<GuildAuctionMyInfo> myInfoData;
        private List<UI_MyFamilySaleCeil> ceilList = new List<UI_MyFamilySaleCeil>();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnAuctionAckEnd, RefreshView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnAuctionMyInfoAckEnd, RefreshView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle<uint>(Sys_Family.EEvents.OnAuctionItemChange, RefreshView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle<uint,uint >(Sys_Family.EEvents.OnAuctionItemRemove, RefreshView, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            
        }

        protected override void OnShow()
        {
            //Sys_Family.Instance.GuildAuctionMyInfoReq();
            RefreshView();
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnClose()
        {
            for (int i = 0; i < ceilList.Count; i++)
            {
                ceilList[i].OnHide();
            }
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_MyFamilySaleCeil entry = new UI_MyFamilySaleCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
            ceilList.Add(entry);
        }
        
        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_MyFamilySaleCeil entry = cell.mUserData as UI_MyFamilySaleCeil;
            if (index < 0 || index >= myInfoData.Count)
            {
                return;
            }
            entry.SetInfo(myInfoData[index]);
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale_My, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilySale_My);
        }

        private void RefreshView()
        {
            myInfoData = Sys_Family.Instance.familyData.familyAuctionInfo.GetMyActiveAuctionDicData();
            layout.SetInfinityGridCell(myInfoData.Count);
        }

        private void RefreshView(uint activeId)
        {
            RefreshView();
        }

        private void RefreshView(uint activeId, uint itemId)
        {
            RefreshView();
        }
    }
}