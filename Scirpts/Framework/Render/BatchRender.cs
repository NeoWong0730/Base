#define SCENE_DISPLAY

using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using System;
using Lib;

namespace Framework
{
    [ExecuteAlways]
    [RequireComponent(typeof(Terrain))]
    [DisallowMultipleComponent]
    public sealed class BatchRender : MonoBehaviour, IDisposable
    {
        [BurstCompile]
        struct RealTimeMatrixJob : IJobParallelFor
        {
            [ReadOnly] public float3 SeedAndRange;
            [ReadOnly] public float3 Scale;
            [ReadOnly] public float3 Rotate;

            [ReadOnly] public float3 TerrainPosition;
            [ReadOnly] public float3 TerrainSize;
            [ReadOnly] public float2 TileSize;

            [ReadOnly] public int DetailResolution;
            [ReadOnly] public int HeightmapResolution;

            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<float2> PositionOffsets;
            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<int3> PosAndIndexs;
            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<float> Scales;
            [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<Matrix4x4> Results;
            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<float> HeightMaps;

            public void Execute(int index)
            {
                int3 posAndIndex = PosAndIndexs[index];
                float dd = (float)HeightmapResolution / (float)DetailResolution;

                float y = HeightMaps[(int)(posAndIndex.y * dd) * HeightmapResolution + (int)(posAndIndex.x * dd)] * TerrainSize.y;
                float3 position = new float3((float)posAndIndex.x / DetailResolution * TerrainSize.x, y, (float)posAndIndex.y / DetailResolution * TerrainSize.z);

                float s = noise.snoise(position.xz * SeedAndRange.z);
                float scale = math.max(math.lerp(SeedAndRange.x, SeedAndRange.y, s) * Scales[index], SeedAndRange.x);

                position += new float3(PositionOffsets[posAndIndex.z].x * TileSize.x, 0, PositionOffsets[posAndIndex.z].y * TileSize.y);
                position += TerrainPosition;
                Results[index] = float4x4.TRS(position, quaternion.Euler(Rotate), math.abs(Scale * scale));
            }
        }

        #region Common
        //TODO 可以设置为全局的风
        public static readonly int kCountPerDrawCall = 1023;
        public static readonly int kMaxMatrix = 8196;
        uint[] args = new uint[5] { 0u, 0u, 0u, 0u, 0u };
        List<ComputeBuffer> argBuffer;
        List<ComputeBuffer> matrixBuffer;

        public static Vector3 CollidePosition;

        public Terrain terrain;

        [Range(0, 4)]
        public int nStripLevel = 0;
        [Range(0, 360)]
        public float fWindRotate = 0;
        public float fWindSpeed = 1;
        public float CollideSize = 0.6f;

        public int3 vQTreeSize = new int3(32, 1, 32);
        private QTree mQTree;

        public List<InstancingRenderData> mRenderDatas;
        private Matrix4x4[] tempMatrix4X4 = new Matrix4x4[kMaxMatrix];
        #endregion

        public InstancingData mInstancingData;
        public bool useInstancedIndirect = true;

#if UNITY_EDITOR && SCENE_DISPLAY
        public bool bDrawScene = true;
        public bool bDrawGizmos = true;
        public int nDrawMip = 0;
        [Range(1, 9)]
        public int maxDensity = 9;

        private NativeArray<float> mHeightMaps;
        private NativeArray<float2> mPositionOffsets;
        private float[,,] alphaData;

        private void CacheHeightMap()
        {
            if (!mHeightMaps.IsCreated || alphaData == null)
            {
                UpdateHeightMap();
            }

            if (!mPositionOffsets.IsCreated)
            {
                mPositionOffsets = new NativeArray<float2>(9, Allocator.Persistent);
                mPositionOffsets[0] = new float2(0.5f, 0.5f);
                mPositionOffsets[1] = new float2(0.25f, 0.25f);
                mPositionOffsets[2] = new float2(0.75f, 0.75f);
                mPositionOffsets[3] = new float2(0.25f, 0.75f);
                mPositionOffsets[4] = new float2(0.75f, 0.25f);
                mPositionOffsets[5] = new float2(0.25f, 0.5f);
                mPositionOffsets[6] = new float2(0.75f, 0.5f);
                mPositionOffsets[7] = new float2(0.5f, 0.25f);
                mPositionOffsets[8] = new float2(0.5f, 0.75f);
            }
        }


