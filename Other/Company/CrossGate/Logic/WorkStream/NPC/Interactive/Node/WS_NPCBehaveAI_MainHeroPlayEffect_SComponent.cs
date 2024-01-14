using Lib.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：主角播放特效///
    /// str: effectID///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroPlayEffect)]
    public class WS_NPCBehaveAI_MainHeroPlayEffect_SComponent : StateBaseComponent
    {
        uint effectID;

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

                CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(effectID);
                if (cSVEffectData == null)
                {
                    DebugUtil.LogError($"CSVEffect.Data not found, id:{effectID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                EffectUtil.Instance.LoadEffect(GameCenter.mainHero.uID, cSVEffectData.effects_path, GameCenter.mainHero.transform, EffectUtil.EEffectTag.InteractiveShow, cSVEffectData.fx_duration / 1000f);
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MainHeroPlayEffect_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            effectID = 0u;

            base.Dispose();
        }
    }
}
