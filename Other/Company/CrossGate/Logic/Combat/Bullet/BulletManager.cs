using Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class BulletManager : Logic.Singleton<BulletManager>
{
    private Dictionary<ulong, BulletEntity> _bulletDic;

    public void OnEnable()
    {
        if(_bulletDic == null)
            _bulletDic = new Dictionary<ulong, BulletEntity>();
    }

    public void Dispose()
    {
        if (_bulletDic != null)
        {
            foreach (var kv in _bulletDic)
            {
                FxManager.Instance.FreeFx(kv.Key, CombatManager.Instance.m_FreeBulletTrans);
                if (kv.Value != null)
                    kv.Value.Dispose();
            }
            _bulletDic.Clear();
        }
    }

    public void CreateBullet(uint bulletId, CSVActiveSkill.Data skillTb, TurnBehaveSkillInfo sourcesTurnBehaveSkillInfo,
        BehaveAIControllParam behaveAIControllParam, MobEntity attach = null, MobEntity target = null, int targetClientNum = -1, 
        Action<MobEntity, MobEntity, BulletEntity> hitAction = null, float delayTime = -1f, uint bulletWorkId = 0u, int isCanProcessTypeAfterHit = 0)
    {
        CSVBullet.Data bulletTb = CSVBullet.Instance.GetConfData(bulletId);
        if (bulletTb == null)
        {
            if (behaveAIControllParam != null)
                behaveAIControllParam.Push();
            return;
        }

        Vector3 pos = CombatManager.Instance.CombatSceneCenterPos;
        float offsetX = bulletTb.position_offsetx * 0.0001f;
        float offsetY = bulletTb.position_offsety * 0.0001f;
        float offsetZ = bulletTb.position_offsetz * 0.0001f;
        Vector3 offsetV3 = new Vector3(offsetX, offsetY, offsetZ);
        Dictionary<uint, Transform> binds = null;
        if (bulletTb.position == 1)
        {
            if (attach != null)
            {
                pos = attach.m_Trans.position;
                offsetV3 = offsetX * attach.m_Trans.right + offsetY * attach.m_Trans.up + offsetZ * attach.m_Trans.forward;
                binds = attach.m_BindGameObjectDic;
            }
        }
        else if (bulletTb.position == 2)
        {
            if (target != null)
            {
                pos = target.m_Trans.position;
                offsetV3 = offsetX * target.m_Trans.right + offsetY * target.m_Trans.up + offsetZ * target.m_Trans.forward;
                binds = target.m_BindGameObjectDic;
            }
            else if (targetClientNum > -1)
            {
                pos = MobManager.Instance.GetPosInClientNum(targetClientNum);
                offsetV3 = CombatManager.Instance.m_AdjustSceneViewAxiss[0] * offsetX + CombatManager.Instance.m_AdjustSceneViewAxiss[1] * offsetY + CombatManager.Instance.m_AdjustSceneViewAxiss[2] * offsetZ;
            }
        }
        else if (bulletTb.position == 3)
        {
            //water

        }

        if (bulletTb.tie_point != 1)
            binds = null;

        ulong bulletModelId = FxManager.Instance.ShowFX(bulletTb.effect_id, pos, CombatManager.Instance.m_UseBulletTrans, null, binds, delegate (GameObject fxGo, ulong modelId) 
        {
            fxGo.transform.position += offsetV3;

            if (_bulletDic.ContainsKey(modelId) && _bulletDic[modelId] != null)
            {
                _bulletDic[modelId].Dispose();
            }

            BulletEntity bulletEntity = EntityFactory.Create<BulletEntity>();
            bulletEntity.Init(modelId, skillTb, fxGo, bulletTb, attach, target, targetClientNum, 
                sourcesTurnBehaveSkillInfo, behaveAIControllParam, hitAction, bulletWorkId, isCanProcessTypeAfterHit);

            _bulletDic[modelId] = bulletEntity;
        }, delayTime);

        if (!_bulletDic.ContainsKey(bulletModelId))
        {
            _bulletDic.Add(bulletModelId, null);
        }
    }

    public bool FreeBullet(ulong bulletModelId)
    {
        BulletEntity bulletEntity;
        if (!_bulletDic.TryGetValue(bulletModelId, out bulletEntity))
            return false;

        _bulletDic.Remove(bulletModelId);

        FxManager.Instance.FreeFx(bulletModelId, CombatManager.Instance.m_FreeBulletTrans);

        if (bulletEntity != null)
            bulletEntity.Dispose();
        else
            return false;

        return true;
    }
}
