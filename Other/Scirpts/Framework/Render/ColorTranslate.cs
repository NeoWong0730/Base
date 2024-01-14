using UnityEngine;

namespace Framework
{
    [ExecuteAlways]
    [RequireComponent(typeof(Renderer))]
    public class ColorTranslate : MonoBehaviour
    {
        static readonly int gBaseColorHash = Shader.PropertyToID("_BaseColor");

        [SerializeField]
        private Gradient mGradientColor = new Gradient();
        private Renderer mRenderer;
        private MaterialPropertyBlock mMaterialPropertyBlock;
        [SerializeField]
        private float fSpeed = 1;

        private void Start()
        {
            mRenderer = GetComponent<Renderer>();
            mMaterialPropertyBlock = new MaterialPropertyBlock();
            mRenderer.GetPropertyBlock(mMaterialPropertyBlock);
        }

        void Update()
        {
            float t = Time.unscaledTime * fSpeed;
            mMaterialPropertyBlock.SetColor(gBaseColorHash, mGradientColor.Evaluate(t - (int)t));
            mRenderer.SetPropertyBlock(mMaterialPropertyBlock, 0);
        }
    }
}