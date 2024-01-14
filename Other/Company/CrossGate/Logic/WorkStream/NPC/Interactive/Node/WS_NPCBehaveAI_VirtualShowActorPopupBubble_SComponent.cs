using Lib.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor冒气泡///
    /// 0: bubbleId///
    /// 1: uid///
    /// 2: npcInfoID///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorPopupBubble)]
    public class WS_NPCBehaveAI_VirtualShowActorPopupBubble_SComponent : StateBaseComponent
    {
        string[] strs;
        uint bubbleId;
        ulong uid;
        uint npcInfoID;
        VirtualSceneActor actor;

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
                uid = ulong.Parse(strs[1]);
                npcInfoID = uint.Parse(strs[2]);

                if (bubbleId > 0u)
                {
                    CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
                    if (cSVBubbleData != null)
                    {
                        if (cSVBubbleData.Type == (uint)EBubbleType.PureWord)
                        {
                            if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                            {
                                TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
                                triggerNpcBubbleEvt.ownerType = 0;
                                triggerNpcBubbleEvt.npcid = uid;
                                triggerNpcBubbleEvt.npcInfoId = npcInfoID;
                                triggerNpcBubbleEvt.bubbleid = bubbleId;
                                triggerNpcBubbleEvt.npcobj = actor.gameObject;

                                Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);
                            }
                            else
                            {
                                DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                            }
                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.EmojiText)
                        {
                            if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                            {
                                TriggerExpressionBubbleEvt triggerEmotionBubbleEvt = new TriggerExpressionBubbleEvt();
                                triggerEmotionBubbleEvt.id = uid;
                                triggerEmotionBubbleEvt.npcInfoId = npcInfoID;
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
                                triggerEmotionBubbleEvt.gameObject = actor.gameObject;
                                triggerEmotionBubbleEvt.bubbleId = bubbleId;
                                Sys_HUD.Instance.eventEmitter.Trigger<TriggerExpressionBubbleEvt>(Sys_HUD.EEvents.OnTriggerExpressionBubble, triggerEmotionBubbleEvt);
                            }
                            else
                            {
                                DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                            }
                        }
                        else if (cSVBubbleData.Type == (uint)EBubbleType.Pic)
                        {
                            if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                            {
                                CreateEmotionEvt createEmotionEvt = new CreateEmotionEvt();
                                createEmotionEvt.actorId = uid;
                                createEmotionEvt.gameObject = actor.gameObject;
                                createEmotionEvt.emtionId = cSVBubbleData.MoodId;
                                Sys_HUD.Instance.eventEmitter.Trigger<CreateEmotionEvt>(Sys_HUD.EEvents.OnCreateEmotion, createEmotionEvt);
                            }
                            else
                            {
                                DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
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
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorPopupBubble_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            bubbleId = npcInfoID = 0u;
            uid = 0L;
            actor = null;

            base.Dispose();
        }
    }
}