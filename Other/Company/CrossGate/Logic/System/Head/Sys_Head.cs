using Packet;
using Logic.Core;
using System.Collections.Generic;
using System;
using Net;
using Lib.Core;
using Table;
using UnityEngine.UI;

namespace Logic
{
    public enum EHeadViewType
    {
        None = 0,
        HeadView = 1,    //头像
        HeadFrameView = 2,     //头像框
        ChatFrameView = 3,     //聊天框
        ChatBackgraoudView = 4,         //聊天背景
        ChatTextView = 5,       //聊天文字
        TeamFalgView = 6,       //队标
    }

    public class HeadItemGetWayEvt
    {
        public uint isLimit;   //0--永久  1--限时  2--均可
        public uint limitGetWay;
        public List<uint> limitGetParam;
        public uint foreverGetWay;
        public List<uint> foreverGetParam;
        public uint unLockTips;
    }

    public class ClientHeadData
    {
        public uint headId;
        public uint headFrameId;
        public uint chatFrameId;
        public uint chatBackId;
        public uint chatTextId;
        public uint teamLogeId;
        public uint headIconId;
        public uint headFrameIconId;
        public uint chatFrameIconId;
        public string chatBackIconId;
        public uint chatTextIconId;
        public uint teamLogeIconId;

        public void SetDataByFrameType(uint type,uint id)
        {
            switch (type)
            {
                case 1:
                    headId = id;
                    CSVHead.Instance.TryGetValue(id, out CSVHead.Data data);
                    CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
                    if (data != null && data.HeadIcon[0]!= 0)
                    {
                        headIconId = Sys_Head.Instance.GetHeadIconIdByRoleType(data.HeadIcon);
                    }
                    else
                    {
                        if (heroData != null)
                        {
                            headIconId = heroData.headid;
                        }
                    }
                    break;
                case 2:
                    headFrameId = id;
                    CSVHeadframe.Instance.TryGetValue(id, out CSVHeadframe.Data headFrameData);
                    if (headFrameData != null)
                    {
                        headFrameIconId = CSVHeadframe.Instance.GetConfData(id).HeadframeIcon;
                    }
                    break;
                case 3:
                    chatFrameId = id;
                    CSVChatframe.Instance.TryGetValue(id, out CSVChatframe.Data chatFrameData);
                    if (chatFrameData != null)
                    {
                        chatFrameIconId = CSVChatframe.Instance.GetConfData(id).ChatIcon;
                    }
                    break;
                case 4:
                        chatBackId = id;
                        CSVChatBack.Instance.TryGetValue(id, out CSVChatBack.Data csvChatBackdata);
                        if (csvChatBackdata != null)
                        {
                            chatBackIconId = CSVChatBack.Instance.GetConfData(id).BackIcon;
                        }               
                    break;
                case 5:
                    chatTextId = id;
                    CSVChatWord.Instance.TryGetValue(id, out CSVChatWord.Data chatWorddata);
                    if (chatWorddata != null)
                    {
                        chatTextIconId = CSVChatWord.Instance.GetConfData(id).WordIcon;
                    }
                    break;
                case 6:
                    teamLogeId = id;
                    CSVTeamLogo.Instance.TryGetValue(id, out CSVTeamLogo.Data teamdata);
                    if (teamdata != null)
                    {
                        teamLogeIconId = CSVTeamLogo.Instance.GetConfData(id).TeamIcon;
                    }
                    break;
            }
        }

        public bool isUsing(uint id, uint type)
        {
            bool isUsing = false;
            switch (type)
            {
                case 1:
                    isUsing = headId == id;
                    break;
                case 2:
                    isUsing = headFrameId == id;
                    break;
                case 3:
                    isUsing = chatFrameId == id;
                    break;
                case 4:
                    isUsing = chatBackId == id;
                    break;
                case 5:
                    isUsing = chatTextId == id;
                    break;
                case 6:
                    isUsing = teamLogeId == id;
                    break;
            }
            return isUsing;
        }
    }

