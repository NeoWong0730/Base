using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_MessageBox_Share
    {
        public class ShareItem
        {
            private Transform transform;

            private Button _btn;
            private Text _text;

            private uint _channel;
            private Action<uint> _action;
            public void Init(Transform trans)
            {
                transform = trans;

                _btn = transform.Find("ShareButton").GetComponent<Button>();
                _btn.onClick.AddListener(OnClickShare);
                _text = transform.Find("ShareButton/Text").GetComponent<Text>();
            }

            public void Show()
            {
                transform.gameObject.SetActive(true);
            }

            public void Hide()
            {
                transform.gameObject.SetActive(false);
            }

            private void OnClickShare()
            {
                _action?.Invoke(_channel);
            }

            public void SetChannel(uint channel)
            {
                _channel = channel;
                _text.text = LanguageHelper.GetTextContent(2011208 + channel);
            }

            public void Register(Action<uint> action)
            {
                _action = action;
            }
        }

        private Transform transform;

        private Button _btnClose;
        private List<ShareItem> _listShares = new List<ShareItem>(4);
        private TradeItem _tradeItem;
        public void Init(Transform trans)
        {
            transform = trans;

            _btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            _btnClose.onClick.AddListener(OnClickClose);

            Transform parent = transform.Find("ButtonScroll/Viewport");
            int count = parent.childCount;
            for (int i = 0; i < count; ++i)
            {
                ShareItem share = new ShareItem();
                share.Init(parent.GetChild(i));
                share.SetChannel((uint)i);
                share.Register(OnShare);

                _listShares.Add(share);

                //最多三条
                if (i >= 3)
                    share.Hide();
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnClickClose()
        {
            Hide();
        }

        private void OnShare(uint channel)
        {
            //检测聊天功能是否开启
            Sys_Chat.ChatChannelData data = Sys_Chat.Instance.GetChannelData((ChatType)channel);
            if (data == null)
                return;

            if (data.nLvLimit > Sys_Role.Instance.Role.Level)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));//
                return;
            }

            ItemData item = ConstructItemData();
            string content = ParseChatString(item);
            //InputCache input = new InputCache();
            //input.AddContent(ParseChatString(item));
            int errorCode = -1;
            //0世界频道; 1当前频道;  2家族频道
            switch (channel)
            {
                case 0:
                    errorCode = Sys_Chat.Instance.SendContent(ChatType.World, content, Sys_Chat.EExtMsgType.Trade);
                    if (errorCode == Sys_Chat.Chat_Success)
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011212u));
                    break;
                case 1:
                    errorCode = Sys_Chat.Instance.SendContent(ChatType.Local, content, Sys_Chat.EExtMsgType.Trade);
                    if (errorCode == Sys_Chat.Chat_Success)
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011213u));
                    break;
                case 2:
                    if (!Sys_Family.Instance.familyData.isInFamily)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011211));
                    }
                    else
                    {
                        errorCode = Sys_Chat.Instance.SendContent(ChatType.Guild, content, Sys_Chat.EExtMsgType.Trade);
                        if (errorCode == Sys_Chat.Chat_Success)
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011214u));
                    }

                    break;
            }

            UIManager.CloseUI(EUIID.UI_Trade_Box_Com);
            UIManager.CloseUI(EUIID.UI_Trade_Box_Equip);
            UIManager.CloseUI(EUIID.UI_Tips_Pet);
            UIManager.CloseUI(EUIID.UI_Trade_Box_Ornament);
            UIManager.CloseUI(EUIID.UI_Trade_Box_PetEquip);
        }

        private ItemData ConstructItemData()
        {
            if (_tradeItem.GoodsType == 2u) //宠物
            {
                ItemData itemData = new ItemData(0, _tradeItem.Pet.Uid, _tradeItem.InfoId, _tradeItem.Count, 0, false, false, null, null, 0, _tradeItem.Pet);
                itemData.SetQuality(_tradeItem.Pet.SimpleInfo.Quality);
                return itemData;
            }
            else if (_tradeItem.GoodsType == 4u) //饰品
            {
                ItemData itemData = new ItemData(0, _tradeItem.Item.Uuid, _tradeItem.InfoId, _tradeItem.Count, 0, false, false, _tradeItem.Item.Equipment, null, 0, null, null, _tradeItem.Item.Ornament);
                itemData.SetQuality(_tradeItem.Item.Ornament.Color);
                return itemData;
            }
            else if (_tradeItem.GoodsType == 6u) //元核
            {
                ItemData itemData = new ItemData(0, _tradeItem.Item.Uuid, _tradeItem.InfoId, _tradeItem.Count, 0, false,
                    false, null, null, 0, null, null, null, _tradeItem.Item.PetEquip);
                itemData.SetQuality(_tradeItem.Item.PetEquip.Color);
                return itemData;
            }
            else
            {
                ItemData itemData = new ItemData(0, _tradeItem.Item.Uuid, _tradeItem.InfoId, _tradeItem.Count, 0, false, false, _tradeItem.Item.Equipment, null, 0);
                return itemData;
            }
        }

        private string ParseChatString(ItemData tradeItem)
        {
            uint colorIndex = tradeItem.Quality - 1u;
            string color = Constants.gChatColors_Item[colorIndex];

            uint cross = _tradeItem.BCross ? 1u : 0u;
            uint showType = GetShowType();
            ulong goodsId = _tradeItem.GoodsUid;

            System.Text.StringBuilder tempStringBuilder = Lib.Core.StringBuilderPool.GetTemporary();
            tempStringBuilder.Append(EmojiTextHelper.gColorStart);
            tempStringBuilder.Append(Constants.gChatColors_Item[colorIndex]);
            tempStringBuilder.Append(EmojiTextHelper.gTradeItemTag);
            tempStringBuilder.Append(tradeItem.Id.ToString());
            tempStringBuilder.Append(string.Format("_{0}|{1}|{2}", cross, showType, goodsId));
            tempStringBuilder.Append('>');
            tempStringBuilder.Append('[');
            tempStringBuilder.Append(LanguageHelper.GetTextContent(tradeItem.cSVItemData.name_id));
            tempStringBuilder.Append(EmojiTextHelper.gColorEnd);
            string rlt = Lib.Core.StringBuilderPool.ReleaseTemporaryAndToString(tempStringBuilder);
            return rlt;
        }

        private uint GetShowType()
        {
            uint showType = (uint)TradeShowType.OnSale;
            if (_tradeItem.Price == 0u)
                showType = (uint)TradeShowType.Discuss;
            else if (IsPublicity())
                showType = (uint)TradeShowType.Publicity;

            return showType;
        }

        private bool IsPublicity()
        {
            CSVCommodity.Data goodData = CSVCommodity.Instance.GetConfData(_tradeItem.InfoId);
            if (goodData != null)
            {
                if (!goodData.publicity)
                    return false;
                else
                    return _tradeItem.OnsaleTime > Sys_Time.Instance.GetServerTime();
            }
            else
            {
                return false;
            }
        }

        public void SetTradeData(TradeItem tradeItem)
        {
            _tradeItem = tradeItem;
        }
    }
}


