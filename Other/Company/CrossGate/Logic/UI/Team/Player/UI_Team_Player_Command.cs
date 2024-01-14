using UnityEngine;
using System.Collections;
using Logic.Core;
using Logic;
using Packet;
using System.Collections.Generic;
using System;

namespace Logic
{
    public partial class UI_Team_Player : UIBase, UI_Team_Player_Layout.IListener
    {
        interface ICommand
        {
            Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo);
        }

        class TeamCommand : ICommand
        {
            Sys_Role_Info.InfoItem mTeamCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                if (doRoleInfo == null)
                    return null;

                mTeamCommand.mName = doRoleInfo.getTeamID() <= 0 ? LanguageHelper.GetTextContent(2002150) : LanguageHelper.GetTextContent(2002151);

                if (doRoleInfo.getTeamID() <= 0)
                {
                    mTeamCommand.mClickAc = OnClickInviteOther;
                    mTeamCommand.mName = LanguageHelper.GetTextContent(2002150);
                }

                else if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.isTeamMem(doRoleInfo.getRoleID()))
                {
                    mTeamCommand.mClickAc = OnClickApplyKickTeam;
                    mTeamCommand.mName = LanguageHelper.GetTextContent(2002096);
                }
                else
                {
                    if (Sys_Team.Instance.isTeamMem(doRoleInfo.getRoleID()))
                        mTeamCommand.mClickAc = null;
                    else
                        mTeamCommand.mClickAc = OnClickApplyJoinTeam;

                    mTeamCommand.mName = LanguageHelper.GetTextContent(2002151);
                }


