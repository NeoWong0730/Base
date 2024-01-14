using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class Sys_SubmitItem : SystemModuleBase<Sys_SubmitItem>
    {
        public enum EEvents
        {
            OnSubmited,
        }
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public class SubmitData
        {
            public uint CsvSubmitID;
            public uint TaskId; // 如果不是task,则不赋值即可
            public ulong npcUID;
            public EFunctionSourceType FunctionSourceType;

            public uint FunctionHandlerID;
            public uint arg;
        }

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdNpc.DialogueChooseReq, (ushort)CmdNpc.DialogueChooseAck, OnSubmited_DialogueSelect, CmdNpcDialogueChooseAck.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdTask.SubmitItemReq, (ushort)CmdTask.SubmitItemRes, OnSubmited_Task, CmdTaskSubmitItemRes.Parser);

            DialogueChooseAnswer.eventEmitter.Handle<DialogueChooseAnswer.SubmitItemAnswerEvt>(DialogueChooseAnswer.EEvents.SubmitItem, OnDialoggueChoseSubmitItem, true);
        }

        // 请求上交道具
        public void ReqSubmit(ulong npcUId, uint taskId, uint taskGoalIndex, uint dialogueId, uint dialogueOption)
        {
            CmdNpcDialogueChooseReq req = new CmdNpcDialogueChooseReq();
            req.UNpcId = npcUId;
            req.TaskId = taskId;
            req.TaskIndex = taskGoalIndex;
            req.DialogueId = dialogueId;
            req.DialogueOption = dialogueOption;

            NetClient.Instance.SendMessage((ushort)CmdNpc.DialogueChooseReq, req);
        }
        private void OnSubmited_DialogueSelect(NetMsg msg)
        {
            CmdNpcDialogueChooseAck response = NetMsgUtil.Deserialize<CmdNpcDialogueChooseAck>(CmdNpcDialogueChooseAck.Parser, msg);
            eventEmitter.Trigger<uint, uint>(EEvents.OnSubmited, response.TaskId, response.TaskIndex);
        }

        public void ReqSubmit(ulong npcUId, uint taskId, uint taskGoalIndex)
        {
            CmdTaskSubmitItemReq req = new CmdTaskSubmitItemReq();
            req.NpcUId = npcUId;
            req.TaskId = taskId;
            req.Position = taskGoalIndex;

            NetClient.Instance.SendMessage((ushort)CmdTask.SubmitItemReq, req);
        }
        private void OnSubmited_Task(NetMsg msg)
        {
            CmdTaskSubmitItemRes response = NetMsgUtil.Deserialize<CmdTaskSubmitItemRes>(CmdTaskSubmitItemRes.Parser, msg);
            eventEmitter.Trigger<uint, uint>(EEvents.OnSubmited, response.TaskId, response.Position);
        }

        // 客户端打开 上交UI
        public void OpenUI(SubmitData submitData)
        {
            UIManager.OpenUI(EUIID.UI_SubmitItem, false, submitData);
        }

        private void OnDialoggueChoseSubmitItem(DialogueChooseAnswer.SubmitItemAnswerEvt evtData)
        {
            SubmitData submitData = new SubmitData();
            submitData.CsvSubmitID = evtData.value;
            submitData.npcUID = evtData.npcuid;
            submitData.TaskId = evtData.handlerID;
            submitData.FunctionSourceType = EFunctionSourceType.None;
            submitData.FunctionHandlerID = evtData.dialogueChooseID;
            submitData.arg = (uint)evtData.index;

            UIManager.OpenUI(EUIID.UI_SubmitItem, false, submitData);
        }
    }
}