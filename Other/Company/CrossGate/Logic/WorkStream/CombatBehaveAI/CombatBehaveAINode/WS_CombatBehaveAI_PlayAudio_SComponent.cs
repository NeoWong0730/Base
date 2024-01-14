using System.Collections.Generic;
using UnityEngine;

[StateComponent((int)StateCategoryEnum.CombatBehaveAI, (int)CombatBehaveAIEnum.PlayAudio)]
public class WS_CombatBehaveAI_PlayAudio_SComponent : StateBaseComponent
{
	public override void Init(string str)
	{
        if (!string.IsNullOrEmpty(str))
        {
            string[] vs = CombatHelp.GetStrParse1Array(str);
            
            if (!uint.TryParse(vs[0], out uint audioId))
            {
                WS_CombatBehaveAIControllerEntity cbace = (WS_CombatBehaveAIControllerEntity)m_CurUseEntity.m_StateControllerEntity;
                
                if (vs.Length < 3 || (int.Parse(vs[2]) == cbace.m_AttachType))
                {
                    var mob = (MobEntity)cbace.m_WorkStreamManagerEntity.Parent;

                    audioId = CombatManager.Instance.GetAudioId(mob.m_MobCombatComponent.m_BattleUnit.UnitType, 
                        mob.m_MobCombatComponent.m_BattleUnit.UnitInfoId,
                        mob.m_MobCombatComponent.m_WeaponId, vs[0]);
                }
                else if (int.Parse(vs[2]) == 0)
                {
                    if(cbace.m_BehaveAIControllParam != null && cbace.m_BehaveAIControllParam.SrcUnitId > 0u)
                    {
                        var attackMob = MobManager.Instance.GetMob(cbace.m_BehaveAIControllParam.SrcUnitId);
                        if (attackMob != null)
                        {
                            audioId = CombatManager.Instance.GetAudioId(attackMob.m_MobCombatComponent.m_BattleUnit.UnitType, 
                                attackMob.m_MobCombatComponent.m_BattleUnit.UnitInfoId,
                                    attackMob.m_MobCombatComponent.m_WeaponId, vs[0]);
                        }
                    }
                }
            }

            if (audioId > 0u)
            {
                if (m_CurUseEntity.m_ParentMachine == null)
                    m_CurUseEntity.GetNeedComponent<PlayAudioComponent>().AddAudio(audioId, float.Parse(vs[1]));
                else
                    m_CurUseEntity.m_ParentMachine.GetNeedComponent<PlayAudioComponent>().AddAudio(audioId, float.Parse(vs[1]));
            }
        }
        
        m_CurUseEntity.TranstionMultiStates(this);
	}
}

public class PlayAudioComponent : BaseComponent<StateMachineEntity>, IUpdate
{
    public class AudioData
    {
        public float Time;
        public uint AudioId;
    }

    private List<AudioData> _audioList;

    public void AddAudio(uint audioId, float totalTime)
    {
        if (_audioList == null)
            _audioList = new List<AudioData>();

        AudioData ad = CombatObjectPool.Instance.Get<AudioData>();
        ad.AudioId = audioId;
        ad.Time = Time.time + totalTime;

        _audioList.Add(ad);
    }

    public override void Dispose()
    {
        if (_audioList.Count > 0)
        {
            for (int i = 0; i < _audioList.Count; i++)
            {
                AudioData ad = _audioList[i];
                if (ad == null)
                    continue;

                CombatObjectPool.Instance.Push(ad);
            }
            _audioList.Clear();
        }

        base.Dispose();
    }

    public void Update()
    {
        for (int i = _audioList.Count - 1; i > -1; i--)
        {
            AudioData ad = _audioList[i];
            if (ad == null)
            {
                _audioList.RemoveAt(i);
                continue;
            }

            if (Time.time > ad.Time)
            {
                AudioUtil.PlayAudio(ad.AudioId);

                CombatObjectPool.Instance.Push(ad);
                _audioList.RemoveAt(i);
            }
        }

        if (_audioList.Count <= 0)
            Dispose();
    }
}