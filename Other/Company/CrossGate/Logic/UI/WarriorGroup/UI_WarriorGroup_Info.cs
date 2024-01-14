using Table;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;

namespace Logic
{
    public partial class UI_WarriorGroup : UIBase, UI_WarriorGroup_Layout.IListener
    {
        void RefreshInfo()
        {
            TextHelper.SetText(layout.groupNameTxt, Sys_WarriorGroup.Instance.MyWarriorGroup.GroupName);
            TextHelper.SetText(layout.groupIDTxt, Sys_WarriorGroup.Instance.MyWarriorGroup.GroupUID.ToString());
            TextHelper.SetText(layout.groupDeclarationTxt, Sys_WarriorGroup.Instance.MyWarriorGroup.GroupDeclaration);
            TextHelper.SetText(layout.groupLeaderNameTxt, Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos[Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID].RoleName);
            TextHelper.SetText(layout.groupRoleCountTxt, $"{Sys_WarriorGroup.Instance.MyWarriorGroup.MemberCount}/{CSVParam.Instance.GetConfData(1376).str_value}");
        }

        void RefreshActions()
        {
            int count = Sys_WarriorGroup.Instance.MyWarriorGroup.actionInfos.Count;
            if (count > 0)
            {
                layout.actionInfinityGrid.CellCount = count;
                layout.actionInfinityGrid.ForceRefreshActiveCell();
                layout.actionInfinityGrid.MoveToIndex(count - 1);
            }
        }

        void ActionInfinityGridCreateCell(InfinityGridCell cell)
        {
            GameObject go = cell.mRootTransform.gameObject;
            UI_WarriorGroup_Layout.ActionInfoItem itemCell = new UI_WarriorGroup_Layout.ActionInfoItem();
            itemCell.BindGameObject(go);
            cell.BindUserData(itemCell);
        }

        void ActionInfinityGridCellChange(InfinityGridCell cell, int index)
        {
            UI_WarriorGroup_Layout.ActionInfoItem mCell = cell.mUserData as UI_WarriorGroup_Layout.ActionInfoItem;
            List<Sys_WarriorGroup.ActionInfo> roleInfos = Sys_WarriorGroup.Instance.GetAllActionInfos();
            if (index < roleInfos.Count)
            {
                var item = roleInfos[index];
                mCell.UpdateItem(item);
            }
        }

        /// <summary>
        /// 点击了举报按钮///
        /// </summary>
        public void OnClickReportButton()
        {
        }

        /// <summary>
        /// 点击了编辑按钮///
        /// </summary>
        public void OnClickEditDeclarationButton()
        {
        }

        /// <summary>
        /// 点击了前往勇者团聊天频道按钮///
        /// </summary>
        public void OnClickEnterChatButton()
        {
            CloseSelf();
            Sys_Chat.Instance.eChatType = ChatType.BraveGroup;
            UIManager.OpenUI(EUIID.UI_Chat, false, null, EUIID.UI_WarriorGroup);
        }
    }
}
