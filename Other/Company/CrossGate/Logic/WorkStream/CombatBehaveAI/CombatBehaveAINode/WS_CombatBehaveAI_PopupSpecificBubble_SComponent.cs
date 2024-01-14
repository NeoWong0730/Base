using Logic;
using Table;
using static Logic.Sys_Chat;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PopupSpecificBubble)]
public class WS_CombatBehaveAI_PopupSpecificBubble_SComponent : StateBaseComponent
{
    public override void Init(string str)
    {
        var curMobEntity = (MobEntity)((BaseStreamControllerEntity)m_CurUseEntity.m_StateControllerEntity).m_WorkStreamManagerEntity.Parent;
        uint bubbleId = uint.Parse(str);
        CSVBubble.Data cSVBubbleData = CSVBubble.Instance.GetConfData(bubbleId);
        if (cSVBubbleData != null)
        {
            //气泡
            TriggerBattleBubbleEvt tbb = CombatObjectPool.Instance.Get<TriggerBattleBubbleEvt>();
            tbb.battleid = curMobEntity.m_MobCombatComponent.m_BattleUnit.UnitId;
            tbb.bubbleid = bubbleId;
            tbb.ClientNum = curMobEntity.m_MobCombatComponent.m_ClientNum;

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
#if DEBUG_MODE
        else
        {
            Lib.Core.DebugUtil.LogError($"CSVBubbleData表中没有Id：{bubbleId.ToString()}");
        }
#endif

        m_CurUseEntity.TranstionMultiStates(this);
    }
}