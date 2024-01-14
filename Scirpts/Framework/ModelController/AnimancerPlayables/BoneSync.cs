using Animancer;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class BoneSync : AnimancerAdditionPlayable
{        
    public Transform[] mRootBones;
    public string[] mRootBonePaths;

    public override void Play(AnimancerComponent animancer, ModelPart modelPart, Skeleton skeleton)
    {
        _Animancer = animancer;

        const Allocator Persistent = Allocator.Persistent;
        const NativeArrayOptions UninitializedMemory = NativeArrayOptions.UninitializedMemory;

        BoneSyncJob _AnimationJob = new BoneSyncJob()
        {
            SrcHandles = new NativeArray<TransformStreamHandle>(mRootBones.Length, Persistent, UninitializedMemory),
            DstHandles = new NativeArray<TransformStreamHandle>(mRootBones.Length, Persistent, UninitializedMemory),
        };

        _DisposableAnimationJob = _AnimationJob;

        var animator = _Animancer.Animator;
        for(int i = 0; i < mRootBones.Length; ++i)
        {
            Transform bone = skeleton.GetBoneByPath(mRootBonePaths[i]);
            if (bone)
            {
                _AnimationJob.SrcHandles[i] = animator.BindStreamTransform(bone);
            }
            _AnimationJob.DstHandles[i] = animator.BindStreamTransform(mRootBones[i]);

            mRootBones[i].SetPositionAndRotation(bone.position, bone.rotation);
        }

        // Add the job to Animancer's output.
        _AnimationScriptPlayable = _Animancer.Playable.InsertOutputJob(_AnimationJob);
    }
}