                return mTeamCommand;
            }

            /// <summary>
            /// 邀请加入队伍
            /// </summary>
            private void OnClickInviteOther(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                //UIManager.HitButton(EUIID.UI_Team_Invite, "InviteOther");

                Sys_Team.Instance.InvitedOther(doRoleInfo.getRoleID());
            }

            /// <summary>
            /// 申请加入队伍
            /// </summary>
            private void OnClickApplyJoinTeam(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                //UIManager.HitButton(EUIID.UI_Team_Invite, "JoinTeam");

                if (doRoleInfo.getTeamID() > 0)
                {
                    Sys_Team.Instance.ApplyJoinTeam(doRoleInfo.getTeamID(), 0);
                }

            }

            /// <summary>
            /// 请离队伍
            /// </summary>
            /// <param name="doRoleInfo"></param>
            private void OnClickApplyKickTeam(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                Sys_Team.Instance.KickMemTeam(doRoleInfo.getRoleID());
            }
        }

        class FriendCommand : ICommand
        {
            Sys_Role_Info.InfoItem mAddFriendCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                if (Sys_Society.Instance.socialFriendsInfo == null)
                    return null;

                bool isFriend = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().ContainsKey(doRoleInfo.getRoleID());


                if (isFriend)
                {
                    mAddFriendCommand.mName = LanguageHelper.GetTextContent(2004104);

                    mAddFriendCommand.mClickAc = OnClickRemoveFriend;

                    return mAddFriendCommand;
                }

                mAddFriendCommand.mName = LanguageHelper.GetTextContent(2002097);

                mAddFriendCommand.mClickAc = OnClickAddFriend;

                return mAddFriendCommand;
            }

            /// <summary>
            /// 加为好友
            /// </summary>
            private void OnClickAddFriend(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                // UIManager.HitButton(EUIID.UI_Team_Invite, "AddFriend");

                bool isFriend = Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos().ContainsKey(doRoleInfo.getRoleID());

                if (isFriend)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2004200));
                    return;
                }

                Sys_Society.Instance.ReqAddFriend(doRoleInfo.getRoleID());
            }

            /// <summary>
            /// 删除好友
            /// </summary>
            private void OnClickRemoveFriend(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                //UIManager.HitButton(EUIID.UI_Team_Invite, "RemoveFriend");

                Sys_Society.Instance.ReqDelFriend(doRoleInfo.getRoleID());
            }
        }

        class CompeteCommand : ICommand
        {
            Sys_Role_Info.InfoItem mCompareCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                mCompareCommand.mName = LanguageHelper.GetTextContent(4000000);

                if (eType == Sys_Role_Info.EType.Avatar
                    || eType == Sys_Role_Info.EType.Family
                    || eType == Sys_Role_Info.EType.FromAvatar)
                    mCompareCommand.mClickAc = OnClickInviteCompete;
                else
                    mCompareCommand.mClickAc = null;

                return mCompareCommand;
            }

            /// <summary>
            /// 切磋
            /// </summary>
            private void OnClickInviteCompete(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                // UIManager.HitButton(EUIID.UI_Team_Invite, "InviteCompete");

                Sys_Compete.Instance.OnInviteReq(doRoleInfo.getRoleID());
            }
        }

        class MsgCommand : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                mCommand.mName = LanguageHelper.GetTextContent(2002111);

                if (eType != Sys_Role_Info.EType.Friend)
                    mCommand.mClickAc = OnClickSendMsg;
                else
                    mCommand.mClickAc = null;

                return mCommand;
            }

            /// <summary>
            /// 发消息
            /// </summary>
            private void OnClickSendMsg(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                //UIManager.HitButton(EUIID.UI_Team_Invite, "SendMsg");

                //UIManager.OpenUI(EUIID.UI_Society, false, doRoleInfo.getRoleID());
                Sys_Society.Instance.OpenPrivateChat(new Sys_Society.RoleInfo()
                {
                    roleID = doRoleInfo.getRoleID(),
                    roleName = doRoleInfo.getRoleName(),
                    level = doRoleInfo.getLevel(),
                    heroID = doRoleInfo.getHeroID(),
                    occ = doRoleInfo.getRoleOccule(),
                    isOnLine = true,
                });
            }
        }

        class KickMemberCommand : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                mCommand.mName = LanguageHelper.GetTextContent(10535);
                mCommand.mClickAc = OnClickKickMember;
                mCommand.isNeedShow = isShow(doRoleInfo);
                return mCommand;
            }

            /// <summary>
            /// 踢出成员
            /// </summary>
            private void OnClickKickMember(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                //UIManager.HitButton(EUIID.UI_Team_Invite, "KickMember");

                UIManager.OpenUI(EUIID.UI_Family_PromptBox_Leave, false, doRoleInfo.getRoleID());
            }
            /// <summary>
            /// 按钮是否显示
            /// </summary>
            /// <returns></returns>
            public bool isShow(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return doRoleInfo.getRoleID() != Sys_Role.Instance.RoleId &&
                  Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.Clear) &&
                  Sys_Family.Instance.familyData.IsHighStatus(Sys_Role.Instance.RoleId, doRoleInfo.getRoleID());
            }
        }

        class SetStatusCommand : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                var member = Sys_Family.Instance.familyData.CheckMember(doRoleInfo.getRoleID());
                if (null == member) return mCommand;
                if (member.Position % 10000 == (uint)Sys_Family.FamilyData.EFamilyStatus.EApprentice)
                {
                    mCommand.mName = LanguageHelper.GetTextContent(10533);
                    mCommand.mClickAc = OnClickSetMember;
                    mCommand.mOnClickClose = true;
                    mCommand.isNeedShow = isShow_SetMember(doRoleInfo);
                }
                else
                {
                    mCommand.mName = LanguageHelper.GetTextContent(10536);
                    mCommand.mClickAc = OnClickSetStatus;
                    mCommand.mOnClickClose = false;
                    mCommand.isNeedShow = isShow_SetStatus(doRoleInfo);
                }
                return mCommand;
            }

            /// <summary>
            /// 设置成员
            /// </summary>
            /// <param name="doRoleInfo"></param>
            private void OnClickSetMember(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                // UIManager.HitButton(EUIID.UI_Team_Invite, "SetMember");

                Sys_Family.Instance.SendGuildChangePositionReq(doRoleInfo.getRoleID(), (uint)Sys_Family.FamilyData.EFamilyStatus.EMember);
            }
            /// <summary>
            /// 设置职业
            /// </summary>
            private void OnClickSetStatus(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                // UIManager.HitButton(EUIID.UI_Team_Invite, "SetStatus");

                UIBase uIBase = UIManager.GetUI((int)EUIID.UI_Team_Player);
                if (null != uIBase)
                {
                    UI_Team_Player uI_Team_Player = uIBase as UI_Team_Player;
                    uI_Team_Player.ShowFamilyInfo();
                }
            }
            /// <summary>
            /// 按钮是否显示
            /// </summary>
            /// <returns></returns>
            public bool isShow_SetMember(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return doRoleInfo.getRoleID() != Sys_Role.Instance.RoleId &&
                   Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.Worker) &&
                   Sys_Family.Instance.familyData.IsHighStatus(Sys_Role.Instance.RoleId, doRoleInfo.getRoleID());
            }
            /// <summary>
            /// 按钮是否显示
            /// </summary>
            /// <returns></returns>
            public bool isShow_SetStatus(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return doRoleInfo.getRoleID() != Sys_Role.Instance.RoleId &&
                   Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.IsAppointment) &&
                   Sys_Family.Instance.familyData.IsHighStatus(Sys_Role.Instance.RoleId, doRoleInfo.getRoleID());
            }
        }

        class InviteCommand : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                mCommand.mName = LanguageHelper.GetTextContent(11008);
                mCommand.mClickAc = OnClickInvite;
                mCommand.isNeedShow = isShow(doRoleInfo);
                return mCommand;
            }
            /// <summary>
            /// 邀请成员
            /// </summary>
            private void OnClickInvite(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                if (doRoleInfo.getGuildID() == 0)
                {
                    Sys_Family.Instance.SendGuildInviteReq(doRoleInfo.getRoleID());
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11634));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11011));
                }
            }
            /// <summary>
            /// 按钮是否显示
            /// </summary>
            /// <returns></returns>
            public bool isShow(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return doRoleInfo.getRoleID() != Sys_Role.Instance.RoleId && Sys_Family.Instance.familyData.isInFamily &&
                       Sys_Family.Instance.familyData.GetMyPostAuthority(Sys_Family.FamilyData.EFamilyAuthority.Invitation);
            }
        }

        class ApplyCommand : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                mCommand.mName = LanguageHelper.GetTextContent(11009);
                mCommand.mClickAc = OnClickApply;
                mCommand.isNeedShow = isShow(doRoleInfo);
                return mCommand;
            }
            /// <summary>
            /// 申请家族
            /// </summary>
            private void OnClickApply(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                Sys_Family.Instance.SendGuildApplyReq(doRoleInfo.getGuildID(), true);
            }
            /// <summary>
            /// 按钮是否显示
            /// </summary>
            /// <returns></returns>
            public bool isShow(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return doRoleInfo.getRoleID() != Sys_Role.Instance.RoleId && !Sys_Family.Instance.familyData.isInFamily &&
                       doRoleInfo.getGuildID() != 0 && Sys_FunctionOpen.Instance.IsOpen(30401);
            }
        }

        class CreateFamilyCommand : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                mCommand.mName = LanguageHelper.GetTextContent(11632);
                mCommand.mClickAc = OnClickCreateFamily;
                mCommand.isNeedShow = isShow(doRoleInfo);
                return mCommand;
            }
            /// <summary>
            /// 申请家族
            /// </summary>
            private void OnClickCreateFamily(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                UIManager.OpenUI(EUIID.UI_ApplyFamily, false, (int)UI_ApplyFamily.EApplyFamilyMenu.Create);
            }
            /// <summary>
            /// 按钮是否显示
            /// </summary>
            /// <returns></returns>
            public bool isShow(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return doRoleInfo.getRoleID() != Sys_Role.Instance.RoleId && !Sys_Family.Instance.familyData.isInFamily &&
                       doRoleInfo.getGuildID() == 0 && Sys_FunctionOpen.Instance.IsOpen(30401);
            }
        }

        class WatchFighting : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                mCommand.mName = LanguageHelper.GetTextContent(4000001);

                //var hero =  GameCenter.mainWorld.GetActor(Hero.Type,doRoleInfo.getRoleID()) as Hero;
                var hero = GameCenter.GetSceneHero(doRoleInfo.getRoleID());

                if (hero != null && hero.heroBaseComponent.bInFight)
                    mCommand.mClickAc = OnClick;
                else
                    mCommand.mClickAc = null;

                mCommand.isNeedShow = isShow(doRoleInfo);

                return mCommand;
            }

            private void OnClick(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                Sys_Fight.Instance.SendCmdBattleWatchReq(doRoleInfo.getRoleID());
            }
            public bool isShow(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return true;
            }
        }

        class BlackList : ICommand
        {
            Sys_Role_Info.InfoItem mCommand = new Sys_Role_Info.InfoItem();
            public Sys_Role_Info.InfoItem getCommand(Sys_Role_Info.EType eType, Sys_Role_Info.DoRoleInfo doRoleInfo)
            {

               bool isInBlackList = Sys_Society.Instance.IsInBlackList(doRoleInfo.getRoleID());

                uint name = !isInBlackList ? 11661u : 11662u;

                mCommand.mName = LanguageHelper.GetTextContent(name);

                if (isInBlackList)
                    mCommand.mClickAc = OnClickRemoveBalicList;
                else
                    mCommand.mClickAc = OnClickAddBalckList;

                mCommand.isNeedShow = isShow(doRoleInfo);

                return mCommand;
            }

            private void OnClickRemoveBalicList(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                Sys_Society.Instance.ReqRemoveBlackList(doRoleInfo.getRoleID());
            }
            public bool isShow(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                return true;
            }

            private void OnClickAddBalckList(Sys_Role_Info.DoRoleInfo doRoleInfo)
            {
                Sys_Society.Instance.ReqAddBlackList(doRoleInfo.getRoleID());
            }
        }
    }
    public partial class UI_Team_Player : UIBase, UI_Team_Player_Layout.IListener
    {
        List<ICommand> mCommandClasses = new List<ICommand>
        { new MsgCommand(), new FriendCommand(), new TeamCommand(), new CompeteCommand(), new InviteCommand(),
            new ApplyCommand(), new CreateFamilyCommand(),new WatchFighting(),new BlackList()};

        List<ICommand> mFamilyClasses = new List<ICommand>
        { new MsgCommand(), new FriendCommand(), new TeamCommand(), new CompeteCommand(), new KickMemberCommand(), new SetStatusCommand() };

        private void getCommand(Sys_Role_Info.InfoParmas parmas, Sys_Role_Info.DoRoleInfo doRoleInfo)
        {
            if (parmas == null || doRoleInfo == null)
                return;

            switch (parmas.eType)
            {
                case Sys_Role_Info.EType.Family:
                    {
                        for (int i = 0, count = mFamilyClasses.Count; i < count; i++)
                        {
                            var data = mFamilyClasses[i].getCommand(parmas.eType, doRoleInfo);
                            if (data != null && data.isNeedShow)
                                mCommands.Add(data);
                        }
                    }
                    break;
                case Sys_Role_Info.EType.None:
                    break;
                default:
                    {
                        for (int i = 0, count = mCommandClasses.Count; i < count; i++)
                        {
                            var data = mCommandClasses[i].getCommand(parmas.eType, doRoleInfo);
                            if (data != null && data.isNeedShow)
                                mCommands.Add(data);
                        }
                    }
                    break;

            }

        }
    }
}