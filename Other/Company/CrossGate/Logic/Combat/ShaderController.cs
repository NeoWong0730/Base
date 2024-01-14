using Framework;
using Lib.Core;
using Logic;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;

//#if USE_ADDRESSABLE_ASSET
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
//#else
//using Lib.AssetLoader;
//#endif

//Gameobject被销毁时，删除实例化材质
public class DestroyMaterials_Evt
{
    public GameObject Go;
}

public class ShaderControllerMaterialInfo : BasePoolClass
{
    public Material Mat;
    public int OriginRenderQueue;
    public bool OriginOutlineState;

    public override void Clear()
    {
        if (Mat != null)
        {
            UnityEngine.Object.DestroyImmediate(Mat);
            Mat = null;
        }
        OriginRenderQueue = 0;
        OriginOutlineState = false;
    }
}

public class ShaderController : Disposer, IUpdate
{
    public static readonly string ShortTimeExistExetendStr = "_SHORT_TIME_EXIST_EXTEND";
    public static readonly string RenderTypeStr = "RenderType";
    public static readonly string OpaqueStr = "Opaque";
    public static readonly string TransparentStr = "Transparent";
    public static readonly int SrcBlendId = Shader.PropertyToID("_SrcBlend");
    public static readonly int DstBlendId = Shader.PropertyToID("_DstBlend");
    public static readonly int ZWriteId = Shader.PropertyToID("_ZWrite");
    public static readonly int CusExtend4ParmId = Shader.PropertyToID("_CusExtend4Parm");
    public static readonly int CusExtendTexId = Shader.PropertyToID("_CusExtendTex");
    public static readonly int TimeId = Shader.PropertyToID("_Time");
    //public static Vector4 CusExtend4Parm = new Vector4(0, 0, 0, 0);
    public static readonly string OutlinePassName = "Outline";

    public class ShaderInfo
    {
        public GameObject m_Go;
        public List<Renderer> m_Renderers;
        public List<Material> m_OldMats;

        public void Reset()
        {
            if (m_Renderers != null && m_OldMats != null)
            {
                for (int i = 0; i < m_OldMats.Count; i++)
                {
                    if (i < m_Renderers.Count)
                    {
                        var renderer = m_Renderers[i];
                        if (renderer != null)
                            renderer.material = m_OldMats[i];
                        else
                        {
                            var oldMat = m_OldMats[i];
                            if (oldMat != null)
                                Object.DestroyImmediate(oldMat);
                        }
                    }
                    else
                    {
                        var oldMat = m_OldMats[i];
                        if (oldMat != null)
                            Object.DestroyImmediate(oldMat);
                    }
                }
            }
        }

        public static void Push(ShaderInfo shaderInfo, bool isForceDestroyMat)
        {
            if (shaderInfo == null)
                return;

            shaderInfo.m_Go = null;

            if (shaderInfo.m_OldMats != null)
            {
                for (int i = 0; i < shaderInfo.m_OldMats.Count; i++)
                {
                    if (isForceDestroyMat || shaderInfo.m_Renderers == null)
                    {
                        if (shaderInfo.m_Renderers != null)
                        {
                            var renderer = shaderInfo.m_Renderers[i];
                            if (renderer != null)
                                renderer.material = null;
                        }

                        var oldMat = shaderInfo.m_OldMats[i];
                        if (oldMat != null)
                            Object.DestroyImmediate(oldMat);
                    }
                    else if (i < shaderInfo.m_Renderers.Count)
                    {
                        var renderer = shaderInfo.m_Renderers[i];
                        if (renderer != null)
                            renderer.material = shaderInfo.m_OldMats[i];
                        else
                        {
                            var oldMat = shaderInfo.m_OldMats[i];
                            if (oldMat != null)
                                Object.DestroyImmediate(oldMat);
                        }
                    }
                }
            }
            if (shaderInfo.m_Renderers != null)
                shaderInfo.m_Renderers.Clear();
            if (shaderInfo.m_OldMats != null)
                shaderInfo.m_OldMats.Clear();

            CombatObjectPool.Instance.Push(shaderInfo);
        }
    }
    
    private static ShaderController _instance;

    public static ShaderController Instance
    {
        get
        {
            if (null == _instance)
            {
                _instance = EntityFactory.Create<ShaderController>();
            }
            return _instance;
        }
    }

    public int ShaderToInt_BaseColor;
    public int ShaderToInt_TintColor;

    private Material _customGhostMat;
    public Material m_CustomGhostMat
    {
        get
        {
            if (_customGhostMat == null)
            {
                var ghostShader = ShaderManager.Find("Custom/CustomGhost");
                if (ghostShader != null)
                {
                    _customGhostMat = new Material(ghostShader);
                    _customGhostMat.SetColor("_TintColor", new Color(1f, 1f, 0.85f, 0.72f));
                    _customGhostMat.SetFloat("_LuminanceFactor", 0.92f);
                }
            }

            return _customGhostMat;
        }
    }

