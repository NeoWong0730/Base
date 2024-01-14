namespace Logic
{
    /// <summary>
    /// 任务表演节点：虚拟Actor朝向指定位置///
    /// 0: UID///
    /// 1: TargetPosX///
    /// 2: TargetPosY///
    /// 3: TargetPosZ///
    /// 4: Time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.VirtualShowActorLookToTargetPos)]
    public class WS_TaskGoalBehaveAI_VirtualShowActorLookToTargetPos_SComponent : WS_NPCBehaveAI_VirtualShowActorLookToTargetPos_SComponent
    {
       
    }
}
