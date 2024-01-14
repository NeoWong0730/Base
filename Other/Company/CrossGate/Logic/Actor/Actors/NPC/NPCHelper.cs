using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Google.Protobuf.Collections;
using Lib.Core;
using Framework;
using UnityEngine.AI;

namespace Logic
{
#if false
    public static class NPCHelper
    {
        static WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

        public static void DeleteNPC(ulong uid)
        {
            //Npc npcActor = GameCenter.mainWorld.GetActor(Npc.Type, uid) as Npc;
            //if (npcActor == null)
            //    return;

            if (!GameCenter.TryGetSceneNPC(uid, out Npc npcActor))
                return;

            if (GameCenter.npcs.TryGetValue(npcActor.cSVNpcData.id, out Dictionary<ulong, Npc> pairs))
            {
                pairs.Remove(uid);

                if (pairs.Count == 0)
                {
                    GameCenter.npcs.Remove(npcActor.cSVNpcData.id);
                }
            }

            //通知系统
            if (GameCenter.mNPCAreaCheckSystem != null)
            {
                GameCenter.mNPCAreaCheckSystem.OnNpcRemove(npcActor);
            }

            GameCenter.npcsList.Remove(npcActor);
            GameCenter.npcsDic.Remove(uid);
            GameCenter.uniqueNpcs.Remove(npcActor.cSVNpcData.id);
            Npc.Destroy(ref npcActor);

            //if (GameCenter.npcs.Count == 0)
            //{
            //    GameCenter.npcs.Clear();
            //}
            //
            //for (int i = GameCenter.npcsList.Count - 1; i >= 0; i--)
            //{
            //    Npc npc = GameCenter.npcsList[i];
            //    if (npc.uID == uid)
            //    {
            //        GameCenter.npcsList.Remove(npc);
            //    }
            //}

            //GameCenter.mainWorld.DestroyActor(Npc.Type, uid);
        }

        public static void AddNPC(MapNpc mapNpc)
        {         
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(mapNpc.InfoId);
            if (cSVNpcData == null)
            {
                Debug.LogError($"npc typeid is null {mapNpc.InfoId}");
                return;
            }

            if (cSVNpcData.type == (uint)ENPCType.Collection)
            {
                CSVCollection.Data cSVCollectionData = CSVCollection.Instance.GetConfData(cSVNpcData.id);
                if (cSVCollectionData.ICollectionType == (uint)EICollectionLimitType.Forever)
                {
                    uint group = cSVCollectionData.CollectionGroup;
                    if (cSVCollectionData.CollectionGroup == 0)
                    {
                        group = cSVCollectionData.id;
                    }
                    if (Sys_CollectItem.Instance.collectItemTimes.ContainsKey(group) && Sys_CollectItem.Instance.collectItemTimes[group] >= cSVCollectionData.ICollectionNum)
                    {
                        return;
                    }
                }
            }
            else if (cSVNpcData.type == (uint)ENPCType.ActiveMonster && cSVNpcData.subtype == 3)
            {
                if (Sys_Npc.Instance.IsActivatedNpc(cSVNpcData.id))
                {
                    return;
                }
            }

            //if (GameCenter.mainWorld.GetActor(typeof(Npc), mapNpc.Uid) != null)                
            //{
            //    DeleteNPC(mapNpc.Uid);
            //}
            DeleteNPC(mapNpc.Uid);

            //Npc npc = GameCenter.mainWorld.CreateActor<Npc>(mapNpc.Uid);
            //npc.cSVNpcData = cSVNpcData;
            Npc npc = Npc.Create(mapNpc.Uid, cSVNpcData, GameCenter.mainWorld);

            Dictionary<ulong, Npc> pairs = null;
            if (!GameCenter.npcs.TryGetValue(mapNpc.InfoId, out pairs))
            {
                pairs = new Dictionary<ulong, Npc>();
            }
            pairs[mapNpc.Uid] = npc;
            GameCenter.npcs[mapNpc.InfoId] = pairs;
            GameCenter.npcsList.Add(npc);
            GameCenter.npcsDic.Add(mapNpc.Uid, npc);

            ///不是采集物则填充唯一NPC集合
            ///不确定明怪需不需要填充
            if (cSVNpcData.type != (uint)ENPCType.Collection || cSVNpcData.type != (uint)ENPCType.ActiveMonster)
            {
                GameCenter.uniqueNpcs[npc.cSVNpcData.id] = npc;
            }

            npc.transform.position = PosConvertUtil.Svr2Client(mapNpc.PosX, mapNpc.PosY);
            npc.transform.rotation = Quaternion.Euler(0f, mapNpc.RotationY, 0f);
            npc.movementComponent.TransformToPosImmediately(npc.transform.position);

            GameCenter.mNPCHUDSystem.AddNewNpc(npc);
        }

