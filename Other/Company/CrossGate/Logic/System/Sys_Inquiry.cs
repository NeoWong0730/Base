using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;

namespace Logic
{
    /// <summary>
    /// 探索///
    /// </summary>
    public class Sys_Inquiry : SystemModuleBase<Sys_Inquiry>
    {
        public class InquiryData
        {
            public uint npcID;
            public uint inquiryID;
        }

        public List<uint> InquiryedID;

        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        List<ulong> areaNpcs;

        public Timer timer;

        public enum EEvents
        {
            EnterAnyInquiryArea,    //进入某个带有激活调查功能的NPC范围
            ExitAllNoInquiryArea,     //退出到所有NPC都不带激活调查功能的范围
            InquiryCompleted,
        }

        public override void OnLogin()
        {
            base.OnLogin();

            InquiryedID = new List<uint>();
            areaNpcs = new List<ulong>();
        }

        public override void OnLogout()
        {
            base.OnLogout();

            InquiryedID.Clear();
            areaNpcs.Clear();
        }

        public override void Init()
        {
            base.Init();

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdNpc.InvestigateNtf, OnInvestigateNtf, CmdNpcInvestigateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdNpc.InvestigateReq, (ushort)CmdNpc.InvestigateAck, OnInvestigateAck, CmdNpcInvestigateAck.Parser);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, OnHeroTel, true);
        }

        public override void Dispose()
        {
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, OnHeroTel, false);

            base.Dispose();
        }

        public void Inquiry()
        {
            Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.UIButton, new InteractiveEvtData()
            {
                eInteractiveAimType = EInteractiveAimType.NPCFunction,
                sceneActor = null,
                immediately = false,
                data = EFunctionType.Inquiry,
            });
        }

        public bool IsInquiryed(uint id)
        {
            return InquiryedID.Contains(id);
        }

        void OnInvestigateNtf(NetMsg netMsg)
        {
            CmdNpcInvestigateNtf ntf = NetMsgUtil.Deserialize<CmdNpcInvestigateNtf>(CmdNpcInvestigateNtf.Parser, netMsg);
            if (ntf != null)
            {
                foreach (var id in ntf.Id)
                {
                    InquiryedID.Add(id);             
                }
            }
        }

        void OnInvestigateAck(NetMsg netMsg)
        {
            InquiryAction.InquiryCompleted = true;
            CmdNpcInvestigateAck ack = NetMsgUtil.Deserialize<CmdNpcInvestigateAck>(CmdNpcInvestigateAck.Parser, netMsg);
            if (ack != null)
            {
                InquiryedID.Add(ack.Id);
                eventEmitter.Trigger(EEvents.InquiryCompleted);
                UIManager.OpenUI(EUIID.UI_Probe_Report, true, new InquiryData()
                {
                    npcID = ack.NpcInfoId,
                    inquiryID = ack.Id,
                });
                ExitArea(ack.Id);
            }         
        }

        public void EnterArea(ulong uid)
        {
            if (!areaNpcs.Contains(uid))
            {
                areaNpcs.Add(uid);
            }

            if (areaNpcs.Count > 0)
            {
                eventEmitter.Trigger(EEvents.EnterAnyInquiryArea);
            }           
        }

        public void ExitArea(ulong uid)
        {
            if (areaNpcs.Contains(uid))
            {
                areaNpcs.Remove(uid);
            }

            if (areaNpcs.Count == 0)
            {
                eventEmitter.Trigger(EEvents.ExitAllNoInquiryArea);
            }
        }

        void OnHeroTel()
        {
            eventEmitter.Trigger(EEvents.ExitAllNoInquiryArea);
        }
    }
}
