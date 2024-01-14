using Animancer;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;

namespace Framework
{
    public sealed class BoneDampingMulti : AnimancerAdditionPlayable
    {        
        [SerializeField] public Transform[] _RootBones;
        [SerializeField] public int _BoneCount = 1;

        private int SetDampingJobData(List<Transform> bones, int start, int end)
        {
            for (int i = start; i < end; ++i)
            {
                int childCount = bones[i].childCount;
                for (int j = 0; j < childCount; ++j)
                {
                    bones.Add(bones[i].GetChild(j));
                }
            }

            return end;
        }

        public override void Play(AnimancerComponent animancer, ModelPart modelPart, Skeleton skeleton)
        {
            _Animancer = animancer;

            int start = 0;
            List<Transform> bones = new List<Transform>(_BoneCount);
            bones.AddRange(_RootBones);
            while (bones.Count > start)
            {
                start = SetDampingJobData(bones, start, bones.Count);
            }

            _BoneCount = bones.Count;

            // Since we are about to use these values several times, we can shorten the following lines a bit by using constants:
            const Allocator Persistent = Allocator.Persistent;
            const NativeArrayOptions UninitializedMemory = NativeArrayOptions.UninitializedMemory;

            NativeArray<Unity.Mathematics.int3> jointInfos = new NativeArray<Unity.Mathematics.int3>(_BoneCount, Persistent, UninitializedMemory);
            NativeArray<TransformStreamHandle> jointHandles = new NativeArray<TransformStreamHandle>(_BoneCount, Persistent, UninitializedMemory);
            NativeArray<Vector3> localPositions = new NativeArray<Vector3>(_BoneCount, Persistent, UninitializedMemory);
            NativeArray<Quaternion> localRotations = new NativeArray<Quaternion>(_BoneCount, Persistent, UninitializedMemory);
            NativeArray<Vector3> positions = new NativeArray<Vector3>(_BoneCount, Persistent, UninitializedMemory);
            NativeArray<Quaternion> rotations = new NativeArray<Quaternion>(_BoneCount, Persistent, UninitializedMemory);
            NativeArray<Vector3> velocities = new NativeArray<Vector3>(_BoneCount, Persistent);

            // Initialize the contents of the arrays for each bone.
            var animator = _Animancer.Animator;

            int childStart = _RootBones.Length;
            for (int i = 0; i < _BoneCount; ++i)
            {
                Transform bone = bones[i];

                jointInfos[i] = new Unity.Mathematics.int3(-1, childStart, bone.childCount);
                jointHandles[i] = animator.BindStreamTransform(bone);
                localPositions[i] = bone.localPosition;
                localRotations[i] = bone.localRotation;
                positions[i] = bone.position;

                childStart += bone.childCount;
            }

            for (int i = 0; i < _BoneCount; ++i)
            {
                Unity.Mathematics.int3 jointInfo = jointInfos[i];

                //Unity.Collections.LowLevel.Unsafe.UnsafeUtility.WriteArrayElementWithStride(jointInfos, jointInfo.y, Unity.Collections.LowLevel.Unsafe.UnsafeUtility.SizeOf<Unity.Mathematics.int3>(), i);

                for(int j = jointInfo.y; j < jointInfo.y + jointInfo.z; ++j)
                {
                    Unity.Mathematics.int3 childJointInfo = jointInfos[j];
                    childJointInfo.x = i;
                    jointInfos[j] = childJointInfo;
                }
            }

            BoneDampingMultiJob _AnimationJob = new BoneDampingMultiJob()
            {
                jointInfos = jointInfos,
                jointHandles = jointHandles,
                localPositions = localPositions,
                localRotations = localRotations,
                positions = positions,
                rotations = rotations,
                velocities = velocities,
            };

            _DisposableAnimationJob = _AnimationJob;

            // Add the job to Animancer's output.
            _AnimationScriptPlayable = _Animancer.Playable.InsertOutputJob(_AnimationJob);            
        }
    }
}
