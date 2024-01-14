using Lib.Core;
using Logic.Core;
using System.IO;
using UnityEngine;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 默认角色ICON外显表ID///
        /// </summary>
        const uint DEFAULTROLEICONID = 100;

        /// <summary>
        /// 默认角色ICON框外显表ID///
        /// </summary>
        const uint DEFAULTROLEICONFRAMEID = 200;

        /// <summary>
        /// 默认聊天信息框外显表ID///
        /// </summary>
        const uint DEFAULTCHATFRAMEID = 300;

        /// <summary>
        /// 默认聊天信息文本格式外显表ID///
        /// </summary>
        const uint DEFAULTCHATTEXTID = 500;

        /// <summary>
        /// 系统消息ID///
        /// </summary>
        public static readonly ulong socialSystemID = 1;

        /// <summary>
        /// 魔力精灵ID///
        /// </summary>
        public static readonly ulong socialMOLIID = 2;

        /// <summary>
        /// 系统提示消息ID///
        /// 例如："你已添加XXX为好友，开始聊天吧"///
        /// </summary>
        public static readonly ulong socialSystemTipID = 3;

        /// <summary>
        /// 所有玩家信息存储路径///
        /// </summary>
        static readonly string allRolesPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/allRoles.log";

        /// <summary>
        /// 最近聊天玩家索引存储路径///
        /// </summary>
        static readonly string recentlysPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/recenltys.log";

        /// <summary>
        /// 好友索引存储路径///
        /// </summary>
        static readonly string friendsPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/friends.log";

        /// <summary>
        /// 队友索引存储路径///
        /// </summary>
        static readonly string teamMembersPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/teamMembers.log";

        /// <summary>
        /// 黑名单索引存储路径///
        /// </summary>
        static readonly string blacksPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/blacks.log";

        /// <summary>
        /// 所有群组数据存储路径///
        /// </summary>
        static readonly string groupsPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/groups.log";

        /// <summary>
        /// 自定义分组数据存储路径///
        /// </summary>
        static readonly string friendGroupsPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/friendGroups.log";

        /// <summary>
        /// 私聊索引存储路径///
        /// </summary>
        static readonly string chatRefsPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/chatRefs.log";
        
        /// <summary>
        /// 私聊数据存储路径///
        /// </summary>
        public readonly string chatsPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/{1}_chats.log";

        /// <summary>
        /// 群聊索引存储路劲///
        /// </summary>
        static readonly string groupRefsPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/groupChatRefs.log";

        /// <summary>
        /// 群聊数据存储路径///
        /// </summary>
        public readonly string groupChatsPath = Path.GetFullPath(Framework.Consts.persistentDataPath) + "/{0}/{1}_groupChats.log";

        
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnSetRoleInfoSuccess,       //设置社交信息成功
            OnIntimacyChange,           //亲密度更新
            OnSendGiftSuccess,          //赠送礼物成功
            OnReceiveGift,              //被送礼物
            OnGetGiftSuccess,           //接受礼物成功

            OnGetBriefInfoSuccess,      //获取玩家详细信息成功
            OnGetDetailSearchFriendSuccess,
            OnGetRelationSearchFriendSuccess,
            OnAddFriendSuccess,         //添加好友成功
            OnDelFriendSuccess,         //删除好友成功
            OnCreateOrSettingFriendGroupSuccess, //创建或编辑好友分组成功
            OnDelFriendGroupSuccess,    //删除好友分组成功
            OnFriendOnLine,             //好友上线
            OnFriendOffLine,            //好友下线
            OnSendSingleChat,           //发送个人消息
            OnGetSingleChat,            //收到个人消息
            OnClearSingleChatLog,       //清空个人聊天记录
            OnReadRoleChat,             //阅读个人聊天
            OnAddBlack,
            OnRemoveBlack,
            OnReNameSuccess,
            OnRoleInfoUpdate,           //人物信息刷新

            OnGetGroupInfo,             //获取全部群组信息
            OnGetOneGroupDetailInfo,    //获取某个群组的详细信息
            OnCreateGroupSuccess,       //创建群组成功
            OnChangeGroupName,          //修改群组名字
            OnChangeGroupNotice,        //修改群组公告
            OnDismissGroup,             //解散群组
            OnSelfAddGroupSuccess,      //自己加群成功
            OnOtherAddGroupSuccess,     //其它人加群成功
            OnSelfQuitGroupSuccess,     //自己退群成功
            OnOtherQuitGroupSuccess,    //其它人退群成功
            OnSelfKickFromGroup,        //自己被踢出群
            OnOtherKickFromGroup,       //其它人被踢出群
            OnSendGroupChat,            //发送群组消息
            OnGetGroupChat,             //收到群组消息
            OnClearGroupChatLog,        //清空群组聊天记录

            InputChange,
            RecodeChange,
            OnAddTeamMember,

            CloseEmoji,
        }
    }

}