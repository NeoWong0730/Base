using Packet;
using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PlayEffect)]
public class WS_CombatBehaveAI_PlayEffect_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (string.IsNullOrEmpty(str))
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        
        //播放特效
        string[] strs = CombatHelp.GetStrParse1Array(str);

        WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
        var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

        WS_CombatBehaveAIDataComponent dataComponent = GetNeedComponent<WS_CombatBehaveAIDataComponent>();

        uint effectTbId = uint.Parse(strs[0]) ;
        float fxScale = 1f;
        if (strs.Length > 1 && !string.IsNullOrWhiteSpace(strs[1]))
            fxScale = float.Parse(strs[1]);

        int offsetType = int.MinValue;
        Vector3 pos = mob.m_Trans == null ? Vector3.zero : mob.m_Trans.position;
        if (strs.Length > 2)
        {
            offsetType = int.Parse(strs[2]);

            if (offsetType == -1 || offsetType == 0 || offsetType == 1)
                cbace.GetSceneCombatPos(mob, offsetType, 7u, ref pos);
            else if (offsetType == -2)
                cbace.GetAttackTypeTargetPos(ref pos);
        }

        var effectModelId = FxManager.Instance.ShowFX(effectTbId, pos, null, mob.m_Go, mob.m_BindGameObjectDic, delegate (GameObject fxGo, ulong modelId)
        {
            if (modelId != 0ul)
                dataComponent.AddEffect(modelId, effectTbId);

            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }, -1, false, 0, false, fxScale);

        if (effectModelId == 0ul)
        {
            m_CurUseEntity.TranstionMultiStates(this);
            return;
        }
        else
        {
            dataComponent.AddEffect(effectModelId, effectTbId);
        }
    }
}