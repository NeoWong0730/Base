using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class Sys_CollectItem : SystemModuleBase<Sys_CollectItem>
    {
        public enum EEvents
        {
            OnCollectStarted,
            OnCollectEnded,
            OnCollectFaild,

            OnCollectSuccess,

            CollectBirth,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public Timer collectTimer;

        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdNpc.ResourceBeginReq, (ushort)CmdNpc.ResourceBeginRes, OnResourceBeginRes, CmdNpcResourceBeginRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdNpc.ResourceEndReq, (ushort)CmdNpc.ResourceEndRes, OnResourceEndRes, CmdNpcResourceEndRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdNpc.CollectionNtf, OnCollectionNtf, CmdNpcCollectionNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdNpc.ResourceErrNtf, OnResourceErrNtf, CmdNpcResourceErrNtf.Parser);
            Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnTransTipStart, OnTransTipStart, true);
        }

        void OnTransTipStart(uint id)
        {
            CollectionCtrl.Instance.CanCollection = false;
        }

        private void OnResourceBeginRes(NetMsg msg)
        {
            //GameCenter.mainHero.stateComponent.CurrentState = EStateType.Collection;

            if (CollectionAction.CurrentCSVCollectionData == null)
                return;

            if (GameCenter.mainHero.stateComponent.CurrentState != EStateType.Collection)
                return;

            GameCenter.mainHero.OffMount();

            CmdNpcResourceBeginRes res = NetMsgUtil.Deserialize<CmdNpcResourceBeginRes>(CmdNpcResourceBeginRes.Parser, msg);
            eventEmitter.Trigger<ulong>(EEvents.OnCollectStarted, res.UNpcId);
            Sys_Npc.Instance.eventEmitter.Trigger(Sys_Npc.EEvents.OnNearNpcClose);
            //GameCenter.mainHero.animationComponent.StopAll();
            CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(CollectionAction.CurrentCSVCollectionData.id);
            if (cSVCollectionProcessData != null)
            {
                if (cSVCollectionProcessData.collectingArms != 0)
                {
                    GameCenter.mainHero.heroLoader.LoadVirtualWeapon(cSVCollectionProcessData.collectingArms, () =>
                    {
                        if (cSVCollectionProcessData.collectingArms == 1100)
                        {
                            GameCenter.mainHero.animationComponent.Play("action_logging");
                        }
                        else if (cSVCollectionProcessData.collectingArms == 1101)
                        {
                            GameCenter.mainHero.animationComponent.Play("action_fish");
                        }
                        else if (cSVCollectionProcessData.collectingArms == 1102)
                        {
                            GameCenter.mainHero.animationComponent.Play("action_mining");
                        }
                        else if (cSVCollectionProcessData.collectingArms == 1103)
                        {
                            GameCenter.mainHero.animationComponent.Play("action_hunt");
                        }
                        //GameCenter.mainHero.animationComponent.UpdateHoldingAnimations(GameCenter.mainHero.heroBaseComponent.HeroID, cSVCollectionProcessData.collectingArms, null, EStateType.Collection);
                        CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectingEffect);
                        if (cSVEffectData != null)
                        {
                            float yOffset = 0f;
                            if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                            {
                                yOffset = 0.95f;
                            }
                            EffectUtil.Instance.LoadEffect(GameCenter.mainHero.uID, cSVEffectData.effects_path, GameCenter.mainHero.fxRoot.transform, EffectUtil.EEffectTag.Collection, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
                        }
                    });
                }
                else
                {
                    GameCenter.mainHero.animationComponent.Play(cSVCollectionProcessData.collectingAction);
                    CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectingEffect);
                    if (cSVEffectData != null)
                    {
                        float yOffset = 0f;
                        if (GameCenter.mainHero.heroBaseComponent.TitleId != 0)
                        {
                            yOffset = 0.95f;
                        }
                        EffectUtil.Instance.LoadEffect(GameCenter.mainHero.uID, cSVEffectData.effects_path, GameCenter.mainHero.fxRoot.transform, EffectUtil.EEffectTag.Collection, cSVEffectData.fx_duration / 1000f, 1, 1, ELayerMask.Default, yOffset);
                    }
                }
            }

            if (res.PickType == (uint)ECollectionSchemeType.Normal)
            {
                CollectionAction.eventEmitter.Trigger<CSVCollection.Data>(CollectionAction.EEvents.StartCollect, CollectionAction.CurrentCSVCollectionData);
                float allTime = CollectionAction.CurrentCSVCollectionData.collectionProgress / 1000f;
                collectTimer?.Cancel();
                collectTimer = Timer.Register(allTime, () =>
                {
                    if (Sys_Interactive.CurInteractiveNPC.uID != 0)
                    {
                        CmdNpcResourceEndReq endReq = new CmdNpcResourceEndReq();
                        endReq.UNpcId = Sys_Interactive.CurInteractiveNPC.uID;
                        NetClient.Instance.SendMessage((ushort)CmdNpc.ResourceEndReq, endReq);
                    }
                    else
                    {
                        eventEmitter.Trigger(EEvents.OnCollectFaild);
                    }
                }, null, false, true);
                DebugUtil.LogFormat(ELogType.eTask, "OnResourceBeginRes");
            }
            else
            {
                Sys_QTE.Instance.OpenQTE(UnlockCollectionAction.unlockID, () =>
                {
                    if (Sys_Interactive.CurInteractiveNPC.uID != 0)
                    {
                        CmdNpcResourceEndReq endReq = new CmdNpcResourceEndReq();
                        endReq.UNpcId = Sys_Interactive.CurInteractiveNPC.uID;
                        NetClient.Instance.SendMessage((ushort)CmdNpc.ResourceEndReq, endReq);
                    }
                    else
                    {
                        eventEmitter.Trigger(EEvents.OnCollectFaild);
                    }
                }, null, EQTESource.Collect);
            }
        }

        void OnResourceErrNtf(NetMsg msg)
        {
            CmdNpcResourceErrNtf ntf = NetMsgUtil.Deserialize<CmdNpcResourceErrNtf>(CmdNpcResourceErrNtf.Parser, msg);
            if (ntf != null)
            {
                eventEmitter.Trigger(EEvents.OnCollectFaild);
            }
        }

        private void OnResourceEndRes(NetMsg msg)
        {
            CmdNpcResourceEndRes res = NetMsgUtil.Deserialize<CmdNpcResourceEndRes>(CmdNpcResourceEndRes.Parser, msg);
            eventEmitter.Trigger<ulong>(EEvents.OnCollectEnded, res.UNpcId);

            if (CollectionAction.CurrentCSVCollectionData != null && CollectionAction.CurrentCSVCollectionData.CollectionTips != 0 && CSVLanguage.Instance.GetConfData(CollectionAction.CurrentCSVCollectionData.CollectionTips) != null)
            {
                Sys_Hint.Instance.PushContent_Normal(CSVLanguage.Instance.GetConfData(CollectionAction.CurrentCSVCollectionData.CollectionTips).words);
            }
            collectItemTimes[res.Group] = res.Count;

            //Npc npc = GameCenter.mainWorld.GetActor(typeof(Npc), res.UNpcId) as Npc;
            //if (npc != null)
            if (GameCenter.TryGetSceneNPC(res.UNpcId, out Npc npc))
            {
                eventEmitter.Trigger<uint>(EEvents.OnCollectSuccess, npc.cSVNpcData.id);

                CSVCollection.Data cSVCollectionData = CSVCollection.Instance.GetConfData(npc.cSVNpcData.id);
                if (cSVCollectionData.ICollectionType == 4)
                {
                    uint group = cSVCollectionData.CollectionGroup;
                    if (cSVCollectionData.CollectionGroup == 0)
                    {
                        group = cSVCollectionData.id;
                    }
                    if (res.Count >= cSVCollectionData.ICollectionNum)
                    {
                        List<ulong> deleteCollectionUID = new List<ulong>();

                        for (int i = 0; i < GameCenter.npcsList.Count; ++i)
                        {
                            Npc tempNpc = GameCenter.npcsList[i];
                            CSVCollection.Data npcCSVCollectionData = CSVCollection.Instance.GetConfData(tempNpc.cSVNpcData.id);
                            if (npcCSVCollectionData != null && npcCSVCollectionData.CollectionGroup == group)
                            {
                                deleteCollectionUID.Add(tempNpc.uID);
                            }
                        }

                        for (int index = 0, len = deleteCollectionUID.Count; index < len; index++)
                        {
                            NPCHelper.DeleteNPC(deleteCollectionUID[index]);
                        }
                        npc.AnimationComponent.CrossFade(CSVCollectionProcess.Instance.GetConfData(npc.cSVNpcData.id).collectObjectDeathAction, Constants.CORSSFADETIME, () =>
                        {
                            NPCHelper.DeleteNPC(npc.uID);
                        });
                    }
                }
            }

            
        }

        /// <summary>
        /// key: GroupID///
        /// Value: CollectedCount///
        /// </summary>
        public Dictionary<uint, uint> collectItemTimes = new Dictionary<uint, uint>();

        void OnCollectionNtf(NetMsg msg)
        {
            CmdNpcCollectionNtf ntf = NetMsgUtil.Deserialize<CmdNpcCollectionNtf>(CmdNpcCollectionNtf.Parser, msg);

            if (ntf != null)
            {
                collectItemTimes.Clear();
                foreach (var collectGroup in ntf.List.Items)
                {
                    collectItemTimes[collectGroup.Id] = collectGroup.Count;
                }
            }
        }

        public override void OnLogout()
        {
            collectItemTimes.Clear();

            base.OnLogout();
        }
    }
}