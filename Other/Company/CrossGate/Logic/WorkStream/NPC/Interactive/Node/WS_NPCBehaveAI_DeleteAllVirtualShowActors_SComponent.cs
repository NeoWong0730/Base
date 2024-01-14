using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：删除所有虚拟Actor///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.DeleteAllVirtualShowActors)]
    public class WS_NPCBehaveAI_DeleteAllVirtualShowActors_SComponent : StateBaseComponent
    {
        public override void Init(string str)
        {
            try
            {
                VirtualShowManager.Instance.ClearVirtualSceneActors();

                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_DeleteAllVirtualShowActors_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }
    }
}
