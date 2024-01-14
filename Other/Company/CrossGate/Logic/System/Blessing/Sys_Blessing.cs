using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public class Sys_Blessing : SystemModuleBase<Sys_Blessing>
    {
        public enum EEvents
        {
            InfoRefresh,
            Start,
            TakeAward,
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public CmdBlessGetInfoRes Info { get; private set; } = null;

        public uint RuningAutoID { get; set; } = 0;

        private bool m_IsActive = false;
        public bool IsActive { get {

                if (Sys_FunctionOpen.Instance.IsOpen(42101) == false)
                    return false;

                return m_IsActive;

            } private set { m_IsActive = value; } }

        public Dictionary<uint, bool> DicAutoSign { get; private set; } = new Dictionary<uint, bool>();

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBless.GetInfoRes, OnNetGetInfo, CmdBlessGetInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBless.StartRes, OnNetStartRes, CmdBlessStartRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdBless.TakeAwardRes, OnNetTakeAwardRes, CmdBlessTakeAwardRes.Parser);

            Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnBattleSettlement, this.OnEndBattle, true);
        }
        public override void OnLogin()
        {
           // if (Sys_FunctionOpen.Instance.IsOpen(42101))
            {
                SendNetInfoReq();

               // DebugUtil.LogError(" sys_blessing  login send info req");
            }
               

            DicAutoSign.Clear();

           // DebugUtil.LogError(" sys_blessing  login");
        }

        public override void OnLogout()
        {
            Info = null;
        }

        public uint GetTimes(uint id)
        {
            if (Info == null)
                return 0;

            var awradinfo = Info.InfoList.Find(o => o.Id == id);

            if (awradinfo == null)
                return 0;

            return awradinfo.Count;
        }

        public int GetState(uint id)
        {
            if (Info == null)
                return -1;

            var awradinfo = Info.InfoList.Find(o => o.Id == id);

            if (awradinfo == null || awradinfo.Condi == false)
                return -1;


            return (int)awradinfo.State;
        }
        public bool IsAuto(uint id)
        {
            bool auto = false;

            DicAutoSign.TryGetValue(id, out auto);

            return auto;
        }

        public void SetAuto(uint id, bool isauto)
        {
            if (DicAutoSign.ContainsKey(id) == false)
            {
                DicAutoSign.Add(id, isauto);
                return;
            }

            DicAutoSign[id] = isauto;
        }


        public uint GetBlessDataIDByBattleID(uint battleID)
        {
            var datalist = CSVBless.Instance.GetAll();
            int count = datalist.Count;

            for (int i = 0; i < count; i++)
            {
                var data = CSVBless.Instance.GetByIndex(i);
                if (data.BattleID == battleID)
                {
                    return data.id;
                }
            }
            return 0;
        }

        public bool HaveReward()
        {
            bool have = false;

            if (Info == null)
                return have;

            int count = Info.InfoList.Count;

            for (int i = 0; i < count; i++)
            {
                if (Info.InfoList[i].State == 2)
                {
                    have = true;
                    break;
                }
            }

            return have;
        }

        private void OnNetGetInfo(NetMsg msg)
        {
            Info = NetMsgUtil.Deserialize<CmdBlessGetInfoRes>(CmdBlessGetInfoRes.Parser, msg);

            int count = Info.InfoList.Count;

            if (IsActive == false)
            {
                for (int i = 0; i < count; i++)
                {
                    if (Info.InfoList[i].Condi)
                    {
                        IsActive = true;
                        break;
                    }
                }
            }


            eventEmitter.Trigger(EEvents.InfoRefresh);
        }

        private void OnNetStartRes(NetMsg msg)
        {
            var startRes = NetMsgUtil.Deserialize<CmdBlessStartRes>(CmdBlessStartRes.Parser, msg);

            var awradinfo = Info.InfoList.Find(o => o.Id == startRes.Id);

            if (awradinfo != null)
            {
                awradinfo.State = startRes.State;
                awradinfo.Count += 1;

               var data =  CSVBless.Instance.GetConfData(awradinfo.Id);

                SetNpcShow(data.npcID, true);

                eventEmitter.Trigger(EEvents.Start);

                if (RuningAutoID == awradinfo.Id)
                {
                    ActionCtrl.Instance.MoveToTargetNPCAndInteractive(data.npcID);        
                }
            }

            RuningAutoID = 0;
        }

        private void SetNpcShow(uint npcid,bool bshow)
        {
            if (GameCenter.TryGetSceneNPC(npcid, out Npc npc))
            {
                npc.VisualComponent.Checking();
            }
        }

        private void OnNetTakeAwardRes(NetMsg msg)
        {
            var takeAwardRes = NetMsgUtil.Deserialize<CmdBlessTakeAwardRes>(CmdBlessTakeAwardRes.Parser, msg);

            var awradinfo = Info.InfoList.Find(o => o.Id == takeAwardRes.Id);

            if (awradinfo != null)
            {
                awradinfo.State = takeAwardRes.State;

                eventEmitter.Trigger(EEvents.TakeAward);
            }
        }

        private void OnEndBattle(CmdBattleEndNtf ntf)
        {

            if (CombatManager.Instance.m_BattleTypeTb.BlessCloseReward == 1)
            {
                uint blessdataid = GetBlessDataIDByBattleID(ntf.BattleTypeId);

                if (blessdataid > 0)
                {
                    if (ntf.BattleResult == 1 && Info != null)
                    {
                       var infobless = Info.InfoList.Find(o => o.Id == blessdataid);
                        if (infobless != null)
                        {
                            infobless.State = 2;
                            eventEmitter.Trigger(EEvents.TakeAward);
                            var data = CSVBless.Instance.GetConfData(blessdataid);
                            SetNpcShow(data.npcID, true);
                        }
                            
                    }
                    UIManager.OpenUI(EUIID.UI_Blessing_Result, false, new UI_BlessingResult_Parma() { id = blessdataid, result = ntf.BattleResult,autonext = IsAuto(blessdataid) });
                }
                    
            }
        }
        public void SendNetInfoReq()
        {

            CmdBlessGetInfoReq info = new CmdBlessGetInfoReq();

            NetClient.Instance.SendMessage((ushort)CmdBless.GetInfoReq, info);
        }

        public void SendNetStartReq(uint id)
        {
            CmdBlessStartReq info = new CmdBlessStartReq();
            info.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdBless.StartReq, info);
        }

        public void SendNetTakeAwardReq(uint id)
        {
            CmdBlessTakeAwardReq info = new CmdBlessTakeAwardReq();
            info.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdBless.TakeAwardReq, info);
        }

        private bool IsOpen(uint id)
        {


            return false;
        }

        enum ECompareState
        {
            Equal, //等于
            Less,//小于
            Greatter,//大于
            EqualLess,//小于等于
            EqualGreater,//大于等于
        }
        //角色当前经验总值+当前声望经验总值 < 当前开服时间对应的角色等级&声望对应值
        private ECompareState GetTotalExpAddTotalReputationState()
        {

            ulong totalexp = Sys_Role.Instance.exp + CSVCharacterAttribute.Instance.GetConfData(Sys_Role.Instance.Role.Level).totol_exp; //角色当前经验总值
            uint totalreputation = Sys_Reputation.Instance.reputationValue;//当前声望经验总值

            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);//开服天数

            if (worldLevelData.LvRepuTotal == (totalexp + totalreputation))
                return ECompareState.Equal;


            if (worldLevelData.LvRepuTotal > (totalexp + totalreputation))
                return ECompareState.Less;

            return ECompareState.Greatter;
        }

        //角色等级 与 当前开服时间对应角色等级 的关系 
        private ECompareState GetRoleAndOpenServerRoleLevelState()
        {
            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);//开服天数

            if (worldLevelData.LvRecom == Sys_Role.Instance.Role.Level)
                return ECompareState.Equal;

            if (worldLevelData.LvRecom > Sys_Role.Instance.Role.Level)
                return ECompareState.Less;

            return ECompareState.Greatter;
        }

        //且声望等级 与 当前开服时间对应声望等级 的关系  
        private ECompareState GetRoleAndOpenServerRoleReputationState()
        {
            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);//开服天数

            if (worldLevelData.RepuRecom == Sys_Reputation.Instance.reputationLevel)
                return ECompareState.Equal;

            if (worldLevelData.RepuRecom > Sys_Reputation.Instance.reputationLevel)
                return ECompareState.Less;

            return ECompareState.Greatter;
        }

        //角色天赋点 与 当前开服时间天赋对应值 的关系  
        // 天赋点同时包含已使用天赋点，未使用天赋点，以及背包中剩余天赋魔晶可兑换的天赋点
        private ECompareState GetRoleAndOpenServerRoleRecom()
        {
            long totaltalent = Sys_Talent.Instance.usingScheme.withoutLianhuaing + Sys_Talent.Instance.lianhuaingPoint;

            long count = Sys_Bag.Instance.GetItemCount(202003u);

            var itemlist = CSVTalentExchange.Instance.GetAll();
            int itemcount = itemlist.Count;

            uint hadtalent = 0;
            for (int i = 0; i < itemcount; i++)
            {
                var data = CSVTalentExchange.Instance.GetByIndex(i);

                if (data != null && data.totalitem > count)
                {
                    hadtalent = data.id;
                    break;
                }
            }

            totaltalent += hadtalent;

            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);//开服天数

            if (worldLevelData.TalentRecom == totaltalent)
                return ECompareState.Equal;

            if (worldLevelData.TalentRecom > totaltalent)
                return ECompareState.Less;

            return ECompareState.Greatter;

        }

        //角色当前历练等级 与 当前开服时间历练等级对应值 的关系  
        private ECompareState GetRoleAndOpenServerRoleExperience()
        {
            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);//开服天数

            if (worldLevelData.PracRecom == Sys_Experience.Instance.exPerienceLevel)
                return ECompareState.Equal;


            if (worldLevelData.PracRecom > Sys_Experience.Instance.exPerienceLevel)
                return ECompareState.Less;

            return ECompareState.Greatter;
        }

        //角色当前历练等级比较
        private ECompareState GetRoleRoleExperience(uint level)
        {
            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);//开服天数

            if (level == Sys_Experience.Instance.exPerienceLevel)
                return ECompareState.Equal;


            if (level < Sys_Experience.Instance.exPerienceLevel)
                return ECompareState.Less;

            return ECompareState.Greatter;
        }
        //宠物驯养等级 与 当前开服时间驯养等级对应值  的关系  
        private ECompareState GetRoleAndOpenServerRolePetDomesticate()
        {
            var worldLevelData = CSVWorldLevel.Instance.GetConfData(Sys_Role.Instance.openServiceDay);//开服天数

            if (worldLevelData.PracRecom == Sys_PetDomesticate.Instance.Level)
                return ECompareState.Equal;


            if (worldLevelData.PracRecom > Sys_PetDomesticate.Instance.Level)
                return ECompareState.Less;

            return ECompareState.Greatter;
        }



    }
}
