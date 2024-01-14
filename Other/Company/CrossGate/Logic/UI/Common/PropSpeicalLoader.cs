using System.Collections;
using System.Collections.Generic;
using Lib.AssetLoader;
using UnityEngine.UI;
using UnityEngine;
using System;
using Table;
using UnityEngine.EventSystems;
using Logic.Core;
using Framework;

namespace Logic
{

    public static class PropSpeicalLoader
    {
        public class MessageBoxEvt
        {
            public EUIID sourceUiId;
            public PropSpeicalLoader.ShowItemData itemData;

            public MessageBoxEvt() { }
            public MessageBoxEvt(EUIID _sourceUiid, PropSpeicalLoader.ShowItemData _itemData)
            {
                Reset(_sourceUiid, _itemData);
            }
            public MessageBoxEvt Reset(EUIID _sourceUiid, PropSpeicalLoader.ShowItemData _itemData)
            {
                sourceUiId = _sourceUiid;
                itemData = _itemData;
                return this;
            }
        }

        public class ShowItemData
        {
            public ItemData itemData { get; private set; }
            public bool bShowCount { get; private set; }
            public bool bShowBagCount { get; private set; }
            public bool bTradeEnd { get; private set; }
            public bool bUseClick { get; private set; }

            public bool bCross { get; private set; }

            public uint Level { get; private set; }

            public uint operationType { get; private set; } //0交易行

            public Action<PropSpecialItem> ClickAction { get; private set; }

            public ShowItemData() { }
            public ShowItemData(ItemData itemData, bool showCount = false, bool showBagCount = false, bool useClick = true)
            {
                this.itemData = itemData;
                this.bShowCount = showCount;
                this.bShowBagCount = showBagCount;
                this.bUseClick = useClick;

                this.bTradeEnd = this.itemData.bMarketEnd;
            }

            public void SetTradeEnd(bool tradeEnd)
            {
                this.bTradeEnd = tradeEnd;
            }

            public void SetCross(bool cross)
            {
                this.bCross = cross;
            }

            public void SetLevel(uint level)
            {
                this.Level = level;
            }

            public void SetOperationType(uint opType)
            {
                this.operationType = opType;
            }

            public void AddClickAction(Action<PropSpecialItem> action)
            {
                ClickAction = action;
            }

            public void UpdateItemData(ItemData item)
            {
                this.itemData = item;
            }
        }
    }



}


