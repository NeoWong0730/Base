using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：播放音效///
    /// 0：音效id///
    /// 1：音效时长///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.PlayAudio)]
    public class WS_NPCBehaveAI_PlayAudio_SComponent : StateBaseComponent
    {
        string[] strs;
        uint aduioID;
        float time;

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

                aduioID = uint.Parse(strs[0]);
                time = float.Parse(strs[1]);

                m_CurUseEntity.GetNeedComponent<PlayAudioComponent>().AddAudio(aduioID, time);

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
            aduioID = 0u;
            time = 0f;

            base.Dispose();
        }
    }
}
