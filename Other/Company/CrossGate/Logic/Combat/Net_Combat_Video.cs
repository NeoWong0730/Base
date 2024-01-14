using Google.Protobuf;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic;
using Net;
using Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class Net_Combat
{
    public enum VideoProtoFlag
    {
        VideoRoundBrief_Flag = 1,
        VideoPlayDetail_Flag = 2,
    }

    public class CombatVideoCatalogRecordInfo : BasePoolClass
    {
        public ulong m_VideoId;
        public uint m_RoundCount;

        public override void Clear()
        {
            m_VideoId = 0ul;
            m_RoundCount = 0u;
        }
    }

    public class VideoProtoInfo : BasePoolClass
    {
        public ulong m_VideoId;
        public VideoRoundBrief m_VideoRoundBrief;
        public List<VideoPlayDetail> m_VideoPlayDetailList;
        public float m_CacheStartTime;
        public uint m_UseCount;
        public bool m_IsLocation;

        public bool CheckExpire()
        {
            if (Time.time - m_CacheStartTime > 300f && m_UseCount < 2)
                return true;

            return false;
        }

        public override void Clear()
        {
            m_VideoId = 0ul;
            m_VideoRoundBrief = null;
            if (m_VideoPlayDetailList != null)
                m_VideoPlayDetailList.Clear();
            m_CacheStartTime = 0f;
            m_UseCount = 0u;
            m_IsLocation = false;
        }
    }

    public class CombatVideoMsg : BasePoolClass
    {
        public ulong m_VideoId;
        public uint m_RoundMax;
        public CmdBattleStartNtf m_CmdBattleStartNtf;
        public List<RoundVideoMsg> m_RoundVideoMsgList;
        public CmdBattleEndNtf m_CmdBattleEndNtf;

        public override void Clear()
        {
            m_VideoId = 0ul;
            m_RoundMax = 0u;
            m_CmdBattleStartNtf = null;
            if (m_RoundVideoMsgList != null)
            {
                for (int i = 0, count = m_RoundVideoMsgList.Count; i < count; i++)
                {
                    RoundVideoMsg rvml = m_RoundVideoMsgList[i];
                    if (rvml == null)
                        continue;

                    rvml.Push();
                }
                m_RoundVideoMsgList.Clear();
            }
            m_CmdBattleEndNtf = null;
        }
    }

    public class RoundVideoMsg : BasePoolClass
    {
        public uint m_RoundId;
        public CmdBattleRoundStartNtf m_CmdBattleRoundStartNtf;
        public CmdBattleRoundNtf m_CmdBattleRoundNtf;
        public CmdBattleShowRoundEndNtf m_CmdBattleShowRoundEndNtf;

        //弹幕信息
        //public List<uint> m_BulletTimeStamp;
        //public List<ByteString> m_BulletContext;
        public RepeatedField<ServerBullet> m_ServerBullets;

        public override void Clear()
        {
            m_RoundId = 0u;
            m_CmdBattleRoundStartNtf = null;
            m_CmdBattleRoundNtf = null;
            m_CmdBattleShowRoundEndNtf = null;

            m_ServerBullets = null;
        }

        public bool IsInvalid()
        {
            return m_CmdBattleRoundStartNtf == null &&
                        m_CmdBattleRoundNtf == null &&
                        m_CmdBattleShowRoundEndNtf == null;
        }
    }

    public class CacheMobRoundBeginState_Video : BasePoolClass
    {
        public GameObject m_Go;
        public AnimationComponent m_AnimationComponent;
        public BattleUnit m_Unit;
        public uint m_WeaponId;
        public uint m_MaxHp;
        public int m_CurHp;
        public uint m_MaxMp;
        public int m_CurMp;
        public Vector3 m_CurPos;
        public Vector3 m_CurAngle;
        public List<MobBuffComponent.BuffData> m_BuffList;

        public override void Clear()
        {
            m_Go = null;
            m_AnimationComponent = null;
            m_Unit = null;
            m_WeaponId = 0u;

            m_MaxHp = 0u;
            m_CurHp = 0;
            m_MaxMp = 0u;
            m_CurMp = 0;

            if (m_BuffList != null)
            {
                for (int i = 0, count = m_BuffList.Count; i < count; i++)
                {
                    MobBuffComponent.BuffData.Push(m_BuffList[i], false);
                }
                m_BuffList.Clear();
            }
        }
    }

    public class CacheRoundStateInfo_Video : BasePoolClass
    {
        public uint m_RoundId;
        public List<CacheMobRoundBeginState_Video> m_CacheMobRoundBeginStateList;

        public override void Clear()
        {
            m_RoundId = 0u;

            if (m_CacheMobRoundBeginStateList != null)
            {
                for (int i = 0, count = m_CacheMobRoundBeginStateList.Count; i < count; i++)
                {
                    m_CacheMobRoundBeginStateList[i]?.Push();
                }
                m_CacheMobRoundBeginStateList.Clear();
            }
        }
    }

    public bool m_IsVideo;

    public ClientVideo m_CurClientVideoIntroduceData;

    private static readonly int _maxVideoRecordCount = 5;
    private static readonly int _maxCacheVideoProtoInfoCount = 10;
    private static readonly string _combatVideoCatalogRecordPlayerPrefs = "CombatVideoCatalogRecordInfoPlayerPrefs";
    private List<CombatVideoCatalogRecordInfo> _combatVideoCatalogRecordList = new List<CombatVideoCatalogRecordInfo>();

    private Dictionary<ushort, MessageParser> _flagMessageParserDic = new Dictionary<ushort, MessageParser>();
    private List<VideoProtoInfo> _videoProtoInfoList = new List<VideoProtoInfo>();
    
    private List<CombatVideoMsg> _combatVideoMsgList = new List<CombatVideoMsg>();
    private CombatVideoMsg _curCombatVideoMsg;
    private ulong _curAutherId;
    private ulong _curVideoId;
    private uint _curPlayRoundId;
    private uint _sendPlayRoundId;

    private uint _needBulletDataOfRoundId;

    private float _isRecordStartAlreadyEnterVideoTime;

    public bool m_IsPauseVideo;
    public bool m_IsNeedPlayNextVideo;

    private List<CacheRoundStateInfo_Video> _cacheRoundStateInfo_VideoList = new List<CacheRoundStateInfo_Video>();

    #region 生命周期
    private void OnAwakeCombatVideo()
    {
        m_IsVideo = false;

        EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.VideoShowDetailReq, (ushort)CmdVideo.VideoShowDetailRes, OnVideoShowDetailRes, CmdVideoVideoShowDetailRes.Parser);
        EventDispatcher.Instance.AddEventListener((ushort)CmdVideo.VideoPlayDetailReq, (ushort)CmdVideo.VideoPlayDetailRes, OnVideoPlayDetailRes, CmdVideoVideoPlayDetailRes.Parser);

        SetLocationCombatVideoCatalog();

        _flagMessageParserDic[(ushort)VideoProtoFlag.VideoRoundBrief_Flag] = VideoRoundBrief.Parser;
        _flagMessageParserDic[(ushort)VideoProtoFlag.VideoPlayDetail_Flag] = VideoPlayDetail.Parser;
    }

    private void OnEnableCombatVideo()
    {
        if (m_IsVideo)
        {
            Sys_Video.Instance.PlayVideoReq(Sys_Role.Instance.RoleId, _curVideoId, _curAutherId);
        }
    }

    private void OnDisableCombatVideo()
    {
        m_IsVideo = false;
        OnSaveVideoDataToLocation(true, _curVideoId,
            m_CurClientVideoIntroduceData == null || m_CurClientVideoIntroduceData.baseBrif == null ? 0u : m_CurClientVideoIntroduceData.baseBrif.MaxRound);

        for (int combatVideoIndex = 0, combatVideoCount = _combatVideoMsgList.Count; combatVideoIndex < combatVideoCount; combatVideoIndex++)
        {
            CombatVideoMsg combatVideoMsg = _combatVideoMsgList[combatVideoIndex];
            if (combatVideoMsg == null)
                continue;

            combatVideoMsg.Push();
        }
        _combatVideoMsgList.Clear();

        _curCombatVideoMsg = null;
        _curAutherId = 0ul;
        _curVideoId = 0ul;
        _curPlayRoundId = 0u;
        _sendPlayRoundId = 0u;

        _needBulletDataOfRoundId = 0u;

        m_IsPauseVideo = false;
        m_IsNeedPlayNextVideo = false;

        m_CurClientVideoIntroduceData = null;

        ClearAllCacheRoundInfo_Video();
    }

    private void OnDestroyCombatVideo()
    {
        m_IsVideo = false;

        m_IsPauseVideo = false;
        m_IsNeedPlayNextVideo = false;

        EventDispatcher.Instance.RemoveEventListener((ushort)CmdVideo.VideoShowDetailRes, OnVideoShowDetailRes);
        EventDispatcher.Instance.RemoveEventListener((ushort)CmdVideo.VideoPlayDetailRes, OnVideoPlayDetailRes);
    }
    #endregion

    #region Logic
    private void ClearAllCacheRoundInfo_Video()
    {
        foreach (var info in _cacheRoundStateInfo_VideoList)
        {
            info.Push();
        }
        _cacheRoundStateInfo_VideoList.Clear();
    }

    private CacheRoundStateInfo_Video GetCacheRoundInfo_Video(uint roundId)
    {
        for (int i = 0, count = _cacheRoundStateInfo_VideoList.Count; i < count; i++)
        {
            CacheRoundStateInfo_Video info = _cacheRoundStateInfo_VideoList[i];
            if (info == null)
                continue;

            if (info.m_RoundId == roundId)
                return info;
        }

        return null;
    }

    private void CacheMobRoundStatesInVideo(uint roundId)
    {
        CacheRoundStateInfo_Video info = GetCacheRoundInfo_Video(roundId);
        if (info != null)
            return;

        info = BasePoolClass.Get<CacheRoundStateInfo_Video>();
        info.m_RoundId = roundId;

        _cacheRoundStateInfo_VideoList.Add(info);
        
        foreach (var kv in MobManager.Instance.m_MobDic)
        {
            MobEntity mobEntity = kv.Value;
            if (mobEntity == null)
                continue;

            MobCombatComponent mobCombatComponent = mobEntity.m_MobCombatComponent;
            if (mobCombatComponent == null)
                continue;

            CacheMobRoundBeginState_Video cacheMobRoundBeginState_Video = BasePoolClass.Get<CacheMobRoundBeginState_Video>();
            cacheMobRoundBeginState_Video.m_Go = mobEntity.m_Go;
            cacheMobRoundBeginState_Video.m_AnimationComponent = mobCombatComponent.m_AnimationComponent;
            cacheMobRoundBeginState_Video.m_Unit = mobCombatComponent.m_BattleUnit;
            cacheMobRoundBeginState_Video.m_WeaponId = mobCombatComponent.m_WeaponId;
            cacheMobRoundBeginState_Video.m_MaxHp = mobCombatComponent.m_BattleUnit.MaxHp;
            cacheMobRoundBeginState_Video.m_CurHp = mobCombatComponent.m_BattleUnit.CurHp;
            cacheMobRoundBeginState_Video.m_MaxMp = mobCombatComponent.m_BattleUnit.MaxMp;
            cacheMobRoundBeginState_Video.m_CurMp = mobCombatComponent.m_BattleUnit.CurMp;
            cacheMobRoundBeginState_Video.m_CurPos = mobEntity.m_Trans.position;
            cacheMobRoundBeginState_Video.m_CurAngle = mobEntity.m_Trans.eulerAngles;

            MobBuffComponent mobBuffComponent = mobEntity.GetComponent<MobBuffComponent>();
            if (mobBuffComponent != null && mobBuffComponent.m_Buffs != null)
            {
                for (int buffIndex = 0, buffCount = mobBuffComponent.m_Buffs.Count; buffIndex < buffCount; buffIndex++)
                {
                    var buffData = mobBuffComponent.m_Buffs[buffIndex];
                    if (buffData == null)
                        continue;

                    if (cacheMobRoundBeginState_Video.m_BuffList == null)
                        cacheMobRoundBeginState_Video.m_BuffList = new List<MobBuffComponent.BuffData>();

                    cacheMobRoundBeginState_Video.m_BuffList.Add(buffData.Copy());
                }
            }

            if (info.m_CacheMobRoundBeginStateList == null)
                info.m_CacheMobRoundBeginStateList = new List<CacheMobRoundBeginState_Video>();
            info.m_CacheMobRoundBeginStateList.Add(cacheMobRoundBeginState_Video);
        }
    }

    private void RestoreMobRoundStatesInVideo(uint preRoundId, uint curRoundId)
    {
        if (curRoundId > preRoundId)
            return;

        CacheRoundStateInfo_Video info = GetCacheRoundInfo_Video(curRoundId);
        if (info == null || 
            info.m_CacheMobRoundBeginStateList == null || 
            info.m_CacheMobRoundBeginStateList.Count < 1)
        {
            DLogManager.LogDebugError($"RestoreMobRoundStatesInVideo从preRoundId：{preRoundId}到curRoundId：{curRoundId}没有缓存第{curRoundId}回合的状态数据");
            return;
        }

        int mobRoundBeginStateCount = info.m_CacheMobRoundBeginStateList.Count;
        if (mobRoundBeginStateCount > 0)
        {
            BulletManager.Instance.Dispose();

            MobManager.Instance.ClearAll();

            ClearDoExcuteTurnData();

            for (int mobRoundStateBeginIndex = 0; mobRoundStateBeginIndex < mobRoundBeginStateCount; mobRoundStateBeginIndex++)
            {
                CacheMobRoundBeginState_Video mobRoundBeginState = info.m_CacheMobRoundBeginStateList[mobRoundStateBeginIndex];
                if (mobRoundBeginState == null ||
                    mobRoundBeginState.m_Go == null ||
                    mobRoundBeginState.m_AnimationComponent == null ||
                    mobRoundBeginState.m_Unit == null)
                    continue;

                mobRoundBeginState.m_Unit.MaxHp = mobRoundBeginState.m_MaxHp;
                mobRoundBeginState.m_Unit.CurHp = mobRoundBeginState.m_CurHp;
                mobRoundBeginState.m_Unit.MaxMp = mobRoundBeginState.m_MaxMp;
                mobRoundBeginState.m_Unit.CurMp = mobRoundBeginState.m_CurMp;

                MobEntity mobEntity = EntityFactory.Create<MobEntity>();
                MobManager.Instance.m_MobDic[mobRoundBeginState.m_Unit.UnitId] = mobEntity;

                mobEntity.Init(mobRoundBeginState.m_Unit, mobRoundBeginState.m_AnimationComponent, mobRoundBeginState.m_Go,
                    mobRoundBeginState.m_WeaponId, false);

                mobEntity.m_MobCombatComponent.m_OriginPos = mobRoundBeginState.m_CurPos;
                mobEntity.m_MobCombatComponent.m_eulerAngles = mobRoundBeginState.m_CurAngle;
                mobEntity.m_MobCombatComponent.ResetTrans(true);

                if (mobRoundBeginState.m_BuffList != null)
                {
                    int buffCount = mobRoundBeginState.m_BuffList.Count;
                    if (buffCount > 0)
                    {
                        MobBuffComponent mobBuffComponent = mobEntity.GetNeedComponent<MobBuffComponent>();

                        for (int buffIndex = 0; buffIndex < buffCount; buffIndex++)
                        {
                            var buffData = mobRoundBeginState.m_BuffList[buffIndex];
                            if (buffData == null)
                                continue;

                            mobBuffComponent.UpdateBuff(buffData.m_BuffTb.id, buffData.m_Count, buffData.m_Overlay, 0u, 0u, 0u, 0u, 0u, 0u);
                        }
                    }
                }
            }

            MobManager.Instance.ResetAllMobState();
        }
    }
    #endregion

    #region Net
    private void OnVideoShowDetailRes(NetMsg msg)
    {
        CmdVideoVideoShowDetailRes res = NetMsgUtil.Deserialize<CmdVideoVideoShowDetailRes>(CmdVideoVideoShowDetailRes.Parser, msg);

        DLogManager.Log(ELogType.eCombat, $"CmdVideoVideoShowDetailRes");

        if (res.DetailList == null || res.DetailList.List.Count < 1)
            return;

        for (int i = 0, count = res.DetailList.List.Count; i < count; i++)
        {
            VideoShowDetailInfo videoShowDetailInfo = res.DetailList.List[i];
            if (videoShowDetailInfo == null)
                continue;

            if (videoShowDetailInfo.VideoId == _curVideoId)
            {
                ParseVideoRoundBrief(_curVideoId, 
                    m_CurClientVideoIntroduceData == null ? 0u : m_CurClientVideoIntroduceData.baseBrif.MaxRound, 
                    videoShowDetailInfo.Brief, true);
            }
        }

        EnterVideoCombat(_curVideoId, m_CurClientVideoIntroduceData);
    }

    private void SendVideoShowDetailReq(ulong videoId)
    {
        if (m_CurClientVideoIntroduceData == null)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000013));
           // DebugUtil.LogError($"SendVideoShowDetailReq---videoId:{videoId.ToString()}的时候缓存的ClientVideo为null");
            return;
        }

        CmdVideoVideoShowDetailReq req = new CmdVideoVideoShowDetailReq();
        req.InfoList = new VideoUniqueInfoList();

        VideoUniqueInfo videoUniqueInfo = new VideoUniqueInfo();
        videoUniqueInfo.VideoId = videoId;
        videoUniqueInfo.AuthorId = m_CurClientVideoIntroduceData.authorBrif.Author;

        req.InfoList.List.Add(videoUniqueInfo);

        DLogManager.Log(ELogType.eCombat, $"SendVideoShowDetailReq---videoId:{videoId.ToString()}");

        NetClient.Instance.SendMessage((ushort)CmdVideo.VideoShowDetailReq, req);
    }

    private void OnVideoPlayDetailRes(NetMsg msg)
    {
        CmdVideoVideoPlayDetailRes res = NetMsgUtil.Deserialize<CmdVideoVideoPlayDetailRes>(CmdVideoVideoPlayDetailRes.Parser, msg);

        DLogManager.Log(ELogType.eCombat, $"CmdVideoVideoPlayDetailRes");

        if (res.DetailList == null || res.DetailList.List.Count < 1)
            return;

        for (int i = 0, count = res.DetailList.List.Count; i < count; i++)
        {
            VideoPlayDetailInfo videoPlayDetailInfo = res.DetailList.List[i];
            if (videoPlayDetailInfo == null)
                continue;

            if (videoPlayDetailInfo.VideoId == _curVideoId)
            {
                ParseVideoPlayDetail(_curVideoId,
                    m_CurClientVideoIntroduceData == null ? 0u : m_CurClientVideoIntroduceData.baseBrif.MaxRound, 
                    videoPlayDetailInfo.Play, true, false);
            }
        }

        PlayRoundVideo(_sendPlayRoundId);
    }

    private void SendVideoPlayDetailReq(ulong videoId, enPlayDetailType playDetailType)
    {
        if (m_CurClientVideoIntroduceData == null)
        {
            DebugUtil.LogError($"SendVideoPlayDetailReq---videoId:{videoId.ToString()}的时候缓存的ClientVideo为null");
            return;
        }

        CmdVideoVideoPlayDetailReq req = new CmdVideoVideoPlayDetailReq();
        req.InfoList = new VideoUniqueInfoList();

        VideoUniqueInfo videoUniqueInfo = new VideoUniqueInfo();
        videoUniqueInfo.VideoId = videoId;
        videoUniqueInfo.AuthorId = m_CurClientVideoIntroduceData.authorBrif.Author;

        req.InfoList.List.Add(videoUniqueInfo);

        req.Type = playDetailType;

        DLogManager.Log(ELogType.eCombat, $"SendVideoPlayDetailReq---videoId:{videoId.ToString()}");

        NetClient.Instance.SendMessage((ushort)CmdVideo.VideoPlayDetailReq, req);
    }
    #endregion

    #region 获取录像数据
    public bool TryLocation_CombatVideoMsg(ulong videoId, ref CombatVideoMsg combatVideoMsg, bool useRoundData, uint roundId, out RoundVideoMsg roundVideoMsg)
    {
        if (combatVideoMsg != null && combatVideoMsg.m_VideoId != videoId)
        {
            DebugUtil.LogError($"TryLocation_CombatVideoMsg  combatVideoMsg.m_VideoId:{combatVideoMsg.m_VideoId.ToString()}和请求的videoId:{videoId.ToString()}不相等");
            combatVideoMsg = null;
        }

        if (combatVideoMsg == null)
            combatVideoMsg = GetCombatVideoMsg(videoId, false);
        
        roundVideoMsg = null;
        if (useRoundData && combatVideoMsg != null)
            roundVideoMsg = GetRoundVideoMsg(combatVideoMsg, videoId, roundId, false, false);
        
        if (combatVideoMsg == null || combatVideoMsg.m_CmdBattleStartNtf == null || 
            (useRoundData && (roundVideoMsg == null || roundVideoMsg.IsInvalid())))
        {
            return false;
        }
        
        if (combatVideoMsg == null)
        {
            DebugUtil.Log(ELogType.eCombat, $"录像Id：{videoId.ToString()}在缓存中没有combatVideoMsg数据");
            return false;
        }
        if (combatVideoMsg.m_CmdBattleStartNtf == null)
        {
            DebugUtil.Log(ELogType.eCombat, $"录像Id：{videoId.ToString()}在缓存中没有combatVideoMsg.m_CmdBattleStartNtf数据");
            return false;
        }
        if (useRoundData && roundVideoMsg == null)
        {
            DebugUtil.Log(ELogType.eCombat, $"录像Id：{videoId.ToString()}在缓存中没有roundId:{roundId.ToString()}的roundVideoMsg数据");
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// 返回：0=没有获取到数据，1=已经缓存了数据，2=刚从本地获取数据, 3=刚new的数据
    /// </summary>
    public int CheckVideoProtoInfo(ulong videoId, uint serverRoundMax, bool isNeedNotNull, out VideoProtoInfo videoProtoInfo)
    {
        int infoState = 0;
        videoProtoInfo = null;
        for (int i = 0, count = _videoProtoInfoList.Count; i < count; i++)
        {
            VideoProtoInfo vpi = _videoProtoInfoList[i];
            if (vpi.m_VideoId == videoId)
            {
                videoProtoInfo = vpi;
                if (videoProtoInfo != null)
                    infoState = 1;
                break;
            }
        }

        if (videoProtoInfo == null)
        {
            for (int i = 0, count = _combatVideoCatalogRecordList.Count; i < count; i++)
            {
                CombatVideoCatalogRecordInfo recordInfo = _combatVideoCatalogRecordList[i];
                if (recordInfo == null)
                    continue;

                if (recordInfo.m_VideoId == videoId)
                {
                    if (recordInfo.m_RoundCount >= serverRoundMax)
                    {
                        videoProtoInfo = GetVideoLocationInfo(videoId, serverRoundMax, recordInfo.m_RoundCount);
                        if (videoProtoInfo != null)
                            infoState = 2;
                    }
                    
                    break;
                }
            }
        }
        
        if (videoProtoInfo == null && isNeedNotNull)
        {
            videoProtoInfo = BasePoolClass.Get<VideoProtoInfo>();
            videoProtoInfo.m_VideoId = videoId;

            _videoProtoInfoList.Add(videoProtoInfo);

            infoState = 3;
        }

        if (videoProtoInfo != null)
        {
            videoProtoInfo.m_CacheStartTime = Time.time;
            videoProtoInfo.m_UseCount += 1;

            if (infoState == 0)
                infoState = int.MaxValue;
        }

        return infoState;
    }

    private CombatVideoMsg GetCombatVideoMsg(ulong videoId, bool isNeedNotNull = true)
    {
        CombatVideoMsg combatVideoMsg = null;
        for (int i = 0, count = _combatVideoMsgList.Count; i < count; i++)
        {
            CombatVideoMsg cvm = _combatVideoMsgList[i];
            if (cvm == null)
                continue;

            if (cvm.m_VideoId == videoId)
            {
                if (cvm.m_CmdBattleStartNtf == null)
                    DebugUtil.LogError($"GetCombatVideoMsg  videoId:{videoId.ToString()}  CombatVideoMsg没有m_CmdBattleStartNtf数据");

                combatVideoMsg = cvm;
                break;
            }
        }
        if (isNeedNotNull)
        {
            if (combatVideoMsg == null)
            {
                combatVideoMsg = BasePoolClass.Get<CombatVideoMsg>();
                combatVideoMsg.m_VideoId = videoId;

                _combatVideoMsgList.Add(combatVideoMsg);
            }
        }

        if (combatVideoMsg != null &&
            m_CurClientVideoIntroduceData != null &&
            m_CurClientVideoIntroduceData.baseBrif != null)
        {
            combatVideoMsg.m_RoundMax = m_CurClientVideoIntroduceData.baseBrif.MaxRound;
        }

        return combatVideoMsg;
    }

    private RoundVideoMsg GetRoundVideoMsg(CombatVideoMsg combatVideoMsg, ulong videoId, uint roundId, bool isNeedNotNull = true, 
        bool isCheckProtoNtf = true)
    {
        if (combatVideoMsg == null)
        {
            combatVideoMsg = GetCombatVideoMsg(videoId, isNeedNotNull);
            if (!isNeedNotNull && combatVideoMsg == null)
                return null;
        }

        RoundVideoMsg roundVideoMsg = null;
        if (combatVideoMsg.m_RoundVideoMsgList == null)
        {
            if (isNeedNotNull)
                combatVideoMsg.m_RoundVideoMsgList = new List<RoundVideoMsg>();
        }
        else
        {
            for (int rvmIndex = 0, rvmCount = combatVideoMsg.m_RoundVideoMsgList.Count; rvmIndex < rvmCount; rvmIndex++)
            {
                RoundVideoMsg rvm = combatVideoMsg.m_RoundVideoMsgList[rvmIndex];
                if (rvm == null)
                {
                    DebugUtil.LogError($"录像Id：{videoId.ToString()}  m_RoundVideoMsgList的Index:{rvmIndex.ToString()}数据为null");
                    combatVideoMsg.m_RoundVideoMsgList.RemoveAt(rvmIndex);
                    --rvmIndex;
                    --rvmCount;
                    continue;
                }

                if (rvm.m_RoundId == roundId)
                {
                    if (rvm.IsInvalid())
                    {
                        if (isCheckProtoNtf)
                            DebugUtil.LogError($"GetRoundVideoMsg  videoId:{videoId.ToString()}  roundId:{rvm.m_RoundId.ToString()}  没有获取到m_CmdBattleRoundStartNtf,m_CmdBattleRoundNtf,m_CmdBattleShowRoundEndNtf数据");
                    }

                    roundVideoMsg = rvm;

                    break;
                }
            }
        }

        if (isNeedNotNull)
        {
            if (roundVideoMsg == null)
            {
                roundVideoMsg = BasePoolClass.Get<RoundVideoMsg>();
                roundVideoMsg.m_RoundId = roundId;

                combatVideoMsg.m_RoundVideoMsgList.Add(roundVideoMsg);
            }
        }

        return roundVideoMsg;
    }
    #endregion

    #region 解析录像数据
    private void ParseVideoRoundBrief(ulong videoId, uint serverMaxRound, VideoRoundBrief videoRoundBrief, bool isSetCache)
    {
        if (videoRoundBrief == null || videoRoundBrief.Contexts == null ||
            videoRoundBrief.Contexts.Count <= 0)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000013));
           // DebugUtil.LogError($"ParseVideoRoundBrief  videoId:{videoId.ToString()}  VideoRoundBrief数据不对");
            return;
        }

        if (isSetCache)
        {
            VideoProtoInfo videoProtoInfo;
            CheckVideoProtoInfo(videoId, serverMaxRound, true, out videoProtoInfo);
            videoProtoInfo.m_IsLocation = false;
            videoProtoInfo.m_VideoRoundBrief = videoRoundBrief;
        }
        
        CombatVideoMsg combatVideoMsg = GetCombatVideoMsg(videoId);
        RoundVideoMsg roundVideoMsg = GetRoundVideoMsg(combatVideoMsg, videoId, 1u);

        for (int contextIndex = 0, contextCount = videoRoundBrief.Contexts.Count; contextIndex < contextCount; contextIndex++)
        {
            VideoFightProto videoFightProto = videoRoundBrief.Contexts[contextIndex];
            if (videoFightProto == null)
                continue;

            ParseVideoProtoMessage(combatVideoMsg, roundVideoMsg, (ushort)videoFightProto.MsgId, videoFightProto.Msg);
        }
    }

    private void ParseVideoPlayDetail(ulong videoId, uint serverMaxRound, VideoPlayDetail videoPlayDetail, bool isSetCache, bool isCheckProtoNtf = true)
    {
        if (videoId == 0ul || videoPlayDetail == null)
            return;

        if (isSetCache)
        {
            VideoProtoInfo videoProtoInfo;
            CheckVideoProtoInfo(videoId, serverMaxRound, true, out videoProtoInfo);
            if (videoProtoInfo.m_VideoPlayDetailList == null)
                videoProtoInfo.m_VideoPlayDetailList = new List<VideoPlayDetail>();
            videoProtoInfo.m_IsLocation = false;
            videoProtoInfo.m_VideoPlayDetailList.Add(videoPlayDetail);
        }

        CombatVideoMsg combatVideoMsg = GetCombatVideoMsg(videoId);

        bool isGetNeedBulletOfRoundId = false;
        //战斗弹幕
        if (videoPlayDetail.Bullet != null && videoPlayDetail.Bullet.Rounds != null &&
            videoPlayDetail.Bullet.Rounds.Count > 0)
        {
            for (int bulletIndex = 0, bulletCount = videoPlayDetail.Bullet.Rounds.Count; bulletIndex < bulletCount; bulletIndex++)
            {
                ServerBulletList serverBulletList = videoPlayDetail.Bullet.Rounds[bulletIndex];
                if (serverBulletList == null)
                    continue;

                RoundVideoMsg roundVideoMsg = GetRoundVideoMsg(combatVideoMsg, videoId, serverBulletList.RoundId, true, isCheckProtoNtf);
                if (roundVideoMsg != null)
                {
                    roundVideoMsg.m_ServerBullets = serverBulletList.Bullets;

                    if (serverBulletList.RoundId == _needBulletDataOfRoundId)
                        isGetNeedBulletOfRoundId = true;
                }
            }
        }

        //战斗演示
        if (videoPlayDetail.Round != null && videoPlayDetail.Round.Rounds != null &&
            videoPlayDetail.Round.Rounds.Count > 0)
        {
            for (int roundIndex = 0, roundCount = videoPlayDetail.Round.Rounds.Count; roundIndex < roundCount; roundIndex++)
            {
                VideoRoundContext videoRoundContext = videoPlayDetail.Round.Rounds[roundIndex];
                if (videoRoundContext == null)
                    continue;

                int contextCount = videoRoundContext.Contexts.Count;
                if (contextCount < 1)
                {
                    DebugUtil.LogError($"ParseVideoPlayDetail  videoId:{videoId.ToString()}  roundId:{videoRoundContext.RoundId.ToString()}   VideoRoundContext消息中没有Contexts内容");
                    continue;
                }

                RoundVideoMsg roundVideoMsg = GetRoundVideoMsg(combatVideoMsg, videoId, videoRoundContext.RoundId, true, isCheckProtoNtf);
                for (int contextIndex = 0; contextIndex < contextCount; contextIndex++)
                {
                    VideoFightProto videoFightProto = videoRoundContext.Contexts[contextIndex];
                    if (videoFightProto == null)
                        continue;

                    ParseVideoProtoMessage(combatVideoMsg, roundVideoMsg, (ushort)videoFightProto.MsgId, videoFightProto.Msg);
                }

                if (roundVideoMsg.IsInvalid())
                {
                    DebugUtil.LogError($"ParseVideoPlayDetail  videoId:{videoId.ToString()}  roundId:{roundVideoMsg.m_RoundId.ToString()}  没有获取到m_CmdBattleRoundStartNtf,m_CmdBattleRoundNtf,m_CmdBattleShowRoundEndNtf数据");
                }
            }
        }

        if (isGetNeedBulletOfRoundId)
        {
            _needBulletDataOfRoundId = 0u;
            Sys_Video.Instance.eventEmitter.Trigger(Sys_Video.EEvents.OnReceiveBullet);
        }
    }

    private void ParseVideoProtoMessage(CombatVideoMsg combatVideoMsg, RoundVideoMsg roundVideoMsg, ushort opcode, ByteString bs)
    {
        if (combatVideoMsg != null)
        {
            if (opcode == (ushort)CmdBattle.StartNtf)
            {
                NetMsgUtil.TryDeserialize(CmdBattleStartNtf.Parser, bs, out CmdBattleStartNtf ntf);
                if (ntf != null)
                {
                    DLogManager.Log(ELogType.eCombat, $"ParseVideoProtoMessage---获取proto数据StartNtf");

                    combatVideoMsg.m_CmdBattleStartNtf = ntf;
                }
            }
            else if (opcode == (ushort)CmdBattle.EndNtf)
            {
                NetMsgUtil.TryDeserialize(CmdBattleEndNtf.Parser, bs, out CmdBattleEndNtf ntf);
                if (ntf != null)
                {
                    DLogManager.Log(ELogType.eCombat, $"ParseVideoProtoMessage---获取proto数据EndNtf");

                    combatVideoMsg.m_CmdBattleEndNtf = ntf;
                }
            }
        }

        if (roundVideoMsg != null)
        {
            if (opcode == (ushort)CmdBattle.RoundStartNtf)
            {
                NetMsgUtil.TryDeserialize(CmdBattleRoundStartNtf.Parser, bs, out CmdBattleRoundStartNtf ntf);
                if (ntf != null)
                {
                    DLogManager.Log(ELogType.eCombat, $"ParseVideoProtoMessage---获取proto数据RoundStartNtf  ntf.CurRound:{ntf.CurRound.ToString()}");

                    roundVideoMsg.m_CmdBattleRoundStartNtf = ntf;
                    if (roundVideoMsg.m_RoundId != roundVideoMsg.m_CmdBattleRoundStartNtf.CurRound)
                    {
                        DebugUtil.LogError($"videoId:{(combatVideoMsg == null ? "" : $"{combatVideoMsg.m_VideoId.ToString()}")}   roundVideoMsg.m_RoundId:{roundVideoMsg.m_RoundId.ToString()}和m_CmdBattleRoundStartNtf.CurRound:{roundVideoMsg.m_CmdBattleRoundStartNtf.CurRound.ToString()}回合值不一样");
                    }
                }
            }
            else if (opcode == (ushort)CmdBattle.RoundNtf)
            {
                NetMsgUtil.TryDeserialize(CmdBattleRoundNtf.Parser, bs, out CmdBattleRoundNtf ntf);
                if (ntf != null)
                {
                    DLogManager.Log(ELogType.eCombat, $"ParseVideoProtoMessage---获取proto数据RoundNtf  ntf.CurRound:{ntf.CurRound.ToString()}");

                    roundVideoMsg.m_CmdBattleRoundNtf = ntf;
                    if (roundVideoMsg.m_RoundId != roundVideoMsg.m_CmdBattleRoundNtf.CurRound)
                    {
                        DebugUtil.LogError($"videoId:{(combatVideoMsg == null ? "" : $"{combatVideoMsg.m_VideoId.ToString()}")}   roundVideoMsg.m_RoundId:{roundVideoMsg.m_RoundId.ToString()}和m_CmdBattleRoundNtf.CurRound:{roundVideoMsg.m_CmdBattleRoundNtf.CurRound.ToString()}回合值不一样");
                    }
                }
            }
            else if (opcode == (ushort)CmdBattle.ShowRoundEndNtf)
            {
                NetMsgUtil.TryDeserialize(CmdBattleShowRoundEndNtf.Parser, bs, out CmdBattleShowRoundEndNtf ntf);
                if (ntf != null)
                {
                    DLogManager.Log(ELogType.eCombat, $"ParseVideoProtoMessage---获取proto数据ShowRoundEndNtf  ntf.CurRound:{ntf.CurRound.ToString()}");

                    roundVideoMsg.m_CmdBattleShowRoundEndNtf = ntf;
                    if (roundVideoMsg.m_RoundId != roundVideoMsg.m_CmdBattleShowRoundEndNtf.CurRound + 1)
                    {
                        DebugUtil.LogError($"videoId:{(combatVideoMsg == null ? "" : $"{combatVideoMsg.m_VideoId.ToString()}")}   roundVideoMsg.m_RoundId:{roundVideoMsg.m_RoundId.ToString()}和m_CmdBattleShowRoundEndNtf.CurRound:{roundVideoMsg.m_CmdBattleShowRoundEndNtf.CurRound.ToString()}回合值不一样");
                    }
                }
            }
        }
    }
    #endregion

    #region 使用录像数据
    public bool PlayVideoPreview(ulong videoId, ClientVideo clientVideo)
    {
        return EnterVideoCombat(videoId, clientVideo);
    }

    public bool EnterVideoCombat(ulong videoId, ClientVideo clientVideo)
    {
        _curCombatVideoMsg = null;
        m_CurClientVideoIntroduceData = clientVideo;

        bool isAlreadySend = _curVideoId > 0ul;

        //如果已经在录像中且请求的不是当前录像，先退出录像
        if (m_IsVideo && _curVideoId != videoId)
        {
            m_IsVideo = false;
            ExitVideo();
        }

        _curAutherId = m_CurClientVideoIntroduceData == null ? 0ul : m_CurClientVideoIntroduceData.authorBrif.Author;
        _curVideoId = videoId;

        _curPlayRoundId = 0u;
        _sendPlayRoundId = 0u;

        _needBulletDataOfRoundId = 0u;

        m_IsPauseVideo = false;
        m_IsNeedPlayNextVideo = false;

        //查询下缓存
        CheckVideoProtoInfo(videoId, m_CurClientVideoIntroduceData == null ? 0u : m_CurClientVideoIntroduceData.baseBrif.MaxRound, 
            false, out VideoProtoInfo videoProtoInfo);

        RoundVideoMsg roundVideoMsg;
        if (TryLocation_CombatVideoMsg(videoId, ref _curCombatVideoMsg, false, 0u, out roundVideoMsg))
        {
            //进行预览
            m_IsVideo = true;

            Sys_Video.Instance.eventEmitter.Trigger(Sys_Video.EEvents.OnPlayVideoSuccess);

            Sys_Fight.Instance.DoStartNtf(_curCombatVideoMsg.m_CmdBattleStartNtf);
            
            return true;
        }
        else
        {
            if (!isAlreadySend || _isRecordStartAlreadyEnterVideoTime < Time.time)
            {
                _isRecordStartAlreadyEnterVideoTime = Time.time + 3f;

                //向服务器请求进入战斗数据
                SendVideoShowDetailReq(videoId);
            }

            return false;
        }
    }

    public void PlayVideoPreRound()
    {
        if (_curPlayRoundId < 2u)
            return;

        PlayRoundVideo(_curPlayRoundId - 1u);
    }

    public void PlayVideoNextRound()
    {
        if (_curPlayRoundId + 1u > _curCombatVideoMsg.m_RoundVideoMsgList.Count)
            return;

        PlayRoundVideo(_curPlayRoundId + 1u);
    }

    public void PauseVideo()
    {
        m_IsPauseVideo = !m_IsPauseVideo;
        if (!m_IsPauseVideo && m_IsNeedPlayNextVideo)
        {
            m_IsNeedPlayNextVideo = false;
            PlayVideoNextRound();
        }
    }

    public void ExitVideo()
    {
        DoBattleResult(true);
    }
    
    public void PlayRoundVideo(uint roundId)
    {
        PlayRoundVideo(_curCombatVideoMsg, roundId);
    }

    private bool PlayRoundVideo(CombatVideoMsg curCombatVideoMsg, uint roundId)
    {
        if (curCombatVideoMsg == null)
        {
            DebugUtil.LogError($"PlayRoundVideo----没有当前录像的数据");
            return false;
        }
        
        RoundVideoMsg roundVideoMsg;
        if (TryLocation_CombatVideoMsg(curCombatVideoMsg.m_VideoId, ref curCombatVideoMsg, true, roundId, out roundVideoMsg))
        {
            if (_curPlayRoundId == roundId)
                return true;

            RestoreMobRoundStatesInVideo(_curPlayRoundId, roundId);

            _curPlayRoundId = roundId;
            _sendPlayRoundId = roundId;
            
            DoRoundStartNtf(roundVideoMsg.m_CmdBattleRoundStartNtf);

            if (!IsExistBullets(roundId))
            {
                _needBulletDataOfRoundId = roundId;
                SendVideoPlayDetailReq(_curVideoId, enPlayDetailType.PlayDetailTypeBullet);
            }
            else
            {
                _needBulletDataOfRoundId = 0u;
                Sys_Video.Instance.eventEmitter.Trigger(Sys_Video.EEvents.OnReceiveBullet);
            }

            DoRound(roundVideoMsg.m_CmdBattleRoundNtf);

            return true;
        }
        else
        {
            if (_sendPlayRoundId != roundId)
            {
                _sendPlayRoundId = roundId;

                //向服务器请求回合战斗数据
                SendVideoPlayDetailReq(_curVideoId, enPlayDetailType.PlayDetailTypePlay);
            }

            return false;
        }
    }

    public RepeatedField<ServerBullet> GetServerBulletList()
    {
        return GetServerBulletList(_curPlayRoundId);
    }

    private RepeatedField<ServerBullet> GetServerBulletList(uint roundId)
    {
        RoundVideoMsg roundVideoMsg = GetRoundVideoMsg(_curCombatVideoMsg, _curCombatVideoMsg.m_VideoId, roundId, false);
        if (roundVideoMsg == null)
        {
            DebugUtil.LogError($"GetServerBulletList----获取_curPlayRoundId:{_curPlayRoundId}数据RoundVideoMsg为null");
            return null;
        }

        return roundVideoMsg.m_ServerBullets;
    }

    private bool IsExistBullets(uint roundId)
    {
        return GetServerBulletList(roundId) != null;
    }
    #endregion

    #region 设备录像数据处理
    private void OnSaveVideoDataToLocation(bool isClearVideoInfo, ulong curVideoId, uint serverMaxRound)
    {
        //FileStore.DelProtoFile($"CombatVideo_{_curVideoId.ToString()}");

        //return;

        SaveVideoInfosToLocation(isClearVideoInfo, curVideoId, serverMaxRound);
    }

    private void SetLocationCombatVideoCatalog()
    {
        if (_combatVideoCatalogRecordList.Count > 0)
            return;

        if (PlayerPrefs.HasKey(_combatVideoCatalogRecordPlayerPrefs))
        {
            string recordStr = PlayerPrefs.GetString(_combatVideoCatalogRecordPlayerPrefs);
            if (!string.IsNullOrWhiteSpace(recordStr))
            {
                string[] records = recordStr.Split('|');
                for (int i = 0, len = records.Length; i < len && (i + 1 < len); i += 2)
                {
                    CombatVideoCatalogRecordInfo recordInfo = BasePoolClass.Get<CombatVideoCatalogRecordInfo>();
                    recordInfo.m_VideoId = ulong.Parse(records[i]);
                    recordInfo.m_RoundCount = uint.Parse(records[i + 1]);

                    _combatVideoCatalogRecordList.Add(recordInfo);
                }
            }
        }
    }

    private void AddLocationCombatVideoCatalog(ulong videoId, uint maxRound)
    {
        for (int i = 0, count = _combatVideoCatalogRecordList.Count; i < count; i++)
        {
            CombatVideoCatalogRecordInfo recordInfo = _combatVideoCatalogRecordList[i];
            if (recordInfo == null)
                continue;

            if (recordInfo.m_VideoId == videoId)
            {
                recordInfo.m_RoundCount = maxRound;
                return;
            }
        }

        CombatVideoCatalogRecordInfo ri = BasePoolClass.Get<CombatVideoCatalogRecordInfo>();
        ri.m_VideoId = videoId;
        ri.m_RoundCount = maxRound;

        _combatVideoCatalogRecordList.Add(ri);
    }

    private bool IsExistLocationCombatVideoCatalog(ulong videoId, bool isDelCacheVideo)
    {
        for (int i = 0, count = _combatVideoCatalogRecordList.Count; i < count; i++)
        {
            CombatVideoCatalogRecordInfo recordInfo = _combatVideoCatalogRecordList[i];
            if (recordInfo == null)
                continue;

            if (recordInfo.m_VideoId == videoId)
                return true;
        }

        if (_combatVideoCatalogRecordList.Count > 0 && 
            _combatVideoCatalogRecordList.Count >= _maxCacheVideoProtoInfoCount && isDelCacheVideo)
        {
            CombatVideoCatalogRecordInfo lastOldRecordInfo = _combatVideoCatalogRecordList[0];
            _combatVideoCatalogRecordList.RemoveAt(0);

            FileStore.DelProtoFile($"CombatVideo_{lastOldRecordInfo.m_VideoId.ToString()}", false);
        }

        return false;
    }

    private void SaveLocationCombatVideoCatalog()
    {
        if (_combatVideoCatalogRecordList.Count <= 0)
            return;

        if (_combatVideoCatalogRecordList.Count == 1)
        {
            CombatVideoCatalogRecordInfo recordInfo = _combatVideoCatalogRecordList[0];

            PlayerPrefs.SetString(_combatVideoCatalogRecordPlayerPrefs, $"{recordInfo.m_VideoId.ToString()}|{recordInfo.m_RoundCount.ToString()}");
        }
        else
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0, count = _combatVideoCatalogRecordList.Count; i < count; i++)
            {
                CombatVideoCatalogRecordInfo recordInfo = _combatVideoCatalogRecordList[i];
                if (recordInfo == null)
                    continue;

                sb.Append(recordInfo.m_VideoId.ToString());
                sb.Append("|");
                sb.Append(recordInfo.m_RoundCount.ToString());
                if (i + 1 < count)
                    sb.Append("|");
            }
            PlayerPrefs.SetString(_combatVideoCatalogRecordPlayerPrefs, sb.ToString());
        }
    }

    private VideoProtoInfo GetVideoLocationInfo(ulong videoId, uint serverRoundMax, uint cacheRoundMax)
    {
        VideoProtoInfo videoProtoInfo = null;
        if (cacheRoundMax == serverRoundMax)
        {
            FileStore.ReadProtoList($"CombatVideo_{videoId.ToString()}", _flagMessageParserDic, (ushort flag, IMessage msg) =>
            {
                if (videoProtoInfo == null)
                {
                    videoProtoInfo = BasePoolClass.Get<VideoProtoInfo>();
                    videoProtoInfo.m_VideoId = videoId;
                    videoProtoInfo.m_CacheStartTime = Time.time;
                    videoProtoInfo.m_UseCount = 1;
                    videoProtoInfo.m_IsLocation = true;

                    _videoProtoInfoList.Add(videoProtoInfo);
                }

                if (flag == (ushort)VideoProtoFlag.VideoRoundBrief_Flag)
                {
                    videoProtoInfo.m_VideoRoundBrief = msg as VideoRoundBrief;
                    ParseVideoRoundBrief(videoId, serverRoundMax, videoProtoInfo.m_VideoRoundBrief, false);
                }
                else if (flag == (ushort)VideoProtoFlag.VideoPlayDetail_Flag)
                {
                    VideoPlayDetail videoPlayDetail = msg as VideoPlayDetail;
                    if (videoPlayDetail != null)
                    {
                        if (videoProtoInfo.m_VideoPlayDetailList == null)
                            videoProtoInfo.m_VideoPlayDetailList = new List<VideoPlayDetail>();
                        videoProtoInfo.m_VideoPlayDetailList.Add(videoPlayDetail);
                        ParseVideoPlayDetail(videoId, serverRoundMax, videoPlayDetail, false, false);
                    }
                }
                else
                {
                    DebugUtil.LogError($"GetVideoInfoFromLocation  未处理的flag:{flag.ToString()}类型");
                }
            }, false);
        }
        
        return videoProtoInfo;
    }

    private void SaveVideoInfosToLocation(bool isClearVideoInfo, ulong curVideoId, uint serverMaxRound)
    {
        for (int infoIndex = 0, infoCount = _videoProtoInfoList.Count; infoIndex < infoCount; infoIndex++)
        {
            VideoProtoInfo videoProtoInfo = _videoProtoInfoList[infoIndex];
            if (videoProtoInfo == null)
                continue;

            //if (videoProtoInfo.m_IsLocation)
            //{
            //    AddLocationCombatVideoCatalog(videoProtoInfo.m_VideoId);
            //    continue;
            //}
            
            videoProtoInfo.m_IsLocation = true;

            uint cacheRoundCount = 0u;
            if (videoProtoInfo.m_VideoPlayDetailList != null)
            {
                for (int vpdIndex = 0, vpdCount = videoProtoInfo.m_VideoPlayDetailList.Count; vpdIndex < vpdCount; vpdIndex++)
                {
                    VideoPlayDetail vpd = videoProtoInfo.m_VideoPlayDetailList[vpdIndex];
                    if (vpd == null)
                        continue;

                    if (vpd.Round != null)
                        ++cacheRoundCount;
                }
            }

            if (videoProtoInfo.m_VideoId != curVideoId ||
                cacheRoundCount >= serverMaxRound)
            {
                IsExistLocationCombatVideoCatalog(videoProtoInfo.m_VideoId, true);

                if (SaveVideoInfoToLocation(videoProtoInfo))
                    AddLocationCombatVideoCatalog(videoProtoInfo.m_VideoId, serverMaxRound);
            }
            
            if (isClearVideoInfo)
                videoProtoInfo.Push();
        }

        if (isClearVideoInfo)
            _videoProtoInfoList.Clear();

        SaveLocationCombatVideoCatalog();
    }

    private bool SaveVideoInfoToLocation(VideoProtoInfo videoProtoInfo)
    {
        FileStore.WriteProtoList_Start(256);
        FileStore.WriteProtoList_Add((ushort)VideoProtoFlag.VideoRoundBrief_Flag, videoProtoInfo.m_VideoRoundBrief);
        if (videoProtoInfo.m_VideoPlayDetailList != null)
        {
            for (int detailIndex = 0, detailCount = videoProtoInfo.m_VideoPlayDetailList.Count; detailIndex < detailCount; detailIndex++)
            {
                VideoPlayDetail videoPlayDetail = videoProtoInfo.m_VideoPlayDetailList[detailIndex];
                if (videoPlayDetail != null && videoPlayDetail.Round != null &&
                    videoPlayDetail.Round.Rounds != null && videoPlayDetail.Round.Rounds.Count > 0)
                {
                    FileStore.WriteProtoList_Add((ushort)VideoProtoFlag.VideoPlayDetail_Flag, videoPlayDetail);
                }
            }
        }
        return FileStore.WriteProtoList_End($"CombatVideo_{videoProtoInfo.m_VideoId.ToString()}", false);
    }
    #endregion

    #region Request Http录像消息
    private UnityWebRequest _combatVideoWebReq;
    //string url = string.Format("{0}?appId={1}&channel={2}&channel_id={3}", VersionHelper.DirNoticeUrl, SDKManager.GetAppid(), SDKManager.GetChannel(), SDKManager.GetPublishAppMarket());
    //string url = "http://192.168.1.15:7777/api/getloginnotice?appId=appId_test&channel=C&channel_id=channel_id_test07";
    private void CombatVideoHttpReq(string url)
    {
        _combatVideoWebReq = UnityWebRequest.Get(url);
        UnityWebRequestAsyncOperation requestAsyncOperation = _combatVideoWebReq.SendWebRequest();
        requestAsyncOperation.completed += OnCombatVideoWebReqCompleted;
    }

    void OnCombatVideoWebReqCompleted(AsyncOperation operation)
    {
        if (_combatVideoWebReq.isHttpError || _combatVideoWebReq.isNetworkError)
        {
            Debug.LogError(_combatVideoWebReq.error);
        }
        else
        {
            //_combatVideoWebReq.downloadHandler.data
        }
    }
    #endregion
}
