using System.Collections.Generic;
using UnityEngine;

public class PlayGhostShadowComponent : AComponent, IUpdate
{
    public bool m_IsCreate;

    private float _time;
    private float _intervalTime;
    private float _lifeTime;
    
    private int _ghostStyle;

    private MobEntity _mobEntity;

    private List<GhostShadowData> _ghostShadowDatas;

    public void Init(float intervalTime, float lifeTime, int ghostStyle)
    {
        if (_ghostShadowDatas == null)
            _ghostShadowDatas = new List<GhostShadowData>();

        _intervalTime = intervalTime;
        _lifeTime = lifeTime;

        _ghostStyle = ghostStyle;
        
        _mobEntity = m_Entity as MobEntity;

        m_IsCreate = true;

        _time = 0f;
    }

    public override void Dispose()
    {
        m_IsCreate = false;

        _mobEntity = null;
        
        int count = _ghostShadowDatas.Count;
        if (count > 0)
        {
            var combatGhostShadowComponent = CombatManager.Instance.m_CombatEntity.GetNeedComponent<CombatGhostShadowComponent>();

            for (int i = 0; i < count; i++)
            {
                var gsd = _ghostShadowDatas[i];
                combatGhostShadowComponent.Push(gsd, gsd.m_IsSMR);
            }
            _ghostShadowDatas.Clear();
        }

        base.Dispose();
    }

    public void Update()
    {
        if (m_IsCreate)
        {
            if (Time.time > _time)
            {
                CreateGhostShadow();
                _time = Time.time + _intervalTime;
            }
        }

        var combatGhostShadowComponent = CombatManager.Instance.m_CombatEntity.GetNeedComponent<CombatGhostShadowComponent>();
        for (int i = _ghostShadowDatas.Count - 1; i > -1; i--)
        {
            GhostShadowData gsd = _ghostShadowDatas[i];
            
            if (Time.time > gsd.m_LifeTime)
            {
                _ghostShadowDatas.RemoveAt(i);
                combatGhostShadowComponent.Push(gsd, gsd.m_IsSMR);

                continue;
            }
            else if (_ghostStyle == 0)
            {
                if (gsd.m_MeshRenderer.material.HasProperty(ShaderController.Instance.ShaderToInt_BaseColor))
                {
                    Color c = gsd.m_MeshRenderer.material.GetColor(ShaderController.Instance.ShaderToInt_BaseColor);
                    c.a = (gsd.m_LifeTime - Time.time) / _lifeTime;
                    gsd.m_MeshRenderer.material.SetColor(ShaderController.Instance.ShaderToInt_BaseColor, c);
                }
                else if (gsd.m_MeshRenderer.material.HasProperty(ShaderController.Instance.ShaderToInt_TintColor))
                {
                    Color c = gsd.m_MeshRenderer.material.GetColor(ShaderController.Instance.ShaderToInt_TintColor);
                    c.a = (gsd.m_LifeTime - Time.time) / _lifeTime;
                    gsd.m_MeshRenderer.material.SetColor(ShaderController.Instance.ShaderToInt_TintColor, c);
                }
            }
        }
    }
    
    public void SetCreateState(bool isCreate)
    {
        m_IsCreate = isCreate;
    }

    public void CreateGhostShadow()
    {
        CombatGhostShadowComponent ghostShadowComp = CombatManager.Instance.m_CombatEntity.GetNeedComponent<CombatGhostShadowComponent>();

        for (int i = 0, length = _mobEntity.SkinnedMeshRenderers.Length; i < length; i++)
        {
            SkinnedMeshRenderer smr = _mobEntity.SkinnedMeshRenderers[i];

            GhostShadowData ghostShadowData = ghostShadowComp.Get(true);
            if (ghostShadowData == null)
                continue;

            _ghostShadowDatas.Add(ghostShadowData);

            ghostShadowData.m_GhostStyle = _ghostStyle;
            
            smr.BakeMesh(ghostShadowData.m_Mesh);

            ghostShadowData.m_LifeTime = Time.time + _lifeTime;
            Transform smrTrans = smr.transform;
            ghostShadowData.m_Trans.position = smrTrans.position;
            ghostShadowData.m_Trans.rotation = smrTrans.rotation;
            ghostShadowData.m_Trans.localScale = smrTrans.lossyScale;

            if (_ghostStyle == 0)
            {
                ghostShadowData.m_MeshRenderer.sharedMaterials = smr.sharedMaterials;

                ghostShadowData.m_MeshRenderer.material.SetInt(ShaderController.SrcBlendId, 5);
                ghostShadowData.m_MeshRenderer.material.SetInt(ShaderController.DstBlendId, 10);
                ghostShadowData.m_MeshRenderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                SetColor(ghostShadowData.m_MeshRenderer);
            }
            else
            {
                ghostShadowData.m_MeshRenderer.sharedMaterial = ShaderController.Instance.m_CustomGhostMat;
            }
        }

        for (int i = 0, length = _mobEntity.MeshFilters.Length; i < length; i++)
        {
            var mf = _mobEntity.MeshFilters[i];

            GhostShadowData ghostShadowData = ghostShadowComp.Get(false);
            if (ghostShadowData == null)
                continue;

            _ghostShadowDatas.Add(ghostShadowData);

            ghostShadowData.m_GhostStyle = _ghostStyle;

            ghostShadowData.m_LifeTime = Time.time + _lifeTime;
            Transform mfTrans = mf.transform;
            ghostShadowData.m_Trans.position = mfTrans.position;
            ghostShadowData.m_Trans.rotation = mfTrans.rotation;
            ghostShadowData.m_Trans.localScale = mfTrans.lossyScale;

            ghostShadowData.m_MeshFilter.sharedMesh = mf.sharedMesh;

            if (_ghostStyle == 0)
            {
                ghostShadowData.m_MeshRenderer.sharedMaterials = mf.GetComponent<MeshRenderer>().sharedMaterials;

                ghostShadowData.m_MeshRenderer.material.SetInt(ShaderController.SrcBlendId, 5);
                ghostShadowData.m_MeshRenderer.material.SetInt(ShaderController.DstBlendId, 10);
                ghostShadowData.m_MeshRenderer.material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            }
            else
            {
                ghostShadowData.m_MeshRenderer.sharedMaterial = ShaderController.Instance.m_CustomGhostMat;
            }
        }
    }

    public void SetColor(Renderer renderer)
    {
        MobCombatComponent mobCombatComponent = _mobEntity.GetComponent<MobCombatComponent>();
        if (mobCombatComponent == null || mobCombatComponent.m_DressDataList == null ||
            mobCombatComponent.m_DressDataList.Count <= 0)
        {
            if (renderer.HasPropertyBlock())
                renderer.SetPropertyBlock(ShaderController.Instance.MatPropertyBlock, 0);
            return;
        }

        bool isUseColor = false;
        for (int i = 0, count = mobCombatComponent.m_DressDataList.Count; i < count; i++)
        {
            var dressData = mobCombatComponent.m_DressDataList[i];
            if (dressData == null)
                continue;

            isUseColor = true;

            ShaderController.Instance.MatPropertyBlock.SetColor(Logic.Constants.kShaderTintIDs[(int)dressData.tintIndex], dressData.color);
        }

        if (isUseColor)
        {
            ShaderController.Instance.MatPropertyBlock.SetFloat(Logic.Constants.kUseTintColor, 1);

            renderer.SetPropertyBlock(ShaderController.Instance.MatPropertyBlock, 0);

            ShaderController.Instance.MatPropertyBlock.Clear();
        }
    }
}
