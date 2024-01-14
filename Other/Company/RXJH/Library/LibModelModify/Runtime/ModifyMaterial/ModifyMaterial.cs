using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkinnedMeshRenderer))]
public class ModifyMaterial : MonoBehaviour
{
    [SerializeField]
    private Renderer mRenderer;
    /// <summary>
    /// 记录原始材质
    /// </summary>
    [SerializeField]
    private Material[] mSharedMaterials;

    [SerializeField]private bool bInit;

    private void _Init()
    {
        if (bInit)
            return;

        bInit = true;
        mRenderer = GetComponent<Renderer>();
        mSharedMaterials = new Material[mRenderer.sharedMaterials.Length];
        for (int i = 0; i < mRenderer.sharedMaterials.Length; ++i)
        {
            mSharedMaterials[i] = mRenderer.sharedMaterials[i];
        }
    }

    public int GetCount()
    {
        _Init();
        return mSharedMaterials.Length;
    }

    public Material GetMaterial(int index)
    {
        _Init();
        return mSharedMaterials[index];
    }

    public void SetMaterial(Material material)
    {
        _Init();
        if (mRenderer.sharedMaterial == mSharedMaterials[0])
        {
            mRenderer.sharedMaterial = mSharedMaterials[0] = material;
        }
        else
        {
            mSharedMaterials[0] = material;
        }
    }

    public void SetMaterial(int index, Material material)
    {
        _Init();
        if (index == 0)
        {
            SetMaterial(material);
        }
        else
        {
            Material[] materials = mRenderer.sharedMaterials;
            if (materials[index] == mSharedMaterials[index])
            {
                materials[index] = mSharedMaterials[index] = material;
                mRenderer.sharedMaterials = materials;
            }
            else
            {
                mSharedMaterials[index] = material;
            }
        }
    }

    public void CancelOverrideMaterial(int index)
    {
        _Init();
        if (index < 0)
        {
            mRenderer.sharedMaterials = mSharedMaterials;
        }
        else
        {
            Material[] materials = mRenderer.sharedMaterials;
            materials[index] = mSharedMaterials[index];
            mRenderer.sharedMaterials = materials;
        }
    }

    public void SetOverrideMaterial(int index, Material material)
    {
        _Init();
        Material[] materials = mRenderer.sharedMaterials;
        if (index < 0)
        {            
            for (int i = 0; i < materials.Length; ++i)
            {
                materials[i] = material;
            }
        }
        else
        {
            materials[index] = material;
        }
        mRenderer.sharedMaterials = materials;
    }
}
