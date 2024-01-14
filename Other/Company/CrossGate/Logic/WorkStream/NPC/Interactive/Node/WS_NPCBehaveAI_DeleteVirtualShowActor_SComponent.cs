using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：删除指定虚拟Actor///
    /// Str: uid///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.DeleteVirtualShowActor)]
    public class WS_NPCBehaveAI_DeleteVirtualShowActor_SComponent : StateBaseComponent
    {
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

                uid = ulong.Parse(str);
                //if (VirtualShowManager.Instance.virtualSceneActors.TryGetValue(uid, out actor))
                //{
                //    GameCenter.mainWorld.DestroyActor(actor);
                //    VirtualShowManager.Instance.virtualSceneActors.Remove(uid);
                //}
                VirtualShowManager.Instance.Remove(uid);
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_DeleteVirtualShowActor_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            uid = 0ul;
            actor = null;

            base.Dispose();
        }
    }
}
