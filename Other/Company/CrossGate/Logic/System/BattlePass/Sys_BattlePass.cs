using System;
using System.Collections.Generic;
using Framework;
using Google.Protobuf.Collections;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_BattlePass : SystemModuleBase<Sys_BattlePass>,ISystemModuleUpdate
    {

        public enum EEvents
        {
            UpdateInfo,
            ActivityState,
            OneKeyLevelRewardRes,
            LevelRewardRes,
            BattlePassTypeChange,
            UnLockNewLevelReward,
            GetTaskReward,
            GetAllTaskReward,
            TaskProcessChange,
            TaskReset,
            LevelExpChange,
            RedState,
            StartNewActivity,
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.OnOffNtf, OnActiveNtf, CmdBattlePassOnOffNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.InfoNtf, OnInfoNtf, CmdBattlePassInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.ResetTasksNtf, OnRestTasksNtf, CmdBattlePassResetTasksNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.ExpNtf, OnExpNtf, CmdBattlePassExpNtf.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.GetBplawardRes, OnGetBPLAward, CmdBattlePassGetBPLAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.GetBplallAwardRes, OnGetBPLAwardAll, CmdBattlePassGetBPLAllAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.BuyBattlePassRes, OnBuyBattlePass, CmdBattlePassBuyBattlePassRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.BuyBattlePassLevelRes, OnBuyBattlePassLevel, CmdBattlePassBuyBattlePassLevelRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.GetBptawardRes, OnGetBPTAward, CmdBattlePassGetBPTAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.GetBptallAwardRes, OnGetBPTAwardAll, CmdBattlePassGetBPTAllAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.GetTasksInfoRes, OnGetTaskInfo, CmdBattlePassGetTasksInfoRes.Parser);

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.TaskProcessNtf, OnGetTaskProcessNtf, CmdBattlePassTaskProcessNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.GetBattlePassInfoRes, OnGetInfoRes, CmdBattlePassGetBattlePassInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBattlePass.RedTipsNtf, OnRedStateTips, CmdBattlePassRedTipsNtf.Parser);

            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnRoleLevelUp,true);

            Sys_OperationalActivity.Instance.eventEmitter.Handle(Sys_OperationalActivity.EEvents.UpdateOperatinalActivityShowOrHide, OnUpdateOperatinalActivityShowOrHide, true);

            //LoadLevelReward();

            ReadMaxLevelConfig();
        }

        public override void OnLogin()
        {
            VipLastLevel = -1;
            FristEnterTime = 0;
            LoadJson();
        }
        public override void OnLogout()
        {
            //SaveJson();

            m_isActive = false;

            Info = new CmdBattlePassInfoNtf();

            ActiveNtfInfo = null;
        }

        public void OnUpdate()
        {
            if (Info != null && isActive)
            {
               var servertime = Sys_Time.Instance.GetServerTime();

                if (servertime > ActiveNtfInfo.EndTime)
                {
                    UpdateActive();
                }
            }
        }


        public bool OpenBattlepassShop()
        {
            if (isActive == false)
            {
                return false;
            }

            UIManager.OpenUI(EUIID.UI_BattlePass,false,new UI_BattlePass_Parma() { Type =2});
            return true;
        }
        
    }

    public partial class Sys_BattlePass : SystemModuleBase<Sys_BattlePass>
    {

        private bool m_isActive = false;
        private bool m_IsActivity;
        public bool isActive { get { return m_isActive; } }

        public uint BranchID { get {

                if (ActiveNtfInfo == null)
                    return 10010u;

                return ActiveNtfInfo.ActivityId;
            } } 
        public bool isFristActive { get {

                if (isInActiveTime() == false)
                    return false;

                if (FristEnterTime > 0 && FristEnterTime > ActiveNtfInfo.StartTime && FristEnterTime <= ActiveNtfInfo.EndTime)
                    return false;

                return true;
                    
            } }

        public CmdBattlePassInfoNtf Info { get; private set; } = new CmdBattlePassInfoNtf();

        public CmdBattlePassOnOffNtf ActiveNtfInfo { get; private set; }

        /// <summary>
        /// 用于记录成为VIP前的等级，然后做关闭VIP界面弹出等级升级界面
        /// </summary>
        public int VipLastLevel { get; set; } = 0;
        public bool isVip { get {

                if (Info == null)
                    return false;

                return Info.BattlePassType != (uint)(BattlePassType.None);
            } }

        private void UpdateActive()
        {           
            var active = DoCheckActive();

            if (active != m_isActive)
            {
                m_isActive = active ;

                eventEmitter.Trigger(EEvents.ActivityState);
            }

        }


        private bool DoCheckActive()
        {
            if (Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(201u) == false)
                return false;

            if (ActiveNtfInfo == null)
                return false;

            if (Sys_Role.Instance.Role.Level < ActiveNtfInfo.MinLevel)
                return false;      

            return isInActiveTime();
        }

        public bool isInActiveTime()
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            return ActiveNtfInfo.StartTime <= nowtime && ActiveNtfInfo.EndTime > nowtime;
        }
        public uint GetSpalceLevel(uint level)
        {         
            CSVBattlePassUpgrade.Data data = Sys_BattlePass.Instance.GetUpgradeTableData(level);

            if (data == null)
            {
                return 0;
            }

            if (data.Reward_Type == 2)
                return level;            

            uint slevel = 0;

            uint count = (uint)CSVBattlePassUpgrade.Instance.Count;
            for (uint i = 0; i < count; i++)
            {
                var curdata = Sys_BattlePass.Instance.GetUpgradeTableData(level + i);

                if (curdata == null)
                    break;

                var curlevel = curdata.id % 1000;

                if (curlevel > level && curdata.Reward_Type == 2)
                {
                    slevel = curlevel;
                    break;
                }
            }

            return slevel;
        }

        /// <summary>
        /// 获得战令等级奖励领取状态 0 未开启 1 可领取 2 已领取
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public uint GetLevelNromalRewardGetState(uint level)
        {
            if (Info == null || level > Info.Level)
                return 0;

            var result = Info.AwardList.Find(o => o.Id == level);

            if (result == null || result.Id == 0 )
                return 2u;

            var levelereward = GetLevelReward(level);

            if (result.Common == 0)
            {
                return levelereward.NormalReward.Count > 0 ? 1u : 0u;
            }
            return 2u;
        }

        public uint GetLevelVipRewardGetState(uint level)
        {
            if (Info == null || level > Info.Level || isVip == false)
                return 0;

            var result = Info.AwardList.Find(o => o.Id == level);

            if (result == null || result.Id == 0)
                return 2u;


            var levelereward = GetLevelReward(level);

            if (result.Token == 0)
            {
                return levelereward.VipReward.Count > 0 ? 1u : 0u;
            }
            return 2u;
        }

        public string GetRewardModelAssetName()
        {
            string value = string.Empty;

            int index = GetRewardModelAssetIndex();

            if (index >= 0)
            {
                var data = GetRewardDisplayTableData(Sys_Role.Instance.Role.Career);
                value = data.Show_Item[index];
            }
            return value;
        }

        public int GetRewardModelAssetIndex()
        {
            var heroId = Sys_Role.Instance.Role.HeroId;

            var data = GetRewardDisplayTableData(Sys_Role.Instance.Role.Career);

            int index = data.Hero_Id.FindIndex(o => o == heroId);

            return index;
        }

        public bool IsRedDotActive()
        {
            return HaveAward() || HaveCanGetRewardTask();
        }
        public bool HaveAward()
        {
            if (Sys_BattlePass.Instance.Info == null)
                return false;

            int count = Sys_BattlePass.Instance.Info.AwardList.Count;

            bool result = false;

            for (int i = 0; i < count; i++)
            {
                var data = Sys_BattlePass.Instance.Info.AwardList[i];


                var levelreward = GetLevelReward(data.Id);

                if ((data.Common == 0 && levelreward.NormalReward.Count > 0) || (data.Token == 0 && isVip && levelreward.VipReward.Count > 0))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        
        public bool HaveCanGetRewardTask()
        {
            if (Info == null)
                return false;

            int count = Info.DailyTasks.Count;

            for (int i = 0; i < count; i++)
            {
                if (Info.DailyTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                {
                    return true;
                }
            }
            count = Info.WeeklyTasks.Count;

            for (int i = 0; i < count; i++)
            {
                if (Info.WeeklyTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                {
                    return true;
                }
            }


            count = Info.SeasonTasks.Count;

            for (int i = 0; i < count; i++)
            {
                if (Info.SeasonTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                {
                    return true;
                }
            }


            return false;
        }

        public void SortTask()
        {
            if (Info == null)
                return;

            SortTask(Info.DailyTasks);
            SortTask(Info.WeeklyTasks);
            SortTask(Info.SeasonTasks);

        }

        private int SortRule(BattlePassTask value0, BattlePassTask value1)
        {
            if (value0.Status == value1.Status)
            {
                if (value0.TaskId > value1.TaskId)
                    return 1;
                else if (value0.TaskId < value1.TaskId)
                    return -1;
                return 0;
            }

            if (value0.Status == 0)
            {
                return value1.Status == 1 ? 1 : -1;
            }
            else if (value0.Status == 1)
            {
                return -1;
            }
            else if (value0.Status == 2)
            {
               return 1; 
            }

            return 0;

        }
        public void SortTask(RepeatedField<global::Packet.BattlePassTask> list)
        {
            List<BattlePassTask> tasks = new List<BattlePassTask>(list);

            tasks.Sort(SortRule);

            list.Clear();

            list.AddRange(tasks);
        }
    }

    public partial class Sys_BattlePass : SystemModuleBase<Sys_BattlePass>
    {
        private void OnUpdateOperatinalActivityShowOrHide()
        {
            UpdateActive();
        }
        private void OnRoleLevelUp()
        {
            UpdateActive();
        }
        private void OnActiveNtf(NetMsg msg)
        {
            CmdBattlePassOnOffNtf netinfo = NetMsgUtil.Deserialize<CmdBattlePassOnOffNtf>(CmdBattlePassOnOffNtf.Parser, msg);

            

            //开启了另外一个活动
            if (ActiveNtfInfo != null && ActiveNtfInfo.ActivityId != netinfo.ActivityId)
            {
                Info = new CmdBattlePassInfoNtf();
                
                SendGetInfo(netinfo.ActivityId);

                m_isActive = false;

                if (UIManager.IsOpen(EUIID.UI_BattlePass))
                {
                    UIManager.CloseUI(EUIID.UI_BattlePass);
                }

                eventEmitter.Trigger(EEvents.StartNewActivity);
            }

            ActiveNtfInfo = netinfo;

            UpdateActive();

        }
        private void OnInfoNtf(NetMsg msg)
        {
            CmdBattlePassInfoNtf netinfo = NetMsgUtil.Deserialize<CmdBattlePassInfoNtf>(CmdBattlePassInfoNtf.Parser, msg);

            Info = netinfo;

            UpdateActive();
        }

        private void OnRestTasksNtf(NetMsg msg)
        {
            CmdBattlePassResetTasksNtf info = NetMsgUtil.Deserialize<CmdBattlePassResetTasksNtf>(CmdBattlePassResetTasksNtf.Parser, msg);

            int count = info.DailyTasks.Count;
            for (int i = 0; i < count; i++)
                RestDailyTask(info.DailyTasks[i]);

            count = info.WeeklyTasks.Count;
            for (int i = 0; i < count; i++)
                RestWeekTask(info.WeeklyTasks[i]);

            count = info.SeasonTasks.Count;
            for (int i = 0; i < count; i++)
                RestSeasonTask(info.SeasonTasks[i]);
            
            SortTask();

            eventEmitter.Trigger(EEvents.TaskReset);

        }

        private void RestDailyTask(uint taskid)
        {
            var value = Info.DailyTasks.Find(o => o.TaskId == taskid);

            if (value == null)
                return;

            value.Process = 0;
            value.Status = 0;
        }

        private void RestWeekTask(uint taskid)
        {
            var value = Info.WeeklyTasks.Find(o => o.TaskId == taskid);

            if (value == null)
                return;

            value.Process = 0;
            value.Status = 0;
        }

        private void RestSeasonTask(uint taskid)
        {
            var value = Info.SeasonTasks.Find(o => o.TaskId == taskid);

            if (value == null)
                return;

            value.Process = 0;
            value.Status = 0;
        }

        private void SetLevel(uint newLevel)
        {
            uint level = Info.Level;

            if (level < newLevel)
            {
                for (uint i = 1; i <= (newLevel - Info.Level); ++i)
                {
                    Info.AwardList.Add(new LevelAward() { Id = level + i, Common = 0, Token = 0 });
                }
            }

            Info.Level = newLevel;


        }
        private void OnExpNtf(NetMsg msg)
        {
            var lastLevel = Info.Level;

            CmdBattlePassExpNtf info = NetMsgUtil.Deserialize<CmdBattlePassExpNtf>(CmdBattlePassExpNtf.Parser, msg);

            Info.Exp = info.Exp;

            SetLevel(info.Level);

            eventEmitter.Trigger(EEvents.LevelExpChange);

            if (Info.Level != lastLevel)
            {
                if (info.Type == 0)
                    UIManager.OpenUI(EUIID.UI_BattlePass_SpecialLv, false, new UI_BattlePass_LevelUp_Parmas() { LastLevel = lastLevel, Level = info.Level });
                else if (info.Type == 1)
                    VipLastLevel = (int)lastLevel;

                eventEmitter.Trigger(EEvents.RedState);
            }

           
        }

        /// <summary>
        /// 领取战斗升级奖励
        /// </summary>
        /// <param name="level"></param>
        public void OnGetBPLAward(NetMsg msg)
        {
            CmdBattlePassGetBPLAwardRes info = NetMsgUtil.Deserialize<CmdBattlePassGetBPLAwardRes>(CmdBattlePassGetBPLAwardRes.Parser, msg);

            

            var data = Info.AwardList.Find(o => o.Id == info.Level);

            if (data != null && data.Id != 0)
            {
                data.Common = 1;

                if (isVip)
                    data.Token = 1;
            }

            eventEmitter.Trigger<uint>(EEvents.LevelRewardRes, info.Level);

            eventEmitter.Trigger(EEvents.RedState);
        }
        /// <summary>
        /// 领取全部战斗战令升级奖励
        /// </summary>
        public void OnGetBPLAwardAll(NetMsg msg)
        {
            CmdBattlePassGetBPLAllAwardRes info = NetMsgUtil.Deserialize<CmdBattlePassGetBPLAllAwardRes>(CmdBattlePassGetBPLAllAwardRes.Parser, msg);

            eventEmitter.Trigger(EEvents.OneKeyLevelRewardRes);

            Info.AwardList.Clear();

            eventEmitter.Trigger(EEvents.RedState);
        }

        /// <summary>
        /// 购买付费战令 
        /// </summary>
        public void OnBuyBattlePass(NetMsg msg)
        {
            CmdBattlePassBuyBattlePassRes info = NetMsgUtil.Deserialize<CmdBattlePassBuyBattlePassRes>(CmdBattlePassBuyBattlePassRes.Parser, msg);

            Info.BattlePassType = info.Type;// (uint)BattlePassType.Advance;

            var newAwardlist = new List<LevelAward>();

            var lastAwardlist = new List<LevelAward>(Info.AwardList);

            for (uint i = 1; i <= Info.Level; i++)
            {
                var data = CSVBattlePassUpgrade.Instance.GetConfData(i);

                var sdata = lastAwardlist.Find(o => o.Id == i);

                newAwardlist.Add(new LevelAward() { Id = i, Common = sdata == null ? 1:sdata.Common });

            }

            Info.AwardList.Clear();

            Info.AwardList.AddRange(newAwardlist);


            eventEmitter.Trigger(EEvents.BattlePassTypeChange);

            eventEmitter.Trigger(EEvents.RedState);
        }

        /// <summary>
        /// 购买战令等级
        /// </summary>
        /// <param name="level"></param>
        public void OnBuyBattlePassLevel(NetMsg msg)
        {
            CmdBattlePassBuyBattlePassLevelRes info = NetMsgUtil.Deserialize<CmdBattlePassBuyBattlePassLevelRes>(CmdBattlePassBuyBattlePassLevelRes.Parser, msg);

            SetLevel(info.CurLevel); 

            eventEmitter.Trigger(EEvents.UnLockNewLevelReward);

            eventEmitter.Trigger(EEvents.RedState);
        }
        /// <summary>
        /// 领取战令任务奖励
        /// </summary>
     
        public void OnGetBPTAward(NetMsg msg)
        {
            CmdBattlePassGetBPTAwardRes info = NetMsgUtil.Deserialize<CmdBattlePassGetBPTAwardRes>(CmdBattlePassGetBPTAwardRes.Parser, msg);

            bool result = false;
            int count = Info.DailyTasks.Count;
            for (int i = 0; i < count; i++)
            {
                if (info.TaskId == Info.DailyTasks[i].TaskId &&  Info.DailyTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                {
                    Info.DailyTasks[i].Status = (uint)BattlePassTaskAwardStatus.Get;

                    result = true;
                    break;
                }
            }

            if (result == false)
            {
                count = Info.WeeklyTasks.Count;
                for (int i = 0; i < count; i++)
                {
                    if (info.TaskId == Info.WeeklyTasks[i].TaskId && Info.WeeklyTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                    {
                        Info.WeeklyTasks[i].Status = (uint)BattlePassTaskAwardStatus.Get;

                        result = true;
                        break;
                    }
                }
            }


            if (result == false)
            {
                count = Info.SeasonTasks.Count;
                for (int i = 0; i < count; i++)
                {
                    if (info.TaskId == Info.SeasonTasks[i].TaskId && Info.SeasonTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                    {
                        Info.SeasonTasks[i].Status = (uint)BattlePassTaskAwardStatus.Get;

                        result = true;
                        break;
                    }
                }
            }

            SortTask();

            eventEmitter.Trigger<uint>(EEvents.GetTaskReward, info.TaskId);

            eventEmitter.Trigger(EEvents.RedState);
        }

        /// <summary>
        /// 领取全部战令任务奖励
        /// </summary>
        /// <param name="id"></param>
        public void OnGetBPTAwardAll(NetMsg msg)
        {
            CmdBattlePassGetBPTAllAwardRes info = NetMsgUtil.Deserialize<CmdBattlePassGetBPTAllAwardRes>(CmdBattlePassGetBPTAllAwardRes.Parser, msg);

            int count = Info.DailyTasks.Count;
            for (int i = 0; i < count; i++)
            {
                if (Info.DailyTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                {
                    Info.DailyTasks[i].Status = (uint)BattlePassTaskAwardStatus.Get;
                }
            }

            count = Info.WeeklyTasks.Count;
            for (int i = 0; i < count; i++)
            {
                if (Info.WeeklyTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                {
                    Info.WeeklyTasks[i].Status = (uint)BattlePassTaskAwardStatus.Get;
                }
            }

            count = Info.SeasonTasks.Count;
            for (int i = 0; i < count; i++)
            {
                if (Info.SeasonTasks[i].Status == (uint)BattlePassTaskAwardStatus.NoGet)
                {
                    Info.SeasonTasks[i].Status = (uint)BattlePassTaskAwardStatus.Get;
                }
            }

            SortTask();

            eventEmitter.Trigger(EEvents.GetAllTaskReward);

            eventEmitter.Trigger(EEvents.RedState);
        }

        /// <summary>
        /// 获取日常周常日志信息
        /// </summary>
        public void OnGetTaskInfo(NetMsg msg)
        {
            CmdBattlePassGetTasksInfoRes info = NetMsgUtil.Deserialize<CmdBattlePassGetTasksInfoRes>(CmdBattlePassGetTasksInfoRes.Parser, msg);

            Info.RefreshTime = info.RefreshTime + 86400u;

            Info.DailyTasks.Clear();
            Info.DailyTasks.AddRange(info.DailyTasks);

            Info.WeeklyTasks.Clear();
            Info.WeeklyTasks.AddRange(info.WeeklyTasks);


            Info.SeasonTasks.Clear();
            Info.SeasonTasks.AddRange(info.SeasonTasks);

            SortTask();

            eventEmitter.Trigger(EEvents.TaskReset);

            eventEmitter.Trigger(EEvents.RedState);
        }


        public void OnGetTaskProcessNtf(NetMsg msg)
        {
            CmdBattlePassTaskProcessNtf info = NetMsgUtil.Deserialize<CmdBattlePassTaskProcessNtf>(CmdBattlePassTaskProcessNtf.Parser, msg);

            if (info.DailyTasks.TaskId > 0)
            {
               var dtask = Info.DailyTasks.Find(o => o.TaskId == info.DailyTasks.TaskId);
                dtask.Process = info.DailyTasks.Process;
                dtask.Status = info.DailyTasks.Status;
            }

            if (info.WeeklyTasks.TaskId > 0)
            {
                var wtask = Info.WeeklyTasks.Find(o => o.TaskId == info.WeeklyTasks.TaskId);
                wtask.Process = info.WeeklyTasks.Process;
                wtask.Status = info.WeeklyTasks.Status;
            }


            if (info.SeasonTasks.TaskId > 0)
            {
                var wtask = Info.SeasonTasks.Find(o => o.TaskId == info.SeasonTasks.TaskId);
                wtask.Process = info.SeasonTasks.Process;
                wtask.Status = info.SeasonTasks.Status;
            }

            eventEmitter.Trigger(EEvents.TaskProcessChange); 
        }

        private void OnGetInfoRes(NetMsg msg)
        {
            CmdBattlePassGetBattlePassInfoRes info = NetMsgUtil.Deserialize<CmdBattlePassGetBattlePassInfoRes>(CmdBattlePassGetBattlePassInfoRes.Parser, msg);

            Info.Exp = info.Exp;
            Info.Level = info.Level;
            Info.BattlePassType = info.BattlePassType;
            Info.AwardList.Clear();
            Info.AwardList.AddRange(info.AwardList);

            Info.DailyTasks.Clear();
            Info.DailyTasks.AddRange(info.DailyTasks);

            Info.WeeklyTasks.Clear();
            Info.WeeklyTasks.AddRange(info.WeeklyTasks);

            Info.SeasonTasks.Clear();
            Info.SeasonTasks.AddRange(info.SeasonTasks);

            Info.RefreshTime = info.RefreshTime;

            SortTask();

            eventEmitter.Trigger(EEvents.UpdateInfo);

            eventEmitter.Trigger(EEvents.RedState);
        }

        private void OnRedStateTips(NetMsg msg)
        {
            CmdBattlePassRedTipsNtf info = NetMsgUtil.Deserialize<CmdBattlePassRedTipsNtf>(CmdBattlePassRedTipsNtf.Parser, msg);

            if (Info == null)
                return;

            int count = info.TipInfos.Count;

            for (int i = 0; i < count; i++)
            {
                var data = info.TipInfos[i];

                RepeatedField<BattlePassTask> list;
                
                switch (data.Type)
                {
                    case 1:
                        list = Info.DailyTasks;
                        break;
                    case 2:
                        list = Info.WeeklyTasks;
                        break;
                    case 4:
                        list = Info.SeasonTasks;
                        break;
                    default:
                        list = new RepeatedField<BattlePassTask>();
                        break;
                }
 
                var dresult =  list.Find(o => o.TaskId == data.TaskId);

                if (dresult != null)
                {
                   var taskData = CSVBattlePassTaskGroup.Instance.GetConfData(dresult.TaskId);
                   dresult.Process =(uint) (taskData.ReachTypeAchievement.Count == 0 ? 0 : taskData.ReachTypeAchievement[taskData.ReachTypeAchievement.Count - 1]);
                   dresult.Status = (uint)BattlePassTaskAwardStatus.NoGet;
                }
                
            }

            eventEmitter.Trigger(EEvents.RedState);
        }
    }

    public partial class Sys_BattlePass : SystemModuleBase<Sys_BattlePass>
    {
        
        public void SendGetInfo()
        {


            SendGetInfo(BranchID);


        }

        public void SendGetInfo(uint id)
        {
    

            CmdBattlePassGetBattlePassInfoReq info = new CmdBattlePassGetBattlePassInfoReq();

            info.ActivityId = id;

            NetClient.Instance.SendMessage((ushort)CmdBattlePass.GetBattlePassInfoReq, info);


        }
        /// <summary>
        /// 领取战斗升级奖励
        /// </summary>
        /// <param name="level"></param>
        public void SendGetBPLAward(uint level)
        {
            if (isActive == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024403u));
                return;
            }
            CmdBattlePassGetBPLAwardReq info = new CmdBattlePassGetBPLAwardReq() { Level = level };

            info.ActivityId = BranchID;

            NetClient.Instance.SendMessage((ushort) CmdBattlePass.GetBplawardReq, info);
        }
        /// <summary>
        /// 领取全部战斗战令升级奖励
        /// </summary>
        public void SendGetBPLAwardAll()
        {
            if (isActive == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024403u));
                return;
            }

            CmdBattlePassGetBPLAllAwardReq info = new CmdBattlePassGetBPLAllAwardReq();

            info.ActivityId = BranchID;

            NetClient.Instance.SendMessage((ushort)CmdBattlePass.GetBplallAwardReq, info);
        }

        /// <summary>
        /// 购买付费战令 type 1 基础 2 进阶
        /// </summary>
        public void SendBuyBattlePass(uint type)
        {
            if (isActive == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024406u));
                return;
            }
            CmdBattlePassBuyBattlePassReq info = new CmdBattlePassBuyBattlePassReq() { Type = type};

            info.ActivityId = BranchID;

            NetClient.Instance.SendMessage((ushort)CmdBattlePass.BuyBattlePassReq, info);
        }

        /// <summary>
        /// 购买战令等级
        /// </summary>
        /// <param name="level"></param>
        public void SendBuyBattlePassLevel(uint level)
        {
            if (isActive == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024404u));
                return;
            }
            CmdBattlePassBuyBattlePassLevelReq info = new CmdBattlePassBuyBattlePassLevelReq() { Level = level };

            info.ActivityId = BranchID;

            NetClient.Instance.SendMessage((ushort)CmdBattlePass.BuyBattlePassLevelReq, info);
        }
        /// <summary>
        /// 领取战令任务奖励
        /// </summary>
        /// <param name="id"></param>
        public void SendGetBPTAward(uint taskid,uint type)
        {
            if (isActive == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024405u));
                return;
            }
            CmdBattlePassGetBPTAwardReq info = new CmdBattlePassGetBPTAwardReq() { TaskId = taskid ,Type = type};

            info.ActivityId = BranchID;

            NetClient.Instance.SendMessage((ushort)CmdBattlePass.GetBptawardReq, info);
        }

        /// <summary>
        /// 领取全部战令任务奖励
        /// </summary>
        /// <param name="id"></param>
        public void SendGetBPTAwardAll()
        {
            if (isActive == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024405u));
                return;
            }
            CmdBattlePassGetBPTAllAwardReq info = new CmdBattlePassGetBPTAllAwardReq();
            info.ActivityId = BranchID;

            NetClient.Instance.SendMessage((ushort)CmdBattlePass.GetBptallAwardReq, info);
        }

        /// <summary>
        /// 获取日常周常日志信息
        /// </summary>
        public void SendGetTaskInfo()
        {
            var nowtime = Sys_Time.Instance.GetServerTime();

            //if (nowtime < Info.RefreshTime)
            //{
            //    return;
            //}

            CmdBattlePassGetTasksInfoReq info = new CmdBattlePassGetTasksInfoReq();

            info.ActivityId = BranchID;

            NetClient.Instance.SendMessage((ushort)CmdBattlePass.GetTasksInfoReq, info);
        }

        public void SendBuyShop(uint id, int count)
        {
            if (isActive == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024407u));
                return;
            }

            Sys_Mall.Instance.OnBuyReq(id, (uint)count);
        }



    }
}
