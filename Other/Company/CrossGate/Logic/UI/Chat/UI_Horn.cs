using System;
using System.Collections.Generic;
using Framework;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Horn : UIBase, UI_Horn_Layout.IListener, IChatInputEvent
    {
        UI_Horn_Layout Layout = null;
        private uint nCurrentHornTable;
        private int nCaretPosition = -1;

        protected override void OnLoaded()
        {            
            Layout = new UI_Horn_Layout();
            Layout.Parse(gameObject);
            Layout.RegisterEvents(this);

            Layout.ipt_Horn_InputField.onEndEdit.AddListener(OnEndEdit);

            Layout.tg_Channel_CP_ToggleRegistry.onToggleChange = OnChannelChange;

            Layout.sv_HornList_InfinityGrid.onCreateCell += OnCreateCell;
            Layout.sv_HornList_InfinityGrid.onCellChange += OnCellChange;
        }

        private void OnChannelChange(int cur, int old)
        {
            switch (cur)
            {
                case 0://SingleServer
                    Sys_Chat.Instance.SelectedHorn(Sys_Chat.Instance.nLastSelectedSingleServerHorn);
                    break;
                case 1://FullServer
                    Sys_Chat.Instance.SelectedHorn(Sys_Chat.Instance.nLastSelectedFullServerHorn);
                    break;
                default:
                    break;
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.HornSelectChange, OnHornSelectChange, toRegister);
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.InputChange, OnInputChange, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int,int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
        }

        private void OnRefreshChangeData(int changeType, int curBoxId)
        {
            //TODO:可以先判断是否是喇叭变更了
            RefreshTable();
            RefreshHorn();
        }

        private void OnInputChange()
        {
            Layout.ipt_Horn_InputField.text = Sys_Chat.Instance.mInputCache.GetContent();
            //Packet.GMAddItemRequest addItemRequest = null;
            //string content = Sys_Chat.Instance.mInputCache.GetSendContent(out addItemRequest);
            //string content2 = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), addItemRequest?.Item, content);
            //Layout.txt_content_EmojiText.text = content2;
        }

        private void RefreshTable()
        {
            Layout.sv_HornList_InfinityGrid.CellCount = Sys_Chat.Instance.nCurrentSelectedHornType == (uint)EItemType.SingleServerHorn ? Sys_Chat.Instance.mSingleServerHornDatas.Count : Sys_Chat.Instance.mFullServerHornDatas.Count;
            Layout.sv_HornList_InfinityGrid.ForceRefreshActiveCell();            
        }

        private void RefreshHorn()
        {
            uint id = Sys_Chat.Instance.nCurrentSelectedHorn;
            CSVHorn.Data hornData = CSVHorn.Instance.GetConfData(id);
            if (hornData != null)
            {
                Layout.txt_costNum_Text.text = hornData.price.ToString();
                ImageHelper.SetIcon(Layout.img_box_Image, null, hornData.background);

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(hornData.id);

                Layout.sv_HornList_InfinityGrid.ForceRefreshActiveCell();
                TextHelper.SetText(Layout.txt_content_EmojiText, LanguageHelper.GetTextContent(itemData.name_id));

                CSVWordStyle.Data wordStyle = CSVWordStyle.Instance.GetConfData(hornData.wordStyle);

                Layout.ipt_Horn_InputField.textComponent.color = wordStyle.FontColor;
                UnityEngine.Color color = wordStyle.FontColor;
                color.a = 0.5f;
                Layout.ipt_Horn_InputField.placeholder.color = color;
                //Layout.txt_costNum_Text.text = hornData.background;
            }
            else
            {
                Layout.txt_costNum_Text.text = string.Empty;
                ImageHelper.SetIcon(Layout.img_box_Image, null, null);

                Layout.sv_HornList_InfinityGrid.ForceRefreshActiveCell();
                TextHelper.SetText(Layout.txt_content_EmojiText, string.Empty);

                //Layout.txt_costNum_Text.text = hornData.background;
            }
        }

        private void OnHornSelectChange()
        {
            if (nCurrentHornTable != Sys_Chat.Instance.nCurrentSelectedHornType)
            {
                RefreshTable();
            }
            RefreshHorn();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_HornEntry entry = new UI_HornEntry();
            entry.BindGameObject(cell.mRootTransform.gameObject);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_HornEntry entry = cell.mUserData as UI_HornEntry;
            entry.SetData(Sys_Chat.Instance.nCurrentSelectedHornType == (uint)EItemType.SingleServerHorn ? Sys_Chat.Instance.mSingleServerHornDatas[index] : Sys_Chat.Instance.mFullServerHornDatas[index]);
        }

        protected override void OnShow()
        {
            Sys_Chat.Instance.eCurrentInput = Sys_Chat.EInputType.Horn;
            OnInputChange();

            nCurrentHornTable = Sys_Chat.Instance.nCurrentSelectedHornType;
            Layout.tg_Channel_CP_ToggleRegistry.SwitchTo(nCurrentHornTable == (uint)EItemType.FullServerHorn ? 1 : 0, false);

            RefreshTable();
            RefreshHorn();
        }

        public void Onclose_ButtonClicked()
        {
            CloseSelf();
            //UIManager.CloseUI(EUIID.UI_ChatInput);
        }

        public void OnEmoji_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_ChatInput, false, this, EUIID.UI_Horn);
        }

        public void OnHorn_InputFieldValueChanged(string arg)
        {
            Sys_Chat.Instance.mInputCache.SetContent(arg);
        }

        public void OnSend_ButtonClicked()
        {
            int rlt = Sys_Chat.Instance.SendContentHorn(Sys_Chat.Instance.nCurrentSelectedHorn, Sys_Chat.Instance.mInputCache);
            if (Sys_Chat.Chat_Success == rlt)
            {
                Sys_Chat.Instance.mInputCache.Clear();
                UIManager.CloseUI(EUIID.UI_Horn);
                //UIManager.CloseUI(EUIID.UI_Chat);
            }
            else
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
        }

        private void OnEndEdit(string arg0)
        {
            nCaretPosition = Layout.ipt_Horn_InputField.caretPosition;
        }

        public void OnChatInputClose()
        {

        }

        public int GetCaretPosition()
        {
            return nCaretPosition;
        }

        public void SetCaretPosition(int caretPosition)
        {
            nCaretPosition = caretPosition;
        }
    }
}