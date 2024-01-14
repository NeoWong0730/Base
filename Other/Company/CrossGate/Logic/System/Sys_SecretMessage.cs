using Logic.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Lib.Core;
using Table;

namespace Logic
{
    public class Sys_SecretMessage : SystemModuleBase<Sys_SecretMessage>
    {
        public enum EEvents
        {
            MessageRight,
            MessageClue,
            MessageWrong,
            GetMessageRightCallBack,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public List<uint> completedSecretMessageID = new List<uint>();

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdNpc.RiddleNtf, OnRiddleNtf, CmdNpcRiddleNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdNpc.RiddleReq, (ushort)CmdNpc.RiddleRes, OnRiddleRes, CmdNpcRiddleRes.Parser);
        }

        void OnRiddleNtf(NetMsg msg)
        {
            completedSecretMessageID.Clear();
            CmdNpcRiddleNtf ntf = NetMsgUtil.Deserialize<CmdNpcRiddleNtf>(CmdNpcRiddleNtf.Parser, msg);
            if (ntf != null)
            {
                for (int index = 0, len = ntf.Id.Count; index < len; index++)
                {
                    completedSecretMessageID.Add(ntf.Id[index]);
                }
            }
        }

        public void ReqSecretMessage(uint id)
        {
            CmdNpcRiddleReq cmdNpcRiddleReq = new CmdNpcRiddleReq();
            cmdNpcRiddleReq.Id = id;
            cmdNpcRiddleReq.NpcUid = Sys_Interactive.CurInteractiveNPC.uID;

            NetClient.Instance.SendMessage((ushort)CmdNpc.RiddleReq, cmdNpcRiddleReq);
        }

        void OnRiddleRes(NetMsg msg)
        {
            CmdNpcRiddleRes res = NetMsgUtil.Deserialize<CmdNpcRiddleRes>(CmdNpcRiddleRes.Parser, msg);
            if (res != null)
            {
                if (completedSecretMessageID.Contains(res.Id))
                {
                    DebugUtil.LogError($"completedSecretMessageID already has id {res.Id}");
                }
                else
                {
                    completedSecretMessageID.Add(res.Id);
                }

                eventEmitter.Trigger(EEvents.GetMessageRightCallBack);
            }
        }

        public void OpenSecretMessage(CSVCode.Data cSVCodeData)
        {
            CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(cSVCodeData.CodeDialogue);
            List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
            ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
            resetDialogueDataEventData.Init(datas, () =>
            {

            }, cSVDialogueData);
            resetDialogueDataEventData.secret = true;
            resetDialogueDataEventData.cSVCodeDataID = cSVCodeData.id;

            Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
        }
    }
}