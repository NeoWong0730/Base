using System.Collections.Generic;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.SkipByState)]
public class WS_CombatBehaveAI_SkipByState_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WorkStreamTranstionComponent wstc = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;
        WS_CombatBehaveAIControllerEntity behaveAIController = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        int selectType = behaveAIController.GetBlockTypeByState(wstc, false);
        if (selectType > 0)
        {
            int attachType = 0;
            int blockType = 0;
            if (!string.IsNullOrWhiteSpace(str))
            {
                int[] ints = CombatHelp.GetStrParseInt1Array(str);
                attachType = ints[0];
                if (ints.Length > 1)
                    blockType = selectType;
            }

            WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
            if (!dataComponent.m_BlockInWorkIdDic.TryGetValue(selectType, out uint workId))
            {
                for (int i = 0, count = wstc.m_WorkBlockDataList.Count; i < count; i++)
                {
                    var blockData = wstc.m_WorkBlockDataList[i];
                    if (blockData.CurWorkBlockType == selectType)
                    {
                        workId = wstc.m_WorkId;
                        break;
                    }
                }
            }

            if (((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).StartChildMachine(this, workId, attachType, blockType))
                return;
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}