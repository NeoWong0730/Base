using DG.Tweening;
using Lib.Core;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：所有虚拟伙伴返回玩家///
    /// 0: time///
    /// 1: deleteFlag///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.AllVirtualShowParnterReturnToMainHero)]
    public class WS_NPCBehaveAI_AllVirtualShowParnterReturnToMainHero_SComponent : StateBaseComponent
    {
        string[] strs;
        float time;
        uint deleteFlag;
        Timer timer;
        List<ulong> parnterUIDs = new List<ulong>();

        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                parnterUIDs.Clear();
                strs = CombatHelp.GetStrParse1Array(str);
                time = float.Parse(strs[0]);
                if (strs.Length > 1)
                {
                    deleteFlag = uint.Parse(strs[1]);
                }

                foreach (var actor in VirtualShowManager.Instance.virtualSceneActors.Values)
                {
                    if (actor is VirtualParnter)
                    {
                        if (deleteFlag != 1)
                        {
                            parnterUIDs.Add(actor.uID);
                        }
                        actor.AnimationComponent.Play((uint)EStateType.Run);
                        actor.transform.DOLookAt(GameCenter.mainHero.transform.position, 0.1f, AxisConstraint.Y, Vector3.up).onComplete = delegate ()
                        {
                            actor.movementComponent?.MoveTo(GameCenter.mainHero.transform.position, null, null, () =>
                            {
                                actor.AnimationComponent.Play((uint)EStateType.Idle);
                                actor.gameObject.SetActive(false);

                                if (deleteFlag != 1)
                                {
                                    //GameCenter.mainWorld.DestroyActor(actor);
                                    Core.World.CollecActor(actor);
                                }

                                VirtualShowManager.Instance.virtualSceneActors.Remove(actor.uID);
                            });
                        };
                    }
                }

                timer?.Cancel();
                timer = Timer.Register(time, () =>
                {
                    m_CurUseEntity?.TranstionMultiStates(this);
                }, null, false, false);
            }
            catch (System.Exception e)
            {
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            time = 0f;
            timer?.Cancel();
            timer = null;
            deleteFlag = 0u;
            parnterUIDs.Clear();

            base.Dispose();
        }
    }  
}
