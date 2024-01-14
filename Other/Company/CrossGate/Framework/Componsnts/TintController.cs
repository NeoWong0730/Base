using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TintController : MonoBehaviour
{
    public Renderer[] mRenderers;
    private MaterialPropertyBlock _materialPropertyBlock;

    public MaterialPropertyBlock Get()
    {
        return _materialPropertyBlock;
    }

    public MaterialPropertyBlock GetOrCreate()
    {
        if (_materialPropertyBlock == null)
            _materialPropertyBlock = new MaterialPropertyBlock();
        return _materialPropertyBlock;
    }

    public void Apply()
    {
        if (mRenderers == null)
            return;

        for (int i = 0; i < mRenderers.Length; ++i)
        {
            Renderer renderer = mRenderers[i];
            if (renderer)
            {
                renderer.SetPropertyBlock(_materialPropertyBlock, 0);
            }
        }
    }
}