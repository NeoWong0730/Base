using Framework;
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
        void OnExtendFriendNumAck(NetMsg netMsg)
        {
            CmdSocialExtendFriendNumAck ack = NetMsgUtil.Deserialize<CmdSocialExtendFriendNumAck>(CmdSocialExtendFriendNumAck.Parser, netMsg);
            if (ack != null)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13001, (ack.NewLimit - friendsMaxCount).ToString()));
                friendsMaxCount = ack.NewLimit;
            }
        }

        /// <summary>
        /// 亲密度更新///
        /// </summary>
        /// <param name="msg"></param>
        void OnIntimacyNtf(NetMsg msg)
        {
            CmdSocialIntimacyNtf ntf = NetMsgUtil.Deserialize<CmdSocialIntimacyNtf>(CmdSocialIntimacyNtf.Parser, msg);
            if (ntf != null)
            {
                if (socialRolesInfo.rolesDic.ContainsKey(ntf.FriendId))
                {
                    socialRolesInfo.rolesDic[ntf.FriendId].friendValue = ntf.IntimacyVal;
                    SerializeAllRolesInfoToJsonFile();
                    eventEmitter.Trigger<ulong, uint>(EEvents.OnIntimacyChange, ntf.FriendId, ntf.IntimacyVal);
                }
            }
        }

        /// <summary>
        /// 赠送礼物成功///
        /// </summary>
        /// <param name="msg"></param>
        void OnSendGiftsAck(NetMsg msg)
        {
            CmdSocialSendGiftsAck ack = NetMsgUtil.Deserialize<CmdSocialSendGiftsAck>(CmdSocialSendGiftsAck.Parser, msg);
            if (ack != null)
            {
                if (socialRolesInfo.rolesDic.ContainsKey(ack.DstRoleId))
                {
                    if (!roleChatDataInfo.roleChatDatas.ContainsKey(ack.DstRoleId))
                        DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ack.DstRoleId.ToString()));

                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(ack.ItemId);
                    string result = string.Empty;
                    if (cSVItemData.PresentType == 1)
                    {
                        result = LanguageHelper.GetTextContent(13008, ack.Count.ToString(), LanguageHelper.GetTextContent(cSVItemData.name_id), socialRolesInfo.rolesDic[ack.DstRoleId].roleName, (ack.Count * cSVItemData.PresentIntimacy).ToString());                        
                    }
                    else if (cSVItemData.PresentType == 2)
                    {
                        socialRolesInfo.rolesDic[ack.DstRoleId].commonSendNum += ack.Count;
                        result = LanguageHelper.GetTextContent(13012, ack.Count.ToString(), LanguageHelper.GetTextContent(cSVItemData.name_id), socialRolesInfo.rolesDic[ack.DstRoleId].roleName);
                    }
                    else if (cSVItemData.PresentType == 3)
                    {
                        socialRolesInfo.rolesDic[ack.DstRoleId].unCommonSendNum += ack.Count;
                        result = LanguageHelper.GetTextContent(13012, ack.Count.ToString(), LanguageHelper.GetTextContent(cSVItemData.name_id), socialRolesInfo.rolesDic[ack.DstRoleId].roleName);
                    }

                    ChatData chatData = roleChatDataInfo.AddClientChatData(ack.DstRoleId, socialSystemTipID, result, string.Empty);
                    roleChatRefDataInfo.AddRoleChatRefDataInfo(ack.DstRoleId);

                    SerializeChatRefsToJsonFile();
                    SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ack.DstRoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ack.DstRoleId.ToString()));

                    eventEmitter.Trigger(EEvents.OnSendSingleChat);

                    SerializeAllRolesInfoToJsonFile();
                }
                eventEmitter.Trigger<ulong, uint, uint>(EEvents.OnSendGiftSuccess, ack.DstRoleId, ack.ItemId, ack.Count);
            }
        }

        /// <summary>
        /// 被送礼物通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnReceiveGiftsNtf(NetMsg msg)
        {
            CmdSocialReceiveGiftsNtf ntf = NetMsgUtil.Deserialize<CmdSocialReceiveGiftsNtf>(CmdSocialReceiveGiftsNtf.Parser, msg);
            if (ntf != null)
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(ntf.ItemId);

                if (cSVItemData.PresentType == 2 || cSVItemData.PresentType == 3)
                {
                    if (socialRolesInfo.rolesDic.ContainsKey(ntf.FriendRoleId))
                    {
                        socialRolesInfo.rolesDic[ntf.FriendRoleId].hasGift = true;
                    }
                    SerializeAllRolesInfoToJsonFile();
                    eventEmitter.Trigger<ulong>(EEvents.OnReceiveGift, ntf.FriendRoleId);

                    object[] objs = new object[1];
                    objs[0] = ntf.FriendRoleId;
                    RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnGetGift, objs);
                }

                if (!roleChatDataInfo.roleChatDatas.ContainsKey(ntf.FriendRoleId))
                    DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.FriendRoleId.ToString()));

                string result = string.Empty;
                if (cSVItemData.PresentType == 1)
                {
                    result = LanguageHelper.GetTextContent(13009, ntf.Count.ToString(), LanguageHelper.GetTextContent(cSVItemData.name_id), socialRolesInfo.rolesDic[ntf.FriendRoleId].roleName, (ntf.Count * cSVItemData.PresentIntimacy).ToString());
                }
                else if (cSVItemData.PresentType == 2 || cSVItemData.PresentType == 3)
                {
                    result = LanguageHelper.GetTextContent(13013, ntf.Count.ToString(), LanguageHelper.GetTextContent(cSVItemData.name_id), socialRolesInfo.rolesDic[ntf.FriendRoleId].roleName);
                }

                ChatData chatData = roleChatDataInfo.AddClientChatData(ntf.FriendRoleId, socialSystemTipID, result, string.Empty);
                roleChatRefDataInfo.AddRoleChatRefDataInfo(ntf.FriendRoleId);

                SerializeChatRefsToJsonFile();
                SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ntf.FriendRoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.FriendRoleId.ToString()));

                eventEmitter.Trigger(EEvents.OnSendSingleChat);
            }
        }

        /// <summary>
        /// 接受礼物成功///
        /// </summary>
        /// <param name="msg"></param>
        void OnGetGiftsAck(NetMsg msg)
        {
            CmdSocialGetGiftsAck ack = NetMsgUtil.Deserialize<CmdSocialGetGiftsAck>(CmdSocialGetGiftsAck.Parser, msg);
            if (ack != null)
            {
                if (socialRolesInfo.rolesDic.ContainsKey(ack.RoleId))
                {
                    socialRolesInfo.rolesDic[ack.RoleId].hasGift = false;
                }
                SerializeAllRolesInfoToJsonFile();
                eventEmitter.Trigger<ulong>(EEvents.OnGetGiftSuccess, ack.RoleId);

                object[] objs = new object[1];
                objs[0] = ack.RoleId;
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnReadGift, objs);

                for (int index = 0, len = ack.Gifts.Count; index < len; index++)
                {
                    if (!roleChatDataInfo.roleChatDatas.ContainsKey(ack.RoleId))
                        DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ack.RoleId.ToString()));

                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(ack.Gifts[index].ItemId);
                    string result = string.Empty;
                    if (cSVItemData.PresentType == 1)
                    {
                        result = LanguageHelper.GetTextContent(13010, ack.Gifts[index].Count.ToString(), LanguageHelper.GetTextContent(cSVItemData.name_id), socialRolesInfo.rolesDic[ack.RoleId].roleName, (ack.Gifts[index].Count * cSVItemData.PresentIntimacy).ToString());
                    }
                    else if (cSVItemData.PresentType == 2 || cSVItemData.PresentType == 3)
                    {
                        result = LanguageHelper.GetTextContent(13011, ack.Gifts[index].Count.ToString(), LanguageHelper.GetTextContent(cSVItemData.name_id), socialRolesInfo.rolesDic[ack.RoleId].roleName);
                    }

                    ChatData chatData = roleChatDataInfo.AddClientChatData(ack.RoleId, socialSystemTipID, result, string.Empty);
                    roleChatRefDataInfo.AddRoleChatRefDataInfo(ack.RoleId);

                    SerializeChatRefsToJsonFile();
                    SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ack.RoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ack.RoleId.ToString()));

                    eventEmitter.Trigger(EEvents.OnSendSingleChat);
                }
            }
        }

        /// <summary>
        /// 设置社交信息回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnSetMySocialInfoAck(NetMsg msg)
        {
            CmdSocialSetMySocialInfoAck ack = NetMsgUtil.Deserialize<CmdSocialSetMySocialInfoAck>(CmdSocialSetMySocialInfoAck.Parser, msg);
            if (ack != null)
            {
                Sex = ack.Sex;
                Age = ack.Age;
                Area = ack.Area;
                Hobbys.Clear();
                for (int index = 0, len = ack.Hobby.Count; index < len; index++)
                {
                    Hobbys.Add(ack.Hobby[index]);
                }

                eventEmitter.Trigger(EEvents.OnSetRoleInfoSuccess);
            }
        }

        /// <summary>
        /// 类别搜索回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnSearchFriendAck(NetMsg msg)
        {
            CmdSocialSearchFriendAck ack = NetMsgUtil.Deserialize<CmdSocialSearchFriendAck>(CmdSocialSearchFriendAck.Parser, msg);
            if (ack != null)
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();
                if (ack.FriendsLists != null && ack.FriendsLists.Count > 0)
                {
                    for (int index = 0, len = ack.FriendsLists.Count; index < len; index++)
                    {
                        RoleInfo roleInfo = new RoleInfo();
                        roleInfo.roleName = ack.FriendsLists[index].Name.ToStringUtf8();
                        roleInfo.roleID = ack.FriendsLists[index].RoleId;
                        roleInfo.level = ack.FriendsLists[index].Lvl;
                        roleInfo.iconId = ack.FriendsLists[index].HeadIcon;
                        roleInfo.iconFrameId = ack.FriendsLists[index].IconFrameId;
                        roleInfo.heroID = ack.FriendsLists[index].Heroid;
                        roleInfo.occ = ack.FriendsLists[index].Occ;

                        roleInfos.Add(roleInfo);
                    }
                    if (ack.TotalCount > 30)
                    {
                        CacheLastSearchRoleID = ack.FriendsLists[ack.FriendsLists.Count - 1].RoleId;
                    }
                    else
                    {
                        CacheLastSearchRoleID = 0;
                    }
                }
                else
                {
                    CacheLastSearchRoleID = 0;
                }

                eventEmitter.Trigger<List<RoleInfo>>(EEvents.OnGetDetailSearchFriendSuccess, roleInfos);
            }
        }

        /// <summary>
        /// 关系搜索回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnRelationFriendAck(NetMsg msg)
        {
            CmdSocialRelationFriendAck ack = NetMsgUtil.Deserialize<CmdSocialRelationFriendAck>(CmdSocialRelationFriendAck.Parser, msg);
            if (ack != null)
            {
                List<RoleInfo> roleInfos = new List<RoleInfo>();
                for (int index = 0, len = ack.FriendsLists.Count; index < len; index++)
                {
                    RoleInfo roleInfo = new RoleInfo();
                    roleInfo.roleName = ack.FriendsLists[index].Name.ToStringUtf8();
                    roleInfo.roleID = ack.FriendsLists[index].RoleId;
                    roleInfo.level = ack.FriendsLists[index].Lvl;
                    roleInfo.iconId = ack.FriendsLists[index].HeadIcon;

                    roleInfos.Add(roleInfo);
                }

                eventEmitter.Trigger<List<RoleInfo>>(EEvents.OnGetRelationSearchFriendSuccess, roleInfos);
            }
        }

        /// <summary>
        /// 好友改名///
        /// </summary>
        /// <param name="msg"></param>
        void OnRenameNtf(NetMsg msg)
        {
            CmdSocialRenameNtf ntf = NetMsgUtil.Deserialize<CmdSocialRenameNtf>(CmdSocialRenameNtf.Parser, msg);
            if (ntf != null)
            {
                if (socialFriendsInfo.friendsIdsDic.ContainsKey(ntf.RoleId))
                {
                    if (!roleChatDataInfo.roleChatDatas.ContainsKey(ntf.RoleId))
                        DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.RoleId.ToString()));

                    string str = LanguageHelper.GetTextContent(13023, ntf.OldName.ToStringUtf8(), ntf.RoleName.ToStringUtf8());
                    ChatData chatData = roleChatDataInfo.AddClientChatData(ntf.RoleId, socialSystemTipID, str, string.Empty);
                    roleChatRefDataInfo.AddRoleChatRefDataInfo(ntf.RoleId);

                    SerializeChatRefsToJsonFile();
                    SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ntf.RoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.RoleId.ToString()));

                    eventEmitter.Trigger(EEvents.OnSendSingleChat);

                    socialRolesInfo.ReName(ntf.RoleId, ntf.RoleName.ToStringUtf8());

                    eventEmitter.Trigger<ulong>(EEvents.OnReNameSuccess, ntf.RoleId);
                }
            }
        }

        /// <summary>
        /// 打开好友面板回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnGetFriendInfoAck(NetMsg msg)
        {
            DeserializeAllRolesInfoFromJsonFile();
            CmdSocialGetFriendInfoAck ack = NetMsgUtil.Deserialize<CmdSocialGetFriendInfoAck>(CmdSocialGetFriendInfoAck.Parser, msg);
            if (ack != null)
            {
                //初始化玩家的社交信息///
                Sex = ack.Sex;
                Age = ack.Age;
                Area = ack.Area;
                Hobbys.Clear();
                for (int index = 0, len = ack.Hobby.Count; index < len; index++)
                {
                    Hobbys.Add(ack.Hobby[index]);
                }

                friendsMaxCount = ack.FriendLimit;

                ///更新服务器传回来的陌生人信息///
                if (ack.Strangers != null)
                {
                    for (int index = 0, len = ack.Strangers.Count; index < len; index++)
                    {
                        socialRolesInfo.UpdateRoleInfo(ack.Strangers[index].RoleId,
                            ack.Strangers[index].RoleName.ToStringUtf8(), ack.Strangers[index].Lvl,
                            ack.Strangers[index].Online, ack.Strangers[index].HeroId,
                            ack.Strangers[index].FriendPro, ack.Strangers[index].Occ, 
                            ack.Strangers[index].RoleHead, ack.Strangers[index].RoleHeadFrame,
                            ack.Strangers[index].GuildName.ToStringUtf8(),
                            ack.Strangers[index].TodayGiftCount, ack.Strangers[index].WeekGiftCount,
                            ack.Strangers[index].HasGift);
                    }
                }

                if (ack.Friends != null)
                {
                    ///赋值服务器传来的好友IDs///
                    socialFriendsInfo.InitFriendsInfoByServerMessage(ack.Friends);

                    ///更新服务器传回来的好友信息///
                    for (int index = 0, len = ack.Friends.Count; index < len; index++)
                    {
                        if (socialRolesInfo.rolesDic.ContainsKey(ack.Friends[index].RoleId))
                        {
                            if (socialRolesInfo.rolesDic[ack.Friends[index].RoleId].roleName != ack.Friends[index].RoleName.ToStringUtf8())
                            {
                                if (!roleChatDataInfo.roleChatDatas.ContainsKey(ack.Friends[index].RoleId))
                                    DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ack.Friends[index].RoleId.ToString()));

                                string str = LanguageHelper.GetTextContent(13023, socialRolesInfo.rolesDic[ack.Friends[index].RoleId].roleName, ack.Friends[index].RoleName.ToStringUtf8());
                                ChatData chatData = roleChatDataInfo.AddClientChatData(ack.Friends[index].RoleId, socialSystemTipID, str, string.Empty);
                                roleChatRefDataInfo.AddRoleChatRefDataInfo(ack.Friends[index].RoleId);

                                SerializeChatRefsToJsonFile();
                                SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ack.Friends[index].RoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ack.Friends[index].RoleId.ToString()));

                                eventEmitter.Trigger(EEvents.OnSendSingleChat);
                            }
                        }

                        socialRolesInfo.UpdateRoleInfo(ack.Friends[index].RoleId, ack.Friends[index].RoleName.ToStringUtf8(),
                            ack.Friends[index].Lvl, ack.Friends[index].Online, ack.Friends[index].HeroId,
                            ack.Friends[index].FriendPro, ack.Friends[index].Occ, 
                            ack.Friends[index].RoleHead, ack.Friends[index].RoleHeadFrame,
                            ack.Friends[index].GuildName.ToStringUtf8(),
                            ack.Friends[index].TodayGiftCount, ack.Friends[index].WeekGiftCount,
                            ack.Friends[index].HasGift);
                    }
                }

                if (ack.Teams != null)
                {
                    ///赋值服务器传来的好友分组Ids///
                    socialFriendGroupsInfo.InitFriendGroupsInfoByServerMessage(ack);
                }

                if (ack.Outdated != null)
                {
                    for (int index = 0, len = ack.Outdated.Count; index < len; index++)
                    {
                        socialRolesInfo.DelRoleInfo(ack.Outdated[index]);
                        socialFriendsInfo.DelFriendInfo(ack.Outdated[index]);
                        socialTeamMembersInfo.DelTeamMemberInfo(ack.Outdated[index]);
                    }
                }

                SerializeAllRolesInfoToJsonFile();
                SerializeFriendsInfoToJsonFile();
                SerializeTeamMembersInfoToJsonFile();
                SerializeFriendGroupsInfoToJsonFile();

                sendOpenReqFlag = false;
                sendOpenReqTimer = Timer.Register(sendOpenReqCd, () =>
                {
                    sendOpenReqFlag = true;
                }, null, false, true);

                CmdSocialGetBlackListReq req = new CmdSocialGetBlackListReq();
                NetClient.Instance.SendMessage((ushort)CmdSocial.GetBlackListReq, req);

                if (ack.Initiative)
                {
                    UIManager.OpenUI(EUIID.UI_Society);
                }
                eventEmitter.Trigger(EEvents.OnRoleInfoUpdate);
            }
        }

        void OnGetBlackListAck(NetMsg msg)
        {
            CmdSocialGetBlackListAck ack = NetMsgUtil.Deserialize<CmdSocialGetBlackListAck>(CmdSocialGetBlackListAck.Parser, msg);
            if (ack != null)
            {
                if (ack.Infos != null)
                {
                    for (int index = 0, len = ack.Infos.Count; index < len; index++)
                    {
                        socialBlacksInfo.AddBlackInfo(ack.Infos[index].RoleId);

                        socialRolesInfo.UpdateRoleInfo(ack.Infos[index].RoleId, ack.Infos[index].RoleName.ToStringUtf8(),
                            ack.Infos[index].Lvl, ack.Infos[index].Online, ack.Infos[index].HeroId,
                            ack.Infos[index].FriendPro, ack.Infos[index].Occ,
                            ack.Infos[index].RoleHead, ack.Infos[index].RoleHeadFrame,
                            ack.Infos[index].GuildName.ToStringUtf8(),
                            ack.Infos[index].TodayGiftCount, ack.Infos[index].WeekGiftCount,
                            ack.Infos[index].HasGift);
                    }

                    SerializeAllRolesInfoToJsonFile();
                    SerializeBlacksInfoToJsonFile();

                    eventEmitter.Trigger(EEvents.OnRoleInfoUpdate);
                }
            }
        }

        /// <summary>
        /// 加好友回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddFriendAck(NetMsg msg)
        {
            CmdSocialAddFriendAck ack = NetMsgUtil.Deserialize<CmdSocialAddFriendAck>(CmdSocialAddFriendAck.Parser, msg);
            if (ack != null)
            {
                Sys_Chat.Instance.PushMessage(ChatType.System, null, LanguageHelper.GetTextContent(13003, ack.Info.RoleName.ToStringUtf8()));
            }
        }

        /// <summary>
        /// 加好友申请处理回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnHandleFriendRequestAck(NetMsg msg)
        {
            CmdSocialHandleFriendRequestAck ack = NetMsgUtil.Deserialize<CmdSocialHandleFriendRequestAck>(CmdSocialHandleFriendRequestAck.Parser, msg);
            if (ack != null)
            {
                if (ack.Agree)
                {
                    socialFriendsInfo.AddFriendInfo(ack.Info.RoleId);
                    socialRecentlysInfo.AddRecentlyInfo(ack.Info.RoleId);
                    socialRolesInfo.UpdateRoleInfo(ack.Info.RoleId, ack.Info.RoleName.ToStringUtf8(), 
                        ack.Info.Lvl, ack.Info.Online, ack.Info.HeroId, ack.Info.FriendPro,
                        ack.Info.Occ, ack.Info.RoleHead, ack.Info.RoleHeadFrame, ack.Info.GuildName.ToStringUtf8(),
                        ack.Info.TodayGiftCount, ack.Info.WeekGiftCount, ack.Info.HasGift);

                    SerializeFriendsInfoToJsonFile();
                    SerializeRecentlysInfoToJsonFile();
                    SerializeAllRolesInfoToJsonFile();

                    eventEmitter.Trigger<ulong>(EEvents.OnAddFriendSuccess, ack.Info.RoleId);
                    string content = LanguageHelper.GetErrorCodeContent(680000005, ack.Info.RoleName.ToStringUtf8());
                    ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000005).pos, content, content);
                }
            }
        }

        /// <summary>
        /// 加好友申请处理通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnHandleFriendRequestNtf(NetMsg msg)
        {
            CmdSocialHandleFriendRequestNtf ntf = NetMsgUtil.Deserialize<CmdSocialHandleFriendRequestNtf>(CmdSocialHandleFriendRequestNtf.Parser, msg);
            if (ntf != null)
            {
                if (ntf.Agree)
                {
                    socialFriendsInfo.AddFriendInfo(ntf.Info.RoleId);
                    socialRecentlysInfo.AddRecentlyInfo(ntf.Info.RoleId);
                    socialRolesInfo.UpdateRoleInfo(ntf.Info.RoleId, ntf.Info.RoleName.ToStringUtf8(),
                        ntf.Info.Lvl, ntf.Info.Online, ntf.Info.HeroId, ntf.Info.FriendPro,
                        ntf.Info.Occ, ntf.Info.RoleHead, ntf.Info.RoleHeadFrame, ntf.Info.GuildName.ToStringUtf8(),
                        ntf.Info.TodayGiftCount, ntf.Info.WeekGiftCount, ntf.Info.HasGift);

                    SerializeFriendsInfoToJsonFile();
                    SerializeRecentlysInfoToJsonFile();
                    SerializeAllRolesInfoToJsonFile();

                    if (!roleChatDataInfo.roleChatDatas.ContainsKey(ntf.Info.RoleId))
                        DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.Info.RoleId.ToString()));

                    string content = LanguageHelper.GetErrorCodeContent(680000005, ntf.Info.RoleName.ToStringUtf8());
                    AddSeverSystemTip(ntf.Info.RoleId, content);
                    eventEmitter.Trigger<ulong>(EEvents.OnAddFriendSuccess, ntf.Info.RoleId);
                    //string content = LanguageHelper.GetErrorCodeContent(680000005, ntf.Info.RoleName.ToStringUtf8());
                    ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000005).pos, content, content);
                }
            }
        }

        /// <summary>
        /// 被加好友通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnAddFriendNtf(NetMsg msg)
        {
            CmdSocialAddFriendNtf ntf = NetMsgUtil.Deserialize<CmdSocialAddFriendNtf>(CmdSocialAddFriendNtf.Parser, msg);
            if (ntf != null)
            {
                //AddSocialSystemInfo($"{ntf.Name.ToStringUtf8()}把你添加为好友了,以后常联系~", ERichType.AddFriend, "加为好友", ntf.RoleId);
                //RedPointElement.eventEmitter.Trigger<ulong>(RedPointElement.EEvents.OnGetChat, socialSystemID);

                //string content = LanguageHelper.GetErrorCodeContent(680000005, ntf.Name.ToStringUtf8());
                //ChatData chatData = roleChatDataInfo.AddClientChatData(ntf.RoleId, socialSystemTipID, content, string.Empty);
                //roleChatDataInfo.roleChatDatas[ntf.RoleId].allRead = false;
                //roleChatRefDataInfo.AddRoleChatRefDataInfo(ntf.RoleId);

                //SerializeChatRefsToJsonFile();
                //SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ntf.RoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.RoleId.ToString()));
                //object[] objs = new object[1];
                //objs[0] = ntf.RoleId;
                //RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnGetChat, objs);

                Sys_MessageBag.MessageContent messageContent = new Sys_MessageBag.MessageContent();
                messageContent.mType = EMessageBagType.Friend;
                messageContent.invitorId = ntf.RoleId;
                messageContent.invitiorName = ntf.Name.ToStringUtf8();
                messageContent.friMess = new Sys_MessageBag.FriendMessage()
                {
                    roleLv = ntf.Lvl,
                    career = ntf.Occupation,
                };

                Sys_MessageBag.Instance.SendMessageInfo(EMessageBagType.Friend, messageContent);
            }
        }

        /// <summary>
        /// 删好友回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnDelFriendAck(NetMsg msg)
        {
            CmdSocialDelFriendAck ack = NetMsgUtil.Deserialize<CmdSocialDelFriendAck>(CmdSocialDelFriendAck.Parser, msg);
            if (ack != null)
            {
                string content = LanguageHelper.GetErrorCodeContent(680000006, socialRolesInfo.rolesDic[ack.RoleId].roleName);
                ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000006).pos, content, content);

                if (socialRolesInfo.rolesDic.ContainsKey(ack.RoleId))
                {
                    socialRolesInfo.rolesDic[ack.RoleId].friendValue = 0;
                }

                //好友ID删除//
                socialFriendsInfo.DelFriendInfo(ack.RoleId);

                //自定义分组好友删除//
                socialFriendGroupsInfo.DelFriendIDEachGroup(ack.RoleId);

                SerializeAllRolesInfoToJsonFile();
                SerializeFriendsInfoToJsonFile();
                SerializeFriendGroupsInfoToJsonFile();

                eventEmitter.Trigger(EEvents.OnDelFriendSuccess);
            }
        }

        /// <summary>
        /// 被删好友回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnDelFriendNtf(NetMsg msg)
        {
            CmdSocialDelFriendNtf ntf = NetMsgUtil.Deserialize<CmdSocialDelFriendNtf>(CmdSocialDelFriendNtf.Parser, msg);
            if (ntf != null)
            {
                string content = LanguageHelper.GetErrorCodeContent(680000006, socialRolesInfo.rolesDic[ntf.RoleId].roleName);
                ErrorCodeHelper.PushErrorCode(CSVErrorCode.Instance.GetConfData(680000006).pos, content, content);

                //好友ID删除//
                socialFriendsInfo.DelFriendInfo(ntf.RoleId);

                //自定义分组好友删除//
                socialFriendGroupsInfo.DelFriendIDEachGroup(ntf.RoleId);

                SerializeFriendsInfoToJsonFile();
                SerializeFriendGroupsInfoToJsonFile();

                eventEmitter.Trigger(EEvents.OnDelFriendSuccess);
            }
        }

        /// <summary>
        /// 好友上线通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnFriendOnlineNtf(NetMsg msg)
        {
            CmdSocialFriendOnlineNtf ntf = NetMsgUtil.Deserialize<CmdSocialFriendOnlineNtf>(CmdSocialFriendOnlineNtf.Parser, msg);
            if (ntf != null)
            {
                for (int index = 0, len = ntf.RoleId.Count; index < len; index++)
                {
                    socialRolesInfo.UpdateRoleInfoOnLineStatus(ntf.RoleId[index], true);
                    eventEmitter.Trigger<ulong>(EEvents.OnFriendOnLine, ntf.RoleId[index]);
                }

                SerializeAllRolesInfoToJsonFile();
            }
        }

        /// <summary>
        /// 好友下线通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnFriendOfflineNtf(NetMsg msg)
        {
            CmdSocialFriendOfflineNtf ntf = NetMsgUtil.Deserialize<CmdSocialFriendOfflineNtf>(CmdSocialFriendOfflineNtf.Parser, msg);
            if (ntf != null)
            {
                for (int index = 0, len = ntf.RoleId.Count; index < len; index++)
                {
                    socialRolesInfo.UpdateRoleInfoOnLineStatus(ntf.RoleId[index], false);
                    eventEmitter.Trigger<ulong>(EEvents.OnFriendOffLine, ntf.RoleId[index]);
                }

                SerializeAllRolesInfoToJsonFile();
            }
        }

        /// <summary>
        /// 私聊通知///
        /// </summary>
        /// <param name="msg"></param>
        void OnChatSingleNtf(NetMsg msg)
        {
            CmdSocialChatSingleNtf ntf = NetMsgUtil.Deserialize<CmdSocialChatSingleNtf>(CmdSocialChatSingleNtf.Parser, msg);
            if (ntf != null)
            {
                ///别人发的消息///
                if (ntf.DstRoleId == Sys_Role.Instance.RoleId)
                {
                    if (IsInBlackList(ntf.Info.RoleId))
                        return;
                    
                    ///收到信息后最近没有的话先加///
                    if (!socialRecentlysInfo.recentlyIdsDic.ContainsKey(ntf.Info.RoleId))
                    {
                        socialRecentlysInfo.AddRecentlyInfo(ntf.Info.RoleId);
                    }

                    //再更新这个人的玩家信息//
                    socialRolesInfo.UpdateRoleInfo(ntf.Info.RoleId, ntf.Info.RoleName.ToStringUtf8(), 
                        ntf.Info.Lvl, ntf.Info.Online, ntf.Info.HeroId, ntf.Info.FriendPro, 
                        ntf.Info.Occ, ntf.Info.RoleHead, ntf.Info.RoleHeadFrame,
                        ntf.Info.GuildName.ToStringUtf8(),
                        ntf.Info.TodayGiftCount, ntf.Info.WeekGiftCount, ntf.Info.HasGift);

                    SerializeRecentlysInfoToJsonFile();
                    SerializeAllRolesInfoToJsonFile();

                    if (!roleChatDataInfo.roleChatDatas.ContainsKey(ntf.Info.RoleId))
                       DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.Info.RoleId.ToString()));

                    //再添加聊天相关信息//
                    ChatExtMsg addItemRequest = null;
                    NetMsgUtil.TryDeserialize<ChatExtMsg>(ChatExtMsg.Parser, ntf.ExtraMsg, out addItemRequest);

                    string result = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), addItemRequest, ntf.ChatMsg.ToStringUtf8());
                    ChatData chatData = roleChatDataInfo.AddServerChatData(ntf.Info.RoleId, result, ntf.ExtraMsg.ToBase64(), ntf.SenderTime, ntf.ChatFrame, ntf.ChatText);
                    roleChatRefDataInfo.AddRoleChatRefDataInfo(ntf.Info.RoleId);

                    SerializeChatRefsToJsonFile();
                    SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ntf.Info.RoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.Info.RoleId.ToString()));

                    eventEmitter.Trigger<ChatData>(EEvents.OnGetSingleChat, chatData);
                    object[] objs = new object[1];
                    objs[0] = ntf.Info.RoleId;
                    RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnGetChat, objs);
                }
                else
                {
                    if (!roleChatDataInfo.roleChatDatas.ContainsKey(ntf.DstRoleId))
                        DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.DstRoleId.ToString()));

                    ChatExtMsg addItemRequest = null;
                    NetMsgUtil.TryDeserialize<ChatExtMsg>(ChatExtMsg.Parser, ntf.ExtraMsg, out addItemRequest);

                    string result = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), addItemRequest, ntf.ChatMsg.ToStringUtf8());
                    ChatData chatData = roleChatDataInfo.AddClientChatData(ntf.DstRoleId, Sys_Role.Instance.RoleId, result, ntf.ExtraMsg.ToBase64());
                    roleChatRefDataInfo.AddRoleChatRefDataInfo(ntf.DstRoleId);

                    SerializeChatRefsToJsonFile();
                    SerializeChatsInfoToJsonFile(roleChatDataInfo.roleChatDatas[ntf.DstRoleId], string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.DstRoleId.ToString()));

                    eventEmitter.Trigger(EEvents.OnSendSingleChat);
                }
            }
        }

        /// <summary>
        /// 好友分组创建,添加,增量回调///
        /// </summary>
        /// <param name=""></param>
        void OnFriendsGroupAck(NetMsg msg)
        {
            CmdSocialFriendsGroupAck ack = NetMsgUtil.Deserialize<CmdSocialFriendsGroupAck>(CmdSocialChatSingleReq.Parser, msg);
            if (ack != null)
            {
                string name = ack.Name.ToStringUtf8();
                string oldName = ack.OldName.ToStringUtf8();

                ///添加新的FriendGroup///
                List<ulong> roleIDs = new List<ulong>();
                for (int index = 0, len = ack.RoleId.Count; index < len; index++)
                {
                    roleIDs.Add(ack.RoleId[index]);
                }
                socialFriendGroupsInfo.AddFriendGroupInfo(name, roleIDs);

                ///删掉老的FriendGroup///
                if (name != oldName && oldName != string.Empty)
                {
                    socialFriendGroupsInfo.DelFriendGroupInfo(oldName);
                }

                SerializeFriendGroupsInfoToJsonFile();

                eventEmitter.Trigger<string>(EEvents.OnCreateOrSettingFriendGroupSuccess, name);
            }
        }

        /// <summary>
        /// 删除自定义好友分组回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnDelFriendsGroupAck(NetMsg msg)
        {
            CmdSocialDelFriendsGroupAck ack = NetMsgUtil.Deserialize<CmdSocialDelFriendsGroupAck>(CmdSocialDelFriendsGroupAck.Parser, msg);
            if (ack != null)
            {
                string name = ack.Name.ToStringUtf8();
                socialFriendGroupsInfo.DelFriendGroupInfo(name);
                SerializeFriendGroupsInfoToJsonFile();
                eventEmitter.Trigger<string>(EEvents.OnDelFriendGroupSuccess, name);
            }
        }

        /// <summary>
        /// 请求玩家信息回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnGetBriefInfoAck(NetMsg msg)
        {
            CmdSocialGetBriefInfoAck ack = NetMsgUtil.Deserialize<CmdSocialGetBriefInfoAck>(CmdSocialGetBriefInfoAck.Parser, msg);
            if (ack != null)
            {
                if (ack.Classify == (uint)EGetBriefInfoClassify.SocialSearchRole)
                {
                    RoleInfo roleInfo = new RoleInfo();
                    roleInfo.roleName = ack.Name.ToStringUtf8();
                    roleInfo.roleID = ack.RoleId;
                    roleInfo.heroID = ack.HeroId;
                    roleInfo.level = ack.Level;
                    roleInfo.occ = ack.Occ;
                    roleInfo.isOnLine = ack.BOnline;
                    roleInfo.rank = ack.CareerRank;
                    roleInfo.iconId = ack.RoleHead;
                    roleInfo.iconFrameId = ack.RoleHeadFrame;
                    roleInfo.guildName = ack.GuildName.ToStringUtf8();
                    eventEmitter.Trigger<RoleInfo>(EEvents.OnGetBriefInfoSuccess, roleInfo);
                }
                else if (ack.Classify == (uint)EGetBriefInfoClassify.Sys_Role_Info)
                {
                    Sys_Role_Info.Instance.PushRoleInfo(ack);
                }
            }
        }

        void OnAddBlackList(NetMsg msg)
        {
            CmdSocialAddBlackListAck ack = NetMsgUtil.Deserialize<CmdSocialAddBlackListAck>(CmdSocialAddBlackListAck.Parser, msg);
            if (ack != null)
            {
                socialBlacksInfo.AddBlackInfo(ack.Info.RoleId);

                socialRolesInfo.UpdateRoleInfo(ack.Info.RoleId, ack.Info.RoleName.ToStringUtf8(),
                        ack.Info.Lvl, ack.Info.Online, ack.Info.HeroId, ack.Info.FriendPro,
                        ack.Info.Occ, ack.Info.RoleHead, ack.Info.RoleHeadFrame,
                        ack.Info.GuildName.ToStringUtf8(),
                        ack.Info.TodayGiftCount, ack.Info.WeekGiftCount, ack.Info.HasGift);

                SerializeAllRolesInfoToJsonFile();
                SerializeBlacksInfoToJsonFile();

                eventEmitter.Trigger(EEvents.OnAddBlack);

                string content = LanguageHelper.GetTextContent(12280, socialRolesInfo.rolesDic[ack.Info.RoleId].roleName);
                Sys_Hint.Instance.PushContent_Normal(content);
                Sys_Chat.Instance.PushMessage(ChatType.System, null, content, Sys_Chat.EMessageProcess.None);
            }
        }

        void OnRemoveBlackList(NetMsg msg)
        {
            CmdSocialRemoveBlackListAck ack = NetMsgUtil.Deserialize<CmdSocialRemoveBlackListAck>(CmdSocialRemoveBlackListAck.Parser, msg);

            if (ack != null)
            {
                socialBlacksInfo.DelBlackInfo(ack.RoleId);

               // SerializeAllRolesInfoToJsonFile();
                SerializeBlacksInfoToJsonFile();

                eventEmitter.Trigger(EEvents.OnRemoveBlack);

                string content = LanguageHelper.GetTextContent(12281, socialRolesInfo.rolesDic[ack.RoleId].roleName);
                Sys_Hint.Instance.PushContent_Normal(content);
                Sys_Chat.Instance.PushMessage(ChatType.System, null, content, Sys_Chat.EMessageProcess.None);
            }
        }

        private void OnSingleFromSystemNtf(NetMsg msg)
        {
            CmdSocialChatSingleFromSystemNty ntf = NetMsgUtil.Deserialize<CmdSocialChatSingleFromSystemNty>(CmdSocialChatSingleFromSystemNty.Parser, msg);
            if (ntf != null)
            {
                for (int i = 0; i < ntf.Units.Count; ++i)
                {
                    ChatSingleFromSystem info = ntf.Units[i];
                    string strContent = ErrorCodeHelper._ParserSysTip(info.Fields, CSVErrorCode.Instance.GetConfData(info.MsgId), false);
                    AddSeverSystemTip(info.SenderId, strContent);
                }
            }
        }

        #region CallBack

        /// <summary>
        /// 每次打开群组界面请求回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnGetGroupBriefInfoAck(NetMsg msg)
        {
            CmdSocialGetGroupBriefInfoAck ack = NetMsgUtil.Deserialize<CmdSocialGetGroupBriefInfoAck>(CmdSocialGetGroupBriefInfoAck.Parser, msg);
            if (ack != null)
            {
                socialGroupsInfo.InitGroupsInfoByServerMessage(ack);
                SerializeGroupsInfoToJsonFile();
                eventEmitter.Trigger(EEvents.OnGetGroupInfo);
            }
        }

        /// <summary>
        /// 某个群详细信息请求回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnGetGroupDetailInfoAck(NetMsg msg)
        {
            CmdSocialGetGroupDetailInfoAck ack = NetMsgUtil.Deserialize<CmdSocialGetGroupDetailInfoAck>(CmdSocialGetGroupDetailInfoAck.Parser, msg);
            if (ack != null)
            {
                ///更新群组的角色ID///
                GroupInfo groupInfo;
                if (socialGroupsInfo.groupsDic.TryGetValue(ack.GroupId, out groupInfo))
                {
                    groupInfo.UpdateRoleIDsFromServer(ack.Roles);
                    SerializeGroupsInfoToJsonFile();
                }

                ///更新角色信息///
                foreach (var role in ack.Roles)
                {
                    if (socialRolesInfo.rolesDic.ContainsKey(role.RoleId))
                    {
                        socialRolesInfo.UpdateRoleInfo(role.RoleId, role.RoleName.ToStringUtf8(), 
                            role.Lvl, role.Online, role.HeroId, role.FriendPro, role.Occ, 
                            role.RoleHead, role.RoleHeadFrame, role.GuildName.ToStringUtf8(),
                            role.TodayGiftCount, role.WeekGiftCount, role.HasGift);
                    }
                    else
                    {
                        socialRolesInfo.UpdateRoleInfo(role.RoleId, role.RoleName.ToStringUtf8(), 
                            role.Lvl, role.Online, role.HeroId, role.FriendPro, role.Occ, 
                            role.RoleHead, role.RoleHeadFrame, role.GuildName.ToStringUtf8(),
                            role.TodayGiftCount, role.WeekGiftCount, role.HasGift);
                    }

                    SerializeAllRolesInfoToJsonFile();
                }
                eventEmitter.Trigger<uint>(EEvents.OnGetOneGroupDetailInfo, ack.GroupId);
            }
        }

        /// <summary>
        /// 创建群组回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnGroupInfoNtf(NetMsg msg)
        {
            CmdSocialGroupInfoNtf ntf = NetMsgUtil.Deserialize<CmdSocialGroupInfoNtf>(CmdSocialGroupInfoNtf.Parser, msg);
            if (ntf != null)
            {
                List<uint> heroIDs = new List<uint>();
                foreach (uint heroID in ntf.HeroIds)
                {
                    heroIDs.Add(heroID);
                }
                socialGroupsInfo.AddGroup(ntf.GroupId, ntf.Count, ntf.Name.ToStringUtf8(), string.Empty, ntf.LeaderId, heroIDs);

                SerializeGroupsInfoToJsonFile();
                eventEmitter.Trigger(EEvents.OnCreateGroupSuccess);
            }
        }

        /// <summary>
        /// 群公告回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnCmdSocialSetGroupNoticeNtf(NetMsg msg)
        {
            CmdSocialSetGroupNoticeNtf ntf = NetMsgUtil.Deserialize<CmdSocialSetGroupNoticeNtf>(CmdSocialSetGroupNoticeNtf.Parser, msg);
            if (ntf != null)
            {
                GroupInfo groupInfo;
                if (socialGroupsInfo.groupsDic.TryGetValue(ntf.GroupId, out groupInfo))
                {
                    groupInfo.UpdateNotice(ntf.Notice.ToStringUtf8());
                    SerializeGroupsInfoToJsonFile();
                    eventEmitter.Trigger<GroupInfo>(EEvents.OnChangeGroupNotice, groupInfo);
                }
            }
        }

        /// <summary>
        /// 群改名回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnGroupChangeNameAck(NetMsg msg)
        {
            CmdSocialGroupChangeNameAck ack = NetMsgUtil.Deserialize<CmdSocialGroupChangeNameAck>(CmdSocialGroupChangeNameAck.Parser, msg);
            if (ack != null)
            {
                GroupInfo groupInfo;
                if (socialGroupsInfo.groupsDic.TryGetValue(ack.GroupId, out groupInfo))
                {
                    groupInfo.UpdateName(ack.Name.ToStringUtf8());
                    SerializeGroupsInfoToJsonFile();
                    eventEmitter.Trigger<GroupInfo>(EEvents.OnChangeGroupName, groupInfo);
                }
            }
        }

        /// <summary>
        /// 群组加人回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnGroupAddMemberAck(NetMsg msg)
        {
            CmdSocialGroupAddMemberAck ack = NetMsgUtil.Deserialize<CmdSocialGroupAddMemberAck>(CmdSocialGroupAddMemberAck.Parser, msg);
            if (ack != null)
            {
                GroupInfo groupInfo;
                if (socialGroupsInfo.groupsDic.TryGetValue(ack.GroupId, out groupInfo))
                {
                    groupInfo.AddRoleIDsToGroup(ack.RoleId);
                }
                else
                {
                    DebugUtil.LogError($"Send a no existed Group, id:{ack.GroupId}");
                }

                SerializeGroupsInfoToJsonFile();
                eventEmitter.Trigger<Sys_Society.GroupInfo>(EEvents.OnOtherAddGroupSuccess, groupInfo);
            }
        }

        /// <summary>
        /// 退出群回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnQuitGroupAck(NetMsg msg)
        {
            CmdSocialQuitGroupAck ack = NetMsgUtil.Deserialize<CmdSocialQuitGroupAck>(CmdSocialQuitGroupAck.Parser, msg);
            if (ack != null)
            {
                socialGroupsInfo.DelGroup(ack.GroupId);
            }

            SerializeGroupsInfoToJsonFile();
            eventEmitter.Trigger<uint>(EEvents.OnSelfQuitGroupSuccess, ack.GroupId);
        }

        void OnGroupKickMemberNtf(NetMsg msg)
        {
            CmdSocialGroupKickMemberNtf ntf = NetMsgUtil.Deserialize<CmdSocialGroupKickMemberNtf>(CmdSocialGroupKickMemberNtf.Parser, msg);
            if (ntf != null)
            {
                ///是自己///
                if (ntf.RoleId == Sys_Role.Instance.RoleId)
                {
                    socialGroupsInfo.DelGroup(ntf.GroupId);

                    SerializeGroupsInfoToJsonFile();
                    eventEmitter.Trigger<uint>(EEvents.OnSelfKickFromGroup, ntf.GroupId);
                }
                else
                {
                    GroupInfo groupInfo;
                    if (socialGroupsInfo.groupsDic.TryGetValue(ntf.GroupId, out groupInfo))
                    {
                        groupInfo.DelRoleIDFromGroup(ntf.RoleId);
                    }

                    SerializeGroupsInfoToJsonFile();
                    eventEmitter.Trigger<GroupInfo>(EEvents.OnOtherKickFromGroup, groupInfo);
                }
            }
        }

        /// <summary>
        /// 解散群回调///
        /// </summary>
        /// <param name="msg"></param>
        void OnDestroyGroupNtf(NetMsg msg)
        {
            CmdSocialDestroyGroupNtf ntf = NetMsgUtil.Deserialize<CmdSocialDestroyGroupNtf>(CmdSocialDestroyGroupNtf.Parser, msg);
            if (ntf != null)
            {
                socialGroupsInfo.DelGroup(ntf.GroupId);
                eventEmitter.Trigger<uint>(EEvents.OnDismissGroup, ntf.GroupId);
                groupChatDataInfo.ClearGroupChatData(ntf.GroupId);
                SerializeGroupsInfoToJsonFile();
            }
        }

        void OnGroupChatNtf(NetMsg msg)
        {
            CmdSocialGroupChatNtf ntf = NetMsgUtil.Deserialize<CmdSocialGroupChatNtf>(CmdSocialGroupChatNtf.Parser, msg);
            if (ntf != null)
            {
                /// 如果发送者是自己///
                if (ntf.RoleId == Sys_Role.Instance.RoleId)
                {
                    ChatExtMsg addItemRequest = null;
                    NetMsgUtil.TryDeserialize<ChatExtMsg>(ChatExtMsg.Parser, ntf.ExtraMsg, out addItemRequest);

                    string result = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), addItemRequest, ntf.ChatMsg.ToStringUtf8());

                    groupChatDataInfo.AddClientChatData(ntf.GroupId, Sys_Role.Instance.RoleId, result, ntf.ExtraMsg.ToBase64());
                    groupChatRefDataInfo.AddGroupChatRefDataInfo(ntf.GroupId);
                    SerializeGroupChatRefsToJsonFile();
                    SerializeGroupChatsInfoToJsonFile(groupChatDataInfo.groupChatDatas[ntf.GroupId], string.Format(groupChatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.GroupId.ToString()));

                    eventEmitter.Trigger(EEvents.OnSendGroupChat);
                }
                ///别人发的消息///
                else
                {
                    ChatExtMsg addItemRequest = null;
                    NetMsgUtil.TryDeserialize<ChatExtMsg>(ChatExtMsg.Parser, ntf.ExtraMsg, out addItemRequest);

                    string result = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), addItemRequest, ntf.ChatMsg.ToStringUtf8());

                    ChatData groupChatData = groupChatDataInfo.AddServerChatData(ntf.GroupId, ntf.RoleId, result, ntf.ExtraMsg.ToBase64(), ntf.SenderChatFrame, ntf.SenderChatText);
                    groupChatRefDataInfo.AddGroupChatRefDataInfo(ntf.GroupId);
                    SerializeGroupChatRefsToJsonFile();
                    SerializeChatsInfoToJsonFile(groupChatDataInfo.groupChatDatas[ntf.GroupId], string.Format(groupChatsPath, Sys_Role.Instance.RoleId.ToString(), ntf.GroupId.ToString()));
                    eventEmitter.Trigger<uint, ChatData>(EEvents.OnGetGroupChat, ntf.GroupId, groupChatData);
                    RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnGetGroupChat, null);
                }
            }
        }

        void OnMemEnterNtf(TeamMem teamMem)
        {
            if (teamMem.IsRob())
                return;

            if (socialRolesInfo.rolesDic.ContainsKey(teamMem.MemId))
            {
                socialRolesInfo.UpdateRoleInfo(teamMem.MemId, teamMem.Name.ToStringUtf8(),
                    teamMem.Level, true, teamMem.HeroId, socialRolesInfo.rolesDic[teamMem.MemId].friendValue, teamMem.Career, teamMem.Photo,
                    teamMem.PhotoFrame, teamMem.GuildName.ToStringUtf8(), socialRolesInfo.rolesDic[teamMem.MemId].commonSendNum, 
                    socialRolesInfo.rolesDic[teamMem.MemId].unCommonSendNum, socialRolesInfo.rolesDic[teamMem.MemId].hasGift);
                socialTeamMembersInfo.AddTeamMemberInfo(teamMem.MemId);
            }
            else
            {
                socialRolesInfo.UpdateRoleInfo(teamMem.MemId, teamMem.Name.ToStringUtf8(),
                    teamMem.Level, true, teamMem.HeroId, 0, teamMem.Career, teamMem.Photo,
                    teamMem.PhotoFrame, teamMem.GuildName.ToStringUtf8(), 0, 0, false);
                socialTeamMembersInfo.AddTeamMemberInfo(teamMem.MemId);
            }

            SerializeAllRolesInfoToJsonFile();
            SerializeTeamMembersInfoToJsonFile();

            eventEmitter.Trigger(EEvents.OnAddTeamMember);
        }

        void OnBeMember()
        {
            var mens = Sys_Team.Instance.getTeamMems();
            for (int index = 0, len = mens.Count; index < len; index++)
            {
                if (mens[index].IsRob())
                    continue;

                if (mens[index].MemId != Sys_Role.Instance.RoleId)
                {
                    if (socialRolesInfo.rolesDic.ContainsKey(mens[index].MemId))
                    {
                        socialRolesInfo.UpdateRoleInfo(mens[index].MemId, mens[index].Name.ToStringUtf8(),
                            mens[index].Level, true, mens[index].HeroId, socialRolesInfo.rolesDic[mens[index].MemId].friendValue, mens[index].Career, mens[index].Photo,
                            mens[index].PhotoFrame, mens[index].GuildName.ToStringUtf8(), socialRolesInfo.rolesDic[mens[index].MemId].commonSendNum, 
                            socialRolesInfo.rolesDic[mens[index].MemId].unCommonSendNum, socialRolesInfo.rolesDic[mens[index].MemId].hasGift);
                        socialTeamMembersInfo.AddTeamMemberInfo(mens[index].MemId);
                    }
                    else
                    {
                        socialRolesInfo.UpdateRoleInfo(mens[index].MemId, mens[index].Name.ToStringUtf8(),
                            mens[index].Level, true, mens[index].HeroId, 0, mens[index].Career, mens[index].Photo,
                            mens[index].PhotoFrame, mens[index].GuildName.ToStringUtf8(), 0, 0, false);
                        socialTeamMembersInfo.AddTeamMemberInfo(mens[index].MemId);
                    }
                    socialTeamMembersInfo.AddTeamMemberInfo(mens[index].MemId);
                }
            }

            SerializeAllRolesInfoToJsonFile();
            SerializeTeamMembersInfoToJsonFile();

            eventEmitter.Trigger(EEvents.OnAddTeamMember);
        }

        /// <summary>
        /// 添加系统提示消息///
        /// </summary>
        /// <param name="content"></param>
        /// <param name="targetID"></param>
        public void AddSocialSystemTipInfo(string content, ulong targetID)
        {
            ChatData chatData = new ChatData();
            chatData.content = content;
            chatData.roleID = socialSystemTipID;

            if (roleChatDataInfo.roleChatDatas.ContainsKey(targetID))
            {

            }
            else
            {

            }
        }      

        public void OpenPrivateChat(RoleInfo roleInfo)
        {
            if (!socialRolesInfo.rolesDic.ContainsKey(roleInfo.roleID))
            {
                socialRolesInfo.UpdateRoleInfo(roleInfo.roleID, roleInfo.roleName, 
                    roleInfo.level, roleInfo.isOnLine, roleInfo.heroID, roleInfo.friendValue,
                    roleInfo.occ, roleInfo.iconId, roleInfo.iconFrameId, roleInfo.guildName,
                    roleInfo.commonSendNum, roleInfo.unCommonSendNum, roleInfo.hasGift);
                SerializeAllRolesInfoToJsonFile();
            }

            if (!socialRecentlysInfo.recentlyIdsDic.ContainsKey(roleInfo.roleID))
            {
                socialRecentlysInfo.AddRecentlyInfo(roleInfo.roleID);
                SerializeRecentlysInfoToJsonFile();
            }

            UIManager.OpenUI(EUIID.UI_Society, false, roleInfo.roleID);
        }

        #endregion
    }
}
