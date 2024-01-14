using UnityEngine;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：等待随机时间///
    /// 0：时间下限///
    /// 1：时间上限///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.WaitRandomTime)]
    public class WS_NPCBehaveAI_WaitRandomTime_SComponent : StateBaseComponent, IUpdate
    {
        string[] strs;
        float minTime;
        float maxTime;
        float _endTime;

        public override void Init(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                m_CurUseEntity.TranstionMultiStates(this);
                return;
            }

            strs = CombatHelp.GetStrParse1Array(str);
            minTime = float.Parse(strs[0]);
            maxTime = float.Parse(strs[1]);
            _endTime = Random.Range(minTime, maxTime) + Time.time;
        }

        public void Update()
        {
            if (Time.time < _endTime)
                return;

            m_CurUseEntity.TranstionMultiStates(this);
        }

        public override void Dispose()
        {
            strs = null;
            minTime = maxTime = _endTime = 0f;

            base.Dispose();
        }
    }
}