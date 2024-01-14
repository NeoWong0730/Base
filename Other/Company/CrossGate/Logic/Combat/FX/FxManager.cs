using Logic;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

public class FxManager : Logic.Singleton<FxManager>
{
    public class FxRecycleClass
    {
        public ulong ModelId;
        public float DurTime;
    }

    public class FxExistLimitNumClass : BasePoolClass
    {
        public uint FxId;
        public List<ulong> ModelIdList;
        public int Count;

        public override void Clear()
        {
            FxId = 0u;
            if (ModelIdList != null)
                ModelIdList.Clear();
            Count = 0;
        }
    }

    private List<FxRecycleClass> _fxRecycleList;

    private List<FxExistLimitNumClass> _fxExistLimitNumList;

    public void OnAwake()
    {
        if(_fxRecycleList == null)
            _fxRecycleList = new List<FxRecycleClass>();
    }

    public void OnUpdate()
    {
        //处理自动回收特效
        if (_fxRecycleList.Count > 0)
        {
            for (int i = _fxRecycleList.Count - 1; i > -1; i--)
            {
                FxRecycleClass recycle = _fxRecycleList[i];
                if (Time.time >= recycle.DurTime)
                {
                    FreeFx(recycle.ModelId);

                    _fxRecycleList.RemoveAt(i);
                    CombatObjectPool.Instance.Push(recycle);
                }
            }
        }
    }

