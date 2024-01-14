using Lib.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Framework
{
    [ExecuteInEditMode]
    public class CP_CameraGrabTexture : MonoBehaviour
    {
        //当前相机 （每次只同时存在一个相机的抓图）
        private Camera targetCamera = null;

        //纪录镜像纹理的引用
        //private Dictionary<Vector4, int> nMirrorRef = new Dictionary<Vector4, int>(1);
        Vector4 vPlane;
        int nMirrorRefCount = 0;

        //纪录截屏纹理的引用
        private Dictionary<CameraEvent, int> mGrabRef = new Dictionary<CameraEvent, int>(4);

        private Dictionary<CameraEvent, CommandBuffer> mCommandBuffers = new Dictionary<CameraEvent, CommandBuffer>(4);

        private void Awake()
        {
            this.hideFlags = HideFlags.DontSave;
            targetCamera = GetComponent<Camera>();
        }

        public void RetainMirror(Vector4 plane)
        {
            if (nMirrorRefCount == 0)
            {
                vPlane = plane;
                ++nMirrorRefCount;
            }
            else
            {
                if (Vector4.Equals(vPlane, plane))
                {
                    ++nMirrorRefCount;
                }
                else
                {
                    DebugUtil.LogErrorFormat("最多支持一个平面反射");
                }
            }
        }

        public void ReleaseMirror(Vector4 plane)
        {
            if (nMirrorRefCount > 0 && Vector4.Equals(vPlane, plane))
            {
                --nMirrorRefCount;
            }
        }

        public void RetainGrabTexture(CameraEvent cameraEvent)
        {
            int count = 0;
            mGrabRef.TryGetValue(cameraEvent, out count);
            if (count == 0)
            {
                AddCommandBuffer(cameraEvent);
            }
            mGrabRef[cameraEvent] = count + 1;
        }

        public void ReleaseGrabTexture(CameraEvent cameraEvent)
        {
            int count = 0;
            mGrabRef.TryGetValue(cameraEvent, out count);
            if (count > 0)
            {
                --count;
                if (count <= 0)
                {
                    RemoveCommandBuffer(cameraEvent);
                }
                mGrabRef[cameraEvent] = count;
            }
            else
            {
                Debug.LogErrorFormat("{0} 引用计数不对", cameraEvent.ToString());
            }
        }

        private void AddCommandBuffer(CameraEvent cameraEvent)
        {
            if (targetCamera == null)
                return;

            CommandBuffer commandBuffer = null;
            mCommandBuffers.TryGetValue(cameraEvent, out commandBuffer);

            if (commandBuffer == null)
            {
                commandBuffer = new CommandBuffer();

                string eventName = cameraEvent.ToString();
                commandBuffer.name = "Grab Screen " + eventName;
                int screenCopyID = Shader.PropertyToID("_GrabTex" + eventName);

                commandBuffer.GetTemporaryRT(screenCopyID, Screen.width, Screen.height, 0, FilterMode.Bilinear);
                commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);

                commandBuffer.SetGlobalTexture(screenCopyID, screenCopyID);

                mCommandBuffers[cameraEvent] = commandBuffer;
            }

            //存储的时候使用原始的event类型
            //实际执行的时候根据情况转换
            CameraEvent targetEvent = cameraEvent;
            if (cameraEvent == CameraEvent.AfterSkybox)
            {
                targetEvent = targetCamera.clearFlags == CameraClearFlags.Skybox ? CameraEvent.AfterSkybox : CameraEvent.AfterImageEffectsOpaque;
            }

            if (cameraEvent == CameraEvent.AfterSkybox || cameraEvent == CameraEvent.AfterImageEffectsOpaque)
            {
                //TODO: 替换成LWRP 自带的截屏
                targetCamera.AddCommandBuffer(targetEvent, commandBuffer);
            }
            else
            {
                targetCamera.AddCommandBuffer(targetEvent, commandBuffer);
            }            
        }

        private void RemoveCommandBuffer(CameraEvent cameraEvent)
        {
            if (targetCamera == null)
                return;

            CommandBuffer commandBuffer = null;
            if (mCommandBuffers.TryGetValue(cameraEvent, out commandBuffer))
            {
                //存储的时候使用原始的event类型
                //实际执行的时候根据情况转换
                CameraEvent targetEvent = cameraEvent;
                if (cameraEvent == CameraEvent.AfterSkybox)
                {
                    targetEvent = targetCamera.clearFlags == CameraClearFlags.Skybox ? CameraEvent.AfterSkybox : CameraEvent.AfterImageEffectsOpaque;
                }

                if(cameraEvent == CameraEvent.AfterSkybox || cameraEvent == CameraEvent.AfterImageEffectsOpaque)
                {
                    //TODO: 替换成LWRP 自带的截屏
                    targetCamera.RemoveCommandBuffer(targetEvent, commandBuffer);
                    mCommandBuffers.Remove(cameraEvent);
                }
                else
                {
                    targetCamera.RemoveCommandBuffer(targetEvent, commandBuffer);
                    mCommandBuffers.Remove(cameraEvent);
                }                                
            }
        }
    }

    public static class GrabTextureManager
    {
        public static void Retain(Camera camera, CameraEvent cameraEvent)
        {
            if (camera == null)
                return;
            CP_CameraGrabTexture cameraGrabTexture = camera.GetComponent<CP_CameraGrabTexture>();
            if (cameraGrabTexture == null)
            {
                cameraGrabTexture = camera.gameObject.AddComponent<CP_CameraGrabTexture>();
            }
            cameraGrabTexture.RetainGrabTexture(cameraEvent);
        }

        public static void Release(Camera camera, CameraEvent cameraEvent)
        {
            if (camera == null)
                return;
            CP_CameraGrabTexture cameraGrabTexture = camera.GetComponent<CP_CameraGrabTexture>();
            cameraGrabTexture?.ReleaseGrabTexture(cameraEvent);
        }
    }
}