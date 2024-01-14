using UnityEngine;
using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 任务表演节点：玩家移动到指定虚拟Actor位置///
    /// 0: TargetUID///
    /// 1: Speed///
    /// 2：OffsetPosX///
    /// 3: OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroMoveToTargetVirtualShowActor)]
    public class WS_NPCBehaveAI_MainHeroMoveToTargetVirtualShowActor_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        ulong targetUid;
        float Speed;
        float OffsetPosX;
        float OffsetPosY;
        float OffsetPosZ;
        VirtualSceneActor targetActor;

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

                targetUid = ulong.Parse(strs[0]);
                Speed = float.Parse(strs[1]);
                OffsetPosX = float.Parse(strs[2]);
                OffsetPosY = float.Parse(strs[3]);
                OffsetPosZ = float.Parse(strs[4]);

                if (VirtualShowManager.Instance.TryGetValue(targetUid, out targetActor))
                {
                    GameCenter.mainHero.movementComponent.fMoveSpeed = Speed;
                    GameCenter.mainHero.movementComponent.MoveTo(targetActor.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ), null, null, MoveToSuccess);
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetActorVirtualSceneActor uid:{uid}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MainHeroMoveToTargetVirtualShowActor_SComponent: {e.ToString()}");
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
            targetUid = 0ul;
            Speed = OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            targetActor = null;

            base.Dispose();
        }
    }
}
