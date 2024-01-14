using Animancer;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework
{
    public sealed class BoneDampingSolo : AnimancerAdditionPlayable
    {
        [SerializeField] public Transform _RootBone;
        [SerializeField] public Transform _EndBone;
        [SerializeField] public int _BoneCount = 1;

        public override void Play(AnimancerComponent animancer, ModelPart modelPart, Skeleton skeleton)
        {
            _Animancer = animancer;

            // Since we are about to use these values several times, we can shorten the following lines a bit by using constants:
            const Allocator Persistent = Allocator.Persistent;
            const NativeArrayOptions UninitializedMemory = NativeArrayOptions.UninitializedMemory;

            BoneDampingSoloJob _AnimationJob = new BoneDampingSoloJob()
            {
                jointHandles = new NativeArray<TransformStreamHandle>(_BoneCount, Persistent, UninitializedMemory),
                localPositions = new NativeArray<Vector3>(_BoneCount, Persistent, UninitializedMemory),
                localRotations = new NativeArray<Quaternion>(_BoneCount, Persistent, UninitializedMemory),
                positions = new NativeArray<Vector3>(_BoneCount, Persistent, UninitializedMemory),
                velocities = new NativeArray<Vector3>(_BoneCount, Persistent),
            };

            _DisposableAnimationJob = _AnimationJob;

            // Initialize the contents of the arrays for each bone.
            var animator = _Animancer.Animator;
            var bone = _EndBone;
            for (int i = _BoneCount - 1; i >= 0; i--)
            {
                _AnimationJob.jointHandles[i] = animator.BindStreamTransform(bone);
                _AnimationJob.localPositions[i] = bone.localPosition;
                _AnimationJob.localRotations[i] = bone.localRotation;
                _AnimationJob.positions[i] = bone.position;

                bone = bone.parent;
            }

            _AnimationJob.rootHandle = animator.BindStreamTransform(_RootBone);

            // Add the job to Animancer's output.
            _AnimationScriptPlayable = _Animancer.Playable.InsertOutputJob(_AnimationJob);            
        }
    }
}
