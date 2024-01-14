using Lib.Core;
using Net;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class LearnActiveSkillFunction : FunctionBase
    {
        public CSVActiveSkillInfo.Data CSVActiveSkillInfoData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(ID);
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVActiveSkillInfoData == null)
            {
                DebugUtil.LogError($"CSVActiveSkillInfo.Data is Null, id: {ID}");
                return false;
            }

            if (Sys_Bag.Instance.GetItemCount(CSVActiveSkillInfoData.upgrade_cost[0][0]) < CSVActiveSkillInfoData.upgrade_cost[0][1])
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000750));

                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                return false;
            }

            return true;
        }

        public override bool IsValid()
        {
            if (Sys_Skill.Instance.bestSkillInfos != null && Sys_Skill.Instance.bestSkillInfos.ContainsKey(CSVActiveSkillInfoData.skill_id) && Sys_Skill.Instance.bestSkillInfos[CSVActiveSkillInfoData.skill_id].Level == 0 && Sys_Skill.Instance.CanUpgradeLevel(Sys_Skill.Instance.bestSkillInfos[CSVActiveSkillInfoData.skill_id], false, false))
                return base.IsValid() && true;

            if (Sys_Skill.Instance.commonSkillInfos != null && Sys_Skill.Instance.commonSkillInfos.ContainsKey(CSVActiveSkillInfoData.skill_id) && Sys_Skill.Instance.commonSkillInfos[CSVActiveSkillInfoData.skill_id].Level == 0 && Sys_Skill.Instance.CanUpgradeLevel(Sys_Skill.Instance.commonSkillInfos[CSVActiveSkillInfoData.skill_id], false, false))
                return base.IsValid() && true;

            return false;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            CmdSkillUpdateSkillLevelReq req = new CmdSkillUpdateSkillLevelReq();
            req.SkillId = CSVActiveSkillInfoData.skill_id;

            NetClient.Instance.SendMessage((ushort)CmdSkill.UpdateSkillLevelReq, req);
        }

        protected override void OnDispose()
        {
            CSVActiveSkillInfoData = null;

            base.OnDispose();
        }
    }
}
