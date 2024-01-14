using Lib.Core;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class InterfaceBubbleFunction : FunctionBase
    {
        public CSVInterfaceBubble.Data CSVInterfaceBubbleData
        {
            get;
            private set;
        }

        Timer timer;

        public override void Init()
        {
            CSVInterfaceBubbleData = CSVInterfaceBubble.Instance.GetConfData(ID);
        }

        protected override void OnDispose()
        {
            CSVInterfaceBubbleData = null;
            timer?.Cancel();
            timer = null;

            base.OnDispose();
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVInterfaceBubbleData == null)
            {
                DebugUtil.LogError($"CSVInterfaceBubble.Data is null ID:{ID}");
                return false;
            }

            return true;
        }

        public override bool IsValid()
        {
            if (npc == null)
                return false;

            if (FunctionSourceType == EFunctionSourceType.Task)
            {
                return base.IsValid();
            }
            else
            {
                return Sys_MenuDialogue.Instance.CanActivedNpcFunction(npc.uID) && base.IsValid();
            }
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            timer?.Cancel();
            timer = Timer.Register(CSVInterfaceBubbleData.delay / 1000f, CallBack);

            ///组队状态下如果是队员且非暂离不能执行///
            if (Sys_Team.Instance.HaveTeam && !Sys_Team.Instance.isCaptain() && !Sys_Team.Instance.isPlayerLeave())
            {
                return;
            }

            Sys_Task.Instance.WhenBubbleCompleted(HandlerID);
        }

        void CallBack()
        {
            List<Sys_MenuDialogue.MenuDialogueDataWrap> datas = Sys_MenuDialogue.GetMenuDialogueDataWrap(CSVInterfaceBubbleData);
            Sys_MenuDialogue.ResetMenuDialogueDataEventData resetMenuDialogueDataEventData = PoolManager.Fetch(typeof(Sys_MenuDialogue.ResetMenuDialogueDataEventData)) as Sys_MenuDialogue.ResetMenuDialogueDataEventData;
            resetMenuDialogueDataEventData.Init(datas);

            Sys_MenuDialogue.Instance.OpenMenuDialogue(resetMenuDialogueDataEventData);
            if (CSVInterfaceBubbleData.loop == 0)
            {
                Sys_MenuDialogue.Instance.AddActiveNpc(npc.uID);
            }
        }
    }
}