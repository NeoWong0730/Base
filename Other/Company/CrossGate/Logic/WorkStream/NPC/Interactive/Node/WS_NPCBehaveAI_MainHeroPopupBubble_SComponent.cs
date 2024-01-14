using Lib.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：主角冒气泡///
    /// str: bubbleId///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.MainHeroPopupBubble)]
    public class WS_NPCBehaveAI_MainHeroPopupBubble_SComponent : StateBaseComponent
    {
        uint bubbleId;

        public override void Init(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }

            DebugUtil.Log(ELogType.eWorkStream, $"str: {str}");
            try
            {
                bubbleId = uint.Parse(str);

                if (bubbleId > 0u)
                {
                    CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
                    if (cSVBubbleData != null)
                    {
                        if (cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                        {
                            TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
                            triggerNpcBubbleEvt.ownerType = 1;
                            triggerNpcBubbleEvt.bubbleid = bubbleId;
                            triggerNpcBubbleEvt.playInfoId = Sys_Role.Instance.Role.HeroId;
                            triggerNpcBubbleEvt.npcobj = GameCenter.mainHero.gameObject;

                            Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);
                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.EmojiText || cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                        {
                            TriggerExpressionBubbleEvt triggerEmotionBubbleEvt = new TriggerExpressionBubbleEvt();
                            triggerEmotionBubbleEvt.ownerType = 1u;
                            triggerEmotionBubbleEvt.playInfoId = Sys_Role.Instance.Role.HeroId;
                            CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(cSVBubbleData.BubbleText);
                            if (cSVLanguageData != null)
                            {
                                triggerEmotionBubbleEvt.content = cSVLanguageData.words;
                            }
                            else
                            {
                                DebugUtil.LogError($"CSVLanguage.Data is null id:{cSVBubbleData.BubbleText}");
                            }
                            triggerEmotionBubbleEvt.showTime = cSVBubbleData.BubbleTime;
                            triggerEmotionBubbleEvt.gameObject = GameCenter.mainHero.gameObject;
                            triggerEmotionBubbleEvt.bubbleId = bubbleId;
                            Sys_HUD.Instance.eventEmitter.Trigger<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, triggerEmotionBubbleEvt);
                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.Pic)
                        {
                            CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                            createEmotionEvt.gameObject = GameCenter.mainHero.gameObject;
                            createEmotionEvt.emtionId = cSVBubbleData.MoodId;
                            Sys_HUD.Instance.eventEmitter.Trigger<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, createEmotionEvt);
                        }
                    }
#if DEBUG_MODE
                    else
                    {
                        Lib.Core.DebugUtil.LogError($"CSVBubbleData表中没有Id：{bubbleId.ToString()}");
                    }
#endif
                }
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError(e.ToString());
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            bubbleId = 0u;

            base.Dispose();
        }
    }
}