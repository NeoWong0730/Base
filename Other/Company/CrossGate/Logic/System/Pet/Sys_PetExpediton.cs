using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary>宠物探险</summary>
    public class Sys_PetExpediton : SystemModuleBase<Sys_PetExpediton>
    {
        /// <summary> 等级百分比 </summary>
        readonly uint MaxLvPresent = 40; 
        /// <summary> 评分百分比 </summary>
        readonly uint MaxScorePresent = 100;
        /// <summary> 元素百分比 </summary>
        readonly uint MaxElePresent = 40; 
        /// <summary> 种族百分比 </summary>
        readonly uint MaxRacePresent = 20;

        public enum EEvents : int
        {
            /// <summary>
            /// 刷新宠物探险数据
            /// </summary>
            OnPetExpeditonDataUpdate,
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        /// <summary> 当前活动ID </summary>
        public uint curActivityId { get; private set; } = 0;
        /// <summary> 当前冒险分数 </summary>
        public uint curScore { get; private set; } = 48;
        public uint curStartTime { get; private set; } = 0;
        public uint curEndTime { get; private set; } = 0;
        public uint scoreRewardState { get; private set; } = 0;
        /// <summary> 可开始的任务列表 </summary>
        private List<uint> listUsableTask = new List<uint>();
        /// <summary> 进行中/已完成的任务列表 </summary>
        private Dictionary<uint, ExploreTaskInfo> dictUnderwayTask = new Dictionary<uint, ExploreTaskInfo>();
        /// <summary> 派遣中的宠物Uid </summary>
        private List<uint> listUnderwayPetUid = new List<uint>();


        /// <summary> 当前可用派遣选中的宠物UID列表 </summary>
        public List<uint> curSelectedPetUid = new List<uint>();

        private Timer timer;

        #region 系统函数
        public override void Init()
        {
            RegisterEvents(true);
        }

        public override void Dispose()
        {
            timer?.Cancel();
            RegisterEvents(false);
        }

        public override void OnLogin()
        {
            InitActivityData();
            //ReqPetExploreInfo();
        }
        public override void OnLogout()
        {
            timer?.Cancel();
            listUsableTask.Clear();
            dictUnderwayTask.Clear();
            listUnderwayPetUid.Clear();
            curScore = 0;
            scoreRewardState = 0;
        }
        #endregion

        #region 服务器消息
        /// <summary> 请求宠物探险数据 </summary>
        public void ReqPetExploreInfo()
        {
            if (CheckPetExpeditonIsOpen())
            {
                CmdPetExploreInfoReq req = new CmdPetExploreInfoReq();
                req.ActId = curActivityId;
                NetClient.Instance.SendMessage((ushort)CmdPetExplore.InfoReq, req);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "宠物探险活动未开启");
            }
        }
        public void OnPetExploreInfoNtf(NetMsg msg)
        {
            CmdPetExploreInfoNtf ntf = NetMsgUtil.Deserialize<CmdPetExploreInfoNtf>(CmdPetExploreInfoNtf.Parser, msg);
            curActivityId = ntf.ActId;
            curScore = ntf.ExplorePoint;
            scoreRewardState = ntf.PointAwardState;
            listUsableTask.Clear();
            listUsableTask.AddRange(ntf.ToStartTask);
            dictUnderwayTask.Clear();
            listUnderwayPetUid.Clear();
            for (int i = 0; i < ntf.TaskInfos.Count; i++)
            {
                var curTaskInfo = ntf.TaskInfos[i];
                dictUnderwayTask.Add(curTaskInfo.TaskId, curTaskInfo);
                if (curTaskInfo.AwardState == 0)
                {
                    for (int j = 0; j < curTaskInfo.PetInfos.Count; j++)
                    {
                        listUnderwayPetUid.Add(curTaskInfo.PetInfos[j].PetUid);
                    }
                }
            }
            //Debug.Log("宠物探险数据同步 " + curActivityId + "|" + curScore + "|" + scoreRewardState);
            eventEmitter.Trigger(EEvents.OnPetExpeditonDataUpdate);
        }
        /// <summary>
        /// 请求开始探险
        /// </summary>
        public void ReqPetExploreStart(uint taskId)
        {
            CmdPetExploreStartReq req = new CmdPetExploreStartReq();
            req.ActId = curActivityId;
            req.TaskId = taskId;
            req.PetList.AddRange(curSelectedPetUid);
            NetClient.Instance.SendMessage((ushort)CmdPetExplore.StartReq, req);
        }
        public void OnPetExploreStartRes(NetMsg msg)
        {
            CmdPetExploreStartRes ntf = NetMsgUtil.Deserialize<CmdPetExploreStartRes>(CmdPetExploreStartRes.Parser, msg);
            var curTaskInfo = ntf.TaskInfo;
            dictUnderwayTask.Add(ntf.TaskInfo.TaskId, curTaskInfo);
            for (int j = 0; j < curTaskInfo.PetInfos.Count; j++)
            {
                listUnderwayPetUid.Add(curTaskInfo.PetInfos[j].PetUid);
            }
            curSelectedPetUid.Clear();//探险请求成功后清空一下当前选中的宠物UId
            eventEmitter.Trigger(EEvents.OnPetExpeditonDataUpdate);
        }
        /// <summary>
        /// 请求领取探险完成奖励
        /// </summary>
        public void ReqPetExploreFinishGetAwardReq(uint taskId)
        {
            CmdPetExploreFinishGetAwardReq req = new CmdPetExploreFinishGetAwardReq();
            req.ActId = curActivityId;
            req.TaskId = taskId;
            NetClient.Instance.SendMessage((ushort)CmdPetExplore.FinishGetAwardReq, req);
        }
        public void OnPetExploreFinishGetAwardRes(NetMsg msg)
        {
            CmdPetExploreFinishGetAwardRes ntf = NetMsgUtil.Deserialize<CmdPetExploreFinishGetAwardRes>(CmdPetExploreFinishGetAwardRes.Parser, msg);
            curScore = ntf.ExplorePoint;
            PetExpeditionResultParam param = new PetExpeditionResultParam
            {
                taskId = ntf.TaskId,
                rewardType = ntf.AwardState,
                rewards = new List<SimpleItem>()
            };
            param.rewards.AddRange(ntf.Items);
            UIManager.OpenUI(EUIID.UI_PetExpedition_Result, false, param);
            ///更新派遣宠物Uidl列表
            if(dictUnderwayTask.TryGetValue(ntf.TaskId, out ExploreTaskInfo taskInfo))
            {
                taskInfo.AwardState = ntf.AwardState;
                List<uint> newPetUid = new List<uint>();
                for (int i = 0; i < listUnderwayPetUid.Count; i++)
                {
                    bool flag = false;
                    uint curUid = listUnderwayPetUid[i];
                    for (int j = 0; j < taskInfo.PetInfos.Count; j++)
                    {
                        if(curUid == taskInfo.PetInfos[j].PetUid)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        newPetUid.Add(curUid);
                    }
                }
                listUnderwayPetUid = newPetUid;
            }
            eventEmitter.Trigger(EEvents.OnPetExpeditonDataUpdate);
        }
        /// <summary>
        /// 请求领取冒险点数奖励
        /// </summary>
        public void ReqPetExploreGetPointAwardReq(uint index)
        {
            CmdPetExploreGetPointAwardReq req = new CmdPetExploreGetPointAwardReq();
            req.ActId = curActivityId;
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdPetExplore.GetPointAwardReq, req);
        }
        public void OnPetExploreGetPointAwardRes(NetMsg msg)
        {
            CmdPetExploreGetPointAwardRes ntf = NetMsgUtil.Deserialize<CmdPetExploreGetPointAwardRes>(CmdPetExploreGetPointAwardRes.Parser, msg);
            //Debug.Log(scoreRewardState + "|" + ntf.ScoreAwardState);
            scoreRewardState = ntf.ScoreAwardState;
            eventEmitter.Trigger(EEvents.OnPetExpeditonDataUpdate);
        }
        #endregion

        #region func
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);

            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPetExplore.InfoNtf, OnPetExploreInfoNtf, CmdPetExploreInfoNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPetExplore.StartReq, (ushort)CmdPetExplore.StartRes, OnPetExploreStartRes, CmdPetExploreStartRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPetExplore.FinishGetAwardReq, (ushort)CmdPetExplore.FinishGetAwardRes, OnPetExploreFinishGetAwardRes, CmdPetExploreFinishGetAwardRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPetExplore.GetPointAwardReq, (ushort)CmdPetExplore.GetPointAwardRes, OnPetExploreGetPointAwardRes, CmdPetExploreGetPointAwardRes.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetExplore.InfoNtf, OnPetExploreInfoNtf);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetExplore.StartRes, OnPetExploreStartRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetExplore.FinishGetAwardRes, OnPetExploreFinishGetAwardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPetExplore.GetPointAwardRes, OnPetExploreGetPointAwardRes);
            }
        }
        /// <summary>
        /// 判断宠物派遣是否开启
        /// </summary>
        public bool CheckPetExpeditonIsOpen()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            bool isActivityTime = nowTime >= curStartTime && nowTime <= curEndTime;
            return isActivityTime && Sys_FunctionOpen.Instance.IsOpen(52301) && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(208);
        }
        /// <summary>
        /// 初始化活动数据,获取当期活动id，启动更新倒计时
        /// </summary>
        private void InitActivityData()
        {
            timer?.Cancel();
            var activityDatas = CSVOperationalActivityRuler.Instance.GetAll();
            var nowTime = Sys_Time.Instance.GetServerTime();
            uint nextTime = 0;
            for (int i = 0; i < activityDatas.Count; i++)
            {
                var activityData = activityDatas[i];
                if (activityData.Product_Type == 6 && activityData.Activity_Switch == 1)
                {
                    uint startTime=0;
                    uint endTime=0;
                    if (activityData.Activity_Type == 3)//开服
                    {
                        startTime = Sys_Role.Instance.openServiceGameTime;
                        endTime = startTime + (activityData.Duration_Day) * 86400;
                    }
                    else if (activityData.Activity_Type == 2)//限时
                    {
                        startTime = activityData.Begining_Date + (uint)TimeManager.TimeZoneOffset;
                        endTime = startTime + (activityData.Duration_Day) * 86400;
                    }
                    //Debug.Log("宠物活动初始化 " + startTime + "|" + nowTime + "|" + endTime);
                    if (nowTime >= startTime && nowTime <= endTime)
                    {
                        curActivityId = activityData.id;
                        curStartTime = startTime;
                        curEndTime = endTime;
                        return;
                    }
                    if(nowTime < startTime)
                    {
                        if (nextTime <= 0)
                        {
                            nextTime = startTime;
                        }
                        else
                        {
                            //取最快要开始的活动时间
                            nextTime = startTime < nextTime ? startTime : nextTime;
                        }
                    }
                }
            }
            //当前不在活动时间段内，启动最近一次的倒计时
            if(nextTime > 0)
            {
                uint cd = nextTime - nowTime;
                timer = Timer.Register(cd, InitActivityData, null, false, false);
            }
        }
        /// <summary>
        /// 检测宠物探险总红点
        /// </summary>
        public bool CheckAllRedPoint()
        {
            return CheckTaskFinishRedPoint() || CheckScoreRewardRedPoint();
        }
        /// <summary>
        /// 检测宠物探险 任务完成红点
        /// </summary>
        public bool CheckTaskFinishRedPoint()
        {
            foreach (var taskInfo in dictUnderwayTask.Values)
            {
                if (taskInfo.AwardState == 0)
                {
                    uint nowtime = Sys_Time.Instance.GetServerTime();
                    if (taskInfo.EndTick < nowtime)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 检测宠物探险 积分奖励红点
        /// </summary>
        public bool CheckScoreRewardRedPoint()
        {
            CSVPetexploreReward.Data activityData = CSVPetexploreReward.Instance.GetConfData(curActivityId);
            if (activityData != null)
            {
                for (int i = 0; i < activityData.Date.Count; i++)
                {
                    uint cellScore = activityData.Date[i];
                    bool canGet = curScore >= cellScore;
                    bool isGet = CheckScoreRewardIsGet(i);
                    if(canGet && !isGet)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 根据积分和奖励分段，计算进度条的value
        /// </summary>
        public float GetSliderValue()
        {
            uint score = curScore;
            uint activityId = curActivityId;
            CSVPetexploreReward.Data activityData = CSVPetexploreReward.Instance.GetConfData(activityId);
            if (activityData != null)
            {
                List<uint> scoreList = activityData.Date;
                if (score >= scoreList[scoreList.Count - 1])
                {
                    return 1;
                }
                int curIndex = 0;
                for (int i = 0; i < scoreList.Count; i++)
                {
                    if (score < scoreList[i])
                    {
                        curIndex = i;
                        break;
                    }
                }
                uint lastScore = curIndex > 0 ? scoreList[curIndex - 1] : 0;
                float value = 1f / scoreList.Count * (curIndex + (float)(score - lastScore) / (scoreList[curIndex] - lastScore));
                return value;
            }
            return 0;
        }
        /// <summary>
        /// 根据下标，检测积分奖励是否已经领取
        /// </summary>
        public bool CheckScoreRewardIsGet(int index)
        {
            return (scoreRewardState >> index & 1) == 1;
        }
        /// <summary>
        /// 根据下标，获取当前阶段宝箱图标
        /// </summary>
        public uint GetScoreRewardImageIconId(int index, bool isGet)
        {
            int lv = 1;
            uint activityId = curActivityId;
            CSVPetexploreReward.Data activityData = CSVPetexploreReward.Instance.GetConfData(activityId);
            if (activityData != null)
            {
                int rewardNum = activityData.Date.Count;
                if(rewardNum <= 4)
                {
                    lv = index + 1;
                }
                else
                {
                    lv = 4 - rewardNum + index + 1;
                    lv = lv > 0 ? lv : 1;
                }
            }
            if (lv == 1)
            {
                return isGet ? 994815u : 994814u;
            }
            else if (lv == 2)
            {
                return isGet ? 994817u : 994816u;
            }
            else if (lv == 3)
            {
                return isGet ? 994819u : 994818u;
            }
            else if (lv == 4)
            {
                return isGet ? 994821u : 994820u;
            }
            return 0;
        }
        /// <summary>
        /// 根据任务难度，获取当前任务列表背景图ID
        /// </summary>
        public uint GetTaskCellBgImageIconId(uint diff)
        {
            if (diff == 1)
            {
                return 994804;
            }
            else if (diff == 2)
            {
                return 994805;
            }
            else if (diff == 3)
            {
                return 994806;
            }
            else if (diff == 4)
            {
                return 994807;
            }
            else if (diff == 5)
            {
                return 994808;
            }
            return 0;
        }
        /// <summary>
        /// 根据任务难度，获取当前任务详情背景图ID
        /// </summary>
        public uint GetTaskInfoBgImageIconId(uint diff)
        {
            if (diff == 1)
            {
                return 994809;
            }
            else if (diff == 2)
            {
                return 994810;
            }
            else if (diff == 3)
            {
                return 994811;
            }
            else if (diff == 4)
            {
                return 994812;
            }
            else if (diff == 5)
            {
                return 994813;
            }
            return 0;
        }
        /// <summary>
        /// 根据任务难度，获取当前任务背景标题难度文本
        /// </summary>
        public string GetTaskBgTitleText(uint diff)
        {
            uint languaId = 0;
            if (diff == 1)
            {
                languaId = 2025601;
            }
            else if (diff == 2)
            {
                languaId = 2025602;
            }
            else if (diff == 3)
            {
                languaId = 2025603;
            }
            else if (diff == 4)
            {
                languaId = 2025604;
            }
            else if (diff == 5)
            {
                languaId = 2025605;
            }
            return LanguageHelper.GetTextContent(languaId);
        }
        /// <summary>
        /// 根据任务难度，获取当前字体颜色富文本
        /// </summary>
        public string GetTaskRichText(uint diff,string str)
        {
            uint languaId = 0;
            if (diff == 1)
            {
                languaId = 2025648;
            }
            else if (diff == 2)
            {
                languaId = 2025649;
            }
            else if (diff == 3)
            {
                languaId = 2025650;
            }
            else if (diff == 4)
            {
                languaId = 2025651;
            }
            else if (diff == 5)
            {
                languaId = 2025652;
            }
            return LanguageHelper.GetTextContent(languaId, str);
        }
        /// <summary>
        /// 根据秒数返回任务总时长文本
        /// </summary>
        public string GetTaskAllTimeText(uint second)
        {
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            uint hour = second / 3600;
            uint minute = (second % 3600) / 60;
            if (hour > 0)
            {
                stringBuilder.Append(LanguageHelper.GetTextContent(10155, hour.ToString()));//{0}小时
            }
            if (minute > 0)
            {
                stringBuilder.Append(LanguageHelper.GetTextContent(10156, minute.ToString()));//{0}分钟
            }
            return StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
        }
        /// <summary>
        /// 获取任务ID列表
        /// </summary>
        public List<uint> GetTaskIds(bool isUnderway = false)
        {
            List<uint> ids = new List<uint>();
            if (isUnderway)
            {
                foreach (var taskInfo in dictUnderwayTask.Values)
                {
                    if (taskInfo.AwardState == 0)
                    {
                        //完成时间从小到大排序
                        if (ids.Count > 0)
                        {
                            var flag = false;
                            for (int j = 0; j < ids.Count; j++)
                            {
                                if (taskInfo.EndTick < dictUnderwayTask[ids[j]].EndTick)
                                {
                                    ids.Insert(j, taskInfo.TaskId);
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                ids.Add(taskInfo.TaskId);
                            }
                        }
                        else
                        {
                            ids.Add(taskInfo.TaskId);
                        }
                    }
                }
            }
            else
            {
                uint maxDay = 0;
                //可用
                for (int i = 0; i < listUsableTask.Count; i++)
                {
                    var taskId = listUsableTask[i];
                    if (!dictUnderwayTask.ContainsKey(taskId))
                    {
                        var curDay = taskId % 1000;
                        if (maxDay < curDay)
                        {
                            ids.Clear();
                            maxDay = curDay;
                            ids.Add(taskId);
                        }
                        else if (maxDay == curDay)
                        {
                            ids.Add(taskId);
                        }
                    }
                }
            }
            return ids;
        }
        /// <summary>
        /// 获取活动开放日期文本
        /// </summary>
        public string GetActivityDateText()
        {
            System.DateTime startDateTime = TimeManager.START_TIME.AddSeconds(curStartTime);
            System.DateTime endDateTime = TimeManager.START_TIME.AddSeconds(curEndTime);
            return LanguageHelper.GetTextContent(2025624, startDateTime.Year.ToString(), startDateTime.Month.ToString(), startDateTime.Day.ToString(), startDateTime.Hour.ToString("D2"), startDateTime.Minute.ToString("D2"), endDateTime.Year.ToString(), endDateTime.Month.ToString(), endDateTime.Day.ToString(), endDateTime.Hour.ToString("D2"), endDateTime.Minute.ToString("D2"));//活动时间：{0}年{1}月{2}日{3}:{4}-{5}年{6}月{7}日{8}:{9}
        }

        /// <summary>
        /// 获取有效的宠物列表
        /// </summary>
        public List<ClientPet> GetValidPetList()
        {
            List<ClientPet> showList = new List<ClientPet>();
            var petsList = Sys_Pet.Instance.petsList;
            for (int i = 0; i < petsList.Count; i++)
            {
                bool flag = false;
                for (int j = 0; j < listUnderwayPetUid.Count; j++)
                {
                    if (listUnderwayPetUid[j] == petsList[i].petUnit.Uid)
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                {
                    showList.Add(petsList[i]);
                }
            }
            return showList;
        }
        /// <summary>
        /// 根据Uid获取宠物ClienPet数据
        /// </summary>
        public ClientPet GetClientPetBuyUid(uint uid)
        {
            var petsList = Sys_Pet.Instance.petsList;
            for (int i = 0; i < petsList.Count; i++)
            {
                if (petsList[i].petUnit.Uid == uid)
                {
                    return petsList[i];
                }
            }
            return null;
        }
        /// <summary>
        /// 检测当前宠物是否已经被选中
        /// </summary>
        public bool CheckPetIsSelected(uint uid)
        {
            for (int i = 0; i < curSelectedPetUid.Count; i++)
            {
                if(uid == curSelectedPetUid[i])
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary> 获取进行中/已完成的任务数据 </summary>
        public ExploreTaskInfo GetExploreTaskInfo(uint taskId)
        {
            if (dictUnderwayTask.TryGetValue(taskId, out ExploreTaskInfo taskInfo))
            {
                return taskInfo;
            }
            return null;
        }
        /// <summary>
        /// 自动派遣
        /// </summary>
        public void AutoSetExplorePetList()
        {
            if (curSelectedPetUid.Count < 3)
            {
                var listCanUsePet = Sys_PetExpediton.Instance.GetValidPetList();
                var listValidPet = new List<ClientPet>();
                for (int i = 0; i < listCanUsePet.Count; i++)
                {
                    if (!curSelectedPetUid.Contains(listCanUsePet[i].petUnit.Uid))
                    {
                        listValidPet.Add(listCanUsePet[i]);
                    }
                }
                if(listValidPet.Count > 0)
                {
                    //排序
                    for (int i = 0; i < listValidPet.Count - 1; i++)
                    {
                        bool flag = false;
                        for (int j = 0; j < listValidPet.Count - i - 1; j++)
                        {
                            if(listValidPet[j].petUnit.SimpleInfo.Score < listValidPet[j+1].petUnit.SimpleInfo.Score)
                            {
                                var temp = listValidPet[j];
                                listValidPet[j] = listValidPet[j + 1];
                                listValidPet[j + 1] = temp;
                                flag = true;
                            }
                        }
                        if (!flag)
                        {
                            break;
                        }
                    }
                    int needNum = 3 - curSelectedPetUid.Count;
                    for (int i = 0; i < listValidPet.Count; i++)
                    {
                        curSelectedPetUid.Add(listValidPet[i].petUnit.Uid);
                        needNum--;
                        if (needNum <= 0)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025655));//没有可派遣的宠物
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025654));//探险小队已满员
            }
        }
        #endregion

        #region 加成概率计算逻辑
        public string GetTaskAttrPresentText(uint nowPre, uint maxPre)
        {
            uint languageId;
            if (nowPre >= maxPre)
            {
                //max
                languageId = 2025615;
            }
            else if (nowPre > (float)maxPre * 0.5f)
            {
                //绿色
                languageId = 2025616;
            }
            else if (nowPre > (float)maxPre * 0.3f)
            {
                //橙色
                languageId = 2025617;
            }
            else
            {
                //红色
                languageId = 2025618;
            }
            return LanguageHelper.GetTextContent(languageId, nowPre.ToString());
        }
        /// <summary> 获取强度加成百分比文本 </summary>
        public string GetStrPresentText(uint taskId, bool isUndeway)
        {
            uint nowPre = 0;
            uint MaxPre = MaxLvPresent + MaxScorePresent;
            if (isUndeway)
            {
                var taskInfo = Sys_PetExpediton.Instance.GetExploreTaskInfo(taskId);
                if (taskInfo != null)
                {
                    nowPre = taskInfo.LevelSuc + taskInfo.ScoreSuc;
                }
            }
            else
            {
                nowPre = GetStrPresentNum(taskId);

            }
            return GetTaskAttrPresentText(nowPre, MaxPre);
        }
        /// <summary> 计算强度加成百分比 </summary>
        public uint GetStrPresentNum(uint taskId)
        {
            var petTaskData = CSVPetexploreTaskGroup.Instance.GetConfData(taskId / 1000);
            if (petTaskData != null)
            {
                float sumLv = 0;
                float sumScore = 0;
                for (int i = 0; i < curSelectedPetUid.Count; i++)
                {
                    var clientPetData = Sys_PetExpediton.Instance.GetClientPetBuyUid(curSelectedPetUid[i]);
                    sumLv += clientPetData.petUnit.SimpleInfo.Level;
                    sumScore += clientPetData.petUnit.SimpleInfo.Score;
                }
                float lvPre = sumLv / (3 * petTaskData.Level_Condition);
                lvPre = lvPre > 1 ? 1 : lvPre;
                float scorePre = sumScore / (3 * petTaskData.Score_Condition);
                scorePre = scorePre > 1 ? 1 : scorePre;
                return (uint)(lvPre * MaxLvPresent) + (uint)(scorePre * MaxScorePresent);
            }
            return 0;
        }
        /// <summary> 获取元素加成百分比文本 </summary>
        public string GetElementPresentText(uint taskId, bool isUndeway)
        {
            uint nowPre = 0;
            uint MaxPre = MaxElePresent;
            if (isUndeway)
            {
                var taskInfo = Sys_PetExpediton.Instance.GetExploreTaskInfo(taskId);
                if (taskInfo != null)
                {
                    nowPre = taskInfo.ElementSuc;
                }
            }
            else
            {
                nowPre = GetElePresentNum(taskId);

            }
            return GetTaskAttrPresentText(nowPre, MaxPre);
        }
        /// <summary> 计算元素加成百分比 </summary>
        public uint GetElePresentNum(uint taskId)
        {
            var petTaskData = CSVPetexploreTaskGroup.Instance.GetConfData(taskId / 1000);
            if (petTaskData != null)
            {
                List<uint> listSumAttr = new List<uint>() { 0, 0, 0, 0 };
                for (int i = 0; i < curSelectedPetUid.Count; i++)
                {
                    var clientPetData = Sys_PetExpediton.Instance.GetClientPetBuyUid(curSelectedPetUid[i]);
                    var csvPetData = clientPetData.petData;
                    for (int j = 0; j < csvPetData.init_attr.Count; j++)
                    {
                        var attr = csvPetData.init_attr[j];
                        if (attr[0] <= listSumAttr.Count)
                        {
                            uint param = listSumAttr[(int)attr[0] - 1];
                            param += attr[1];
                            listSumAttr[(int)attr[0] - 1] = param;
                        }
                    }
                }
                float sumPre = 0;//属性百分比总和
                uint attrNum = 0;//有效属性数
                for (int i = 0; i < petTaskData.Attr_Condition.Count; i++)
                {
                    var attrCon = petTaskData.Attr_Condition[i];
                    if (attrCon[1] > 0)
                    {
                        var curPre = (float)listSumAttr[(int)attrCon[0] - 1] / (3 * attrCon[1]);
                        curPre = curPre > 1 ? 1 : curPre;
                        sumPre += curPre;
                        attrNum++;
                    }
                }
                return (uint)(sumPre / attrNum * MaxElePresent);
            }
            return 0;
        }
        /// <summary> 获取种族加成百分比文本 </summary>
        public string GetRacePresentText(uint taskId, bool isUndeway)
        {
            uint nowPre = 0;
            uint MaxPre = MaxRacePresent;
            if (isUndeway)
            {
                var taskInfo = Sys_PetExpediton.Instance.GetExploreTaskInfo(taskId);
                if (taskInfo != null)
                {
                    nowPre = taskInfo.RaceSuc;
                }
            }
            else
            {
                nowPre = GetRacePresentNum(taskId);

            }
            return GetTaskAttrPresentText(nowPre, MaxPre);
        }
        /// <summary> 计算种族加成百分比 </summary>
        public uint GetRacePresentNum(uint taskId)
        {
            var petTaskData = CSVPetexploreTaskGroup.Instance.GetConfData(taskId / 1000);
            if (petTaskData != null)
            {
                float sumNum = 0;
                for (int i = 0; i < curSelectedPetUid.Count; i++)
                {
                    var clientPetData = Sys_PetExpediton.Instance.GetClientPetBuyUid(curSelectedPetUid[i]);
                    if (clientPetData.petData.race == petTaskData.Race_Condition)
                    {
                        sumNum++;
                    }
                }
                float racePre = (float)sumNum / 3;
                racePre = racePre > 1 ? 1 : racePre;
                return (uint)(racePre * MaxRacePresent);
            }
            return 0;
        }
        /// <summary> 获取成功率百分比文本 </summary>
        public string GetSuccessPresentText(uint nowPre)
        {
            uint languageId;
            if (nowPre > 80)
            {
                //绿色
                languageId = 2025619;
            }
            else if (nowPre > 60)
            {
                //橙色
                languageId = 2025620;
            }
            else
            {
                //红色
                languageId = 2025621;
            }
            return LanguageHelper.GetTextContent(languageId, nowPre.ToString());
        }
        /// <summary> 计算总加成百分比 </summary>
        public uint GetAllPresentNum(uint taskId,bool isUndeway)
        {
            if (isUndeway)
            {
                var taskInfo = Sys_PetExpediton.Instance.GetExploreTaskInfo(taskId);
                if (taskInfo != null)
                {
                    return taskInfo.LevelSuc + taskInfo.ScoreSuc + taskInfo.ElementSuc + taskInfo.RaceSuc;
                }
            }
            else
            {
                return GetStrPresentNum(taskId) + GetElePresentNum(taskId) + GetRacePresentNum(taskId);
            }
            return 0;
        }

        /// <summary>
        /// 检测宠物是否在派遣中
        /// </summary>
        public bool CheckPetIsUnderway(uint petUid)
        {
            return listUnderwayPetUid.Contains(petUid);
        }
        #endregion

        #region event
        /// <summary> 服务器时间同步 </summary>
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            InitActivityData();
        }
        #endregion
    }
}
