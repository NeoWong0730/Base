using Framework;
using Logic.Core;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public enum EChatInputType
    {
        BQ,
        LS,
        DJ,
        RW,
        CW,
        ZQ,
    }

    public interface IChatInputEvent
    {
        void OnChatInputClose();
        int GetCaretPosition();
        void SetCaretPosition(int caretPosition);
    }

    public class UI_ChatInput : UIBase, UI_ChatInput_Layout.IListener
    {
        protected UI_ChatInput_Layout Layout;
        protected IChatInputEvent chatInputEvent = null;
        public int tabID = 0;
        protected bool bLSDirty = true;
        protected bool bDJDirty = true;
        protected bool bRWDirty = true;
        protected bool bCHDirty = true;
        protected bool bCWDirty = true;
        protected bool bCJDirty = true;
        protected List<TaskEntry> taskEntries = null;
        protected Type _openFrom = null;
        EmojiAsset emojiAsset = null;
        List<AchievementDataCell> achievementDataCellList = new List<AchievementDataCell>();

        protected override void OnLoaded()
        {
            Layout = new UI_ChatInput_Layout();
            Layout.Parse(gameObject);
            Layout.RegisterEvents(this);

            Layout.tg_funTab_CP_ToggleRegistry.onToggleChange += OnTabChange;

            Layout.sv_BQ_InfinityGrid.onCreateCell += OnCreateCell_BQ;
            Layout.sv_BQ_InfinityGrid.onCellChange += OnCellChange_BQ;

            Layout.sv_LS_InfinityGrid.onCreateCell += OnCreateCell_LS;
            Layout.sv_LS_InfinityGrid.onCellChange += OnCellChange_LS;

            Layout.sv_DJ_InfinityGrid.onCreateCell += OnCreateCell_DJ;
            Layout.sv_DJ_InfinityGrid.onCellChange += OnCellChange_DJ;

            Layout.sv_RW_InfinityGrid.onCreateCell += OnCreateCell_RW;
            Layout.sv_RW_InfinityGrid.onCellChange += OnCellChange_RW;

            Layout.sv_CH_InfinityGrid.onCreateCell += OnCreateCell_CH;
            Layout.sv_CH_InfinityGrid.onCellChange += OnCellChange_CH;

            Layout.sv_CW_InfinityGrid.onCreateCell += OnCreateCell_CW;
            Layout.sv_CW_InfinityGrid.onCellChange += OnCellChange_CW;

            Layout.sv_CJ_InfinityGrid.onCreateCell += OnCreateCell_CJ;
            Layout.sv_CJ_InfinityGrid.onCellChange += OnCellChange_CJ;
        }

        protected override void OnShow()
        {
            Layout.tg_funTab_CP_ToggleRegistry.SwitchTo(tabID);
            bLSDirty = true;
            bDJDirty = true;
            bCHDirty = true;
            bCJDirty = true;

            if (_openFrom == typeof(UI_Horn))
            {
                Layout.btn_RW.gameObject.SetActive(false);
            }
            else
            {
                Layout.btn_RW.gameObject.SetActive(true);
            }

            emojiAsset = FontManager.GetEmoji(GlobalAssets.sEmoji_0);
            Layout.sv_BQ_InfinityGrid.CellCount = emojiAsset != null ? emojiAsset.Count : 0;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.RecodeChange, OnRecodeChange, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int>(Sys_Bag.EEvents.OnRefreshMainBagData, OnRefreshMainBagData, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, OnTaskDataChange, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, OnTaskDataChange, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnForgoed, OnTaskDataChange, toRegister);
            Sys_Title.Instance.eventEmitter.Handle<uint>(Sys_Title.EEvents.OnTitleGet, OnTitleGet, toRegister);
            Sys_Achievement.Instance.eventEmitter.Handle(Sys_Achievement.EEvents.OnAchievementAdd, OnAchievementAdd, toRegister);
        }
        private void OnTaskDataChange(int taskCategory, uint taskId, TaskEntry taskEntry)
        {
            bRWDirty = true;
        }

        private void OnTitleGet(uint titleId)
        {
            bCHDirty = true;
        }

        private void OnRefreshMainBagData(int boxID)
        {
            bDJDirty = true;
        }
        private void OnAchievementAdd()
        {
            bCJDirty = true;
        }
        protected override void OnUpdate()
        {
            if (bLSDirty)
            {
                bLSDirty = false;

                Layout.sv_LS_InfinityGrid.CellCount = Sys_Chat.Instance.mInputCacheRecord.mContentRecords.Count;
                Layout.sv_LS_InfinityGrid.ForceRefreshActiveCell();
            }
            if (bDJDirty)
            {
                bDJDirty = false;
                List<ItemData> totalItems = new List<ItemData> ();
                List<ItemData> items = null;
                List<ItemData> cardsItems = null;
               if( Sys_Bag.Instance.BagItems.TryGetValue(1, out items))
                {
                    totalItems.AddRange(items);
                }           
                if(Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdShapeshift, out cardsItems))
                {
                    totalItems.AddRange(cardsItems);
                }                          
                Layout.sv_DJ_InfinityGrid.CellCount = totalItems==null?0: totalItems.Count;
                Layout.sv_DJ_InfinityGrid.ForceRefreshActiveCell();
            }
            if (bRWDirty)
            {
                bRWDirty = false;
                taskEntries = Sys_Task.Instance.receivedTaskList;
                Layout.sv_RW_InfinityGrid.CellCount = taskEntries.Count;
                Layout.sv_RW_InfinityGrid.ForceRefreshActiveCell();
            }
            if (bCHDirty)
            {
                bCHDirty = false;
                Layout.sv_CH_InfinityGrid.CellCount = Sys_Title.Instance.GetActiveTitles().Count;
                Layout.sv_CH_InfinityGrid.ForceRefreshActiveCell();
            }
            if (bCWDirty)
            {
                bCWDirty = false;
                Layout.sv_CW_InfinityGrid.CellCount = Sys_Pet.Instance.petsList.Count;
                Layout.sv_CW_InfinityGrid.ForceRefreshActiveCell();
            }
            if (bCJDirty)
            {
                bCJDirty = false;
                achievementDataCellList = Sys_Achievement.Instance.GetAchievementData(0, 0, EAchievementDegreeType.Finished, 1, true, true);
                Layout.sv_CJ_InfinityGrid.CellCount = achievementDataCellList.Count;
                Layout.sv_CJ_InfinityGrid.ForceRefreshActiveCell();
            }
        }

        protected void OnRecodeChange()
        {
            bLSDirty = true;
        }

        private void OnCreateCell_LS(InfinityGridCell cell)
        {
            UI_ChatLS entry = new UI_ChatLS();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.onClick += OnLSClick;

            cell.BindUserData(entry);
        }

        protected virtual void OnCellChange_LS(InfinityGridCell cell, int index)
        {
            UI_ChatLS entry = cell.mUserData as UI_ChatLS;
            entry.SetData(Sys_Chat.Instance.mInputCacheRecord.mContentRecords[index]);
        }

        private void OnCreateCell_DJ(InfinityGridCell cell)
        {
            CeilGrid entry = new CeilGrid();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.AddClickListener(OnDJClick, OnDJLongPressed);

            cell.BindUserData(entry);
        }

        private void OnCellChange_DJ(InfinityGridCell cell, int index)
        {
            CeilGrid entry = cell.mUserData as CeilGrid;
            List<ItemData> totalItems = new List<ItemData>();
            List<ItemData> items = null;
            List<ItemData> cardsItems = null;
            if (Sys_Bag.Instance.BagItems.TryGetValue(1, out items))
            {
                totalItems.AddRange(items);
            }
            if (Sys_Bag.Instance.BagItems.TryGetValue((int)BoxIDEnum.BoxIdShapeshift, out cardsItems))
            {
                totalItems.AddRange(cardsItems);
            }
            if (totalItems !=null && totalItems.Count > index)
            {
                entry.SetData(totalItems[index], index, CeilGrid.EGridState.Normal, CeilGrid.ESource.e_InputChat);
            }
        }

        private void OnCreateCell_RW(InfinityGridCell cell)
        {
            UI_ChatRW entry = new UI_ChatRW();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.onClick += OnRWClick;

            cell.BindUserData(entry);
        }

        private void OnCellChange_RW(InfinityGridCell cell, int index)
        {
            UI_ChatRW entry = cell.mUserData as UI_ChatRW;
            entry.SetData(taskEntries[index]);
        }

        private void OnTabChange(int cur, int old)
        {
            tabID = cur;
        }

        protected override void OnOpen(object arg)
        {
            chatInputEvent = arg as IChatInputEvent;
            _openFrom = arg.GetType();

            if (chatInputEvent == null)
            {
                CloseSelf();
            }
        }

        private void OnCreateCell_BQ(InfinityGridCell cell)
        {
            UI_ChatBQ entry = new UI_ChatBQ();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.onClick += OnBQClick;

            cell.BindUserData(entry);
        }

        private void OnCellChange_BQ(InfinityGridCell cell, int index)
        {
            UI_ChatBQ entry = cell.mUserData as UI_ChatBQ;
            entry.Refresh(string.Format("[{0}]", emojiAsset.GetIDByIndex(index).ToString()));
        }

        private void OnCreateCell_CH(InfinityGridCell cell)
        {
            UI_ChatCH entry = new UI_ChatCH();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.onClick += OnCHClick;

            cell.BindUserData(entry);
        }

        private void OnCellChange_CH(InfinityGridCell cell, int index)
        {
            UI_ChatCH entry = cell.mUserData as UI_ChatCH;
            entry.SetData(Sys_Title.Instance.GetActiveTitles()[index], index);
        }

        private void OnCreateCell_CW(InfinityGridCell cell)
        {
            UI_ChatCW entry = new UI_ChatCW();

            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.onClick += OnCWClick;

            cell.BindUserData(entry);
        }

        private void OnCellChange_CW(InfinityGridCell cell, int index)
        {
            UI_ChatCW entry = cell.mUserData as UI_ChatCW;
            entry.SetData(Sys_Pet.Instance.petsList[index]);
        }
        private void OnCreateCell_CJ(InfinityGridCell cell)
        {
            UI_ChatCJ entry = new UI_ChatCJ();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            entry.onClick += OnCJClick;
            cell.BindUserData(entry);
        }
        protected virtual void OnCellChange_CJ(InfinityGridCell cell, int index)
        {
            UI_ChatCJ entry = cell.mUserData as UI_ChatCJ;
            entry.SetData(achievementDataCellList[index]);
        }
        public void OnCloseFunc_ButtonClicked()
        {
            CloseSelf();
            Sys_Society.Instance.eventEmitter.Trigger(Sys_Society.EEvents.CloseEmoji);
        }

        protected override void OnClose()
        {
            emojiAsset = null;
            chatInputEvent?.OnChatInputClose();
            chatInputEvent = null;
        }

        public virtual void OnBQClick(string content)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Chat.Instance.mInputCache.GetContent().Length;

            int rlt = Sys_Chat.Instance.mInputCache.AddContent(content, caretPosition);

            if (rlt != Sys_Chat.Chat_Success)
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Chat.Instance.mInputCache.GetContent().Length - len);
                }
            }
        }

        public virtual void OnLSClick(InputCache cache)
        {
            Sys_Chat.Instance.mInputCache.CopyFrom(cache);
        }

        protected virtual void OnDJClick(CeilGrid bagCeilGrid)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Chat.Instance.mInputCache.GetContent().Length;

            ItemData data = bagCeilGrid.mItemData;
            int rlt = Sys_Chat.Instance.mInputCache.AddItemContent(data, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Chat.Instance.mInputCache.GetContent().Length - len);
                }
            }
        }

        protected virtual void OnDJLongPressed(CeilGrid bagCeilGrid)
        {
            ItemData itemData = bagCeilGrid.mItemData;

            CSVItem.Data infoData = CSVItem.Instance.GetConfData(itemData.Id);
            if (infoData != null)
            {
                if (infoData.type_id == (uint)EItemType.Equipment)
                {
                    EquipTipsData tipData = new EquipTipsData();
                    tipData.equip = itemData;//new ItemData(itemData.BoxId, itemData.Uuid, itemData.Id, itemData.Count, 0u, false, false, itemData.Equip, null, 0);
                    tipData.isCompare = false;
                    tipData.isShowOpBtn = false;

                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                }
                else if (infoData.type_id == (int)EItemType.PetEquipment)
                {
                    PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                    petEquipTipsData.openUI = EUIID.UI_Chat;
                    petEquipTipsData.petEquip = itemData;
                    petEquipTipsData.isCompare = false;
                    petEquipTipsData.isShowOpBtn = false;
                    petEquipTipsData.isShowLock = false;
                    UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
                }
                else
                {
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(itemData.Id, itemData.Count, true, false, false, false, false);
                    showItem.SetQuality(itemData.Quality);

                    UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Chat, showItem));
                }
            }
        }

        protected virtual void OnRWClick(UI_ChatRW rw)
        {
            //int rlt = Sys_Chat.Instance.SendContent(Sys_Chat.Instance.eChatType, rw.ToSendString());
            //if(rlt != Sys_Chat.Chat_Success)
            //{
            //    Sys_Chat.Instance.PushErrorTip(rlt);
            //}
            //CloseSelf();
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Chat.Instance.mInputCache.GetContent().Length;

            int rlt = Sys_Chat.Instance.mInputCache.AddTask(rw.id, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Chat.Instance.mInputCache.GetContent().Length - len);
                }
            }
        }

        protected virtual void OnCHClick(UI_ChatCH ch)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Chat.Instance.mInputCache.GetContent().Length;

            int rlt = Sys_Chat.Instance.mInputCache.AddTitleContent(ch.title, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Chat.Instance.mInputCache.GetContent().Length - len);
                }
            }
        }

        protected virtual void OnCWClick(UI_ChatCW cw)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Chat.Instance.mInputCache.GetContent().Length;

            int rlt = Sys_Chat.Instance.mInputCache.AddPetContent(cw.mPetData, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Chat.Instance.mInputCache.GetContent().Length - len);
                }
            }
        }
        protected virtual void OnCJClick(UI_ChatCJ cj)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Chat.Instance.mInputCache.GetContent().Length;

            int rlt = Sys_Chat.Instance.mInputCache.AddAchievement(cj.achData, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Chat.Instance.mInputCache.GetContent().Length - len);
                }
            }
        }
    }
}