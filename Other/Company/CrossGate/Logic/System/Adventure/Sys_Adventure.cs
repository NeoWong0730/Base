using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;
using Net;
using Table;
using Lib.Core;


namespace Logic
{
    public enum EAdventureProgressType
    {
        //对应CSVAdventureProgress表的id
        /// <summary>求助任务</summary>
        LoveTask = 1,
        /// <summary>挑战任务</summary>
        ChallengeTask = 2, 
        /// <summary>探索点</summary>
        Explore = 3,
        /// <summary>传送点</summary>
        Transfer = 4,
        /// <summary>侦探任务</summary>
        DetectiveTask = 5,
        /// <summary>悬赏令</summary>
        Reward = 6,
        /// <summary>资源点</summary>
        Resource = 7,
    }
    public partial class Sys_Adventure : SystemModuleBase<Sys_Adventure>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents : int
        {
            OnLevelInfoUpdate,
            OnTipsNumUpdate,        //标签次数刷新的事件
            OnCLoseAdventureView,   //关闭冒险手册
            OnTryDoMainTaskByRewardTaskFinish,        //执行主线任务（完成悬赏令）
            OnTryDoMainTaskByAdventureLevelUp,        //执行主线任务（冒险等级提升）
        }
        public uint Level { get; private set; } = 0;
        public uint Exp { get; private set; } = 0;
        /// <summary> 冒险手册地图探索表的id列表 </summary>
        //public List<uint> MapIds { get; private set; } = new List<uint>();
        /// <summary> 冒险手册悬赏令表的id列表 </summary>
        //public List<uint> RewardIds { get; private set; } = new List<uint>();
        /// <summary> 冒险手册悬赏令最后一个任务的id列表 </summary>
        public List<uint> RewardLastTaskIds { get; private set; } = new List<uint>();
        /// <summary> 冒险手册悬赏令最后一个任务的id对应悬赏令ID lastTaskId,rewardId </summary>
        public Dictionary<uint, uint> RewardLastTaskIdsDic { get; private set; } = new Dictionary<uint, uint>();
        /// <summary> 冒险手册悬赏令紧急任务的前置任务id列表 </summary>
        public List<uint> RewardUrgentPreTaskIds { get; private set; } = new List<uint>();
        /// <summary> 冒险手册悬赏令紧急任务的id对应悬赏令ID UrgentTaskId,rewardId </lastTaskId> </summary>
        public Dictionary<uint, uint> RewardUrgentPreTaskIdsDic { get; private set; } = new Dictionary<uint, uint>();
        /// <summary> 冒险手册悬赏令非紧急的第一个任务的id列表 </summary>
        public List<uint> RewardNormalFirstTaskIds { get; private set; } = new List<uint>();
        /// <summary> 是否注册了 完成悬赏令后执行主线任务的事件 </summary>
        private bool IsRegisterTryToMainTask;
        /// <summary> 冒险手册进度排序后的id列表 </summary>
        public List<uint> ListProIds { get; private set; } = new List<uint>();
        #region 系统函数
        public override void Init()
        {
            base.Init();
            //MapIds.AddRange(CSVAdventureMap.Instance.GetDictData().Keys);
            //RewardIds.AddRange(CSVAdventureCriminal.Instance.GetDictData().Keys);

            var rewardDatas = CSVAdventureCriminal.Instance.GetAll();
            for (int i = 0, len = rewardDatas.Count; i < len; i++)
            {
                //CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(RewardIds[i]);
                CSVAdventureCriminal.Data data = rewardDatas[i];// CSVAdventureCriminal.Instance.GetConfData(RewardIds[i]);
                uint lastTaskId = data.finishTaskId;
                RewardLastTaskIds.Add(lastTaskId);
                RewardLastTaskIdsDic.Add(lastTaskId, data.id);
                if (data.isUrgent == 1)
                {
                    RewardUrgentPreTaskIds.Add(data.preTaskId);
                    RewardUrgentPreTaskIdsDic.Add(data.preTaskId, data.id);
                }
                else
                {
                    RewardNormalFirstTaskIds.Add(data.acceptTaskId);
                }
            }


            List<CSVAdventureProgress.Data>  lstPro = new List<CSVAdventureProgress.Data>(CSVAdventureProgress.Instance.GetAll());
            //lstPro.AddRange(CSVAdventureProgress.Instance.GetDictData().Values);
            for (int i = 0; i < lstPro.Count - 1 ; i++)
            {
                bool flag = false;
                for (int j = 0; j < lstPro.Count - i - 1; j++)
                {
                    if (lstPro[j].sort > lstPro[j + 1].sort)
                    {
                        var tamp = lstPro[j];
                        lstPro[j] = lstPro[j + 1];
                        lstPro[j + 1] = tamp;
                        flag = true;
                    }
                }
                if (!flag)
                {
                    break;
                }
            }
            ListProIds.Clear();
            for (int i = 0; i < lstPro.Count; i++)
            {
                ListProIds.Add(lstPro[i].id);
            }
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdAdventure.GetInfoRes, OnAdventureGetInfoRes, CmdAdventureGetInfoRes.Parser);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, OnTaskSubmited, true);
            Sys_Exploration.Instance.eventEmitter.Handle(Sys_Exploration.EEvents.ExplorationRewardNotice, OnTipNumUpdate, true);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnReceived, OnTaskReceived, true);
        }
        public override void OnLogin()
        {
            base.OnLogin();
            AdventureGetInfoReq();
        }
        public override void OnLogout()
        {
            base.OnLogout();
        }
        #endregion

        #region net
        public void AdventureGetInfoReq()
        {
            CmdAdventureGetInfoReq req = new CmdAdventureGetInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdAdventure.GetInfoReq, req);
        }

        public void OnAdventureGetInfoRes(NetMsg msg)
        {
            CmdAdventureGetInfoRes res = NetMsgUtil.Deserialize<CmdAdventureGetInfoRes>(CmdAdventureGetInfoRes.Parser, msg);
            if (res.Addexp > 0)
            {
                if (res.Level > 1 && res.Level > Level)
                {
                    UIManager.CloseUI(EUIID.UI_Adventure_LevelUp);
                    if (IsRegisterTryToMainTask)
                    {
                        RegisterTryDoMainTaskByRewardTaskFinish(false);
                        RegisterTryDoMainTaskByAdventureLevelUp(true);
                    }
                    UIScheduler.Push(EUIID.UI_Adventure_LevelUp, res.Level, null, true, UIScheduler.popTypes[EUIPopType.WhenLastPopedUIClosed]);
                }
                uint addExp = res.Addexp;
                string txtName = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(16).name_id);
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, LanguageHelper.GetTextContent(600010000, txtName, addExp.ToString()), Sys_Chat.EMessageProcess.None);
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(600010000, txtName, addExp.ToString()));
            }
            Level = res.Level;
            Exp = res.Exp;
            DebugUtil.Log(ELogType.eNone, "Adventure LEVEL:" + Level + " | EXP:" + Exp + " | AddEXP:" + res.Addexp);
            eventEmitter.Trigger(EEvents.OnLevelInfoUpdate);
        }
        #endregion

        #region function
        /// <summary> 获取冒险进度内容 EAdventureProgressType {Cur,Max} </summary>
        public Dictionary<EAdventureProgressType, List<uint>> GetProgressInfoDict()
        {
            Dictionary<EAdventureProgressType, List<uint>> dicPro = new Dictionary<EAdventureProgressType, List<uint>>();
            //地图相关
            uint LoveTaskCur = 0;       //爱心任务
            uint LoveTaskMax = 0;
            uint ChallengeTaskCur = 0;  //挑战任务
            uint ChallengeTaskMax = 0;
            uint ExplorationCur = 0;    //探索点
            uint ExplorationMax = 0;
            uint TransmitCur = 0;       //传送点
            uint TransmitMax = 0;
            uint ResouceCur = 0;       //资源点
            uint ResouceMax = 0;

            var mapDatas = CSVAdventureMap.Instance.GetAll();
            for (int i = 0, len = mapDatas.Count; i < len; i++)
            {
                //CSVAdventureMap.Data data = CSVAdventureMap.Instance.GetConfData(MapIds[i]);
                CSVAdventureMap.Data data = mapDatas[i];
                if (data != null)
                {
                    uint mapInfoId = data.mapId;
                    Sys_Exploration.ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(mapInfoId);
                    if (explorationData != null)
                    {
                        List<ENPCMarkType> Keys = new List<ENPCMarkType>(explorationData.dict_Process.Keys);
                        for (int j = 0; j < Keys.Count; j++)
                        {
                            Sys_Exploration.ExplorationProcess explorationProcess = explorationData.GetExplorationProcess(Keys[j]);
                            if (Keys[j] == ENPCMarkType.LoveTask)
                            {
                                LoveTaskCur += explorationProcess.CurNum;
                                LoveTaskMax += explorationProcess.TargetNum;
                            }
                            else if (Keys[j] == ENPCMarkType.ChallengeTask)
                            {
                                ChallengeTaskCur += explorationProcess.CurNum;
                                ChallengeTaskMax += explorationProcess.TargetNum;
                            }
                            else if (Keys[j] == ENPCMarkType.Exploration)
                            {
                                ExplorationCur += explorationProcess.CurNum;
                                ExplorationMax += explorationProcess.TargetNum;
                            }
                            else if (Keys[j] == ENPCMarkType.Transmit)
                            {
                                TransmitCur += explorationProcess.CurNum;
                                TransmitMax += explorationProcess.TargetNum;
                            }
                            else if (Keys[j] == ENPCMarkType.Resources)
                            {
                                ResouceCur += explorationProcess.CurNum;
                                ResouceMax += explorationProcess.TargetNum;
                            }
                        }
                    }
                }
            }
            dicPro[EAdventureProgressType.LoveTask] = new List<uint> { LoveTaskCur, LoveTaskMax };
            dicPro[EAdventureProgressType.ChallengeTask] = new List<uint> { ChallengeTaskCur, ChallengeTaskMax };
            dicPro[EAdventureProgressType.Explore] = new List<uint> { ExplorationCur, ExplorationMax };
            dicPro[EAdventureProgressType.Transfer] = new List<uint> { TransmitCur, TransmitMax };
            dicPro[EAdventureProgressType.Resource] = new List<uint> { ResouceCur, ResouceMax };
            //侦探任务
            uint DetectiveTaskCur = 0;
            uint DetectiveTaskMax = 0;
            List<uint> taskIds = Sys_ClueTask.Instance.GetTasks(0, EClueTaskType.Detective);
            for (int i = 0; i < taskIds.Count; i++)
            {
                ClueTask taskData = Sys_ClueTask.Instance.tasks[taskIds[i]];
                if (taskData != null)
                {
                    DetectiveTaskMax++;
                    taskData.RefreshStatus();
                    if (taskData.taskStatus == EClueTaskStatus.Finished)
                    {
                        DetectiveTaskCur++;
                    }
                }
            }
            dicPro[EAdventureProgressType.DetectiveTask] = new List<uint> { DetectiveTaskCur, DetectiveTaskMax };
            //悬赏令
            uint RewardCur = 0;
            uint RewardMax = 0;
            var rewardDatas = CSVAdventureCriminal.Instance.GetAll();
            for (int i = 0, len = rewardDatas.Count; i < len; i++)
            {
                //CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(RewardIds[i]);
                CSVAdventureCriminal.Data data = rewardDatas[i];
                if (data != null)
                {
                    RewardMax++;
                    uint lastTaskId = data.finishTaskId;
                    bool isFinish = TaskHelper.HasSubmited(lastTaskId);
                    if (isFinish)
                    {
                        RewardCur++;
                    }
                }
            }
            dicPro[EAdventureProgressType.Reward] = new List<uint> { RewardCur, RewardMax };

            return dicPro;
        }
        /// <summary> 获取任务链中的所有任务id 倒序 </summary>
        public List<uint> GetAllTaskId(uint acceptTaskId, uint lastTaskId)
        {
            List<uint> allTaskIds = new List<uint>();
            uint MAX_NUM = 100;  //保护机制最大值，不使用while，以免表配错
            uint curTaksId = lastTaskId;
            allTaskIds.Add(lastTaskId);
            for (int i = 0; i < MAX_NUM; i++)
            {
                CSVTask.Data data = CSVTask.Instance.GetConfData(curTaksId);
                var preposeTasks = data.preposeTask;
                preposeTasks.Reverse();
                allTaskIds.AddRange(preposeTasks);
                if (preposeTasks.Contains(acceptTaskId))
                {
                    break;
                }
                curTaksId = data.preposeTask[0];
                if (i == MAX_NUM - 1)
                {
                    DebugUtil.Log(ELogType.eNone, "AdventureGetAllTask | 任务链遍历已达限额:" + MAX_NUM + "|请检查表或联系程序增加上限");
                }
            }
            //string str = "";
            //for (int i = 0; i < allTaskIds.Count; i++)
            //{
            //    str += " | " + allTaskIds[i] + ":" + (TaskHelper.HasSubmited(allTaskIds[i]) ? 1 : 0);
            //}
            //Debug.Log("adventure AllTaskState" + str);
            return allTaskIds;
        }
        public List<ItemData> GetTaskItems(uint taskId)
        {
            List<ItemData> items = new List<ItemData>();
            CSVTask.Data taskData = CSVTask.Instance.GetConfData(taskId);
            if (taskData != null)
            {
                if (taskData.RewardGold != null)
                {
                    ItemData gold = new ItemData(0, 0, taskData.RewardGold[0], taskData.RewardGold[1], 0, false, false, null, null, 0);
                    items.Add(gold);
                }
                if (taskData.RewardExp != null)
                {
                    ItemData exp = new ItemData(0, 0, taskData.RewardExp[0], taskData.RewardExp[1], 0, false, false, null, null, 0);
                    items.Add(exp);
                }
                if (taskData.DropId != null)
                {
                    int length = taskData.DropId.Count;
                    for (int i = 0; i < length; i++)
                    {
                        List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(taskData.DropId[i]);
                        if (dropItems != null)
                        {
                            int len = dropItems.Count;
                            for (int j = 0; j < len; j++)
                            {
                                ItemData item = new ItemData(0, 0, dropItems[j].id, (uint)dropItems[j].count, 0, false, false, null, null, 0);
                                items.Add(item);
                            }
                        }
                    }
                }
            }
            return items;
        }
        /// <summary> 悬赏令排序 </summary>
        public List<uint> GetSortRewardIds()
        {
            List<uint> sortIds = new List<uint>();
            List<uint> finishIds = new List<uint>();      //完成
            List<uint> openUrgentIds = new List<uint>();        //开启，紧急
            List<uint> openNormalIds = new List<uint>();        //开启，非紧急
            List<uint> UnOpenIds = new List<uint>();      //未开启

            var rewardDatas = CSVAdventureCriminal.Instance.GetAll();
            for (int i = 0, len = rewardDatas.Count; i < len; i++)
            {
                //uint rewardId = RewardIds[i];
                //CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
                CSVAdventureCriminal.Data data = rewardDatas[i];
                uint rewardId = data.id;
                if (data != null)
                {
                    bool isOpen = CheckRewardIsUnlock(data.id);
                    bool isUrgent = data.isUrgent == 1;
                    if (isOpen)
                    {
                        uint lastTaskId = data.finishTaskId;
                        bool isFinish = TaskHelper.HasSubmited(lastTaskId);
                        if (isFinish)
                        {
                            finishIds.Add(rewardId);
                        }
                        else
                        {
                            if (isUrgent)
                            {
                                openUrgentIds.Add(rewardId);
                            }
                            else
                            {
                                openNormalIds.Add(rewardId);
                            }
                        }
                    }
                    else
                    {
                        UnOpenIds.Add(rewardId);
                    }

                }
            }
            sortIds.AddRange(finishIds);
            sortIds.AddRange(openUrgentIds);
            sortIds.AddRange(openNormalIds);
            sortIds.AddRange(UnOpenIds);
            return sortIds;
        }
        /// <summary> 注册/注销悬赏令完成界面关闭后，执行主线任务的事件 </summary>
        private void RegisterTryDoMainTaskByRewardTaskFinish(bool toRegister)
        {
            IsRegisterTryToMainTask = toRegister;
            eventEmitter.Handle(EEvents.OnTryDoMainTaskByRewardTaskFinish, OnRewardTaskFinishToTryDoMainTask, toRegister);
        }
        /// <summary> 注册/注销冒险等级提升界面关闭后，执行主线任务的事件 </summary>
        private void RegisterTryDoMainTaskByAdventureLevelUp(bool toRegister)
        {
            eventEmitter.Handle(EEvents.OnTryDoMainTaskByAdventureLevelUp, OnRewardTaskFinishToTryDoMainTask, toRegister);
        }
        /// <summary> 获取冒险手册特权封印加成概率（万分比） </summary>
        public uint GetCaptureProbability()
        {
            uint pro = 0;
            CSVAdventureLevel.Data levelData = CSVAdventureLevel.Instance.GetConfData(Level);
            if (levelData != null && levelData.addPrivilegeAttribute != null)
            {
                List<List<uint>> attrs = levelData.addPrivilegeAttribute;
                for (int i = 0; i < attrs.Count; i++)
                {
                    //封印成功概率 89
                    if(attrs[i][0] == 89)
                    {
                        return attrs[i][1];
                    }
                }
            }
            return pro;
        }
        /// <summary> 检测悬赏是否已完成  rewardId 悬赏令ID </summary>
        public bool CheckRewardIsFinish(uint rewardId)
        {
            CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
            if (data != null)
            {
                return TaskHelper.HasSubmited(data.finishTaskId);
            }
            return false;
        }
        /// <summary> 检测悬赏是否已解锁  rewardId 悬赏令ID </summary>
        public bool CheckRewardIsUnlock(uint rewardId)
        {
            CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
            if (data != null)
            {
                bool taskCondition = true;
                bool lvCondition = true;
                if (data.preTaskId > 0)
                {
                    taskCondition = TaskHelper.HasSubmited(data.preTaskId);
                }
                if (data.preLevel > 0)
                {
                    lvCondition = Sys_Role.Instance.Role.Level >= data.preLevel;
                }
                return taskCondition && lvCondition;
            }
            return false;
        }
        /// <summary> 获取悬赏令是未解锁提示文本  rewardId 悬赏令ID </summary>
        public string GetRewardUnlockConditionText(uint rewardId)
        {
            CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(rewardId);
            if (data != null)
            {
                if (data.preTaskId > 0)
                {
                    CSVTask.Data preTask = CSVTask.Instance.GetConfData(data.preTaskId);
                    return LanguageHelper.GetTextContent(680000001, CSVTaskLanguage.Instance.GetConfData(preTask.taskName).words);
                }
                if (data.preLevel > 0)
                {
                    return LanguageHelper.GetTextContent(680000004, data.preLevel.ToString());
                }
            }
            return "";
        }

        /// <summary>
        /// 冒险者手册埋点
        /// </summary>
        public void ReportClickEventHitPoint(string strValue)
        {
            //Debug.Log("adventrueHitPoint" + strValue);
            UIManager.HitButton(EUIID.UI_Adventure, strValue);
        }
        #endregion

        #region 响应事件
        private void OnTipNumUpdate()
        {
            eventEmitter.Trigger(EEvents.OnTipsNumUpdate);
        }
        private void OnTaskSubmited(int menuId, uint taskId, TaskEntry taskEntry)
        {
            if (RewardUrgentPreTaskIds.Contains(taskId))
            {
                uint rewardId = RewardUrgentPreTaskIdsDic[taskId];
                UIScheduler.Push(EUIID.UI_Adventure_RewardGetTips, rewardId, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
            }
            if (RewardLastTaskIds.Contains(taskId))
            {
                uint rewardId = RewardLastTaskIdsDic[taskId];
                RegisterTryDoMainTaskByRewardTaskFinish(true);
                UIScheduler.Push(EUIID.UI_Adventure_RewardFinishTips, rewardId, null, true, UIScheduler.popTypes[EUIPopType.WhenMaininterfaceRealOpenning]);
            }
            OnTipNumUpdate();
        }
        private void OnTaskReceived(int taskCategory, uint taskId, TaskEntry taskEntry)
        {
            if (RewardNormalFirstTaskIds.Contains(taskId))
            {
                Sys_Task.Instance.TryDoTask(taskId, true, false, true);
            }
        }
        /// <summary> 悬赏令完成后自动进行主线任务 </summary>
        private void OnRewardTaskFinishToTryDoMainTask()
        {
            if (Sys_Task.Instance.receivedTasks.TryGetValue((int)ETaskCategory.Trunk, out SortedDictionary<uint, TaskEntry> tasks))
            {
                foreach (var task in tasks)
                {
                    uint taskId = task.Key;
                    DebugUtil.Log(ELogType.eNone, "Adventure_TryDoMainTask" + taskId);
                    Sys_Task.Instance.TryDoTask(taskId);
                    break;
                }
            }
            RegisterTryDoMainTaskByRewardTaskFinish(false);
            RegisterTryDoMainTaskByAdventureLevelUp(false);
        }
        #endregion

        #region numCheck
        public uint GetCheckNumByType(EAdventurePageType type)
        {
            if (type == EAdventurePageType.Map)
            {
                return GetMapCheckNum();
            }
            else if (type == EAdventurePageType.Reward)
            {
                return GetRewardCheckNum();
            }
            else if (type == EAdventurePageType.Task)
            {
                return GetTaskCheckNum();
            }
            return 0;
        }
        public uint GetAllCheckNum()
        {
            uint allNum = 0;
            allNum += GetMapCheckNum();
            allNum += GetRewardCheckNum();
            if (Sys_FunctionOpen.Instance.IsOpen(20601))
            {
                allNum += GetTaskCheckNum();
            }
            return allNum;
        }
        public uint GetMapCheckNum()
        {
            uint num = 0;
            var mapDatas = CSVAdventureMap.Instance.GetAll();
            for (int i = 0, len = mapDatas.Count; i < len; i++)
            {
                //CSVAdventureMap.Data data = CSVAdventureMap.Instance.GetConfData(MapIds[i]);
                CSVAdventureMap.Data data = mapDatas[i];
                if (data != null)
                {
                    uint mapInfoId = data.mapId;
                    CSVMapInfo.Data mapData = CSVMapInfo.Instance.GetConfData(mapInfoId);
                    Sys_Exploration.ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(mapInfoId);
                    uint userLevel = Sys_Role.Instance.Role.Level;
                    if (mapData != null && explorationData != null && userLevel >= mapData.map_lv[0])
                    {
                        List<ENPCMarkType> Keys = new List<ENPCMarkType>(explorationData.dict_Process.Keys);
                        for (int j = 0; j < Keys.Count; j++)
                        {
                            Sys_Exploration.ExplorationProcess explorationProcess = explorationData.GetExplorationProcess(Keys[j]);
                            if (explorationProcess.CurNum < explorationProcess.TargetNum)
                            {
                                num++;
                                break;
                            }
                        }
                    }
                }
            }
            return num;
        }
        public uint GetRewardCheckNum()
        {
            uint num = 0;
            var rewardDatas = CSVAdventureCriminal.Instance.GetAll();
            for (int i = 0, len = rewardDatas.Count; i < len; i++)
            {
                //CSVAdventureCriminal.Data data = CSVAdventureCriminal.Instance.GetConfData(RewardIds[i]);
                CSVAdventureCriminal.Data data = rewardDatas[i];
                if (data != null)
                {
                    bool isOpen = CheckRewardIsUnlock(data.id);
                    if (isOpen)
                    {
                        uint lastTaskId = data.finishTaskId;
                        bool isFinish = TaskHelper.HasSubmited(lastTaskId);
                        if (!isFinish)
                        {
                            num++;
                        }
                    }
                }
            }
            return num;
        }
        public uint GetTaskCheckNum()
        {
            uint num = 0;
            List<uint> taskIds = Sys_ClueTask.Instance.GetTasks(0, EClueTaskType.Detective);
            Sys_ClueTask.Instance.PreCheck();
            for (int i = 0; i < taskIds.Count; i++)
            {
                ClueTask taskData = Sys_ClueTask.Instance.tasks[taskIds[i]];
                if (taskData != null && taskData.isTriggered)
                {
                    taskData.RefreshStatus();
                    if (taskData.taskStatus != EClueTaskStatus.Finished)
                    {
                        num++;
                    }
                }
            }
            return num;
        }


        public void GetMapFinishAndMaxNum(out int finishnum,out int maxnum)
        {
            finishnum = 0;
            maxnum = 0;

            var mapDatas = CSVAdventureMap.Instance.GetAll();
            for (int i = 0, len = mapDatas.Count; i < len; i++)
            {
                //CSVAdventureMap.Data data = CSVAdventureMap.Instance.GetConfData(MapIds[i]);
                CSVAdventureMap.Data data = mapDatas[i];
                if (data != null)
                {
                    uint mapInfoId = data.mapId;
                    CSVMapInfo.Data mapData = CSVMapInfo.Instance.GetConfData(mapInfoId);
                    Sys_Exploration.ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(mapInfoId);

                    uint userLevel = Sys_Role.Instance.Role.Level;
                    if (mapData != null && explorationData != null && userLevel >= mapData.map_lv[0])
                    {
                        maxnum += 1;

                        if (explorationData.totalProcess.isFinish)
                            finishnum += 1;
                    }
                }
            }
        }
        #endregion
    }
}
