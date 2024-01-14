using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：主角播放动作///
    /// str: anim///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroPlayAnimation)]
    public class WS_NPCBehaveAI_MainHeroPlayAnimation_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                GameCenter.mainHero.animationComponent?.CrossFade(str, 0.1f);
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_MainHeroPlayAnimation_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }
    }
}
