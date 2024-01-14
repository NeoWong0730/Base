using System.ComponentModel;

public enum NPCEnum
{
    None = 0,

    #region 1-5000是node节点枚举

    [Description("自身播放特效")]
    PlayEffect = 1,
    [Description("等待时间")]
    WaitTime = 3,
    [Description("自身播放动作")]
    PlayAnimation = 4,
    [Description("自身向目标点位移")]
    MoveToTargetPos = 5,
    [Description("等待随机时间")]
    WaitRandomTime = 6,
    [Description("自身朝向指定位置")]
    LookToTarget = 7,
    [Description("记录初始点位置")]
    CacheStartTransform = 8,
    [Description("返回初始点位置")]
    BackToStartTransform = 9,
    [Description("自身播放随机动作")]
    PlayRandomAnimation = 10,
    [Description("播放音效")]
    PlayAudio = 20,
    [Description("自身冒气泡")]
    PopupBubble = 23,
    [Description("循环节点标记")]
    LoopNodeMark = 28,
    [Description("返回循环节点标记")]
    BackToLoopNodeMark = 29,

    [Description("自身播放条件动作")]
    PlayConditionAnimation = 30,

    [Description("指定坐标生成虚拟Actor")]
    CreateVirtualShowActorOnTargetPos = 31,
    [Description("指定虚拟Actor位置生成虚拟Actor")]
    CreateVirtualShowActorOnTargetVirtualShowActor = 32,
    [Description("玩家位置生成虚拟Actor")]
    CreateVirtualShowActorOnMainHero = 33,
    [Description("指定NPC位置生成虚拟Actor")]
    CreateVirualShowActorOnNpc = 34,

    [Description("删除指定虚拟Actor")]
    DeleteVirtualShowActor = 41,
    [Description("删除所有虚拟Actor")]
    DeleteAllVirtualShowActors = 42,

    [Description("虚拟Actor移动到指定位置")]
    VirtualShowActorMoveToTargetPos = 51,
    [Description("虚拟Actor移动到指定虚拟Actor位置")]
    VirtualShowActorMoveToTargetVirtualShowActor = 52,
    [Description("虚拟Actor移动到玩家位置")]
    VirtualShowActorMoveToMainHero = 53,
    [Description("虚拟Actor移动到指定NPC位置")]
    VirtualShowActorMoveToNPC = 54,
    [Description("玩家移动到指定位置")]
    MainHeroMoveToTargetPos = 55,
    [Description("玩家移动到指定虚拟Actor位置")]
    MainHeroMoveToTargetVirtualShowActor = 56,
    [Description("玩家移动到指定NPC位置")]
    MainHeroMoveToNPC = 57,
    [Description("NPC移动到指定位置")]
    NPCMoveToTargetPos = 58,
    [Description("NPC移动到指定虚拟Actor位置")]
    NPCMoveToTargetVirtualShowActor = 59,
    [Description("NPC移动到玩家位置")]
    NPCMoveToMainHero = 60,

    [Description("虚拟Actor朝向指定位置")]
    VirtualShowActorLookToTargetPos = 61,
    [Description("虚拟Actor朝向指定虚拟Actor位置")]
    VirtualShowActorLookToTargetVirtualShowActor = 62,
    [Description("虚拟Actor朝向玩家位置")]
    VirtualShowActorLookToMainHero = 63,
    [Description("虚拟Actor朝向指定NPC位置")]
    VirtualShowActorLookToNPC = 64,
    [Description("玩家朝向指定位置")]
    MainHeroLookToTargetPos = 65,
    [Description("玩家朝向指定Actor位置")]
    MainHeroLookToTargetVirtualShowActor = 66,
    [Description("玩家朝向指定NPC位置")]
    MainHeroLookToNPC = 67,
    [Description("NPC朝向指定位置")]
    NPCLookToTargetPos = 68,
    [Description("NPC朝向指定虚拟Actor位置")]
    NPCLookToTargetVirtualShowActor = 69,
    [Description("NPC朝向玩家位置")]
    NPCLookToMainHero = 70,

    [Description("虚拟Actor播放动作")]
    VirtualShowActorPlayAnimation = 71,
    [Description("玩家播放动作")]
    MainHeroPlayAnimation = 72,
    [Description("NPC播放动作")]
    NpcPlayAnimation = 73,

    [Description("虚拟Actor冒气泡")]
    VirtualShowActorPopupBubble = 81,
    [Description("玩家冒气泡")]
    MainHeroPopupBubble = 82,
    [Description("NPC冒气泡")]
    NpcPopupBubble = 83,

    [Description("所有虚拟伙伴返回玩家")]
    AllVirtualShowParnterReturnToMainHero = 91,

    [Description("虚拟Actor播放特效")]
    VirtualShowActorPlayEffect = 101,
    [Description("主角播放特效")]
    MainHeroPlayEffect = 102,
    [Description("NPC播放特效")]
    NpcPlayEffect = 103,

    #endregion

    #region 1101+是block枚举

    [Description("生成时")]
    B_BeCreate = 1101,

    [Description("进入触发范围时")]
    B_EnterTrigger = 1201,

    [Description("开始交互时")]
    B_StartInteractive = 1301,

    [Description("结束交互时")]
    B_EndInteractive = 1401,

    [Description("怪物进入战斗时")]
    B_WorldBossEnterFight = 1501,

    [Description("怪物战斗胜利时")]
    B_WorldBossExitFight_Success = 1502,

    [Description("怪物战斗失败时")]
    B_WorldBossExitFight_Faild = 1503,

    [Description("怪物死亡时")]
    B_WorldBossDead = 1504,

    #endregion
}