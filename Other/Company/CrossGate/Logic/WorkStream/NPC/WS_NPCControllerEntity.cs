using UnityEngine;
using System.Collections.Generic;
using Logic;
using Logic.Core;

[StateController((int)StateCategoryEnum.NPC, "Config/WorkStreamData/NPC{0}/NPC.txt")]
public class WS_NPCControllerEntity : BaseStreamControllerEntity
{
    public Npc npc;
    public AnimationComponent m_AnimationComponent;
    public MovementComponent m_MovementComponent;
    public string m_CurAnimationName;
    public GameObject m_Go;

    public override bool PrepareControllerEntity(WorkStreamManagerEntity workStreamManagerEntity, uint workId, List<WorkBlockData> workBlockDatas, ulong uid = 0)
	{
        Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eWorkStream, $"<color=yellow>{Id.ToString()}--NPCController初始化</color>");

		if (m_WorkStreamManagerEntity != workStreamManagerEntity)
		{
			m_WorkStreamManagerEntity = workStreamManagerEntity;
			OnInit((int)StateCategoryEnum.NPC);
		}

		if (!PrepareNPC(workId, workBlockDatas, uid))
		{
			OnOver(false);
			return false;
		}

		return true;
	}

	public override void Dispose()
	{
        Lib.Core.DebugUtil.Log(Lib.Core.ELogType.eWorkStream, $"<color=yellow>{Id.ToString()}--NPCController结束</color>");

		base.Dispose();
	}

	public bool PrepareNPC(uint workId, List<WorkBlockData> workBlockDatas, ulong uid)
	{
        ///交互
        if (uid == 0)
        {

        }
        ///某个NPC
        else
        {
            //npc = GameCenter.mainWorld.GetActor(typeof(Npc), uid) as Npc;
            //if (npc != null)
            if (GameCenter.TryGetSceneNPC(uid, out npc))
            {
                m_Go = npc.gameObject;
                m_AnimationComponent = npc.AnimationComponent;
                m_MovementComponent = npc.movementComponent;

                //m_AnimationComponent = World.GetComponent<AnimationComponent>(npc);
                //m_MovementComponent = World.GetComponent<MovementComponent>(npc);
            }
            else
            {
                return false;
            }
        }

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