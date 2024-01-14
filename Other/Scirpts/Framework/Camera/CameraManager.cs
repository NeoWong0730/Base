using Framework;
using Lib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Framework
{
    public static class CameraManager
    {
        private static bool renderShadow = false;
        private const int nLowQualityMask = (int)(ELayerMask.Grass | ELayerMask.Build | ELayerMask.Terrain | ELayerMask.Water | ELayerMask.Tree | ELayerMask.SmallObject | ELayerMask.TransparentFX);
        private static int nLayerMask;

        public static bool bPostProcessSetting { get; private set; }
        public static int nReduceMainCameraQualityRef { get; private set; }
        public static int nHideCameraRef { get; private set; }
        public static Camera mCamera { get; private set; }
        public static Camera mSkillPlayCamera { get; private set; }
        public static Camera mUICamera { get; private set; }
        public static Vector2 relativePos;
        public static bool b_MatchWitchHeight;

        public static void CheckCamera()
        {
            if (Camera.main == mCamera)
                return;

            if (Camera.main == null && mCamera != null && mCamera.CompareTag(Tags.MainCamera))
                return;

            mCamera = Camera.main;
            nLayerMask = mCamera.cullingMask;
            UniversalAdditionalCameraData cameraData = mCamera.GetUniversalAdditionalCameraData();
            if (cameraData)
            {
                Camera[] cameras = Camera.allCameras;
                for (int i = 0; i < cameras.Length; ++i)
                {
                    if (cameras[i].CompareTag(Tags.UICamera))
                    {
                        cameras[i].GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
                        cameraData.cameraStack.AddOnce<Camera>(cameras[i]);
                    }
                }
            }

            AutoCameraStack cameraStack;
            if (!mCamera.TryGetComponent<AutoCameraStack>(out cameraStack))
            {
                cameraStack = mCamera.gameObject.AddComponent<AutoCameraStack>();
                cameraStack.Set(0, 0, new List<string>() { "UICamera" });
            }

            SetMainCameraEnable(nHideCameraRef <= 0);
            RefreshMainCameraQuality();
            RefreshShadow();

            if (onCameraChange != null)
                onCameraChange();
        }

        public static Action onCameraChange;
        public static Action onCameraVisiableChange;

        public static void World2UI(GameObject go, Transform worldTrans, Camera c3, Camera c2)
        {
            if (go != null && worldTrans != null && c3 != null && c2 != null)
            {
                World2UI(go, worldTrans.position, c3, c2);
            }
        }

        public static Vector3 GetUIPositionByWorldPosition(Vector3 worldPosition, Camera c3, Camera c2)
        {
            Vector3 position = Vector3.zero;
            if (c3 != null && c2 != null)
            {
                Vector2 screenPosition = c3.WorldToScreenPoint(worldPosition);
                position = c2.ScreenToWorldPoint(screenPosition);
            }
            return position;
        }

        public static void World2UI(GameObject go, Vector3 worldPosition, Camera c3, Camera c2)
        {
            if (go != null && c3 != null && c2 != null)
            {
                Vector2 screenPosition = c3.WorldToScreenPoint(worldPosition);
                Screen2UI(go, screenPosition, c2);
            }
        }

        public static void World2UI(GameObject uigo, Vector3 worldPosition, Camera c3, Camera c2, float targetUiwidth, float targetUiheight, float xoffset, float yoffset)
        {
            if (uigo != null && c3 != null && c2 != null)
            {
                World2UI(uigo, worldPosition, c3, c2);
                RectTransform rectTransform = uigo.transform as RectTransform;
                float scale = 0;
                if (b_MatchWitchHeight)
                {
                    scale = Screen.height / 720f;
                }
                else
                {
                    scale = Screen.width / 1280f;
                }
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x * scale + xoffset,
                    rectTransform.anchoredPosition.y * scale + yoffset);
            }
        }

        public static void Screen2UI(GameObject go, Vector3 screenPosition, Camera c2)
        {
            if (go != null && c2 != null)
            {
                Vector3 uiPosition = c2.ScreenToWorldPoint(screenPosition);
                go.transform.position = new Vector3(uiPosition.x, uiPosition.y, uiPosition.z);
            }
        }

        public static void Hide()
        {
            ++nHideCameraRef;
            if (nHideCameraRef == 1)
            {
                SetMainCameraEnable(false);
            }
        }

        public static void CancelHide()
        {
            if (nHideCameraRef <= 0)
            {
                DebugUtil.LogErrorFormat("CancelHide camera request count already equals {0}, check logic", nHideCameraRef);
                nHideCameraRef = 0;
            }
            else
            {
                --nHideCameraRef;
                if (nHideCameraRef == 0)
                {
                    SetMainCameraEnable(true);
                }
            }
        }

        public static void ReduceMainCameraQuality()
        {
            ++nReduceMainCameraQualityRef;
            if (nReduceMainCameraQualityRef == 1)
            {
                RefreshMainCameraQuality();
                RefreshShadow();
            }
        }

        public static void CancelReduceMainCameraQuality()
        {
            if (nReduceMainCameraQualityRef <= 0)
            {
                DebugUtil.LogErrorFormat("CancelHide camera request count already equals {0}, check logic", nReduceMainCameraQualityRef);
                nReduceMainCameraQualityRef = 0;
            }
            else
            {
                --nReduceMainCameraQualityRef;
                if (nReduceMainCameraQualityRef == 0)
                {
                    RefreshMainCameraQuality();
                    RefreshShadow();
                }
            }
        }

        public static void SetActiveShadow(bool active)
        {
            renderShadow = active;
            RefreshShadow();
        }

        private static void RefreshShadow()
        {
            if (mCamera)
            {
                UniversalAdditionalCameraData additionalCameraData = mCamera.gameObject.GetOrAddComponent<UniversalAdditionalCameraData>();
                additionalCameraData.renderShadows = renderShadow && nReduceMainCameraQualityRef <= 0;
            }
        }

        public static void SetActivePostProcess(bool active)
        {
            bPostProcessSetting = active;
            RefreshPostProcessing();
        }

        public static void SetMainCameraEnable(bool enable)
        {
            if (mCamera != null)
            {
                mCamera.gameObject.SetActive(enable);
                if (onCameraVisiableChange != null)
                    onCameraVisiableChange();
            }
        }

        public static int GetMainCameraCullingMask()
        {
            return nLayerMask;
        }

        public static void SetMainCameraCullingMask(int cullingMask)
        {
            nLayerMask = cullingMask;
            RefreshMainCameraQuality();
        }

        public static void RefreshMainCameraQuality()
        {
            if (mCamera)
            {
                if (nReduceMainCameraQualityRef > 0)
                {
                    mCamera.cullingMask = nLowQualityMask & nLayerMask;
                    RenderExtensionSetting.bUsageGaussianBlur = true;
                }
                else
                {
                    mCamera.cullingMask = nLayerMask;
                    RenderExtensionSetting.bUsageGaussianBlur = false;
                }
                RefreshPostProcessing();
            }
        }

        public static void SetSkillPlayCamera(Camera camera)
        {
            mSkillPlayCamera = camera;
        }

        public static void SetUICamera(Camera camera)
        {
            mUICamera = camera;
        }

        private static void RefreshPostProcessing()
        {
            if (mCamera)
            {
                mCamera.GetUniversalAdditionalCameraData().renderPostProcessing = bPostProcessSetting && nReduceMainCameraQualityRef <= 0;
            }
        }
    }
}
