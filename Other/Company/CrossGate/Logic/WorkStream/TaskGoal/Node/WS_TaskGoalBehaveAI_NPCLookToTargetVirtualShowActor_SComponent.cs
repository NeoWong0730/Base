namespace Logic
{
    /// <summary>
    /// 任务表演节点：NPC朝向指定虚拟Actor位置
    /// 0: npcInfoID///
    /// 1: TargetUID///
    /// 2: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.NPCLookToTargetVirtualShowActor)]
    public class WS_TaskGoalBehaveAI_NPCLookToTargetVirtualShowActor_SComponent : WS_NPCBehaveAI_NPCLookToTargetVirtualShowActor_SComponent
    {
       
    }
}