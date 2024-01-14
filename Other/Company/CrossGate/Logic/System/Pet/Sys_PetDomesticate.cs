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
    public class Sys_PetDomesticate : SystemModuleBase<Sys_PetDomesticate>
    {
        /// <summary> 阶段 </summary>
        public uint Stage { get; private set; }
        /// <summary> 等级 </summary>
        public uint Level { get; private set; }
        /// <summary> 等级 </summary>
        public uint Exp { get; private set; }
        /// <summary> 今日加训次数 </summary>
        public uint ExTaskCount { get; private set; }
        /// <summary> 任务列表 </summary>
        public List<DomesticationTask> listTask = new List<DomesticationTask>();

        private Timer timer;
        private float countDownTime = 0;

        public enum EEvents : int
        {
            /// <summary>
            /// 刷新宠物驯养数据
            /// </summary>
            OnPetDomesticateDataUpdate,
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        #region 系统函数
        public override void Init()
        {
            RegisterEvents(true);
        }

        public override void Dispose()
        {
            RegisterEvents(false);
        }

        public override void OnLogin()
        {
            StartTimer();
        }
        public override void OnLogout()
        {
            Stage = 1;
            Level = 0;
            Exp = 0;
            ExTaskCount = 0;
            listTask.Clear();
            timer?.Cancel();
        }
        #endregion

        #region 服务器消息
        /// <summary> 请求宠物驯养数据 </summary>
        public void ReqGetPetDomesticationInfo()
        {
            CmdPetGetDomesticationReq req = new CmdPetGetDomesticationReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.GetDomesticationReq, req);
        }
        public void OnPetDomesticationInfoRes(NetMsg msg)
        {
            CmdPetGetDomesticationRes ntf = NetMsgUtil.Deserialize<CmdPetGetDomesticationRes>(CmdPetGetDomesticationRes.Parser, msg);
            Stage = ntf.Stage;
            Level = ntf.Level;
            Exp = ntf.Exp;
            ExTaskCount = ntf.ExTaskCount;
            listTask.Clear();
            listTask.AddRange(ntf.TaskList);
            //Debug.Log("OnPetDomesticationInfoRes 获取宠物驯养数据 Stage " + Stage.ToString() + " | Level " + Level.ToString() + " | Exp " + Exp.ToString());

            //for (int i = 0; i < listTask.Count; i++)
            //{
            //    Debug.Log("OnPetDomesticationInfoRes 获取宠物驯养数据 " + i.ToString() + " | TaskId" + listTask[i].InfoId.ToString() + " | endTime" + listTask[i].EndTime.ToString());
            //}

            eventEmitter.Trigger(EEvents.OnPetDomesticateDataUpdate);
        }
        /// <summary> 请求开始驯养 </summary>
        public void ReqStartPetDomestication(uint taskId, uint petUid)
        {
            CmdPetDomesticationStartReq req = new CmdPetDomesticationStartReq();
            req.InfoId = taskId;
            req.PetUid = petUid;
            NetClient.Instance.SendMessage((ushort)CmdPet.DomesticationStartReq, req);
        }
        public void OnPetDomesticationStartRes(NetMsg msg)
        {
            CmdPetDomesticationStartRes ntf = NetMsgUtil.Deserialize<CmdPetDomesticationStartRes>(CmdPetDomesticationStartRes.Parser, msg);
            for (int i = 0; i < listTask.Count; i++)
            {
                if(listTask[i].InfoId == ntf.Task.InfoId)
                {
                    listTask[i] = ntf.Task;
                    break;
                }
            }
            //Debug.Log("OnPetDomesticationStartRes 开始驯养数据 TaskId" + ntf.Task.InfoId.ToString() + " | addition" + ntf.Task.AwardRatio.ToString() + " | endTime" + ntf.Task.EndTime.ToString());
            eventEmitter.Trigger(EEvents.OnPetDomesticateDataUpdate);
        }
        /// <summary> 请求领取驯养奖励</summary>
        public void ReqGetPetDomesticationReward()
        {
            CmdPetDomesticationEndReq req = new CmdPetDomesticationEndReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.DomesticationEndReq, req);
        }
        public void OnGetPetDomesticationRewardRes(NetMsg msg)
        {
            CmdPetDomesticationEndRes ntf = NetMsgUtil.Deserialize<CmdPetDomesticationEndRes>(CmdPetDomesticationEndRes.Parser, msg);
            List<ItemIdCount> listRewards = new List<ItemIdCount>();
            for (int i = listTask.Count-1; i >= 0; i--)
            {
                var curTask = listTask[i];
                for (int j = 0; j < ntf.InfoId.Count; j++)
                {
                    if (curTask.InfoId == ntf.InfoId[j])
                    {
                        var csvTask = CSVDomesticationTask.Instance.GetConfData(curTask.InfoId);
                        uint curCount = (uint)Mathf.Floor(csvTask.reward[(int)curTask.Grade - 1] * curTask.AwardRatio / 100);
                        ItemIdCount item = new ItemIdCount()
                        {
                            id = 37,
                            count = curCount
                        };
                        listRewards.Add(item);
                        listTask.RemoveAt(i);
                    }
                }
            }
            //奖励弹窗
            if (listRewards.Count > 0)
            {
                PetDomesticateResultParam param = new PetDomesticateResultParam()
                {
                    listRewardItems = listRewards
                };
                UIManager.CloseUI(EUIID.UI_PetDomesticateResult);
                UIManager.OpenUI(EUIID.UI_PetDomesticateResult, false, param);
            }
            Stage = ntf.Stage;
            Level = ntf.Level;
            Exp = ntf.Exp;
            //Debug.Log("OnGetPetDomesticationRewardRes 驯养奖励长度 " + ntf.InfoId.Count);
            //for (int i = 0; i < ntf.InfoId.Count; i++)
            //{
            //    Debug.Log("OnGetPetDomesticationRewardRes 领取驯养奖励 " + ntf.InfoId[i].ToString());

            //}
            eventEmitter.Trigger(EEvents.OnPetDomesticateDataUpdate);
        }
        /// <summary> 请求刷新驯养任务</summary>
        public void ReqRefreshPetDomesticationTask()
        {
            CmdPetDomesticationRefreshReq req = new CmdPetDomesticationRefreshReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.DomesticationRefreshReq, req);
        }
        public void OnPetDomesticationRefreshRes(NetMsg msg)
        {
            CmdPetDomesticationRefreshRes ntf = NetMsgUtil.Deserialize<CmdPetDomesticationRefreshRes>(CmdPetDomesticationRefreshRes.Parser, msg);
            for (int i = listTask.Count - 1; i >= 0; i--)
            {
                var curTask = listTask[i];
                for (int j = 0; j < ntf.OldTaskList.Count; j++)
                {
                    if (curTask.InfoId == ntf.OldTaskList[j])
                    {
                        listTask.RemoveAt(i);
                    }
                }
            }
            //string str1 = "";
            //for (int i = 0; i < ntf.OldTaskList.Count; i++)
            //{
            //    str1 += ntf.OldTaskList[i].ToString() + " | ";
            //}
            //Debug.Log("OldTaskList 刷新驯养任务 TaskId " + str1);
            //string str2 = "";
            //for (int i = 0; i < ntf.OldTaskList.Count; i++)
            //{
            //    str2 += ntf.NewTaskList[i].InfoId.ToString() + " | ";
            //}
            //Debug.Log("NewTaskList 刷新驯养任务 TaskId" + str2);
            listTask.AddRange(ntf.NewTaskList);
            eventEmitter.Trigger(EEvents.OnPetDomesticateDataUpdate);
        }
        /// <summary> 请求加训 </summary>
        public void ReqAddPetDomesticationTask()
        {
            CmdPetDomesticationAddReq req = new CmdPetDomesticationAddReq();
            NetClient.Instance.SendMessage((ushort)CmdPet.DomesticationAddReq, req);
        }
        public void OnAddPetDomesticationTaskRes(NetMsg msg)
        {
            CmdPetDomesticationAddRes ntf = NetMsgUtil.Deserialize<CmdPetDomesticationAddRes>(CmdPetDomesticationAddRes.Parser, msg);
            //listTask.Add(ntf.Task);
            //eventEmitter.Trigger(EEvents.OnPetDomesticateDataUpdate);
            ReqGetPetDomesticationInfo();
        }
        public void OnPetDomesticationExpNtf(NetMsg msg)
        {
            CmdPetDomesticationExpNtf ntf = NetMsgUtil.Deserialize<CmdPetDomesticationExpNtf>(CmdPetDomesticationExpNtf.Parser, msg);
            if (ntf.Num > 0)
            {
                string content = LanguageHelper.GetTextContent(2052053, ntf.Num.ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
            }
        }
        #endregion

        #region func
        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents(bool toRegister)
        {
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnFunctionOpen, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.InitFinish, OnFunctionOpenInitFinish, toRegister);
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener((ushort)CmdPet.GetDomesticationReq, (ushort)CmdPet.GetDomesticationRes, OnPetDomesticationInfoRes, CmdPetGetDomesticationRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPet.DomesticationStartReq, (ushort)CmdPet.DomesticationStartRes, OnPetDomesticationStartRes, CmdPetDomesticationStartRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPet.DomesticationEndReq, (ushort)CmdPet.DomesticationEndRes, OnGetPetDomesticationRewardRes, CmdPetDomesticationEndRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPet.DomesticationRefreshReq, (ushort)CmdPet.DomesticationRefreshRes, OnPetDomesticationRefreshRes, CmdPetDomesticationRefreshRes.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdPet.DomesticationAddReq, (ushort)CmdPet.DomesticationAddRes, OnAddPetDomesticationTaskRes, CmdPetDomesticationAddRes.Parser);
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdPet.DomesticationExpNtf, OnPetDomesticationExpNtf, CmdPetDomesticationExpNtf.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPet.GetDomesticationRes, OnPetDomesticationInfoRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPet.DomesticationStartRes, OnPetDomesticationStartRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPet.DomesticationEndRes, OnGetPetDomesticationRewardRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPet.DomesticationRefreshRes, OnPetDomesticationRefreshRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPet.DomesticationAddRes, OnAddPetDomesticationTaskRes);
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdPet.DomesticationExpNtf, OnPetDomesticationExpNtf);
            }
        }
        /// <summary>
        /// 检测驯养功能是否开启
        /// </summary>
        public bool CheckIsOpen()
        {
            return Sys_FunctionOpen.Instance.IsOpen(11101);
        }
        /// <summary>
        /// 检测驯养功能红点
        /// </summary>
        public bool CheckRedPoint()
        {
            for (int i = 0; i < listTask.Count; i++)
            {
                //策划说有未开始的任务就显示红点
                if (listTask[i].PetUid == 0)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 检测是否有可领取的奖励
        /// </summary>
        public bool CheckCanGetReward()
        {
            for (int i = 0; i < listTask.Count; i++)
            {
                var nowTime = Sys_Time.Instance.GetServerTime();
                if (listTask[i].PetUid != 0 && listTask[i].EndTime <= nowTime)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 一键领取奖励
        /// </summary>
        public void CheckToGetReward()
        {
            if(CheckCanGetReward())
            {
                ReqGetPetDomesticationReward();
            }
        }
        /// <summary>
        /// 获取任务列表
        /// </summary>
        public List<DomesticationTask> GetTaskList()
        {
            List<DomesticationTask> listUsable = new List<DomesticationTask>();//可开始
            List<DomesticationTask> listUnderway = new List<DomesticationTask>();//进行中
            for (int i = 0; i < listTask.Count; i++)
            {
                var curTask = listTask[i];
                if (curTask.PetUid == 0)
                {
                    bool flag = false;
                    for (int j = 0; j < listUsable.Count; j++)
                    {
                        if (curTask.Grade < listUsable[j].Grade)
                        {
                            flag = true;
                            listUsable.Insert(j, curTask);
                            break;
                        }
                    }
                    if (!flag)
                    {
                        listUsable.Add(curTask);
                    }
                }
                else
                {
                    bool flag = false;
                    for (int j = 0; j < listUnderway.Count; j++)
                    {
                        if (curTask.Grade < listUnderway[j].Grade)
                        {
                            flag = true;
                            listUnderway.Insert(j, curTask);
                            break;
                        }
                    }
                    if (!flag)
                    {
                        listUnderway.Add(curTask);
                    }
                }
            }
            listUsable.AddRange(listUnderway);
            return listUsable;
        }
        /// <summary>
        /// 检测并刷新任务
        /// </summary>
        public void CheckToRefreshTask()
        {
            uint canRefreshNum = 0;
            for (int i = 0; i < listTask.Count; i++)
            {
                if(listTask[i].PetUid == 0)
                {
                    canRefreshNum++;
                }
            }
            if(canRefreshNum > 0)
            {
                var config = CSVParam.Instance.GetConfData(1572).str_value.Split('|');
                uint costItemId = uint.Parse(config[0]);
                uint needCostNum = uint.Parse(config[1]) * canRefreshNum;
                long hasNum = Sys_Bag.Instance.GetItemCount(costItemId);
                if(hasNum >= needCostNum)
                {
                    ReqRefreshPetDomesticationTask();
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2052141));//货币不足
                    Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)costItemId, needCostNum);
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2052143));//没有可刷新的驯养任务
            }
        }

        /// <summary>
        /// 获取刷新消耗货币的iconId
        /// </summary>
        public uint GetRefreshCostIconId()
        {
            var config = CSVParam.Instance.GetConfData(1572).str_value.Split('|');
            uint costItemId = uint.Parse(config[0]);
            var csvItem = CSVItem.Instance.GetConfData(costItemId);
            return csvItem.small_icon_id;
        }
        /// <summary>
        /// 获取刷新消耗的文本
        /// </summary>
        public string GetRefreshCostText()
        {
            uint canRefreshNum = 0;
            for (int i = 0; i < listTask.Count; i++)
            {
                if (listTask[i].PetUid == 0)
                {
                    canRefreshNum++;
                }
            }
            var config = CSVParam.Instance.GetConfData(1572).str_value.Split('|');
            uint costItemId = uint.Parse(config[0]);
            uint needCostNum = uint.Parse(config[1]) * canRefreshNum;
            long hasNum = Sys_Bag.Instance.GetItemCount(costItemId);
            if (hasNum >= needCostNum)
            {
                //return LanguageHelper.GetTextContent(2052004, hasNum.ToString(), ); //{0}/{1}
                return needCostNum.ToString();
            }
            else
            {
                //return LanguageHelper.GetTextContent(2052004, LanguageHelper.GetTextContent(2052045, hasNum.ToString()), needCostNum.ToString()); //{0}/{1}
                return LanguageHelper.GetTextContent(2052045, needCostNum.ToString());
            }
        }

        /// <summary>
        /// 检测并加驯任务
        /// </summary>
        public void CheckToAddTask()
        {
            uint maxTimes = uint.Parse(CSVParam.Instance.GetConfData(1574).str_value);//最大次数
            if(ExTaskCount >= maxTimes)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2052142));//今日加驯已达上限
                return;
            }
            var config = CSVParam.Instance.GetConfData(1573).str_value.Split('|');
            int index = (int)Math.Min(ExTaskCount, maxTimes - 1);
            var curConfig = config[index].Split('&');
            uint costItemId = uint.Parse(curConfig[0]);
            uint needCostNum = uint.Parse(curConfig[1]);
            long hasNum = Sys_Bag.Instance.GetItemCount(costItemId);
            if (hasNum >= needCostNum)
            {
                ReqAddPetDomesticationTask();
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2052141));//货币不足
                Sys_Bag.Instance.TryOpenExchangeCoinUI((ECurrencyType)costItemId, needCostNum);
            }
        }
        /// <summary>
        /// 获取加训消耗货币的iconId
        /// </summary>
        public uint GetAddTaskCostIconId()
        {
            uint maxTimes = uint.Parse(CSVParam.Instance.GetConfData(1574).str_value);//最大次数
            var config = CSVParam.Instance.GetConfData(1573).str_value.Split('|');
            int index = (int)Math.Min(ExTaskCount, maxTimes - 1);
            var curConfig = config[index].Split('&');
            uint costItemId = uint.Parse(curConfig[0]);
            var csvItem = CSVItem.Instance.GetConfData(costItemId);
            return csvItem.small_icon_id;
        }
        /// <summary>
        /// 获取加训消耗文本
        /// </summary>
        public string GetAddTaskCostText()
        {
            uint maxTimes = uint.Parse(CSVParam.Instance.GetConfData(1574).str_value);//最大次数
            var config = CSVParam.Instance.GetConfData(1573).str_value.Split('|');
            int index = (int)Math.Min(ExTaskCount, maxTimes - 1);
            var curConfig = config[index].Split('&');
            uint costItemId = uint.Parse(curConfig[0]);
            uint needCostNum = uint.Parse(curConfig[1]);
            long hasNum = Sys_Bag.Instance.GetItemCount(costItemId);
            if (hasNum >= needCostNum)
            {
                //return LanguageHelper.GetTextContent(2052004, hasNum.ToString(), needCostNum.ToString()); //{0}/{1}
                return needCostNum.ToString();
            }
            else
            {
                return LanguageHelper.GetTextContent(2052045, needCostNum.ToString());
            }
        }
        /// <summary>
        /// 获取加训次数文本
        /// </summary>
        public string GetAddTaskTimesText()
        {
            uint maxTimes = uint.Parse(CSVParam.Instance.GetConfData(1574).str_value);//最大次数
            return LanguageHelper.GetTextContent(2052036, (maxTimes - ExTaskCount).ToString()); //每日限购：{0}
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
            return LanguageHelper.GetTextContent(2052022, StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder));//驯养时长：{0}
        }
        /// <summary>
        /// 获取条件描述 type档位类型
        /// </summary>
        public string GetConditionText(int type,uint needNum)
        {
            uint languageId = 0;
            switch(type)
            {
                case 0:
                    languageId = 2052131;//体力 
                    break;
                case 1:
                    languageId = 2052132;//力量
                    break;
                case 3:
                    languageId = 2052133;//速度
                    break;
                case 4:
                    languageId = 2052134;//魔法
                    break;
            }
            return LanguageHelper.GetTextContent(2052025, LanguageHelper.GetTextContent(languageId), needNum.ToString());// {0}档位≥{1}
        }
        /// <summary>
        /// 获取条件档位对应的枚举类型
        /// </summary>
        public EBaseAttr GetEBaseAttrByType(int type)
        {
            switch (type)
            {
                case 0://体力 
                    return EBaseAttr.Vit;
                case 1://力量
                    return EBaseAttr.Snh;
                case 3://速度
                    return EBaseAttr.Speed;
                case 4://魔法
                    return EBaseAttr.Magic;
            }
            return EBaseAttr.None;
        }
        /// <summary>
        /// 获取有效的宠物列表
        /// </summary>
        public List<ClientPet> GetValidPetList(DomesticationTask taskData)
        {
            List<ClientPet> showList = new List<ClientPet>();
            var csvTask = CSVDomesticationTask.Instance.GetConfData(taskData.InfoId);
            var petsList = Sys_Pet.Instance.petsList;
            int gradeindex = (int)taskData.Grade - 1;
            uint needNum = csvTask.condition[gradeindex];
            for (int i = 0; i < petsList.Count; i++)
            {
                var curPet = petsList[i];
                if (curPet.GetPetAllGradeAttr(GetEBaseAttrByType(csvTask.type)) >= needNum)
                {
                    bool flag = false;
                    for (int j = 0; j < listTask.Count; j++)
                    {
                        if (listTask[j].PetUid == curPet.petUnit.Uid)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        bool isInsert = false;
                        for (int j = 0; j < showList.Count; j++)
                        {
                            if (curPet.petUnit.SimpleInfo.Score > showList[j].petUnit.SimpleInfo.Score)
                            {
                                isInsert = true;
                                showList.Insert(j, curPet);
                                break;
                            }
                        }
                        if (!isInsert)
                        {
                            showList.Add(curPet);
                        }
                    }
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
        /// 获取自己段位文本
        /// </summary>
        public string GetRankStageText()
        {
            uint rankId = Stage * 1000 + Level;
            CSVDomesticationRank.Data csvRank = CSVDomesticationRank.Instance.GetConfData(rankId);
            if (csvRank != null)
            {
                string rank = LanguageHelper.GetTextContent(csvRank.name);
                return LanguageHelper.GetTextContent(2022120, rank, Stage.ToString(), Level.ToString());
            }
            return LanguageHelper.GetTextContent(540000028);
        }
        /// <summary>
        /// 获取别人段位文本
        /// </summary>
        public string GetRankStageText(uint rankScore)
        {
            uint rankId = rankScore;
            uint stage = rankScore / 1000;
            uint level = rankScore % 1000;
            CSVDomesticationRank.Data csvRank = CSVDomesticationRank.Instance.GetConfData(rankId);
            if (csvRank != null)
            {
                string rank = LanguageHelper.GetTextContent(csvRank.name);
                return LanguageHelper.GetTextContent(2022120, rank, stage.ToString(), level.ToString());
            }
            else
            {
                return rankScore.ToString();
            }
        }

        private void StartTimer()
        {
            timer?.Cancel();
            var nowTime = Sys_Time.Instance.GetServerTime();
            //凌晨 0点 5点都要刷新
            ulong zeroTime = Sys_Time.Instance.GetDayZeroTimestamp();
            ulong updateTime = zeroTime + 3600 * 5;
            if(nowTime >= updateTime)
            {
                updateTime = zeroTime + 86400;
            }
            countDownTime = updateTime - nowTime + 10;//加10秒延时
            if (countDownTime > 0)
            {
                timer = Timer.Register(countDownTime, OnTimerComplete, null, false, false);
            }
        }
        #endregion

        #region 加成概率相关计算
        /// <summary>
        /// 获取评分加成
        /// </summary>
        public uint GetScoreAddition(DomesticationTask taskData,uint petUid)
        {
            var csvTask = CSVDomesticationTask.Instance.GetConfData(taskData.InfoId);
            var scoreList = csvTask.addition_point[(int)taskData.Grade - 1];
            var clientPet = GetClientPetBuyUid(petUid);
            uint curScore = clientPet.petUnit.SimpleInfo.Score;
            for (int i = 0; i < scoreList.Count; i++)
            {
                if(curScore < scoreList[i])
                {
                    return (uint)(10 * (i + 1));
                }
            }
            return 100u;
        }
        /// <summary>
        /// 获取种族加成
        /// </summary>
        public uint GetRaceAddition(DomesticationTask taskData, uint petUid)
        {
            var csvTask = CSVDomesticationTask.Instance.GetConfData(taskData.InfoId);
            var clientPet = GetClientPetBuyUid(petUid);
            if(csvTask.race == clientPet.petData.race)
            {
                return (uint)csvTask.addition_race;
            }
            return 0u;
        }

        /// <summary>
        /// 获取技能加成
        /// </summary>
        /// <returns></returns>
        public uint GetSkillAddition(DomesticationTask taskData, uint petUid)
        {
            var csvTask = CSVDomesticationTask.Instance.GetConfData(taskData.InfoId);
            var clientPet = GetClientPetBuyUid(petUid);
            var buildSkills = clientPet.petUnit.BuildInfo.BuildSkills;
            for (int i = 0; i < buildSkills.Count; i++)
            {
                if (taskData.LuckySkill == buildSkills[i])
                {
                    return (uint)csvTask.addition_skill;
                }
            }
            return 0u;
        }

        /// <summary>
        /// 获取总加成文本
        /// </summary>
        /// <returns></returns>
        public uint GetAllAddition(DomesticationTask taskData, uint petUid)
        {
            return GetScoreAddition(taskData, petUid) + GetRaceAddition(taskData, petUid) + GetSkillAddition(taskData, petUid);
        }
        /// <summary>
        /// 检测宠物是否在驯养中 
        /// </summary>
        public bool CheckPetIsDomesticating(uint petUid, bool showMsg = false)
        {
            for (int i = 0; i < listTask.Count; i++)
            {
                var curTask = listTask[i];
                if (curTask.PetUid != 0 && curTask.PetUid == petUid)
                {
                    if (showMsg)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2052046));//该宠物正在驯养中，无法上架
                    }
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 获取下一个将要完成的任务结束cd
        /// </summary>
        public uint GetNextWillFinishTaskCD()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            uint endTime = 0;

            for (int i = 0; i < listTask.Count; i++)
            {
                var curTask = listTask[i];
                if (curTask.EndTime > nowTime && (curTask.EndTime < endTime || endTime == 0))
                {
                    endTime = curTask.EndTime;
                }
            }
            if(endTime > nowTime)
            {
                return endTime - nowTime;
            }
            return 0;
        }
        #endregion

        #region event
        private void OnFunctionOpen(Sys_FunctionOpen.FunctionOpenData obj)
        {
            if (obj.id == 11101)
            {
                ReqGetPetDomesticationInfo();
            }
        }
        private void OnFunctionOpenInitFinish()
        {
            if (CheckIsOpen())
            {
                
                    ReqGetPetDomesticationInfo();
            }
        }
        private void OnTimerComplete()
        {
            timer?.Cancel();
            if (CheckIsOpen())
            {
                //Debug.Log("OnTimerComplete");
                ReqGetPetDomesticationInfo();
            }
            StartTimer();
        }
        /// <summary> 服务器时间同步 </summary>
        private void OnTimeNtf(uint oldTime, uint newTime)
        {
            StartTimer();
        }
        #endregion

    }
}
