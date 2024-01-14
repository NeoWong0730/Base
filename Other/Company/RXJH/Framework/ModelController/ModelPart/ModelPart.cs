using UnityEngine;
using Framework;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ModelPart : MonoBehaviour
{
    public ModelPartConfig mSkinConfig;
    public AnimancerAdditionPlayable[] mAnimancerAdditionPlayables;

    public void BindingSkeleton(Skeleton skeleton, Animancer.AnimancerComponent animancer)
    {
        SkinnedMeshRenderer skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer.rootBone == null)
        {
            Transform bone = skeleton.GetBoneByPath(mSkinConfig.mRootBoneName);
            skinnedMeshRenderer.rootBone = bone;
        }

        Transform[] bones = skinnedMeshRenderer.bones;
        string[] boneNames = mSkinConfig.mBoneNames;
        for (int i = 0; i < bones.Length; ++i)
        {
            if (bones[i] != null)
                continue;

            Transform bone = skeleton.GetBoneByPath(boneNames[i]);
            bones[i] = bone;
            if (!bone)
            {
                Debug.Log(boneNames[i]);
            }
        }
        skinnedMeshRenderer.bones = bones;
        skinnedMeshRenderer.ResetLocalBounds();

        PlayAdditionPlayables(animancer, skeleton);
    }

    public void PlayAdditionPlayables(Animancer.AnimancerComponent animancer, Skeleton skeleton)
    {
        if (mAnimancerAdditionPlayables == null)
            return;

        for (int i = 0; i < mAnimancerAdditionPlayables.Length; ++i)
        {
            mAnimancerAdditionPlayables[i].Play(animancer, this, skeleton);
        }
    }

    public void StopAdditionPlayables()
    {
        if (mAnimancerAdditionPlayables == null)
            return;

        for (int i = 0; i < mAnimancerAdditionPlayables.Length; ++i)
        {
            mAnimancerAdditionPlayables[i].Stop();
        }
    }

    private void OnDestroy()
    {
        StopAdditionPlayables();
    }

    public void CancelOverrideMaterial(int index)
    {
        if (gameObject.TryGetComponent<ModifyMaterial>(out ModifyMaterial modifyMaterial))
        {
            modifyMaterial.CancelOverrideMaterial(index);
        }
    }

    public void SetOverrideMaterial(int index, Material material)
    {
        if (material)
        {
            ModifyMaterial modifyMaterial = gameObject.GetOrAddComponent<ModifyMaterial>();
            modifyMaterial.SetOverrideMaterial(index, material);
        }
    }

    public Material GetMaterial(int index)
    {
        ModifyMaterial modifyMaterial = gameObject.GetOrAddComponent<ModifyMaterial>();
        return modifyMaterial.GetMaterial(index);
    }
}
