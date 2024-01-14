using System;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;
using System.Collections.Generic;

namespace Logic {

    public partial class Sys_FamilyBoss : SystemModuleBase<Sys_FamilyBoss>
    {
        public uint State = 0; //0 未开启阶段，1 开启阶段，2结算阶段
        public uint NextTime = 0;

        private uint m_bossCreateTime;
        //private string scorePowerStr;
        private List<List<uint>> m_listPowerConf;
        //private uint m_attackBossCD;
        //private uint m_attackRoleCD;

        private List<CmdRankRole> m_listRankRoles = new List<CmdRankRole>();
        private List<CmdRankGuild> m_listRankGuilds = new List<CmdRankGuild>();
        private List<CmdRankRole> m_listSeizeRoles = new List<CmdRankRole>();
 
        public uint AttackBossCD { set; get; }
        public uint AttackRoleCD { set; get; }
        public uint MyRank { set; get; }
        public uint MyScore { set; get; }
        public uint MyGuildScore { set; get; }
        public uint MyGuildRank { set; get; }
        public uint MyGuildAttendNum { set; get; }

        //public bool IsFamilyBossBattle { set; get; }

        public uint AttackEndRet { set; get; }
        public uint AttackAddScore { get; set; }
        public uint AttackDmg { get; set; }

        public uint ActivityGroup { get; set; }

        //排行奖励预览 
        public class RankRewardShow
        {
            public uint[] arrRank;
            public uint dropId;
        }
        //private CmdGuildBossEndNtf bossEndNtf;

        public uint GetScorePower()
        {
            uint scorePower = 0;
            uint time = Sys_Time.Instance.GetServerTime() - m_bossCreateTime;

            for (int i = m_listPowerConf.Count - 1; i >= 0; --i)
            {
                if (time >= m_listPowerConf[i][0])
                {
                    scorePower = m_listPowerConf[i][1];
                    break;
                }
            }

            return scorePower;
        }

        public bool GetPowerLeftTime(ref uint leftTime, ref uint sliderTime)
        {
            uint duration = CSVDailyActivity.Instance.GetConfData(112u).Duration;
            uint endTime = duration + m_bossCreateTime;
            if (Sys_Time.Instance.GetServerTime() >= endTime)
            {
                leftTime = 0u;
                sliderTime = 0u;
                return false;
            }
            else
            {
                uint releaseTime = Sys_Time.Instance.GetServerTime() - m_bossCreateTime; //活动流逝时间
                if (releaseTime >= duration)
                {
                    leftTime = 0;
                    sliderTime = 0;
                    return false;
                }

                for (int i = m_listPowerConf.Count - 1; i >= 0; --i)
                {
                    if (releaseTime >= m_listPowerConf[i][0])
                    {
                        if (i == m_listPowerConf.Count - 1)
                        {
                            leftTime = duration - releaseTime;
                            sliderTime = duration - m_listPowerConf[i][0];
                        }
                        else
                        {
                            leftTime = m_listPowerConf[i + 1][0] - releaseTime;
                            sliderTime = m_listPowerConf[i + 1][0];
                        }
                        break;
                    }
                }

                return true;
            }
        }

        public uint GetLeftTime()
        {
            uint releaseTime = Sys_Time.Instance.GetServerTime() - m_bossCreateTime;
            uint duration = CSVDailyActivity.Instance.GetConfData(112u).Duration;
            if (releaseTime > duration)
                return 0u;
            else
                return duration - releaseTime;
        }

        public List<CmdRankRole> GetRankRoles()
        {
            return m_listRankRoles;
        }

        public List<CmdRankGuild> GetRankGuilds()
        {
            return m_listRankGuilds;
        }

        public List<CmdRankRole> GetSeizeRoles()
        {
            return m_listSeizeRoles;
        }

        public List<RankRewardShow> GetRankRewardShow(int index)
        {
            List<RankRewardShow> list = new List<RankRewardShow>();

            if (index == 0) //0 个人排行； 1 家族排行
            {
                var familyBossRankRewards = CSVFamilyBossRankReward.Instance.GetAll();
                for (int i = 0, len = familyBossRankRewards.Count; i < len; ++i)
                {
                    if (familyBossRankRewards[i].Activity_Group == ActivityGroup)
                    {
                        RankRewardShow show = new RankRewardShow();
                        show.arrRank = new uint[2];
                        show.arrRank[0] = 1;
                        show.arrRank[1] = familyBossRankRewards[i].Presonal_Rank;
                        show.dropId = familyBossRankRewards[i].Reward;

                        if (list.Count > 0)
                            show.arrRank[0] = list[list.Count - 1].arrRank[1] + 1;

                        list.Add(show);
                    }
                }
            }
            else
            {
                var familyBossFamilyRewards = CSVFamilyBossFamilyReward.Instance.GetAll();
                for (int i = 0, len = familyBossFamilyRewards.Count; i < len; ++i)
                {
                    RankRewardShow show = new RankRewardShow();
                    show.arrRank = new uint[2];
                    show.arrRank[0] = 1;
                    show.arrRank[1] = familyBossFamilyRewards[i].Family_Rank;
                    show.dropId = familyBossFamilyRewards[i].Reward;

                    if (list.Count > 0)
                        show.arrRank[0] = list[list.Count - 1].arrRank[1] + 1;

                    list.Add(show);
                }
            }

            return list;
        }

        //public bool IsAttackBossCD()
        //{
        //    return Sys_Time.Instance.GetServerTime() < AttackBossCD;
        //}

        //public bool IsAttackRoleCD()
        //{
        //    return Sys_Time.Instance.GetServerTime() < AttackRoleCD;
        //}

        //public void ReadFile()
        //{
        //    bossEndNtf = null;
        //    byte[] bytes = FileStore.ReadBytes(fileName);
        //    if (bytes != null)
        //    {
        //        NetMsgUtil.TryDeserialize<CmdGuildBossEndNtf>(CmdGuildBossEndNtf.Parser, bytes, out bossEndNtf);
        //    }
        //}

        //private string fileName = "FamilyBoss";
        //public void WriteFile(CmdGuildBossEndNtf data)
        //{
        //    FileStore.WriteBytes(fileName, NetMsgUtil.Serialzie(data));
        //}
    }
}


