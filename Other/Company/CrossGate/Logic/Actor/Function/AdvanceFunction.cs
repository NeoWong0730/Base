using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;

namespace Logic
{
    /// <summary>
    /// 进阶功能///
    /// </summary>
    public class AdvanceFunction : FunctionBase
    {
        Timer timer;

        public override bool IsValid()
        {
            if (Sys_Advance.Instance.NextAdvanceRank() == -1)
                return false;
            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();
            if (CSVPromoteCareer.Instance.GetConfData(Sys_Advance.Instance.SetAdvanceRank()+1).isFull == Sys_Role.Instance.Role.Level)
            {
                timer?.Cancel();
                timer = Timer.Register(0.1f, TimerCallBack, null, false, true);            
            }
            else
            {
                CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(CSVPromoteCareer.Instance.GetConfData((uint)Sys_Advance.Instance.NextAdvanceRank()).advanceDia);
                if (null == cSVDialogueData)
                    return;
                List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
                ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                resetDialogueDataEventData.Init(datas, DialogueCallBack, cSVDialogueData);
                Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
            }
        }

        void DialogueCallBack()
        {
            Sys_Role.Instance.PromoteCareerRankReq(1);
        }

        void TimerCallBack()
        {
            uint nextId = (uint)Sys_Advance.Instance.NextAdvanceRank();
            int count = CSVPromoteCareer.Instance.GetConfData(nextId).teamCondition;
            if (count == 0)
            {
                UIManager.OpenUI(EUIID.UI_Advance_Warning, false, true);
            }
            else
            {
                Sys_Role.Instance.TeamLeaderCheckPromoteCareerReq();
            }
        }

        protected override void OnDispose()
        {
            timer?.Cancel();
            timer = null;

            base.OnDispose();
        }
    }
}
