using Lib;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Framework
{
    public class QTree : IDisposable
    {
        public AABB mBounds;
        public int3 vSize;

        private NativeArray<Unity.Rendering.FrustumPlanes.IntersectResult>[] _results;
        private NativeArray<float4> _planes;

        private Matrix4x4 _projectionMatrixRecode;


        /// <summary>
        /// 求位置对应的内存索引
        /// 
        /// 位置对应的内存排布
        /// 10 11 14 15
        /// 8  9  12 13
        /// 2  3  6  7
        /// 0  1  4  5
        /// </summary>
        /// <param name="pos">空间位置</param>
        /// <returns>内存索引</returns>

        [BurstCompile]
        public static int PosToTreeNodeIndex(int2 pos)
        {
            if (pos.x == 0 && pos.y == 0)
                return 0;

            int2 s = pos % 2;
            int index = s.y * 2 + s.x;
            return index + PosToTreeNodeIndex(pos / 2) * 4;
        }

        /// <summary>
        /// 求内存索引对应的位置
        /// 
        /// 位置对应的内存排布
        /// 10 11 14 15
        /// 8  9  12 13
        /// 2  3  6  7
        /// 0  1  4  5
        /// </summary>
        /// <param name="index">内存索引</param>
        /// <returns>空间位置</returns>
        [BurstCompile]
        public static int2 TreeNodeIndexToPos(int index)
        {
            return _TreeNodeIndexToPos(index, 1).xy;
        }

        private static int3 _TreeNodeIndexToPos(int index, int l)
        {
            int3 pos = 0;
            if (index >= (l << 2))
            {
                int3 parent = _TreeNodeIndexToPos(index, l << 2);

                pos.z = parent.z % l;

                int s = parent.z / l;
                pos.x = s % 2 + parent.x * 2;
                pos.y = s / 2 + parent.y * 2;

            }
            else
            {
                pos.z = index % l;

                int s = index / l;
                pos.x = s % 2;
                pos.y = s / 2;
            }

            return pos;
        }

        /// <summary>
        /// 内存的索引对应空间坐标
        /// 
        /// 内存的布局
        /// 12 13 14 15
        /// 8  9 10 11
        /// 4  5  6  7 
        /// 0  1  2  3 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [BurstCompile]
        public static int2 OrderIndexToPos(int index, int2 size)
        {
            int2 pos = 0;
            pos.x = index % size.x;
            pos.y = index / size.x;
            return pos;
        }

        /// <summary>
        /// 空间坐标对应内存的索引
        /// 
        /// 内存的布局
        /// 12 13 14 15
        /// 8  9 10 11
        /// 4  5  6  7 
        /// 0  1  2  3 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [BurstCompile]
        public static int PosToOrderIndex(int2 pos, int2 size)
        {
            return pos.y * size.x + pos.x;
        }

        public static int TreeNodeIndexToOrderIndex(int index, int2 size)
        {
            return PosToOrderIndex(TreeNodeIndexToPos(index), size);
        }

        public static int OrderIndexToTreeNodeIndex(int index, int2 size)
        {
            return PosToTreeNodeIndex(OrderIndexToPos(index, size));
        }

        [BurstCompile]
        public void SetSize(AABB bounds, int3 gridSize, int mip)
        {
            bool3 isPow2 = math.ispow2(gridSize);

            if (!isPow2.x || !isPow2.y || !isPow2.z)
            {
                DebugUtil.LogErrorFormat("gridSize {0} is not pow 2", gridSize);
                return;
            }

            mBounds = bounds;

            if (!vSize.Equals(gridSize) || _results == null || mip != _results.Length)
            {
                vSize = gridSize;
                jobHandle.Complete();
                DisposeNativeArray();
                if (_results == null || mip != _results.Length)
                {
                    _results = new NativeArray<Unity.Rendering.FrustumPlanes.IntersectResult>[mip];
                }
                for (int i = 0; i < mip; ++i)
                {
                    int3 size = math.max(vSize >> i, 1);
                    int count = size.x * size.y * size.z;
                    _results[i] = new NativeArray<Unity.Rendering.FrustumPlanes.IntersectResult>(count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
                }
            }
        }

        private JobHandle jobHandle;
        [BurstCompile]
        public void AsyncCull(Camera camera, float distance)
        {
            float farClipPlane = camera.farClipPlane;
            camera.farClipPlane = math.min(farClipPlane, distance);

            //优化相机视口没变化时 不重新计算裁剪
            Matrix4x4 projectionMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
            if (_projectionMatrixRecode.Equals(projectionMatrix))
            {
                camera.farClipPlane = farClipPlane;
                return;
            }

            jobHandle.Complete();

            _projectionMatrixRecode = projectionMatrix;

            if (!_planes.IsCreated)
            {
                _planes = new NativeArray<float4>(6, Allocator.Persistent);
            }
            Unity.Rendering.FrustumPlanes.FromCamera(camera, _planes);

            camera.farClipPlane = farClipPlane;

            int3 parentSize = int3.zero;

            for (int i = _results.Length - 1; i >= 0; --i)
            {
                int3 size = math.max(vSize >> i, 1);

                AABB aabb = new AABB();
                aabb.Extents = mBounds.Extents / size;
                aabb.Center = mBounds.Min + aabb.Extents;

                if (i == _results.Length - 1)
                {
                    QTreeCullJobNoParent job = new QTreeCullJobNoParent()
                    {
                        GridSize = size,
                        Bounds = aabb,
                        Planes = _planes,
                        Result = _results[i],
                    };
                    jobHandle = job.Schedule<QTreeCullJobNoParent>(_results[i].Length, 32);
                }
                else
                {
                    QTreeCullJob job = new QTreeCullJob()
                    {
                        ParentGridSize = parentSize,
                        GridSize = size,
                        Bounds = aabb,
                        Planes = _planes,
                        ParentResult = _results[i + 1],
                        Result = _results[i],
                    };
                    jobHandle = job.Schedule<QTreeCullJob>(_results[i].Length, 32, jobHandle);
                }

                parentSize = size;
            }
        }

        public void Complete()
        {
            jobHandle.Complete();
        }

        [BurstCompile]
        public void Cull(Camera camera, float distance)
        {
            float farClipPlane = camera.farClipPlane;
            camera.farClipPlane = math.min(farClipPlane, distance);

            //优化相机视口没变化时 不重新计算裁剪
            Matrix4x4 projectionMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
            if (_projectionMatrixRecode.Equals(projectionMatrix))
            {
                camera.farClipPlane = farClipPlane;
                return;
            }

            jobHandle.Complete();

            _projectionMatrixRecode = projectionMatrix;

            if (!_planes.IsCreated)
            {
                _planes = new NativeArray<float4>(6, Allocator.Persistent);
            }
            Unity.Rendering.FrustumPlanes.FromCamera(camera, _planes);

            camera.farClipPlane = farClipPlane;

            int3 parentSize = int3.zero;
            //JobHandle jobHandle = default;
            for (int i = _results.Length - 1; i >= 0; --i)
            {
                int3 size = math.max(vSize >> i, 1);

                AABB aabb = new AABB();
                aabb.Extents = mBounds.Extents / size;
                aabb.Center = mBounds.Min + aabb.Extents;

                if (i == _results.Length - 1)
                {
                    QTreeCullJobNoParent job = new QTreeCullJobNoParent()
                    {
                        GridSize = size,
                        Bounds = aabb,
                        Planes = _planes,
                        Result = _results[i],
                    };
                    jobHandle = job.Schedule<QTreeCullJobNoParent>(_results[i].Length, 32);
                }
                else
                {
                    QTreeCullJob job = new QTreeCullJob()
                    {
                        ParentGridSize = parentSize,
                        GridSize = size,
                        Bounds = aabb,
                        Planes = _planes,
                        ParentResult = _results[i + 1],
                        Result = _results[i],
                    };
                    jobHandle = job.Schedule<QTreeCullJob>(_results[i].Length, 32, jobHandle);
                }

                parentSize = size;
            }
            jobHandle.Complete();
        }
        public void DrawGizmos(int mip)
        {
            jobHandle.Complete();
            mip = math.clamp(mip, 0, _results.Length - 1);
            NativeArray<Unity.Rendering.FrustumPlanes.IntersectResult> results = _results[mip];

            int3 size = math.max(vSize >> mip, 1);

            AABB aabb = new AABB();
            aabb.Extents = mBounds.Extents / size;
            aabb.Center = mBounds.Min + aabb.Extents;

            int resultsLength = results.Length;

            for (int i = 0; i < resultsLength; ++i)
            {
                if (results[i] == Unity.Rendering.FrustumPlanes.IntersectResult.Out)
                {
                    Gizmos.color = Color.red;
                }
                else if (results[i] == Unity.Rendering.FrustumPlanes.IntersectResult.Partial)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.white;
                }

                int3 pos = GetNodePos(i, mip);

                Gizmos.DrawWireCube(pos * aabb.Size + aabb.Center, aabb.Size);
                Gizmos.color = Color.white;
            }
        }
        public void Dispose()
        {
            jobHandle.Complete();
            if (_planes.IsCreated)
            {
                _planes.Dispose();
            }
            DisposeNativeArray();
        }

        private void DisposeNativeArray()
        {
            if (_results != null)
            {
                for (int i = 0; i < _results.Length; ++i)
                {
                    if (_results[i].IsCreated)
                    {
                        _results[i].Dispose();
                    }
                }
            }
        }
        [BurstCompile]
        public int3 GetNodePos(int index, int mip)
        {
            int3 size = vSize >> mip;
            int3 pos = 0;
            pos.z = index / size.x;
            pos.x = index - pos.z * size.x;
            return pos;
        }
        public Unity.Rendering.FrustumPlanes.IntersectResult GetIntersectResult(int index, int mip = 0)
        {
            return _results[mip][index];
        }
        public int GetGridSize(int mip)
        {
            return _results[mip].Length;
        }
        [BurstCompile]
        public int GetIndex(int3 index)
        {
            return index.y * vSize.z * vSize.x + index.z * vSize.x + index.x;
        }
        public bool CheckVisibilityByIndex(int index)
        {
            if (GetIntersectResult(index) != Unity.Rendering.FrustumPlanes.IntersectResult.Out)
                return true;
            return false;
        }
        [BurstCompile]
        public int GetIndexByPosition(float3 pos)
        {
            float3 d = (pos - mBounds.Min);
            int3 index = (int3)(d / mBounds.Size * vSize);
            index = math.max(index, 0);
            index = math.min(index, vSize - 1);
            return GetIndex(index);
        }
        [BurstCompile]
        public bool CheckVisibility(AABB aabb)
        {
            int3 minPos = (int3)((aabb.Min - mBounds.Min) / mBounds.Size);
            int3 maxPos = (int3)((aabb.Min - mBounds.Min) / mBounds.Size);
            for (int y = minPos.y; y < maxPos.y; ++y)
            {
                for (int z = minPos.z; z < maxPos.z; ++z)
                {
                    for (int x = minPos.x; x < maxPos.x; ++x)
                    {
                        if (GetIntersectResult(GetIndex(new int3(x, y, z))) != Unity.Rendering.FrustumPlanes.IntersectResult.Out)
                            return true;
                    }
                }
            }
            return false;
        }
        [BurstCompile]
        struct QTreeCullJob : IJobParallelFor
        {
            /// <summary>
            /// 上一级mip的网格尺寸
            /// </summary>
            [ReadOnly] public int3 ParentGridSize;
            /// <summary>
            /// 当前mip的网格尺寸
            /// </summary>
            [ReadOnly] public int3 GridSize;
            /// <summary>
            /// 单个AABB的大小, (0,0,0)位置的包围盒信息
            /// </summary>
            [ReadOnly] public AABB Bounds;

            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<float4> Planes;
            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<Unity.Rendering.FrustumPlanes.IntersectResult> ParentResult;
            [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<Unity.Rendering.FrustumPlanes.IntersectResult> Result;

            public void Execute(int index)
            {
                int3 pos = 0;
                pos.z = index / GridSize.x;
                pos.x = index - pos.z * GridSize.x;

                int parent = (pos.z / 2) * ParentGridSize.x + pos.x / 2;

                Unity.Rendering.FrustumPlanes.IntersectResult parentResult = ParentResult[parent];

                if (parentResult == Unity.Rendering.FrustumPlanes.IntersectResult.Partial)
                {
                    AABB aabb = Bounds;
                    aabb.Center = aabb.Center + pos * aabb.Size;
                    Result[index] = Unity.Rendering.FrustumPlanes.Intersect(Planes, aabb);
                }
                else
                {
                    Result[index] = parentResult;
                }
            }
        }

        [BurstCompile]
        struct QTreeCullJobNoParent : IJobParallelFor
        {
            [ReadOnly] public int3 GridSize;
            /// <summary>
            /// 单个AABB的大小, (0,0,0)位置的包围盒信息
            /// </summary>
            [ReadOnly] public AABB Bounds;

            [NativeDisableParallelForRestriction] [ReadOnly] public NativeArray<float4> Planes;
            [NativeDisableParallelForRestriction] [WriteOnly] public NativeArray<Unity.Rendering.FrustumPlanes.IntersectResult> Result;

            public void Execute(int index)
            {
                int3 pos = 0;
                pos.z = index / GridSize.x;
                pos.x = index - pos.z * GridSize.x;

                AABB aabb = Bounds;
                aabb.Center = aabb.Center + pos * aabb.Size;
                Result[index] = Unity.Rendering.FrustumPlanes.Intersect(Planes, aabb);
            }
        }
    }
}