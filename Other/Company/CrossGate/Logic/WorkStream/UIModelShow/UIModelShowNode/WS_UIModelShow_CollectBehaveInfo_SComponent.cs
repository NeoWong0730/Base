using System.Collections.Generic;

[StateComponent((int)StateCategoryEnum.UIModelShow, (int)UIModelShowEnum.CollectBehaveInfo)]
public class WS_UIModelShow_CollectBehaveInfo_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        WorkStreamTranstionComponent wstc = (WorkStreamTranstionComponent)m_CurUseEntity.m_StateTranstionComponent;

        if (!string.IsNullOrEmpty(str))
        {
            string[] workIdStrs = CombatHelp.GetStrParse1Array(str);
            for (int i = 0; i < workIdStrs.Length; i++)
            {
                uint workId = uint.Parse(workIdStrs[i]);

                List<WorkBlockData> workBlockDatas = WorkStreamConfigManager.Instance.GetWorkBlockDatas<WS_UIModelShowControllerEntity>(workId, wstc.m_CurWorkBlockData.AttachType);
                if (workBlockDatas == null || workBlockDatas.Count == 0)
                {
                    Lib.Core.DebugUtil.LogError($"WS_CombatBehaveAIControllerEntity类型的数据中没有workId ：{workId.ToString()}, attachType : {wstc.m_CurWorkBlockData.AttachType.ToString()}");
                    continue;
                }

                wstc.UpdateWorkBlockDatas(workBlockDatas);
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}