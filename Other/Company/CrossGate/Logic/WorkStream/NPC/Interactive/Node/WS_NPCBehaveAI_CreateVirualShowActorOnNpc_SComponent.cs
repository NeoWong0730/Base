using Lib.Core;
using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：指定NPC位置生成虚拟Actor///
    /// 0: UID///
    /// 1: Type///
    /// 2: InfoID///
    /// 3: TargetNpcInfoID///
    /// 4: OffsetPosX///
    /// 5: OffsetPosY///
    /// 6: OffsetPosZ///
    /// 7: RotX///
    /// 8: RotY///
    /// 9: RotZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.CreateVirualShowActorOnNpc)]
    public class WS_NPCBehaveAI_CreateVirualShowActorOnNpc_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        uint type;
        uint infoID;
        uint targetNpcInfoID;
        float OffsetPosX;
        float OffsetPosY;
        float OffsetPosZ;
        float rotX;
        float rotY;
        float rotZ;
        string _modelPath;
        VirtualNpc virtualNpc;
        VirtualParnter virtualParnter;
        VirtualMonster virtualMonster;
        VirtualSceneActor virtualSceneActor;
        Npc targetNpc;

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

                uid = ulong.Parse(strs[0]);
                type = uint.Parse(strs[1]);
                infoID = uint.Parse(strs[2]);
                targetNpcInfoID = uint.Parse(strs[3]);
                OffsetPosX = float.Parse(strs[4]);
                OffsetPosY = float.Parse(strs[5]);
                OffsetPosZ = float.Parse(strs[6]);
                if (strs.Length == 10)
                {
                    rotX = float.Parse(strs[7]);
                    rotY = float.Parse(strs[8]);
                    rotZ = float.Parse(strs[9]);
                }

                if (VirtualShowManager.Instance.ContainsKey(uid))
                {
                    DebugUtil.LogError($"WS_NPCBehaveAI_CreateVirualShowActorOnNpc_SComponent alreay existed VirtualShowActor uid: {uid}");
                    VirtualShowManager.Instance.Remove(uid);
                }

                if (GameCenter.uniqueNpcs.TryGetValue(targetNpcInfoID, out targetNpc))
                {
                    if (type == (uint)EDialogueActorType.NPC)
                    {
                        if (CSVNpc.Instance.TryGetValue(infoID, out CSVNpc.Data npcData)
                            && VirtualShowManager.Instance.TryCreateVirtualNpc(uid, npcData, out virtualNpc))
                        {
                            _modelPath = npcData.model;
                            virtualSceneActor = virtualNpc;
                        }
                    }
                    else if (type == (uint)EDialogueActorType.Parnter)
                    {
                        if (CSVPartner.Instance.TryGetValue(infoID, out CSVPartner.Data partnerData)
                            && VirtualShowManager.Instance.TryCreateVirtualParnter(uid, partnerData, out virtualParnter))
                        {
                            _modelPath = partnerData.model;
                            virtualSceneActor = virtualParnter;
                        }
                    }
                    else if (type == (uint)EDialogueActorType.Monster)
                    {
                        if (CSVMonster.Instance.TryGetValue(infoID, out CSVMonster.Data monsterData)
                            && VirtualShowManager.Instance.TryCreateVirtualMonster(uid, monsterData, out virtualMonster))
                        {
                            _modelPath = monsterData.model;
                            virtualSceneActor = virtualMonster;
                        }
                    }

                    if (virtualSceneActor != null)
                    {
                        //virtualSceneActor.movementComponent.TransformToPosImmediately(targetNpc.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ));
                        NavMeshHit navMeshHit;
                        Vector3 hitPos = targetNpc.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ);
                        MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
                        if (navMeshHit.hit)
                            virtualSceneActor.transform.position = navMeshHit.position;
                        else
                            virtualSceneActor.transform.position = hitPos;

                        virtualSceneActor.movementComponent.InitNavMeshAgent();

                        if (strs.Length == 10)
                        {
                            virtualSceneActor.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
                        }
                        else
                        {
                            virtualSceneActor.transform.rotation = targetNpc.transform.rotation;
                        }

                        virtualSceneActor.LoadModel(_modelPath, OnModelLoaded);
                    }
                    else
                    {
                        DebugUtil.LogError($"WS_NPCBehaveAI_CreateVirualShowActorOnNpc_SComponent create virtualSceneActor failed uid = {uid} configID = {infoID} type = {type}");
                        m_CurUseEntity?.TranstionMultiStates(this);
                    }
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found targetNpc id:{targetNpcInfoID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_CreateVirualShowActorOnNpc_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        private void OnModelLoaded(SceneActor actor)
        {
            if (actor != virtualSceneActor)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_CreateVirtualShowActorOnTargetPos_SComponent actor is not current virtualSceneActor");
                return;
            }

            if (virtualSceneActor != null)
            {                
                virtualSceneActor.AnimationComponent.SetSimpleAnimation(virtualSceneActor.modelTransform.GetChild(0).gameObject.GetNeedComponent<Framework.SimpleAnimation>());
                virtualSceneActor.AnimationComponent.UpdateHoldingAnimations(virtualSceneActor.ID, Constants.UMARMEDID, null, EStateType.Idle);
            }

            m_CurUseEntity?.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            uid = 0ul;
            type = infoID = targetNpcInfoID = 0u;
            OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            rotX = rotY = rotZ = 0f;
            _modelPath = string.Empty;
            virtualNpc = null;
            virtualParnter = null;
            virtualMonster = null;
            virtualSceneActor = null;
            targetNpc = null;

            base.Dispose();
        }
    }
}
