using Lib.Core;
using Logic.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：自身冒气泡///
    /// str: bubbleId///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.PopupBubble)]
    public class WS_NPCBehaveAI_PopupBubble_SComponent : StateBaseComponent
    {
        uint bubbleId;
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

                entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;

                if (!entity.npc.VisualComponent.Visiable)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                bubbleId = uint.Parse(str);

                if (bubbleId > 0u)
                {
                    CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
                    if (cSVBubbleData != null)
                    {
                        if (cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                        {
                            TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
                            triggerNpcBubbleEvt.ownerType = 0;
                            triggerNpcBubbleEvt.npcid = entity.npc.uID;
                            triggerNpcBubbleEvt.npcInfoId = entity.npc.cSVNpcData.id;
                            triggerNpcBubbleEvt.bubbleid = bubbleId;
                            triggerNpcBubbleEvt.npcobj = entity.npc.gameObject;

                            Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);

                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.EmojiText)
                        {
                            TriggerExpressionBubbleEvt triggerEmotionBubbleEvt = new TriggerExpressionBubbleEvt();
                            triggerEmotionBubbleEvt.id = entity.npc.uID;
                            triggerEmotionBubbleEvt.npcInfoId = entity.npc.cSVNpcData.id;
                            triggerEmotionBubbleEvt.ownerType = 0;
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
                            triggerEmotionBubbleEvt.gameObject = entity.npc.gameObject;
                            triggerEmotionBubbleEvt.bubbleId = bubbleId;
                            Sys_HUD.Instance.eventEmitter.Trigger<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, triggerEmotionBubbleEvt);

                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.Pic)
                        {
                            CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                            createEmotionEvt.actorId = entity.npc.uID;
                            createEmotionEvt.gameObject = entity.npc.gameObject;
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
                DebugUtil.LogError($"WS_NPCBehaveAI_PopupBubble_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            bubbleId = 0u;
            entity = null;

            base.Dispose();
        }
    }
}