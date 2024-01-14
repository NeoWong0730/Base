using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTextureSetting : ScriptableObject
{
    [SerializeField]
    public List<MaterialTextureTool.MaterialArea> MaterialValue = new List<MaterialTextureTool.MaterialArea>();
}
