using Lib.Core;
using Packet;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[StateController((int)StateCategoryEnum.CombatBehaveAI, "Config/WorkStreamData/CombatBehaveAI{0}/CombatBehaveAI.txt")]
public class WS_CombatSceneAIControllerEntity : BaseStreamControllerEntity
{
    public int m_ExcuteIndex;

    public OutBoInfo m_OutBoInfo;

    public override void Dispose()
    {
        m_ExcuteIndex = -1;

        m_OutBoInfo = null;

        base.Dispose();
    }

    public override bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0)
    {
        DLogManager.Log(ELogType.eCombat, $"<color=yellow>{Id.ToString()}--CombatBehaveAIController初始化</color>");

        if (m_WorkStreamManagerEntity != workStreamManagerEntity)
        {
            m_WorkStreamManagerEntity = workStreamManagerEntity;
            OnInit((int)StateCategoryEnum.CombatBehaveAI);
        }

        if (!PrepareCombatSceneAI(workId, workBlockDatas))
        {
            OnOver(false);
            return false;
        }

        return true;
    }
    
    public bool PrepareCombatSceneAI(uint workId, List<WorkBlockData> workBlockDatas, int blockType = 0)
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

    public bool StartSceneController(int excuteIndex, OutBoInfo outBoInfo)
    {
        m_ExcuteIndex = excuteIndex;

        m_OutBoInfo = outBoInfo;

        return StartController();
    }
}
