using System.Collections.Generic;

namespace Logic
{
    public class UI_SocietyChatInput : UI_ChatInput
    {
        List<AchievementDataCell> achievementDataCellList = new List<AchievementDataCell>();
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_Society.Instance.eventEmitter.Handle(Sys_Society.EEvents.RecodeChange, OnRecodeChange, toRegister);
        }

        protected override void OnUpdate()
        {
            if (bLSDirty)
            {
                bLSDirty = false;

                Layout.sv_LS_InfinityGrid.CellCount = Sys_Society.Instance.mInputCacheRecord.mContentRecords.Count;
                Layout.sv_LS_InfinityGrid.ForceRefreshActiveCell();
            }
            if (bDJDirty)
            {
                bDJDirty = false;
                List<ItemData> items = null;
                Layout.sv_DJ_InfinityGrid.CellCount = Sys_Bag.Instance.BagItems.TryGetValue(1, out items) ? items.Count : 0;
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

        public override void OnBQClick(string content)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Society.Instance.inputCache.GetContent().Length;

            int rlt = Sys_Society.Instance.inputCache.AddContent(content, caretPosition);

            if (rlt != Sys_Chat.Chat_Success)
            {
                //Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Society.Instance.inputCache.GetContent().Length - len);
                }
            }
        }

        public override void OnLSClick(InputCache cache)
        {
            Sys_Society.Instance.inputCache.CopyFrom(cache);
        }

        protected override void OnDJClick(CeilGrid bagCeilGrid)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Society.Instance.inputCache.GetContent().Length;

            ItemData data = bagCeilGrid.mItemData;
            int rlt = Sys_Society.Instance.inputCache.AddItemContent(data, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                //Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Society.Instance.inputCache.GetContent().Length - len);
                }
            }
        }

        protected override void OnRWClick(UI_ChatRW rw)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Society.Instance.inputCache.GetContent().Length;
            int rlt =  Sys_Society.Instance.inputCache.AddTask(rw.id, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                //Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Society.Instance.inputCache.GetContent().Length - len);
                }
            }
        }

        protected override void OnCHClick(UI_ChatCH ch)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Society.Instance.inputCache.GetContent().Length;

            int rlt = Sys_Society.Instance.inputCache.AddTitleContent(ch.title, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                //Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Society.Instance.inputCache.GetContent().Length - len);
                }
            }
        }

        protected override void OnCWClick(UI_ChatCW cw)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Society.Instance.inputCache.GetContent().Length;

            int rlt = Sys_Society.Instance.inputCache.AddPetContent(cw.mPetData, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());

            if (rlt != Sys_Chat.Chat_Success)
            {
                //Sys_Chat.Instance.PushErrorTip(rlt);
            }
            else
            {
                if (caretPosition >= 0)
                {
                    chatInputEvent.SetCaretPosition(caretPosition + Sys_Society.Instance.inputCache.GetContent().Length - len);
                }
            }
        }

        protected override void OnCellChange_LS(InfinityGridCell cell, int index)
        {
            UI_ChatLS entry = cell.mUserData as UI_ChatLS;
            entry.SetData(Sys_Society.Instance.mInputCacheRecord.mContentRecords[index]);
        }
        protected override void OnCJClick(UI_ChatCJ cj)
        {
            int caretPosition = chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition();
            int len = Sys_Society.Instance.inputCache.GetContent().Length;

            int rlt = Sys_Society.Instance.inputCache.AddAchievement(cj.achData, chatInputEvent == null ? -1 : chatInputEvent.GetCaretPosition());
        }
        protected override void OnCellChange_CJ(InfinityGridCell cell, int index)
        {
            UI_ChatCJ entry = cell.mUserData as UI_ChatCJ;
            entry.SetData(achievementDataCellList[index]);
        }
    }
}
