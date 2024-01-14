using Lib.Core;
using Logic;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WS_UIModelShowManagerEntity : WorkStreamManagerEntity
{
    [System.Flags]
    public enum ControlUIOperationEnum
    {
        OnSwitchModel = 1,      //这个不用处理
        OnRotateModel = 1 << 1,
        OnTouchModel = 1 << 2,
    }

    private Action<int, bool> _controlUIAction;

    private int _operationRecord;

    public Action m_TouchModelAction;

    public GameObject m_Go;
    public uint m_WeaponId;
    public GameObject m_WeaponGo;

    private VirtualGameObject _weapon02VGO;
    private GameObject _weapon02Go;
    private Transform _weapon02ParentTrans;

    public List<ShaderControllerMaterialInfo> m_ShaderControllerMaterialInfoList;

    public Transform m_WeaponParentTrans;
    public AnimationComponent m_AnimationComponent;
    public AnimationControl m_AnimationControl;
    public string m_CurPlayAanimtionName;

    public CP_BehaviourCollector m_BehaviourCollector;

    public override void Dispose()
    {
        DoClearData(true);

        base.Dispose();
    }

    public void DoClearData(bool isNeedDestroyMat = true)
    {
        try
        {
            if (_controlUIAction != null)
            {
                _controlUIAction = null;
            }

            m_TouchModelAction = null;

            m_Go = null;
            if (m_BehaviourCollector != null)
            {
                m_BehaviourCollector.Enable(true);
                m_BehaviourCollector = null;
            }
            m_WeaponId = 0u;
            if (m_WeaponGo != null)
            {
                if (m_WeaponParentTrans != null)
                {
                    m_WeaponGo.transform.SetParent(m_WeaponParentTrans);
                    m_WeaponParentTrans = null;
                }
                m_WeaponGo = null;
            }

            if (_weapon02VGO != null)
                _weapon02VGO = null;
            if (_weapon02Go != null)
            {
                if (_weapon02ParentTrans != null)
                {
                    _weapon02Go.transform.SetParent(_weapon02ParentTrans);
                    _weapon02ParentTrans = null;
                }
                _weapon02Go = null;
            }
            if (_weapon02ParentTrans != null)
                _weapon02ParentTrans = null;

            DoDestroyMat(isNeedDestroyMat);

            m_CurPlayAanimtionName = null;

            if (m_AnimationComponent != null)
            {
                m_AnimationComponent.StopAll();
                m_AnimationComponent = null;
            }
            if (m_AnimationControl != null)
            {
                m_AnimationControl.StopAll();
                m_AnimationControl = null;
            }
        }
        catch (Exception e)
        {
            Lib.Core.DebugUtil.LogError($"WS_UIModelShowManagerEntity  DoClearData  Exception:{e}");

            m_AnimationComponent = null;
            m_AnimationControl = null;
        }
    }

    private void DoDestroyMat(bool isNeedDestroyMat)
    {
        if (m_ShaderControllerMaterialInfoList != null)
        {
            for (int i = 0, len = m_ShaderControllerMaterialInfoList.Count; i < len; i++)
            {
                ShaderControllerMaterialInfo shaderControllerMaterialInfo = m_ShaderControllerMaterialInfoList[i];
                if (shaderControllerMaterialInfo == null || shaderControllerMaterialInfo.Mat == null)
                    continue;

                ShaderController.Instance.ShowDissolveBloom(shaderControllerMaterialInfo.Mat, false, 0f, 0f, 13, 
                    (RenderQueue)shaderControllerMaterialInfo.OriginRenderQueue, shaderControllerMaterialInfo.OriginOutlineState);

                if (isNeedDestroyMat)
                    shaderControllerMaterialInfo.Push();
            }
            if (isNeedDestroyMat)
                m_ShaderControllerMaterialInfoList.Clear();
        }
    }

    #region 示例
    private static WS_UIModelShowManagerEntity StartUIModelShow<T>(uint workId, int attachType = 0,
        SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
        Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null, bool isSelfDestroy = true) where T : BaseStreamControllerEntity
    {
        WS_UIModelShowManagerEntity me = CreateWorkStreamManagerEntity<WS_UIModelShowManagerEntity>(isSelfDestroy);
        if (me.StartController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction))
            return me;
        else
        {
            me.Dispose();
            return null;
        }
    }

    private static WS_UIModelShowManagerEntity StartUIModelShow<T>(WS_UIModelShowManagerEntity me, uint workId, int attachType = 0,
        SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
        Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null) where T : BaseStreamControllerEntity
    {
        if (me.StartController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction))
            return me;
        else
        {
            me.Dispose();
            return null;
        }
    }
    #endregion

    public static WS_UIModelShowManagerEntity Start(uint workId, AnimationComponent animationComponent, AnimationControl animationControl,
        GameObject go, uint weaponId = 0u, GameObject weaponGo = null, VirtualGameObject weapon02VGO = null, Action<int, bool> controlUIAction = null)
    {
        if (workId == 0u || (animationComponent == null && animationControl == null))
            return null;

        WS_UIModelShowManagerEntity me = CreateWorkStreamManagerEntity<WS_UIModelShowManagerEntity>(true);
        if (me.m_ShaderControllerMaterialInfoList != null && me.m_ShaderControllerMaterialInfoList.Count > 0)
        {
            DebugUtil.LogError($"WS_UIModelShowManagerEntity.Start从池中获取实列的m_ShaderControllerMaterialInfoList里面还有值");
            me.DoDestroyMat(true);
        }
        return me.StartWork(workId, animationComponent, animationControl, go, weaponId, weaponGo, weapon02VGO, controlUIAction);
    }

    public WS_UIModelShowManagerEntity StartWork(uint workId, AnimationComponent animationComponent, AnimationControl animationControl,
        GameObject go, uint weaponId = 0u, GameObject weaponGo = null, VirtualGameObject weapon02VGO = null, Action<int, bool> controlUIAction = null)
    {
        if (Id == 0L)
        {
            DebugUtil.LogError($"执行WS_UIModelShowManagerEntity workId：{workId.ToString()}时WS_UIModelShowManagerEntity已被Dispose");
            return null;
        }

        if (workId == 0u || (animationComponent == null && animationControl == null))
        {
            Dispose();
            return null;
        }

        _operationRecord = ~0;
        if (_controlUIAction != null)
        {
            DebugUtil.LogError($"执行WS_UIModelShowManagerEntity workId：{workId.ToString()}时事件controlUIAction未被注销");
        }
        _controlUIAction = controlUIAction;

        m_Go = go;
        m_WeaponId = weaponId;
        m_WeaponGo = weaponGo;
        if (weaponGo != null)
            m_WeaponParentTrans = weaponGo.transform.parent;

        _weapon02VGO = weapon02VGO;

#if UNITY_EDITOR
        if (m_Go != null)
        {
            Workstream_Test.AddWorkstream_Test(this, m_Go);
        }
#endif

        WS_UIModelShowManagerEntity wS_UIModelShowManagerEntity = StartUIModelShow<WS_UIModelShowControllerEntity>(this, workId);
        if (wS_UIModelShowManagerEntity != null)
        {
            wS_UIModelShowManagerEntity.m_AnimationComponent = animationComponent;
            wS_UIModelShowManagerEntity.m_AnimationControl = animationControl;
        }

        return wS_UIModelShowManagerEntity;
    }

    public void RecordControlUIOperation(ControlUIOperationEnum controlUIOperationEnum, bool isCan)
    {
        int cuo = (int)controlUIOperationEnum;
        _operationRecord = isCan ? (_operationRecord | cuo) : (_operationRecord & ~cuo);

        if (_controlUIAction != null)
            _controlUIAction.Invoke(cuo, isCan);
    }

    public bool IsCanControlUIOperation(ControlUIOperationEnum controlUIOperationEnum)
    {
        int cuo = (int)controlUIOperationEnum;
        return (_operationRecord & cuo) > 0;
    }

    public void TouchModelOperation()
    {
        if (m_TouchModelAction != null && IsCanControlUIOperation(ControlUIOperationEnum.OnTouchModel))
            m_TouchModelAction();
    }

    public bool SetMeshRendererMatArray(GameObject go, bool isForce = false)
    {
        if (!isForce && m_ShaderControllerMaterialInfoList != null && m_ShaderControllerMaterialInfoList.Count > 0)
            return false;

        Renderer[] rs = go.GetComponentsInChildren<Renderer>();
        if (rs == null || rs.Length < 1)
            return false;

        for (int mrIndex = 0, mrLen = rs.Length; mrIndex < mrLen; mrIndex++)
        {
            Renderer mr = rs[mrIndex];
            if (mr == null)
                continue;

            Material[] mats = mr.materials;
            if (mats == null)
                continue;

            foreach (var mat in mats)
            {
                if (mat == null)
                    continue;

                if (m_ShaderControllerMaterialInfoList == null)
                    m_ShaderControllerMaterialInfoList = new List<ShaderControllerMaterialInfo>();

                ShaderControllerMaterialInfo shaderControllerMaterialInfo = BasePoolClass.Get<ShaderControllerMaterialInfo>();
                shaderControllerMaterialInfo.Mat = mat;
                shaderControllerMaterialInfo.OriginRenderQueue = mat.renderQueue;
                shaderControllerMaterialInfo.OriginOutlineState = mat.GetShaderPassEnabled(ShaderController.OutlinePassName);

                m_ShaderControllerMaterialInfoList.Add(shaderControllerMaterialInfo);
            }
        }

        return true;
    }

    public void GetOtherWeapon(out GameObject weapon02Go, out Transform weapon02ParentTrans)
    {
        if (_weapon02Go != null)
        {
            weapon02Go = _weapon02Go;
            weapon02ParentTrans = _weapon02ParentTrans;

            return;
        }

        if (_weapon02VGO != null)
        {
            _weapon02Go = _weapon02VGO.gameObject;
            if (_weapon02Go != null)
                _weapon02ParentTrans = _weapon02Go.transform.parent;
        }

        weapon02Go = _weapon02Go;
        weapon02ParentTrans = _weapon02ParentTrans;
    }
}
