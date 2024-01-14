using Lib.Core;
using System;
using System.Collections.Generic;
using Table;

namespace Logic
{
    /// <summary>
    /// NPC交互表演节点：自身播放条件动作///
    /// Str: dance|10001&dance1&idle1|10002&dance2&idle2///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.PlayConditionAnimation)]
    public class WS_NPCBehaveAI_PlayConditionAnimation_SComponent : StateBaseComponent
    {
        WS_NPCControllerEntity entity;

        string[][] strs;
        CSVCheckseq.Data cSVCheckseq;
        List<List<string>> strLists = new List<List<string>>();

        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;

                strs = CombatHelp.GetStrParse2Array(str);

                strLists.Clear();
                foreach (var stringArr in strs)
                {
                    foreach (var value in stringArr)
                    {
                        List<string> strList = new List<string>();

                        string[] tempStrs = value.Split('&');
                        for (int j = 0, len = tempStrs.Length; j < len; j++)
                        {
                            strList.Add(tempStrs[j]);
                        }
                        strLists.Add(strList);
                    }
                }

                for (int index = 1, len = strLists.Count; index < len; index++)
                {
                    uint conditionGroupID = uint.Parse(strLists[index][0]);
                    cSVCheckseq = CSVCheckseq.Instance.GetConfData(conditionGroupID);
                    if (cSVCheckseq != null && cSVCheckseq.IsValid())
                    {
                        if (strLists[index][1] == entity.m_CurAnimationName || strLists[index][2] == entity.m_CurAnimationName)
                        {
                            m_CurUseEntity.TranstionMultiStates(this);
                            return;
                        }

                        entity.m_AnimationComponent.Stop(strLists[index][1]);
                        entity.m_AnimationComponent?.CrossFade(strLists[index][1], 0.1f, () =>
                        {
                            entity.m_AnimationComponent.Play(strLists[index][2]);
                            entity.m_CurAnimationName = strLists[index][2];
                            m_CurUseEntity?.TranstionMultiStates(this);
                        });
                        entity.m_CurAnimationName = strLists[index][1];

                        return;
                    }
                }

                if (strLists[0][0] == entity.m_CurAnimationName)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                entity.m_AnimationComponent.Stop(strLists[0][0]);
                entity.m_AnimationComponent?.CrossFade(strLists[0][0], 0.1f, () =>
                {
                    entity.m_AnimationComponent.Play((uint)EStateType.Idle);
                    entity.m_CurAnimationName = "action_idle";
                    m_CurUseEntity?.TranstionMultiStates(this);
                });
                entity.m_CurAnimationName = strLists[0][0];
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
            cSVCheckseq = null;
            strLists.Clear();

            base.Dispose();
        }
    }

    /// <summary>
    /// NPC交互表演节点：自身播放随机动作///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.PlayRandomAnimation)]
    public class WS_NPCBehaveAI_PlayRandomAnimation_SComponent : StateBaseComponent
    {
        string[] strs;
        WS_NPCControllerEntity entity;

        public override void Init(string str)
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;

                if (entity == null || entity.m_AnimationComponent == null)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                if (str == entity.m_CurAnimationName)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                strs = CombatHelp.GetStrParse1Array(str);

                entity.m_AnimationComponent.StopAll();
                entity.m_AnimationComponent?.CrossFade(strs[UnityEngine.Random.Range(0, strs.Length)], 0.1f, () =>
                {
                    if (entity != null)
                    {
                        entity.m_AnimationComponent.Play((uint)EStateType.Idle);
                        entity.m_CurAnimationName = string.Empty;
                        m_CurUseEntity?.TranstionMultiStates(this);
                    }
                });
                entity.m_CurAnimationName = str;
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_PlayConditionAnimation_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            entity = null;

            base.Dispose();
        }
    }

    /// <summary>
    /// NPC交互表演节点：自身播放动作///
    /// str: anim///
    /// </summary>
    [StateComponent((int)StateCategoryEnum.NPC, (int)NPCEnum.PlayAnimation)]
    public class WS_NPCBehaveAI_PlayAnimation_SComponent : StateBaseComponent
    {
        string[] strs;
        int dontPlayerIdleFlag;
        string amim;
        WS_NPCControllerEntity entity;

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
                amim = strs[0];
                if (strs.Length > 1)
                {
                    dontPlayerIdleFlag = int.Parse(strs[1]);
                }
                else
                {
                    dontPlayerIdleFlag = 0;
                }

                entity = (WS_NPCControllerEntity)m_CurUseEntity.m_StateControllerEntity;

                if (entity == null || entity.m_AnimationComponent == null)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                if (amim == entity.m_CurAnimationName)
                {
                    m_CurUseEntity.TranstionMultiStates(this);
                    return;
                }

                entity.m_AnimationComponent.Stop(amim);
                entity.m_AnimationComponent?.CrossFade(amim, 0.1f, () =>
                {
                    if (entity != null)
                    {
                        if (dontPlayerIdleFlag == 0)
                        {
                            entity.m_AnimationComponent.Play((uint)EStateType.Idle);
                        }
                        entity.m_CurAnimationName = string.Empty;
                        m_CurUseEntity?.TranstionMultiStates(this);
                    }
                });
                entity.m_CurAnimationName = amim;
            }
            catch (Exception e)
            {
                DebugUtil.LogError($"WS_NPCBehaveAI_PlayAnimation_SComponent: {e.ToString()}");
                m_CurUseEntity?.TranstionMultiStates(this);
            }
        }

        public override void Dispose()
        {
            strs = null;
            entity = null;
            amim = string.Empty;
            dontPlayerIdleFlag = 0;

            base.Dispose();
        }
    }
}
