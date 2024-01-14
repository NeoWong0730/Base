using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Logic.Core;
using System;

namespace Logic
{
    public partial class UI_WarriorGroup : UIBase, UI_WarriorGroup_Layout.IListener
    {
        void RefreshMember()
        {
            TextHelper.SetText(layout.leaveText, string.Empty);
            TextHelper.SetText(layout.pageText, $"{currentPageIndex + 1}/{Mathf.CeilToInt((Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Count / 5.0f))}");
            layout.transferButton.gameObject.SetActive(Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId);
            layout.inviteButton.gameObject.SetActive(Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId);
                    
            quitColdTimer?.Cancel();
            quitColdTimer = Timer.Register(1f, () =>
            {
                DateTime dateTime = Sys_Time.ConvertToLocalTime(Sys_WarriorGroup.Instance.MyWarriorGroup.QuitBeforeThinkingTime);
                TextHelper.SetText(layout.leaveText, LanguageHelper.GetTextContent(13550, dateTime.Hour.ToString(LanguageHelper.gTimeFormat_2), dateTime.Minute.ToString(LanguageHelper.gTimeFormat_2), dateTime.Second.ToString(LanguageHelper.gTimeFormat_2)));
            }, null, true, true);

            if (Sys_WarriorGroup.Instance.MyWarriorGroup.QuitBeforeThinkingTime == 0)
            {
                layout.leaveButton.gameObject.SetActive(true);
                layout.leaveCancelButton.gameObject.SetActive(false);
                TextHelper.SetText(layout.leaveText, string.Empty);
            }
            else
            {
                layout.leaveButton.gameObject.SetActive(false);
                layout.leaveCancelButton.gameObject.SetActive(true);
            }
            
            RefreshTeamButton();
        }

        void RefreshTeamButton()
        {
            if (Sys_WarriorGroup.Instance.MyWarriorGroup.FastInviteTime == 0)
            {
                layout.teamButton.interactable = true;
                TextHelper.SetText(layout.teamButtonText, LanguageHelper.GetTextContent(1002811));
                fastInviteTimer?.Cancel();
                fastInviteTimer = null;
            }
            else
            {
                layout.teamButton.interactable = false;
                TextHelper.SetText(layout.teamButtonText, $"{LanguageHelper.GetTextContent(1002811)}\n({LanguageHelper.TimeToString(Sys_WarriorGroup.Instance.MyWarriorGroup.FastInviteTime, LanguageHelper.TimeFormat.Type_4)})");
                fastInviteTimer?.Cancel();
                fastInviteTimer = Timer.Register(1f, () =>
                {
                    if (Sys_WarriorGroup.Instance.MyWarriorGroup.FastInviteTime != 0)
                    {
                        TextHelper.SetText(layout.teamButtonText, $"{LanguageHelper.GetTextContent(1002811)}\n({LanguageHelper.TimeToString(Sys_WarriorGroup.Instance.MyWarriorGroup.FastInviteTime, LanguageHelper.TimeFormat.Type_4)})");
                    }
                }, null, true, true);
            }
        }

        Dictionary<ulong, WarriorGroupModelShow> warriorGroupShowActors = new Dictionary<ulong, WarriorGroupModelShow>();
        int currentPageIndex = 0;

        public void RefreshMemberActors(int pageIndex)
        {
            layout.UnLoadShowScene();
            layout.LoadShowScene();

            currentPageIndex = pageIndex;
            foreach (var warriorGroupShowActor in warriorGroupShowActors.Values)
            {
                warriorGroupShowActor.Dispose();
            }
            warriorGroupShowActors.Clear();

            List<Sys_WarriorGroup.WarriorInfo> warriorInfos = new List<Sys_WarriorGroup.WarriorInfo>();
            foreach (var warrior in Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Values)
            {
                warriorInfos.Add(warrior);
            }
            warriorInfos.Sort((a, b) =>
            {
                if (a.JoinTime > b.JoinTime)
                    return 1;
                else
                    return -1;
            });

            if ((pageIndex + 1) * 5 <= warriorInfos.Count)
            {
                for (int index = 0, len = 5; index < len; index++)
                {
                    layout.SetMemberModel((UI_WarriorGroup_Layout.EMemeModelShowPos)index, warriorInfos[index + pageIndex * 5]);
                    layout.members[index].Refresh(warriorInfos[index + pageIndex * 5]);
                }
            }
            else
            {
                for (int index = 0, len = warriorInfos.Count - pageIndex * 5; index < len; index++)
                {
                    layout.SetMemberModel((UI_WarriorGroup_Layout.EMemeModelShowPos)index, warriorInfos[index + pageIndex * 5]);
                    layout.members[index].Refresh(warriorInfos[index + pageIndex * 5]);
                }

                for (int index = 0, len = (pageIndex + 1) * 5 - warriorInfos.Count; index < len; index++)
                {
                    layout.members[index + (warriorInfos.Count % 5)].Refresh(null);
                }
            }
        }

        /// <summary>
        /// 点击了离开家族按钮///
        /// </summary>
        public void OnClickLeaveButton()
        {
            if (Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13549));
                return;
            }

            Sys_WarriorGroup.Instance.ReqQuit();
        }

        /// <summary>
        /// 点击了取消离开家族按钮///
        /// </summary>
        public void OnClickLeaveCancelButton()
        {
            Sys_WarriorGroup.Instance.ReqCancelQuit();
        }

        /// <summary>
        /// 点击了转移团长按钮///
        /// </summary>
        public void OnClickTransferButton()
        {
            UIManager.OpenUI(EUIID.UI_WarriorGroup_Transfer);
        }

        /// <summary>
        /// 点击了邀请好友按钮///
        /// </summary>
        public void OnClickInviteButton()
        {
            UIManager.OpenUI(EUIID.UI_WarriorGroup_Invite);
        }

        /// <summary>
        /// 点击了发起组队按钮///
        /// </summary>
        public void OnClickTeamButton()
        {
            Sys_WarriorGroup.Instance.ReqFastTeam();
        }

        /// <summary>
        /// 点击了向左翻页按钮///
        /// </summary>
        public void OnClickLiftButton()
        {
            if (currentPageIndex <= 0)
                return;

            RefreshMemberActors(--currentPageIndex);
            TextHelper.SetText(layout.pageText, $"{currentPageIndex + 1}/{Mathf.CeilToInt((Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Count / 5.0f))}");
        }

        /// <summary>
        /// 点击了向右翻页按钮///
        /// </summary>
        public void OnClickRightButton()
        {
            if ((currentPageIndex + 1) * 5 < Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Count)
            {
                RefreshMemberActors(++currentPageIndex);
                TextHelper.SetText(layout.pageText, $"{currentPageIndex + 1}/{Mathf.CeilToInt((Sys_WarriorGroup.Instance.MyWarriorGroup.warriorInfos.Count / 5.0f))}");
            }
        }
    }
}
