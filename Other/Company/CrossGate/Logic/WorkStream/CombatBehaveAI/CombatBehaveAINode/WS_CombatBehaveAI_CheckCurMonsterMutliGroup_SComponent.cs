[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CheckCurMonsterMutliGroup)]
public class WS_CombatBehaveAI_CheckCurMonsterMutliGroup_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        uint[] groups = CombatHelp.GetStrParseUint1Array(str);
        int selectType = 0;
        for (int i = 0, len = groups.Length; i < len; i++)
        {
            if (Logic.Sys_Fight.curMonsterGroupId == groups[i])
            {
                selectType = 1;
                break;
            }
        }
        m_CurUseEntity.TranstionMultiStates(this, 1, selectType);
    }
}