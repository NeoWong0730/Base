using Lib.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：NPC冒气泡///
    /// 0: bubbleId///
    /// 1: npcInfoID///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.NpcPopupBubble)]
    public class WS_NPCBehaveAI_NpcPopupBubble_SComponent : StateBaseComponent
    {
        string[] strs;
        uint bubbleId;
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

                bubbleId = uint.Parse(strs[0]);
                npcInfoID = uint.Parse(strs[1]);

                if (bubbleId > 0u)
                {
                    CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
                    if (cSVBubbleData != null)
                    {
                        if (cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                        {
                            if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                            {
                                TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
                                triggerNpcBubbleEvt.ownerType = 0;
                                triggerNpcBubbleEvt.npcid = npc.uID;
                                triggerNpcBubbleEvt.npcInfoId = npcInfoID;
                                triggerNpcBubbleEvt.bubbleid = bubbleId;
                                triggerNpcBubbleEvt.npcobj = npc.gameObject;

                                Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);
                            }
                            else
                            {
                                DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcInfoID}");
                            }
                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.EmojiText)
                        {
                            if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                            {
                                TriggerExpressionBubbleEvt triggerEmotionBubbleEvt = new TriggerExpressionBubbleEvt();
                                triggerEmotionBubbleEvt.id = npc.uID;
                                triggerEmotionBubbleEvt.npcInfoId = npcInfoID;
                                triggerEmotionBubbleEvt.ownerType = 0;
                                triggerEmotionBubbleEvt.bubbleId = bubbleId;
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
                                triggerEmotionBubbleEvt.gameObject = npc.gameObject;
                                Sys_HUD.Instance.eventEmitter.Trigger<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, triggerEmotionBubbleEvt);
                            }
                            else
                            {
                                DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcInfoID}");
                            }
                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.Pic)
                        {
                            if (GameCenter.uniqueNpcs.TryGetValue(npcInfoID, out npc))
                            {
                                CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                                createEmotionEvt.actorId = npc.uID;
                                createEmotionEvt.gameObject = npc.gameObject;
                                createEmotionEvt.emtionId = cSVBubbleData.MoodId;
                                Sys_HUD.Instance.eventEmitter.Trigger<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, createEmotionEvt);
                            }
                            else
                            {
                                DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found Npc npcinfoid:{npcInfoID}");
                            }
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
                DebugUtil.LogError($"WS_NPCBehaveAI_NpcPopupBubble_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            bubbleId = npcInfoID = 0u;
            npc = null;

            base.Dispose();
        }
    }
}