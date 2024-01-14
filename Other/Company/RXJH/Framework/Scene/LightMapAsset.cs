using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightMapAsset : ScriptableObject
{
    public Texture2D[] lightmapColor;
    public Texture2D[] lightmapDir;
    public Texture2D[] shadowMask;
}