using Lib.Core;
using System;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：虚拟Actor播放特效///
    /// 0：UID///
    /// 1：EffectID///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.VirtualShowActorPlayEffect)]
    public class WS_NPCBehaveAI_VirtualShowActorPlayEffect_SComponent : StateBaseComponent
    {
        string[] strs;
        ulong uid;
        uint effectID;
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
                effectID = uint.Parse(strs[1]);

                CSVEffect.Data cSVEffectData = CSVEffect.Instance.GetConfData(effectID);
                if (cSVEffectData == null)
                {
                    DebugUtil.LogError($"CSVEffect.Data not found, id:{effectID}");
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                if (VirtualShowManager.Instance.TryGetValue(uid, out actor))
                {
                    EffectUtil.Instance.LoadEffect(uid, cSVEffectData.effects_path, actor.transform, EffectUtil.EEffectTag.InteractiveShow, cSVEffectData.fx_duration / 1000f);
                }
                else
                {
                    DebugUtil.LogError($"NodeID:{m_DataNodeId}, Can not found VirtualSceneActor uid:{uid}");
                }
                m_CurUseEntity.TranstionMultiStates(this);
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_VirtualShowActorPlayEffect_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            uid = 0ul;
            effectID = 0u;
            actor = null;

            base.Dispose();
        }
    }
}