        [ContextMenu("UpdateHeightMap")]
        private void UpdateHeightMap()
        {
            alphaData = TerrainUtil.GetAlphaMap(terrain, 0, 0, 0, 0);

            int heightmapResolution = terrain.terrainData.heightmapResolution;
            if (mHeightMaps.IsCreated && mHeightMaps.Length != heightmapResolution * heightmapResolution)
            {
                mHeightMaps.Dispose();
            }
            if (!mHeightMaps.IsCreated)
            {
                mHeightMaps = new NativeArray<float>(heightmapResolution * heightmapResolution, Allocator.Persistent);
            }

            float[,] heightMap = TerrainUtil.GetHeightMap(terrain, 0, 0, 0, 0);

            for (int y = 0; y < heightmapResolution; ++y)
            {
                for (int x = 0; x < heightmapResolution; ++x)
                {
                    mHeightMaps[y * heightmapResolution + x] = heightMap[y, x];
                }
            }
        }

        [ContextMenu("Bake")]
        private void Bake()
        {
            UpdateHeightMap();

            //总的尺寸
            int heightmapResolution = terrain.terrainData.heightmapResolution;
            int detailResolution = terrain.terrainData.detailResolution;

            //单个块所占的尺寸
            int2 gridNodeSize = new int2(detailResolution / vQTreeSize.x, detailResolution / vQTreeSize.z);

            int layerCount = mRenderDatas.Count;

            List<Matrix4x4> matrices = new List<Matrix4x4>(1024 * 1024);

            DetailLayerDate[] layerDatas = new DetailLayerDate[layerCount];
            for (int layer = 0; layer < layerDatas.Length; ++layer)
            {
                layerDatas[layer].chunckDatas = new DetailChunckData[vQTreeSize.z * vQTreeSize.x];
            }

            //计算每一层的内容
            int detailOffset = 0;
            for (int layer = 0; layer < layerCount; ++layer)
            {
                int[,] detailData = TerrainUtil.GetDetailMap(terrain, 0, 0, detailResolution, detailResolution, layer);
                float[,,] alphaData = TerrainUtil.GetAlphaMap(terrain, 0, 0, 0, 0);

                DetailChunckData[] detailChunckDatas = layerDatas[layer].chunckDatas;

                float3 randomScale = mRenderDatas[layer].randomScale;
                float3 scale = mRenderDatas[layer].scale;
                float3 rotate = mRenderDatas[layer].rotate;
                int[] alphaMasks = mRenderDatas[layer].alphaMasks;
                float threshold = mRenderDatas[layer].threshold;

                int index = 0;

                for (int z = 0; z < vQTreeSize.z; ++z)
                {
                    for (int x = 0; x < vQTreeSize.x; ++x)
                    {
                        int count = BakeChunckData(detailData, alphaData, alphaMasks, threshold, matrices, mPositionOffsets, x * gridNodeSize.x, z * gridNodeSize.y, gridNodeSize.x, gridNodeSize.y, randomScale, rotate, scale);

                        if (count > 0)
                        {
                            DetailChunckData detailChunckData = new DetailChunckData();
                            detailChunckData.boundIndex = z * vQTreeSize.x + x;
                            detailChunckData.offset = detailOffset;
                            detailChunckData.count = count;

                            detailChunckDatas[index] = detailChunckData;
                            ++index;
                        }

                        detailOffset += count;
                    }
                }

                DetailChunckData[] details = new DetailChunckData[index];
                Array.Copy(detailChunckDatas, details, index);
                layerDatas[layer].chunckDatas = details;
            }

            InstancingData instancingData = ScriptableObject.CreateInstance<InstancingData>();

            instancingData.layerDatas = layerDatas;
            instancingData.matrices = matrices.ToArray();

            string path = UnityEditor.AssetDatabase.GetAssetPath(terrain.terrainData);
            string dir = System.IO.Path.GetDirectoryName(path);
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            mInstancingData = instancingData;

            string outPut = dir + "/" + fileName + "_GrassData.asset";
            UnityEditor.AssetDatabase.CreateAsset(instancingData, outPut);

            Debug.LogFormat("bake finished {0}", outPut);
        }
        private int BakeChunckData(int[,] detailData, float[,,] alphaData, int[] alphaMasks, float threshold, List<Matrix4x4> list, NativeArray<float2> offset, int xBase, int yBase, int width, int height, float3 seedAndRange, float3 rotate, float3 scale)
        {
            int count = 0;

            int HeightmapResolution = terrain.terrainData.heightmapResolution;
            int DetailResolution = terrain.terrainData.detailResolution;
            float3 TerrainSize = terrain.terrainData.size;
            float2 TileSize = TerrainSize.xz / terrain.terrainData.detailResolution;
            float3 SeedAndRange = new float3(seedAndRange.z, seedAndRange.x, seedAndRange.y);
            float3 TerrainPosition = terrain.GetPosition();
            float2 alphaDD = new float2((float)terrain.terrainData.alphamapWidth / terrain.terrainData.detailResolution, (float)terrain.terrainData.alphamapHeight / terrain.terrainData.detailResolution);

            for (int y = yBase; y < height + yBase; ++y)
            {
                for (int x = xBase; x < width + xBase; ++x)
                {
                    int detailCount = math.min(maxDensity, detailData[y, x]);

                    float2 posXZ = TerrainUtil.GetIndexPostion(terrain, x, y);
                    float posY = TerrainUtil.GetPointHeight(terrain, new Vector3(posXZ.x, 0, posXZ.y));

                    float alphaScale = 1f;
                    if (alphaMasks != null && alphaMasks.Length > 0)
                    {
                        float alpha = 0;
                        for (int maskIndex = 0; maskIndex < alphaMasks.Length; ++maskIndex)
                        {
                            alpha += alphaData[(int)(y * alphaDD.y), (int)(x * alphaDD.x), alphaMasks[maskIndex]];
                        }

                        if (alpha <= threshold)
                        {
                            detailCount = 0;
                        }
                        else
                        {
                            detailCount = math.min(math.max(1, (int)(detailCount * alpha)), detailCount);
                            alphaScale = math.lerp(0.5f, 1f, (alpha - threshold) / (1f - threshold));
                        }
                    }

                    for (int i = 0; i < detailCount; ++i)
                    {
                        int3 posAndIndex = new int3(x, y, i);

                        float dd = (float)HeightmapResolution / (float)DetailResolution;

                        float positionY = mHeightMaps[(int)(posAndIndex.y * dd) * HeightmapResolution + (int)(posAndIndex.x * dd)] * TerrainSize.y;
                        float3 position = new float3((float)posAndIndex.x / DetailResolution * TerrainSize.x, positionY, (float)posAndIndex.y / DetailResolution * TerrainSize.z);

                        float s = noise.snoise(position.xz * SeedAndRange.x);
                        float randomScale = math.max(math.lerp(SeedAndRange.y, SeedAndRange.z, s) * alphaScale, SeedAndRange.y);

                        position += new float3(mPositionOffsets[posAndIndex.z].x * TileSize.x, 0, mPositionOffsets[posAndIndex.z].y * TileSize.y);
                        position += TerrainPosition;

                        list.Add(float4x4.TRS(position, quaternion.Euler(rotate), scale * randomScale));
                        //list.Add(new float4(position, scale));

                        ++count;
                    }
                }
            }
            return count;
        }
        public void ExecuteRealTime(Camera camera)
        {
            if (nStripLevel == 4)
                return;

            CacheHeightMap();

            //计算风向的垂直轴
            float3 axis = new float3(1, 0, 0);
            axis = math.mul(quaternion.RotateY((fWindRotate / 180f + 0.5f) * math.PI), axis);

            float tileSizeX = terrain.terrainData.size.x / terrain.terrainData.detailWidth / 3f;
            float tileSizeY = terrain.terrainData.size.z / terrain.terrainData.detailHeight / 3f;
            int alphamapLayers = terrain.terrainData.alphamapLayers;

            int arrayCount = 0;
            NativeArray<int3> posAndIndexs = new NativeArray<int3>(kCountPerDrawCall, Allocator.TempJob);
            NativeArray<float> scales = new NativeArray<float>(kCountPerDrawCall, Allocator.TempJob);
            NativeArray<Matrix4x4> matrix4X4s = new NativeArray<Matrix4x4>(kCountPerDrawCall, Allocator.TempJob);
            float2 dd = new float2((float)terrain.terrainData.alphamapWidth / terrain.terrainData.detailResolution, (float)terrain.terrainData.alphamapHeight / terrain.terrainData.detailResolution);

            int offset = 0;
            int count = mQTree.GetGridSize(0);

            for (int layer = 0; layer < mRenderDatas.Count; ++layer)
            {
                InstancingRenderData renderData = mRenderDatas[layer];
                if (renderData.material == null || renderData.mesh == null)
                {
                    continue;
                }

                int[,] detailData = TerrainUtil.GetDetailMap(terrain, 0, 0, 0, 0, layer);

                for (int visualIndex = offset; visualIndex < offset + count; ++visualIndex)
                {
                    if (mQTree.GetIntersectResult(visualIndex) == Unity.Rendering.FrustumPlanes.IntersectResult.Out)
                        continue;

                    int index = 0;

                    int2 gridSize = terrain.terrainData.detailResolution / mQTree.vSize.xz;

                    int2 gridPos = mQTree.GetNodePos(visualIndex, 0).xz * gridSize;

                    for (int y = 0; y < gridSize.y; ++y)
                    {
                        for (int x = 0; x < gridSize.x; ++x)
                        {
                            int2 pos = new int2(x + gridPos.x, y + gridPos.y);
                            int2 alphaPos = new int2((int)(pos.x * dd.x), (int)(pos.y * dd.y));

                            int detailCount = math.min(maxDensity, detailData[pos.y, pos.x]);

                            float scale = 1;
                            if (renderData.alphaMasks != null && renderData.alphaMasks.Length > 0)
                            {
                                float alpha = 0;
                                for (int maskIndex = 0; maskIndex < renderData.alphaMasks.Length; ++maskIndex)
                                {
                                    int alphaLayer = renderData.alphaMasks[maskIndex];
                                    if (alphaLayer < alphamapLayers)
                                    {
                                        alpha += alphaData[alphaPos.y, alphaPos.x, alphaLayer];
                                    }
                                }
                                if (alpha <= renderData.threshold)
                                {
                                    detailCount = 0;
                                }
                                else
                                {
                                    detailCount = math.min(math.max(1, (int)(detailCount * alpha)), detailCount);
                                    scale = math.lerp(0.5f, 1f, (alpha - renderData.threshold) / (1f - renderData.threshold));
                                    //scale = (alpha - renderData.threshold) / (1f - renderData.threshold) + 0.2f;
                                }
                            }

                            for (int i = 0; i < detailCount; ++i)
                            {
                                if (arrayCount >= kCountPerDrawCall)
                                {
                                    DrawBatch(camera, mRenderDatas[layer], arrayCount, posAndIndexs, matrix4X4s, scales);
                                    arrayCount = 0;
                                }
                                else
                                {
                                    ++index;

                                    if (index % (1 << nStripLevel) == 0)
                                    {
                                        scales[arrayCount] = scale;
                                        posAndIndexs[arrayCount] = new int3(pos, i);
                                        ++arrayCount;
                                    }
                                }
                            }
                        }
                    }
                }
                DrawBatch(camera, mRenderDatas[layer], arrayCount, posAndIndexs, matrix4X4s, scales);
                arrayCount = 0;
            }

            scales.Dispose();
            posAndIndexs.Dispose();
            matrix4X4s.Dispose();
        }
        private void DrawBatch(Camera camera, InstancingRenderData renderData, int count, NativeArray<int3> posAndIndexs, NativeArray<Matrix4x4> matrix4X4s, NativeArray<float> scales)
        {
            if (count == 0)
                return;

            float3 terrainSize = terrain.terrainData.size;
            float2 tileSize = terrainSize.xz / terrain.terrainData.detailResolution;

            RealTimeMatrixJob matrixJob = new RealTimeMatrixJob
            {
                SeedAndRange = renderData.randomScale,
                Scale = renderData.scale,
                Rotate = renderData.rotate,

                TerrainPosition = terrain.GetPosition(),
                TerrainSize = terrainSize,
                TileSize = tileSize,

                DetailResolution = terrain.terrainData.detailResolution,
                HeightmapResolution = terrain.terrainData.heightmapResolution,
                PositionOffsets = mPositionOffsets,

                Scales = scales,
                PosAndIndexs = posAndIndexs,
                Results = matrix4X4s,
                HeightMaps = mHeightMaps,
            };

            JobHandle jobHandle = matrixJob.Schedule<RealTimeMatrixJob>(count, 32);
            jobHandle.Complete();

            NativeArray<Matrix4x4>.Copy(matrix4X4s, tempMatrix4X4, matrix4X4s.Length);

            //SphericalHarmonicsL2 ambientSH;
            //LightProbes.GetInterpolatedProbe(Vector3.zero, null, out ambientSH);

            //         renderData.material.SetVector("unity_SHAr", new Vector4(0, 0, 0, ambientSH[0, 0]));
            //         renderData.material.SetVector("unity_SHAg", new Vector4(0, 0, 0, ambientSH[1, 0]));
            //         renderData.material.SetVector("unity_SHAb", new Vector4(0, 0, 0, ambientSH[2, 0]));

            //renderData.material.SetColor(Consts._AmbientSH_ID, new Color(ambientSH[0, 0], ambientSH[1, 0], ambientSH[2, 0], 1));

            Graphics.DrawMeshInstanced(renderData.mesh, 0, renderData.material, tempMatrix4X4, count, null, ShadowCastingMode.Off, true, 0, camera, LightProbeUsage.Off);
        }
#endif
        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
#if UNITY_EDITOR && SCENE_DISPLAY
            if (camera.cameraType == CameraType.Game)
            {
                if (LayerMaskUtil.ContainLayer(camera.cullingMask, (int)ELayerMask.Terrain))
                {
                    //视锥体裁剪
                    CreateQTreeIfNull();
                    if (mQTree != null)
                    {
                        mQTree.Cull(camera, terrain.detailObjectDistance);

                        if (!Application.isPlaying)
                        {
                            ExecuteRealTime(camera);
                        }
                    }
                }
            }
            else if (camera.cameraType == CameraType.SceneView)
            {
                if (!bDrawScene)
                    return;

                //视锥体裁剪
                CreateQTreeIfNull();
                if (mQTree != null)
                {
                    mQTree.Cull(camera, terrain.detailObjectDistance);
                    ExecuteRealTime(camera);
                }
            }
#else
        if (camera.cameraType == CameraType.Game)
        {
            if (LayerMaskUtil.ContainLayer(camera.cullingMask, (int)ELayerMask.Terrain))
            {
                //视锥体裁剪
                CreateQTreeIfNull();
                if (mQTree != null)
                {
                    if (Time.frameCount % 4 == 2)
                    {
                        mQTree.Cull(camera, terrain.detailObjectDistance);
                    }
                }
            }
        }
#endif
        }
        public void Execute(ProfilingSampler profilingSampler, ScriptableRenderContext context, UnityEngine.Rendering.Universal.RenderingData renderingData, UnityEngine.Experimental.Rendering.Universal.RenderQueueType renderQueueType)
        {
            if (mInstancingData == null)
                return;
            if (useInstancedIndirect && SystemInfo.supportsComputeShaders)
            {
                ExecuteDrawMeshInstancedIndirect(profilingSampler, context, renderingData.cameraData.camera, mInstancingData, mRenderDatas, renderQueueType);
            }
            else
            {
                ExecuteDrawMeshInstanced(profilingSampler, context, renderingData.cameraData.camera, mInstancingData, mRenderDatas, renderQueueType);
            }
        }
        public void ExecuteDrawMeshInstanced(ProfilingSampler profilingSampler, ScriptableRenderContext context, Camera camera, InstancingData instancingData, List<InstancingRenderData> renderDatas, UnityEngine.Experimental.Rendering.Universal.RenderQueueType renderQueueType)
        {
            if (nStripLevel == 4)
                return;

            if (!LayerMaskUtil.ContainLayer(camera.cullingMask, (int)ELayerMask.Terrain) || mQTree == null)
                return;

            //计算风向的垂直轴
            float3 axis = new float3(1, 0, 0);
            axis = math.mul(quaternion.RotateY((fWindRotate / 180f + 0.5f) * math.PI), axis);

            //绘制视锥体内的
            CommandBuffer cmd = CommandBufferPool.Get(profilingSampler.name);
            cmd.Clear();
            cmd.SetGlobalVector(Consts._CollideInfo_ID, new float4(CollidePosition, CollideSize));
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            int arrayCount = 0;

            for (int layer = 0; layer < instancingData.layerDatas.Length; ++layer)
            {
                InstancingRenderData renderData = renderDatas[layer];

                if (renderData.material == null || renderData.mesh == null)
                {
                    continue;
                }

                if ((renderData.material.renderQueue <= 2450 && renderQueueType == UnityEngine.Experimental.Rendering.Universal.RenderQueueType.Opaque)
                    || (renderData.material.renderQueue > 2450 && renderQueueType == UnityEngine.Experimental.Rendering.Universal.RenderQueueType.Transparent))
                {

                }
                else
                {
                    continue;
                }

                DetailChunckData[] chunckDatas = instancingData.layerDatas[layer].chunckDatas;
                for (int i = 0; i < chunckDatas.Length; ++i)
                {
                    DetailChunckData chunckData = chunckDatas[i];

                    if (mQTree.GetIntersectResult(chunckData.boundIndex) == Unity.Rendering.FrustumPlanes.IntersectResult.Out)
                        continue;

                    for (int j = chunckData.offset; j < chunckData.offset + chunckData.count; ++j)
                    {
                        if (arrayCount >= kCountPerDrawCall)
                        {
                            DrawMeshInstanced(profilingSampler, context, cmd, renderData, arrayCount);
                            arrayCount = 0;
                        }

                        if (j % (1 << nStripLevel) == 0)
                        {
                            tempMatrix4X4[arrayCount] = instancingData.matrices[j];
                            ++arrayCount;
                        }
                    }
                }

                if (arrayCount > 0)
                {
                    DrawMeshInstanced(profilingSampler, context, cmd, renderData, arrayCount);
                    arrayCount = 0;
                }
            }

            CommandBufferPool.Release(cmd);
        }
        private void DrawMeshInstanced(ProfilingSampler profilingSampler, ScriptableRenderContext context, CommandBuffer cmd, InstancingRenderData renderData, int count)
        {
            cmd.DrawMeshInstanced(renderData.mesh, 0, renderData.material, -1, tempMatrix4X4, count);
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
        }

