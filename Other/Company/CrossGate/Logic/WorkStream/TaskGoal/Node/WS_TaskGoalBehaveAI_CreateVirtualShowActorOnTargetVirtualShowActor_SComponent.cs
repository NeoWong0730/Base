namespace Logic
{
    /// <summary>
    /// 任务表演节点：指定虚拟Actor位置生成虚拟Actor///
    /// 0: UID///
    /// 1: Type///
    /// 2: InfoID///
    /// 3: targetUID///
    /// 4：OffsetPosX///
    /// 5: OffsetPosY///
    /// 6: OffsetPosZ///
    /// 7: RotX///
    /// 8: RotY///
    /// 9: RotZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.CreateVirtualShowActorOnTargetVirtualShowActor)]
    public class WS_TaskGoalBehaveAI_CreateVirtualShowActorOnTargetVirtualShowActor_SComponent : WS_NPCBehaveAI_CreateVirtualShowActorOnTargetVirtualShowActor_SComponent
    {
        
    }
}
