using Lib.Core;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// 任务表演节点：NPC移动到玩家位置///
    /// 0: npcinfoid///
    /// 1: Speed///
    /// 2: OffsetPosX///
    /// 3：OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.TaskGoal, (int)TaskGoalEnum.NPCMoveToMainHero)]
    public class WS_TaskGoalBehaveAI_NPCMoveToMainHero_SComponent : WS_NPCBehaveAI_NPCMoveToMainHero_SComponent
    {
       
    }
}