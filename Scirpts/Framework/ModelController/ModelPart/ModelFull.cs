using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelFull : MonoBehaviour
{
    public ModifyMaterial[] mModelMaterials;

    public void CancelOverrideMaterial(int index)
    {
        if (mModelMaterials == null)
            return;

        for (int i = 0; i < mModelMaterials.Length; ++i)
        {
            mModelMaterials[i].CancelOverrideMaterial(index);
        }
    }

    public void SetOverrideMaterial(int index, Material material)
    {
        if (mModelMaterials == null)
            return;

        for (int i = 0; i < mModelMaterials.Length; ++i)
        {
            mModelMaterials[i].SetOverrideMaterial(index, material);
        }
    }
}