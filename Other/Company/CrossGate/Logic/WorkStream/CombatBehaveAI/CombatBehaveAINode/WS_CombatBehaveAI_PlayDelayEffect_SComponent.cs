using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PlayDelayEffect)]
public class WS_CombatBehaveAI_PlayDelayEffect_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (!string.IsNullOrEmpty(str))
        {
            WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;

            //播放特效
            string[] strs = CombatHelp.GetStrParse1Array(str);

            float fxScale = 1f;
            if (strs.Length > 2 && !string.IsNullOrWhiteSpace(strs[2]))
                fxScale = float.Parse(strs[2]);

            Vector3 pos;
            MobEntity mob = null;
            if (strs.Length > 3 && !string.IsNullOrWhiteSpace(strs[3]))
            {
                int clientNum = int.Parse(strs[3]);
                mob = MobManager.Instance.GetMobByClientNum(clientNum);
                if (mob == null)
                {
                    pos = Vector3.zero;

                    CombatPosData posData;
                    CombatManager.Instance.GetPosByClientNum(clientNum, out posData, ref pos);
                }
                else
                    pos = mob.m_Trans.position;
            }
            else
            {
                mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;
                pos = mob.m_Trans == null ? Vector3.zero : mob.m_Trans.position;
            }
            
            int offsetType = int.MinValue;
            if (strs.Length > 4)
            {
                offsetType = int.Parse(strs[4]);

                if (offsetType == -1 || offsetType == 0 || offsetType == 1)
                    cbace.GetSceneCombatPos(mob, offsetType, 7u, ref pos);
                else if (offsetType == -2)
                    cbace.GetAttackTypeTargetPos(ref pos);
            }

            var effectModelId = FxManager.Instance.ShowFX(uint.Parse(strs[0]), pos, null,
                                        mob == null ? null : mob.m_Go, mob == null ? null : mob.m_BindGameObjectDic, 
                                        null, -1, true, float.Parse(strs[1]), false, fxScale);
        }

        m_CurUseEntity.TranstionMultiStates(this);
	}
}