using Lib.Core;
using Net;
using Packet;
using System.Collections.Generic;
using Table;

namespace Logic
{
    /// <summary>
    /// 声望功能///
    /// </summary>
    public class PrestigeFunction : FunctionBase
    {
        public struct PrestigeFunctionData
        {
            public uint nextPrestigeID;     //下一个声望称号ID
            public bool getFlag;        //是否可以获取
            public uint name;       //称号名称
            public uint successDialogueID;      //满足条件对话ID
            public uint failDialogueID;     //不满足条件对话ID
        }

        PrestigeFunctionData prestigeFunctionData;

        protected override void OnDispose()
        {
            prestigeFunctionData = default;

            base.OnDispose();
        }

        public override void DeserializeObject(List<uint> strs, bool taskCreate = false)
        {
            base.DeserializeObject(strs, taskCreate);

            ResetDesc();
        }

        public override bool IsValid()
        {
            return !Sys_Title.Instance.IsReachMaxPrestigeLevel();
        }

        public void ResetDesc()
        {
            prestigeFunctionData = Sys_Title.Instance.GetPrestigeFunctionData();
            Desc = prestigeFunctionData.name;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            prestigeFunctionData = Sys_Title.Instance.GetPrestigeFunctionData();

            if (prestigeFunctionData.getFlag)
            {
                CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(prestigeFunctionData.successDialogueID);
                if (cSVDialogueData != null)
                {
                    List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
                    ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                    resetDialogueDataEventData.Init(datas, DialogueCallback, cSVDialogueData);
                    Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
                }
                else
                {
                    DebugUtil.LogError($"DialogueID is null id:{prestigeFunctionData.successDialogueID}");
                }
            }
            else
            {
                CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(prestigeFunctionData.failDialogueID);
                if (cSVDialogueData != null)
                {
                    List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
                    ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                    resetDialogueDataEventData.Init(datas, null, cSVDialogueData);
                    Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
                }
                else
                {
                    DebugUtil.LogError($"DialogueID is null id:{prestigeFunctionData.successDialogueID}");
                }
            }
        }

        void DialogueCallback()
        {
            CmdTitleNpcGetTitleReq req = new CmdTitleNpcGetTitleReq();
            req.NpcId = npc.cSVNpcData.id;

            NetClient.Instance.SendMessage((ushort)CmdTitle.NpcGetTitleReq, req);
        }
    }
}
