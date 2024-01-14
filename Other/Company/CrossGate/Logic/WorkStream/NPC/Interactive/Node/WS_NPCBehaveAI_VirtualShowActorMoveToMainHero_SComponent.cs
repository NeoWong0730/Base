using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor移动到玩家位置///
    /// 0: UID///
    /// 1: Speed///
    /// 2: OffsetPosX///
    /// 3：OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorMoveToMainHero)]
    public class WS_NPCBehaveAI_VirtualShowActorMoveToMainHero_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        float Speed;
        float OffsetPosX;
        float OffsetPosY;
        float OffsetPosZ;
        VirtualSceneActor actor;

        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                strs = CombatHelp.GetStrParse1Array(str);

                uid = ulong.Parse(strs[0]);
                Speed = float.Parse(strs[1]);
                OffsetPosX = float.Parse(strs[2]);
                OffsetPosY = float.Parse(strs[3]);
                OffsetPosZ = float.Parse(strs[4]);

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    actor.movementComponent.fMoveSpeed = Speed;
                    actor.movementComponent?.MoveTo(GameCenter.mainHero.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ), null, null, MoveToSuccess);
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorMoveToMainHero_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        void MoveToSuccess()
        {
            m_CurUseEntity?.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            strs = null;
            uid = 0ul;
            OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            actor = null;

            base.Dispose();
        }
    }
}
