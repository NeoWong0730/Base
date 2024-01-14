using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class HornItemData
    {
        public CSVItem.Data mItemData;
        public CSVHorn.Data mHornData;
    }

    public partial class Sys_Chat : SystemModuleBase<Sys_Chat>
    {
        //>0 发送函数返回剩余CD 
        public const int Chat_Success_GM = -6;              //GM命令
        public const int Chat_Success = 0;
        public const int Chat_Error_ContentEmpty = -1;      //内容为空
        public const int Chat_Error_NotHasTeam = -2;        //没有组队
        public const int Chat_Error_NotHasGuild = -3;       //没有家族
        public const int Chat_Error_HornLack = -4;          //喇叭不足
        public const int Chat_SetVersion_HotFix = -5;       //改变本地版本中的热更方式
        public const int Chat_Voice_ToShort = -7;           //语音录制时长过短
        public const int Chat_Lv_Short_World = -8;          //等级不足
        public const int Chat_Lv_Short_Local = -9;          //等级不足
        public const int Chat_Lv_Short_Guild = -10;         //等级不足
        public const int Chat_Lv_Short_Team = -11;          //等级不足
        public const int Chat_Lv_Short_Horn = -12;          //等级不足
        public const int Chat_RealName_Error = -13;         //未实名认证
        public const int Chat_HasIllegalWord_Error = -14;   //有敏感词
        public const int Chat_Count_Up_Limit = -15;         //输入字数上限
        public const int Chat_Lv_Short_Career = -16;        //等级不足
        public const int Chat_Lv_Short_BraveGroup = -17;    //等级不足
        public const int Chat_Error_NotHasBraveGroup = -18; //没有勇者团

        public const string Chat_GM_Header = "%%$";         //GM命令的开头        


        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
            MessageAdd,                 //消息新增
            RecodeChange,               //历史纪录更改
            InputChange,                //输入信息更改
            HornSelectChange,           //广播选中更改            
            SimplifyMessageAdd,         //精简消息新增
            ScreenLockChange,           //精简消息锁定显示更改
            EnterRoom,                  //进入聊天室
            ExitRoom,                   //退出聊天室
            EndpointsUpdate,            //聊天室成员变化
            VoiceRecordStateChange,     //
            VoicePlayStateChange,       //
            MessageContentChange,       //文本内容更改
        }

        public enum EInputType
        {
            Chat = 0,
            Horn = 1,
        }

        /// <summary>
        /// 拓展数据的类型
        /// </summary>
        public enum EExtMsgType : uint
        {
            Normal = 0,
            Trade = 1,
            Voice = 2,
            Video=3,
        }

        public enum EChatContentReference
        {
            None = 0,

            SimplifyUI = 1,
            VoiceQueue = 2,
            HornHUD = 4,

            WorldChannelUI = 8,
            GuildChannelUI = 16,
            TeamChannelUI = 32,
            LookForTeamChannelUI = 64,
            LocalChannelUI = 128,
            PersonChannelUI = 256,
            HornChannelUI = 512,
            SystemChannelUI = 1024,
            CareerChannelUI = 2048,
            BraveGroupChannelUI = 4096,
        }

        public class SendTimeSpace
        {
            public float fTimeSpace;
            public float fLastTime;
            public SendTimeSpace(float timeSpace)
            {
                fTimeSpace = timeSpace;
            }

            public float Surplus()
            {
                return fTimeSpace - Time.unscaledTime - fLastTime;
            }

            public void Recode()
            {
                fLastTime = Time.unscaledTime;
            }
        }
        public class ChatBaseInfo
        {
            public EFightActorType eActorType;
            public ulong nRoleID;
            public uint nHeroID;
            public string sSenderName;
            public uint nHornItemID;

            public uint SenderChatFrame;
            public uint SenderChatText;
            public uint SenderHead;
            public uint SenderHeadFrame;
            public bool BackActivity;
        }
        public class ChatContent : IDisposable
        {
            public ulong uid = 0;
            public ChatType eChatType;
            public ChatBaseInfo mBaseInfo;
            public string sContent;
            public string sUIContent;
            public string sSimplifyUIContent;
            public string sFileID;
            public int fDuration;
            public bool bPlayed = false;
            public bool bHasIllegalWord = false;
            public ChatExtMsg mChatExtData;

            public uint nTimePoint;
            public EChatContentReference nReferenceFlags;            
            
            //public void CopyFrom(ChatContent other)
            //{
            //    eChatType = other.eChatType;
            //    mBaseInfo = other.mBaseInfo;
            //    sContent = other.sContent;
            //    sUIContent = other.sUIContent;
            //    sSimplifyUIContent = other.sSimplifyUIContent;
            //    sFileID = other.sFileID;
            //    fDuration = other.fDuration;
            //    bPlayed = other.bPlayed;
            //    bHasIllegalWord = other.bHasIllegalWord;
            //    mChatExtData = other.mChatExtData;
            //}

            /// <summary>
            /// 显示样式
            /// </summary>
            /// <returns>0 = 单条显示 1 = 右侧（自己） 2 = 左侧（其他玩家）</returns>
            public int DisplayType()
            {
                if (mBaseInfo == null ||
                    eChatType == ChatType.Horn ||
                    eChatType == ChatType.Notice ||
                    eChatType == ChatType.Person ||
                    eChatType == ChatType.System)
                {                    
                    return 0;
                }
                else if (mBaseInfo.eActorType == EFightActorType.Monster
                    || mBaseInfo.eActorType == EFightActorType.Partner
                    || mBaseInfo.eActorType == EFightActorType.Pet)
                {
                    return 0;
                }
                else
                {
                    return Sys_Role.Instance.IsSelfRole(mBaseInfo.nRoleID) ? 1 : 2;
                }
            }

            public EChatContentReference AddReference(EChatContentReference chatContentReference)
            {
                nReferenceFlags |= chatContentReference;
                return nReferenceFlags;
            }

            public EChatContentReference RemoveReference(EChatContentReference chatContentReference)
            {
                nReferenceFlags &= (~chatContentReference);
                return nReferenceFlags;
            }

            public void Dispose()
            {
                uid = 0;
                eChatType = ChatType.World;
                mBaseInfo = null;
                sContent = null;
                sUIContent = null;
                sSimplifyUIContent = null;
                sFileID = null;
                fDuration = 0;
                bPlayed = false;
                bHasIllegalWord = false;
                mChatExtData = null;
                nTimePoint = 0;
                nReferenceFlags = EChatContentReference.None;
            }
        }

        public class ChatChannelData
        {
            readonly public float fTimeSpace;
            public float fLastSendTime;
            public ChatType eChatType;
            public List<ChatContent> mMessages = new List<ChatContent>(256);
            //最后接收到消息的时间
            public uint LastReceiveMessageTime = 0;
            //间隔多少时间标记时间
            public uint ReceiveMessageMaxInterval = 180;
            //等级限制
            internal uint nLvLimit;
            //已经移除的数量
            public int nRemovedCount { get; private set; }
            //最下面一条已读的消息
            public int nMaxReadCount { get; private set; }
            //最大的保留数量
            public int nMaxSaveCount { get { return mMessages.Capacity; } }

            public EChatContentReference eChatContentReference = EChatContentReference.None;

            public ChatChannelData(float spaceTime, uint lvLimit, EChatContentReference chatContentReference)
            {
                fTimeSpace = spaceTime;
                nLvLimit = lvLimit;
                eChatContentReference = chatContentReference;
            }

            public ChatContent GetMessageByIndex(int index)
            {
                if (mMessages.Count > index && index >= 0)
                {
                    return mMessages[index];
                }
                return null;
            }

            public float Surplus()
            {
                return fTimeSpace + fLastSendTime - Time.unscaledTime;
            }

            public void Recode()
            {
                fLastSendTime = Time.unscaledTime;
            }

            public void ClearCD()
            {
                fLastSendTime = 0;
            }

            public int GetAllCount()
            {
                return nRemovedCount + mMessages.Count;
            }

            public void Clear()
            {
                mMessages.Clear();
                fLastSendTime = 0;
            }

            public bool IsFull()
            {
                return mMessages.Count >= mMessages.Capacity;
            }

            public ChatContent RemoveTop()
            {
                ChatContent content = mMessages[0];                
                mMessages.RemoveAt(0);
                ++nRemovedCount;
                return content;
            }

            public void AddLast(ChatContent content)
            {
                mMessages.Add(content);                
            }
        }

        public List<HornItemData> mSingleServerHornDatas = null;
        public List<HornItemData> mFullServerHornDatas = null;
        public uint nLastSelectedSingleServerHorn { get; private set; }
        public uint nLastSelectedFullServerHorn { get; private set; }
        public uint nCurrentSelectedHorn { get; private set; }
        public uint nCurrentSelectedHornType { get; private set; }

        public ChatChannelData mSimplifyDisplay = null;

        //存储了有功能的聊天内容 
        private Dictionary<ulong, ChatContent> mChatContentDic = new Dictionary<ulong, ChatContent>();
        private ulong currentUID = 0;

        private Dictionary<ChatType, ChatChannelData> mData = new Dictionary<ChatType, ChatChannelData>(9, new FastEnumIntEqualityComparer<ChatType>());
        public ChatType eChatType = ChatType.World;

        private ChatType _eSystemChannelShow = ChatType.System;
        public ChatType eSystemChannelShow
        {
            get
            {
                return _eSystemChannelShow;
                //return (ChatType)OptionManager.Instance.GetInt(OptionManager.EOptionID.ChatSystemChannelShow);
            }

            private set
            {
                _eSystemChannelShow = value;
                //OptionManager.Instance.SetInt(OptionManager.EOptionID.ChatSystemChannelShow, (int)value, false);
            }
        }

        //private int bSimplifyDisplayFlag = 0xffff; //精简界面消息接受flag

        //public readonly List<InputCache> mContentRecords = new List<InputCache>(10);
        public InputCacheRecord mInputCacheRecord = null;

        public EInputType eCurrentInput = EInputType.Chat;
        public InputCache mInputCache
        {
            get
            {
                if (eCurrentInput == EInputType.Horn)
                {
                    return mHornInputCache;
                }
                else
                {
                    return mChatInputCache;
                }
            }
        }

        private InputCache mChatInputCache = new InputCache(true);
        private InputCache mHornInputCache = new InputCache();

        public Queue<ChatContent> mChatHUD = new Queue<ChatContent>();        
        public readonly ChatBaseInfo gSystemChatBaseInfo = null; //new ChatBaseInfo();// "魔力精灵";

        /// <summary>
        /// 广播播放次数
        /// </summary>
        public int nHornPlayTimes { get; private set; }
        /// <summary>
        /// 系统广播播放次数
        /// </summary>
        public int nSystemHornPlayTimes { get; private set; }
        /// <summary>
        /// 广播滚动速度
        /// </summary>
        public float nHornPlaySpeed { get; private set; }

        public override void OnLogin()
        {
            mInputCacheRecord = new InputCacheRecord(10, Sys_Role.Instance.RoleId.ToString());
            mInputCacheRecord.ReadInputCache();

            _OnLogin();
            _OnLogin_BlackIndustry();
        }

        public override void OnLogout()
        {
            _OnLogout_BlackIndustry();
            _UninitSDK();
                        
            mChatInputCache.Clear();
            mHornInputCache.Clear();

            while(mChatHUD.Count > 0)
            {
                ChatContent content = mChatHUD.Dequeue();
                RemoveChatContentReference(content, EChatContentReference.HornHUD);
            }            
            //mContents.Clear();

            mChatContentDic.Clear();

            if (mSimplifyDisplay != null)
            {
                for (int i = mSimplifyDisplay.mMessages.Count - 1; i >= 0; --i)
                {
                    RemoveChatContentReference(mSimplifyDisplay.mMessages[i], mSimplifyDisplay.eChatContentReference);
                }

                mSimplifyDisplay.Clear();
            }

            foreach (var data in mData.Values)
            {
                for (int i = data.mMessages.Count - 1; i >= 0; --i)
                {
                    RemoveChatContentReference(data.mMessages[i], data.eChatContentReference);
                }

                data.Clear();
            }
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdGm.Req, (ushort)CmdGm.Res, OnReceivedGM, CmdGmRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdSocial.PubChatReq, (ushort)CmdSocial.PubChatRes, OnPubChatRes, CmdSocialPubChatRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.PubChatNtf, OnPubChatNtf, CmdSocialPubChatNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.LocalChatNtf, OnLocalChatNtf, CmdSocialLocalChatNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.HornChatNtf, OnHornChatNtf, CmdSocialHornChatNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSocial.GameChatNtf, OnGameChatNtf, CmdSocialGameChatNtf.Parser);

            _Init();            

            //设置跑马灯时间
            CSVParam.Data csvHornTimes = CSVParam.Instance.GetConfData(521);
            List<int> hornTimes = ReadHelper.ReadArray_ReadInt(csvHornTimes.str_value, '|');
            nSystemHornPlayTimes = hornTimes.Count > 1 ? hornTimes[1] : 0;
            nHornPlayTimes = hornTimes.Count > 0 ? hornTimes[0] : 0;

            CSVParam.Data csvHornSpeed = CSVParam.Instance.GetConfData(522);
            nHornPlaySpeed = ReadHelper.ReadFloat(csvHornSpeed.str_value);

            //设置频道及频道发言间隔时间
            CSVParam.Data csvTimes = CSVParam.Instance.GetConfData(520);
            List<float> times = ReadHelper.ReadArray_ReadFloat(csvTimes.str_value, '|');
            float world = times.Count > 0 ? times[0] * 0.001f : 0;
            float Local = times.Count > 1 ? times[1] * 0.001f : 0;
            float Guild = times.Count > 2 ? times[2] * 0.001f : 0;
            float Team = times.Count > 3 ? times[3] * 0.001f : 0;
            float LookForTeam = times.Count > 4 ? times[4] * 0.001f : 0;
            float Career = times.Count > 5 ? times[5] * 0.001f : 0;
            float BraveGroupLv = times.Count > 6 ? times[6] * 0.001f : 0;

            //聊天等级限制
            CSVParam.Data csvLvLimit = CSVParam.Instance.GetConfData(923);
            uint.TryParse(csvLvLimit.str_value, out uint localLv);

            csvLvLimit = CSVParam.Instance.GetConfData(924);
            uint.TryParse(csvLvLimit.str_value, out uint worldLv);

            csvLvLimit = CSVParam.Instance.GetConfData(925);
            uint.TryParse(csvLvLimit.str_value, out uint guildLv);

            csvLvLimit = CSVParam.Instance.GetConfData(926);
            uint.TryParse(csvLvLimit.str_value, out uint teamLv);

            csvLvLimit = CSVParam.Instance.GetConfData(927);
            uint.TryParse(csvLvLimit.str_value, out uint hornLv);

            csvLvLimit = CSVParam.Instance.GetConfData(1299);
            uint.TryParse(csvLvLimit.str_value, out uint careerLv);

            csvLvLimit = CSVParam.Instance.GetConfData(1408);
            uint.TryParse(csvLvLimit.str_value, out uint braveGroupLv);

            mData.Add(ChatType.World, new ChatChannelData(world, worldLv, EChatContentReference.WorldChannelUI));
            mData.Add(ChatType.Guild, new ChatChannelData(Guild, guildLv, EChatContentReference.GuildChannelUI));
            mData.Add(ChatType.Team, new ChatChannelData(Team, teamLv, EChatContentReference.TeamChannelUI));
            mData.Add(ChatType.LookForTeam, new ChatChannelData(LookForTeam, 0, EChatContentReference.LookForTeamChannelUI));
            mData.Add(ChatType.Local, new ChatChannelData(Local, localLv, EChatContentReference.LocalChannelUI));
            mData.Add(ChatType.Person, new ChatChannelData(0, 0, EChatContentReference.PersonChannelUI));
            mData.Add(ChatType.Horn, new ChatChannelData(1, hornLv, EChatContentReference.HornChannelUI));
            mData.Add(ChatType.System, new ChatChannelData(0, 0, EChatContentReference.SystemChannelUI));
            mData.Add(ChatType.Career, new ChatChannelData(Career, careerLv, EChatContentReference.CareerChannelUI));
            mData.Add(ChatType.BraveGroup, new ChatChannelData(BraveGroupLv, braveGroupLv, EChatContentReference.BraveGroupChannelUI));

            mSimplifyDisplay = new ChatChannelData(0, 0, EChatContentReference.SimplifyUI);

            //注册输入回调
            mChatInputCache.onChange = OnInputChange;
            mHornInputCache.onChange = OnInputChange;

            //解析喇叭
            mSingleServerHornDatas = new List<HornItemData>(CSVHorn.Instance.Count / 2);
            mFullServerHornDatas = new List<HornItemData>(CSVHorn.Instance.Count / 2);

            List<HornItemData> temp = null;

            //foreach (var data in CSVHorn.Instance.GetDictData())
            var hornDatas = CSVHorn.Instance.GetAll();
            for (int csvIndex = 0, len = hornDatas.Count; csvIndex < len; ++csvIndex)
            {
                //CSVHorn.Data hornData = data.Value;
                CSVHorn.Data hornData = hornDatas[csvIndex];

                CSVItem.Data itemData = CSVItem.Instance.GetConfData(hornData.id);
                if (itemData != null)
                {
                    if (itemData.type_id == (uint)EItemType.SingleServerHorn)
                    {
                        temp = mSingleServerHornDatas;
                    }
                    else
                    {
                        temp = mFullServerHornDatas;
                    }

                    HornItemData hornItemData = new HornItemData();
                    hornItemData.mHornData = hornData;
                    hornItemData.mItemData = itemData;

                    int index = 0;
                    for (int i = 0; i < temp.Count; ++i)
                    {
                        if (temp[i].mHornData.sortIndex_id > hornData.sortIndex_id)
                        {
                            index = i;
                            break;
                        }
                        else
                        {
                            index = i + 1;
                        }
                    }
                    temp.Insert(index, hornItemData);
                }
            }

            if (mFullServerHornDatas.Count > 0)
            {
                HornItemData itemData = mFullServerHornDatas[0];
                nCurrentSelectedHorn = nLastSelectedFullServerHorn = itemData.mItemData.id;
                nCurrentSelectedHornType = itemData.mItemData.type_id;
            }

            if (mSingleServerHornDatas.Count > 0)
            {
                HornItemData itemData = mSingleServerHornDatas[0];
                nCurrentSelectedHorn = nLastSelectedSingleServerHorn = itemData.mItemData.id;
                nCurrentSelectedHornType = itemData.mItemData.type_id;
            }

            //设置跑马灯时间
            CSVParam.Data csvTextCountLimits = CSVParam.Instance.GetConfData(523);
            List<int> textCountLimits = ReadHelper.ReadArray_ReadInt(csvTextCountLimits.str_value, '|');
            mChatInputCache.SetLimitCount(textCountLimits.Count > 0 ? textCountLimits[0] : 0);
            mHornInputCache.SetLimitCount(textCountLimits.Count > 1 ? textCountLimits[1] : 0);
            //gSystemChatBaseInfo.nRoleID = 0;
            //gSystemChatBaseInfo.nHeroID = 0;
            //gSystemChatBaseInfo.sSenderName = LanguageHelper.GetTextContent(2004003);// "魔力精灵";            
        }

        private void OnInputChange()
        {
            eventEmitter.Trigger(EEvents.InputChange);
        }

        private void OnPubChatRes(NetMsg msg)
        {
            CmdSocialPubChatRes res = NetMsgUtil.Deserialize<CmdSocialPubChatRes>(CmdSocialPubChatRes.Parser, msg);
            if (res.Ret == 0)
                return;
            ChatChannelData channelData = GetChatChannelData(res.ChatType);
            if (channelData != null)
            {
                channelData.ClearCD();
            }
            uint languageID = 100000u + res.Ret;
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetErrorCodeContent(languageID));
        }

        private void OnPubChatNtf(NetMsg msg)
        {
            CmdSocialPubChatNtf ntf = NetMsgUtil.Deserialize<CmdSocialPubChatNtf>(CmdSocialPubChatNtf.Parser, msg);

            ChatBaseInfo chatBaseInfo = new ChatBaseInfo();
            chatBaseInfo.nRoleID = ntf.RoleId;
            chatBaseInfo.nHeroID = ntf.HeroId;
            chatBaseInfo.sSenderName = ntf.SenderName.ToStringUtf8();
            chatBaseInfo.SenderChatFrame = ntf.SenderChatFrame;
            chatBaseInfo.SenderChatText = ntf.SenderChatText;
            chatBaseInfo.SenderHead = ntf.SenderHead;
            chatBaseInfo.SenderHeadFrame = ntf.SenderHeadFrame;
            chatBaseInfo.BackActivity = ntf.IsBack;
            PushMessageFromServer(ntf.ChatType, chatBaseInfo, ntf.ChatMsg.ToStringUtf8(), EMessageProcess.None, (EExtMsgType)ntf.ExtraType, ntf.ExtraMsg, ntf.HasMaskWord);
        }

        private void OnLocalChatNtf(NetMsg msg)
        {
            CmdSocialLocalChatNtf ntf = NetMsgUtil.Deserialize<CmdSocialLocalChatNtf>(CmdSocialLocalChatNtf.Parser, msg);

            ChatBaseInfo chatBaseInfo = new ChatBaseInfo();
            chatBaseInfo.nRoleID = ntf.RoleId;
            chatBaseInfo.nHeroID = ntf.HeroId;
            chatBaseInfo.sSenderName = ntf.SenderName.ToStringUtf8();
            chatBaseInfo.SenderChatFrame = ntf.SenderChatFrame;
            chatBaseInfo.SenderChatText = ntf.SenderChatText;
            chatBaseInfo.SenderHead = ntf.SenderHead;
            chatBaseInfo.SenderHeadFrame = ntf.SenderHeadFrame;
            chatBaseInfo.BackActivity = ntf.IsBack;

            PushMessageFromServer(ChatType.Local, chatBaseInfo, ntf.ChatMsg.ToStringUtf8(), EMessageProcess.None, (EExtMsgType)ntf.ExtraType, ntf.ExtraMsg, ntf.HasMaskWord);
        }

        private void OnGameChatNtf(NetMsg msg)
        {
            CmdSocialGameChatNtf ntf = NetMsgUtil.Deserialize<CmdSocialGameChatNtf>(CmdSocialGameChatNtf.Parser, msg);

            ChatType chatType = ntf.ChatType;
            ChatBaseInfo chatBaseInfo = null;
            if (chatType == ChatType.GuildNotify)
            {
                chatType = ChatType.Guild;
            }
            else
            {
                chatBaseInfo = new ChatBaseInfo();
                chatBaseInfo.nRoleID = ntf.RoleId;
                chatBaseInfo.nHeroID = ntf.HeroId;
                chatBaseInfo.sSenderName = ntf.SenderName.ToStringUtf8();

                chatBaseInfo.SenderChatFrame = ntf.SenderChatFrame;
                chatBaseInfo.SenderChatText = ntf.SenderChatText;
                chatBaseInfo.SenderHead = ntf.SenderHead;
                chatBaseInfo.SenderHeadFrame = ntf.SenderHeadFrame;
                chatBaseInfo.BackActivity = false;
            }

            PushMessageFromServer(chatType, chatBaseInfo, ntf.ChatMsg.ToStringUtf8(), EMessageProcess.None, (EExtMsgType)ntf.ExtraType, ntf.ExtraMsg, ntf.HasMaskWord);
        }

        private void OnHornChatNtf(NetMsg msg)
        {
            CmdSocialHornChatNtf ntf = NetMsgUtil.Deserialize<CmdSocialHornChatNtf>(CmdSocialHornChatNtf.Parser, msg);

            //TODO 物品消耗
            //chatBaseInfo.nItemID = ntf.ItemId;

            if (ntf.HeroId > 0)
            {
                ChatBaseInfo chatBaseInfo = new ChatBaseInfo();
                chatBaseInfo.nHeroID = ntf.HeroId;
                chatBaseInfo.sSenderName = ntf.SenderName.ToStringUtf8();
                chatBaseInfo.nHornItemID = ntf.ItemId;

                chatBaseInfo.SenderChatFrame = ntf.SenderChatFrame;
                chatBaseInfo.SenderChatText = ntf.SenderChatText;
                chatBaseInfo.SenderHead = ntf.SenderHead;
                chatBaseInfo.SenderHeadFrame = ntf.SenderHeadFrame;
                chatBaseInfo.BackActivity = false;

                PushMessageFromServer(ChatType.Horn, chatBaseInfo, ntf.ChatMsg.ToStringUtf8(), EMessageProcess.None, (EExtMsgType)ntf.ExtraType, ntf.ExtraMsg, ntf.HasMaskWord);
            }
            else
            {
                //默认heroid 不大于0的为系统公告
                PushMessageFromServer(ChatType.Notice, null, ntf.ChatMsg.ToStringUtf8(), EMessageProcess.None, (EExtMsgType)ntf.ExtraType, ntf.ExtraMsg, ntf.HasMaskWord);
            }
        }

        public int CheckCanSend(ChatType type)
        {
            //if (!SDKManager.GetRealNameStatus())
            //    return Chat_RealName_Error;

            switch (type)
            {                
                case ChatType.Guild:
                    {
                        if (!Sys_Family.Instance.familyData.isInFamily)
                        {
                            return Chat_Error_NotHasGuild;
                        }
                    }
                    break;
                case ChatType.Team:
                    {
                        if (!Sys_Team.Instance.HaveTeam)
                        {
                            return Chat_Error_NotHasTeam;
                        }
                    }
                    break;
                case ChatType.BraveGroup:
                    {
                        if (Sys_WarriorGroup.Instance.MyWarriorGroup.GroupUID == 0)
                        {
                            return Chat_Error_NotHasBraveGroup;
                        }
                    }
                    break;
                default:
                    break;
            }          

            ChatChannelData chatChannelData = null;
            if (mData.TryGetValue(type, out chatChannelData))
            {
                if (Sys_Role.Instance.Role.Level < chatChannelData.nLvLimit)
                {
                    switch (type)
                    {
                        case ChatType.World:
                            return Chat_Lv_Short_World;
                        case ChatType.Guild:
                            return Chat_Lv_Short_Guild;
                        case ChatType.Team:
                            return Chat_Lv_Short_Team;
                        case ChatType.Local:
                            return Chat_Lv_Short_Local;
                        case ChatType.Horn:
                            return Chat_Lv_Short_Horn;
                        case ChatType.Career:
                            return Chat_Lv_Short_Career;
                        case ChatType.BraveGroup:
                            return Chat_Lv_Short_BraveGroup;
                        default:
                            break;
                    }
                }

                float surplus = chatChannelData.Surplus();
                if (surplus > 0f)
                {
                    //"当前频道{0}秒后可再次发送"
                    //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007907, Mathf.CeilToInt(surplus).ToString()));
                    return Mathf.CeilToInt(surplus);
                }
            }
            return Chat_Success;
        }        

        public int CheckHornItem(uint itemID)
        {
            if (Sys_Bag.Instance.GetItemCount(itemID) > 0)
            {
                return Chat_Success;
            }

            CSVHorn.Data itemData = CSVHorn.Instance.GetConfData(itemID);
            if (itemData != null)
            {
                long hasPriceCount = Sys_Bag.Instance.GetItemCount(1);
                if (hasPriceCount >= (long)itemData.price)
                {
                    return Chat_Success;
                }
            }

            return Chat_Error_HornLack;
        }

        private int CheckContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                //发送内容为空
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007912));
                return Chat_Error_ContentEmpty;
            }
            return Chat_Success;
        }

        private void RecodeSendTime(ChatType type)
        {
            ChatChannelData chatChannelData = null;
            if (mData.TryGetValue(type, out chatChannelData))
            {
                chatChannelData.Recode();
            }
        }

        private void _SendContent(ChatType type, string content, EExtMsgType extMsgType, Google.Protobuf.ByteString extraData)
        {
            RecodeSendTime(type);

            //判断发送权限
            switch (type)
            {
                case ChatType.Local:
                    {
                        CmdSocialLocalChatReq req = new CmdSocialLocalChatReq();
                        req.ExtraType = (uint)extMsgType;
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            req.ChatMsg = Google.Protobuf.ByteString.CopyFrom(content, System.Text.Encoding.UTF8);
                        }
                        if (extraData != null)
                        {
                            req.ExtraMsg = extraData;
                        }

                        NetClient.Instance.SendMessage((ushort)CmdSocial.LocalChatReq, req);
                    }
                    break;
                case ChatType.Career:
                case ChatType.Team:
                case ChatType.Guild:
                case ChatType.BraveGroup:
                {
                        CmdSocialGameChatReq req = new CmdSocialGameChatReq();
                        req.ChatType = type;
                        req.ExtraType = (uint)extMsgType;
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            req.ChatMsg = Google.Protobuf.ByteString.CopyFrom(content, System.Text.Encoding.UTF8);
                        }
                        if (extraData != null)
                        {
                            req.ExtraMsg = extraData;
                        }

                        NetClient.Instance.SendMessage((ushort)CmdSocial.GameChatReq, req);
                        //if (type == ChatType.Guild)
                        //    Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event24);
                    }
                    break;
                default:
                    {
                        CmdSocialPubChatReq req = new CmdSocialPubChatReq();
                        req.ChatType = type;
                        req.ExtraType = (uint)extMsgType;
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            req.ChatMsg = Google.Protobuf.ByteString.CopyFrom(content, System.Text.Encoding.UTF8);
                        }
                        if (extraData != null)
                        {
                            req.ExtraMsg = extraData;
                        }

                        NetClient.Instance.SendMessage((ushort)CmdSocial.PubChatReq, req);
                    }
                    break;
            }
        }
        public int SendVoice(ChatType type, string content, string fileID, int duration, int hitFlag)
        {
            //类型为家族红包时content文本按配置概率比对
            if (type == ChatType.FamilyRedPacket)
            {
                string curContent = Sys_Family.Instance.CheckVoiceIsPass(content);
                if (Sys_Family.Instance.voiceIsCanSend)
                {
                    type = ChatType.Guild;
                    content = curContent;
                }
                else
                    return Chat_Error_NotHasGuild;
            }

            int rlt = CheckCanSend(type);
            if (rlt != Chat_Success)
            {
                return rlt;
            }

            ChatExtMsgVoice chatExtMsg = new ChatExtMsgVoice();
            chatExtMsg.Duration = duration;
            chatExtMsg.FileID = Google.Protobuf.ByteString.CopyFrom(fileID, System.Text.Encoding.UTF8); //文件ID
            chatExtMsg.HitFlag = hitFlag;

            Google.Protobuf.ByteString extraData = NetMsgUtil.ToByteString(chatExtMsg);

            _SendContent(type, content, EExtMsgType.Voice, extraData);
            return rlt;
        }

        /// <summary>        
        /// 发送消息
        /// </summary>
        /// <param name="type"></param>
        /// <param name="content">内容 格式 : [@RoleName][#ItemIndex][!TaskID][$TeamID]</param>
        /// <returns></returns>
        public int SendContent(ChatType type, string content, EExtMsgType extType = EExtMsgType.Normal)
        { 
            int rlt = CheckCanSend(type);
            if (rlt != Chat_Success)
            {
                return rlt;
            }

            rlt = CheckContent(content);
            if (rlt != Chat_Success)
            {
                return rlt;
            }

            _SendContent(type, content, extType, null);
            return rlt;
        }

        public int SendContent(ChatType type, InputCache input, EExtMsgType extType = EExtMsgType.Normal)
        {
            if (input.bCheckGM && extType == EExtMsgType.Normal)
            {
                string s = input.GetContent();
                if (s.StartsWith(Chat_GM_Header, StringComparison.Ordinal))
                {
#if GM_PROPAGATE_VERSION && UNITY_STANDALONE_WIN
                    if (Gm_PropagateVersion(s))
                        return Chat_Success_GM;
#endif
                    CmdSocialCurrentChannelExtralReq req = new CmdSocialCurrentChannelExtralReq();
                    req.Chatmsg = Google.Protobuf.ByteString.CopyFrom(s, System.Text.Encoding.UTF8);
                    NetClient.Instance.SendMessage((ushort)CmdSocial.CurrentChannelExtralReq, req);
                    return Chat_Success_GM;
                }
            }

            int rlt = CheckCanSend(type);
            if (rlt != Chat_Success)
            {
                return rlt;
            }

            if (extType == EExtMsgType.Normal || extType == EExtMsgType.Trade || extType == EExtMsgType.Video)
            {
                ChatExtMsg chatExtMsg = null;
                string content = input.GetSendContent(out chatExtMsg);

                rlt = CheckContent(content);
                if (rlt != Chat_Success)
                {
                    return rlt;
                }

                Google.Protobuf.ByteString extraData = chatExtMsg != null ? NetMsgUtil.ToByteString(chatExtMsg) : null;
                RecodeInputCache(input);

                _SendContent(type, content, extType, extraData);
            }
            else
            {
                DebugUtil.LogErrorFormat("EExtMsgType类型无法解析 {0}", extType.ToString());
            }

            return rlt;
        }

        public int SendContentHorn(uint itemId, InputCache input)
        {
            int rlt = CheckCanSend(ChatType.Horn);
            if (rlt != Chat_Success)
            {
                return rlt;
            }

            rlt = CheckHornItem(itemId);
            if (rlt != Chat_Success)
            {
                return rlt;
            }

            RecodeInputCache(input);

            ChatExtMsg chatExtMsg = null;
            string content = input.GetSendContent(out chatExtMsg);

            rlt = CheckContent(content);
            if (rlt != 0)
            {
                return rlt;
            }

            ChatChannelData chatChannelData = null;
            if (mData.TryGetValue(ChatType.Horn, out chatChannelData))
            {
                chatChannelData.Recode();
            }

            byte[] extraData = chatExtMsg != null ? NetMsgUtil.Serialzie(chatExtMsg) : null;

            CmdSocialHornChatReq req = new CmdSocialHornChatReq();
            req.ItemId = itemId;
            req.ChatMsg = Google.Protobuf.ByteString.CopyFrom(content, System.Text.Encoding.UTF8);
            if (extraData != null)
            {
                req.ExtraMsg = Google.Protobuf.ByteString.CopyFrom(extraData);
            }

            NetClient.Instance.SendMessage((ushort)CmdSocial.HornChatReq, req);
            return rlt;
        }

        [Flags]
        public enum EMessageProcess
        {
            None = 0,            
            AddUID = 1,
            IgnoreSimplify = 2,
        }

        /// <summary>
        /// 压入聊天数据
        /// </summary>
        /// <param name="channel">频道 个人/系统/公告 : channel = System, chatBaseInfo = null</param>
        /// <param name="chatBaseInfo">发送者的信息 魔力精灵使用 Sys_Chat.Instance.gSystemChatBaseInfo </param>
        /// <param name="content">内容 格式 : [@RoleName][#ItemIndex][!TaskID][$TeamID]</param>        
        /// <param name="messageProcess"></param>
        /// <param name="extMsgType">附加数据的类型</param>
        /// <param name="datas">附加数据</param>        
        /// <param name="hasIllegalWord">是否需要屏蔽字</param>
        public void PushMessageFromServer(
            ChatType channel,
            ChatBaseInfo chatBaseInfo,
            string content,
            EMessageProcess messageProcess,
            EExtMsgType extMsgType,
            Google.Protobuf.ByteString datas,
            bool hasIllegalWord)
        {
            //如果是黑名单中的玩家
            if (chatBaseInfo != null && Sys_Society.Instance.IsInBlackList(chatBaseInfo.nRoleID))
                return;

            ChatExtMsgVoice chatExtMsgVoice = null;
            ChatExtMsg chatExtMsgItem = null;

            //当内容为空的时候 信息为语音
            if (datas != null && datas.Length > 0)
            {
                switch (extMsgType)
                {
                    case EExtMsgType.Normal:
                    case EExtMsgType.Trade:
                        NetMsgUtil.TryDeserialize<ChatExtMsg>(ChatExtMsg.Parser, datas, out chatExtMsgItem);
                        break;
                    case EExtMsgType.Voice:
                        NetMsgUtil.TryDeserialize<ChatExtMsgVoice>(ChatExtMsgVoice.Parser, datas, out chatExtMsgVoice);
                        break;
                    default:
                        break;
                }
            }

            DebugUtil.LogFormat(ELogType.eChat, "PushMessage({0}, {1}, {2}, {3}, {4})", channel.ToString(), chatBaseInfo == null ? "null" : chatBaseInfo.sSenderName, content, chatExtMsgVoice, messageProcess.ToString());

            //构建单条聊天数据
            PushMessage(channel, chatBaseInfo, content, messageProcess, extMsgType, chatExtMsgItem, chatExtMsgVoice, hasIllegalWord);
        }

        /// <summary>
        /// 压入聊天数据
        /// </summary>
        /// <param name="channel">频道 个人/系统/公告 : channel = System, chatBaseInfo = null</param>
        /// <param name="chatBaseInfo">发送者的信息 魔力精灵使用 Sys_Chat.Instance.gSystemChatBaseInfo </param>
        /// <param name="content">内容 格式 : [@RoleName][#ItemIndex][!TaskID][$TeamID]</param>        
        /// <param name="messageProcess">额外的标识</param>
        /// <param name="extMsgType">附加数据的类型</param>
        /// <param name="chatExtMsgItem">道具数据</param>
        /// <param name="chatExtMsgVoice">语音数据</param>
        /// <param name="hasIllegalWord">是否需要屏蔽字</param>
        public ulong PushMessage(
            ChatType channel,
            ChatBaseInfo chatBaseInfo,
            string content,
            EMessageProcess messageProcess = EMessageProcess.None,
            EExtMsgType extMsgType = EExtMsgType.Normal,
            ChatExtMsg chatExtMsgItem = null,
            ChatExtMsgVoice chatExtMsgVoice = null,
            bool hasIllegalWord = false)
        {
            #region 构建单条聊天数据
            
            //构建最终显示的不带发送者名字前缀的内容
            string noNameContent;
            if (extMsgType == EExtMsgType.Voice)
            {
                noNameContent = content;
            }
            else
            {
                noNameContent = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), chatExtMsgItem, content);
                if (extMsgType== EExtMsgType.Video)
                {
                    string[] ss = content.Split('_');
                    string[] param = ss[1].Split('>');
                    string[] Ids = param[0].Split('|');
                    ulong videoId = 0u;
                    ulong authorId = 0u;
                    ulong.TryParse(Ids[0], out videoId);
                    ulong.TryParse(Ids[1], out authorId);
                    ShareChannelType type = ShareChannelType.ShareChannelNone;
                    if (channel == ChatType.World)
                    {
                        type = ShareChannelType.ShareChannelWorld;
                    }
                    else if (channel == ChatType.Team)
                    {
                        type = ShareChannelType.ShareChannelTeam;
                    }
                    else if (channel == ChatType.Guild)
                    {
                        type = ShareChannelType.ShareChannelFamily;
                    }
            
                }
            }

            //ChatContent entry = new ChatContent();            
            ChatContent entry = PoolManager.Fetch<ChatContent>();
            entry.eChatType = channel;
            entry.mBaseInfo = chatBaseInfo;
            entry.bHasIllegalWord = hasIllegalWord;     //文本有敏感词
            entry.bPlayed = false;
            entry.sContent = noNameContent;             //不带发送者名字的内容
            entry.sSimplifyUIContent = noNameContent;   //精简聊天框中不带 发送者名字的内容
            entry.mChatExtData = chatExtMsgItem;
            //entry.nTimePoint = TimeManager.GetServerTime();
            if (chatExtMsgVoice != null)
            {
                entry.sFileID = chatExtMsgVoice.FileID.ToStringUtf8();
                entry.fDuration = chatExtMsgVoice.Duration;
                //文本或者语音由敏感词 都标记为有敏感词
                entry.bHasIllegalWord |= (chatExtMsgVoice.HitFlag != 0); //语音有敏感词
            }
            else
            {
                entry.sFileID = null;
                entry.fDuration = 0;
            }

            //构建最终显示的带发送者名字前缀的内容，如果有需求的话
            bool needNamePrefix = false;
            if (chatBaseInfo != null && !string.IsNullOrWhiteSpace(chatBaseInfo.sSenderName))
            {
                if (channel == ChatType.Horn)
                {
                    needNamePrefix = true;
                }
                else if (chatBaseInfo.eActorType == EFightActorType.Monster
                    || chatBaseInfo.eActorType == EFightActorType.Partner
                    || chatBaseInfo.eActorType == EFightActorType.Pet)
                {
                    needNamePrefix = true;
                }
            }
            //设置正式聊天框中的内容
            if (needNamePrefix)
            {                
                entry.sUIContent = EmojiTextHelper.AppendPlayerName(noNameContent, chatBaseInfo.sSenderName, chatBaseInfo.nRoleID);
            }
            else
            {
                entry.sUIContent = noNameContent;
            }

            #endregion 构建单条聊天数据
            //如果需要UID索引的则加入
            if (messageProcess.HasFlag(EMessageProcess.AddUID))
            {
                entry.uid = ++currentUID;
                mChatContentDic.Add(entry.uid, entry);
            }
            else
            {
                entry.uid = 0;
            }

            //加入相应频道列表
            ChatChannelData chatChannelData = null;
            if (mData.TryGetValue(channel, out chatChannelData))
            {
                //chatChannelData.Add(entry);
                AddToChannel(chatChannelData, entry);

                if (!(channel == ChatType.System || channel == ChatType.Person || channel == ChatType.Horn))
                {
                    uint currentTime = TimeManager.GetServerTime();

                    if (currentTime >= chatChannelData.LastReceiveMessageTime + chatChannelData.ReceiveMessageMaxInterval)
                    {
                        entry.nTimePoint = currentTime;
                        chatChannelData.LastReceiveMessageTime = currentTime;
                    }
                    else
                    {
                        entry.nTimePoint = 0;
                    }
                }
            }

            //个人的和喇叭的在系统里面再存一份
            if (channel == ChatType.Person || channel == ChatType.Horn || channel == ChatType.Notice)
            {
                if (mData.TryGetValue(ChatType.System, out chatChannelData))
                {
                    //chatChannelData.Add(entry);
                    AddToChannel(chatChannelData, entry);
                }
            }

            //判断并添加至广播
            if (channel == ChatType.Horn || channel == ChatType.Notice)
            {
                //添加到广播队列
                mChatHUD.Enqueue(entry);
                AddChatContentReference(entry, EChatContentReference.HornHUD);
            }

            //判断并添加精简聊天面板中显示的内容
            if (!messageProcess.HasFlag(EMessageProcess.IgnoreSimplify) && GetSimplifyDisplayActive(channel))
            {
                //mSimplifyDisplay.Add(entry);
                AddToChannel(mSimplifyDisplay, entry);
                eventEmitter.Trigger<ChatContent>(EEvents.SimplifyMessageAdd, entry);
            }

            //当需要添加到聊天窗口的时候 发送聊天窗口内容变更信息
            eventEmitter.Trigger<ChatType>(EEvents.MessageAdd, channel);

            //添加入自动播放语音列表
            EnqueueVoiceContent(entry);

            //添加气泡
            if (chatBaseInfo != null)
            {
                Sys_HUD.Instance.CreatePlayerChatBubble(chatBaseInfo.eActorType, chatBaseInfo.nRoleID, entry.sContent, channel);
            }

            return entry.uid;
        }

        public void AddToChannel(ChatChannelData chatChannelData, ChatContent content)
        {
            if (chatChannelData.IsFull())
            {
                ChatContent removeContent = chatChannelData.RemoveTop();
                RemoveChatContentReference(removeContent, chatChannelData.eChatContentReference);
            }

            chatChannelData.AddLast(content);
            AddChatContentReference(content, chatChannelData.eChatContentReference);
        }

        public void AddChatContentReference(ChatContent content, EChatContentReference chatContentReference)
        {
            content.AddReference(chatContentReference);
        }

        public void RemoveChatContentReference(ChatContent content, EChatContentReference chatContentReference)
        {
            if (content.RemoveReference(chatContentReference) == EChatContentReference.None)
            {
                mChatContentDic.Remove(content.uid);
                PoolManager.Recycle(content);
            }
        }

        public void SetChatContent(ulong uid, string content)
        {
            if (mChatContentDic.TryGetValue(uid, out ChatContent chatContent))
            {
                //目前用不到其他的
                string noNameContent = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), null, content);
                chatContent.sContent = noNameContent;
                chatContent.sUIContent = noNameContent;
                chatContent.sSimplifyUIContent = noNameContent;

                eventEmitter.Trigger<ulong>(EEvents.MessageContentChange, uid);
            }
        }

        public ChatChannelData GetChatChannelData(ChatType type)
        {
            mData.TryGetValue(type, out ChatChannelData chatChannelData);
            return chatChannelData;
        }

        public void RecodeInputCache(InputCache cache)
        {
            mInputCacheRecord.RecodeInputCache(cache);
            //TODO 判断是否发送变更消息
            Sys_Chat.Instance.eventEmitter.Trigger(Sys_Chat.EEvents.RecodeChange);
        }

        //public Dictionary<ulong, Packet.ItemCommonData> recItems = new Dictionary<ulong, Packet.ItemCommonData>();

        public void SelectedHorn(uint id)
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(id);
            SelectedHorn(itemData);
        }

        public void SelectedHorn(CSVItem.Data data)
        {
            if (data == null
                || data.id == nCurrentSelectedHorn)
                return;

            switch (data.type_id)
            {
                case (uint)EItemType.FullServerHorn:
                    {
                        nCurrentSelectedHorn = nLastSelectedFullServerHorn = data.id;
                        nCurrentSelectedHornType = data.type_id;
                        eventEmitter.Trigger(EEvents.HornSelectChange);
                    }
                    break;
                case (uint)EItemType.SingleServerHorn:
                    {
                        nCurrentSelectedHorn = nLastSelectedSingleServerHorn = data.id;
                        nCurrentSelectedHornType = data.type_id;
                        eventEmitter.Trigger(EEvents.HornSelectChange);
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetSimplifyDisplayActive(ChatType chatType, bool active)
        {
            int simplifyDisplayFlag = OptionManager.Instance.GetInt(OptionManager.EOptionID.ChatSimplifyDisplayFlag);
            if (active)
            {
                simplifyDisplayFlag |= (1 << (int)chatType);
            }
            else
            {
                simplifyDisplayFlag &= (~(1 << (int)chatType));
            }

            OptionManager.Instance.SetInt(OptionManager.EOptionID.ChatSimplifyDisplayFlag, simplifyDisplayFlag, false);

            //PlayerPrefs.SetInt(kSimplifyDisplayActive, simplifyDisplayFlag);
        }

        public bool GetSimplifyDisplayActive(ChatType chatType)
        {
            int simplifyDisplayFlag = OptionManager.Instance.GetInt(OptionManager.EOptionID.ChatSimplifyDisplayFlag);
            return (simplifyDisplayFlag & (1u << (int)chatType)) != 0u;
        }

        public void SetSystemChannelShow(ChatType systemChannelShow)
        {
            eSystemChannelShow = systemChannelShow;
            //PlayerPrefs.SetInt(kSystemChannelShow, (int)eSystemChannelShow);
        }

        public void PushErrorTip(int rlt)
        {
            if (rlt > 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007907, rlt.ToString()));
            }
            else
            {
                switch (rlt)
                {
                    case Chat_Error_ContentEmpty:
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007912));
                        break;

                    case Chat_Error_NotHasTeam:
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007913));
                        break;

                    case Chat_Error_NotHasGuild:
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007914));
                        break;

                    case Chat_Error_HornLack:
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000945));
                        break;

                    case Chat_SetVersion_HotFix:
                        Sys_Hint.Instance.PushContent_Normal("设置本地热更版本号成功");
                        break;

                    case Chat_Voice_ToShort:
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10891));// ("语音录制时间过短");
                        break;

                    case Chat_Lv_Short_World:
                        {
                            if (mData.TryGetValue(ChatType.World, out ChatChannelData data))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                            }
                        }
                        break;

                    case Chat_Lv_Short_Local:
                        {
                            if (mData.TryGetValue(ChatType.Local, out ChatChannelData data))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                            }
                        }
                        break;

                    case Chat_Lv_Short_Guild:
                        {
                            if (mData.TryGetValue(ChatType.Guild, out ChatChannelData data))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                            }
                        }
                        break;

                    case Chat_Lv_Short_Team:
                        {
                            if (mData.TryGetValue(ChatType.Team, out ChatChannelData data))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                            }
                        }
                        break;

                    case Chat_Lv_Short_Horn:
                        {
                            if (mData.TryGetValue(ChatType.Horn, out ChatChannelData data))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                            }                                
                        }
                        break;

                    case Chat_Lv_Short_Career:
                        {
                            if (mData.TryGetValue(ChatType.Career, out ChatChannelData data))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                            }
                        }
                        break;

                    case Chat_Lv_Short_BraveGroup:
                        {
                            if (mData.TryGetValue(ChatType.BraveGroup, out ChatChannelData data))
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10984, data.nLvLimit.ToString()));// 角色等级需到达{0}级，可使用该功能
                            }
                        }
                        break;

                    case Chat_RealName_Error:
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12459));//没有通过实名认证
                            //SDKManager.GetRealNameWebRequest();
                        }
                        break;

                    case Chat_HasIllegalWord_Error:
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11834));// 该消息存在敏感词,无法播放
                        }
                        break;

                    case Chat_Count_Up_Limit:
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2007908));// 输入文字过长
                        }
                        break;

                    case Chat_Error_NotHasBraveGroup:
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13501));//没有勇者团
                        }
                        break;

                    default:
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(12460, rlt.ToString()));//没有通过实名认证
                        }
                        break;
                }
            }
        }


