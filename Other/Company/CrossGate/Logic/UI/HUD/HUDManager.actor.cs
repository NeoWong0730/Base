using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;

namespace Logic
{
    public partial class HUD : UIBase
    {
        #region ActorHUD

        public void CreateActorHUD(CreateActorHUDEvt createActorHUDEvt)
        {
            if (actorHuds.ContainsKey(createActorHUDEvt.id))
            {
                DebugUtil.LogErrorFormat("已经存在id={0}的actor", createActorHUDEvt.id);
                return;
            }
            GameObject go;
            ActorHUDShow actorHUDShow;
            go = ActorHUDPools.Get(root_ActorHUD);
            go.SetActive(true);
            if (createActorHUDEvt.eFightOutActorType == EFightOutActorType.Npc)
            {
                go.transform.SetAsLastSibling();
            }
            else
            {
                go.transform.SetAsFirstSibling();
            }
            actorHUDShow = HUDFactory.Get<ActorHUDShow>();
#if UNITY_EDITOR
            go.name = createActorHUDEvt.name;
#endif            
            actorHUDShow.Construct(go, _tansformWorldToScreen);
            actorHUDShow.SetUID(createActorHUDEvt.id);
            actorHUDShow.SetTarget(createActorHUDEvt.gameObject.transform);
            actorHUDShow.CalOffest(createActorHUDEvt.offest);
            actorHuds[createActorHUDEvt.id] = actorHUDShow;
            actorObjs[createActorHUDEvt.id] = createActorHUDEvt.gameObject;
            actorHUDShow.SetAppellation_Name(createActorHUDEvt.eFightOutActorType, createActorHUDEvt.appellation, createActorHUDEvt.name,createActorHUDEvt.isBack);

            if (createActorHUDEvt.eFightOutActorType == EFightOutActorType.Npc)
            {
                actorHUDShow.ShowOrHide(false);
            }
        }