    public class Sys_Head : SystemModuleBase<Sys_Head>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public ClientHeadData clientHead = new ClientHeadData();
        public List<PictureFrameMap> activeList = new List<PictureFrameMap>();
        public List<uint> activeInfos = new List<uint>();
        public List<uint> expInfos = new List<uint>();
        public Dictionary<uint, uint> activeInfosDic = new Dictionary<uint, uint>();
        public Dictionary<uint, bool> activeInfosCheckDic= new Dictionary<uint, bool>(); 

        public enum EEvents : int
        {
            OnSelectViewType, //角色面板选择类型
            OnSelectItem,     //选中
            OnExpritedUpdate,     //过期刷新
            OnUsingUpdate,     //使用刷新
            OnAddActiveDataUpdate,     //激活新数据刷新
            OnChatBackChangeUpdate,     //聊天背景刷新
            OnAddUpdate,     //添加刷新
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHeadFrame.AddNtf, OnAddNtf, CmdHeadFrameAddNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHeadFrame.DataNtf, OnDataNtf, CmdHeadFrameDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHeadFrame.ExpiredNtf, OnExpiredNtf, CmdHeadFrameExpiredNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdHeadFrame.SetNtf, OnSetNtf, CmdHeadFrameSetNtf.Parser);
        }

        private void OnSetNtf(NetMsg msg)
        {
            CmdHeadFrameSetNtf ntf = NetMsgUtil.Deserialize<CmdHeadFrameSetNtf>(CmdHeadFrameSetNtf.Parser, msg);
            clientHead.SetDataByFrameType(ntf.FrameType, ntf.FrameId);
            if (ntf.FrameType == 4)
            {
                eventEmitter.Trigger(EEvents.OnChatBackChangeUpdate);
            }
            eventEmitter.Trigger(EEvents.OnUsingUpdate);
        }

        private void OnExpiredNtf(NetMsg msg)
        {
            CmdHeadFrameExpiredNtf ntf = NetMsgUtil.Deserialize<CmdHeadFrameExpiredNtf>(CmdHeadFrameExpiredNtf.Parser, msg);
            expInfos.Clear();
            for (int i = 0; i < ntf.FrameMap.Count; ++i)
            {
                for (int j = 0; j < ntf.FrameMap[i].Info.Count; ++j)
                {
                    if (activeInfos.Contains(ntf.FrameMap[i].Info[j].FrameId))
                    {
                        expInfos.Add(ntf.FrameMap[i].Info[j].FrameId);
                        activeInfos.Remove(ntf.FrameMap[i].Info[j].FrameId);
                    }
                    if (activeInfosCheckDic.ContainsKey(ntf.FrameMap[i].Info[j].FrameId))
                    {
                        activeInfosCheckDic.Remove(ntf.FrameMap[i].Info[j].FrameId);
                    }
                }
            }

            for (int i = 0; i < activeList.Count; ++i)
            {
                for (int j = 0; j < activeList[i].Info.Count; ++j)
                {
                    if (expInfos.Contains(activeList[i].Info[j].FrameId))
                    {
                        activeList[i].Info.Remove(activeList[i].Info[j]);
                    }
                }
            }
            eventEmitter.Trigger(EEvents.OnExpritedUpdate);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnRoleIconRedPoint, null);
        }

        private void OnDataNtf(NetMsg msg)
        {
            CmdHeadFrameDataNtf ntf = NetMsgUtil.Deserialize<CmdHeadFrameDataNtf>(CmdHeadFrameDataNtf.Parser, msg);
            clientHead.SetDataByFrameType((uint)EHeadViewType.HeadView, ntf.HeadPhoto);
            clientHead.SetDataByFrameType((uint)EHeadViewType.HeadFrameView, ntf.HeadFrame);
            clientHead.SetDataByFrameType((uint)EHeadViewType.ChatFrameView, ntf.ChatFrame);
            clientHead.SetDataByFrameType((uint)EHeadViewType.ChatBackgraoudView, ntf.ChatBack);
            clientHead.SetDataByFrameType((uint)EHeadViewType.ChatTextView, ntf.ChatText);
            clientHead.SetDataByFrameType((uint)EHeadViewType.TeamFalgView, ntf.TeamLogo);
            for (int i = 0; i < ntf.FrameMap.Count; ++i)
            {
                if (!activeList.Contains(ntf.FrameMap[i]))
                {               
                    activeList.Add(ntf.FrameMap[i]);
                }
                for (int j = 0; j < ntf.FrameMap[i].Info.Count; ++j)
                {
                    if (!activeInfos.Contains(ntf.FrameMap[i].Info[j].FrameId))
                    {
                        activeInfos.Add(ntf.FrameMap[i].Info[j].FrameId);
                    }
                }
            }      
            CheckHeadExpired();
        }

        private void OnAddNtf(NetMsg msg)
        {
            CmdHeadFrameAddNtf ntf = NetMsgUtil.Deserialize<CmdHeadFrameAddNtf>(CmdHeadFrameAddNtf.Parser, msg);
            GetActivceInfosDicByActiveList();
            HeadItemGetWayEvt addEvt = new HeadItemGetWayEvt();
            for (int i = 0; i < ntf.FrameMap.Count; ++i)
            {
                for (int j = 0; j < ntf.FrameMap[i].Info.Count; ++j)
                {
                    addEvt = GetItemWay(ntf.FrameMap[i].Info[j].FrameId, (EHeadViewType)ntf.FrameMap[i].FrameType);
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(addEvt.unLockTips));
                    if (!activeInfos.Contains(ntf.FrameMap[i].Info[j].FrameId))
                    {
                        activeInfos.Add(ntf.FrameMap[i].Info[j].FrameId);
                        activeList.Add(ntf.FrameMap[i]);
                    }
                    if (activeInfosDic.ContainsKey(ntf.FrameMap[i].Info[j].FrameId))
                    {
                        activeInfosDic[ntf.FrameMap[i].Info[j].FrameId] = ntf.FrameMap[i].Info[j].EndTick;
                    }
                    else
                    {
                        activeInfosDic.Add(ntf.FrameMap[i].Info[j].FrameId, ntf.FrameMap[i].Info[j].EndTick);
                    }
                    if (!activeInfosCheckDic.ContainsKey(ntf.FrameMap[i].Info[j].FrameId))
                    {
                        activeInfosCheckDic.Add(ntf.FrameMap[i].Info[j].FrameId, false);
                    }
                }
            }
            CheckHeadActiveData();
            CheckHeadExpired();
            eventEmitter.Trigger(EEvents.OnAddUpdate);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnRoleIconRedPoint, null);
        }

        public void HeadFrameAllInfoReq()
        {
            CmdHeadFrameAllInfoReq req = new CmdHeadFrameAllInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdHeadFrame.AllInfoReq, req);
        }

        public void HeadFrameExpiredReq()
        {
            CmdHeadFrameExpiredReq req = new CmdHeadFrameExpiredReq();
            NetClient.Instance.SendMessage((ushort)CmdHeadFrame.ExpiredReq, req);
        }

        public void HeadFrameSetReq(uint frameType, uint frameId)
        {
            CmdHeadFrameSetReq req = new CmdHeadFrameSetReq();
            req.FrameId = frameId;
            req.FrameType = frameType;
            NetClient.Instance.SendMessage((ushort)CmdHeadFrame.SetReq, req);
        }

        public override void OnLogin()
        {
            base.OnLogin();
            Sys_Head.Instance.HeadFrameAllInfoReq();
            activeInfosCheckDic.Clear();
        }

        public override void OnLogout()
        {
            base.OnLogout();
            activeList.Clear();
            activeInfos.Clear();
            expInfos.Clear();
            activeInfosCheckDic.Clear();
        }

        #region Function

        public List<uint> GetDatasId(EHeadViewType type)
        {
            List<uint> list = new List<uint>();
            string chanel = SDKManager.GetChannel();
            switch (type)
            {
                case EHeadViewType.HeadView:
                    foreach (var data in CSVHead.Instance.GetAll())
                    {
                        if (data.SubPackageShow==null || ( chanel!=null&& data.SubPackageShow.Contains(chanel)))
                        {
                            list.Add(data.id);
                        }
                    }
                    break;
                case EHeadViewType.HeadFrameView:
                    foreach (var data in CSVHeadframe.Instance.GetAll())
                    {
                        if (data.SubPackageShow == null || (chanel != null && data.SubPackageShow.Contains(chanel)))
                        {
                            list.Add(data.id);
                        }
                    }
                    break;
                case EHeadViewType.ChatFrameView:
                    foreach (var data in CSVChatframe.Instance.GetAll())
                    {
                        if ( data.SubPackageShow == null || (chanel != null && data.SubPackageShow.Contains(chanel)))
                        {
                            list.Add(data.id);
                        }
                    }
                    break;
                case EHeadViewType.ChatBackgraoudView:
                    foreach (var data in CSVChatBack.Instance.GetAll())
                    {
                        if ( data.SubPackageShow == null || (chanel != null && data.SubPackageShow.Contains(chanel)))
                        {
                            list.Add(data.id);
                        }
                    }
                    break;
                case EHeadViewType.ChatTextView:
                    foreach (var data in CSVChatWord.Instance.GetAll())
                    {
                        if ( data.SubPackageShow == null || (chanel != null && data.SubPackageShow.Contains(chanel)))
                        {
                            list.Add(data.id);
                        }
                    }
                    break;
                case EHeadViewType.TeamFalgView:
                    foreach (var data in CSVTeamLogo.Instance.GetAll())
                    {
                        if ( data.SubPackageShow == null || (chanel != null && data.SubPackageShow.Contains(chanel)))
                        {
                            list.Add(data.id);
                        }
                    }
                    break;
                default:
                    break;
            }
            return list;
        }

        public PictureFrameMap.Types.FraInfo GetActivePictureFrameMap(uint id, EHeadViewType type)
        {
            for (int i = 0; i < activeList.Count; ++i)
            {
                if (activeList[i].FrameType == (uint)type)
                {
                    for (int j = 0; j < activeList[i].Info.Count; ++j)
                    {
                        if (activeList[i].Info[j].FrameId == id)
                        {
                            return activeList[i].Info[j];
                        }
                    }
                }
            }
            return null;
        }

        public int GetTypesActiveCount(EHeadViewType type)
        {
            int count = 0;
            for (int i = 0; i < activeList.Count; ++i)
            {
                if (activeList[i].FrameType == (uint)type)
                {
                    count+= activeList[i].Info.Count;
                }
            }
            return count;
        }

        public HeadItemGetWayEvt GetItemWay(uint id, EHeadViewType type)
        {
            HeadItemGetWayEvt evt = new HeadItemGetWayEvt();
            switch (type)
            {
                case EHeadViewType.HeadView:
                    CSVHead.Data headData = CSVHead.Instance.GetConfData(id);
                   if(headData!=null)
                    {
                        evt.isLimit = headData.LimitedTime;
                        evt.foreverGetWay = headData.HeadGetFor;
                        evt.foreverGetParam = headData.HeadParamFor;
                        evt.limitGetWay = headData.HeadGetLimit;
                        evt.limitGetParam = headData.HeadParamLimit;
                        evt.unLockTips = headData.Unlocktips;
                    }
                    break;
                case EHeadViewType.HeadFrameView:
                    CSVHeadframe.Data headFrameData = CSVHeadframe.Instance.GetConfData(id);
                    if (headFrameData != null)
                    {
                        evt.isLimit = headFrameData.LimitedTime;
                        evt.foreverGetWay = headFrameData.HeadframeGetFor;
                        evt.foreverGetParam = headFrameData.HeadframeParamFor;
                        evt.limitGetWay = headFrameData.HeadframeGetLimit;
                        evt.limitGetParam = headFrameData.HeadframeParamLimit;
                        evt.unLockTips = headFrameData.Unlocktips;
                    }
                    break;
                case EHeadViewType.ChatFrameView:
                    CSVChatframe.Data chatFrameData = CSVChatframe.Instance.GetConfData(id);
                    if (chatFrameData != null)
                    {
                        evt.isLimit = chatFrameData.LimitedTime;
                        evt.foreverGetWay = chatFrameData.ChatGetFor;
                        evt.foreverGetParam = chatFrameData.ChatParamFor;
                        evt.limitGetWay = chatFrameData.ChatGetLimit;
                        evt.limitGetParam = chatFrameData.ChatParamLimit;
                        evt.unLockTips = chatFrameData.Unlocktips;
                    }
                    break;
                case EHeadViewType.ChatBackgraoudView:
                    CSVChatBack.Data chatBgData = CSVChatBack.Instance.GetConfData(id);
                    if (chatBgData != null)
                    {
                        evt.isLimit = chatBgData.LimitedTime;
                        evt.foreverGetWay = chatBgData.BackGetFor;
                        evt.foreverGetParam = chatBgData.BackParamFor;
                        evt.limitGetWay = chatBgData.BackGetLimit;
                        evt.limitGetParam = chatBgData.BackParamLimit;
                        evt.unLockTips = chatBgData.Unlocktips;
                    }
                    break;
                case EHeadViewType.ChatTextView:
                    CSVChatWord.Data chatWordData = CSVChatWord.Instance.GetConfData(id);
                    if (chatWordData != null)
                    {
                        evt.isLimit = chatWordData.LimitedTime;
                        evt.foreverGetWay = chatWordData.WordGetFor;
                        evt.foreverGetParam = chatWordData.WordParamFor;
                        evt.limitGetWay = chatWordData.WordGetLimit;
                        evt.limitGetParam = chatWordData.WordParamLimit;
                        evt.unLockTips = chatWordData.Unlocktips;
                    }
                    break;
                case EHeadViewType.TeamFalgView:
                    CSVTeamLogo.Data teamLogoData = CSVTeamLogo.Instance.GetConfData(id);
                    if (teamLogoData != null)
                    {
                        evt.isLimit = teamLogoData.LimitedTime;
                        evt.foreverGetWay = teamLogoData.TeamGetFor;
                        evt.foreverGetParam = teamLogoData.TeamParamFor;
                        evt.limitGetWay = teamLogoData.TeamGetLimit;
                        evt.limitGetParam = teamLogoData.TeamParamLimit;
                        evt.unLockTips = teamLogoData.Unlocktips;
                    }
                    break;
                default:
                    break;
            }

            return evt;
        }

        public void CheckHeadExpired()
        {
            uint nowTime = Sys_Time.Instance.GetServerTime();
            for (int i = 0; i < activeList.Count; ++i)
            {
                for (int j = 0; j < activeList[i].Info.Count; ++j)
                {
                    if (activeList[i].Info[j].EndTick != 0)
                    {
                        if (nowTime < activeList[i].Info[j].EndTick)
                        {
                            Timer timer;
                            uint time = activeList[i].Info[j].EndTick - nowTime;
                            timer = Timer.Register(time, () =>
                              {
                                  HeadFrameExpiredReq();
                              }, null, false, true);
                        }
                    }
                }
            }
        }

        public void CheckHeadActiveData()
        {
            for (int i = 0; i < activeList.Count; ++i)
            {
                for (int j = 0; j < activeList[i].Info.Count; ++j)
                {
                    if (activeInfosDic.ContainsKey(activeList[i].Info[j].FrameId))
                    {
                        activeList[i].Info[j].EndTick = activeInfosDic[activeList[i].Info[j].FrameId];
                    }
                }
            }
        }


        public bool IsActiveData(uint id)
        {
            return activeInfos.Contains(id);
        }

        public void SetHeadAndFrameData(Image headIcon)
        {
            ImageHelper.SetIcon(headIcon, clientHead.headIconId);
            Image headFrame = headIcon.transform.Find("Image_Before_Frame").GetComponent<Image>();
            if (clientHead.headFrameIconId == 0)
            {
                headFrame.gameObject.SetActive(false);
            }
            else
            {
                headFrame.gameObject.SetActive(true);
                ImageHelper.SetIcon(headFrame, clientHead.headFrameIconId);
            }
        }

        public uint GetHeadIconIdByRoleType(List<uint> headIcons, uint heroId=0)
        {
            if (heroId == 0)
            {
                heroId = Sys_Role.Instance.Role.HeroId;
            }
            if (headIcons.Count > 1)   // 分角色头像
            {
                for (int i = 0; i < headIcons.Count; ++i)
                {
                    uint roleId = headIcons[i] % 10000;
                    if (heroId == roleId)
                    {
                       return headIcons[i];
                    }
                }
                return 0;
            }
            else
            {
                return  headIcons[0];
            }
        }

        public void GetActivceInfosDicByActiveList()
        {
            activeInfosDic.Clear();
            for (int i = 0; i < activeList.Count; ++i)
            {
                for (int j = 0; j < activeList[i].Info.Count; ++j)
                {
                    if (!activeInfosDic.ContainsKey(activeList[i].Info[j].FrameId))
                    {
                        activeInfosDic.Add(activeList[i].Info[j].FrameId, activeList[i].Info[j].EndTick);
                    }
                }
            }
        }

        public uint GetHeadImageId(uint heroId,uint headId)
        {
            List<uint> headIcons = new List<uint>();
            headIcons = CSVHead.Instance.GetConfData(headId).HeadIcon;
            if (headIcons.Count > 1)   // 分角色头像
            {
                for (int i = 0; i < headIcons.Count; ++i)
                {
                    uint roleId = headIcons[i] % 10000;
                    if (heroId == roleId)
                    {
                        return headIcons[i];
                    }
                }
                return 0;
            }
            else if(headIcons[0]==0)
            {
                CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(heroId);
                if (heroData == null)
                {
                    return 0;
                }
                else
                {
                  return  heroData.headid;
                }
            }
            else
            {
                return headIcons[0];
            }
        }

        public bool CheckShowRedPoint()
        {
            foreach(var item in activeInfosCheckDic)
            {
                if (!item.Value)
                {
                    return true;
                }
            }
            return false;
        }

        public EHeadViewType GetTypeById(uint id)
        {
            if (id < 200)
            {
                return EHeadViewType.HeadView;
            }
            else if (id >= 200 && id < 300)
            {
                return EHeadViewType.HeadFrameView;
            }
            else if (id >= 300 && id < 400)
            {
                return EHeadViewType.ChatFrameView;
            }
            else if (id >= 400 && id < 500)
            {
                return EHeadViewType.ChatBackgraoudView;
            }
            else if (id >= 500 && id < 600)
            {
                return EHeadViewType.ChatTextView;
            }
            else
            {
                return EHeadViewType.TeamFalgView;
            }
        }

        public uint GetTeamLogoId()
        {
            if (!Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId))
            {
                return 0;
            }
            int fullId = Sys_Team.Instance.isFull() ? 1 : 0;
            return clientHead.teamLogeId * 10 + (uint)fullId;
        }
        #endregion
    }
}

