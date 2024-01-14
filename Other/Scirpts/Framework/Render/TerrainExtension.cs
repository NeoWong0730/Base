using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    [RequireComponent(typeof(Terrain))]
    public class TerrainExtension : MonoBehaviour
    {
        [Range(0, 1)]
        public float SnowMask1 = 1.0f;
        [Range(0, 1)]
        public float SnowMask2 = 1.0f;
        [Range(0, 1)]
        public float SnowMask3 = 1.0f;
        [Range(0, 1)]
        public float SnowMask4 = 1.0f;

        void Start()
        {
            Terrain terrain = GetComponent<Terrain>();
            Color color = new Color(SnowMask1, SnowMask2, SnowMask3, SnowMask4);
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            terrain.GetSplatMaterialPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_SnowStrength", color);
            terrain.SetSplatMaterialPropertyBlock(materialPropertyBlock);
        }

#if UNITY_EDITOR
        private void Update()
        {
            Terrain terrain = GetComponent<Terrain>();
            Color color = new Color(SnowMask1, SnowMask2, SnowMask3, SnowMask4);
            MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
            terrain.GetSplatMaterialPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor("_SnowStrength", color);
            terrain.SetSplatMaterialPropertyBlock(materialPropertyBlock);
        }
#endif
    }
}