        private void UpdateActorName(ActorHUDNameUpdateEvt evt)
        {
            if (actorHuds.TryGetValue(evt.id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateName(evt.eFightOutActorType, evt.name,evt.upBack);
            }
        }

        public void UpdateActorHUD_TitleName(ActorHUDTitleNameUpdateEvt actorHUDTitleNameUpdateEvt)
        {
            if (actorHuds.TryGetValue(actorHUDTitleNameUpdateEvt.id, out ActorHUDShow actorHUDShow))
            {
                actorHuds[actorHUDTitleNameUpdateEvt.id].UpdateAppellatiom_Name(actorHUDTitleNameUpdateEvt.eFightOutActorType);
            }
        }

        public void UpdateActorHUD_icon(ActorHUDUpdateEvt actorHUDUpdateEvt)
        {
            if (actorHuds.TryGetValue(actorHUDUpdateEvt.id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateState(actorHUDUpdateEvt.iconId);
            }
        }

        private void ShowOrHideActorHUD_Icon(ulong id, bool flag)
        {
            if (actorHuds.TryGetValue(id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ShowOrHideFun1Icon(flag);
                actorHUDShow.ShowOrHideFun2Icon(flag);
                actorHUDShow.ShowOrHideFun3Icon(flag);
            }
        }

        private void CreateTitle(CreateTitleEvt createTitleEvt)
        {
            if (actorHuds.TryGetValue(createTitleEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.CreateTitle(createTitleEvt.titleId, createTitleEvt.titleName, createTitleEvt.pos);
            }
        }

        private void ClearTitle(ClearTitleEvt clearTitleEvt)
        {
            if (actorHuds.TryGetValue(clearTitleEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ClearTitle();
            }
        }

        private void UpdateFamilyTitle(ulong actorId, uint title)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateFamilyTitleName(title);
            }
        }

        private void UpdateBGroupTitle(UpdateBGroupTitleEvt updateBGroupTitle)
        {
            if (actorHuds.TryGetValue(updateBGroupTitle.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateBGroupTitleName(updateBGroupTitle.titleId, updateBGroupTitle.name, updateBGroupTitle.pos);
            }
        }

        private void UpdateFavirability(UpdateFavirabilityEvt updateFavirabilityEvt)
        {
            if (actorHuds.TryGetValue(updateFavirabilityEvt.npcId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateFavirability(updateFavirabilityEvt.val);
            }
        }

        private void ClearFavirability(ClearFavirabilityEvt clearFavirabilityEvt)
        {
            if (actorHuds.TryGetValue(clearFavirabilityEvt.npcId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ClearFavirability();
            }
        }

        private void UpdateWorldBoss(UpdateWorldBossHuDEvt updateWorldBossHuDEvt)
        {
            if (actorHuds.TryGetValue(updateWorldBossHuDEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateWorldBoss(updateWorldBossHuDEvt.level, updateWorldBossHuDEvt.iconId);
            }
        }

        private void ClearWorldBoss(ClearWorldBossHuDEvt clearWorldBossHuDEvt)
        {
            if (actorHuds.TryGetValue(clearWorldBossHuDEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ClearWorldBoss();
            }
        }

        private void UpdateFightState(UpdateActorFightStateEvt updateActorFightStateEvt)
        {
            if (actorHuds.TryGetValue(updateActorFightStateEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateBattleState(updateActorFightStateEvt.state);
            }
        }

        private void PlayLevelUpFx(PlayActorLevelUpHudEvt playActorLevelUpHudEvt)
        {
            if (actorHuds.TryGetValue(playActorLevelUpHudEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.PlayLevelUpFx();
            }
        }

        private void PlayAdvanceUpFx(PlayActorAdvanceUpHudEvt playActorAdvanceUpHudEvt)
        {
            if (actorHuds.TryGetValue(playActorAdvanceUpHudEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.PlayAdvanceUpFx();
            }
        }

        private void PlayReputationUpFx(PlayActorReputationHudEvt playActorReputationUpHudEvt)
        {
            if (actorHuds.TryGetValue(playActorReputationUpHudEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.PlayReputationUpFx();
            }
        }

        private void CreateTeamLogo(ulong actorId, uint teamLogoId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.CreateTeamLogo(teamLogoId);
            }
        }

        private void ClearTeamLogo(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ClearTeamLogo();
            }
        }

        private void CreateTeamFx(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.CreateTeamFx();
            }
        }

        private void ClearTeamFx(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ClearTeamFx();
            }
        }


        private void CreateNpcBattleCd(NpcBattleCdEvt npcBattleCdEvt)
        {
            if (actorHuds.TryGetValue(npcBattleCdEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.CreateNpcBattleCd(npcBattleCdEvt.cd);
            }
        }

        public void CreateOrUpdateActorHUDStateFlag_icon(CreateOrUpdateActorHUDStateFlagEvt actorHUDUpdateStateFlagEvt)
        {
            if (!actorHuds.ContainsKey(actorHUDUpdateStateFlagEvt.id))
                return;
            ActorHUDShow actorHUDShow = actorHuds[actorHUDUpdateStateFlagEvt.id];
            actorHUDShow.UpdateStateFlag(actorHUDUpdateStateFlagEvt.type);
        }

        public void ClearActorHUDStateFlag(ulong id)
        {
            if (!actorHuds.ContainsKey(id))
                return;

            ActorHUDShow actorHUDShow = actorHuds[id];
            actorHUDShow.ClearStateFlag();
        }

        private void UpdateHeroFunState(ulong id, uint state)
        {
            if (actorHuds.TryGetValue(id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.UpdateHeroFunState(state);
            }
        }


        private void OnShowNpcArrow(ulong id)
        {
            if (actorHuds.TryGetValue(id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ShowNpcArrow();
            }
        }

        private void OnHideNpcArrow(ulong id)
        {
            if (actorHuds.TryGetValue(id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.HideNpcArrow();
            }
        }

        private void OnShowNpcSliderNotice(ulong id, uint duration)
        {
            if (actorHuds.TryGetValue(id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.ShowSliderNotice(duration);
            }
        }

        private void OnHideNpcSliderNotice(ulong id)
        {
            if (actorHuds.TryGetValue(id, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.HideSliderNotice();
            }
        }


        public void ShowOrHideActorHUD(ShowOrHideActorHUDEvt showOrHideActorHUDEvt)
        {
            if (!actorHuds.ContainsKey(showOrHideActorHUDEvt.id))
            {
                return;
            }
            ActorHUDShow actorHUDShow = actorHuds[showOrHideActorHUDEvt.id];
            actorHUDShow.ShowOrHide(showOrHideActorHUDEvt.flag);
        }

        public void ShowOrHideActorsHUD(ShowOrHideActorsHUDEvt showOrHideActorsHUDEvt)
        {
            if (showOrHideActorsHUDEvt.showIds != null)
            {
                for (int i = 0; i < showOrHideActorsHUDEvt.showIds.Count; i++)
                {
                    if (actorHuds.TryGetValue(showOrHideActorsHUDEvt.showIds[i], out ActorHUDShow actorHUDShow))
                    {
                        actorHUDShow.ShowOrHide(true);
                    }
                }
            }
            if (showOrHideActorsHUDEvt.hideIds != null)
            {
                for (int i = 0; i < showOrHideActorsHUDEvt.hideIds.Count; i++)
                {
                    if (actorHuds.TryGetValue(showOrHideActorsHUDEvt.hideIds[i], out ActorHUDShow actorHUDShow))
                    {
                        actorHUDShow.ShowOrHide(false);
                    }
                }
            }
        }

        public void RemoveActorHUD(ulong id)
        {
            if (!actorHuds.ContainsKey(id))
            {
                return;
            }

            ActorHUDShow actorHUDShow = actorHuds[id];
            actorHUDShow.Dispose();
            HUDFactory.Recycle(actorHUDShow);
            ActorHUDPools.Recovery(actorHUDShow.mRootGameObject);

            actorHuds.Remove(id);
            actorObjs.Remove(id);
            if (npcBubbleHuds.ContainsKey(id))
            {
                NpcBubbleShow bubbleShow = npcBubbleHuds[id];
                if (bubbleShow != null)
                {
                    bubbleShow.Dispose();
                    HUDFactory.Recycle(bubbleShow);
                    NpcBubblePools.Recovery(bubbleShow.mRootGameobject);
                    npcBubbleHuds.Remove(id);
                }
            }
        }

        private void OnClearActorFx()
        {
            foreach (var item in actorHuds)
            {
                item.Value.ClearFx();
            }
        }

        public void ActiveAllActorHUD()
        {
            root_ActorHUD.gameObject.SetActive(true);
        }

        public void InActiveAllActorHUD()
        {
            root_ActorHUD.gameObject.SetActive(false);
        }

        private void OnUpMount(UpMountEvt upMountEvt)
        {
            if (actorHuds.TryGetValue(upMountEvt.actorId, out ActorHUDShow actorHUDShow))
            {
                CSVPetNew.Data cSVPetNew = CSVPetNew.Instance.GetConfData(upMountEvt.mountId);
                Vector3 offest = new Vector3(0, (cSVPetNew.mountsignposition[1] / 1000f), 0);
                actorHUDShow.OnUpMount(offest);
            }
        }

        private void OnDownMount(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnDownMount();
            }
        }

        private void OnScaleUp(ulong actorId, uint scale)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnScaleUp(scale);
            }
        }

        private void OnResetScale(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnResetScale();
            }
        }

        private void OnCreateFamilyBattle(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                //Hero hero = GameCenter.mainWorld.GetActor(typeof(Hero), actorId) as Hero;
                Hero hero = GameCenter.GetSceneHero(actorId);
                actorHUDShow.OnCreateFamilyBattle(hero.heroBaseComponent.FamilyName, hero.heroBaseComponent.Pos);
            }
        }

        private void OnUpdateFamilyName(UpdateFamliyNameBeforeBattleRescorce updateFamliyNameBeforeBattleRescorce)
        {
            if (actorHuds.TryGetValue(updateFamliyNameBeforeBattleRescorce.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnUpdateFamilyName(updateFamliyNameBeforeBattleRescorce.name, updateFamliyNameBeforeBattleRescorce.pos);
            }
        }

        private void OnClearFamilyBattle(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnClearFamilyBattle();
            }
        }

        private void OnUpdateFamilyBattleResource(ulong actorId, uint resourceId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnUpdateFamilyBattleResource(resourceId);
            }
        }

        private void OnUpdateFamilyTeamNum(UpdateFamilyTeamNumInBattleResource updateFamilyTeamNumInBattleResource)
        {
            if (actorHuds.TryGetValue(updateFamilyTeamNumInBattleResource.actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnUpdateFamilyTeamNum(updateFamilyTeamNumInBattleResource.teamNum, updateFamilyTeamNumInBattleResource.maxCount);
            }
        }

        private void OnUpdateGuildBattleName(ulong actorId)
        {
            if (actorHuds.TryGetValue(actorId, out ActorHUDShow actorHUDShow))
            {
                actorHUDShow.OnUpdateGuildBattleName();
            }
        }
        

        #endregion
    }
}

