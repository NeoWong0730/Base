using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using static Logic.Sys_Guide;

namespace Logic
{
    /// <summary>
    /// 红包稀有度
    /// </summary>
    public enum ERedEnvelopeRarity
    {
        Normal=1,//普通
        Rarity=2,//稀有
    }
    /// <summary>
    /// 红包品质
    /// </summary>
    public enum RedEnvelopeQuality
    {
        Normal = 1,//普通红包
        Golden = 2,//金红包
    }
    public enum EActivityState
    {
        Finished=1,//已结束
        Underway=2,//进行中
        NotOpen=3,//未开启
    }
    public class Sys_RedEnvelopeRain : SystemModuleBase<Sys_RedEnvelopeRain>
    {
        #region 系统函数
        public override void Init()
        {
            ProcessEvents(true);
            InitData();
        }
        public override void OnLogin()
        {
            CheckRainActivityTime();
        }
        public override void OnLogout()
        {
            isShowFaceView = true;
            CaneelAllTimer();
            ClearAllData();
        }
        public override void Dispose()
        {
            ProcessEvents(false);
            isShowFaceView = true;
            CaneelAllTimer();
            ClearAll();
        }
        #endregion
        #region 数据定义
        public enum EEvents
        {
            OnRefreshEnvelopeData = 1,//刷新红包cell
            OnRefreshActivityPreviewData=2,//刷新红包雨时间段预览
            OnRefreshRainRecord=3,//刷新红包雨记录
            OnRefreshRainStartHint=4,//刷新红包雨开始提示
            OnRefreshRainStartBeforeHint = 5,//红包雨开始前提示
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 红包生成起点个数</summary>
        public int originNum = 20;
        /// <summary> 起点新坐标集合</summary>
        public float[] originPosX;
        /// <summary> 红包缩放比例集合</summary>
        public float[] cellScale = new float[] { 0.5f, 0.6f, 0.7f, 0.8f };
        /// <summary> 红包item间隔时间计时器</summary>
        Timer intrevalTime;
        /// <summary>红包雨开始计时器 </summary>
        Timer startTimer;
        /// <summary>红包雨结束计时器 </summary>
        Timer endTimer;

        /// <summary> 红包雨记录集合</summary>
        public List<RedEnvelopeGetRecordData> redEnvelopeRecordList = new List<RedEnvelopeGetRecordData>();
        /// <summary> 红包雨活动是否屏蔽</summary>
        public bool isHide;
        /// <summary> 各阶段红包领取次数</summary>
        Dictionary<int,uint> awardCountDic=new Dictionary<int, uint>();
        /// <summary> 当前红包雨表id</summary>
        public uint infoId;
        /// <summary> 当前红包雨抽奖数据</summary>
        public RedEnvelopeTakeAwardData curRedEnvelopeTakeAwardData;

        /// <summary> 当前红包雨活动规则表数据</summary>
        public CSVOperationalActivityRuler.Data curCSVActivityRulerData;
        /// <summary> 当前红包雨表数据</summary>
        public CSVRedEnvelopRain.Data curCSVRainData;
        /// <summary> 红包雨表数据集合</summary>
        Dictionary<uint, List<CSVRedEnvelopRain.Data>> rainDataDic = new Dictionary<uint, List<CSVRedEnvelopRain.Data>>();
        ///<summary>红包日当天所有的时间段预览 <summary>
        public List<ActivityPreviewData> activityPreviewDataList = new List<ActivityPreviewData>();

        /// <summary>当天红包进行到第几轮 curRainNum=-1当天活动之前 curRainNum=-2当天所有阶段活动后 curRainNum=?活动进行到第几轮</summary>
        public int curRainNum;
        /// </summary> 当前时间段是否是轮询空白期</summary>
        public bool isBlank;
        /// </summary> 活动开始时间 年月日</summary>
        public string activityStartTime;
        /// </summary> 活动开始时间 年月日</summary>
        public string activityEndTime;
        /// <summary>是否有拍脸图</summary>
        public bool isHaveBottom;
        /// <summary>是否有跑马灯</summary>
        public bool isHaveRolling;
        /// <summary>活动日第一天第一次活动时间</summary>
        DateTime firstDateTime;
        /// <summary>整个活动结束时间</summary>
        DateTime endDateTime;
        /// <summary>整个活动最后一场结束时间</summary>
        DateTime endLastDateTime;
        /// <summary>活动日每天第一次活动结束时间</summary>
        DateTime firstEveryDayDateTime;
        /// <summary>活动日每天最后一次活动结束时间</summary>
        DateTime lastEveryDayDateTime;
        /// <summary>拍脸图开始计时器</summary>
        Timer bottomStartTimer;
        /// <summary>跑马灯开始计时器</summary>
        Timer rollingStartTimer;
        /// <summary>跑马灯计时器</summary>
        Timer rollingTimer;
        /// <summary>跑马灯事件</summary>
        Action rollingAction = null;
        /// <summary>是否弹出拍脸view </summary>
        bool isShowFaceView;
        /// <summary>当天是否是活动日 </summary>
        bool isActivityDay;
        /// <summary>跑马灯倒计时时间 </summary>
        float rollingStartSecond;
        /// <summary>活动结束计时器 </summary>
        Timer activityEndTimer;
        /// <summary>每轮红包雨开始前提示计时器 </summary>
        Timer rainStartHint;

        bool isStart;
        int oldPosXIndex;
        int oldScaleIndex;
        System.Random random = new System.Random();
        bool UIRedEnvelopeRainIsOpen = false;
        bool UIRedEnvelopeRain_RemindIsOpen = false;
        private void ClearAll()
        {
            redEnvelopeRecordList.Clear();
            rainDataDic.Clear();
            activityPreviewDataList.Clear();
            awardCountDic.Clear();
            originPosX = null;
        }
        #endregion
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="v"></param>
        private void ProcessEvents(bool toRegister)
        {
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdRedEnvelope.GetInfoReq, (ushort)CmdRedEnvelope.GetInfoRes, OnGetInfoRes, CmdRedEnvelopeGetInfoRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdRedEnvelope.GetRecordReq, (ushort)CmdRedEnvelope.GetRecordRes, OnGetRecordRes, CmdRedEnvelopeGetRecordRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdRedEnvelope.SetHideReq, (ushort)CmdRedEnvelope.SetHideRes, OnSetHideRes, CmdRedEnvelopeSetHideRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdRedEnvelope.TakeAwardReq, (ushort)CmdRedEnvelope.TakeAwardRes, OnTakeAwardRes, CmdRedEnvelopeTakeAwardRes.Parser);
                Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
                Sys_Fight.Instance.OnExitFight += OnExitFight;
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRedEnvelope.GetInfoRes, OnGetInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRedEnvelope.GetRecordRes, OnGetRecordRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRedEnvelope.SetHideRes, OnSetHideRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdRedEnvelope.SetHideRes, OnTakeAwardRes);
                Sys_Fight.Instance.OnEnterFight -= OnEnterBattle;
                Sys_Fight.Instance.OnExitFight -= OnExitFight;
            }