    private Material _customGhostTimeMat;
    public Material m_CustomGhostTimeMat
    {
        get
        {
            if (_customGhostTimeMat == null)
            {
                var ghostTimeShader = ShaderManager.Find("Custom/CustomGhostTime");
                if (ghostTimeShader != null)
                {
                    _customGhostTimeMat = new Material(ghostTimeShader);
                    _customGhostTimeMat.SetColor("_TintColor", new Color(1.0f, 0.961f, 0.447f, 0.667f));
                    _customGhostTimeMat.SetFloat("_LuminanceFactor", 1.3f);
                    _customGhostTimeMat.SetFloat("_Move", 0.02f);
                }
            }

            return _customGhostTimeMat;
        }
    }

    private MaterialPropertyBlock _matPropertyBlock;
    public MaterialPropertyBlock MatPropertyBlock
    {
        get
        {
            if (_matPropertyBlock == null)
                _matPropertyBlock = new MaterialPropertyBlock();

            return _matPropertyBlock;
        }
    }

    private Material _buffRockMat;

    private List<ShaderInfo> _shaderInfos;

    private float _time;

    private Texture _snowNoiseTex;

    public void OnAwake()
    {
        ShaderToInt_BaseColor = Shader.PropertyToID("_BaseColor");
        ShaderToInt_TintColor = Shader.PropertyToID("_TintColor");

        if (_shaderInfos == null)
            _shaderInfos = new List<ShaderInfo>();

        Shader buffEffectShader = ShaderManager.Find("Custom/BuffEffect");

        _buffRockMat = new Material(buffEffectShader);
        
        AsyncOperationHandle<Texture> requestRef = default;
        AddressablesUtil.LoadAssetAsync(ref requestRef, "Texture/BuffEffect/BuffRock.png", (handle) =>
        {
            _buffRockMat.SetTexture("_BMainTex", handle.Result);
            var buffRockParmStrs = CSVParam.Instance.GetConfData(421).str_value.Split('|');
            _buffRockMat.SetFloat("_BDiffSSMax", float.Parse(buffRockParmStrs[0]) * 0.0001f);
            _buffRockMat.SetFloat("_BDiffuseFac", float.Parse(buffRockParmStrs[1]) * 0.0001f);
            _buffRockMat.SetColor("_BColor", new Color(float.Parse(buffRockParmStrs[2]) * 0.0001f, float.Parse(buffRockParmStrs[3]) * 0.0001f, float.Parse(buffRockParmStrs[4]) * 0.0001f, 1f));
            _buffRockMat.SetFloat("_BGlossiness", float.Parse(buffRockParmStrs[5]) * 0.0001f);
            _buffRockMat.SetColor("_BSpecularColor", new Color(float.Parse(buffRockParmStrs[6]) * 0.0001f, float.Parse(buffRockParmStrs[7]) * 0.0001f, float.Parse(buffRockParmStrs[8]) * 0.0001f, 1f));
        });
        
        AsyncOperationHandle<Texture> noiseRequestRef = default;
        AddressablesUtil.LoadAssetAsync(ref noiseRequestRef, "Texture/BuffEffect/SnowNoiseTex.png", (handle) =>
        {
            _snowNoiseTex = handle.Result;
        });

        CombatManager.Instance.m_EventEmitter.Handle(CombatManager.EEvents.DestroyMaterial, DestroyMaterialsEvt, true);
    }

    public void Update()
    {
        if (_shaderInfos != null && _shaderInfos.Count > 0)
        {
            _time += Time.deltaTime;

            if (_time > 20f)
            {
                _time = 0f;

                for (int i = _shaderInfos.Count - 1; i > -1; --i)
                {
                    var shaderInfo = _shaderInfos[i];

                    bool isNeedRemove = false;
                    for (int renderIndex = 0, renderCount = shaderInfo.m_Renderers.Count; renderIndex < renderCount; renderIndex++)
                    {
                        if (shaderInfo.m_Renderers[renderIndex] == null)
                        {
                            isNeedRemove = true;

                            break;
                        }
                    }

                    if (isNeedRemove)
                    {
                        _shaderInfos.RemoveAt(i);
                        ShaderInfo.Push(shaderInfo, true);
                    }
                }
            }
        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < _shaderInfos.Count; i++)
        {
            _shaderInfos[i].Reset();
        }

        _time = 19f;
    }

    public void OnDispose()
    {
        CombatManager.Instance.m_EventEmitter.Handle(CombatManager.EEvents.DestroyMaterial, DestroyMaterialsEvt, false);
    }

    private void DestroyMaterialsEvt()
    {
        for (int i = _shaderInfos.Count - 1; i > -1; --i)
        {
            var shaderInfo = _shaderInfos[i];
            if (shaderInfo.m_Go == null)
            {
                _shaderInfos.RemoveAt(i);
                ShaderInfo.Push(shaderInfo, true);
            }
        }
    }
    
