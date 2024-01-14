using Lib.Core;
using Logic;
using Packet;
using Table;
using static Logic.Sys_Chat;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PopupBubble)]
public class WS_CombatBehaveAI_PopupBubble_SComponent : StateBaseComponent
{
    public override void Init(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
            var curMobEntity = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
            uint popupBubbleId = uint.Parse(str);
            if (popupBubbleId < 100)
            {
                var talkInfos = Net_Combat.Instance.GetTalkInfoId(cbace.m_BehaveAIControllParam == null ? -1 : cbace.m_BehaveAIControllParam.ExcuteTurnIndex);
                if (talkInfos != null)
                {
                    for (int i = 0, count = talkInfos.Count; i < count; i++)
                    {
                        var talkInfo = talkInfos[i];
                        //因为会给该次行为毫无关系的Mob弹对话框，所以注释掉
                        //if (talkInfo.UnitId == curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitId)
                        {
                            uint bubbleId = talkInfo.TalkId;
                            if (bubbleId > 0u)
                            {
                                CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
                                if (cSVBubbleData != null)
                                {
                                    if (cSVBubbleData.BubbleType == popupBubbleId)
                                    {
                                        MobEntity talkMob = MobManager.Instance.GetMob(talkInfo.UnitId);
                                        if (talkMob == null)
                                            continue;

                                        //气泡
                                        TriggerBattleBubbleEvt tbb = CombatObjectPool.Instance.Get<TriggerBattleBubbleEvt>();
                                        tbb.battleid = talkInfo.UnitId;
                                        tbb.bubbleid = bubbleId;
                                        tbb.ClientNum = talkMob.m_MobCombatComponent.m_ClientNum;
                                        Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnTriggerBattleBubble, tbb);
                                        CombatObjectPool.Instance.Push(tbb);


                                        //聊天
                                        bool pushMessageInChat = false;
                                        if (Sys_HUD.Instance.battleTypes.Contains(CombatManager.Instance.m_BattleTypeTb.id))
                                        {
                                            pushMessageInChat = true;
                                        }
                                        else if (Sys_HUD.Instance.mosterGroupIds.Contains(Sys_Fight.curMonsterGroupId))
                                        {
                                            pushMessageInChat = true;
                                        }
                                        if (pushMessageInChat)
                                        {
                                            uint fightType = talkMob.m_MobCombatComponent.m_BattleUnit.UnitType;
                                            uint battleId = talkInfo.UnitId;
                                            if (Sys_HUD.Instance.battleAttrs.ContainsKey(battleId))
                                            {
                                                if (fightType == (uint)EFightActorType.Monster || fightType == (uint)EFightActorType.Partner)
                                                {
                                                    ChatBaseInfo chatBaseInfo = new ChatBaseInfo();
                                                    chatBaseInfo.eActorType = (EFightActorType)talkMob.m_MobCombatComponent.m_BattleUnit.UnitType;
                                                    chatBaseInfo.nRoleID = 0;
                                                    if (chatBaseInfo.eActorType == EFightActorType.Partner)
                                                    {
                                                        CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(Sys_HUD.Instance.battleAttrs[battleId].notPlayerAttr);
                                                        if (cSVPartnerData != null)
                                                        {
                                                            chatBaseInfo.sSenderName = LanguageHelper.GetTextContent(cSVPartnerData.name);
                                                        }
                                                    }
                                                    else if (chatBaseInfo.eActorType == EFightActorType.Monster)
                                                    {
                                                        CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(Sys_HUD.Instance.battleAttrs[battleId].notPlayerAttr);
                                                        if (cSVMonsterData != null)
                                                        {
                                                            CSVAiBubbleChat.Data cSVAiBubbleChat = CSVAiBubbleChat.Instance.GetConfData(Sys_Fight.curMonsterGroupId);
                                                            if (cSVAiBubbleChat != null)
                                                            {
                                                                if (cSVAiBubbleChat.moster_id != null && cSVAiBubbleChat.moster_id.Contains(cSVMonsterData.id))
                                                                {
                                                                    chatBaseInfo.eActorType = EFightActorType.Partner;
                                                                }
                                                            }
                                                            //bool b_Mixuer = cSVMonsterData.id == 50118;
                                                            //if (b_Mixuer && Sys_Fight.curMonsterGroupId == 120502002)
                                                            //{
                                                            //    chatBaseInfo.eActorType = EFightActorType.Partner;
                                                            //}
                                                            //bool b_Hanke = cSVMonsterData.id == 50321;
                                                            //if (b_Hanke && Sys_Fight.curMonsterGroupId == 15510002)
                                                            //{
                                                            //    chatBaseInfo.eActorType = EFightActorType.Partner;
                                                            //}
                                                            chatBaseInfo.sSenderName = LanguageHelper.GetTextContent(cSVMonsterData.monster_name);
                                                        }
                                                    }
                                                    string content = LanguageHelper.GetTextContent(cSVBubbleData.BubbleText);
                                                    Sys_Chat.Instance.PushMessage(Packet.ChatType.Local, chatBaseInfo, content);
                                                }
                                            }
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
                        }
                    }
                }
            }
            else
            {
                if (curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Monster)
                {
                    CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitInfoId);
                    if (cSVMonsterData != null)
                    {
                        uint bubbleId = 0u;
                        if (popupBubbleId == 101)    //怪物进场冒气泡
                        {
                            bubbleId = cSVMonsterData.approach_bubble;
                        }
                        else if (popupBubbleId == 102)  //怪物死亡冒气泡
                        {
                            bubbleId = cSVMonsterData.die_bubble;
                        }

                        if (bubbleId > 0u)
                        {
                            TriggerBattleBubbleEvt tbb = CombatObjectPool.Instance.Get<TriggerBattleBubbleEvt>();
                            tbb.battleid = curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitId;
                            tbb.bubbleid = bubbleId;
                            tbb.ClientNum = curMobEntity.m_MobCombatComponent.m_ClientNum;
                            Sys_HUD.Instance.eventEmitter.Trigger(Sys_HUD.EEvents.OnTriggerBattleBubble, tbb);
                            CombatObjectPool.Instance.Push(tbb);
                        }


                        //聊天
                        bool pushMessageInChat = false;
                        if (Sys_HUD.Instance.battleTypes.Contains(CombatManager.Instance.m_BattleTypeTb.id))
                        {
                            pushMessageInChat = true;
                        }
                        else if (Sys_HUD.Instance.mosterGroupIds.Contains(Sys_Fight.curMonsterGroupId))
                        {
                            pushMessageInChat = true;
                        }
                        if (pushMessageInChat)
                        {
                            uint fightType = curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitType;
                            uint battleId = curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitId;
                            if (Sys_HUD.Instance.battleAttrs.ContainsKey(battleId))
                            {
                                if (fightType == (uint)EFightActorType.Monster || fightType == (uint)EFightActorType.Partner)
                                {
                                    ChatBaseInfo chatBaseInfo = new ChatBaseInfo();
                                    chatBaseInfo.eActorType = (EFightActorType)curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitType;
                                    chatBaseInfo.nRoleID = 0;
                                    if (chatBaseInfo.eActorType == EFightActorType.Partner)
                                    {
                                        CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(Sys_HUD.Instance.battleAttrs[battleId].notPlayerAttr);
                                        if (cSVPartnerData != null)
                                        {
                                            chatBaseInfo.sSenderName = LanguageHelper.GetTextContent(cSVPartnerData.name);
                                        }
                                    }
                                    else if (chatBaseInfo.eActorType == EFightActorType.Monster)
                                    {
                                        CSVAiBubbleChat.Data cSVAiBubbleChat = CSVAiBubbleChat.Instance.GetConfData(Sys_Fight.curMonsterGroupId);
                                        if (cSVAiBubbleChat != null)
                                        {
                                            if (cSVAiBubbleChat.moster_id != null && cSVAiBubbleChat.moster_id.Contains(cSVMonsterData.id))
                                            {
                                                chatBaseInfo.eActorType = EFightActorType.Partner;
                                            }
                                        }
                                        //bool b_Mixuer = cSVMonsterData.id == 50118;
                                        //if (b_Mixuer && Sys_Fight.curMonsterGroupId == 120502002)
                                        //{
                                        //    chatBaseInfo.eActorType = EFightActorType.Partner;
                                        //}
                                        //bool b_Hanke = cSVMonsterData.id == 50321;
                                        //if (b_Hanke && Sys_Fight.curMonsterGroupId == 15510002)
                                        //{
                                        //    chatBaseInfo.eActorType = EFightActorType.Partner;
                                        //}
                                        chatBaseInfo.sSenderName = LanguageHelper.GetTextContent(cSVMonsterData.monster_name);
                                    }
                                    CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
                                    if (cSVBubbleData != null)
                                    {
                                        string content = LanguageHelper.GetTextContent(cSVBubbleData.BubbleText);
                                        Sys_Chat.Instance.PushMessage(Packet.ChatType.Local, chatBaseInfo, content);
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }

        m_CurUseEntity.TranstionMultiStates(this);
    }
}