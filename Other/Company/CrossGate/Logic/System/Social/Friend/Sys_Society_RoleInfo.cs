using Logic.Core;
using System.Json;

namespace Logic
{
    public partial class Sys_Society : SystemModuleBase<Sys_Society>
    {
        /// <summary>
        /// 数据类(封装id，方便Json序列化)///
        /// </summary>
        public class InfoID
        {
            /// <summary>
            /// ID///
            /// </summary>
            public ulong infoID;

            /// <summary>
            /// 反序列化///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
            }
        }

        /// <summary>
        /// 玩家信息///
        /// </summary>
        public class RoleInfo
        {
            /// <summary>
            /// 角色UID///
            /// </summary>
            public ulong roleID;

            /// <summary>
            /// 角色名///
            /// </summary>
            public string roleName;

            /// <summary>
            /// 角色等级///
            /// </summary>
            public uint level;

            /// <summary>
            /// 是否在线///
            /// </summary>
            public bool isOnLine;

            /// <summary>
            /// 角色类型ID///
            /// </summary>
            public uint heroID;

            /// <summary>
            /// 角色职业///
            /// </summary>
            public uint occ;

            /// <summary>
            /// 角色进阶///
            /// </summary>
            public uint rank;

            /// <summary>
            /// 好友度///
            /// </summary>
            public uint friendValue;

            /// <summary>
            /// 上次发送消息时间///
            /// </summary>
            public uint lastChatTime;

            /// <summary>
            /// 头像ID///
            /// </summary>
            public uint iconId;

            /// <summary>
            /// 头像框ID///
            /// </summary>
            public uint iconFrameId;

            /// <summary>
            /// 成为队友的时间
            /// </summary>
            public uint beTeamMemberTime;

            /// <summary>
            /// 家族名///
            /// </summary>
            public string guildName;

            /// <summary>
            /// 道具赠送///
            /// </summary>
            public uint commonSendNum;

            /// <summary>
            /// 稀有道具赠送///
            /// </summary>
            public uint unCommonSendNum;

            /// <summary>
            /// 是否有礼物没领///
            /// </summary>
            public bool hasGift;

            /// <summary>
            /// 反序列化///
            /// </summary>
            /// <param name="jo"></param>
            public void DeserializeObject(JsonObject jo)
            {
                JsonHeler.DeserializeObject(jo, this);
            }
        }
    }
}