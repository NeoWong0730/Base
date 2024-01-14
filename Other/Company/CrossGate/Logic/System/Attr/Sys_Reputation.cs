using Packet;
using Logic.Core;
using Net;
using Lib.Core;
using Table;
using System.Collections.Generic;
using System.Linq;

namespace Logic
{
    public partial class Sys_Reputation : SystemModuleBase<Sys_Reputation>
    {
        public uint reputationLevel;
        public uint reputationValue;
        public uint yesterdayMaxLevel;
        public uint danLevel;
        public uint specificLevel;

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public List<RankDescRole> rankDesRoleList = new List<RankDescRole>();
        private uint maxDanLv;

        public enum EEvents : int
        {
            OnReputationUpdate,  //声望更新
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdReputation.ExpExchangeReq, (ushort)CmdReputation.ExpExchangeRes, OnExpExchangeRes, CmdReputationExpExchangeRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdReputation.ExploitExchangeReq, (ushort)CmdReputation.ExploitExchangeRes, OnExploitExchangeRes, CmdReputationExploitExchangeRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdReputation.YesterdayMaxLevelNtf, OnYesterdayMaxLevelNtf, CmdReputationYesterdayMaxLevelNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdReputation.RoleReputationNtf, OnRoleReputationNtf, CmdReputationRoleReputationNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdReputation.EnterGameNtf, OnEnterGameNtf, CmdReputationEnterGameNtf.Parser);

            maxDanLv = CSVFameRank.Instance.GetKeys().Max();
        }

        public void ExpExchangeReq()
        {
            CmdReputationExpExchangeRes req = new CmdReputationExpExchangeRes();
            NetClient.Instance.SendMessage((ushort)CmdReputation.ExpExchangeReq, req);
        }

        private void OnExpExchangeRes(NetMsg msg)
        {
            CmdReputationExpExchangeRes res = NetMsgUtil.Deserialize<CmdReputationExpExchangeRes>(CmdReputationExpExchangeRes.Parser, msg);
            if (reputationLevel != res.ReputationLevel)
            {
                GameCenter.mainHero.heroFxComponent.UpdateReputationFx(true);
                PlayActorReputationHudEvt evt = new PlayActorReputationHudEvt();
                evt.actorId = GameCenter.mainHero.UID;
                Sys_HUD.Instance.eventEmitter.Trigger<PlayActorReputationHudEvt>(Sys_HUD.EEvents.OnPlayActorReputationUpFx, evt);
                AudioUtil.PlayAudio(4002);
            }
            reputationLevel = res.ReputationLevel;
            reputationValue = res.ReputationValue;
            SetReputationLv(reputationLevel);
            eventEmitter.Trigger(EEvents.OnReputationUpdate);
        }

        public void ExploitExchangeReq()
        {
            CmdReputationExploitExchangeReq req = new CmdReputationExploitExchangeReq();
            NetClient.Instance.SendMessage((ushort)CmdReputation.ExploitExchangeReq, req);
        }

        private void OnExploitExchangeRes(NetMsg msg)
        {
            CmdReputationExploitExchangeRes res = NetMsgUtil.Deserialize<CmdReputationExploitExchangeRes>(CmdReputationExploitExchangeRes.Parser, msg);
            if (reputationLevel != res.ReputationLevel)
            {
                GameCenter.mainHero.heroFxComponent.UpdateReputationFx(true);
                PlayActorReputationHudEvt evt = new PlayActorReputationHudEvt();
                evt.actorId = GameCenter.mainHero.UID;
                Sys_HUD.Instance.eventEmitter.Trigger<PlayActorReputationHudEvt>(Sys_HUD.EEvents.OnPlayActorReputationUpFx, evt);
                AudioUtil.PlayAudio(4002);
            }
            reputationLevel = res.ReputationLevel;
            reputationValue = res.ReputationValue;
            SetReputationLv(reputationLevel);
            eventEmitter.Trigger(EEvents.OnReputationUpdate);
        }

        private void OnYesterdayMaxLevelNtf(NetMsg msg)
        {
            CmdReputationYesterdayMaxLevelNtf yesterdayMaxLevelNtf = NetMsgUtil.Deserialize<CmdReputationYesterdayMaxLevelNtf>(CmdReputationRoleReputationNtf.Parser, msg);
            yesterdayMaxLevel = yesterdayMaxLevelNtf.ReputationLevel;
        }