    public void Dispose()
    {
        if (null == _fxRecycleList)
            return;

        //处理播放完成的特效
        if (_fxRecycleList.Count > 0)
        {
            for (int i = _fxRecycleList.Count - 1; i > -1; i--)
            {
                FreeFx(_fxRecycleList[i].ModelId);

                CombatObjectPool.Instance.Push(_fxRecycleList[i]);
            }

            _fxRecycleList.Clear();
        }

        if (_fxExistLimitNumList != null && _fxExistLimitNumList.Count > 0)
        {
            for (int i = 0, count = _fxExistLimitNumList.Count; i < count; i++)
            {
                _fxExistLimitNumList[i].Push();
            }
            _fxExistLimitNumList.Clear();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fxId">特效表的Id</param>
    /// <param name="parent">父节点，如果特效表中特效是跟随且artGo不为空，该参数就没用</param>
    /// <param name="artGo">特效参照的世界坐标 或者 特效表中特效是跟随,那么该artGo为特效的父节点</param>
    /// <param name="binds">存储游戏物体的绑点容器</param>
    /// <param name="action">加载完特效进行的action</param>
    /// <param name="delayTime">延迟创建，默认<0是使用特效表中的延迟时间，其他为用户自定义的延迟时间</param>
    /// <param name="isNeedRecycle">是否托管给特效管理</param>
    /// <param name="durationTime">如果托管，=0的话就使用特效表中的持续时间，>0就直接使用该值作为持续时间</param>
    /// <returns>模型Id</returns>
    public ulong ShowFX(uint fxId, Transform parent = null, GameObject artGo = null, Dictionary<uint, Transform> binds = null, 
        Action<GameObject, ulong> action = null, float delayTime = -1f, bool isNeedRecycle = false, 
        float durationTime = 0f, bool isPreRegister = false, float fxScale = 1f, int styleState = -1)
    {
        return ShowFX(fxId, artGo == null ? Vector3.zero : artGo.transform.position, parent, artGo, binds, action, delayTime, isNeedRecycle, durationTime, isPreRegister, fxScale, styleState);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fxId">特效表的Id</param>
    /// <param name="pos">特效的世界坐标</param>
    /// <param name="parent">父节点，如果特效表中特效是跟随且artGo不为空，该参数就没用</param>
    /// <param name="artGo">特效参照的世界坐标 或者 特效表中特效是跟随,那么该artGo为特效的父节点</param>
    /// <param name="binds">存储游戏物体的绑点容器</param>
    /// <param name="action">加载完特效进行的action</param>
    /// <param name="delayTime">延迟创建，默认<0是使用特效表中的延迟时间，其他为用户自定义的延迟时间</param>
    /// <param name="isNeedRecycle">是否托管</param>
    /// <param name="durationTime">如果不托管，该参数没用；如果托管，=0的话就使用特效表中的持续时间，>0就直接使用该值作为持续时间</param>
    /// <param name="fxScale">对特效进行缩放</param>
    /// <returns>模型Id</returns>
    public ulong ShowFX(uint fxId, Vector3 pos, Transform parent = null, GameObject artGo = null, 
        Dictionary<uint, Transform> binds = null, Action<GameObject, ulong> action = null, float delayTime = -1f, 
        bool isNeedRecycle = false, float durationTime = 0f, bool isPreRegister = false, float fxScale = 1f,
        int styleState = -1)
    {
        if (fxId == 0u)
            return 0ul;

        CSVEffect.Data effectTb = CSVEffect.Instance.GetConfData(fxId);
        if(effectTb == null)
            return 0ul;

        FxExistLimitNumClass checkExistLimit = null;
        if (effectTb.max_effect > 0u)
        {
            if (_fxExistLimitNumList == null)
                _fxExistLimitNumList = new List<FxExistLimitNumClass>();
            if (_fxExistLimitNumList.Count > 0)
            {
                for (int i = 0, count = _fxExistLimitNumList.Count; i < count; i++)
                {
                    FxExistLimitNumClass fxExistLimitNumClass = _fxExistLimitNumList[i];
                    if (fxExistLimitNumClass == null)
                        continue;

                    if (fxExistLimitNumClass.FxId == fxId)
                    {
                        if (fxExistLimitNumClass.Count < effectTb.max_effect)
                        {
                            checkExistLimit = fxExistLimitNumClass;
                            break;
                        }
                        else
                            return 0ul;
                    }
                }
            }

            if (checkExistLimit == null)
            {
                checkExistLimit = BasePoolClass.Get<FxExistLimitNumClass>();
                checkExistLimit.FxId = fxId;
                checkExistLimit.Count = 1;

                _fxExistLimitNumList.Add(checkExistLimit);
            }
            else
                checkExistLimit.Count += 1;
        }

        ulong fxModelId = CombatModelManager.Instance.CreateModel(effectTb.effects_path, null, delegate(GameObject fxGo, ulong modelId) 
        {
            CombatManager.Instance.SetLayerByStyle(fxGo, styleState == -1 ? CombatManager.Instance.m_CombatStyleState : styleState);

            Transform fxTrans = fxGo.transform;

            if (fxScale != 1f)
            {
                bool isChangePssm = false;
                for (int i = 0, cpsCount = CombatModelManager.Instance.m_ChangeParticleSystemScaleModelList.Count; i < cpsCount; i++)
                {
                    if (CombatModelManager.Instance.m_ChangeParticleSystemScaleModelList[i] == fxGo)
                    {
                        isChangePssm = true;
                        break;
                    }
                }

                if (!isChangePssm)
                {
                    SetParticleSystemScaleModelHierarchy(fxTrans);
                    CombatModelManager.Instance.m_ChangeParticleSystemScaleModelList.Add(fxGo);
                }

                fxTrans.localScale = new Vector3(fxScale, fxScale, fxScale);
            }
            else
                fxTrans.localScale = Vector3.one;

            Vector3 offsetV3 = new Vector3(effectTb.position_offsetx * 0.001f, effectTb.position_offsety * 0.001f, effectTb.position_offsetz * 0.001f);

            Transform bind = null;
            if (binds != null && binds.Count > 0 && effectTb.tie_point > 0u && 
                    binds.TryGetValue(effectTb.tie_point, out Transform bindTrans) && bindTrans != null)
            {
                if (effectTb.isrotate == 0u)
                {
                    pos = bindTrans.position;
                }
                else
                {
                    bind = bindTrans;
                }
            }

            if (bind != null)
            {
                fxTrans.SetParent(bind);
                fxTrans.localPosition = offsetV3;
            }
            else if (artGo != null && effectTb.isrotate == 1u)
            {
                fxTrans.SetParent(artGo.transform);
                fxTrans.localPosition = offsetV3;
            }
            else if (parent != null)
            {
                fxTrans.SetParent(parent);
                fxTrans.position = pos + offsetV3;
            }
            else
            {
                if (fxTrans.parent != CombatManager.Instance.m_WorkStreamTrans)
                    fxTrans.SetParent(CombatManager.Instance.m_WorkStreamTrans);
                fxTrans.position = pos + offsetV3;
            }

            float localAngleY = 0f;
            if (effectTb.position_type_rotation != null)
            {
                for (int localAngleIndex = 0, localAngleCount = effectTb.position_type_rotation.Count; localAngleIndex < localAngleCount; localAngleIndex += 2)
                {
                    if (effectTb.position_type_rotation[localAngleIndex] == CombatManager.Instance.m_BattlePosType)
                    {
                        localAngleY = effectTb.position_type_rotation[localAngleIndex + 1] * 0.0001f;
                        break;
                    }
                }
            }
            
            fxTrans.localEulerAngles = new Vector3(effectTb.rotation_x * 0.0001f, effectTb.rotation_y * 0.0001f + localAngleY, effectTb.rotation_z * 0.0001f);

            if (isNeedRecycle)
            {
                FxRecycleClass fxRecycleStruct = CombatObjectPool.Instance.Get<FxRecycleClass>();
                fxRecycleStruct.ModelId = modelId;
                fxRecycleStruct.DurTime = durationTime == 0f ? effectTb.fx_duration * 0.001f + Time.time : durationTime + Time.time;

                _fxRecycleList.Add(fxRecycleStruct);
            }

            action?.Invoke(fxGo, modelId);
        }, delayTime < 0f ? effectTb.fx_starttime * 0.001f : delayTime, isPreRegister);

        if (checkExistLimit != null)
        {
            if (checkExistLimit.ModelIdList == null)
                checkExistLimit.ModelIdList = new List<ulong>();

#if DEBUG_MODE
            if (checkExistLimit.ModelIdList.Contains(fxModelId))
                Lib.Core.DebugUtil.LogError($"处理限制特效FxId:{fxId.ToString()}次数发现已经有fxModelId：{fxModelId.ToString()}");
#endif

            checkExistLimit.ModelIdList.Add(fxModelId);
        }

        return fxModelId;
    }

    public void FreeFx(ulong modelId, Transform freeParent = null)
    {
        if (modelId == 0ul)
            return;

        if (_fxExistLimitNumList != null && _fxExistLimitNumList.Count > 0)
        {
            for (int i = 0, count = _fxExistLimitNumList.Count; i < count; i++)
            {
                FxExistLimitNumClass fxExistLimitNumClass = _fxExistLimitNumList[i];
                if (fxExistLimitNumClass == null || fxExistLimitNumClass.ModelIdList == null)
                    continue;

                if (fxExistLimitNumClass.ModelIdList.Remove(modelId))
                {
                    --fxExistLimitNumClass.Count;

                    if(fxExistLimitNumClass.Count < 1)
                    {
                        _fxExistLimitNumList.RemoveAt(i);
                        fxExistLimitNumClass.Push();
                    }

                    break;
                }
            }
        }

        if (freeParent == null)
            freeParent = CombatManager.Instance.m_WorkStreamTrans;

        CombatModelManager.Instance.FreeModel(modelId, freeParent, true);
    }

    public void SetParticleSystemScaleModelHierarchy(Transform trans)
    {
        ParticleSystem particleSystem = trans.GetComponent<ParticleSystem>();
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.scalingMode = ParticleSystemScalingMode.Hierarchy;
        }

        for (int i = 0, count = trans.childCount; i < count; i++)
        {
            var child = trans.GetChild(i);
            if (child == null)
                continue;

            SetParticleSystemScaleModelHierarchy(child);
        }
    }
}