        public static IEnumerator CreateNpcModelIEs(RepeatedField<MapNpc> mapNpcs)
        {
            //for (int index = 0, len = mapNpcs.Count; index < len; index++)
            //{
            //    if (GameCenter.npcs.ContainsKey(mapNpcs[index].InfoId) && GameCenter.npcs[mapNpcs[index].InfoId].ContainsKey(mapNpcs[index].Uid))
            //    {
            //        Npc npc = GameCenter.npcs[mapNpcs[index].InfoId][mapNpcs[index].Uid];
            //        npc.transform.position = PosConvertUtil.Svr2Client(mapNpcs[index].PosX, mapNpcs[index].PosY);
            //        npc.movementComponent = World.AddComponent<MovementComponent>(npc);
            //        npc.movementComponent.TransformToPosImmediately(npc.transform.position);
            //        npc.gameObject.transform.rotation = Quaternion.Euler(0f, mapNpcs[index].RotationY, 0f);
            //        npc.NPCFunctionComponent = World.AddComponent<NPCFunctionComponent>(npc);
            //
            //        //TODO 调度优化 NearbyNpcComponent 先去掉 由NearbyNpcSystem执行
            //        //npc.NearNpcComponent = World.AddComponent<NearbyNpcComponent>(npc);
            //        npc.ActiveListenerComponent = World.AddComponent<NpcActiveListenerComponent>(npc);                
            //    }
            //}

            for (int index = 0, len = mapNpcs.Count; index < len; index++)
            {                
                if (GameCenter.npcs.ContainsKey(mapNpcs[index].InfoId) && GameCenter.npcs[mapNpcs[index].InfoId].ContainsKey(mapNpcs[index].Uid))
                {
                    Npc npc = GameCenter.npcs[mapNpcs[index].InfoId][mapNpcs[index].Uid];
                    CreateNpcModel(npc, npc.cSVNpcData, mapNpcs[index]);
                }
                yield return waitForSeconds;
            }
        }

        static void CreateNpcModel(Npc npc, CSVNpc.Data cSVNpcData, MapNpc mapNpc)
        {
            npc.LoadModel(cSVNpcData.model, (actor) =>
            {
                npc.AnimationComponent.SetSimpleAnimation(npc.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
                AddComponents(npc, mapNpc);
            });
        }

        static void AddComponents(Npc npc, MapNpc mapNpc)
        {
            //if (World.GetComponent<AnimationComponent>(npc) != null)
            //    return;
            //if (npc.AnimationComponent != null)
            //    return;

            //npc.AnimationComponent = World.AddComponent<AnimationComponent>(npc);
            npc.AnimationComponent.UpdateHoldingAnimations(npc.cSVNpcData.action_id, Constants.UMARMEDID, null, EStateType.Idle);

            //ClickComponent clickComponent = npc.clickComponent = World.AddComponent<ClickComponent>(npc);
            ClickComponent clickComponent = npc.clickComponent;
            clickComponent.InteractiveAimType = EInteractiveAimType.NPCFunction;
            clickComponent.LayMask = ELayerMask.NPC;

            //if (npc.cSVNpcData.nameShow != 1)
            //{
            //    npc.NPCHUDComponent = World.AddComponent<NPCHUDComponent>(npc);
            //}

            if (npc.cSVNpcData.type == (uint)ENPCType.WorldBoss)
            {
                World.AddComponent<WorldBossComponent>(npc).BattleID = mapNpc.BattleId;
                World.AddComponent<WorldBossListenerComponent>(npc);
            }

            //if (npc.cSVNpcData.TriggerPerformRange != 0)
            //{
            //    World.AddComponent<NPCActionListenerComponent>(npc);
            //}
#if false
            if (npc.cSVNpcData.type == (uint)ENPCType.ActiveMonster)
            {
                npc.nPCAreaCheckComponent = World.AddComponent<NPCAreaCheckComponent>(npc);
                //npc.activeMonsterComponent = World.AddComponent<ActiveMonsterComponent>(npc);
            }
            else if (npc.cSVNpcData.type == (uint)ENPCType.EventNPC)
            {
                npc.nPCAreaCheckComponent = World.AddComponent<NPCAreaCheckComponent>(npc);
            } 
            else if (npc.cSVNpcData.type == (uint)ENPCType.LittleKnow)
            {
                uint knowledgeId = npc.cSVNpcData.subtype;

                if (!Sys_Knowledge.Instance.IsKnowledgeActive(knowledgeId))
                {
                    World.AddComponent<LittleDistanceCheckComponent>(npc).knowid = knowledgeId;
                }
            }
            else if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                //World.AddComponent<CollectionComponent>(npc).AddToCtrlList();             
                npc.collectionComponent = World.AddComponent<CollectionComponent>(npc);
                npc.collectionComponent.AddToCtrlList();
            }

            if (npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
            {
                npc.nPCAreaCheckComponent = World.AddComponent<NPCAreaCheckComponent>(npc);
                //npc.inquiryDistanceCheckComponent = World.AddComponent<InquiryDistanceCheckComponent>(npc);
            }
#else
            if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                //World.AddComponent<CollectionComponent>(npc).AddToCtrlList();             
                npc.collectionComponent = World.AddComponent<CollectionComponent>(npc);
                npc.collectionComponent.AddToCtrlList();
            }
#endif

            int stage;
            if (Sys_NPCFavorability.Instance.TryGetNpcStage(npc.cSVNpcData.id, out stage))
            {
                Sys_HUD.Instance.eventEmitter.Trigger<UpdateFavirabilityEvt>(Sys_HUD.EEvents.OnUpdateFavirability, new UpdateFavirabilityEvt()
                {
                    npcId = npc.uID,
                    val = (uint)stage,
                });
            }

            //npc.VisualComponent = World.AddComponent<VisualComponent>(npc);
            npc.VisualComponent.Checking();

            //npc.NPCHUDComponent?.UpdateStateFlag();
            GameCenter.mNPCHUDSystem.UpdateStateFlag(npc);
            
            if (GameCenter.uniqueNpcs.ContainsKey(npc.cSVNpcData.id))
            {
                if (npc.cSVNpcData.behaviorid != 0)
                {
                    npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(npc.cSVNpcData.behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, true, (int)NPCEnum.B_BeCreate, npc.uID);
                }
                else
                {
                    npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(1, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, true, (int)NPCEnum.B_BeCreate, npc.uID);
                }
            }

            if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectLock);

