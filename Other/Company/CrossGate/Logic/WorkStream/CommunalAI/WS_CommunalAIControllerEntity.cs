using UnityEngine;using System.Collections.Generic;
using Lib.Core;

[StateController((int)StateCategoryEnum.CommunalAI, "Config/WorkStreamData/CommunalAI{0}/CommunalAI.txt")]
public class WS_CommunalAIControllerEntity : BaseStreamControllerEntity
{
	public override bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0)
	{
        DebugUtil.Log(ELogType.eCommunalAIWorkStream, $"<color=yellow>{Id.ToString()}--CommunalAIController初始化</color>");

		if (m_WorkStreamManagerEntity != workStreamManagerEntity)
		{
			m_WorkStreamManagerEntity = workStreamManagerEntity;
			OnInit((int)StateCategoryEnum.CommunalAI);
		}

		if (!PrepareCommunalAI(workId, workBlockDatas))
		{
			OnOver(false);
			return false;
		}

		return true;
	}

	public override void Dispose()
	{
        DebugUtil.Log(ELogType.eCommunalAIWorkStream, $"<color=yellow>{Id.ToString()}--CommunalAIController结束</color>");

		base.Dispose();
	}

	public bool PrepareCommunalAI(uint workId, List<WorkBlockData> workBlockDatas, int blockType = 0)
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
}