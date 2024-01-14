using Table;
using System.Collections.Generic;
using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 对话功能///
    /// </summary>
    public class DialogueFunction : FunctionBase
    {
        public CSVDialogue.Data CSVDialogueData
        {
            get;
            private set;
        }

        public List<Sys_Dialogue.DialogueDataWrap> datas;

        public override void Init()
        {
            CSVDialogueData = CSVDialogue.Instance.GetConfData(ID);
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            return ParseContent();
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
            resetDialogueDataEventData.Init(datas, DialogueCallBack, CSVDialogueData);
            Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
            UIManager.CloseUI(EUIID.UI_NPC);

            // cuibinbin 进入对话的时候，不认为正在做自动任务
            Sys_Task.Instance.InterruptCurrentTaskDoing();

            //NPC对话打点
            if (npc != null && npc.cSVNpcData != null)
                Sys_Dialogue.Instance.HitPointDialog(npc.cSVNpcData.id);
        }

        void DialogueCallBack()
        {
            ///组队状态下如果是队员且非暂离不能执行///
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
            {
                return;
            }            

            ///限时任务处理///
            if (FunctionSourceType == EFunctionSourceType.Task && CSVTaskGoal.Instance.GetConfData(HandlerTaskTargetID).LimitTime != 0)
            {
                if (SpecialType == ESpecialType.TimeLimit)
                {
                    Sys_Task.Instance.ReqStartTimeLimit(HandlerID, HandlerIndex);
                }
            }

            Sys_Task.Instance.WhenDialogueCompleted(HandlerID);
        }

        bool ParseContent()
        {
            if (CSVDialogueData.dialogueContent == null)
            {
                return false;
            }
            datas = Sys_Dialogue.GetDialogueDataWraps(CSVDialogueData);

            return true;
        }

        protected override void OnDispose()
        {
            CSVDialogueData = null;
            datas?.Clear();
            datas = null;

            base.OnDispose();
        }
    }
}