    public void SwitchBuffEffect(MobEntity mobEntity, uint effect_parma)
    {
        for (int i = _shaderInfos.Count - 1; i > -1; --i)
        {
            var si = _shaderInfos[i];
            if (si.m_Go == mobEntity.m_Go)
            {
                if (si.m_Renderers != null && si.m_OldMats != null)
                {
                    bool isExist = true;
                    foreach (var rd in si.m_Renderers)
                    {
                        if (rd == null)
                        {
                            isExist = false;
                            break;
                        }
                        else
                            rd.material = GetBuffEffectMatByType(effect_parma);
                    }

                    if (isExist)
                        return;
                    else
                    {
                        _shaderInfos.RemoveAt(i);
                        ShaderInfo.Push(si, true);
                    }
                }
            }
        }
        
        ShaderInfo shaderInfo = CombatObjectPool.Instance.Get<ShaderInfo>();
        shaderInfo.m_Go = mobEntity.m_Go;
        if (shaderInfo.m_Renderers == null)
            shaderInfo.m_Renderers = new List<Renderer>();
        if (shaderInfo.m_OldMats == null)
            shaderInfo.m_OldMats = new List<Material>();

        int index = 0;
        for (int i = 0; i < mobEntity.SkinnedMeshRenderers.Length; i++)
        {
            SkinnedMeshRenderer skinnedMeshRenderer = mobEntity.SkinnedMeshRenderers[i];

            shaderInfo.m_Renderers.Add(skinnedMeshRenderer);
            shaderInfo.m_OldMats.Add(skinnedMeshRenderer.material);
            
            skinnedMeshRenderer.material = GetBuffEffectMatByType(effect_parma);

            index++;
        }

        for (int i = 0; i < mobEntity.MeshFilters.Length; i++)
        {
            MeshFilter meshFilter = mobEntity.MeshFilters[i];
            if (meshFilter == null)
                continue;

            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
                continue;

            string mrGoName = meshRenderer.gameObject.name.ToLower();
            if (!mrGoName.Contains("mesh"))
                continue;

            shaderInfo.m_Renderers.Add(meshRenderer);
            shaderInfo.m_OldMats.Add(meshRenderer.material);
            
            meshRenderer.material = GetBuffEffectMatByType(effect_parma);

            index++;
        }

        _shaderInfos.Add(shaderInfo);
    }

    public void RestoreMat(MobEntity mobEntity)
    {
        for (int i = 0; i < _shaderInfos.Count; i++)
        {
            ShaderInfo shaderInfo = _shaderInfos[i];
            if (shaderInfo == null)
                continue;

            if (shaderInfo.m_Go == mobEntity.m_Go)
            {
                shaderInfo.Reset();
                break;
            }
        }
    }

    public void ShowDissolveBloom(Material mat, bool isOpenDissolve, float speed, float offset, float displayType,  
        RenderQueue renderQueue, bool isHaveOutline = true, float shineRange = 0f, float shineOffset = 0f)
    {
        if (isOpenDissolve)
        {
            mat.EnableKeyword(ShortTimeExistExetendStr);

            if (renderQueue == RenderQueue.Transparent)
            {
                mat.SetOverrideTag(RenderTypeStr, TransparentStr);
                //mat.SetOverrideTag(RenderTypeStr, "TransparentCutout");
                mat.SetInt(SrcBlendId, (int)BlendMode.SrcAlpha);
                mat.SetInt(DstBlendId, (int)BlendMode.OneMinusSrcAlpha);
                //mat.SetInt(ZWriteId, 0);
                mat.renderQueue = (int)renderQueue;
            }
            
            Vector4 vector4 = Shader.GetGlobalVector(TimeId);
            vector4.x = vector4.y;

            int speedParam = (int)(speed * 100f);
            offset *= 0.1f;
            bool isPlus = true;
            if (offset < 0f)
            {
                offset = -offset;
                isPlus = false;
            }
            offset = offset - (int)offset;
            vector4.y = isPlus ? (speedParam + offset) : -(speedParam + offset);

            if (shineRange <= 0f)
                shineRange = 0.196f;
            if (shineOffset <= 0f)
                shineOffset = 0.189f;
            vector4.z = (int)(shineRange * 1000f) + (shineOffset - (int)shineOffset);

            vector4.w = displayType;
            mat.SetVector(CusExtend4ParmId, vector4);
            mat.SetTexture(CusExtendTexId, _snowNoiseTex);
            if (isHaveOutline)
                mat.SetShaderPassEnabled(OutlinePassName, false);
        }
        else
        {
            mat.DisableKeyword(ShortTimeExistExetendStr);

            if (renderQueue > 0)
            {
                mat.SetOverrideTag(RenderTypeStr, OpaqueStr);
                mat.SetInt(SrcBlendId, (int)BlendMode.One);
                mat.SetInt(DstBlendId, (int)BlendMode.Zero);
                //mat.SetInt(ZWriteId, 1);
                mat.renderQueue = (int)renderQueue;
            }
            
            mat.SetTexture(CusExtendTexId, null);
            if (isHaveOutline)
                mat.SetShaderPassEnabled(OutlinePassName, true);
        }
    }

    private Material GetBuffEffectMatByType(uint effect_param)
    {
        if (effect_param == 1)
            return _buffRockMat;

        return m_CustomGhostTimeMat;
    }
}
