using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor播放动作///
    /// 0: UID///
    /// 1: Anim///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorPlayAnimation)]
    public class WS_NPCBehaveAI_VirtualShowActorPlayAnimation_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
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

                uid = ulong.Parse(strs[0]);

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    actor.AnimationComponent?.CrossFade(strs[1], 0.1f);
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                }
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_PlayAudio_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            uid = 0ul;
            actor = null;

            base.Dispose();
        }
    }
}