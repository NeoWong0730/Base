namespace Logic
{
    /// <summary>
    /// 任务表演节点：NPC朝向指定位置///
    /// 0: NPCInfoID///
    /// 1: TargetPosX///
    /// 2: TargetPosY///
    /// 3: TargetPosZ///
    /// 4: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.NPCLookToTargetPos)]
    public class WS_TaskGoalBehaveAI_NPCLookToTargetPos_SComponent : WS_NPCBehaveAI_NPCLookToTargetPos_SComponent
    {
        
    }
}