            Sys_ActivityOperationRuler.Instance.eventEmitter.Handle(Sys_ActivityOperationRuler.EEvents.OnRefreshActivityInfo, CheckRainActivityTime, toRegister);

            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, RefreshActivityState, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnLoadOK, OnLoadMapOk, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.EndExit, EndExit, toRegister);
            UIManager.GetStackEventEmitter().Handle<uint, int>(UIStack.EUIStackEvent.BeginEnter, BeginEnter, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateCrossSrvState, OnUpdateCrossSrvState, toRegister);
            //线上调整完时间刷新红包雨数据，测试用
            //Sys_Chat.Instance.eventEmitter.Handle<ChatType>(Sys_Chat.EEvents.MessageAdd, OnMessageAdd, toRegister);
        }
        //private void OnMessageAdd(ChatType obj)
        //{
        //    if (obj == ChatType.Person)
        //    {
        //        DebugUtil.LogError("刷新红包雨数据");
        //        CheckRainActivityTime();
        //    }
        //}
        /// <summary>
        /// 刷新跨服状态
        /// </summary>
        private void OnUpdateCrossSrvState()
        {
            if (Sys_Role.Instance.isCrossSrv)
                RainStop();
            else
                RainStart();
        }
        private void BeginEnter(uint stack, int id)
        {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_FavorabilityClue || eId == EUIID.UI_FavorabilityMusicList || eId == EUIID.UI_FavorabilityDanceList ||
                eId == EUIID.UI_FavorabilitySendGift || eId == EUIID.UI_FavorabilityThanks || eId == EUIID.UI_Dialogue || eId == EUIID.UI_FavorabilityFete
                || eId == EUIID.UI_CutSceneTop)
            {
                if (UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain))
                {
                    UIRedEnvelopeRainIsOpen = true;
                    UIManager.CloseUI(EUIID.UI_RedEnvelopeRain);
                }
                if (UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain_Remind))
                {
                    UIRedEnvelopeRain_RemindIsOpen = true;
                    UIManager.CloseUI(EUIID.UI_RedEnvelopeRain_Remind);
                }
            }
        }
        private void EndExit(uint stack, int id)
        {
            EUIID eId = (EUIID)id;
            if (eId == EUIID.UI_FavorabilityClue || eId == EUIID.UI_FavorabilityMusicList || eId == EUIID.UI_FavorabilityDanceList ||
                eId == EUIID.UI_FavorabilitySendGift || eId == EUIID.UI_FavorabilityThanks || eId == EUIID.UI_Dialogue || eId == EUIID.UI_FavorabilityFete
                ||eId==EUIID.UI_CutSceneTop)
            {
                if (UIRedEnvelopeRainIsOpen)
                {
                    UIRedEnvelopeRainIsOpen = false;
                    UIManager.OpenUI(EUIID.UI_RedEnvelopeRain);
                }
                if (UIRedEnvelopeRain_RemindIsOpen)
                {
                    UIRedEnvelopeRain_RemindIsOpen = false;
                    UIManager.OpenUI(EUIID.UI_RedEnvelopeRain_Remind);
                }
            }
        }
        /// <summary>
        /// 进入地图加载隐藏打开的界面
        /// </summary>
        private void OnEnterMap()
        {
            if (UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain))
            {
                UIRedEnvelopeRainIsOpen = true;
                UIManager.CloseUI(EUIID.UI_RedEnvelopeRain);
            }
            if (UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain_Remind))
            {
                UIRedEnvelopeRain_RemindIsOpen = true;
                UIManager.CloseUI(EUIID.UI_RedEnvelopeRain_Remind);
            }
        }
        private void OnLoadMapOk()
        {
            if (UIRedEnvelopeRainIsOpen)
            {
                UIRedEnvelopeRainIsOpen = false;
                UIManager.OpenUI(EUIID.UI_RedEnvelopeRain);
            }
            if (UIRedEnvelopeRain_RemindIsOpen)
            {
                UIRedEnvelopeRain_RemindIsOpen = false;
                UIManager.OpenUI(EUIID.UI_RedEnvelopeRain_Remind);
            }
        }

        private void OnEnterBattle(CSVBattleType.Data obj)
        {
            RainStop();
        }
        private void OnExitFight()
        {
            RainStart();
        }
        private void RefreshActivityState()
        {
            if (Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(204))
                RainStart();
            else
                RainStop();
        }
        #region Res
        private void OnGetInfoRes(NetMsg msg)
        {
            CmdRedEnvelopeGetInfoRes ntf = NetMsgUtil.Deserialize<CmdRedEnvelopeGetInfoRes>(CmdRedEnvelopeGetInfoRes.Parser, msg);
            infoId = ntf.InfoId;
            awardCountDic.Clear();
            if (ntf.AwardCount != null && ntf.AwardCount.Count > 0)
            {
                for (int i = 0; i < ntf.AwardCount.Count; i++)
                {
                    awardCountDic[i] = ntf.AwardCount[i];
                }
            }
            isHide = ntf.Hide;
        }
        private void OnGetRecordRes(NetMsg msg)
        {
            CmdRedEnvelopeGetRecordRes ntf = NetMsgUtil.Deserialize<CmdRedEnvelopeGetRecordRes>(CmdRedEnvelopeGetRecordRes.Parser, msg);
            redEnvelopeRecordList.Clear();
            if (ntf.Record != null && ntf.Record.Count > 0)
            {
                for (int i = 0; i < ntf.Record.Count; i++)
                {
                    RedEnvelopeGetRecordData data = new RedEnvelopeGetRecordData();
                    data.time = ntf.Record[i].Time;
                    if (ntf.Record[i].ItemList.Count > 0)
                    {
                        int count = ntf.Record[i].ItemList.Count / 2;
                        for (int j = 0; j < count; j++)
                        {
                            RedEnvelopeGetRecordData.ItemData itemData = new RedEnvelopeGetRecordData.ItemData();
                            itemData.id = ntf.Record[i].ItemList[2 * j];
                            itemData.count = ntf.Record[i].ItemList[2 * j + 1];
                            data.itemList.Add(itemData);
                        }
                    }
                    data.Init();
                    redEnvelopeRecordList.Add(data);
                }
                redEnvelopeRecordList.Sort((a,b)=> {
                    return (int)(b.time - a.time);
                });
                eventEmitter.Trigger(EEvents.OnRefreshRainRecord);
            }
        }
        private void OnSetHideRes(NetMsg msg)
        {
            CmdRedEnvelopeSetHideRes ntf = NetMsgUtil.Deserialize<CmdRedEnvelopeSetHideRes>(CmdRedEnvelopeSetHideRes.Parser, msg);
            isHide = ntf.Hide;
            if (isHide)
            {
                RainStop();
            }
            else
            {
                RainStart();
            }
        }
        private void OnTakeAwardRes(NetMsg msg)
        {
            CmdRedEnvelopeTakeAwardRes ntf = NetMsgUtil.Deserialize<CmdRedEnvelopeTakeAwardRes>(CmdRedEnvelopeTakeAwardRes.Parser, msg);
            awardCountDic[(int)ntf.AwardCountLoop] = ntf.AwardCount;
            if (curRedEnvelopeTakeAwardData == null)
                curRedEnvelopeTakeAwardData = new RedEnvelopeTakeAwardData();
            curRedEnvelopeTakeAwardData.infoId = ntf.InfoId;
            //if (ntf.ItemList.Count > 0)
            //{
            //    int count = ntf.ItemList.Count / 2;
            //    for (int i = 0; i < count; i++)
            //    {
            //        RedEnvelopeGetRecordData.ItemData itemData = new RedEnvelopeGetRecordData.ItemData();
            //        itemData.id = ntf.ItemList[2 * i];
            //        itemData.count = ntf.ItemList[2 * i + 1];
            //        curRedEnvelopeTakeAwardData.itemList.Add(itemData);
            //    }
            //}
            curRedEnvelopeTakeAwardData.time = ntf.Time;
            int index = GetCurIndexByCurRainNum();
            if (GetCurAwardByIndex(index) >= curCSVRainData.Limit_Max[index] && curCSVRainData.Limit_Max[index] != 0)
            {
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(1001919).words);
                RainStop();
            }
        }
        #endregion
        #region Req
        /// <summary>
        /// 请求获取红包相关信息
        /// </summary>
        /// <param name="id"></param>
        public void OnGetInfoReq()
        {
            if (curCSVRainData != null)
            {
                CmdRedEnvelopeGetInfoReq req = new CmdRedEnvelopeGetInfoReq();
                req.InfoId = curCSVRainData.id;
                NetClient.Instance.SendMessage((ushort)CmdRedEnvelope.GetInfoReq, req);
            }
        }
        /// <summary>
        /// 获取红包记录
        /// </summary>
        public void OnGetRecordReq()
        {
            CmdRedEnvelopeGetRecordReq req = new CmdRedEnvelopeGetRecordReq();
            NetClient.Instance.SendMessage((ushort)CmdRedEnvelope.GetRecordReq, req);
        }
        /// <summary>
        /// 请求设置屏蔽红包雨
        /// </summary>
        /// <param name="isHide"></param>
        public void OnSetHideReq(bool isHide)
        {
            CmdRedEnvelopeSetHideReq req = new CmdRedEnvelopeSetHideReq();
            req.Hide = isHide;
            NetClient.Instance.SendMessage((ushort)CmdRedEnvelope.SetHideReq, req);
        }
        /// <summary>
        /// 请求获取红包
        /// </summary>
        /// <param name="id">红包表中的id</param>
        /// <param name="dropId">掉落id</param>
        public void OnTakeAwardReq(uint dropId)
        {
            if (!ActivityIsOpen())//活动紧急关闭期间无法领取奖励
            {
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(599003018).words);
                return;
            }
            CmdRedEnvelopeTakeAwardReq req = new CmdRedEnvelopeTakeAwardReq();
            req.InfoId = curCSVRainData.id;
            req.DropId = dropId;
            NetClient.Instance.SendMessage((ushort)CmdRedEnvelope.TakeAwardReq, req);
        }

        #endregion

        public float GetCurWidthOrHeight(bool isWidth=true)
        {
            float curWidth = 1280f;
            float curHeight = 720f;
            float scale_a = curWidth / curHeight;
            float scale_b = (float)Screen.width / Screen.height;
            if (scale_a < scale_b)
                curWidth = Screen.width * (curHeight / Screen.height);
            else
                curHeight = Screen.height * (curWidth / Screen.width);
            if (isWidth)
                return curWidth;
            else
                return curHeight;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        private void InitData()
        {
            oldPosXIndex = -1;
            oldScaleIndex = -1;
            isStart = false;

            curRainNum = -1;

            isHaveBottom = false;
            isHaveRolling = false;

            isShowFaceView = true;
            isActivityDay = false;

            originPosX = new float[originNum - 1];

            float curWidth = GetCurWidthOrHeight();
            int diff = Mathf.FloorToInt(curWidth / originNum);
            float half = curWidth / 2;
            for (int i = 0; i < originNum; i++)
            {
                if (i < originNum - 1)
                {
                    int posX = diff * (i + 1);
                    originPosX[i] = posX - half;
                }
            }
            rainDataDic.Clear();

            var redEnvelopRainDatas = CSVRedEnvelopRain.Instance.GetAll();
            for (int i = 0, len = redEnvelopRainDatas.Count; i < len; i++)
            {
                CSVRedEnvelopRain.Data data = redEnvelopRainDatas[i];
                if (rainDataDic.ContainsKey(data.Activity_Id))
                {
                    rainDataDic[data.Activity_Id].Add(data);
                }
                else
                {
                    rainDataDic[data.Activity_Id] = new List<CSVRedEnvelopRain.Data>() { data };
                }
            }
            foreach (var item in rainDataDic.Values)
            {
                item.Sort((a, b) => {
                    return (int)(a.id - b.id);
                });
            }
        }
        #region 数据处理
        /// <summary>
        /// 退出登录清除所有数据
        /// </summary>
        private void ClearAllData()
        {
            curCSVActivityRulerData = null;
            curCSVRainData = null;
            isActivityDay = false;
            if (UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain))
                UIManager.CloseUI(EUIID.UI_RedEnvelopeRain);
            if (UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain_Remind))
                UIManager.CloseUI(EUIID.UI_RedEnvelopeRain_Remind);
        }
        uint curDay;
        /// <summary>
        /// 检测当日是否为红包雨日,是的话获取相应数据
        /// </summary>
        private void CheckRainActivityTime()
        {
            CheckActivityData checkActivityData = Sys_ActivityOperationRuler.Instance.ChechActivityDay(EActivityRulerType.RedEnvelopeRain);
            curCSVActivityRulerData = checkActivityData.curCSVActivityRulerData;
            isActivityDay = checkActivityData.isActivityDay;
            //活动未开启
            if (curCSVActivityRulerData == null || curCSVActivityRulerData.Activity_Switch == 0)
                return;

            DateTime dateTime = GetServerDateTime();
            ActivityInfo activityInfo = Sys_ActivityOperationRuler.Instance.GetActivityInfo(EActivityRulerType.RedEnvelopeRain);
            curDay = activityInfo != null ? activityInfo.currDay : 0;
            List <CSVRedEnvelopRain.Data>  dataList = null;
            if (rainDataDic.ContainsKey(curCSVActivityRulerData.id))
                dataList = rainDataDic[curCSVActivityRulerData.id];
            else
            {
                DebugUtil.LogError("CSVRedEnvelopRain not found id：" + curCSVActivityRulerData.id);
                return;
            }
            curCSVRainData = null;
            if (isActivityDay)
            {
                if (dataList != null && dataList.Count > 0)
                {
                    for (int i = 0; i < dataList.Count; i++)
                    {
                        if (curDay == dataList[i].Activity_Date)
                        {
                            curCSVRainData = dataList[i];
                            break;
                        }
                    }
                    //活动日期间数据为空，可能中间存在跨天，向后查找最近一天
                    if (curCSVRainData == null)
                    {
                        uint nextDay = curDay;
                        while (nextDay <= dataList[dataList.Count - 1].Activity_Date)
                        {
                            nextDay++;
                            for (int i = 0; i < dataList.Count; i++)
                            {
                                if (nextDay == dataList[i].Activity_Date)
                                {
                                    curCSVRainData = dataList[i];
                                    break;
                                }
                            }
                            if (curCSVRainData != null) break;
                        }
                    }
                }
            }
            else
            {
                //当天不是活动日 默认先给活动第一天数据
                if (dataList != null && dataList.Count > 0)
                {
                    curCSVRainData = dataList[0];
                }
            }
            if (curCSVRainData != null)
            {
                OnGetInfoReq();
            }
            
            SetFirstAndEndDateTime();
            SetActivityStartAndEndTime();
            ChechAdvanceNotice();
            SetActivityPreviewData();
            CheckCurRainNum();
            //今日红包雨已全部轮完
            if (curRainNum == -2)
                return;
            if (isActivityDay)
            {
                SetTimer();
            }
        }
        /// <summary>
        /// 设置第一天第一场、最后一天最后一场活动时间
        /// </summary>
        private void SetFirstAndEndDateTime()
        {
            DateTime dateTime = GetServerDateTime();
            int openServiceDay = Sys_ActivityOperationRuler.Instance.GetOpenServiceDay();
            int startDay = 0;
            int startMonth = 0;
            int startYear = 0;
            if (curCSVActivityRulerData.Activity_Type == 1 || curCSVActivityRulerData.Activity_Type == 4 || curCSVActivityRulerData.Activity_Type == 5)
            {
                startDay = (int)curCSVActivityRulerData.Begining_Date;
                startMonth = dateTime.Month;
                startYear = dateTime.Year;
            }
            else if (curCSVActivityRulerData.Activity_Type == 2)
            {
                DateTime time = TimeManager.START_TIME.AddSeconds(TimeManager.ConvertFromZeroTimeZone(curCSVActivityRulerData.Begining_Date));
                startDay = time.Day;
                startMonth = time.Month;
                startYear = time.Year;
            }
            else if (curCSVActivityRulerData.Activity_Type == 3)
            {
                int diffDay = (int)(openServiceDay - curCSVActivityRulerData.Begining_Date);
                DateTime diffTime = dateTime.Subtract(TimeSpan.FromDays(diffDay));
                startDay = diffTime.Day;
                startMonth = diffTime.Month;
                startYear = diffTime.Year;
            }

            int hourStart = 0;
            int secondStart = 0;
            int hourEnd = 0;
            int secondSEnd = 0;
            if (curCSVRainData.Envelop_Begin != null && curCSVRainData.Envelop_Begin.Count > 0)
            {
                hourStart = (int)curCSVRainData.Envelop_Begin[0][0];
                secondStart = (int)curCSVRainData.Envelop_Begin[0][1];
                hourEnd = (int)curCSVRainData.Envelop_Begin[curCSVRainData.Envelop_Begin.Count - 1][0];
                secondSEnd = (int)curCSVRainData.Envelop_Begin[curCSVRainData.Envelop_Begin.Count - 1][1];
            }
            if (startYear == 0 || startMonth == 0 || startDay == 0)
                return;

            firstDateTime = new DateTime(startYear, startMonth, startDay, hourStart, secondStart, 0);
            DateTime diffDateTime1 = firstDateTime.Subtract(TimeSpan.FromDays(-curCSVActivityRulerData.Duration_Day));
            DateTime diffDateTime2 = firstDateTime.Subtract(TimeSpan.FromDays(-(curCSVActivityRulerData.Duration_Day - 1)));
            endDateTime = new DateTime(diffDateTime1.Year, diffDateTime1.Month, diffDateTime1.Day, 0, 0, 0);
            endLastDateTime = new DateTime(diffDateTime2.Year, diffDateTime2.Month, diffDateTime2.Day, hourEnd, secondSEnd, 0);

            int day,month,year;
            if (isActivityDay)
            {
                if (curDay != curCSVRainData.Activity_Date)
                    day = firstDateTime.Subtract(TimeSpan.FromDays(-curCSVRainData.Activity_Date + 1)).Day;
                else
                    day = dateTime.Day;
            }
            else
                day = firstDateTime.Day;
            //开服时间大于当前服务器时间
            if (openServiceDay < 0)
            {
                month = startMonth;
                year = startYear;
            }
            else
            {
                year = dateTime.Year;
                month = dateTime.Month;
            }
            List<uint> firstTimeData = curCSVRainData.Envelop_Begin[0];
            List<uint> lastTimeData = curCSVRainData.Envelop_Begin[curCSVRainData.Envelop_Begin.Count - 1];
            uint durationSecond = curCSVRainData.Duration_Second[curCSVRainData.Envelop_Begin.Count - 1];
            
            firstEveryDayDateTime = new DateTime(year, month, day, (int)firstTimeData[0], (int)firstTimeData[1], 0);
            DateTime last = new DateTime(year, month, day, (int)lastTimeData[0], (int)lastTimeData[1], 0);
            lastEveryDayDateTime = last.Subtract(TimeSpan.FromSeconds(-durationSecond));

            DateTime endLocalTime = endDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            DateTime localServerTime = TimeManager.GetDateTime(TimeManager.GetServerTime());
            //活动结束倒计时刷新
            float duration = (float)(DiffDateTime(endLocalTime, localServerTime).TotalSeconds);
            activityEndTimer?.Cancel();
            activityEndTimer = Timer.Register(duration, () => {
                activityEndTimer?.Cancel();
                curCSVActivityRulerData = null;
                curCSVRainData = null;
                isActivityDay = false;
                eventEmitter.Trigger(EEvents.OnRefreshActivityPreviewData);
            }, null, false, true);
        }
        /// <summary>
        /// 设置活动开始、结束时间
        /// </summary>
        private void SetActivityStartAndEndTime()
        {
            DateTime startLocalTime = firstDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            DateTime endLocalTime = endLastDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            activityStartTime = LanguageHelper.GetTextContent(1001922, startLocalTime.Year.ToString(), startLocalTime.Month.ToString(), startLocalTime.Day.ToString());
            activityEndTime = LanguageHelper.GetTextContent(1001922, endLocalTime.Year.ToString(), endLocalTime.Month.ToString(), endLocalTime.Day.ToString());
        }
        /// <summary>
        /// 检查红包雨活动是否存活
        /// </summary>
        public bool CheckRainActivityIsAlive()
        {
            bool isAlive = false;
            if (curCSVActivityRulerData == null || curCSVActivityRulerData.Activity_Switch == 0 || curCSVRainData == null)
            {
                isAlive = false;
            }
            else
            {
                if (isActivityDay)
                {
                    isAlive = true;
                }
                else
                {
                    DateTime dateTime = GetServerDateTime();
                    uint advanceTime = 0;
                    if (curCSVActivityRulerData.Open_Bottom != 0)
                    {
                        if (curCSVActivityRulerData.Rolling_Time != 0)
                        {
                            if (curCSVActivityRulerData.Open_Bottom > curCSVActivityRulerData.Rolling_Time)
                                advanceTime = curCSVActivityRulerData.Open_Bottom;

                            else
                                advanceTime = curCSVActivityRulerData.Rolling_Time;
                        }
                        else
                            advanceTime = curCSVActivityRulerData.Open_Bottom;
                    }
                    if (advanceTime != 0)
                    {
                        DateTime bottomDateTime = firstDateTime.Subtract(TimeSpan.FromMinutes(advanceTime));
                        TimeSpan startspan = DiffDateTime(dateTime, bottomDateTime);
                        TimeSpan endSpan = DiffDateTime(dateTime, endDateTime);
                        if (startspan.TotalSeconds >= 0 && endSpan.TotalSeconds < 0)
                            isAlive = true;
                        else
                            isAlive = false;
                    }
                }
            }
            return isAlive;
        }
        DateTime bottomDateTime;
        DateTime rollingDateTime;
        /// <summary>
        /// 检查活动预告(拍脸图、跑马灯、系统公告)
        /// </summary>
        private void ChechAdvanceNotice()
        {
            DateTime localServerTime = TimeManager.GetDateTime(TimeManager.GetServerTime());
            TimeSpan bottomSpan;
            TimeSpan rollingSpan;
            DateTime startLocalTime = firstDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            DateTime endLastLocalTime = endLastDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            TimeSpan spanStart = DiffDateTime(localServerTime, startLocalTime);
            TimeSpan spanEnd = DiffDateTime(localServerTime, endLastLocalTime);
            if (curCSVActivityRulerData.Open_Bottom != 0)
            {
                bottomDateTime = startLocalTime.Subtract(TimeSpan.FromMinutes(curCSVActivityRulerData.Open_Bottom));
                bottomSpan = DiffDateTime(localServerTime, bottomDateTime);
                isHaveBottom = bottomSpan.TotalSeconds < 0 ? false : spanEnd.TotalSeconds < 0 ? true : false;
            }
            else
                isHaveBottom = false;
            if (curCSVActivityRulerData.Rolling_Time != 0)
            {
                rollingDateTime = startLocalTime.Subtract(TimeSpan.FromMinutes(curCSVActivityRulerData.Rolling_Time));
                rollingSpan = DiffDateTime(localServerTime, rollingDateTime);
                isHaveRolling = rollingSpan.TotalSeconds < 0 ? false : spanStart.TotalSeconds < 0 ? true : false;
            }
            else
                isHaveRolling = false;

            if (curCSVActivityRulerData.Open_Bottom != 0 && bottomSpan != null && bottomSpan.TotalSeconds < 0)
            {
                bottomStartTimer?.Cancel();
                bottomStartTimer = Timer.Register((float)(-bottomSpan.TotalSeconds), () =>
                {
                    bottomStartTimer?.Cancel();
                    localServerTime = TimeManager.GetDateTime(TimeManager.GetServerTime());
                    bottomSpan = DiffDateTime(localServerTime, bottomDateTime);
                    spanEnd = DiffDateTime(localServerTime, endLastLocalTime);
                    isHaveBottom = bottomSpan.TotalSeconds < 0 ? false : spanEnd.TotalSeconds < 0 ? true : false;
                    eventEmitter.Trigger(EEvents.OnRefreshActivityPreviewData);
                }, null, false, true);
            }
            if (curCSVActivityRulerData.Rolling_Time != 0 && rollingSpan != null && rollingSpan.TotalSeconds < 0)
            {
                rollingStartTimer?.Cancel();
                rollingStartTimer = Timer.Register((float)(-rollingSpan.TotalSeconds), () => {
                    rollingStartTimer?.Cancel();
                    localServerTime = TimeManager.GetDateTime(TimeManager.GetServerTime());
                    rollingSpan = DiffDateTime(localServerTime, rollingDateTime);
                    spanStart = DiffDateTime(localServerTime, startLocalTime);
                    isHaveRolling = rollingSpan.TotalSeconds < 0 ? false : spanStart.TotalSeconds < 0 ? true : false;
                    rollingStartSecond = 0.1f;
                    SetRollingData();
                }, null, false, true);
            }

            if (isHaveRolling)
            {
                int second = (int)curCSVActivityRulerData.RollingLoop_Time * 60;
                rollingStartSecond = (int)(second - rollingSpan.TotalSeconds % second);
                SetRollingData();
            }
        }
        private void SetRollingData()
        {
            if (isHaveRolling)
            {
                rollingAction = () =>
                {
                    CSVErrorCode.Data errorData = CSVErrorCode.Instance.GetConfData(curCSVActivityRulerData.RollingShow_Id);
                    string startTime = firstDateTime.ToString("yyyy/MM/dd HH:mm:ss");
                    string content = string.Format(errorData.words, startTime);
                    ErrorCodeHelper.PushErrorCode(errorData.pos, content, content, curCSVActivityRulerData.RollingShow_Id, null, true);
                    rollingTimer?.Cancel();
                    SetRollingTime((int)curCSVActivityRulerData.RollingLoop_Time * 60);
                };
                SetRollingTime(rollingStartSecond);
            }
            else
            {
                rollingTimer?.Cancel();
            }
        }
        private void SetRollingTime(float duration)
        {
            rollingTimer?.Cancel();
            rollingTimer = Timer.Register(duration, () =>
            {
                if(isHaveRolling)
                  rollingAction?.Invoke();
            }, null, false, true);
        }
        /// <summary>
        /// 获取随机数据
        /// </summary>
        /// <param name="type">1为缩放比例，2为起始x坐标</param>
        /// <returns></returns>
        private int GetRandomValue(int type)
        {
            int index;
            int length = type == 1 ? cellScale.Length : originPosX.Length;
            int oldIndex = type == 1 ? oldScaleIndex : oldPosXIndex;
            while (true)
            {
                index = random.Next(0, length);
                if (oldIndex != index)
                {
                    if (type == 1)
                        oldScaleIndex = index;
                    else
                        oldPosXIndex = index;
                    break;
                }
            }
            return index;
        }
        /// <summary>
        /// 权重随机掉落
        /// </summary>
        /// <returns></returns>
        private void RandowDrop(EnvelopeData data)
        {
            if (curCSVRainData != null)
            {
                uint normalWeight = curCSVRainData.Envelop_Weight[0];
                uint goldWeight = curCSVRainData.Envelop_Weight[1];
                int weightValue = random.Next(1, (int)(normalWeight + goldWeight)+1);
                RedEnvelopeQuality quality = weightValue <= normalWeight ? RedEnvelopeQuality.Normal : RedEnvelopeQuality.Golden;
                data.quality = quality;
                uint value1 = quality == RedEnvelopeQuality.Normal ? curCSVRainData.Red_Quantity[0] : curCSVRainData.Gold_Quantity[0];
                uint value2 = quality == RedEnvelopeQuality.Normal ? curCSVRainData.Red_Quantity[1] : curCSVRainData.Gold_Quantity[1];
                int rarityValue = random.Next(1, (int)(value1 + value2)+1);
                ERedEnvelopeRarity rarity = rarityValue <= value1 ? ERedEnvelopeRarity.Normal : ERedEnvelopeRarity.Rarity;
                data.rarity = rarity;

                data.dropId = GetDropItemId(quality, rarity);
            }
        }
        /// <summary>
        /// 获取掉落物品id
        /// </summary>
        /// <param name="quality">红包品质</param>
        /// <param name="rarity">红包稀有度</param>
        /// <returns></returns>
        private uint GetDropItemId(RedEnvelopeQuality quality, ERedEnvelopeRarity rarity)
        {
            List<List<uint>> dropList;
            if (quality == RedEnvelopeQuality.Normal)
            {
                if (rarity == ERedEnvelopeRarity.Normal)
                    dropList = curCSVRainData.RedEnvelop_Drop;
                else
                    dropList = curCSVRainData.RedRare_Drop;
            }
            else
            {
                if (rarity == ERedEnvelopeRarity.Normal)
                    dropList = curCSVRainData.GoldEnvelop_Drop;
                else
                    dropList = curCSVRainData.GoldRare_Drop;
            }
            int allValue = 0;
            List<int> list = new List<int>();
            for (int i = 0; i < dropList.Count; i++)
            {
                allValue += (int)dropList[i][0];
                list.Add(allValue);
            }
            int index = 0;
            int dropValue = random.Next(1, allValue+1);
            for (int i = 0; i < list.Count; i++)
            {
                if (dropValue <= list[i])
                {
                    index = i;
                    break;
                }
            }
            list.Clear();
            return dropList[index][1];
        }
        /// <summary>
        /// 设置计时器
        /// </summary>
        private void SetTimer()
        {
            CheckCurRainNum();
            if (curRainNum == -2)
                return;
            int index;
            if (CheckCurRainIsUnderway())
            {
                index = GetCurIndexByCurRainNum();
            }
            else
                index = GetNextIndex();
            int diffStart = GetResidueSecond(true, index);
            int diffEnd = GetResidueSecond(false, index);

            SetRainStartBeforHint(diffStart - (int)curCSVRainData.AnnounceTime);
            SetStartOrEndTimer(true, diffStart);
            SetStartOrEndTimer(false, diffEnd);
        }
        private void SetStartOrEndTimer(bool isStartTime, int duration)
        {
            if (duration <= 0)
                return;
            if (isStartTime)
            {
                startTimer?.Cancel();
                startTimer = Timer.Register(duration, () =>
                {
                    startTimer?.Cancel();
                    if (UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain) && CheckCurRainIsUnderway())
                    {
                        int index = GetCurIndexByCurRainNum();
                        if (GetCurAwardByIndex(index) < curCSVRainData.Limit_Max[index] && curCSVRainData.Limit_Max[index] != 0)
                        {
                            if (!UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain_Remind))
                                UIManager.OpenUI(EUIID.UI_RedEnvelopeRain_Remind);
                            else
                                eventEmitter.Trigger(EEvents.OnRefreshRainStartHint);
                        }
                    }
                    CloseAllAdvance();
                    RefreshActivityPreviewDataState();
                    RainStart();
                }, null, false, true);
            }
            else
            {
                endTimer?.Cancel();
                endTimer = Timer.Register(duration, () => {
                    endTimer?.Cancel();
                    Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(1001920).words);
                    isStart = false;
                    eventEmitter.Trigger(EEvents.OnRefreshRainStartBeforeHint);
                    RainStop();
                    RefreshActivityPreviewDataState();
                    SetTimer();
                }, null, false, true);
            }
        }
        public bool CheckRainStartBeforHint()
        {
            bool isShow = false;
            if (curCSVActivityRulerData != null && curCSVRainData != null)
            {
                int curIndex = GetCurIndexByCurRainNum();
                int diffStart = GetResidueSecond(true, curIndex);
                int diffEnd = GetResidueSecond(false, curIndex);
                if (diffStart > 0 && diffStart - curCSVRainData.AnnounceTime <= 0 || (diffStart <= 0 && diffEnd > 0))
                {
                    isShow = true;
                }
                else
                {
                    int nextIndex = GetNextIndex();
                    diffStart = GetResidueSecond(true, nextIndex);
                    diffEnd = GetResidueSecond(false, nextIndex);
                    if (diffStart > 0 && diffStart - curCSVRainData.AnnounceTime <= 0 || (diffStart <= 0 && diffEnd > 0))
                    {
                        isShow = true;
                    }
                }
            }
            return isShow;
        }
        public int GetRainStartBeforHintDiffTime()
        {
            int diffTime = -1;
            if (curCSVActivityRulerData != null && curCSVRainData != null)
            {
                int index = GetNextIndex();
                int diffStart = GetResidueSecond(true, index);
                if (diffStart > 0 && diffStart - curCSVRainData.AnnounceTime <= 0)
                    diffTime = diffStart;
            }
            return diffTime;
        }
        /// <summary>
        /// 设置每场红包雨开始前的提示
        /// </summary>
        private void SetRainStartBeforHint(int duration)
        {
            if (duration <= 0)
                return;
            rainStartHint?.Cancel();
            rainStartHint = Timer.Register(duration, () => {
                rainStartHint?.Cancel();
                eventEmitter.Trigger(EEvents.OnRefreshRainStartBeforeHint);
            }, null, false, true);
        }
        /// <summary>
        /// 设置活动日当天所有时间段活动数据
        /// </summary>
        private void SetActivityPreviewData()
        {
            activityPreviewDataList.Clear();
            for (int i = 0; i < curCSVRainData.Envelop_Begin.Count; i++)
            {
                ActivityPreviewData data = new ActivityPreviewData();
                data.index = i + 1;
                data.startTime = curCSVRainData.Envelop_Begin[i];
                data.durationTime = curCSVRainData.Duration_Second[i];
                if (isActivityDay)
                {
                    int startResidueSceond = GetResidueSecond(true, i);
                    int endResidueSceond = GetResidueSecond(false, i);
                    if (startResidueSceond > 0)
                        data.state = EActivityState.NotOpen;
                    else if (startResidueSceond <= 0 && endResidueSceond > 0)
                        data.state = EActivityState.Underway;
                    else
                        data.state = EActivityState.Finished;
                }
                else
                {
                    data.state = EActivityState.NotOpen;
                }
                activityPreviewDataList.Add(data);
            }
            eventEmitter.Trigger(EEvents.OnRefreshActivityPreviewData);
        }
        /// <summary>
        /// 刷新活动日当天所有时间段活动状态
        /// </summary>
        private void RefreshActivityPreviewDataState()
        {
            for (int i = 0; i < curCSVRainData.Envelop_Begin.Count; i++)
            {
                ActivityPreviewData data = GetActivityPreviewDataByIndex(i + 1);
                if (isActivityDay)
                {
                    int startResidueSceond = GetResidueSecond(true, i);
                    int endResidueSceond = GetResidueSecond(false, i);
                    if (startResidueSceond > 0)
                        data.state = EActivityState.NotOpen;
                    else if (startResidueSceond <= 0 && endResidueSceond > 0)
                        data.state = EActivityState.Underway;
                    else
                        data.state = EActivityState.Finished;
                }
                else
                {
                    data.state = EActivityState.NotOpen;
                }
            }
            eventEmitter.Trigger(EEvents.OnRefreshActivityPreviewData);
        }
        public bool CheckCurDayActivityIsOver()
        {
            bool isOver = true;
            for (int i = 0; i < activityPreviewDataList.Count; i++)
            {
                if (activityPreviewDataList[i].state != EActivityState.Finished)
                {
                    isOver = false;
                    break;
                }
            }
            return isOver;
        }
        /// <summary>
        /// 获取后动预览数据by索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ActivityPreviewData GetActivityPreviewDataByIndex(int index)
        {
            for (int i = 0; i < activityPreviewDataList.Count; i++)
            {
                if (activityPreviewDataList[i].index == index)
                {
                    return activityPreviewDataList[i];
                }
            }
            return null;
        }
        public EnvelopeData GetEnvelopeData(EnvelopeData data)
        {
            if (data == null)
                data = new EnvelopeData();
            data.scale = cellScale[GetRandomValue(1)];
            data.posX = originPosX[GetRandomValue(2)];
            RandowDrop(data);
            return data;
        }
        /// <summary>
        /// 出队刷新红包雨下落数据
        /// </summary>
        public void GetNextEnvelopeData()
        {
            if (curCSVActivityRulerData != null && curCSVRainData != null && isStart && UIManager.IsVisibleAndOpen(EUIID.UI_RedEnvelopeRain_Main))
            {
                eventEmitter.Trigger(EEvents.OnRefreshEnvelopeData);
                intrevalTime = Timer.Register(curCSVRainData.Envelop_Quantity / 1000f, () =>
                {
                    intrevalTime?.Cancel();
                    if (isStart)
                    {
                        GetNextEnvelopeData();
                    }
                }, null, false, true);
            }
            else
            {
                intrevalTime?.Cancel();
            }
        }
        #endregion
        #region function
        public int GetCurIndexByCurRainNum()
        {
            CheckCurRainNum();
            int index = 0;
            if (curRainNum == -1)
                index = 0;
            if (!isBlank && curRainNum != -1 && curRainNum != -2)
                index = curRainNum - 1;
            if (curRainNum >= curCSVRainData.Envelop_Begin.Count || curRainNum == -2)
                index = curCSVRainData.Envelop_Begin.Count - 1;
            return index;
        }
        public int GetNextIndex()
        {
            CheckCurRainNum();
            return curRainNum == -1 ? 0 : (curRainNum >= curCSVRainData.Envelop_Begin.Count || curRainNum == -2) ? curCSVRainData.Envelop_Begin.Count - 1 : curRainNum;
        }
        public uint GetCurAwardByIndex(int index)
        {
            if (awardCountDic.ContainsKey(index))
            {
                return awardCountDic[index];
            }
            return 0;
        }

        public void RainStart()
        {
            ActivityIsOpen();
        }
        private void CheckRainStartCondition()
        {
            //正在进行红包雨 && 在主界面 && 不在战斗中 && 不在跨服 && 不在本服家族资源战
            if (CheckCurRainIsUnderway() && UIManager.IsVisibleAndOpen(EUIID.UI_Menu) && !Sys_Fight.Instance.IsFight() && !Sys_Role.Instance.isCrossSrv && !Sys_FamilyResBattle.Instance.InFamilyBattle)
            {
                if (!isHide)
                {
                    int index = GetCurIndexByCurRainNum();
                    if (GetCurAwardByIndex(index) < curCSVRainData.Limit_Max[index] && curCSVRainData.Limit_Max[index] != 0)
                    {
                        isStart = true;
                        UIManager.OpenUI(EUIID.UI_RedEnvelopeRain_Main);
                    }
                }
            }
        }
        public void RainStop()
        {
            isStart = false;
            UIManager.CloseUI(EUIID.UI_RedEnvelopeRain_Main);
        }
        /// <summary>
        /// 取消所有计时器
        /// </summary>
        private void CaneelAllTimer()
        {
            intrevalTime?.Cancel();
            startTimer?.Cancel();
            endTimer?.Cancel();
            bottomStartTimer?.Cancel();
            rollingStartTimer?.Cancel();
            rollingTimer?.Cancel();
            activityEndTimer?.Cancel();
            rainStartHint?.Cancel();
        }
        public DateTime GetServerDateTime()
        {
            return  TimeManager.START_TIME.AddSeconds(TimeManager.GetServerTime());
        }
        /// <summary>
        /// 活动是否激活
        /// </summary>
        /// <returns></returns>
        public bool ActivityIsOpen(bool isCheckCurRainState = true)
        {
            if (CheckRainActivityIsAlive() && Sys_FunctionOpen.Instance.IsOpen(52001) && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(204))
            {
                if (isCheckCurRainState)
                    CheckRainStartCondition();
                return true;
            }
            return false;
        }
        public void CheckRedEnvelopeRainFaceIsShow()
        {
            if (isShowFaceView)
            {
                if (ActivityIsOpen(false))
                {
                    if (isHaveBottom)
                    {
                        UIManager.OpenUI(EUIID.UI_RedEnvelopeRain_Face);
                    }
                }
                isShowFaceView = false;
            }
        }
        /// <summary>
        /// 获取每天第一场活动剩余时间
        /// </summary>
        public double GetFirstEveryDayResidueTime()
        {
            DateTime serverTime = TimeManager.GetDateTime(TimeManager.GetServerTime());
            DateTime firstLocalTime = firstEveryDayDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            TimeSpan span = DiffDateTime(firstLocalTime, serverTime);
            return span.TotalSeconds;
        }
        /// <summary>
        /// 获取每天最后一场剩余时间
        /// </summary>
        /// <returns></returns>
        public double GetLastEveryDayResidueTime()
        {
            DateTime serverTime = TimeManager.GetDateTime(TimeManager.GetServerTime());
            DateTime lastLocalTime = lastEveryDayDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            TimeSpan span = DiffDateTime(lastLocalTime, serverTime);
            return span.TotalSeconds;
        }
        /// <summary>
        /// 关闭所有预告
        /// </summary>
        private void CloseAllAdvance()
        {
            isHaveRolling = false;
            SetRollingData();
        }
        /// <summary>
        /// 获取两个DateTime时间差
        /// </summary>
        /// <param name="time1"></param>
        /// <param name="time2"></param>
        /// <returns></returns>
        private TimeSpan DiffDateTime(DateTime time1, DateTime time2)
        {
            TimeSpan span = new TimeSpan(time1.Ticks - time2.Ticks);
            return span;
        }
        /// <summary>
        /// 检测当前是否正在进行红包雨
        /// </summary>
        public bool CheckCurRainIsUnderway()
        {
            bool isUnderway = false;
            CheckCurRainNum();
            if (!isBlank && curRainNum != -1 && curRainNum != -2)
            {
                isUnderway = true;
            }
            return isUnderway;
        }
        /// <summary>
        /// 检测当天红包雨进行到第几轮
        /// </summary>
        public void CheckCurRainNum()
        {
            if (CheckRainActivityIsAlive())
            {
                if (isActivityDay)
                {
                    if (GetFirstEveryDayResidueTime() > 0)
                    {
                        isBlank = true;
                        curRainNum = -1;
                        return;
                    }
                    if (GetLastEveryDayResidueTime() <= 0 || CheckCurDayActivityIsOver())
                    {
                        isBlank = true;
                        curRainNum = -2;
                        return;
                    }
                    for (int i = 0; i < curCSVRainData.Envelop_Begin.Count; i++)
                    {
                        int startSecond = GetResidueSecond(true, i);
                        int endSecond = GetResidueSecond(false, i);
                        if ((startSecond <= 0 && endSecond > 0) || (startSecond <= 0 && endSecond <= 0))
                        {
                            curRainNum = i + 1;
                        }
                    }
                    isBlank = true;
                    for (int i = 0; i < curCSVRainData.Envelop_Begin.Count; i++)
                    {
                        int startSecond = GetResidueSecond(true, i);
                        int endSecond = GetResidueSecond(false, i);
                        if (startSecond <= 0 && endSecond > 0)
                        {
                            isBlank = false;
                            break;
                        }
                    }
                }
                else
                {
                    isBlank = true;
                    curRainNum = -1;
                }
            }
            else
            {
                isBlank = true;
                curRainNum = -1;
            }
        }
        /// <summary>
        /// 获取当天或指定场数剩余时间
        /// </summary>
        /// <param name="isStart">true指开始，false指结束</param>
        /// <param name="selectedIndex">指定索引</param>
        /// <returns></returns>
        public int GetResidueSecond(bool isStart, int selectedIndex = -1)
        {
            if (curCSVRainData == null)
                return 0;
            DateTime serverTime = GetServerDateTime();
            DateTime localTime = TimeManager.GetDateTime(TimeManager.GetServerTime());
            int index = curRainNum == -1 ? 0 : (curRainNum >= curCSVRainData.Envelop_Begin.Count || curRainNum == -2) ? curCSVRainData.Envelop_Begin.Count - 1 : curRainNum - 1;
            if (selectedIndex != -1)
            {
                index = (selectedIndex >= curCSVRainData.Envelop_Begin.Count || selectedIndex == -2) ? curCSVRainData.Envelop_Begin.Count - 1 : selectedIndex;
            }
            List<uint> dataList = curCSVRainData.Envelop_Begin[index];
            int day;
            if (isActivityDay)
            {
                if (curDay != curCSVRainData.Activity_Date)
                    day = firstDateTime.Subtract(TimeSpan.FromDays(-curCSVRainData.Activity_Date + 1)).Day;
                else
                    day = serverTime.Day;
            }
            else
                day = firstDateTime.Day;
            DateTime curDateTime = new DateTime(serverTime.Year, serverTime.Month, day, (int)dataList[0], (int)dataList[1], 0);
            DateTime curLocalTime = curDateTime.Subtract(TimeSpan.FromSeconds(TimeManager.TimeZoneOffset)).ToLocalTime();
            TimeSpan span = DiffDateTime(curLocalTime, localTime);
            if (isStart)
                return (int)(span.TotalSeconds);
            else
                return (int)(span.TotalSeconds + curCSVRainData.Duration_Second[index]);
        }
        public int GetCurActivityDay()
        {
            DateTime dateTime = GetServerDateTime();
            int day = isActivityDay ? dateTime.Day : firstDateTime.Day;
            return day;
        }
        /// <summary>
        /// 检查是否是默认入口
        /// </summary>
        /// <returns></returns>
        public bool CheckIsDefaultRedEnvelopeRain()
        {
            bool isDefault = true;
            //if (Sys_ActivityTopic.Instance.CommonActivityTimeDictionary.TryGetValue((uint)EActivityTopic.RedEnvelopeRain,out List<uint> listData))
            //{
            //    var tid = listData[2];
            //    CSVActivityUiJump.Data data = CSVActivityUiJump.Instance.GetConfData(tid);
            //    if (data != null && curCSVActivityRulerData != null)
            //    {
            //        if (curCSVActivityRulerData.id == data.UiParam)
            //            isDefault = false;
            //    }
            //}
            return isDefault;
        }
        /// <summary>
        /// 获取活动第一天数据
        /// </summary>
        /// <returns></returns>
        public CSVRedEnvelopRain.Data GetFirstDayRedEnvelopRainData()
        {
            if (curCSVRainData != null)
            {
                if (rainDataDic.ContainsKey(curCSVRainData.Activity_Id))
                    return rainDataDic[curCSVRainData.Activity_Id][0];
            }
            return null;
        }
        #endregion
    }
    public class EnvelopeData
    {
        public float posX;//起始位置x坐标
        public float scale;//缩放比例
        public RedEnvelopeQuality quality;//红包品质 普通红包|金红包
        public ERedEnvelopeRarity rarity;//红包稀有度 普通|稀有
        public uint dropId = 2;//掉落id
    }
    public class RedEnvelopeGetRecordData
    {
        public struct ItemData
        {
            public uint id;
            public uint count;
        }
        public uint time = 1;//获取时间
        public List<ItemData> itemList=new List<ItemData>();//掉落id
        public string content;

        public void Init()
        {
            DateTime dateTime = TimeManager.GetDateTime(time);
            string timeStr = dateTime.ToString("HH:mm:ss");
            string str = GetDropItemStr();
            content = LanguageHelper.GetTextContent(1001912, timeStr, str);
        }
        public string GetDropItemStr()
        {
            System.Text.StringBuilder itemStr = new System.Text.StringBuilder();
            if (itemList.Count > 0)
            {
                for (int i = 0; i < itemList.Count; i++)
                {
                    CSVItem.Data data = CSVItem.Instance.GetConfData(itemList[i].id);
                    string itemName = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(data.name_id), TextHelper.GetQuailtyLangId(data.quality));
                    string str = LanguageHelper.GetTextContent(11904, itemName, itemList[i].count.ToString());
                    itemStr.Append(str);
                    if (i < itemList.Count - 1)
                    {
                        itemStr.Append("、");
                    }
                }
            }
            return itemStr.ToString();
        }
    }
    public class RedEnvelopeTakeAwardData
    {
        public uint infoId = 1; //红包表中的id
        public List<RedEnvelopeGetRecordData.ItemData> itemList=new List<RedEnvelopeGetRecordData.ItemData>();//掉落id
        public uint time = 3;//获取时间
    }
    public class ActivityPreviewData
    {
        public int index;//第几轮
        public List<uint> startTime;//开始时间
        public uint durationTime;//持续时间
        public EActivityState state;//状态
    }
}