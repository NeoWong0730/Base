using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    public partial class UI_WarriorGroup : UIBase, UI_WarriorGroup_Layout.IListener
    {
        void RefreshMeeting()
        {
            if (Sys_WarriorGroup.Instance.MyWarriorGroup.NextCreateMeetingTime > 0)
            {
                layout.createMeetTimeText.gameObject.SetActive(true);
                TextHelper.SetText(layout.createMeetTimeText, LanguageHelper.GetTextContent(1002826, LanguageHelper.TimeToString(Sys_WarriorGroup.Instance.MyWarriorGroup.NextCreateMeetingTime, LanguageHelper.TimeFormat.Type_1)));
                createMeetTimer?.Cancel();
                createMeetTimer = Timer.Register(1f, () =>
                {
                    TextHelper.SetText(layout.createMeetTimeText, LanguageHelper.GetTextContent(1002826, LanguageHelper.TimeToString(Sys_WarriorGroup.Instance.MyWarriorGroup.NextCreateMeetingTime, LanguageHelper.TimeFormat.Type_1)));
                }, null, true, true);
            }
            else
            {
                layout.createMeetTimeText.gameObject.SetActive(false);
            }

            RefreshMeetingRedPoint();
        }

        void RefreshMeetingRedPoint()
        {
            layout.currentMeetingRedPoint.SetActive(Sys_WarriorGroup.Instance.HavingUnVoteDoingMeeting());
            layout.historyMeetingRedPoint.SetActive(Sys_WarriorGroup.Instance.HavingUnReadHistoryMeeting());
        }

        public void OnCurrentMeetingToggleValueChanged(bool isOn)
        {
            int count = Sys_WarriorGroup.Instance.MyWarriorGroup.currentMeetingInfos.Count;
            if (count > 0)
            {
                layout.currentMeetingGrid.CellCount = count;
                layout.currentMeetingGrid.ForceRefreshActiveCell();
            }
        }

        public void OnHistoryMeetingToggleValueChanged(bool isOn)
        {
            int count = Sys_WarriorGroup.Instance.MyWarriorGroup.historyMeetingInfos.Count;
            if (count > 0)
            {
                layout.historyMeetingGrid.CellCount = count;
                layout.historyMeetingGrid.ForceRefreshActiveCell();
            }
        }

        void CurrentMeetingGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_Layout.CurrentMeetingInfoItem itemCell = new UI_WarriorGroup_Layout.CurrentMeetingInfoItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void CurrentMeetingGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_Layout.CurrentMeetingInfoItem mCell = cell.mUserData as UI_WarriorGroup_Layout.CurrentMeetingInfoItem;
            List<Sys_WarriorGroup.MeetingInfoBase> meetings = new List<Sys_WarriorGroup.MeetingInfoBase>();
            foreach (var meeting in Sys_WarriorGroup.Instance.GetCurrentMeetingInfos().Values)
            {
                meetings.Add(meeting);
            }
            if (index < meetings.Count)
            {
                var item = meetings[index];
                mCell.UpdateItem(item);
            }
        }

        void HistoryMeetingGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_Layout.HistoryMeetingInfoItem itemCell = new UI_WarriorGroup_Layout.HistoryMeetingInfoItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void HistoryMeetingGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_Layout.HistoryMeetingInfoItem mCell = cell.mUserData as UI_WarriorGroup_Layout.HistoryMeetingInfoItem;
            List<Sys_WarriorGroup.MeetingInfoBase> roleInfos = new List<Sys_WarriorGroup.MeetingInfoBase>();
            foreach (var meeting in Sys_WarriorGroup.Instance.GetHistoryMeetingInfos().Values)
            {
                roleInfos.Add(meeting);
            }
            if (index < roleInfos.Count)
            {
                var item = roleInfos[index];
                mCell.UpdateItem(item);
            }
        }
    }
}
