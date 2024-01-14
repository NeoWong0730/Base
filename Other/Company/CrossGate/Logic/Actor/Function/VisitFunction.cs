using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    /// <summary>
    /// 拜访功能///
    /// </summary>
    public class VisitFunction : FunctionBase
    {
        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (npc == null)
                return false;
            return true;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
            Sys_NPCFavorability.Instance.OnBtnVisitClicked(npc.cSVNpcData.id);
        }

        public override bool IsValid()
        {           
            if (!Sys_Inquiry.Instance.IsInquiryed(ID))
                return false;

            if (npc == null)
                return false;

            if (Sys_NPCFavorability.Instance.IsNPCReachedMax(npc.cSVNpcData.id))
                return false;

            return base.IsValid();
        }
    }
}
