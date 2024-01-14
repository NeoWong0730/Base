using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ModelTint : MonoBehaviour
{    
    public Color ColorA;
    public Material mMaterial;

    [Range(0, 1)]
    public float H;
    [Range(-1, 1)]
    public float S;
    [Range(-1, 1)]
    public float V;

    [Range(0, 1)]
    public float H2;
    [Range(-1, 1)]
    public float S2;
    [Range(-1, 1)]
    public float V2;

    private void Awake()
    {        
        mMaterial = GetComponent<Renderer>().sharedMaterial;
        ColorA = mMaterial.GetColor("_ColorA");
    }

    void Update()
    {
        Color baseColor = mMaterial.GetColor("_BaseColor");
        mMaterial.SetColor("_ColorA", ColorA);
        Color.RGBToHSV(ColorA * baseColor, out float h, out float s, out float v);

        float h1 = h;
        if(h1 > (1 / 6f) && h1 < (2 / 3f))
        {
            h1 += H;
        }
        else
        {
            h1 -= H;
            if(h1 < 0)
            {
                h1 += 1;
            }
        }

        float s1 = s + S;
        float v1 = v + V;

        
        mMaterial.SetColor("_1st_ShadeColor", Color.HSVToRGB(h1, s1, v1));

        float h2 = h;
        if (h2 > (1 / 6f) && h2 < (2 / 3f))
        {
            h2 += H2;
        }
        else
        {
            h2 -= H2;
            if (h2 < 0)
            {
                h2 += 1;
            }
        }

        float s2 = s + S2;
        float v2 = v + V2;

        
        mMaterial.SetColor("_2nd_ShadeColor", Color.HSVToRGB(h2, s2, v2));
    }
}
