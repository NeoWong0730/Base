using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class CP_ClippableFx : MonoBehaviour {
    public UnityEngine.Rendering.CompareFunction compare = UnityEngine.Rendering.CompareFunction.Equal;
    public int stencilRefValue = 2;
    public bool forParticleSystem = true;
    [SerializeField] private ParticleSystemRenderer[] renderers;
    [SerializeField] private MeshRenderer[] meshRenderers;
    private readonly List<Material> mats = new List<Material>();

    private void Start() {
        // why dont work in ParticleSystemRenderer?
        // MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
        // materialPropertyBlock.SetFloat("_Stencil", stencilRefValue);
        // materialPropertyBlock.SetFloat("_StencilComp", (int)compare);

        if (this.forParticleSystem) {
            this.renderers = this.GetComponentsInChildren<ParticleSystemRenderer>(true);

            for (int i = 0, length = this.renderers.Length; i < length; ++i) {
                var mat = this.renderers[i].material;
                //var mat = new Material(this.renderers[i].sharedMaterial);
                //this.renderers[i].sharedMaterial = mat;
                mat.SetFloat("_Stencil", this.stencilRefValue);
                mat.SetFloat("_StencilComp", (int)this.compare);

                this.mats.Add(mat);
            }
        }
        else {
            this.meshRenderers = this.GetComponentsInChildren<MeshRenderer>(true);
            for (int i = 0, length = this.meshRenderers.Length; i < length; ++i) {
                var mat = this.meshRenderers[i].material;
                mat.SetFloat("_Stencil", this.stencilRefValue);
                mat.SetFloat("_StencilComp", (int)this.compare);

                this.mats.Add(mat);
            }
        }
    }

    private void OnDestroy() {
        if (Application.isEditor) {
            foreach (var mat in this.mats) {
                DestroyImmediate(mat);
            }
        }
        else {
            foreach (var mat in this.mats) {
                Destroy(mat);
            }
        }
    }
}