                if (npc.VisualComponent.Visiable)
                {
                    CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(npc.cSVNpcData.id);
                    if (cSVCollectionProcessData != null)
                    {
                        CSVEffect.Data collectBirthEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectionBirthEffect);
                        if (collectBirthEffectData != null)
                        {
                            EffectUtil.Instance.LoadEffect(npc.uID, collectBirthEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.CollectBirth, collectBirthEffectData.fx_duration / 1000f);
                            Sys_CollectItem.Instance.eventEmitter.Trigger<ulong>(Sys_CollectItem.EEvents.CollectBirth, npc.uID);
                        }
                    }
                }
            }
        }
    }
#else
    public static class NPCHelper
    {
        static WaitForSeconds waitForSeconds = new WaitForSeconds(0.1f);

        public static void DeleteNPC(ulong uid)
        {            
            if (!GameCenter.TryGetSceneNPC(uid, out Npc npcActor))
                return;

            if (GameCenter.npcs.TryGetValue(npcActor.cSVNpcData.id, out Dictionary<ulong, Npc> pairs))
            {
                pairs.Remove(uid);

                if (pairs.Count == 0)
                {
                    GameCenter.npcs.Remove(npcActor.cSVNpcData.id);
                }
            }

            //通知系统
            if (GameCenter.mNPCAreaCheckSystem != null)
            {
                GameCenter.mNPCAreaCheckSystem.OnNpcRemove(npcActor);
            }

            GameCenter.npcsList.Remove(npcActor);
            GameCenter.npcsDic.Remove(uid);
            GameCenter.uniqueNpcs.Remove(npcActor.cSVNpcData.id);
            
            if(npcActor.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                //CollectionNpc collectionNpc = npcActor as CollectionNpc;
                //CollectionNpc.Destroy(ref collectionNpc);                
            }
            else if (npcActor.cSVNpcData.type == (uint)ENPCType.WorldBoss)
            {
                WorldBossNpc worldBoss = npcActor as WorldBossNpc;
                GameCenter.worldBossList.Remove(worldBoss);

                //WorldBossNpc.Destroy(ref worldBoss);                
            }
            else
            {
                //Npc.Destroy(ref npcActor);                
            }

            World.CollecActor(ref npcActor);
        }

        public static void AddNPC(MapNpc mapNpc)
        {
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(mapNpc.InfoId);
            if (cSVNpcData == null)
            {
                Debug.LogError($"npc typeid is null {mapNpc.InfoId}");
                return;
            }

            if (cSVNpcData.type == (uint)ENPCType.Collection)
            {
                CSVCollection.Data cSVCollectionData = CSVCollection.Instance.GetConfData(cSVNpcData.id);
                if (cSVCollectionData.ICollectionType == (uint)EICollectionLimitType.Forever)
                {
                    uint group = cSVCollectionData.CollectionGroup;
                    if (cSVCollectionData.CollectionGroup == 0)
                    {
                        group = cSVCollectionData.id;
                    }
                    if (Sys_CollectItem.Instance.collectItemTimes.ContainsKey(group) && Sys_CollectItem.Instance.collectItemTimes[group] >= cSVCollectionData.ICollectionNum)
                    {
                        return;
                    }
                }
            }
            else if (cSVNpcData.type == (uint)ENPCType.ActiveMonster && cSVNpcData.subtype == 3)
            {
                if (Sys_Npc.Instance.IsActivatedNpc(cSVNpcData.id))
                {
                    return;
                }
            }

            DeleteNPC(mapNpc.Uid);

            Npc npc;
            if (cSVNpcData.type == (uint)ENPCType.Collection)
            {
                //npc = CollectionNpc.Create(mapNpc.Uid, cSVNpcData, GameCenter.mainWorld);
                npc = World.AllocActor<CollectionNpc>(mapNpc.Uid, (actor) => {
                    actor._csvData = cSVNpcData;
                    actor.InitRigData();
                });
            }
            else if(cSVNpcData.type == (uint)ENPCType.WorldBoss)
            {
                //WorldBossNpc worldBoss = WorldBossNpc.Create(mapNpc.Uid, cSVNpcData, GameCenter.mainWorld);

                WorldBossNpc worldBoss = World.AllocActor<WorldBossNpc>(mapNpc.Uid, (actor) => {
                    actor._csvData = cSVNpcData;
                    actor.InitRigData();
                });
                GameCenter.worldBossList.Add(worldBoss);

                npc = worldBoss;
            }
            else
            {
                //npc = Npc.Create(mapNpc.Uid, cSVNpcData, GameCenter.mainWorld);
                npc = World.AllocActor<Npc>(mapNpc.Uid, (actor) =>
                {
                    actor._csvData = cSVNpcData;
                    actor.InitRigData();
                });
            }

            //设置节点名称
            if (cSVNpcData.type > 0 && cSVNpcData.type < 9)
            {
                npc.SetName($"{((ENPCType)cSVNpcData.type).ToString()}_{cSVNpcData.id.ToString()}_{mapNpc.Uid.ToString()}");
            }
            else
            {
                npc.SetName($"Npc_{cSVNpcData.id.ToString()}_{mapNpc.Uid.ToString()}");
            }
            //设置父节点
            npc.SetParent(GameCenter.npcRoot.transform);

            Dictionary<ulong, Npc> pairs = null;
            if (!GameCenter.npcs.TryGetValue(mapNpc.InfoId, out pairs))
            {
                pairs = new Dictionary<ulong, Npc>();
            }
            pairs[mapNpc.Uid] = npc;
            GameCenter.npcs[mapNpc.InfoId] = pairs;
            GameCenter.npcsList.Add(npc);
            GameCenter.npcsDic.Add(mapNpc.Uid, npc);            

            ///不是采集物则填充唯一NPC集合
            ///不确定明怪需不需要填充
            if (cSVNpcData.type != (uint)ENPCType.Collection || cSVNpcData.type != (uint)ENPCType.ActiveMonster)
            {
                GameCenter.uniqueNpcs[npc.cSVNpcData.id] = npc;
            }

            npc.transform.position = PosConvertUtil.Svr2Client(mapNpc.PosX, mapNpc.PosY);
            npc.transform.rotation = Quaternion.Euler(0f, mapNpc.RotationY, 0f);
            //npc.movementComponent.TransformToPosImmediately(npc.transform.position);

            NavMeshHit navMeshHit;
            Vector3 hitPos = npc.transform.position;
            MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
            if (navMeshHit.hit)
                npc.transform.position = navMeshHit.position;
            else
                npc.transform.position = hitPos;

            npc.movementComponent.InitNavMeshAgent();


            GameCenter.mNPCHUDSystem.AddNewNpc(npc);
        }

        public static IEnumerator CreateNpcModelIEs(RepeatedField<MapNpc> mapNpcs)
        {
            //for (int index = 0, len = mapNpcs.Count; index < len; index++)
            //{
            //    if (GameCenter.npcs.ContainsKey(mapNpcs[index].InfoId) && GameCenter.npcs[mapNpcs[index].InfoId].ContainsKey(mapNpcs[index].Uid))
            //    {
            //        Npc npc = GameCenter.npcs[mapNpcs[index].InfoId][mapNpcs[index].Uid];
            //        npc.transform.position = PosConvertUtil.Svr2Client(mapNpcs[index].PosX, mapNpcs[index].PosY);
            //        npc.movementComponent = World.AddComponent<MovementComponent>(npc);
            //        npc.movementComponent.TransformToPosImmediately(npc.transform.position);
            //        npc.gameObject.transform.rotation = Quaternion.Euler(0f, mapNpcs[index].RotationY, 0f);
            //        npc.NPCFunctionComponent = World.AddComponent<NPCFunctionComponent>(npc);
            //
            //        //TODO 调度优化 NearbyNpcComponent 先去掉 由NearbyNpcSystem执行
            //        //npc.NearNpcComponent = World.AddComponent<NearbyNpcComponent>(npc);
            //        npc.ActiveListenerComponent = World.AddComponent<NpcActiveListenerComponent>(npc);                
            //    }
            //}

            for (int index = 0, len = mapNpcs.Count; index < len; index++)
            {
                if (GameCenter.npcs.ContainsKey(mapNpcs[index].InfoId) && GameCenter.npcs[mapNpcs[index].InfoId].ContainsKey(mapNpcs[index].Uid))
                {
                    Npc npc = GameCenter.npcs[mapNpcs[index].InfoId][mapNpcs[index].Uid];
                    CreateNpcModel(npc, npc.cSVNpcData, mapNpcs[index]);
                }
                yield return waitForSeconds;
            }
        }

        static void CreateNpcModel(Npc npc, CSVNpc.Data cSVNpcData, MapNpc mapNpc)
        {
            npc.LoadModel(cSVNpcData.model, (actor) =>
            {
                AddComponents(npc, mapNpc);
            });
        }

        static void AddComponents(Npc npc, MapNpc mapNpc)
        {
            npc.AnimationComponent.SetSimpleAnimation(npc.modelTransform.GetChild(0).gameObject.GetNeedComponent<SimpleAnimation>());
            npc.AnimationComponent.UpdateHoldingAnimations(npc.cSVNpcData.action_id, Constants.UMARMEDID, null, EStateType.Idle);

            ClickComponent clickComponent = npc.clickComponent;
            clickComponent.InteractiveAimType = EInteractiveAimType.NPCFunction;
            clickComponent.LayMask = ELayerMask.NPC;

            if (npc.cSVNpcData.type == (uint)ENPCType.WorldBoss)
            {
                (npc as WorldBossNpc).worldBossComponent.BattleID = mapNpc.BattleId;
            }
            if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                (npc as CollectionNpc).collectionComponent.AddToCtrlList();
            }

            int stage;
            if (Sys_NPCFavorability.Instance.TryGetNpcStage(npc.cSVNpcData.id, out stage))
            {
                Sys_HUD.Instance.eventEmitter.Trigger<UpdateFavirabilityEvt>(Sys_HUD.EEvents.OnUpdateFavirability, new UpdateFavirabilityEvt()
                {
                    npcId = npc.uID,
                    val = (uint)stage,
                });
            }

            npc.VisualComponent.Checking();

            GameCenter.mNPCHUDSystem.UpdateStateFlag(npc);

            if (GameCenter.uniqueNpcs.ContainsKey(npc.cSVNpcData.id))
            {
                if (npc.cSVNpcData.behaviorid != 0)
                {
                    npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(npc.cSVNpcData.behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, true, (int)NPCEnum.B_BeCreate, npc.uID);
                }
                else
                {
                    npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(1, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, null, true, (int)NPCEnum.B_BeCreate, npc.uID);
                }
            }

            if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                EffectUtil.Instance.UnloadEffectByTag(npc.uID, EffectUtil.EEffectTag.CollectLock);

                if (npc.VisualComponent.Visiable)
                {
                    CSVCollectionProcess.Data cSVCollectionProcessData = CSVCollectionProcess.Instance.GetConfData(npc.cSVNpcData.id);
                    if (cSVCollectionProcessData != null)
                    {
                        CSVEffect.Data collectBirthEffectData = CSVEffect.Instance.GetConfData(cSVCollectionProcessData.collectionBirthEffect);
                        if (collectBirthEffectData != null)
                        {
                            EffectUtil.Instance.LoadEffect(npc.uID, collectBirthEffectData.effects_path, npc.fxRoot.transform, EffectUtil.EEffectTag.CollectBirth, collectBirthEffectData.fx_duration / 1000f);
                            Sys_CollectItem.Instance.eventEmitter.Trigger<ulong>(Sys_CollectItem.EEvents.CollectBirth, npc.uID);
                        }
                    }
                }
            }
        }
    }
#endif
}