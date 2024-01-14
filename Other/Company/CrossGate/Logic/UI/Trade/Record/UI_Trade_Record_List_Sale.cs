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
    public class UI_Trade_Record_List_Sale
    {
        private class Cell
        {
            private Transform transform;

            private PropSpecialItem propItem;
            private Text textName;
            private Text textPrice;
            private Text textTime;
            private Button btnRecovery;
            private Button btnAppeal;
            private Text textAppealed;

            private TradeSaleRecord _tradeRecord;

            public void Init(Transform trans)
            {
                transform = trans;

                propItem = new PropSpecialItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);

                textName = transform.Find("Text_Name").GetComponent<Text>();
                textPrice = transform.Find("Text_Num").GetComponent<Text>();
                textTime = transform.Find("Text_Time").GetComponent<Text>();

                btnRecovery = transform.Find("Button_Right").GetComponent<Button>();
                btnRecovery.onClick.AddListener(OnClickRecovery);

                btnAppeal = transform.Find("Button_Right2").GetComponent<Button>();
                btnAppeal.onClick.AddListener(OnClickAppeal);

                textAppealed = transform.Find("Text").GetComponent<Text>();

                //Sys_Trade.Instance.eventEmitter.Handle<TradeSaleRecord>(Sys_Trade.EEvents.OnRerordUpdateNtf, OnUpdateNtf, true);
            }

            //public void Destroy()
            //{
            //    Sys_Trade.Instance.eventEmitter.Handle<TradeSaleRecord>(Sys_Trade.EEvents.OnRerordUpdateNtf, OnUpdateNtf, false);
            //}

            private void OnClickRecovery()
            {
                UIManager.OpenUI(EUIID.UI_Trade_Record_Recovery, false, _tradeRecord);
            }

            private void OnClickAppeal()
            {
                UIManager.OpenUI(EUIID.UI_Trade_AppealTip, false, _tradeRecord);
            }

            private void OnUpdateNtf(TradeSaleRecord record)
            {
                if(_tradeRecord != null && _tradeRecord.DealId == record.DealId)
                    OnUpdateInfo(record);
            }

            public void OnUpdateInfo(TradeSaleRecord record)
            {
                _tradeRecord = record;

                textTime.gameObject.SetActive(false);
                btnRecovery.gameObject.SetActive(false);
                btnAppeal.gameObject.SetActive(false);
                textAppealed.gameObject.SetActive(false);

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(record.InfoId);

                ItemData item = new ItemData(99, 1, record.InfoId, record.Count, 0, false, false, null, null, 0, null);
                item.SetQuality(record.Color);

                PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(item, true, false, false);
                showItem.SetTradeEnd(true);
                propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_Record, showItem));

                textName.text = LanguageHelper.GetTextContent(itemData.name_id);

                textPrice.text = _tradeRecord.Coin.ToString();


                textPrice.text = _tradeRecord.Coin.ToString();

                switch ((TradeCheckStatus)_tradeRecord.CheckStatus)
                {
                    case TradeCheckStatus.TradeCheckFreeze:
                        {
                            if (_tradeRecord.ReceiveStep != 2u) //没有领完
                                btnRecovery.gameObject.SetActive(true);

                            if (_tradeRecord.MarkStatus == (int)TradeCheckReviewStatus.TradeCheckReviewNoReview)
                                btnAppeal.gameObject.SetActive(Sys_Trade.Instance.IsShowFreezeAppealBtn(_tradeRecord));
                            else if (_tradeRecord.MarkStatus == (int)TradeCheckReviewStatus.TradeCheckReviewReview)
                                textAppealed.gameObject.SetActive(true);

                        }
                        break;
                    case TradeCheckStatus.TradeCheckPunish:
                    case TradeCheckStatus.TradeCheckKdipFreeze:
                    case TradeCheckStatus.TradeCheckKdipPunish:
                         {
                            if (_tradeRecord.MarkStatus == (int)TradeCheckReviewStatus.TradeCheckReviewNoReview)
                                btnAppeal.gameObject.SetActive(Sys_Trade.Instance.IsShowPunishAppealBtn(_tradeRecord));
                            else if (_tradeRecord.MarkStatus == (int)TradeCheckReviewStatus.TradeCheckReviewReview)
                                textAppealed.gameObject.SetActive(true);
                        }
                        break;
                    case TradeCheckStatus.TradeCheckUndecided:
                        {
                            textTime.gameObject.SetActive(true);
                            uint leftTime = Sys_Trade.Instance.CalAccountTime(_tradeRecord.RecordTime);
                            textTime.text = LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_4);
                        }
                        break;
                    case TradeCheckStatus.TradeCheckPutOff:
                        textTime.gameObject.SetActive(true);
                        textTime.text = LanguageHelper.GetTextContent(2011234);
                        break;
                    case TradeCheckStatus.TradeCheckPass:
                    case TradeCheckStatus.TradeCheckNoCheck:
                        break;
                    default:
                        break;
                }
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        //private Dictionary<GameObject, Cell> dicCells = new Dictionary<GameObject, Cell>();

        private List<TradeSaleRecord> _RecordList = new List<TradeSaleRecord>();
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

            Sys_Trade.Instance.eventEmitter.Handle<TradeSaleRecord>(Sys_Trade.EEvents.OnRerordUpdateNtf, OnRecordUpdateNtf, false);
            Sys_Trade.Instance.eventEmitter.Handle<TradeSaleRecord>(Sys_Trade.EEvents.OnRerordUpdateNtf, OnRecordUpdateNtf, true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
            Sys_Trade.Instance.eventEmitter.Handle<TradeSaleRecord>(Sys_Trade.EEvents.OnRerordUpdateNtf, OnRecordUpdateNtf, false);
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

        public void OnUpdateListInfos(List<TradeSaleRecord> list)
        {
            _RecordList = list;
            _RecordList.Sort((d1, d2) => { return d2.RecordTime.CompareTo(d1.RecordTime); });
            _infinityGrid.CellCount = _RecordList.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }

        private void OnRecordUpdateNtf(TradeSaleRecord record)
        {
            if (_RecordList != null)
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

                if (isRefresh)
                    _infinityGrid.ForceRefreshActiveCell();
            }
        }
    }
}


