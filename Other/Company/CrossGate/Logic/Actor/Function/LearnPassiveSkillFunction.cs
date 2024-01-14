using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public class LearnPassiveSkillFunction : FunctionBase
    {
        public CSVPassiveSkillInfo.Data CSVPassiveSkillInfoData
        {
            get;
            private set;
        }

        public override void Init()
        {
            CSVPassiveSkillInfoData = CSVPassiveSkillInfo.Instance.GetConfData(ID);
        }

        protected override bool CanExecute(bool CheckVisual = true)
        {
            if (CSVPassiveSkillInfoData == null)
            {
                DebugUtil.LogError($"CSVPassiveSkillInfo.Data is Null, id: {ID}");
                return false;
            }

            if (Sys_Bag.Instance.GetItemCount(CSVPassiveSkillInfoData.upgrade_cost[0][0]) < CSVPassiveSkillInfoData.upgrade_cost[0][1])
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000750));

                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                return false;
            }

            return true;
        }

        public override bool IsValid()
        {
            if (Sys_Skill.Instance.bestSkillInfos != null && Sys_Skill.Instance.bestSkillInfos.ContainsKey(CSVPassiveSkillInfoData.skill_id) && Sys_Skill.Instance.bestSkillInfos[CSVPassiveSkillInfoData.skill_id].Level == 0 && Sys_Skill.Instance.CanUpgradeLevel(Sys_Skill.Instance.bestSkillInfos[CSVPassiveSkillInfoData.skill_id], false, false))
                return base.IsValid() && true;

            if (Sys_Skill.Instance.commonSkillInfos != null && Sys_Skill.Instance.commonSkillInfos.ContainsKey(CSVPassiveSkillInfoData.skill_id) && Sys_Skill.Instance.commonSkillInfos[CSVPassiveSkillInfoData.skill_id].Level == 0 && Sys_Skill.Instance.CanUpgradeLevel(Sys_Skill.Instance.commonSkillInfos[CSVPassiveSkillInfoData.skill_id], false, false))
                return base.IsValid() && true;

            return false;
        }

        protected override void OnExecute()
        {
            base.OnExecute();

            CmdSkillPassiveSkillUpdateReq req = new CmdSkillPassiveSkillUpdateReq();
            req.SkillId = CSVPassiveSkillInfoData.skill_id;

            NetClient.Instance.SendMessage((ushort)CmdSkill.PassiveSkillUpdateReq, req);
        }

        protected override void OnDispose()
        {
            CSVPassiveSkillInfoData = null;

            base.OnDispose();
        }
    }
}