using UnityEngine;
using System.Collections.Generic;
using Logic;
using Logic.Core;

[StateController((int)StateCategoryEnum.TaskGoal, "Config/WorkStreamData/TaskGoal{0}/TaskGoal.txt")]
public class WS_TaskGoalControllerEntity : BaseStreamControllerEntity
{
    public override bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0)
    {
        Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eWorkStream, $"<color=yellow>{Id.ToString()}--TaskGoalController初始化</color>");

        if (m_WorkStreamManagerEntity != workStreamManagerEntity)
        {
            m_WorkStreamManagerEntity = workStreamManagerEntity;
            OnInit((int)StateCategoryEnum.TaskGoal);
        }

        if (!PrepareTaskGoal(workId, workBlockDatas, uid))
        {
            OnOver(false);
            return false;
        }

        return true;
    }

    public override void Dispose()
    {
        Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eWorkStream, $"<color=yellow>{Id.ToString()}--TaskGoalController结束</color>");

        base.Dispose();
    }

    public bool PrepareTaskGoal(uint workId, List<WorkBlockData> workBlockDatas, ulong uid)
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
}
