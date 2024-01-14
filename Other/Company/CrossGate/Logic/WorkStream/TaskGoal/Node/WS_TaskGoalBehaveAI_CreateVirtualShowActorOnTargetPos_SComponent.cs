namespace Logic
{
    /// <summary>
    /// 任务表演节点：指定坐标生成虚拟Actor///
    /// 0: UID///
    /// 1: Type///
    /// 2: InfoID///
    /// 3: PosX///
    /// 4: PosY///
    /// 5: PosZ///
    /// 6: RotX///
    /// 7: RotY///
    /// 8: RotZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.CreateVirtualShowActorOnTargetPos)]
    public class WS_TaskGoalBehaveAI_CreateVirtualShowActorOnTargetPos_SComponent : WS_NPCBehaveAI_CreateVirtualShowActorOnTargetPos_SComponent
    {
        
    }
}
