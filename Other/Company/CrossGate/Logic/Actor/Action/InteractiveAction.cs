using Table;
using Lib.Core;
using System.Collections.Generic;

namespace Logic
{
    public class InteractiveData
    {
        public CSVNpc.Data CSVNpcData;
        public System.Action OnloadCallBack;
    }

    /// <summary>
    /// 交互行为///
    /// </summary>
    public class InteractiveWithNPCAction : ActionBase
    {
        //public const string TypeName = "Logic.InteractiveWithNPCAction";

        public Npc npc
        {
            get;
            set;
        }

        public TaskEntry currentTaskEntry
        {
            get;
            set;
        }

        protected override void OnDispose()
        {
            currentTaskEntry = null;
            InteractiveActionCompletedFlag = false;

            base.OnDispose();
        }

        /// <summary>
        /// 手动///
        /// </summary>
        protected override void OnExecute()
        {
            DebugUtil.LogFormat(ELogType.eTask, "OnExecute 进入交互 {0}", npc == null);
            if (npc == null)
                return;

            List<FunctionBase> functions = npc.NPCFunctionComponent.FilterFunctions();

            DebugUtil.LogFormat(ELogType.eTask, "OnExecute {0} {1}", functions.Count.ToString(), (npc.cSVNpcData.AcquiesceDialogue() == null));
            ///NPC身上激活的功能为空且默认对话为空
            if (functions.Count == 0 && npc.cSVNpcData.AcquiesceDialogue() == null)
                return;

            //进入交互状态
            Sys_Interactive.Instance.InteractiveProcess(npc.cSVNpcData, functions);
        }

        /// <summary>
        /// 自动///
        /// </summary>
        protected override void OnAutoExecute()
        {
            DebugUtil.LogFormat(ELogType.eTask, "OnAutoExecute 进入交互 npc == null ? {0}", npc == null);

            if (npc == null)
                return;

            List<FunctionBase> functions = npc.NPCFunctionComponent.FilterFunctions();

            DebugUtil.LogFormat(ELogType.eTask, "OnAutoExecute {0} {1} {2} {3}", 
                functions.Count.ToString(), 
                (npc.cSVNpcData.AcquiesceDialogue() == null), 
                functions.Count <= 0 ? null : functions[0].GetType(), 
                npc.uID);
            
            ///NPC身上激活的功能为空且默认对话为空
            if (functions.Count == 0 && npc.cSVNpcData.AcquiesceDialogue() == null)
                return;

            //进入交互状态
            ActionCtrl.Instance.actionCtrlStatus = ActionCtrl.EActionCtrlStatus.Auto;
            Sys_Interactive.Instance.AutoInteractiveProcess(npc, functions, currentTaskEntry);
        }

        public bool InteractiveActionCompletedFlag;

        public override bool IsCompleted()
        {
            return InteractiveActionCompletedFlag;
        }
    }
}
