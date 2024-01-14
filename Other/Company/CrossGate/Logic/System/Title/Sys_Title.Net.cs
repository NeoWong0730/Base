using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;

namespace Logic
{
    public partial class Sys_Title : SystemModuleBase<Sys_Title>, ISystemModuleUpdate
    {
        public List<TitleSeries> titleSeries = new List<TitleSeries>(); //所有系列

        public List<Title> titles = new List<Title>(); //所有称号
        public List<Title> AchievementTitles = new List<Title>(); //成就
        public List<Title> Prestigetitles = new List<Title>(); //声望
        public List<Title> CareerTitles = new List<Title>(); //职业
        public List<Title> Specialtitles = new List<Title>(); //特殊

        private int m_MaxPrestigeCount;
        private const uint s_FirstPrestigeId = 10001;

        private List<Title> actived_TimeLimitTitles = new List<Title>(); //已经激活的限时称号
        private List<Title> activedTitles = new List<Title>(); //已经激活的称号(包含永久)

        public uint curShowTitle; //当前穿戴的称号(称号外观)
        public uint curUseTitle; //当前使用的称号(称号属性)
        public List<uint> titlePos = new List<uint>(); //称号系列栏位   titlePos[i]=j  表示i+1 号栏位，装有j号id 称号系列

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        private uint totalTitleCategory; //称号一共有多少类 (读全局参数表)

        public int TotalTitlePos
        {
            get { return CSVTitleColumn.Instance.Count; }
        }

        public uint familyTitle;

        public uint bGroupTitle;

        public List<Title> unReadTitles = new List<Title>();

        public bool infoReceived;

