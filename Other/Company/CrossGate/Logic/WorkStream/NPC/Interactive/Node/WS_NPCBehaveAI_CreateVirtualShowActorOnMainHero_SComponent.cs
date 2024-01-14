using Lib.Core;
using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.AI;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：玩家位置生成虚拟Actor///
    /// 0: UID///
    /// 1: Type///
    /// 2: InfoID///
    /// 3: OffsetPosX///
    /// 4: OffsetPosY///
    /// 5: OffsetPosZ///
    /// 6: RotX///
    /// 7: RotY///
    /// 8: RotZ///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.CreateVirtualShowActorOnMainHero)]
    public class WS_NPCBehaveAI_CreateVirtualShowActorOnMainHero_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        uint type;
        uint infoID;
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

        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity?.TranstionMultiStates(this);
                    return;
                }

                strs = CombatHelp.GetStrParse1Array(str);

                uid = ulong.Parse(strs[0]);
                type = uint.Parse(strs[1]);
                infoID = uint.Parse(strs[2]);
                OffsetPosX = float.Parse(strs[3]);
                OffsetPosY = float.Parse(strs[4]);
                OffsetPosZ = float.Parse(strs[5]);
                if (strs.Length == 9)
                {
                    rotX = float.Parse(strs[6]);
                    rotY = float.Parse(strs[7]);
                    rotZ = float.Parse(strs[8]);
                }

                if (VirtualShowManager.Instance.ContainsKey(uid))
                {
                    //DebugUtil.LogError($"WS_NPCBehaveAI_CreateVirtualShowActorOnMainHero_SComponent alreay existed VirtualShowActor uid: {uid}");
                    VirtualShowManager.Instance.Remove(uid);
                }

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
                    //virtualSceneActor.movementComponent.TransformToPosImmediately(GameCenter.mainHero.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ));
                    NavMeshHit navMeshHit;
                    Vector3 hitPos = GameCenter.mainHero.transform.position + new Vector3(OffsetPosX, OffsetPosY, OffsetPosZ);
                    MovementComponent.GetNavMeshHit(hitPos, out navMeshHit);
                    if (navMeshHit.hit)
                        virtualSceneActor.transform.position = navMeshHit.position;
                    else
                        virtualSceneActor.transform.position = hitPos;

                    virtualSceneActor.movementComponent.InitNavMeshAgent();

                    if (strs.Length == 9)
                    {
                        virtualSceneActor.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
                    }
                    else
                    {
                        virtualSceneActor.transform.rotation = GameCenter.mainHero.transform.rotation;
                    }

                    virtualSceneActor.LoadModel(_modelPath, OnModelLoaded);
                }
                else
                {
                    DebugUtil.LogError($"WS_NPCBehaveAI_CreateVirtualShowActorOnMainHero_SComponent create VirtualShowActor failed uid = {uid} configID = {infoID} type = {type}");
                    m_CurUseEntity?.TranstionMultiStates(this);
                }
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_CreateVirtualShowActorOnMainHero_SComponent: {e.ToString()}");
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
            strs = null;
            uid = 0ul;
            type = infoID = 0u;
            OffsetPosX = OffsetPosY = OffsetPosZ = 0f;
            rotX = rotY = rotZ = 0f;
            _modelPath = string.Empty;
            virtualNpc = null;
            virtualParnter = null;
            virtualMonster = null;
            virtualSceneActor = null;

            base.Dispose();
        }
    }
}
