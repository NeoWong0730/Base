using UnityEngine;using System.Collections.Generic;
using Lib.Core;

[StateController((int)StateCategoryEnum.UIModelShow, "Config/WorkStreamData/UIModelShow{0}/UIModelShow.txt")]
public class WS_UIModelShowControllerEntity : BaseStreamControllerEntity
{
	public override bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0)
	{
        DebugUtil.Log(ELogType.eUIModelShowWorkStream, $"<color=yellow>{Id.ToString()}--UIModelShowController初始化</color>");

		if (m_WorkStreamManagerEntity != workStreamManagerEntity)
		{
			m_WorkStreamManagerEntity = workStreamManagerEntity;
			OnInit((int)StateCategoryEnum.UIModelShow);
		}

		if (!PrepareUIModelShow(workId, workBlockDatas))
		{
			OnOver(false);
			return false;
		}

		return true;
	}

	public override void Dispose()
	{
        DebugUtil.Log(ELogType.eUIModelShowWorkStream, $"<color=yellow>{Id.ToString()}--UIModelShowController结束</color>");
        
        if (!CombatManager.Instance.m_IsFight)
        {
            CombatManager.Instance.m_CombatStyleState = -1;
            CombatModelManager.Instance.OnDisable();
        }

        base.Dispose();
	}

	public bool PrepareUIModelShow(uint workId, List<WorkBlockData> workBlockDatas, int blockType = 0)
	{
		if (workBlockDatas == null || workBlockDatas.Count == 0)
			return false;

		if (m_FirstMachine != null)
			m_FirstMachine.Dispose();

		m_FirstMachine = CreateFirstStateMachineEntity();
		WorkStreamTranstionComponent workStreamTranstionComponent = m_FirstMachine.AddTranstion<WorkStreamTranstionComponent>();
		workStreamTranstionComponent.InitWorkBlockDatas(workId, workBlockDatas);
		m_ControllerBeginAction?.Invoke(this);

		return true;
	}

	/// <summary>
	/// 自定义专属初始化函数，参数根据情况自定义添加
	/// 也可以依此拓展自己定义的函数
	/// </summary>
	public bool DoInit() 
	{
		return true;
	}

    public override bool StartController(int blockType = 0)
    {
        if (base.StartController(blockType))
        {
            if (!CombatManager.Instance.m_IsFight)
            {
                CombatManager.Instance.m_CombatStyleState = 10001;
                CombatModelManager.Instance.OnEnable();
            }
            return true;
        }

        return false;
    }
}