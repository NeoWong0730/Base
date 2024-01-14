using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public struct RendererLightMapInfo
{
    public Renderer mRenderer;
    public int nLightmapIndex;
    public Vector4 vLightmapScaleOffset;
}

[System.Serializable]
public struct TerrainLightMapInfo
{
    public Terrain mTerrain;
    public int nLightmapIndex;
    public Vector4 vLightmapScaleOffset;
}

public class SceneLightMapData : MonoBehaviour
{
    [SerializeField] public RendererLightMapInfo[] mRendererLightMapInfos = null;
    [SerializeField] public TerrainLightMapInfo[] mTerrainLightMapInfos = null;
    [SerializeField] public string mLightMapDataAddress = null;
    public LightMapAsset mLightMapData = null;
    private AsyncOperationHandle<LightMapAsset> _handle;

    private void Start()
    {        
        if(mLightMapData)
        {
            SetLightMapActive(mLightMapData);
        }
        else if(!string.IsNullOrWhiteSpace(mLightMapDataAddress))
        {
            AddressablesUtil.LoadAssetAsync<LightMapAsset>(ref _handle, mLightMapDataAddress, OnLoaded);
        }
    }

    private void OnDestroy()
    {        
        LightmapSettings.lightmaps = null;
        AddressablesUtil.Release<LightMapAsset>(ref _handle, OnLoaded);
    }

    private void OnLoaded(AsyncOperationHandle<LightMapAsset> handle)
    {
        mLightMapData = handle.Result;
        if (mLightMapData)
        {
            SetLightMapActive(mLightMapData);
            SetRendererLightMapData();
        }
    }

    private void SetLightMapActive(LightMapAsset lightMapAsset)
    {
        int len = Mathf.Max(lightMapAsset.lightmapColor.Length, lightMapAsset.lightmapDir.Length, lightMapAsset.shadowMask.Length);
        LightmapData[] lightmapDatas = new LightmapData[len];
        
        for (int i = 0; i < len; ++i)
        {
            LightmapData lightmapData = new LightmapData();
            lightmapData.lightmapColor = lightMapAsset.lightmapColor[i];
            lightmapData.lightmapDir = lightMapAsset.lightmapDir[i];
            lightmapData.shadowMask = lightMapAsset.shadowMask[i];
            lightmapDatas[i] = lightmapData;
        }

        LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
        LightmapSettings.lightmaps = lightmapDatas;
    }

    private void SetRendererLightMapData()
    {
        RendererLightMapInfo rendererLightMapInfo;
        Renderer renderer;
        for (int i = 0; i < mRendererLightMapInfos.Length; ++i)
        {
            rendererLightMapInfo = mRendererLightMapInfos[i];
            renderer = rendererLightMapInfo.mRenderer;
            renderer.lightmapIndex = rendererLightMapInfo.nLightmapIndex;
            renderer.lightmapScaleOffset = rendererLightMapInfo.vLightmapScaleOffset;
        }

        TerrainLightMapInfo terrainLightMapInfo;
        Terrain terrain;
        for (int i = 0; i < mTerrainLightMapInfos.Length; ++i)
        {
            terrainLightMapInfo = mTerrainLightMapInfos[i];
            terrain = terrainLightMapInfo.mTerrain;
            terrain.lightmapIndex = terrainLightMapInfo.nLightmapIndex;
            terrain.lightmapScaleOffset = terrainLightMapInfo.vLightmapScaleOffset;
        }
    }
}