        public void ExecuteDrawMeshInstancedIndirect(ProfilingSampler profilingSampler, ScriptableRenderContext context, Camera camera, InstancingData instancingData, List<InstancingRenderData> renderDatas, UnityEngine.Experimental.Rendering.Universal.RenderQueueType renderQueueType)
        {
            if (nStripLevel == 4)
                return;

            if (!LayerMaskUtil.ContainLayer(camera.cullingMask, (int)ELayerMask.Terrain) || mQTree == null)
                return;

            if (argBuffer == null)
            {
                argBuffer = new List<ComputeBuffer>(instancingData.layerDatas.Length);
            }

            if (matrixBuffer == null)
            {
                matrixBuffer = new List<ComputeBuffer>(instancingData.layerDatas.Length);
            }

            for (int i = argBuffer.Count; i < instancingData.layerDatas.Length; ++i)
            {
                argBuffer.Add(new ComputeBuffer(1, 5 * 4, ComputeBufferType.IndirectArguments));
            }

            for (int i = matrixBuffer.Count; i < instancingData.layerDatas.Length; ++i)
            {
                matrixBuffer.Add(null);
            }

            //计算风向的垂直轴
            float3 axis = new float3(1, 0, 0);
            axis = math.mul(quaternion.RotateY((fWindRotate / 180f + 0.5f) * math.PI), axis);

            //绘制视锥体内的
            CommandBuffer cmd = CommandBufferPool.Get(profilingSampler.name);
            cmd.Clear();
            cmd.SetGlobalVector(Consts._CollideInfo_ID, new float4(CollidePosition, CollideSize));
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            for (int layer = 0; layer < instancingData.layerDatas.Length; ++layer)//instancingData.layerDatas.Length
            {
                InstancingRenderData renderData = renderDatas[layer];

                if (renderData.material == null || renderData.mesh == null)
                {
                    continue;
                }

                if ((renderData.material.renderQueue <= (int)RenderQueue.AlphaTest && renderQueueType == UnityEngine.Experimental.Rendering.Universal.RenderQueueType.Opaque)
                    || (renderData.material.renderQueue > (int)RenderQueue.AlphaTest && renderQueueType == UnityEngine.Experimental.Rendering.Universal.RenderQueueType.Transparent))
                {

                }
                else
                {
                    continue;
                }

                int arrayCount = 0;
                DetailChunckData[] chunckDatas = instancingData.layerDatas[layer].chunckDatas;
                for (int i = 0; i < chunckDatas.Length; ++i)
                {
                    DetailChunckData chunckData = chunckDatas[i];

                    if (mQTree.GetIntersectResult(chunckData.boundIndex) == Unity.Rendering.FrustumPlanes.IntersectResult.Out)
                        continue;

                    int realcount = math.min(chunckData.count, tempMatrix4X4.Length - arrayCount);
                    Array.Copy(instancingData.matrices, chunckData.offset, tempMatrix4X4, arrayCount, realcount);
                    arrayCount += realcount;

                    if (arrayCount >= tempMatrix4X4.Length)
                    {
                        DebugUtil.LogWarningFormat("BatchRender 绘制的实例数量超过8192， 希望控制一下");
                        break;
                    }
                }

                if (arrayCount == 1)
                {
                    cmd.DrawMesh(renderData.mesh, tempMatrix4X4[0], renderData.material, 0, 0);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
                else if (arrayCount <= kCountPerDrawCall)
                {
                    cmd.DrawMeshInstanced(renderData.mesh, 0, renderData.material, 0, tempMatrix4X4, arrayCount);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
                else if (arrayCount > kCountPerDrawCall)
                {
                    ComputeBuffer tempMatrixBuffer = matrixBuffer[layer];
                    ComputeBuffer tempArgBuffer = argBuffer[layer];

                    if (tempMatrixBuffer == null)
                    {
                        tempMatrixBuffer = new ComputeBuffer(arrayCount, 16 * 4);
                        matrixBuffer[layer] = tempMatrixBuffer;
                    }

                    else if (tempMatrixBuffer.count < arrayCount)
                    {
                        tempMatrixBuffer.SetCounterValue((uint)arrayCount);
                        tempMatrixBuffer = new ComputeBuffer(arrayCount, 16 * 4);
                        matrixBuffer[layer] = tempMatrixBuffer;
                    }

                    tempMatrixBuffer.SetData(tempMatrix4X4, 0, 0, arrayCount);

                    args[0] = renderData.mesh.GetIndexCount(0);
                    args[1] = (uint)arrayCount;
                    args[2] = renderData.mesh.GetIndexStart(0);
                    args[3] = renderData.mesh.GetBaseVertex(0);
                    tempArgBuffer.SetData(args);

                    renderData.material.SetBuffer(Consts._MatrixBuffer_ID, tempMatrixBuffer);
                    cmd.DrawMeshInstancedIndirect(renderData.mesh, 0, renderData.material, 0, tempArgBuffer);
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();
                }
            }

            CommandBufferPool.Release(cmd);
        }

        private void CreateQTreeIfNull()
        {
            if (mQTree == null || !mQTree.vSize.Equals(vQTreeSize))
            {
                if (terrain == null || terrain.terrainData == null)
                    return;

                if (mQTree == null)
                {
                    mQTree = new QTree();
                }

                Vector3 extents = terrain.terrainData.size / 2f;
                extents.y = 2f;

                Vector3 center = terrain.GetPosition() + extents;
                AABB bounds = new AABB();
                bounds.Center = center;
                extents.y = 1;
                bounds.Extents = extents;

                mQTree.SetSize(bounds, vQTreeSize, 3);
            }
        }

        private void Awake()
        {
            if (terrain == null)
            {
                terrain = GetComponent<Terrain>();
            }
            if (terrain != null)
            {
                terrain.drawTreesAndFoliage = false;
            }
            if (mQTree == null)
            {
                mQTree = new QTree();
            }
        }
        private void OnEnable()
        {
            if (SystemInfo.supportsInstancing)
            {
                if (Application.isPlaying)
                {
                    DrawInstance.onDrawInstance += Execute;
                }

                RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            }
            else
            {
                DebugUtil.LogError("supportsInstancing false");
            }
        }
        private void OnDisable()
        {
            if (SystemInfo.supportsInstancing)
            {
                if (Application.isPlaying)
                {
                    if (DrawInstance.onDrawInstance != null)
                    {
                        DrawInstance.onDrawInstance -= Execute;
                    }

                    if (argBuffer != null)
                    {
                        for (int i = 0; i < argBuffer.Count; ++i)
                        {
                            argBuffer[i].Release();
                        }
                        argBuffer.Clear();
                        argBuffer = null;
                    }
                    if (matrixBuffer != null)
                    {
                        for (int i = 0; i < matrixBuffer.Count; ++i)
                        {
                            if (matrixBuffer[i] != null)
                            {
                                matrixBuffer[i].Release();
                            }
                        }
                        matrixBuffer.Clear();
                        matrixBuffer = null;
                    }
                }

                RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            }
        }
        private void OnDestroy()
        {
            Dispose();
        }

#if UNITY_EDITOR && SCENE_DISPLAY
        private void OnDrawGizmos()
        {
            if (bDrawGizmos && mQTree != null)
            {
                mQTree.DrawGizmos(nDrawMip);
            }
        }
#endif
        public void Dispose()
        {
#if UNITY_EDITOR && SCENE_DISPLAY
            if (mHeightMaps.IsCreated)
            {
                mHeightMaps.Dispose();
            }
            if (mPositionOffsets.IsCreated)
            {
                mPositionOffsets.Dispose();
            }
#endif
            if (mQTree != null)
            {
                mQTree.Dispose();
            }
        }

    }
}