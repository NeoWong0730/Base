using System;

public class WS_TaskGoalManagerEntity : WorkStreamManagerEntity
{
    public static WS_TaskGoalManagerEntity StartTaskGoal<T>(uint workId, int attachType = 0,
        SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
        Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true, int blockType = 0, ulong uid = 0) where T : BaseStreamControllerEntity
    {
        WS_TaskGoalManagerEntity me = CreateWorkStreamManagerEntity<WS_TaskGoalManagerEntity>(isSelfDestroy);
        if (me.StartController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction, blockType, uid))
            return me;
        else
        {
            me.Dispose();
            return null;
        }
    }
}