#if DEBUG_MODE
        public string gmContent = string.Empty;
#endif
        private void OnReceivedGM(NetMsg msg)
        {
            CmdGmRes res = NetMsgUtil.Deserialize<CmdGmRes>(CmdGmRes.Parser, msg);
            string retStr = string.Empty;
            if (res.Ret == 0)
            {
                retStr = res.Result.ToStringUtf8();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000972));
            }
            else
            {
                retStr = res.Ret.ToString();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000971, retStr));
            }

#if DEBUG_MODE
            gmContent = retStr;
            //gmContent = gmContent.Replace("{", "\\{").Replace("}", "\\}");
            Debug.LogFormat(string.Format("<color=#ff0000>{0}:{1}</color>", "GM", res.Ret));
            Debug.Log(gmContent);
#endif

            SetGMTypeFunction(res.Type);
        }

        public void SetGMTypeFunction(uint type)
        {
            switch (type)
            {
                case 1: //客户端打开全部功能
                    {
                        Sys_FunctionOpen.Instance.OnUnlockAllFunctionOpen(false);
                    }
                    break;
                case 2: //客户端设置引导开关
                    {
                        Sys_Role.Instance.RoleClientStateReq((uint)Sys_Role.EClientState.Guide, Sys_Guide.Instance.isUseGuide);
                    }
                    break;
            }
        }

        public ChatChannelData GetChannelData(ChatType channelType)
        {
            mData.TryGetValue(ChatType.Horn, out ChatChannelData data);
            return data;
        }               

#if GM_PROPAGATE_VERSION && UNITY_STANDALONE_WIN
        public bool isActionHideUI = false;
        private bool Gm_PropagateVersion(string content)
        {
            //仅用于港台宣发版本
            if (content.Length <= Sys_Chat.Chat_GM_Header.Length + 1)
                return false;
            string str = content.Substring(Sys_Chat.Chat_GM_Header.Length + 1);
            if (!string.IsNullOrEmpty(str))
            {
                if (str == "feelview")
                {
                    GameCenter.mCameraController.isCanRightJoystick = true;
                    return true;
                }
                else if (string.Equals(str, "hideui"))
                {
                    //UIManager.CloseUI(EUIID.UI_MainInterface);
                    UIManager.CloseUI(EUIID.UI_Chat);
                    UIManager.CloseUI(EUIID.UI_Menu,false,false);
                    isActionHideUI = true;
                    Sys_Input.Instance.SetEnableJoystick();
                    UIManager.CloseUI(EUIID.UI_ChatSimplify);
                    return true;
                }
            }
            return false;
        }
#endif
    }
}