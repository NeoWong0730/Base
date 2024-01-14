using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Record_List_Buy
    {
        private class Cell
        {
            private Transform transform;

            private PropSpecialItem propItem;
            private Text textName;
            private Text textPrice;
            private Text textTime;
            private Button btnRecovery;
            private Button btnCancelAdvanceBuy;
            private Text textAdvanceBuy;

            private TradeBuyRecord _tradeRecord;

            public void Init(Transform trans)
            {
                transform = trans;

                propItem = new PropSpecialItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);

                textName = transform.Find("Text_Name").GetComponent<Text>();
                textPrice = transform.Find("Text_Num").GetComponent<Text>();
                textTime = transform.Find("Text_Time").GetComponent<Text>();
                textAdvanceBuy = transform.Find("Text_State").GetComponent<Text>();

                btnRecovery = transform.Find("Button_Right").GetComponent<Button>();
                btnRecovery.onClick.AddListener(OnClickRecovery);
                
                btnCancelAdvanceBuy = transform.Find("Button02").GetComponent<Button>();
                btnCancelAdvanceBuy.onClick.AddListener(OnCancelAdvanceBuy);
            }

            private void OnClickRecovery()
            {
                Sys_Trade.Instance.OnRecordTakeOutReq(_tradeRecord.RecordUid);
            }

            private void OnCancelAdvanceBuy()
            {
                PromptBoxParameter.Instance.Clear();
                //PromptBoxParameter.Instance.SetCountdown(10f, PromptBoxParameter.ECountdown.Cancel);
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2011911);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Trade.Instance.OnCancelAdvanceOffPriceReq(_tradeRecord.RecordUid);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }

            public void OnUpdateInfo(TradeBuyRecord record)
            {
                _tradeRecord = record;

                textTime.gameObject.SetActive(false);
                btnRecovery.gameObject.SetActive(record.ReceiveState == (int)TradeAdvanceBuyState.ToReceive);
                btnCancelAdvanceBuy.gameObject.SetActive(record.ReceiveState == (int)TradeAdvanceBuyState.Offered);

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(record.InfoId);

                ItemData item = new ItemData(99, 1, record.InfoId, record.Count, 0, false, false, null, null, 0, null);
                item.SetQuality(record.Color);

                PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, true, false, false);
                showItem.SetTradeEnd(true);
                propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_Record, showItem));

                textName.text = LanguageHelper.GetTextContent(itemData.name_id);

                textPrice.text = _tradeRecord.Coin.ToString();

                textAdvanceBuy.text = "";
                textAdvanceBuy.gameObject.SetActive(true);
                if (record.ReceiveState == (int)TradeAdvanceBuyState.Failed)
                {
                    uint lanId = 2011290; //预购结果：失败
                    textAdvanceBuy.text = LanguageHelper.GetTextContent(lanId);
                }
                else if (record.ReceiveState == (int) TradeAdvanceBuyState.CancelOffered)
                {
                    uint lanId = 2011910; //已取消预购
                    textAdvanceBuy.text = LanguageHelper.GetTextContent(lanId);
                }
            }
            
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        //private Dictionary<GameObject, Cell> dicCells = new Dictionary<GameObject, Cell>();

        private List<TradeBuyRecord> _RecordList = new List<TradeBuyRecord>();
        public void Init(Transform trans)
        {
            transform = trans;


            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
            Sys_Trade.Instance.eventEmitter.Handle<TradeBuyRecord>(Sys_Trade.EEvents.OnRecordBuyNtf, OnRecordBuyNtf, false);
            Sys_Trade.Instance.eventEmitter.Handle<TradeBuyRecord>(Sys_Trade.EEvents.OnRecordBuyNtf, OnRecordBuyNtf, true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
            Sys_Trade.Instance.eventEmitter.Handle<TradeBuyRecord>(Sys_Trade.EEvents.OnRecordBuyNtf, OnRecordBuyNtf, false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            Cell entry = new Cell();

            entry.Init(cell.mRootTransform);
            cell.BindUserData(entry);

            //dicCells.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            Cell entry = cell.mUserData as Cell;
            entry.OnUpdateInfo(_RecordList[index]);
        }

        public void OnUpdateListInfos(List<TradeBuyRecord> list)
        {
            _RecordList = list;
            _RecordList.Sort(this.CompareRecord);
            _infinityGrid.CellCount = _RecordList.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }

        private int CompareRecord(TradeBuyRecord d1, TradeBuyRecord d2)
        {
            int state2 = d2.ReceiveState == 1 ? 1 : 0;
            int state1 = d1.ReceiveState == 1 ? 1 : 0;;
            int result = state2.CompareTo(state1);
            if (result == 0)
                return d2.RecordTime.CompareTo(d1.RecordTime);
            else
                return result;
        }

        private void OnRecordBuyNtf(TradeBuyRecord record)
        {
            //Debug.LogError("TradeBuyRecord");
            if(_RecordList != null)
            {
                bool isRefresh = false;
                for (int i = 0; i < _RecordList.Count; ++i)
                {
                    if (_RecordList[i].RecordUid == record.RecordUid)
                    {
                        _RecordList[i] = record;
                        isRefresh = true;
                        break;
                    }
                }

                //Debug.LogError(isRefresh.ToString());
                if (isRefresh)
                    _infinityGrid.ForceRefreshActiveCell();
            }
        }
    }
}


