using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GrassShaderMaterial : MonoBehaviour
{
    public static int _BaseColor = Shader.PropertyToID("_BaseColor");

    private Material oldMaterial;
    public Material mMaterial;
    public Color color;
    private Color oldColor;

    private void Update()
    {
        if (mMaterial == null)
            return;

        if(oldMaterial != mMaterial)
        {
            oldMaterial = mMaterial;
            color = oldColor = mMaterial.GetColor(_BaseColor);
        }

        if (color.Equals(oldColor))
            return;

        mMaterial.SetColor(_BaseColor, color);
        oldColor = color;
    }
}
