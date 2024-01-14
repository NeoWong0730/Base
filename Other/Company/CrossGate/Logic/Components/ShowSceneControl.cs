using Framework;
using UnityEngine;
using Lib.Core;

namespace Logic
{
    public class ShowSceneControl
    {
        public GameObject mRoot { get; private set; }
        public VirtualGameObject mModelPos { get; private set; } = new VirtualGameObject();
        public Camera mCamera { get; private set; }

        public GameObject Pos;

        //public GameObject Bg;

        private int nWidth = 0;
        private int nHeight = 0;
        private int nDepthBuffer = 0;
        private RenderTextureFormat eRenderTextureFormat = RenderTextureFormat.Default;

        private RenderTexture mRenderTexture;
#if UNITY_EDITOR
        EditorDialogueActor editorDialogueActor;
#endif
        public void Parse(GameObject go)
        {
            mRoot = go;
            mCamera = go.transform.Find("Camera").GetComponent<Camera>();
            Pos = go.transform.Find("Pos").gameObject;
            mModelPos.SetGameObject(Pos, true);
            //Bg = go.transform.Find("bg").gameObject;
#if UNITY_EDITOR
            editorDialogueActor = new EditorDialogueActor();
            editorDialogueActor.Add(mRoot, Pos);
#endif
        }

        public void Dispose()
        {
            ReleaseTemporary();
            mModelPos.Dispose();
            mCamera = null;
            //Bg = null;
            if (mRoot != null)
            {
                UnityEngine.Object.Destroy(mRoot);
                mRoot = null;
            }

#if UNITY_EDITOR
            editorDialogueActor.Dispose();
#endif
        }

        /// <summary>
        /// 获取相机绘制的RenderTexture
        /// </summary>
        /// <param name="scale">像素等比缩放</param>
        /// <returns></returns>
        public RenderTexture GetTemporary(int width, int height, int depthBuffer, RenderTextureFormat format, float scale, bool useSetting = true, int antialiasing = 0)
        {
            if (useSetting)
            {
                ShowSceneSetting showSceneSetting = mCamera.GetComponent<ShowSceneSetting>();
                if (showSceneSetting)
                {
                    width = showSceneSetting.vResolution.x;
                    height = showSceneSetting.vResolution.y;
                    scale = showSceneSetting.fScale;                    

                    //if (width == 2048 || height == 2048)
                    //{
                    //    width >>= 1;
                    //    height >>= 1;
                    //}
                }
            }

            //TODO 应该是第一个写错了 16写错成6 后面的小伙伴都复制成6了
            //if (depthBuffer == 6 || depthBuffer == 0)
            //{
            //    depthBuffer = 16;
            //}

            depthBuffer = 24;

            float realScale = Mathf.Clamp(scale, 0.125f, 8f);

            width = width > 0 ? width : Screen.width;
            height = height > 0 ? height : Screen.height;

            width = (int)(width * realScale);
            height = (int)(height * realScale);

            if (mRenderTexture == null || nWidth != width || nHeight != height
                || format != eRenderTextureFormat
                || depthBuffer != nDepthBuffer)
            {
                ReleaseTemporary();

                nWidth = width;
                nHeight = height;
                nDepthBuffer = depthBuffer;
                eRenderTextureFormat = format;

                //TODO 值为0时自动调整
                if (antialiasing == 0)
                {
                    int antiAliasingLevel = Core.OptionManager.Instance.GetInt(Core.OptionManager.EOptionID.SceneScale, true);
                    antialiasing = 1 << antiAliasingLevel;
                }
                mRenderTexture = RenderTexture.GetTemporary(nWidth, nHeight, nDepthBuffer, eRenderTextureFormat, RenderTextureReadWrite.Default, antialiasing, RenderTextureMemoryless.MSAA);

                mCamera.targetTexture = mRenderTexture;
            }

            return mRenderTexture;
        }

        public void ReleaseTemporary()
        {
            if (mRenderTexture != null)
            {
                if (mCamera != null)
                    mCamera.targetTexture = null;
                RenderTexture.ReleaseTemporary(mRenderTexture);
                mRenderTexture = null;
            }
        }

        private void OnDisable()
        {
            ReleaseTemporary();
        }
    }
}
