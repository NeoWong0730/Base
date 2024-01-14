using Lib.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：npc播放特效///
    /// 0: npcInfoID///
    /// 1: EffectID///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NpcPlayEffect)]
    public class WS_NPCBehaveAI_NpcPlayEffect_SComponent : StateBaseComponent
    {
        string[] strs;
        uint npcInfoId;
        uint effectID;
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

                npcInfoId = uint.Parse(strs[0]);
                effectID = uint.Parse(strs[1]);

                CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(effectID);
                if (cSVEffectData == null)
                {
                    DebugUtil.LogError($"CSVEffect.Data not found, id:{effectID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                if (GameCenter.uniqueNpcs.TryGetValue(npcInfoId, out npc))
                {
                    EffectUtil.Instance.LoadEffect(npc.uID, cSVEffectData.effects_path, npc.transform, EffectUtil.EEffectTag.InteractiveShow, cSVEffectData.fx_duration / 1000f);
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcInfoId}");
                }
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_NpcPlayEffect_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            npcInfoId = effectID = 0u;
            npc = null;

            base.Dispose();
        }
    }
}
