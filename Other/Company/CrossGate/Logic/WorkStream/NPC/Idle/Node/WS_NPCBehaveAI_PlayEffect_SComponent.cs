using Lib.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：自身播放特效///
    /// str: EffectID///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.PlayEffect)]
    public class WS_NPCBehaveAI_PlayEffect_SComponent : StateBaseComponent
    {
        uint effectID;
        WS_NPCControllerEntity entity;

        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                effectID = uint.Parse(str);

                entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;

                CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(effectID);
                if (cSVEffectData == null)
                {
                    DebugUtil.LogError($"CSVEffect.Data not found, id:{effectID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                EffectUtil.Instance.LoadEffect(entity.npc.uID, cSVEffectData.effects_path, entity.npc.transform, EffectUtil.EEffectTag.InteractiveShow, cSVEffectData.fx_duration / 1000f);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_PlayEffect_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            effectID = 0u;
            entity = null;

            base.Dispose();
        }
    }
}
