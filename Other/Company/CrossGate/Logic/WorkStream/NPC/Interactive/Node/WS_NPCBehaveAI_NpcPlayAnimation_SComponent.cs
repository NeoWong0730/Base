using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC播放动作///
    /// 0: npcInfoID///
    /// 1: Anim///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NpcPlayAnimation)]
    public class WS_NPCBehaveAI_NpcPlayAnimation_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcInfoID;
        Npc npc;

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

                npcInfoID = uint.Parse(strs[0]);

                if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                {
                    npc.AnimationComponent?.CrossFade(strs[1], 0.1f);
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcInfoID}");
                }
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_PlayAudio_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            npcInfoID = 0u;
            npc = null;

            base.Dispose();
        }
    }
}
