using Logic.Core;
using Lib.Core;
using Packet;
using Net;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class Sys_JSBattle : SystemModuleBase<Sys_JSBattle>
    {
        public enum EEvents
        {
            ChallengeInfoRefresh, //挑战对手刷新
            BuyTimesEnd, //购买次数刷新
            RecordDataRefresh, //战斗数据刷新
            GetRewardEnd, //奖励领取结束
            GetRankListEnd, //获取排行数据
        }
        /// <summary> 事件列表 </summary>
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public readonly int OnePageDatasNum = 50;
        public int maxNum = 0;
        public int MaxNum
        {
            get
            {
                if(maxNum == 0)
                {
                    var configData = CSVDecisiveArenaParam.Instance.GetConfData(3u);
                    maxNum = ReadHelper.ReadInt(configData.str_value);
                }
                return maxNum;
            }
        }

        public int MaxPage
        {
            get
            {
                return MaxNum / OnePageDatasNum;
            }
        }

        public int battleType = 0;
        public int BattleType
        {
            get
            {
                if (battleType == 0)
                {
                    var configData = CSVDecisiveArenaParam.Instance.GetConfData(10u);
                    battleType = ReadHelper.ReadInt(configData.str_value);
                }
                return battleType;
            }
        }

        public bool IsHigh = false;

        public uint quickCount = 0;
        public override void Init()
        {
            AddListeners();
        }

        public override void OnLogin()
        {
            IsHigh = false;
            IsBattleOver = false;
            cmdBattleEndNtf = null;
            nextRefreshTime = 0;
            victoryArenaRankUnits.Clear();
            victoryArenaFightRecords.Clear();
        }

        public override void OnLogout()
        {
            nextRefreshTime = 0;
            victoryArenaRankUnits.Clear();
            victoryArenaFightRecords.Clear();
        }

        private void AddListeners()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdVictoryArena.DataNty, OnVictoryArenaDataNty, CmdVictoryArenaDataNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdVictoryArena.DailyUpdateNty, OnVictoryArenaDailyUpdateNty, CmdVictoryArenaDailyUpdateNty.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdVictoryArena.HighestRankUpdateNty, OnVictoryArenaHighestRankUpdateNty, CmdVictoryArenaHighestRankUpdateNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVictoryArena.ReceiveRankRewardReq, (ushort)CmdVictoryArena.ReceiveRankRewardRes, OnVictoryArenaReceiveRankRewardRes, CmdVictoryArenaReceiveRankRewardRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVictoryArena.PullFightRecordReq, (ushort)CmdVictoryArena.PullFightRecordRes, OnVictoryArenaPullFightRecordRes, CmdVictoryArenaPullFightRecordRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdVictoryArena.SystemUpdateNty, OnVictoryArenaSystemUpdateNty, CmdVictoryArenaSystemUpdateNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVictoryArena.RankListReq, (ushort)CmdVictoryArena.RankListRes, OnVictoryArenaRankListRes, CmdVictoryArenaRankListRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdVictoryArena.RefreshNty, OnVictoryArenaRefreshNty, CmdVictoryArenaRefreshNty.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdVictoryArena.FastChallengeReq, (ushort)CmdVictoryArena.FastChallengeRes, OnVictoryArenaFastChallengeRes, CmdVictoryArenaFastChallengeRes.Parser);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnBattleOver, OnBattleOver, true);
            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnBattleSettlement, this.OnEndBattle, true);
            Sys_Fight.Instance.OnEnterFight += this.OnEnterFight;
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.BeMember, OnBeMember, true);
        }

        #region 服务器协议数据

        /// <summary> 每日刷新数据 </summary>
        private RoleVictoryArenaDaily arenaDaily;
        /// <summary> 奖励数据 </summary>
        private RoleVictoryArenaReward arenaReward;
        /// <summary> 系统数据 </summary>
        private SystemVictoryArena arenaSystemData;

        private void OnVictoryArenaDataNty(NetMsg msg)
        {
            CmdVictoryArenaDataNty nty = NetMsgUtil.Deserialize<CmdVictoryArenaDataNty>(CmdVictoryArenaDataNty.Parser, msg);
            arenaDaily = nty.Daily;
            arenaReward = nty.Reward;
            arenaSystemData = nty.System;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasReward, null);
        }

        /// <summary>
        /// 请求挑战对手
        /// </summary>
        /// <param name="index"> 当前对手信息列表的下标(0~2) </param>
        public void VictoryArenaChallengeReq(uint index)
        {
            CmdVictoryArenaChallengeReq req = new CmdVictoryArenaChallengeReq();
            req.Index = index;
            NetClient.Instance.SendMessage((ushort)CmdVictoryArena.ChallengeReq, req);
        }

        /// <summary>
        /// 请求刷新数据-换一批
        /// </summary>
        public void VictoryArenaRefreshReq()
        {
            if(Sys_Time.Instance.GetServerTime() > nextRefreshTime)
            {
                CmdVictoryArenaRefreshReq req = new CmdVictoryArenaRefreshReq();
                NetClient.Instance.SendMessage((ushort)CmdVictoryArena.RefreshReq, req);
            }
            else
            {
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasRecord, null);
                RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasReward, null);
                Instance.eventEmitter.Trigger(EEvents.ChallengeInfoRefresh);
            }
        }

        private uint nextRefreshTime;//下次可刷新时间
        private uint myRank;//我的排名
        public bool showNewDefence;//显示新防守记录
        public List<VictoryArenaOppoUnit> oppoList = new List<VictoryArenaOppoUnit>();//我的对手列表
        private void OnVictoryArenaRefreshNty(NetMsg msg)
        {
            CmdVictoryArenaRefreshNty nty = NetMsgUtil.Deserialize<CmdVictoryArenaRefreshNty>(CmdVictoryArenaRefreshNty.Parser, msg);
            nextRefreshTime = nty.NextRefreshTime;
            myRank = nty.Rank;
            oppoList.Clear();
            oppoList.AddRange(nty.OppoList);
            showNewDefence = nty.NewDefencerRecord;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasRecord, null);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasReward, null);
            Instance.eventEmitter.Trigger(EEvents.ChallengeInfoRefresh);
        }

        /// <summary>
        /// 购买次数
        /// </summary>
        public void VictoryArenaBuyChallengeTimesReq()
        {
            CmdVictoryArenaBuyChallengeTimesReq req = new CmdVictoryArenaBuyChallengeTimesReq();
            NetClient.Instance.SendMessage((ushort)CmdVictoryArena.BuyChallengeTimesReq, req);
        }

        /// <summary>
        /// 每日数据更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnVictoryArenaDailyUpdateNty(NetMsg msg)
        {
            CmdVictoryArenaDailyUpdateNty nty = NetMsgUtil.Deserialize<CmdVictoryArenaDailyUpdateNty>(CmdVictoryArenaDailyUpdateNty.Parser, msg);
            arenaDaily = nty.Daily;
            Instance.eventEmitter.Trigger(EEvents.BuyTimesEnd);
        }

        /// <summary>
        /// 历史最高排名更新
        /// </summary>
        /// <param name="msg"></param>
        private void OnVictoryArenaHighestRankUpdateNty(NetMsg msg)
        {
            CmdVictoryArenaHighestRankUpdateNty nty = NetMsgUtil.Deserialize<CmdVictoryArenaHighestRankUpdateNty>(CmdVictoryArenaHighestRankUpdateNty.Parser, msg);
            IsHigh = arenaReward.HighestRank > nty.HighestRank;
            arenaReward.HighestRank = nty.HighestRank;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasReward, null);
        }

        /// <summary>
        /// 请求领取排名奖励
        /// </summary>
        public void VictoryArenaReceiveRankRewardReq(uint rewardId)
        {
            CmdVictoryArenaReceiveRankRewardReq req = new CmdVictoryArenaReceiveRankRewardReq();
            req.RewardId = rewardId;
            NetClient.Instance.SendMessage((ushort)CmdVictoryArena.ReceiveRankRewardReq, req);
        }

        /// <summary>
        /// 领取排名奖励返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnVictoryArenaReceiveRankRewardRes(NetMsg msg)
        {
            CmdVictoryArenaReceiveRankRewardRes nty = NetMsgUtil.Deserialize<CmdVictoryArenaReceiveRankRewardRes>(CmdVictoryArenaReceiveRankRewardRes.Parser, msg);
            var rewardId = nty.RewardId;
            SetBitvalue(rewardId);
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasReward, null);
            Instance.eventEmitter.Trigger(EEvents.GetRewardEnd); 
        }

        /// <summary>
        /// 请求获取战斗纪录
        /// </summary>
        public void VictoryArenaPullFightRecordReq()
        {
            CmdVictoryArenaPullFightRecordReq req = new CmdVictoryArenaPullFightRecordReq();
            NetClient.Instance.SendMessage((ushort)CmdVictoryArena.PullFightRecordReq, req);
        }

        private List<VictoryArenaFightRecord> victoryArenaFightRecords = new List<VictoryArenaFightRecord>();
        /// <summary>
        /// 战斗纪录获取返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnVictoryArenaPullFightRecordRes(NetMsg msg)
        {
            CmdVictoryArenaPullFightRecordRes nty = NetMsgUtil.Deserialize<CmdVictoryArenaPullFightRecordRes>(CmdVictoryArenaPullFightRecordRes.Parser, msg);
            if(victoryArenaFightRecords.Count == 5)
            {                 
                var index = victoryArenaFightRecords.Count - nty.Records.Count;
                victoryArenaFightRecords.RemoveRange(index, nty.Records.Count);
                for (int i = 0; i < nty.Records.Count; i++)
                {
                    victoryArenaFightRecords.Insert(0, nty.Records[i]);
                }
                //victoryArenaFightRecords.AddRange(nty.Records);
            }
            else if(victoryArenaFightRecords.Count + nty.Records.Count > 5)
            {
                var needRemoveCount = victoryArenaFightRecords.Count - (5 - nty.Records.Count);
                var index = (5 - nty.Records.Count);
                victoryArenaFightRecords.RemoveRange(index, needRemoveCount);
                for (int i = 0; i < nty.Records.Count; i++)
                {
                    victoryArenaFightRecords.Insert(0, nty.Records[i]);
                }
                //victoryArenaFightRecords.AddRange(nty.Records);
            }
            else
            {
                for (int i = 0; i < nty.Records.Count; i++)
                {
                    victoryArenaFightRecords.Insert(0, nty.Records[i]);
                }
            }
            showNewDefence = false;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnJSBattleHasRecord, null);
            Instance.eventEmitter.Trigger(EEvents.RecordDataRefresh);
        }

        /// <summary>
        /// 反击战斗
        /// </summary>
        /// <param name="roleId"></param>
        public void VictoryArenaFightBackReq(ulong roleId)
        {
            CmdVictoryArenaFightBackReq req = new CmdVictoryArenaFightBackReq();
            req.OppoRoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdVictoryArena.FightBackReq, req);
        }

        /// <summary>
        /// 竞技场系统数据通知
        /// </summary>
        /// <param name="msg"></param>
        private void OnVictoryArenaSystemUpdateNty(NetMsg msg)
        {
            CmdVictoryArenaSystemUpdateNty nty = NetMsgUtil.Deserialize<CmdVictoryArenaSystemUpdateNty>(CmdVictoryArenaSystemUpdateNty.Parser, msg);
            arenaSystemData = nty.System;
            Instance.eventEmitter.Trigger(EEvents.BuyTimesEnd);
        }

        /// <summary>
        /// 获取排行榜数据
        /// </summary>
        /// <param name="page"> 页码(0~19, 一页算10条记录)</param>
        public void VictoryArenaRankListReq(uint page)
        {
            CmdVictoryArenaRankListReq req = new CmdVictoryArenaRankListReq();
            req.Page = page;
            NetClient.Instance.SendMessage((ushort)CmdVictoryArena.RankListReq, req);
        }

        private List<VictoryArenaRankUnit> victoryArenaRankUnits = new List<VictoryArenaRankUnit>(200);
        /// <summary>
        /// 排行榜数据返回
        /// </summary>
        /// <param name="msg"></param>
        private void OnVictoryArenaRankListRes(NetMsg msg)
        {
            CmdVictoryArenaRankListRes nty = NetMsgUtil.Deserialize<CmdVictoryArenaRankListRes>(CmdVictoryArenaRankListRes.Parser, msg);
            int index = (int)nty.Page * OnePageDatasNum;
            if (index >= victoryArenaRankUnits.Count)
            {
                victoryArenaRankUnits.AddRange(nty.List);
            }
            else
            {
                victoryArenaRankUnits.RemoveRange(index, OnePageDatasNum);
                victoryArenaRankUnits.InsertRange(index, nty.List);
            }
            Instance.eventEmitter.Trigger(EEvents.GetRankListEnd);
        }

        /// <summary>
        /// 请求扫荡 次数 大于零得时候才发送
        /// </summary>
        public void VictoryArenaFastChallengeReq()
        {
            quickCount = arenaDaily.LeftChallengeTimes;
            CmdVictoryArenaFastChallengeReq req = new CmdVictoryArenaFastChallengeReq();
            NetClient.Instance.SendMessage((ushort)CmdVictoryArena.FastChallengeReq, req);
        }

        private void OnVictoryArenaFastChallengeRes(NetMsg msg)
        {
            CmdVictoryArenaFastChallengeRes res = NetMsgUtil.Deserialize<CmdVictoryArenaFastChallengeRes>(CmdVictoryArenaFastChallengeRes.Parser, msg);
            
            UIManager.OpenUI(EUIID.UI_JSBattle_Quick_Tip, false, res);
        }

        #endregion

        /// <summary>
        /// 获取系统数据-幸运伙伴种族
        /// </summary>
        /// <returns></returns>
        public SystemVictoryArena GetSysTemVictoryData()
        {
            return arenaSystemData;
        }

        /// <summary>
        /// 获取每日刷新数据-挑战次数-购买次数-刷新时间
        /// </summary>
        /// <returns></returns>
        public RoleVictoryArenaDaily GetRoleVictoryArenaDaily()
        {
            return arenaDaily;
        }

        /// <summary>
        /// 获取历史最高排名,领取奖励的位
        /// </summary>
        /// <returns></returns>
        public RoleVictoryArenaReward GetRoleVictoryArenaReward()
        {
            return arenaReward;
        }

        /// <summary>
        /// 获取我的排名
        /// </summary>
        /// <returns></returns>
        public uint GetMyCurrentRank()
        {
            return myRank;
        }

        /// <summary>
        /// 获取下次换一批时间
        /// </summary>
        /// <returns></returns>
        public uint GetNextChallengesTimes()
        {
            return nextRefreshTime;
        }

        /// <summary>
        /// 获取战斗纪录数据
        /// </summary>
        /// <returns></returns>
        public List<VictoryArenaFightRecord> GetVictoryArenaFightRecords()
        {
            return victoryArenaFightRecords;
        }

        /// <summary>
        /// 检查是否可以反击
        /// </summary>
        /// <returns></returns>
        public bool CheckCanFightRecord(ulong roleId, int index)
        {
            for (int i = 0; i < victoryArenaFightRecords.Count; i++)
            {
                VictoryArenaFightRecord record = victoryArenaFightRecords[i];
                if(roleId == record.Oppo.RoleId && index > i)
                {
                    //最新的纪录是攻击者且赢了 就不能再次反击
                    if(record.IsAttacker)
                    {
                        return !record.Win;
                    }
                }
            }
            return true;
        }

        public bool CheckReward(uint id)
        {
            RoleVictoryArenaReward roleVictoryArenaReward = Sys_JSBattle.Instance.GetRoleVictoryArenaReward();
            if(null != roleVictoryArenaReward)
            {
                int _id = (int)id - 1;
                uint state = roleVictoryArenaReward.ReceivedReward;
                uint value = (state & (1u << _id));
                return value > 0;
            }
            return false;
        }

        public void SetBitvalue(uint id)
        {
            RoleVictoryArenaReward roleVictoryArenaReward = Sys_JSBattle.Instance.GetRoleVictoryArenaReward();
            if (null != roleVictoryArenaReward)
            {
                int bit = (int)id - 1;

                roleVictoryArenaReward.ReceivedReward = roleVictoryArenaReward.ReceivedReward | (1u << bit);
            }
            
        }

        public uint GetCanRewardId()
        {
            uint rewardId = 0;

            var decisiveArenaRankingRedwardDatas = CSVDecisiveArenaRankingRedward.Instance.GetAll();
            for (int i = 0, len = decisiveArenaRankingRedwardDatas.Count; i < len; i++)
            {
                var data = decisiveArenaRankingRedwardDatas[i];
                if (!CheckReward(data.id))
                {
                    rewardId = data.id;
                }
            }

            if (rewardId == 0)
            {
                rewardId = CSVDecisiveArenaRankingRedward.Instance.GetByIndex(CSVDecisiveArenaRankingRedward.Instance.Count - 1).id;
            }

            return rewardId;
        }

        /// <summary>
        /// 获取排行数据
        /// </summary>
        /// <returns></returns>
        public List<VictoryArenaRankUnit> GetVictoryArenaRankUnits()
        {
            return victoryArenaRankUnits;
        }

        /// <summary>
        /// 清空排行数据
        /// </summary>
        public void ClearVictoryArenaRankUnits()
        {
            victoryArenaRankUnits.Clear();
        }

        public uint GetDaliyChallengeTime()
        {
            if (null == arenaDaily)
                return 0;
            return arenaDaily.TodayChallengeTimes;
        }

        public bool HasJSBattleReward()
        {
            if (null == arenaReward)
                return false;
            var rewardId = GetCanRewardId();
            var configData = CSVDecisiveArenaRankingRedward.Instance.GetConfData(rewardId);
            if (null != configData)
            {
                return !CheckReward(rewardId) && arenaReward.HighestRank <= configData.Ranking;
            }
            return false;
        }

        #region BattleEndResult
        
        /// <summary> 战斗结束 </summary>
        private bool IsBattleOver = false;
        private CmdBattleEndNtf cmdBattleEndNtf;
        /// <summary>
        /// 战斗结束
        /// </summary>
        /// <param name="value"></param>
        private void OnBattleOver(bool value)
        {
            IsBattleOver = value;
            OpenJSBattleResultView();
        }

        private void OnEndBattle(CmdBattleEndNtf ntf)
        {
            if(null != ntf && null != ntf.VictoryArena)
            {
                cmdBattleEndNtf = ntf;
                OpenJSBattleResultView();
            }
        }

        /// <summary>
        /// 打开训练结算界面
        /// </summary>
        public void OpenJSBattleResultView()
        {
            if (null != cmdBattleEndNtf && IsBattleOver)
            {
                UIManager.OpenUI(EUIID.UI_JSBattle_Result, false, cmdBattleEndNtf);
                cmdBattleEndNtf = null;
                IsBattleOver = false;
            }
        }
        #endregion

        #region 其他事件
        private void OnEnterFight(CSVBattleType.Data csvBattleType)
        {
            //if (csvBattleType.battle_type == BattleType)
            {
                UIManager.CloseUI(EUIID.UI_JSBattle, true);
                UIManager.CloseUI(EUIID.UI_JSBattle_Record, true);
                UIManager.CloseUI(EUIID.UI_JSBattle_Rank, true);
                UIManager.CloseUI(EUIID.UI_JSBattle_Reward, true);
                UIManager.CloseUI(EUIID.UI_JSBattle_Tips, true);
            }
        }

        /// <summary>
        /// 成为组队成员
        /// </summary>
        private void OnBeMember()
        {
            UIManager.CloseUI(EUIID.UI_JSBattle, true);
            UIManager.CloseUI(EUIID.UI_JSBattle_Record, true);
            UIManager.CloseUI(EUIID.UI_JSBattle_Rank, true);
            UIManager.CloseUI(EUIID.UI_JSBattle_Reward, true);
            UIManager.CloseUI(EUIID.UI_JSBattle_Tips, true);
        }
        #endregion
    }
}