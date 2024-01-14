using Table;

namespace Logic
{
#if false
    /// <summary>
    /// 调查功能距离检测组件///
    /// </summary>
    public class InquiryDistanceCheckComponent : NPCAreaCheckComponent
    {
        protected override bool CheckResult()
        {
            if (!Npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
                return false;

            float areaDistance = CSVExploringSkill.Instance.GetConfData(100).range / 10000f;
            float distance = (GameCenter.mainHero.transform.position - Npc.transform.position).sqrMagnitude;

            if (distance > (areaDistance * areaDistance))
                return false;

            return true;
        }

        protected override void Trigger()
        {
            Sys_Inquiry.Instance.EnterArea(Npc.uID);
        }

        protected override void TriggerExit()
        {
            Sys_Inquiry.Instance.ExitArea(Npc.uID);
        }
    }
#endif
}