        private void OnRoleReputationNtf(NetMsg msg)
        {
            CmdReputationRoleReputationNtf reputationRoleNtf = NetMsgUtil.Deserialize<CmdReputationRoleReputationNtf>(CmdReputationRoleReputationNtf.Parser, msg);
            if(reputationLevel < reputationRoleNtf.ReputationLevel)
            {
                GameCenter.mainHero.heroFxComponent.UpdateReputationFx(true);
                PlayActorReputationHudEvt evt = new PlayActorReputationHudEvt();
                evt.actorId = GameCenter.mainHero.UID;
                Sys_HUD.Instance.eventEmitter.Trigger<PlayActorReputationHudEvt>(Sys_HUD.EEvents.OnPlayActorReputationUpFx, evt);
                AudioUtil.PlayAudio(4002);
                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent2,EMagicAchievement.Event28,reputationRoleNtf.ReputationLevel);
            }
            reputationLevel = reputationRoleNtf.ReputationLevel;
            if (reputationRoleNtf.AddValue != 0)
            {
                string content = LanguageHelper.GetTextContent(2020917, reputationRoleNtf.AddValue.ToString());
                Sys_Chat.Instance.PushMessage(ChatType.Person, null, content, Sys_Chat.EMessageProcess.None);
            }
            reputationValue = reputationRoleNtf.ReputationValue;
            SetReputationLv(reputationLevel);
            eventEmitter.Trigger(EEvents.OnReputationUpdate);
        }

        private void OnEnterGameNtf(NetMsg msg)
        {
            CmdReputationEnterGameNtf reputationEnterGameNtf = NetMsgUtil.Deserialize<CmdReputationEnterGameNtf>(CmdReputationEnterGameNtf.Parser, msg);
            reputationLevel = reputationEnterGameNtf.ReputationLevel;
            reputationValue = reputationEnterGameNtf.ReputationValue;
            yesterdayMaxLevel = reputationEnterGameNtf.YesterdayMaxLevel;
            SetReputationLv(reputationLevel);
        }

        public string GetRankTitleByReputationLevel(uint reputationLevel)
        {
            string title = null;
            uint level = GetDanLevelByReputationLevel(reputationLevel);
            if (CSVFameRank.Instance.ContainsKey(level))
            {
                title = LanguageHelper.GetTextContent(CSVFameRank.Instance.GetConfData(level).name);
            }
            else
            {
                DebugUtil.LogError("CSVFameRank not containsKey "+ level.ToString());
            }
            return title;
        }

        public uint GetDanLevelByReputationLevel(uint reputationLevel)
        {
            uint level = (reputationLevel + 100) / 100;
            uint maxLv = maxDanLv * 100;
            if (!CSVFameRank.Instance.ContainsKey(level) && level != 0 && reputationLevel >= maxLv)
            {
                level = maxDanLv;
            }
            return level;
        }


        public uint GetLevelByReputationLevel(uint reputationLevel)
        {
            uint specificlvl = reputationLevel % 100;
            uint maxLv =(maxDanLv-1) * 100;
            if (reputationLevel >= maxLv)
            {
                specificlvl = reputationLevel - maxLv;
            }
            return specificlvl;
        }

        public string GetRankAndLevelTitle(uint reputationLevel)
        {
            string rank = GetRankTitleByReputationLevel(reputationLevel);
            uint danLv = GetDanLevelByReputationLevel(reputationLevel);
            uint level = GetLevelByReputationLevel(reputationLevel);
            return LanguageHelper.GetTextContent(2022120, rank, danLv.ToString(), level.ToString());
        }

        public void SetReputationLv(uint reputationLevel)
        {
            danLevel = GetDanLevelByReputationLevel(reputationLevel);
            specificLevel = GetLevelByReputationLevel(reputationLevel);
        }

        public override void OnLogout()
        {
            base.OnLogout();
            reputationValue = 0;
            reputationLevel = 0;
            yesterdayMaxLevel = 0;
            danLevel = 0;
            specificLevel = 0;
            rankDesRoleList.Clear();
        }

    }
}