        public enum EEvents
        {
            OnRefreshTitleButtonShowState, //更新称号按钮显示状态
            OnRefreshTitleButtonDressState, //更新称号按钮穿戴状态
            OnUpdateTitleAttrView, //更新任务属性界面角色头顶title
            OnUpdateTitleSeriesPos, //更新称号系列栏位
            OnRemoveNewFlagForTitlePosChange, //装备系列栏位 去除新
            OnUpdateSeriesCollectProgress, //更新称号系列收集进度
            OnNewRewardAvaliable, //新奖励可领取
            OnRefreshTitleCeil, //更新称号ceil
            OnUpdateCurTitleDes, //更新当前称号状态描述
            OnTitleGet, //获取到新title
            OnRefreshTitleRedState, //更新title红点状态  
            OnShowTitle,//穿戴(显示)称号
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort) CmdTitle.DressReq, (ushort) CmdTitle.DressRes, TitleDressRes, CmdTitleDressRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTitle.ShowReq, (ushort) CmdTitle.ShowRes, TitleShowRes, CmdTitleShowRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTitle.SuitChangeReq, (ushort) CmdTitle.SuitChangeRes, TitleSuitChangeRes, CmdTitleSuitChangeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTitle.GetRewardReq, (ushort) CmdTitle.GetRewardRes, GetRewardRes, CmdTitleGetRewardRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTitle.DressUnloadReq, (ushort) CmdTitle.DressUnloadRes, DressUnloadRes, CmdTitleDressUnloadRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTitle.ShowUnloadReq, (ushort) CmdTitle.ShowUnloadRes, ShowUnloadRes, CmdTitleShowUnloadRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort) CmdTitle.ShowUnloadReq, (ushort) CmdTitle.SuitFirstCollectReq, TitleSuitFirstCollectRes, CmdTitleSuitFirstCollectRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTitle.InfoRes, OnTitleInfoRes, CmdTitleInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTitle.StateNtf, TitleStateNtf, CmdTitleStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTitle.NewRewardNtf, NewRewardNtf, CmdTitleNewRewardNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTitle.SceneShowNtf, SceneShowNtf, CmdTitleSceneShowNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTitle.PosListNtf, PosListNtf, CmdTitlePosListNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort) CmdTitle.ExpireTimeReq, ExpireTimeRes, CmdTitleDressRes.Parser);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyInfo, OnGetFamilityTitle, true);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.JoinFamily, OnGetFamilityTitle, true);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyName, OnGetFamilityTitle, true);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMemberStatus, OnGetFamilityTitle, true);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMember, OnGetFamilityTitle, true);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.DestroyBranch, OnGetFamilityTitle, true);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.QuitFamily, OnRemoveFamilityTitle, true);

            totalTitleCategory = uint.Parse(CSVParam.Instance.GetConfData(677).str_value);
            familyTitle = uint.Parse(CSVParam.Instance.GetConfData(1059).str_value);
            bGroupTitle = 90001u;
        }

        public override void Dispose()
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyInfo, OnGetFamilityTitle, false);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.JoinFamily, OnGetFamilityTitle, false);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateFamilyName, OnGetFamilityTitle, false);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMemberStatus, OnGetFamilityTitle, false);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.DestroyBranch, OnGetFamilityTitle, false);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.UpdateMember, OnGetFamilityTitle, false);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.QuitFamily, OnRemoveFamilityTitle, false);
        }

        private void ParseClientData()
        {
            titles.Clear();
            AchievementTitles.Clear();
            Prestigetitles.Clear();
            CareerTitles.Clear();
            Specialtitles.Clear();
            titleSeries.Clear();
            actived_TimeLimitTitles.Clear();
            m_MaxPrestigeCount = 0;

            var titleSeriesDatas = CSVTitleSeries.Instance.GetAll();
            for (int i = 0, len = titleSeriesDatas.Count; i < len; i++)
            {
                TitleSeries titleSerie = new TitleSeries(titleSeriesDatas[i]);
                titleSeries.Add(titleSerie);
            }

            var titleDatas = CSVTitle.Instance.GetAll();
            for (int i = 0, len = titleDatas.Count; i < len; i++)
            {
                CSVTitle.Data item = titleDatas[i];
                if (item.isShow == 0)
                    continue;
                if (item.titleShowClass == 2 && item.titleType == 1) //声望
                {
                    m_MaxPrestigeCount++;
                }

                if (item.titleTypeNum > 1) //初始只加一级的
                {
                    continue;
                }

                if (item.titleShowClass == 3) //职业相关称号
                {
                    continue;
                }

                if (item.id == familyTitle) //家族特殊称号
                {
                    continue;
                }

                Title title = new Title(item.id, (TitleType) item.titleShowClass);
                titles.Add(title);
                if (item.titleShowClass == 1)
                {
                    AchievementTitles.Add(title);
                }
                else if (item.titleShowClass == 2)
                {
                    Prestigetitles.Add(title);
                }
                else if (item.titleShowClass == 3)
                {
                    CareerTitles.Add(title);
                }
                else if (item.titleShowClass == 4)
                {
                    Specialtitles.Add(title);
                }

                if (title.cSVTitleData.titleSeries != null)
                {
                    foreach (var kv in title.cSVTitleData.titleSeries)
                    {
                        TitleSeries titleSerie = titleSeries.Find(x => x.Id == kv);
                        if (titleSerie != null)
                        {
                            titleSerie.AddTitle(title);
                        }
                        else
                        {
                            DebugUtil.LogErrorFormat("没有找到id={0}的系列属性", kv);
                        }
                    }
                }
            }
        }

        public override void OnLogin()
        {
            infoReceived = false;
            TitleInfoReq();
        }

        //上线称号信息请求
        private void TitleInfoReq()
        {
            CmdTitleInfoReq cmdTitleInfoReq = new CmdTitleInfoReq();
            NetClient.Instance.SendMessage((ushort) CmdTitle.InfoReq, cmdTitleInfoReq);
        }


        // 称号信息返回
        private void OnTitleInfoRes(NetMsg netMsg)
        {
            CmdTitleInfoRes cmdTitleInfoRes = NetMsgUtil.Deserialize<CmdTitleInfoRes>(CmdTitleInfoRes.Parser, netMsg);

            ParseClientData();
            //处理称号
            foreach (var item in cmdTitleInfoRes.TitleList)
            {
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(item.TitleId);
                Title title = null;
                if (!HasTitle(item.TitleId))
                {
                    title = new Title(item.TitleId, (TitleType) cSVTitleData.titleShowClass);
                }
                else
                {
                    title = GetTitleData(item.TitleId);
                }

                title.ExpireTime = (ulong) item.ExpireTime;
                title.CalTitleEndTime();
            }

            //处理系列
            foreach (var item in cmdTitleInfoRes.TitleSuitList)
            {
                TitleSeries titleSerie = titleSeries.Find(x => x.Id == item.SuitId);
                if (titleSerie != null)
                {
                    titleSerie.IsFirstactive = item.IsFirst;
                    List<uint> temp = new List<uint>();
                    foreach (var kv in item.RewardType)
                    {
                        temp.Add(kv);
                    }

                    titleSerie.InitRewardState(temp);
                }
            }

            SortTitleSeries();

            //处理系列称号栏位
            titlePos.Clear();
            foreach (var item in cmdTitleInfoRes.TitlePosList)
            {
                titlePos.Add(item);
            }

            curShowTitle = cmdTitleInfoRes.ShowId;
            curUseTitle = cmdTitleInfoRes.UseId;

            //对4类数据分别排序
            for (int i = 0; i < totalTitleCategory; i++)
            {
                Sort((uint) i + 1);
            }

            foreach (var item in titleSeries)
            {
                item.SortTitleList();
                item.UpdateActiveState();
            }
            
            infoReceived = true;
            
            if (Sys_WarriorGroup.Instance.MyWarriorGroup.GroupUID != 0) 
            {
                UpdateBGroupTitle();
            }
        }

        //称号状态变更
        private void TitleStateNtf(NetMsg netMsg)
        {
            CmdTitleStateNtf cmdTitleStateNtf = NetMsgUtil.Deserialize<CmdTitleStateNtf>(CmdTitleStateNtf.Parser, netMsg);
            if (cmdTitleStateNtf.TitleId == familyTitle || cmdTitleStateNtf.TitleId == bGroupTitle) 
            {
                return;
            }

            //FliterCareerData();
            Title title = null;
            if (!HasTitle(cmdTitleStateNtf.TitleId))
            {
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(cmdTitleStateNtf.TitleId);
                title = new Title(cmdTitleStateNtf.TitleId, (TitleType) cSVTitleData.titleShowClass);
            }
            else
            {
                title = GetTitleData(cmdTitleStateNtf.TitleId);
            }

            title.ExpireTime = cmdTitleStateNtf.ExpireTime;
            title.CalTitleEndTime();
            if (title.ExpireTime != 1 && Contains(title.Id))
            {
                //提示
                CSVLanguage.Data data = CSVLanguage.Instance.GetConfData(title.cSVTitleData.titleLan);
                if (data == null)
                {
                    DebugUtil.LogErrorFormat($"languageId not found titleId: {title.Id}, lanId: {title.cSVTitleData.titleLan}");
                }
                else
                {
                    string content = LanguageHelper.GetTextContent(2020745, CSVLanguage.Instance.GetConfData(title.cSVTitleData.titleLan).words);
                    Sys_Hint.Instance.PushContent_Normal(content);
                }
                
                //Sys_MagicBook.Instance.eventEmitter.Trigger<EMagicAchievement, uint>(Sys_MagicBook.EEvents.MagicTaskCheckEvent2, EMagicAchievement.Event22, title.Id);
                title.read = false;
                unReadTitles.Add(title);
                eventEmitter.Trigger<uint>(EEvents.OnTitleGet, title.Id);
                eventEmitter.Trigger(EEvents.OnRefreshTitleRedState);
            }
            else
            {
                if (cmdTitleStateNtf.TitleId == curShowTitle)
                {
                    curShowTitle = 0;
                    ClearTitleEvt clearTitleEvt = new ClearTitleEvt();
                    clearTitleEvt.actorId = GameCenter.mainHero.uID;
                    Sys_HUD.Instance.eventEmitter.Trigger<ClearTitleEvt>(Sys_HUD.EEvents.OnClearTitle, clearTitleEvt);
                    eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleButtonShowState, cmdTitleStateNtf.TitleId);
                    eventEmitter.Trigger<uint>(EEvents.OnUpdateTitleAttrView, curShowTitle);
                }

                if (cmdTitleStateNtf.TitleId == curUseTitle)
                {
                    curUseTitle = 0;
                    eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleButtonDressState, cmdTitleStateNtf.TitleId);
                }
            }

            if (title.cSVTitleData.titleSeries != null)
            {
                foreach (var item in title.cSVTitleData.titleSeries)
                {
                    TitleSeries titleSerie = titleSeries.Find(x => x.Id == item);
                    if (cmdTitleStateNtf.ExpireTime == 1) //称号过期
                    {
                        titleSerie.SetActive(false);
                    }
                    else
                    {
                        titleSerie.UpdateActiveState(true);
                    }

                    titleSerie.SortTitleList();
                }
            }

            Sort(title.cSVTitleData.titleShowClass);
            SortTitleSeries();
            
            eventEmitter.Trigger(EEvents.OnUpdateSeriesCollectProgress);
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleCeil, title.Id);
        }

        //称号穿戴请求
        public void TitleDressReq(uint titleId)
        {
            CmdTitleDressReq cmdTitleDressReq = new CmdTitleDressReq();
            cmdTitleDressReq.TitleId = titleId;
            NetClient.Instance.SendMessage((ushort) CmdTitle.DressReq, cmdTitleDressReq);
        }

        //称号穿戴返回
        private void TitleDressRes(NetMsg netMsg)
        {
            CmdTitleDressRes cmdTitleDressRes = NetMsgUtil.Deserialize<CmdTitleDressRes>(CmdTitleDressRes.Parser, netMsg);
            curUseTitle = cmdTitleDressRes.UseId;
            bool firstshow = false;
            if (curShowTitle == 0)
            {
                firstshow = true;
            }

            curShowTitle = cmdTitleDressRes.ShowId;
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleButtonDressState, curUseTitle);
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleButtonShowState, curShowTitle);
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleCeil, curUseTitle);
            eventEmitter.Trigger(EEvents.OnUpdateCurTitleDes);
            eventEmitter.Trigger<uint>(EEvents.OnUpdateTitleAttrView, curShowTitle);
            if (firstshow)
            {
                CreateTitleEvt createTitleEvt = new CreateTitleEvt();
                createTitleEvt.actorId = GameCenter.mainHero.uID;
                createTitleEvt.titleId = curShowTitle;
                Sys_HUD.Instance.eventEmitter.Trigger<CreateTitleEvt>(Sys_HUD.EEvents.OnCreateTitle, createTitleEvt);
            }
        }

        //称号显示请求
        public void TitleShowReq(uint titleshowId)
        {
            CmdTitleShowReq cmdTitleShowReq = new CmdTitleShowReq();
            cmdTitleShowReq.TitleId = titleshowId;
            NetClient.Instance.SendMessage((ushort) CmdTitle.ShowReq, cmdTitleShowReq);
        }

        //称号显示返回
        private void TitleShowRes(NetMsg netMsg)
        {
            CmdTitleShowRes CmdTitleShowRes = NetMsgUtil.Deserialize<CmdTitleShowRes>(CmdTitleShowRes.Parser, netMsg);
            curShowTitle = CmdTitleShowRes.ShowId;
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleButtonShowState, curShowTitle);
            eventEmitter.Trigger<uint>(Sys_Title.EEvents.OnUpdateTitleAttrView, curShowTitle);
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleCeil, curShowTitle);
            eventEmitter.Trigger(EEvents.OnUpdateCurTitleDes);

            GameCenter.mainHero.heroBaseComponent.TitleId = curShowTitle;
            CreateTitleEvt createTitleEvt = new CreateTitleEvt();
            createTitleEvt.actorId = GameCenter.mainHero.uID;
            createTitleEvt.titleId = curShowTitle;
            if (createTitleEvt.titleId == familyTitle)
            {
                createTitleEvt.titleName = Sys_Family.Instance.GetFamilyName();
                createTitleEvt.pos = Sys_Family.Instance.familyData.CheckMe() == null ? 0 : Sys_Family.Instance.familyData.CheckMe().Position;
            }
            else if (createTitleEvt.titleId == bGroupTitle)
            {
                createTitleEvt.titleName = Sys_WarriorGroup.Instance.MyWarriorGroup.GroupName;
                createTitleEvt.pos = Sys_Role.Instance.RoleId == Sys_WarriorGroup.Instance.MyWarriorGroup.LeaderRoleID  ? 1u : 0u;
            }
            Sys_HUD.Instance.eventEmitter.Trigger<CreateTitleEvt>(Sys_HUD.EEvents.OnCreateTitle, createTitleEvt);
            eventEmitter.Trigger<uint>(EEvents.OnShowTitle,curShowTitle);
            //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.WearTitle);
        }

        //卸下称号请求
        public void DressUnloadReq()
        {
            CmdTitleDressUnloadReq cmdTitleDressUnloadReq = new CmdTitleDressUnloadReq();
            NetClient.Instance.SendMessage((ushort) CmdTitle.DressUnloadReq, cmdTitleDressUnloadReq);
        }

        //卸下称号返回
        private void DressUnloadRes(NetMsg netMsg)
        {
            CmdTitleDressUnloadRes cmdTitleDressUnloadRes = NetMsgUtil.Deserialize<CmdTitleDressUnloadRes>(CmdTitleDressUnloadRes.Parser, netMsg);
            if (cmdTitleDressUnloadRes.TitleId != curUseTitle)
            {
                DebugUtil.LogErrorFormat("当前穿戴的称号 {0},与需要卸下的称号{1}不一致", curUseTitle, cmdTitleDressUnloadRes.TitleId);
                return;
            }

            curUseTitle = 0;
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleButtonDressState, cmdTitleDressUnloadRes.TitleId);
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleCeil, cmdTitleDressUnloadRes.TitleId);
            eventEmitter.Trigger(EEvents.OnUpdateCurTitleDes);
        }

        //卸下显示请求
        public void ShowUnloadReq()
        {
            CmdTitleShowUnloadReq cmdTitleShowUnloadReq = new CmdTitleShowUnloadReq();
            NetClient.Instance.SendMessage((ushort) CmdTitle.ShowUnloadReq, cmdTitleShowUnloadReq);
        }

        //卸下显示返回
        private void ShowUnloadRes(NetMsg netMsg)
        {
            CmdTitleShowUnloadRes cmdTitleShowUnloadRes = NetMsgUtil.Deserialize<CmdTitleShowUnloadRes>(CmdTitleShowUnloadRes.Parser, netMsg);
            if (cmdTitleShowUnloadRes.TitleId != curShowTitle)
            {
                DebugUtil.LogErrorFormat("当前显示的称号 {0},与需要卸下的称号{1}不一致", curShowTitle, cmdTitleShowUnloadRes.TitleId);
                return;
            }

            curShowTitle = 0;
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleButtonShowState, cmdTitleShowUnloadRes.TitleId);
            eventEmitter.Trigger<uint>(Sys_Title.EEvents.OnUpdateTitleAttrView, curShowTitle);
            eventEmitter.Trigger<uint>(EEvents.OnRefreshTitleCeil, cmdTitleShowUnloadRes.TitleId);
            eventEmitter.Trigger(EEvents.OnUpdateCurTitleDes);

            ClearTitleEvt clearTitleEvt = new ClearTitleEvt();
            clearTitleEvt.actorId = GameCenter.mainHero.uID;
            Sys_HUD.Instance.eventEmitter.Trigger<ClearTitleEvt>(Sys_HUD.EEvents.OnClearTitle, clearTitleEvt);
        }

        //系列属性更换请求
        public void TitleSuitChangeReq(uint suitId, uint posIndex)
        {
            CmdTitleSuitChangeReq cmdTitleSuitChangeReq = new CmdTitleSuitChangeReq();
            cmdTitleSuitChangeReq.SuitId = suitId;
            cmdTitleSuitChangeReq.IndexId = posIndex;
            NetClient.Instance.SendMessage((ushort) CmdTitle.SuitChangeReq, cmdTitleSuitChangeReq);
        }

        //系列属性更换返回
        private void TitleSuitChangeRes(NetMsg netMsg)
        {
            CmdTitleSuitChangeRes cmdTitleSuitChangeRes = NetMsgUtil.Deserialize<CmdTitleSuitChangeRes>(CmdTitleSuitChangeRes.Parser, netMsg);
            int posIndex = (int) cmdTitleSuitChangeRes.IndexId;
            uint suitId = cmdTitleSuitChangeRes.SuitId;
            if (titlePos[posIndex] == suitId)
            {
                DebugUtil.LogErrorFormat("要装备的系列属性{0}，已经在{1}号栏位上了", cmdTitleSuitChangeRes.SuitId, cmdTitleSuitChangeRes.IndexId);
            }
            else
            {
                titlePos[posIndex] = suitId;
            }

            eventEmitter.Trigger(EEvents.OnUpdateTitleSeriesPos);
        }

        //系列奖励请求
        public void GetRewardReq(uint suitId)
        {
            CmdTitleGetRewardReq cmdTitleGetRewardReq = new CmdTitleGetRewardReq();
            cmdTitleGetRewardReq.SuitId = suitId;
            NetClient.Instance.SendMessage((ushort) CmdTitle.GetRewardReq, cmdTitleGetRewardReq);
        }

        //系列奖励返回
        private void GetRewardRes(NetMsg netMsg)
        {
            CmdTitleGetRewardRes cmdTitleGetRewardRes = NetMsgUtil.Deserialize<CmdTitleGetRewardRes>(CmdTitleGetRewardRes.Parser, netMsg);
            TitleSeries titleSerie = titleSeries.Find(x => x.Id == cmdTitleGetRewardRes.SuitId);
            titleSerie.SetRewardAvaliable(false);
            titleSerie.UpdateRewardStateAfterAchieve();
            eventEmitter.Trigger<uint>(EEvents.OnNewRewardAvaliable, titleSerie.Id);
        }

        public void TitleSuitFirstCollectReq(uint suitId)
        {
            CmdTitleSuitFirstCollectReq cmdTitleSuitFirstCollectReq = new CmdTitleSuitFirstCollectReq();
            cmdTitleSuitFirstCollectReq.SuitId = suitId;
            NetClient.Instance.SendMessage((ushort) CmdTitle.SuitFirstCollectReq, cmdTitleSuitFirstCollectReq);
        }

        private void TitleSuitFirstCollectRes(NetMsg netMsg)
        {
        }

        public void ExpireTimeReq(uint titleId)
        {
            CmdTitleExpireTimeReq cmdTitleExpireTimeReq = new CmdTitleExpireTimeReq();
            cmdTitleExpireTimeReq.TitleId = titleId;
            NetClient.Instance.SendMessage((ushort) CmdTitle.ExpireTimeReq, cmdTitleExpireTimeReq);
        }

        private void ExpireTimeRes(NetMsg netMsg)
        {
        }

        private void NewRewardNtf(NetMsg netMsg)
        {
            CmdTitleNewRewardNtf cmdTitleNewRewardNtf = NetMsgUtil.Deserialize<CmdTitleNewRewardNtf>(CmdTitleNewRewardNtf.Parser, netMsg);
            foreach (var item in cmdTitleNewRewardNtf.Rewards)
            {
                TitleSeries titleSerie = titleSeries.Find(x => x.Id == item.SuitId);
                titleSerie.UpdateRewardStateAvailbale((int) item.IndexId);
            }
        }

        private void PosListNtf(NetMsg netMsg)
        {
            CmdTitlePosListNtf cmdTitlePosListNtf = NetMsgUtil.Deserialize<CmdTitlePosListNtf>(CmdTitlePosListNtf.Parser, netMsg);
            //处理系列称号栏位
            titlePos.Clear();
            foreach (var item in cmdTitlePosListNtf.PosList)
            {
                titlePos.Add(item);
                foreach (var kv in titleSeries)
                {
                    if (kv.Id == item)
                    {
                        kv.IsFirstactive = false;
                        TitleSuitFirstCollectReq(kv.Id);
                        eventEmitter.Trigger(EEvents.OnRemoveNewFlagForTitlePosChange);
                    }
                }
            }

            eventEmitter.Trigger(EEvents.OnUpdateTitleSeriesPos);
        }

        private void SceneShowNtf(NetMsg netMsg)
        {
            CmdTitleSceneShowNtf cmdTitleNewRewardNtf = NetMsgUtil.Deserialize<CmdTitleSceneShowNtf>(CmdTitleSceneShowNtf.Parser, netMsg);
            if (cmdTitleNewRewardNtf.TitleId == 0)
            {
                Hero hero = GameCenter.GetSceneHero(cmdTitleNewRewardNtf.RoleId);
                if (hero != null) 
                {
                    hero.heroBaseComponent.TitleId = 0;
                    ClearTitleEvt clearTitleEvt = new ClearTitleEvt();
                    clearTitleEvt.actorId = cmdTitleNewRewardNtf.RoleId;
                    Sys_HUD.Instance.eventEmitter.Trigger<ClearTitleEvt>(Sys_HUD.EEvents.OnClearTitle, clearTitleEvt);
                }
            }
            else
            {
                if (GameCenter.otherActorsDic.TryGetValue(cmdTitleNewRewardNtf.RoleId, out Hero hero))
                {
                    CreateTitleEvt createTitleEvt = new CreateTitleEvt();
                    createTitleEvt.actorId = cmdTitleNewRewardNtf.RoleId;
                    createTitleEvt.titleId = cmdTitleNewRewardNtf.TitleId;
                    if ( createTitleEvt.titleId==Sys_Title.Instance.familyTitle)
                    {
                        createTitleEvt.titleName = hero.heroBaseComponent.FamilyName;
                        createTitleEvt.pos =  hero.heroBaseComponent.Pos;
                    }
                    else if(createTitleEvt.titleId==Sys_Title.Instance.bGroupTitle)
                    {
                        createTitleEvt.titleName = hero.heroBaseComponent.bGroupName;
                        createTitleEvt.pos = hero.heroBaseComponent.bGPos;
                    }
                    Sys_HUD.Instance.eventEmitter.Trigger<CreateTitleEvt>(Sys_HUD.EEvents.OnCreateTitle, createTitleEvt);
                }
            }
        }


        public void AddActiveTitle_Limit(Title title)
        {
            actived_TimeLimitTitles.AddOnce<Title>(title);
        }

        public void RemoveActiveTitle_Limit(Title title)
        {
            actived_TimeLimitTitles.TryRemove<Title>(title);
        }

        private void OnGetFamilityTitle()
        {
            Title title = GetTitleData(familyTitle);
            if (title == null)
            {
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(familyTitle);
                title = new Title(familyTitle, (TitleType) cSVTitleData.titleShowClass);
                titles.Add(title);
                Specialtitles.Add(title);
            }

            title.ExpireTime = 0;
            title.CalTitleEndTime();
            if (familyTitle == curShowTitle)
            {
                if (GameCenter.mainHero != null)
                {
                    ulong actorId = GameCenter.mainHero.uID;
                    uint titleId = familyTitle;
                    Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnUpdateFamilyTitleName, actorId, titleId);
                }
            }

            eventEmitter.Trigger(EEvents.OnRefreshTitleRedState);
            //Sys_MagicBook.Instance.eventEmitter.Trigger<EMagicAchievement, uint>(Sys_MagicBook.EEvents.MagicTaskCheckEvent2, EMagicAchievement.Event22, title.Id);
            Sort(title.cSVTitleData.titleShowClass);
        }

        private void OnRemoveFamilityTitle()
        {
            Title title = GetTitleData(familyTitle);
            if (title == null)
            {
                return;
            }

            titles.Remove(title);
            Specialtitles.Remove(title);
            title.ExpireTime = 1;
            title.CalTitleEndTime();
            Sort(title.cSVTitleData.titleShowClass);
            if (familyTitle == curShowTitle)
            {
                ClearTitleEvt clearTitleEvt = new ClearTitleEvt();
                clearTitleEvt.actorId = GameCenter.mainHero.uID;
                Sys_HUD.Instance.eventEmitter.Trigger<ClearTitleEvt>(Sys_HUD.EEvents.OnClearTitle, clearTitleEvt);
                curShowTitle = 0;
            }
        }


        public void UpdateBGroupTitle()
        {
            Title title = GetTitleData(bGroupTitle);
            if (title == null)
            {
                CSVTitle.Data cSVTitleData = CSVTitle.Instance.GetConfData(bGroupTitle);
                title = new Title(bGroupTitle, (TitleType) cSVTitleData.titleShowClass);
                titles.Add(title);
                Specialtitles.Add(title);
            }

            title.ExpireTime = 0;
            title.CalTitleEndTime();
            Sort(title.cSVTitleData.titleShowClass);
        }

        public void RemoveBGroupTitle()
        {
            Title title = GetTitleData(bGroupTitle);
            if (title == null)
            {
                return;
            }

            titles.Remove(title);
            Specialtitles.Remove(title);
            title.ExpireTime = 1;
            title.CalTitleEndTime();
            Sort(title.cSVTitleData.titleShowClass);
        }

        public void OnUpdate()
        {
            for (int i = 0; i < actived_TimeLimitTitles.Count; i++)
            {
                actived_TimeLimitTitles[i].Update();
            }
        }
    }
}