using System.Collections.Generic;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.CollectBehaveInfoNoSkip)]
public class WS_CombatBehaveAI_CollectBehaveInfoNoSkip_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WorkStreamTranstionComponent wstc = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;

        if (!string.IsNullOrEmpty(str))
        {
            uint[] workIds = CombatHelp.GetStrParseUint1Array(str);
            for (int i = 0, length = workIds.Length; i < length; i++)
            {
                uint workId = workIds[i];

                List<WorkBlockData> workBlockDatas = WorkStreamConfigManager.Instance.GetWorkBlockDatas<WS_CombatBehaveAIControllerEntity>(workId, wstc.m_CurWorkBlockData.AttachType);
                if (workBlockDatas == null || workBlockDatas.Count == 0)
                {
                    //Lib.Core.DebugUtil.LogError($"WS_CombatBehaveAIControllerEntity类型的数据中没有workId ：{workId.ToString()}, attachType : {wstc.m_CurWorkBlockData.AttachType.ToString()}");
                    continue;
                }

                WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();
                for (int cwbdIndex = 0, cwbdCount = workBlockDatas.Count; cwbdIndex < cwbdCount; cwbdIndex++)
                {
                    WorkBlockData workBlockData = workBlockDatas[cwbdIndex];
                    if (workBlockData == null)
                        continue;

                    dataComponent.m_BlockInWorkIdDic[workBlockData.CurWorkBlockType] = workId;
                }

                wstc.UpdateWorkBlockDatas(workBlockDatas);
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}