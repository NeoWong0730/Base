using Lib.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WS_CommunalAIManagerEntity : WorkStreamManagerEntity
{
    public GameObject m_Go;
    public Vector3 m_OriginPos;

    public List<ShaderControllerMaterialInfo> m_ShaderControllerMaterialInfoList;

    public override void Dispose()
    {
        DoClearData(true);
        
        base.Dispose();
    }

    public void DoClearData(bool isNeedDestroyMat = true)
    {
        if (m_Go != null)
        {
            m_Go.SetActive(false);
            m_Go = null;
        }

        DoDestroyMat(isNeedDestroyMat);
    }

    private void DoDestroyMat(bool isNeedDestroyMat = true)
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

    public static WS_CommunalAIManagerEntity Start(uint workId, GameObject go)
    {
        if (workId == 0u || go == null)
            return null;

        WS_CommunalAIManagerEntity me = CreateWorkStreamManagerEntity<WS_CommunalAIManagerEntity>(true);
        if (me.m_ShaderControllerMaterialInfoList != null && me.m_ShaderControllerMaterialInfoList.Count > 0)
        {
            DebugUtil.LogError($"WS_CommunalAIManagerEntity.Start从池中获取实列的m_ShaderControllerMaterialInfoList里面还有值");
            me.DoDestroyMat();
        }
        return me.StartWork(workId, go);
    }

    public WS_CommunalAIManagerEntity StartWork(uint workId, GameObject go)
    {
        if (Id == 0L)
        {
            DebugUtil.LogError($"执行WS_CommunalAIManagerEntity workId：{workId.ToString()}时WS_CommunalAIManagerEntity已被Dispose");
            return null;
        }

        if (workId == 0u || go == null)
        {
            Dispose();
            return null;
        }
        
        m_Go = go;
        m_OriginPos = go.transform.position;
        if (m_ShaderControllerMaterialInfoList == null)
            m_ShaderControllerMaterialInfoList = new List<ShaderControllerMaterialInfo>();
        if (m_ShaderControllerMaterialInfoList.Count < 1)
        {
            Renderer[] rs = go.GetComponentsInChildren<Renderer>();
            if (rs != null)
            {
                for (int i = 0, len = rs.Length; i < len; i++)
                {
                    var mats = rs[i].materials;
                    if (mats != null)
                    {
                        for (int matIndex = 0, matLen = mats.Length; matIndex < matLen; matIndex++)
                        {
                            var mat = mats[matIndex];
                            if (mat == null)
                                continue;

                            ShaderControllerMaterialInfo shaderControllerMaterialInfo = BasePoolClass.Get<ShaderControllerMaterialInfo>();
                            shaderControllerMaterialInfo.Mat = mat;
                            shaderControllerMaterialInfo.OriginRenderQueue = mat.renderQueue;
                            shaderControllerMaterialInfo.OriginOutlineState = mat.GetShaderPassEnabled(ShaderController.OutlinePassName);

                            m_ShaderControllerMaterialInfoList.Add(shaderControllerMaterialInfo);
                        }
                    }
                }
            }
        }

        return StartCommunalAI<WS_CommunalAIControllerEntity>(workId);
    }

    private WS_CommunalAIManagerEntity StartCommunalAI<T>(uint workId, int attachType = 0,
        SwitchWorkStreamEnum switchWorkStreamEnum = SwitchWorkStreamEnum.Stop_AllWorkStream,
        Action<StateControllerEntity> controllerBeginAction = null, Action controllerOverAction = null) where T : BaseStreamControllerEntity
    {
        if (StartController<T>(workId, attachType, switchWorkStreamEnum, controllerBeginAction, controllerOverAction))
            return this;
        else
        {
            Dispose();
            return null;
        }
    }
}