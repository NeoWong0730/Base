using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GetBlackListReq, (ushort)CmdSocial.GetBlackListAck, OnGetBlackListAck, CmdSocialGetBlackListAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.RenameNtf, OnRenameNtf, CmdSocialRenameNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.ExtendFriendNumReq, (ushort)CmdSocial.ExtendFriendNumAck, OnExtendFriendNumAck, CmdSocialExtendFriendNumAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.IntimacyNtf, OnIntimacyNtf, CmdSocialIntimacyNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.SendGiftsReq, (ushort)CmdSocial.SendGiftsAck, OnSendGiftsAck, CmdSocialSendGiftsAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.ReceiveGiftsNtf, OnReceiveGiftsNtf, CmdSocialReceiveGiftsNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GetGiftsReq, (ushort)CmdSocial.GetGiftsAck, OnGetGiftsAck, CmdSocialGetGiftsAck.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.SearchFriendReq, (ushort)CmdSocial.SearchFriendAck, OnSearchFriendAck, CmdSocialSearchFriendAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.SetMySocialInfoReq, (ushort)CmdSocial.SetMySocialInfoAck, OnSetMySocialInfoAck, CmdSocialSetMySocialInfoAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GetFriendInfoReq, (ushort)CmdSocial.GetFriendInfoAck, OnGetFriendInfoAck, CmdSocialGetFriendInfoAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.AddFriendReq, (ushort)CmdSocial.AddFriendAck, OnAddFriendAck, CmdSocialAddFriendAck.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.HandleFriendRequestReq, (ushort)CmdSocial.HandleFriendRequestAck, OnHandleFriendRequestAck, CmdSocialHandleFriendRequestAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.HandleFriendRequestNtf, OnHandleFriendRequestNtf, CmdSocialHandleFriendRequestNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.AddFriendNtf, OnAddFriendNtf, CmdSocialAddFriendNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.DelFriendReq, (ushort)CmdSocial.DelFriendAck, OnDelFriendAck, CmdSocialDelFriendAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.DelFriendNtf, OnDelFriendNtf, CmdSocialDelFriendNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.FriendOnlineNtf, OnFriendOnlineNtf, CmdSocialFriendOnlineNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.FriendOfflineNtf, OnFriendOfflineNtf, CmdSocialFriendOfflineNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.ChatSingleNtf, OnChatSingleNtf, CmdSocialChatSingleNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.FriendsGroupReq, (ushort)CmdSocial.FriendsGroupAck, OnFriendsGroupAck, CmdSocialFriendsGroupAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.DelFriendsGroupReq, (ushort)CmdSocial.DelFriendsGroupAck, OnDelFriendsGroupAck, CmdSocialDelFriendsGroupAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GetBriefInfoReq, (ushort)CmdSocial.GetBriefInfoAck, OnGetBriefInfoAck, CmdSocialGetBriefInfoAck.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.AddBlackListReq, (ushort)CmdSocial.AddBlackListAck, OnAddBlackList, CmdSocialAddBlackListAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.RemoveBlackListReq, (ushort)CmdSocial.RemoveBlackListAck, OnRemoveBlackList, CmdSocialRemoveBlackListAck.Parser);

            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GetGroupBriefInfoReq, (ushort)CmdSocial.GetGroupBriefInfoAck, OnGetGroupBriefInfoAck, CmdSocialGetGroupBriefInfoAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GetGroupDetailInfoReq, (ushort)CmdSocial.GetGroupDetailInfoAck, OnGetGroupDetailInfoAck, CmdSocialGetGroupDetailInfoAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.GroupInfoNtf, OnGroupInfoNtf, CmdSocialGroupInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.SetGroupNoticeReq, (ushort)CmdSocial.SetGroupNoticeNtf, OnCmdSocialSetGroupNoticeNtf, CmdSocialSetGroupNoticeNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.QuitGroupReq, (ushort)CmdSocial.QuitGroupAck, OnQuitGroupAck, CmdSocialQuitGroupAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.GroupKickMemberNtf, OnGroupKickMemberNtf, CmdSocialGroupKickMemberNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GroupChangeNameReq, (ushort)CmdSocial.GroupChangeNameAck, OnGroupChangeNameAck, CmdSocialGroupChangeNameAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.GroupAddMemberReq, (ushort)CmdSocial.GroupAddMemberAck, OnGroupAddMemberAck, CmdSocialGroupAddMemberAck.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.DestroyGroupNtf, OnDestroyGroupNtf, CmdSocialDestroyGroupNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.GroupChatNtf, OnGroupChatNtf, CmdSocialGroupChatNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.ChatSingleFromSystemNty, OnSingleFromSystemNtf, CmdSocialChatSingleFromSystemNty.Parser);

            Sys_Team.Instance.eventEmitter.Handle<TeamMem>(Sys_Team.EEvents.MemEnterNtf, OnMemEnterNtf, true);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, OnBeMember, true);
            eventEmitter.Handle<ulong>(EEvents.OnReadRoleChat, OnReadRoleChat, true);

            inputCache.onChange = OnInputChange;

            uint.TryParse(CSVParam.Instance.GetConfData(1058).str_value, out recentlyMaxCount);
            uint.TryParse(CSVParam.Instance.GetConfData(972).str_value, out teammemberMaxCount);
            uint.TryParse(CSVParam.Instance.GetConfData(969).str_value, out blacksMaxCount);
            uint.TryParse(CSVParam.Instance.GetConfData(967).str_value, out GroupMaxCount);
        }

        public override void Dispose()
        {
            Sys_Team.Instance.eventEmitter.Handle<TeamMem>(Sys_Team.EEvents.MemEnterNtf, OnMemEnterNtf, false);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, OnBeMember, false);
            eventEmitter.Handle<ulong>(EEvents.OnReadRoleChat, OnReadRoleChat, false);

            sendOpenReqTimer?.Cancel();
            sendOpenReqTimer = null;

            base.Dispose();
        }

        public override void OnSyncFinished()
        {
            mInputCacheRecord = new InputCacheRecord(10, Sys_Role.Instance.RoleId.ToString());
            mInputCacheRecord.ReadInputCache();

            DeserializeAllRolesInfoFromJsonFile();
            socialRolesInfo.UpdateRoleInfo(Sys_Role.Instance.Role.RoleId, Sys_Role.Instance.Role.Name.ToStringUtf8(),
                Sys_Role.Instance.Role.Level, true, Sys_Role.Instance.Role.HeroId, 0,
                Sys_Role.Instance.Role.Career, Sys_Head.Instance.clientHead.headId,
                Sys_Head.Instance.clientHead.headFrameId, string.Empty, 0, 0, false);
            DeserializeRecentlysInfoFromJsonFile();
            DeserializeFriendsInfoFromJsonFile();
            DeserializeTeamMembersInfoFromJsonFile();
            DeserializeBlacksInfoFromJsonFile();
            DeserializeGroupsInfoFromJsonFile();
            DeserializeFriendGroupInfoFromJsonFile();

            DeserializeChatRefsFromJsonFile();
            foreach (var roleInfoID in roleChatRefDataInfo.roleChatRefDatas.Values)
            {
                //DeserializeChatsInfoFromJsonFile(string.Format(chatsPath, Sys_Role.Instance.RoleId.ToString(), roleInfoID.infoID.ToString()));
            }

            DeserializeGroupChatRefsFromJsonFile();
            foreach (var groupInfoID in groupChatRefDataInfo.groupChatRefDatas.Values)
            {
                DeserializeGroupChatsInfoFromJsonFile(string.Format(groupChatsPath, Sys_Role.Instance.RoleId.ToString(), groupInfoID.infoID.ToString()));
            }
        }

        public override void OnLogout()
        {
            socialRolesInfo.rolesDic.Clear();
            socialRolesInfo.rolesList.Clear();

            socialRecentlysInfo.recentlyIds.Clear();
            socialRecentlysInfo.recentlyIdsDic.Clear();

            socialFriendsInfo.friendsIds.Clear();
            socialFriendsInfo.friendsIdsDic.Clear();

            socialTeamMembersInfo.teamMemberIds.Clear();
            socialTeamMembersInfo.teamMemberIdsDic.Clear();

            socialBlacksInfo.blacksIds.Clear();
            socialBlacksInfo.blacksIdsDic.Clear();

            socialGroupsInfo.groupsDic.Clear();
            socialGroupsInfo.groupsList.Clear();

            socialFriendGroupsInfo.friendGroupInfosDic.Clear();
            socialFriendGroupsInfo.friendGroupInfosList.Clear();

            roleChatDataInfo.roleChatDatas.Clear();
            roleChatDataInfo.roleChatDatasList.Clear();

            roleChatRefDataInfo.roleChatRefDatas.Clear();
            roleChatRefDataInfo.roleChatRefDatasList.Clear();

            groupChatDataInfo.groupChatDatas.Clear();
            groupChatDataInfo.groupChatDatasList.Clear();

            groupChatRefDataInfo.groupChatRefDatas.Clear();
            groupChatRefDataInfo.groupChatRefDatasList.Clear();
        }
    }
}