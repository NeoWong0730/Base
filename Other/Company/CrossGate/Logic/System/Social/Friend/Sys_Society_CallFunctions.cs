using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 领取礼物
        /// </summary>
        /// <param name="roleID"></param>
        public void GetGift(ulong roleID)
        {
            CmdSocialGetGiftsReq req = new CmdSocialGetGiftsReq();
            req.RoleId = roleID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.GetGiftsReq, req);
        }

        /// <summary>
        /// 发送礼物///
        /// </summary>
        /// <param name="roleID"></param>
        /// <param name="itemID"></param>
        /// <param name="count"></param>
        public void SendGift(ulong roleID, uint itemID, uint count)
        {
            CmdSocialSendGiftsReq req = new CmdSocialSendGiftsReq();
            req.DstRoleId = roleID;
            req.ItemId = itemID;
            req.Count = count;

            NetClient.Instance.SendMessage((ushort)CmdSocial.SendGiftsReq, req);
        }

        #region Call Role

        /// <summary>
        /// 打开好友列表时发送请求///
        /// CD 30s//
        /// 申请获取陌生人最近队友和好友的信息///
        /// </summary>
        public void ReqGetSocialRoleInfo(bool initiative = true)
        {
            CmdSocialGetFriendInfoReq req = new CmdSocialGetFriendInfoReq();
            req.Initiative = initiative;
            var erator = socialRecentlysInfo.recentlyIdsDic.GetEnumerator();
            while (erator.MoveNext())
            {
                if (!socialFriendsInfo.friendsIdsDic.ContainsKey(erator.Current.Key))
                {
                    req.Roleid.Add(erator.Current.Key);
                }
            }

            var erator2 = socialTeamMembersInfo.teamMemberIdsDic.GetEnumerator();
            while (erator2.MoveNext())
            {
                if (!socialFriendsInfo.friendsIdsDic.ContainsKey(erator2.Current.Key))
                {
                    req.Roleid.Add(erator2.Current.Key);
                }
            }

            NetClient.Instance.SendMessage((ushort)CmdSocial.GetFriendInfoReq, req);
        }


        /// <summary>
        /// 加好友///
        /// </summary>
        /// <param name="roleID"></param>
        public void ReqAddFriend(ulong roleID)
        {
            ///是玩家自己///
            if (roleID == Sys_Role.Instance.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11840));
                return;
            }

            ///已经是好友了///
            if (socialFriendsInfo.friendsIdsDic.ContainsKey(roleID))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11841));
                return;
            }

            ///在我的黑名单中///
            if (socialBlacksInfo.blacksIdsDic.ContainsKey(roleID))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11842));
                return;
            }

            if (socialFriendsInfo.friendsIdsDic.Count >= friendsMaxCount)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11843));
                return;
            }

            CmdSocialAddFriendReq req = new CmdSocialAddFriendReq();
            req.RoleId = roleID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.AddFriendReq, req);
        }

        /// <summary>
        /// 删好友///
        /// </summary>
        /// <param name="roleID"></param>
        public void ReqDelFriend(ulong roleID)
        {
            if (socialFriendsInfo.friendsIdsDic.ContainsKey(roleID))
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(13020, socialRolesInfo.rolesDic[roleID].roleName);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    CmdSocialDelFriendReq req = new CmdSocialDelFriendReq();
                    req.RoleId = roleID;

                    NetClient.Instance.SendMessage((ushort)CmdSocial.DelFriendReq, req);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);               
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11844));
            }
        }


        /// <summary>
        /// 发送私聊///
        /// </summary>
        /// <param name="targetRoleID"></param>
        /// <param name="msg"></param>
        public void ReqChatSingle(ulong targetRoleID, string msg, bool needExtraData = true)
        {         
            string content = string.Empty;
            byte[] extraData;
            if (needExtraData)
            {
                ChatExtMsg addItemRequest = null;
                content = Instance.inputCache.GetSendContent(out addItemRequest);
                extraData = addItemRequest != null ? NetMsgUtil.Serialzie(addItemRequest) : null;
            }
            else
            {
                content = msg;
                extraData = null;
            }

            CmdSocialChatSingleReq req = new CmdSocialChatSingleReq();
            req.RoleId = targetRoleID;
            req.ChatMsg = FrameworkTool.ConvertToGoogleByteString(content);
            if (extraData != null)
            {
                req.ExtraMsg = Google.Protobuf.ByteString.CopyFrom(extraData);
            }
            NetClient.Instance.SendMessage((ushort)CmdSocial.ChatSingleReq, req);

            ///最近聊天中没有的话此时加入最近聊天当中///
            if (!socialRecentlysInfo.recentlyIdsDic.ContainsKey(targetRoleID))
            {
                socialRecentlysInfo.AddRecentlyInfo(targetRoleID);
                SerializeRecentlysInfoToJsonFile();
            }          
        }

        /// <summary>
        /// 好友分组创建,添加,增量///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="oldName"></param>
        /// <param name="roleIDs"></param>
        public void ReqFriendGroup(string name, string oldName, List<ulong> roleIDs, bool isCreate = false)
        {
            if (name == MYFRIEND || name == TEAMPLAYER || name == BLACK)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13016));
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13017));
                return;
            }

            if (isCreate)
            {
                if (socialFriendGroupsInfo.friendGroupInfosDic.ContainsKey(name))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13019));
                    return;
                }
            }
            else
            {
                if (socialFriendGroupsInfo.friendGroupInfosDic.ContainsKey(name) && name != oldName)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13019));
                    return;
                }
            }

            CmdSocialFriendsGroupReq req = new CmdSocialFriendsGroupReq();
            req.Name = FrameworkTool.ConvertToGoogleByteString(name);
            req.OldName = FrameworkTool.ConvertToGoogleByteString(oldName);
            for (int index = 0, len = roleIDs.Count; index < len; index++)
            {
                req.RoleId.Add(roleIDs[index]);
            }

            NetClient.Instance.SendMessage((ushort)CmdSocial.FriendsGroupReq, req);
        }

        /// <summary>
        /// 删除好友分组///
        /// </summary>
        /// <param name="friendGroupInfo"></param>
        public void ReqDelFriendGroup(FriendGroupInfo friendGroupInfo)
        {
            if (socialFriendGroupsInfo.friendGroupInfosDic.ContainsKey(friendGroupInfo.name))
            {
                CmdSocialDelFriendsGroupReq req = new CmdSocialDelFriendsGroupReq();
                req.Name = FrameworkTool.ConvertToGoogleByteString(friendGroupInfo.name);

                NetClient.Instance.SendMessage((ushort)CmdSocial.DelFriendsGroupReq, req);
            }
        }

        /// <summary>
        /// 请求设置社交信息///
        /// </summary>
        /// <param name="ageID"></param>
        /// <param name="sexID"></param>
        /// <param name="AreaID"></param>
        /// <param name="hobbys"></param>
        public void SetSocialInfoReq(uint ageID, uint sexID, uint AreaID, List<uint> hobbys)
        {
            CmdSocialSetMySocialInfoReq req = new CmdSocialSetMySocialInfoReq();
            req.Age = ageID;
            req.Sex = sexID;
            if (hobbys != null)
            {
                for (int index = 0, len = hobbys.Count; index < len; index++)
                {
                    req.Hobby.Add(hobbys[index]);
                }
            }
            req.Area = AreaID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.SetMySocialInfoReq, req);
        }

        /// <summary>
        /// 请求玩家信息///
        /// </summary>
        /// <param name="inputString"></param>
        public void ReqGetBriefInfo(string inputString)
        {
            CmdSocialGetBriefInfoReq req = new CmdSocialGetBriefInfoReq();
            req.Classify = (uint)EGetBriefInfoClassify.SocialSearchRole;
            ulong resultID;
            ulong.TryParse(inputString, out resultID);

            if (resultID != 0)
            {
                req.RoleId = resultID;
                req.Name = FrameworkTool.ConvertToGoogleByteString(inputString);
            }
            else
            {
                req.Name = FrameworkTool.ConvertToGoogleByteString(inputString);
            }

            NetClient.Instance.SendMessage((ushort)CmdSocial.GetBriefInfoReq, req);
        }     

        /// <summary>
        /// 请求类别搜索好友///
        /// </summary>
        public void ReqSearchFriendByType(uint typeID)
        {
            CmdSocialSearchFriendReq req = new CmdSocialSearchFriendReq();
            req.RoleId = CacheLastSearchRoleID;
            req.ConditionID = typeID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.SearchFriendReq, req);
        }

        /// <summary>
        /// 请求关系搜索好友///
        /// </summary>
        public void ReqSearchFriendByRelation()
        {
            CmdSocialRelationFriendReq req = new CmdSocialRelationFriendReq();

            NetClient.Instance.SendMessage((ushort)CmdSocial.RelationFriendReq, req);
        }

        /// <summary>
        /// 加入黑名单///
        /// </summary>
        /// <param name="roleID"></param>
        public void ReqAddBlackList(ulong roleID)
        {
            if (socialBlacksInfo.blacksIdsDic.ContainsKey(roleID))
                return;

            if (socialFriendsInfo.friendsIdsDic.ContainsKey(roleID))
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(11663, socialRolesInfo.rolesDic[roleID].roleName);
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    CmdSocialDelFriendReq req = new CmdSocialDelFriendReq();
                    req.RoleId = roleID;

                    NetClient.Instance.SendMessage((ushort)CmdSocial.DelFriendReq, req);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
            else
            {
                CmdSocialAddBlackListReq req = new CmdSocialAddBlackListReq();
                req.RoleId = roleID;

                NetClient.Instance.SendMessage((ushort)CmdSocial.AddBlackListReq, req);
            }
        }

        /// <summary>
        /// 删除黑名单///
        /// </summary>
        /// <param name="roleID"></param>
        public void ReqRemoveBlackList(ulong roleID)
        {
            if (!socialBlacksInfo.blacksIdsDic.ContainsKey(roleID))
                return;

            CmdSocialRemoveBlackListReq req = new CmdSocialRemoveBlackListReq();
            req.RoleId = roleID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.RemoveBlackListReq, req);
        }

        #endregion

        #region Call Group    

        /// <summary>
        /// 每次打开群组界面请求///
        /// </summary>
        public void ReqGetGroupBriefInfo()
        {
            CmdSocialGetGroupBriefInfoReq req = new CmdSocialGetGroupBriefInfoReq();

            NetClient.Instance.SendMessage((ushort)CmdSocial.GetGroupBriefInfoReq, req);
        }

        /// <summary>
        /// 某个群详细信息请求///
        /// </summary>
        /// <param name="groupID"></param>
        public void ReqGetGroupDetailInfo(uint groupID)
        {
            CmdSocialGetGroupDetailInfoReq req = new CmdSocialGetGroupDetailInfoReq();
            req.GroupId = groupID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.GetGroupDetailInfoReq, req);
        }

        /// <summary>
        /// 群公告设置///
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="notice"></param>
        public void ReqSetGroupNotice(uint groupID, string notice)
        {
            CmdSocialSetGroupNoticeReq req = new CmdSocialSetGroupNoticeReq();
            req.GroupId = groupID;
            req.Notice = FrameworkTool.ConvertToGoogleByteString(notice);

            NetClient.Instance.SendMessage((ushort)CmdSocial.SetGroupNoticeReq, req);
        }

        /// <summary>
        /// 自己请求退出群组///
        /// </summary>
        /// <param name="groupID"></param>
        public void ReqQuitGroup(uint groupID)
        {
            CmdSocialQuitGroupReq req = new CmdSocialQuitGroupReq();
            req.GroupId = groupID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.QuitGroupReq, req);
        }

        /// <summary>
        /// 将别人移出群组///
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="roleID"></param>
        public void ReqKickMember(uint groupID, ulong roleID)
        {
            CmdSocialGroupKickMemberReq req = new CmdSocialGroupKickMemberReq();
            req.GroupId = groupID;
            req.RoleId = roleID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.GroupKickMemberReq, req);
        }

        /// <summary>
        /// 群改名///
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="name"></param>
        public void ReqGroupChangeName(uint groupID, string name)
        {
            CmdSocialGroupChangeNameReq req = new CmdSocialGroupChangeNameReq();
            req.GroupId = groupID;
            req.Name = FrameworkTool.ConvertToGoogleByteString(name);

            NetClient.Instance.SendMessage((ushort)CmdSocial.GroupChangeNameReq, req);
        }

        /// <summary>
        /// 创建群组///
        /// </summary>
        /// <param name="name"></param>
        /// <param name="roleIDs"></param>
        public void ReqCreateGroup(string name, List<ulong> roleIDs)
        {
            if (string.IsNullOrEmpty(name))
            {
                Sys_Hint.Instance.PushContent_Normal("群组名字不为空");
                return;
            }

            CmdSocialCreateGroupReq req = new CmdSocialCreateGroupReq();
            req.Name = FrameworkTool.ConvertToGoogleByteString(name);
            for (int index = 0, len = roleIDs.Count; index < len; index++)
            {
                req.RoleIds.Add(roleIDs[index]);
            }

            NetClient.Instance.SendMessage((ushort)CmdSocial.CreateGroupReq, req);
        }

        /// <summary>
        /// 群组加人///
        /// </summary>
        /// <param name="roleIDs"></param>
        /// <param name="groupID"></param>
        public void ReqGroupAddMember(List<ulong> roleIDs, uint groupID)
        {
            CmdSocialGroupAddMemberReq req = new CmdSocialGroupAddMemberReq();
            req.GroupId = groupID;
            for (int index = 0, len = roleIDs.Count; index < len; index++)
            {
                req.RoleId.Add(roleIDs[index]);
            }

            NetClient.Instance.SendMessage((ushort)CmdSocial.GroupAddMemberReq, req);
        }

        /// <summary>
        /// 群组解散///
        /// </summary>
        /// <param name="groupID"></param>
        public void ReqDestroyGroup(uint groupID)
        {
            CmdSocialDestroyGroupReq req = new CmdSocialDestroyGroupReq();
            req.GroupId = groupID;

            NetClient.Instance.SendMessage((ushort)CmdSocial.DestroyGroupReq, req);
        }

        /// <summary>
        /// 发送群聊///
        /// </summary>
        /// <param name="groupID"></param>
        /// <param name="msg"></param>
        public void ReqGroupChat(uint groupID, string msg)
        {
            ChatExtMsg addItemRequest = null;
            string content = Instance.inputCache.GetSendContent(out addItemRequest);
            byte[] extraData = addItemRequest != null ? NetMsgUtil.Serialzie(addItemRequest) : null;

            CmdSocialGroupChatReq req = new CmdSocialGroupChatReq();
            req.GroupId = groupID;
            req.ChatMsg = FrameworkTool.ConvertToGoogleByteString(content);
            if (extraData != null)
            {
                req.ExtraMsg = Google.Protobuf.ByteString.CopyFrom(extraData);
            }
            NetClient.Instance.SendMessage((ushort)CmdSocial.GroupChatReq, req);
        }

        #endregion

        /// <summary>
        /// 与好友一起战斗需要增加双方的亲密度
        /// </summary>
        public void AddIntimacy()
        {
            List<ulong> friends = new List<ulong>();
            if (Sys_Team.Instance.HaveTeam)
            {
                foreach(var data in Sys_Team.Instance.teamMems)
                {
                    if (data.MemId != Sys_Role.Instance.RoleId
                        && Sys_Society.Instance.IsFriend(data.MemId))
                    {
                        friends.Add(data.MemId);
                    }
                }
            }

            if (friends.Count > 0)
            {
                CmdSocialBattleAddIntimacyReq req = new CmdSocialBattleAddIntimacyReq();
                for (int i = 0; i < friends.Count; ++i)
                    req.FriendId.Add(friends[i]);
                NetClient.Instance.SendMessage((ushort)CmdSocial.BattleAddIntimacyReq, req);
            }
        }

        /// <summary>
        /// 是否是好友///
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public bool IsFriend(ulong roleID)
        {
            if (socialFriendsInfo.friendsIdsDic.ContainsKey(roleID))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///是否在黑名单内///
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public bool IsInBlackList(ulong roleID)
        {
            if (socialBlacksInfo.blacksIdsDic.ContainsKey(roleID))
                return true;
            return false;
        }
    }
}
