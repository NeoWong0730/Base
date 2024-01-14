using Lib.Core;
using System;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：玩家移动到指定NPC位置///
    /// 0: TargetNPCID///
    /// 1: Speed///
    /// 2：OffsetPosX///
    /// 3: OffsetPosY///
    /// 4: OffsetPosZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroMoveToNPC)]
    public class WS_NPCBehaveAI_MainHeroMoveToNPC_SComponent : StateBaseComponent
    {
        string[] strs;
        uint targetNPCID;
        float Speed;
        float OffsetPosX;
        float OffsetPosY;
        float OffsetPosZ;
        Npc targetNpc;

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

                targetNPCID = uint.Parse(strs[0]);
                Speed = float.Parse(strs[1]);
                OffsetPosX = float.Parse(strs[2]);
                OffsetPosY = float.Parse(strs[3]);
                OffsetPosZ = float.Parse(strs[4]);

                if (GameCenter.uniqueNpcs.TryGetValue(targetNPCID, out targetNpc))
                {
                    GameCenter.mainHero.movementComponent.fMoveSpeed = Speed;
                    GameCenter.mainHero.movementComponent.MoveTo(targetNpc.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ), null, null, MoveToSuccess);
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetNPC targetNPCID:{targetNPCID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MainHeroMoveToNPC_SComponent: {e.ToString()}");
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
            targetNPCID = 0u;
            Speed = OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            targetNpc = null;

            base.Dispose();
        }
    }
}
