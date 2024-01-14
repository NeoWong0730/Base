using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic {

    public partial class Sys_FamilyBoss : SystemModuleBase<Sys_FamilyBoss> {

        public enum EEvents
        {
            OnBossSimpleInfo,
            OnBossInfo,
            OnBossRankInfo,
            OnSeizeRolesInfo,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public override void Init() {

            EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBoss.SimpleInfoReq, (ushort)CmdGuildBoss.SimpleInfoNtf, this.OnGuildBossSimpleInfoNtf, CmdGuildBossSimpleInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBoss.InfoReq, (ushort)CmdGuildBoss.InfoRes, this.OnGuildBossInfoRes, CmdGuildBossInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBoss.WorldAttackInfoReq, (ushort)CmdGuildBoss.WorldAttackInfoRes, this.OnGuildBossWorldAttackInfoRes, CmdGuildBossWorldAttackInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBoss.RankInfoReq, (ushort)CmdGuildBoss.RankInfoRes, this.OnGuildBossRankInfoRes, CmdGuildBossRankInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildBoss.SelfAttackInfoNtf, this.OnGuildBossSelfAttackInfoNtf, CmdGuildBossSelfAttackInfoNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBoss.GetRoleListReq, (ushort)CmdGuildBoss.GetRoleListRes, this.OnGuildBossGetRoleListRes, CmdGuildBossGetRoleListRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildBoss.AttackEndNtf, this.OnGuildBossAttackEndNtf, CmdGuildBossAttackEndNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildBoss.EndNtf, this.OnGuildBossEndNtf, CmdGuildBossEndNtf.Parser);
            //EventDispatcher.Instance.AddEventListener(0, (ushort)CmdGuildBattle.PunishNty, this.OnRecvPunishNty, CmdGuildBattlePunishNty.Parser);
            //EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBattle.HandInResourceReq, (ushort)CmdGuildBattle.HandInResourceRes, this.OnRecvSubmitRes, CmdGuildBattleHandInResourceReq.Parser);
        }

        public override void OnLogin()
        {
            m_listPowerConf = ReadHelper.ReadArray2_ReadUInt(CSVFamilyBossParam.Instance.GetConfData(1095).str_value, '|', '&');
            Sys_Fight.Instance.OnEnterFight -= OnEnterFight;
            Sys_Fight.Instance.OnEnterFight += OnEnterFight;
        }

        public void OnGuildBossSimpleInfoReq()
        {
            CmdGuildBossSimpleInfoReq req = new CmdGuildBossSimpleInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildBoss.SimpleInfoReq, req);
        }

        private void OnGuildBossSimpleInfoNtf(NetMsg msg)
        {
            
            CmdGuildBossSimpleInfoNtf ntf = NetMsgUtil.Deserialize<CmdGuildBossSimpleInfoNtf>(CmdGuildBossSimpleInfoNtf.Parser, msg);
            //DebugUtil.LogError("OnGuildBossSimpleInfoNtf " + ntf.Luckypartener + "  " + ntf.Luckypet);
            State = ntf.State;
            NextTime = ntf.NextTime;

            eventEmitter.Trigger(EEvents.OnBossSimpleInfo, ntf.Luckypartener, ntf.Luckypet);
        }

        public void OnGuildBossInfoReq()
        {
            //DebugUtil.LogError("OnGuildBossInfoReq " + "  ");
            CmdGuildBossInfoReq req = new CmdGuildBossInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildBoss.InfoReq, req);
        }

        private void OnGuildBossInfoRes(NetMsg msg)
        {
            CmdGuildBossInfoRes bossInfo = NetMsgUtil.Deserialize<CmdGuildBossInfoRes>(CmdGuildBossInfoRes.Parser, msg);

            m_bossCreateTime = bossInfo.BossCreateTime;
            AttackBossCD = bossInfo.AttackBossCdEnd;
            AttackRoleCD = bossInfo.AttackRoleCdEnd;

            //if (m_listRankRoles == null)
            //    m_listRankRoles = new List<CmdRankRole>();
            //m_listRankRoles.Clear();
            //m_listRankRoles.AddRange(bossInfo.RankRoles);
            //foreach (var data in bossInfo.RankRoles)
            //    DebugUtil.LogErrorFormat("OnGuildBossInfoRes res  rank= {0}  roleId= {1}  roleName={2}", data.Rank, data.RoleId, data.RoleName.ToStringUtf8());

            MyRank = bossInfo.MyRank;
            MyScore = bossInfo.MyScore;
            ActivityGroup = bossInfo.Mythird;

            //if (m_listRankGuilds == null)
            //    m_listRankGuilds = new List<CmdRankGuild>();
            //m_listRankGuilds.Clear();
            //m_listRankGuilds.AddRange(bossInfo.)

            eventEmitter.Trigger(EEvents.OnBossInfo);
            //DebugUtil.LogError("OnGuildBossInfoRes "  + "  ");
        }

        public void OnGuildBossWorldAttackInfoReq()
        {
            CmdGuildBossWorldAttackInfoReq req = new CmdGuildBossWorldAttackInfoReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildBoss.WorldAttackInfoReq, req);
        }

        private void OnGuildBossWorldAttackInfoRes(NetMsg msg)
        {
            CmdGuildBossWorldAttackInfoRes res = NetMsgUtil.Deserialize<CmdGuildBossWorldAttackInfoRes>(CmdGuildBossWorldAttackInfoRes.Parser, msg);
            if (res.Infos != null)
            {
                //foreach (AttackInfo info in res.Infos)
                //{
                //    string content = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(3910010175u), info.AttackerName.ToStringUtf8(), info.DefenderName.ToStringUtf8(), info.Score.ToString());
                //    Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content);
                //}
            }
        }

        public void OnGuildBossRankInfoReq(uint type) //1个人 2 帮会
        {
            CmdGuildBossRankInfoReq req = new CmdGuildBossRankInfoReq();
            req.Type = type;
            NetClient.Instance.SendMessage((ushort)CmdGuildBoss.RankInfoReq, req);
        }

        private void OnGuildBossRankInfoRes(NetMsg msg)
        {
            CmdGuildBossRankInfoRes res = NetMsgUtil.Deserialize<CmdGuildBossRankInfoRes>(CmdGuildBossRankInfoRes.Parser, msg);

            //if (m_listRankRoles == null)
            //    m_listRankRoles = new List<CmdRankRole>();
            if (res.Type == 1)
            {
                m_listRankRoles.Clear();
                m_listRankRoles.AddRange(res.RankRoles);
            }

            if (res.Type == 2)
            {
                m_listRankGuilds.Clear();
                m_listRankGuilds.AddRange(res.RankGuilds);
            }


            MyGuildScore = res.GuildScore;
            MyGuildRank = res.GuildRank;
            MyGuildAttendNum = res.AttendNum;
            MyRank = res.Myrank;
            MyScore = res.Myscore;

            eventEmitter.Trigger(EEvents.OnBossRankInfo);
        }

        private void OnGuildBossSelfAttackInfoNtf(NetMsg msg)
        {
            CmdGuildBossSelfAttackInfoNtf ntf = NetMsgUtil.Deserialize<CmdGuildBossSelfAttackInfoNtf>(CmdGuildBossSelfAttackInfoNtf.Parser, msg);

            //别人抢夺我
            if (ntf.LostScore != 0)
            {
                string content = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(3910010177u), ntf.AttackerName.ToStringUtf8(), ntf.LostScore.ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content);
            }

            MyScore = ntf.Score;
            MyRank = ntf.Rank;
            MyGuildScore = ntf.GuildScore;
            MyGuildRank = ntf.GuildRank;
        }

        public void OnGuildBossGetRoleListReq()
        {
            CmdGuildBossGetRoleListReq req = new CmdGuildBossGetRoleListReq();
            NetClient.Instance.SendMessage((ushort)CmdGuildBoss.GetRoleListReq, req);
        }

        private void OnGuildBossGetRoleListRes(NetMsg msg)
        {
            CmdGuildBossGetRoleListRes res = NetMsgUtil.Deserialize<CmdGuildBossGetRoleListRes>(CmdGuildBossGetRoleListRes.Parser, msg);

            //if (m_listSeizeRoles == null)
            //    m_listSeizeRoles = new List<CmdRankRole>();
            m_listSeizeRoles.Clear();
            m_listSeizeRoles.AddRange(res.Roles);

            eventEmitter.Trigger(EEvents.OnSeizeRolesInfo);
        }

        public void OnGuildBossAttackReq(ulong roleId = 0)
        {
            CmdGuildBossAttackReq req = new CmdGuildBossAttackReq();
            if (roleId != 0)
                req.RoleId = roleId;
            NetClient.Instance.SendMessage((ushort)CmdGuildBoss.AttackReq, req);

            //DebugUtil.LogError("OnGuildBossAttackReq " + roleId);
            //IsFamilyBossBattle = false;
        }

        private void OnGuildBossAttackEndNtf(NetMsg msg)
        {
            CmdGuildBossAttackEndNtf ntf = NetMsgUtil.Deserialize<CmdGuildBossAttackEndNtf>(CmdGuildBossAttackEndNtf.Parser, msg);

            if (!(ntf.Targetroleid == Sys_Role.Instance.RoleId))
            {
                if (ntf.Ret != 0)//我抢夺别人
                {
                    //0:打boss 打人有成功失败 1:成功 2:失败
                    if (ntf.Ret == 1)
                    {
                        if (ntf.Score != MyScore)
                        {
                            //uint score = ntf.Score - MyScore;
                            string content = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(3910010175u), Sys_Role.Instance.sRoleName, ntf.AttackerName.ToStringUtf8(), ntf.AddScore.ToString());
                            Sys_Chat.Instance.PushMessage(ChatType.Person, null, content);
                        }
                    }
                }
                else //挑战首领
                {
                    if (ntf.Score != MyScore)
                    {
                        //uint score = ntf.Score - MyScore;
                        string content = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(3910010176u), Sys_Role.Instance.sRoleName, ntf.AddScore.ToString());
                        Sys_Chat.Instance.PushMessage(ChatType.Person, null, content);
                    }
                }
            }
           

            MyScore = ntf.Score;
            MyRank = ntf.Rank;
            MyGuildScore = ntf.GuildScore;
            MyGuildRank = ntf.GuildRank;
            AttackAddScore = ntf.AddScore;
            AttackDmg = ntf.Dmg;
            AttackEndRet = ntf.Ret;

            if (!(ntf.Targetroleid == Sys_Role.Instance.RoleId))
                UIManager.OpenUI(EUIID.UI_FamilyBoss_Result);
        }

        private void OnGuildBossEndNtf(NetMsg msg)
        {
            CmdGuildBossEndNtf ntf = NetMsgUtil.Deserialize<CmdGuildBossEndNtf>(CmdGuildBossEndNtf.Parser, msg);
            
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(3910010311, ntf.DelayTime.ToString());
            PromptBoxParameter.Instance.SetConfirm(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }

        //private void PushChatMsg(string attackerName, string defenderName, uint score)
        //{
        //    string content = LanguageHelper.GetTextContent(CSVLanguage.Instance.GetConfData(3910010175u), attackerName, defenderName, score.ToString());
        //    Sys_Chat.Instance.PushMessage(ChatType.Person, null, content);
        //}

        private void OnEnterFight(CSVBattleType.Data battleData)
        {
            UIManager.CloseUI(EUIID.UI_FamilyBoss_Seize);
            UIManager.CloseUI(EUIID.UI_FamilyBoss);
        }
    }
}


