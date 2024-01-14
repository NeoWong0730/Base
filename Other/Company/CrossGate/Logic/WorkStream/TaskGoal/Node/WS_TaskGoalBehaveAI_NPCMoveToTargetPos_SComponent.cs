namespace Logic
{
    /// <summary>
    /// 任务表演节点：NPC移动到指定位置///
    /// 0: npcinfoid///
    /// 1: TargetPosX///
    /// 2: TargetPosY///
    /// 3: TargetPosZ///
    /// 4：Speed///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.NPCMoveToTargetPos)]
    public class WS_TaskGoalBehaveAI_NPCMoveToTargetPos_SComponent : WS_NPCBehaveAI_NPCMoveToTargetPos_SComponent
    {
        
    }
}