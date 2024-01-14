using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：等待时间///
    /// str: time///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.WaitTime)]
    public class WS_NPCBehaveAI_WaitTime_SComponent : StateBaseComponent, IUpdate
    {
        float time;
        float _endTime;

        public override void Init(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }

            time = float.Parse(str);
            _endTime = time + Time.time;
        }

        public void Update()
        {
            if (Time.time < _endTime)
                return;

            m_CurUseEntity.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            time = _endTime = 0f;

            base.Dispose();
        }
    }
}
