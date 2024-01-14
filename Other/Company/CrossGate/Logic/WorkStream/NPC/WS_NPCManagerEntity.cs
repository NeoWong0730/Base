using System;
using UnityEngine;

public class WS_NPCManagerEntity : WorkStreamManagerEntity
{
	#region 示例
	public static WS_NPCManagerEntity StartNPC<T>(uint workId, int attachType = 0, 
		SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream, 
		Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true, int blockType = 0, ulong uid = 0) where T : BaseStreamControllerEntity
	{
		WS_NPCManagerEntity me = CreateWorkStreamManagerEntity<WS_NPCManagerEntity>(isSelfDestroy);
		if (me.StartController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction, blockType, uid))
			return me;
		else
		{
			me.Dispose();
			return null;
		}
	}

	public static WS_NPCManagerEntity StartNPC02<T>(uint workId, int attachType = 0, 
		SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream, 
		Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true) where T : BaseStreamControllerEntity
	{
		WS_NPCManagerEntity me = CreateWorkStreamManagerEntity<WS_NPCManagerEntity>(isSelfDestroy);
		T t = me.CreateController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction);
		if (t != null && t.StartController())
			return me;
		else
		{
			me.Dispose();
			return null;
		}
	}

    public static WS_NPCManagerEntity StartNPC03(uint workId, int attachType = 0,
        SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
        Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true)
    {
        WS_NPCManagerEntity me = CreateWorkStreamManagerEntity<WS_NPCManagerEntity>(isSelfDestroy);
        WS_NPCControllerEntity t = me.CreateController<WS_NPCControllerEntity>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction);
        if (t != null)
        {
            if (t.DoInit())
            {
                if (t.StartController())
                {
                    return me;
                }
            }
        }

        me.Dispose();
        return null;
    }

    #endregion
}