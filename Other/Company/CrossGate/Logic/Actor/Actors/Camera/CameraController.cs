using DG.Tweening;
using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;
using Table;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System.Collections;
using Framework;
using System;

namespace Logic
{
    public enum ECameraType
    {
        Follow,        
        Fixed,
    }

    public struct CameraData
    {
        public float pith;
        public float yaw;
        public float roll;
        public float distance;
        public float fov;
        public float clipFar;
        public Vector3 lookPointOffset;
    }

    public class CameraController : Actor
    {
        private ECameraType eCameraType = ECameraType.Fixed;
        private AttitudeAngleTransform _virtualCamera;
        public AttitudeAngleTransform virtualCamera { get { return _virtualCamera; } }        
        private CameraData lastCameraData;
        private SceneActor followActor;
        public bool isCanRightJoystick = false;

        protected override void OnConstruct()
        {
            Sys_Input.Instance.onRightJoystick += OnRightJoystick;
            Sys_Input.Instance.onScale += OnScale;
        }

        private void OnScale(float dis)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            return;
#endif
            if (dis != 0)
            {
                virtualCamera.distance += dis;
            }
        }

        private void OnRightJoystick(Vector2 dir, float dis)
        {
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if(!isCanRightJoystick)
                return;
#endif
            float modifyX = -dir.y * dis;
            float modifyY = dir.x * dis;
            if (modifyX != 0 || modifyY != 0)
            {
                virtualCamera.pith += modifyX;
                virtualCamera.yaw += modifyY;
            }
        }
        
        public void EnterFight(Vector3 lookPoint, CSVBattleScene.Data cSVBattleSceneData)
        {
            eCameraType = ECameraType.Fixed;
            virtualCamera.autoFollowTarget = false;
            
            if (cSVBattleSceneData != null)
            {              
                virtualCamera.fov = cSVBattleSceneData.fov / 10000f;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
                if (AspectRotioController.Instance.curRatio == Enum_Ratio.Type_1)
                {                  
                    virtualCamera.distance = cSVBattleSceneData.distance[1] / 10000f;
                    virtualCamera.clipFar = cSVBattleSceneData.far[1] / 10000f;
                }
                else
                {
                    virtualCamera.distance = cSVBattleSceneData.distance[0] / 10000f;
                    virtualCamera.clipFar = cSVBattleSceneData.far[0] / 10000f;
                }
#else
                if (Screen.width == 2480 && Screen.height == 2200)
                {
                    virtualCamera.distance = cSVBattleSceneData.distance[2] / 10000f;
                    virtualCamera.clipFar = cSVBattleSceneData.far[2] / 10000f;
                }
                else if (Screen.width == 2208 && Screen.height == 1768)
                {
                    virtualCamera.distance = cSVBattleSceneData.distance[3] / 10000f;
                    virtualCamera.clipFar = cSVBattleSceneData.far[3] / 10000f;
                }
                else if (Screen.width == 2048 && Screen.height == 1536 || Screen.width == 2224 && Screen.height == 1668 || Screen.width == 2732 && Screen.height == 2048)
                {
                    virtualCamera.distance = cSVBattleSceneData.distance[4] / 10000f;
                    virtualCamera.clipFar = cSVBattleSceneData.far[4] / 10000f;
                }
                else
                {
                    if ((float)Screen.width / Screen.height <= 2480f / 2200f)
                    {
                        virtualCamera.distance = cSVBattleSceneData.distance[2] / 10000f;
                        virtualCamera.clipFar = cSVBattleSceneData.far[2] / 10000f;
                    }
                    else if (((float)Screen.width / Screen.height > 2480f / 2200f) && ((float)Screen.width / Screen.height <= 2208f / 1768f))
                    {
                        virtualCamera.distance = cSVBattleSceneData.distance[3] / 10000f;
                        virtualCamera.clipFar = cSVBattleSceneData.far[3] / 10000f;
                    }
                    else if (((float)Screen.width / Screen.height > 2208f / 1768f) && ((float)Screen.width / Screen.height <= 2048f / 1536f))
                    {
                        virtualCamera.distance = cSVBattleSceneData.distance[4] / 10000f;
                        virtualCamera.clipFar = cSVBattleSceneData.far[4] / 10000f;
                    }
                    else
                    {
                        virtualCamera.distance = cSVBattleSceneData.distance[0] / 10000f;
                        virtualCamera.clipFar = cSVBattleSceneData.far[0] / 10000f;
                    }
                }
#endif
                virtualCamera.pith = cSVBattleSceneData.pith / 10000f;
                virtualCamera.yaw = cSVBattleSceneData.yaw / 10000f;
                virtualCamera.fixedLookPoint = lookPoint;
                Vector3 offset = new Vector3(cSVBattleSceneData.x_offset / 10000f, cSVBattleSceneData.y_offset / 10000f, cSVBattleSceneData.z_offset / 10000f);
                virtualCamera.lookPointOffset = offset;
            }
            else
            {
                virtualCamera.distance = System.Convert.ToSingle(CSVParam.Instance.GetConfData(173).str_value) / 10000f;
                virtualCamera.fov = System.Convert.ToSingle(CSVParam.Instance.GetConfData(174).str_value) / 10000f;
                virtualCamera.pith = System.Convert.ToSingle(CSVParam.Instance.GetConfData(170).str_value) / 10000f;
                virtualCamera.yaw = System.Convert.ToSingle(CSVParam.Instance.GetConfData(171).str_value) / 10000f;
                virtualCamera.fixedLookPoint = lookPoint;
                Vector3 offset = new Vector3(System.Convert.ToSingle(CSVParam.Instance.GetConfData(175).str_value) / 10000f, System.Convert.ToSingle(CSVParam.Instance.GetConfData(176).str_value) / 10000f, System.Convert.ToSingle(CSVParam.Instance.GetConfData(177).str_value) / 10000f);
                virtualCamera.lookPointOffset = offset;
            }

            CoroutineManager.Instance.Start(SetCullMask());

            virtualCamera.Recalculation();
        }

        private IEnumerator SetCullMask()
        {
            yield return new WaitForEndOfFrame();
         
            int cullingMask = CameraManager.GetMainCameraCullingMask();

            cullingMask |= (int)ELayerMask.Monster;
            cullingMask |= (int)ELayerMask.Partner;
            cullingMask &= ~(int)ELayerMask.OtherActor;
            cullingMask &= ~(int)ELayerMask.Player;
            cullingMask &= ~(int)ELayerMask.NPC;
            cullingMask &= ~(int)ELayerMask.HidingSceneActor;

            CameraManager.SetMainCameraCullingMask(cullingMask);
        }

        TweenerCore<float, float, FloatOptions> A;
        TweenerCore<float, float, FloatOptions> B;
        TweenerCore<float, float, FloatOptions> C;
        TweenerCore<float, float, FloatOptions> D;
        TweenerCore<float, float, FloatOptions> E;
        TweenerCore<Vector3, Vector3, VectorOptions> F;
        TweenerCore<float, float, FloatOptions> G;

        public void EnterWorld()
        {
            if (A != null) A.Kill();
            if (B != null) B.Kill();
            if (C != null) C.Kill();
            if (D != null) D.Kill();
            if (E != null) E.Kill();
            if (F != null) F.Kill();
            if (G != null) G.Kill();

            eCameraType = ECameraType.Follow;
            virtualCamera.autoFollowTarget = true;

            try
            {
                virtualCamera.pith = 40f;
                virtualCamera.yaw = 45f;
                virtualCamera.roll = 0f;
                virtualCamera.distance = 45f;
                virtualCamera.fov = 10.0f;
                virtualCamera.clipFar = 80.0f;
                virtualCamera.lookPointOffset = new Vector3(0f, 0.6f, 0f);
            }
            catch (Exception e)
            { }
            
            int cullingMask = CameraManager.GetMainCameraCullingMask();
            cullingMask |= (int)ELayerMask.OtherActor;
            cullingMask |= (int)ELayerMask.NPC;
            cullingMask |= (int)ELayerMask.Player;
            cullingMask |= (int)ELayerMask.Partner;
            cullingMask &= ~(int)ELayerMask.Monster;
            cullingMask &= ~(int)ELayerMask.HidingSceneActor;

            CameraManager.SetMainCameraCullingMask(cullingMask);
        }

        public void EnterNPCInteractive()
        {
            eCameraType = ECameraType.Follow;
            virtualCamera.autoFollowTarget = true;
          
            virtualCamera.TargetCamera.cullingMask &= ~(int)ELayerMask.Partner;
            virtualCamera.TargetCamera.cullingMask &= ~(int)ELayerMask.HidingSceneActor;
            virtualCamera.TargetCamera.cullingMask &= ~(int)ELayerMask.OtherActor;
        }

        public void EnterModelShow()
        {
            eCameraType = ECameraType.Fixed;
            virtualCamera.autoFollowTarget = false;

            virtualCamera.transform.eulerAngles = new Vector3(0, 180, 0);
            virtualCamera.transform.position = new Vector3(100, 100, 0);
        }

        public void RecordLastCameraData()
        {
            lastCameraData.pith = virtualCamera.pith;
            lastCameraData.yaw = virtualCamera.yaw;
            lastCameraData.roll = virtualCamera.roll;
            lastCameraData.distance = virtualCamera.distance;           
            lastCameraData.fov = virtualCamera.fov;
            lastCameraData.clipFar = virtualCamera.clipFar;
            lastCameraData.lookPointOffset = virtualCamera.lookPointOffset;
        }

        public void SetCameraData(CameraData cameraData,float tweenerTime)
        {
            A = DOTween.To(() => virtualCamera.pith, x => virtualCamera.pith = x, cameraData.pith, tweenerTime).SetEase(Ease.Linear);
            B = DOTween.To(() => virtualCamera.yaw, x => virtualCamera.yaw = x, cameraData.yaw, tweenerTime).SetEase(Ease.Linear);
            C = DOTween.To(() => virtualCamera.roll, x => virtualCamera.roll = x, cameraData.roll, tweenerTime).SetEase(Ease.Linear);
            D = DOTween.To(() => virtualCamera.distance, x => virtualCamera.distance = x, cameraData.distance, tweenerTime).SetEase(Ease.Linear);
            E = DOTween.To(() => virtualCamera.fov, x => virtualCamera.fov = x, cameraData.fov, tweenerTime).SetEase(Ease.Linear);
            F = DOTween.To(() => virtualCamera.lookPointOffset, x => virtualCamera.lookPointOffset = x, cameraData.lookPointOffset, tweenerTime).SetEase(Ease.Linear);
            G = DOTween.To(() => virtualCamera.clipFar, x => virtualCamera.clipFar = x, cameraData.clipFar, tweenerTime).SetEase(Ease.Linear);
        }

        public void RevertToLastCameraData(float tweenerTime,TweenCallback tweenCallback=null)
        {
            A = DOTween.To(() => virtualCamera.pith, x => virtualCamera.pith = x, lastCameraData.pith, tweenerTime).SetEase(Ease.Linear);
            B = DOTween.To(() => virtualCamera.yaw, x => virtualCamera.yaw = x, lastCameraData.yaw, tweenerTime).SetEase(Ease.Linear);
            C = DOTween.To(() => virtualCamera.roll, x => virtualCamera.roll = x, lastCameraData.roll, tweenerTime).SetEase(Ease.Linear);
            D = DOTween.To(() => virtualCamera.distance, x => virtualCamera.distance = x, lastCameraData.distance, tweenerTime).SetEase(Ease.Linear);
            E = DOTween.To(() => virtualCamera.fov, x => virtualCamera.fov = x, lastCameraData.fov, tweenerTime).SetEase(Ease.Linear);
            F = DOTween.To(() => virtualCamera.lookPointOffset, x => virtualCamera.lookPointOffset = x, lastCameraData.lookPointOffset, tweenerTime).SetEase(Ease.Linear);
            G = DOTween.To(() => virtualCamera.clipFar, x => virtualCamera.clipFar = x, lastCameraData.clipFar, tweenerTime).SetEase(Ease.Linear);

            if (tweenCallback!=null)
            {
                A.onComplete += tweenCallback;
            }
        }

        public void SetCameraData(AttitudeAngleTransform TargetvirtualCamera)
        {
            A = DOTween.To(() => virtualCamera.pith, x => virtualCamera.pith = x, TargetvirtualCamera.pith, 0).SetEase(Ease.Linear);
            B = DOTween.To(() => virtualCamera.yaw, x => virtualCamera.yaw = x, TargetvirtualCamera.yaw, 0).SetEase(Ease.Linear);
            C = DOTween.To(() => virtualCamera.roll, x => virtualCamera.roll = x, TargetvirtualCamera.roll, 0).SetEase(Ease.Linear);
            D = DOTween.To(() => virtualCamera.distance, x => virtualCamera.distance = x, TargetvirtualCamera.distance, 0).SetEase(Ease.Linear);
            E = DOTween.To(() => virtualCamera.fov, x => virtualCamera.fov = x, TargetvirtualCamera.fov, 0).SetEase(Ease.Linear);
            F = DOTween.To(() => virtualCamera.lookPointOffset, x => virtualCamera.lookPointOffset = x, TargetvirtualCamera.lookPointOffset, 0).SetEase(Ease.Linear);
            G = DOTween.To(() => virtualCamera.clipFar, x => virtualCamera.clipFar = x, TargetvirtualCamera.clipFar, 0).SetEase(Ease.Linear);
        }


        public void SetVirtualCamera(Camera camera)
        {
            if (camera == null)
                return;

            _virtualCamera = camera.gameObject.GetNeedComponent<AttitudeAngleTransform>();

            if (followActor != null)
            {
                virtualCamera.target = followActor.gameObject.transform;
            }
        }

        public void SetFollowActor(SceneActor sceneActor)
        {
            followActor = sceneActor;
            if (followActor != null && virtualCamera != null)
            {
                virtualCamera.target = followActor.transform;
            }
        }

#region CameraShake

        public void DoShake(float duration, Vector3 strength, int vibrato = 10, float randomness = 90, bool fadeOut = true)
        {
            virtualCamera.DoShake(duration, strength, vibrato, randomness, fadeOut);
        }
        public void StopShark() {
            virtualCamera.StopShark();
        }
#endregion


        //TODO 做成loadingUI
        //RenderTexture FadeTexture = new RenderTexture(Screen.width, Screen.height, 0, UnityEngine.Experimental.Rendering.DefaultFormat.LDR);
        public void FadeInOut(uint effectType, System.Action func)
        {
            if (effectType == 1u ||
                effectType == 3u || effectType == 4u ||
                effectType == 5u)
            {
                Material mat = GlobalAssets.GetAsset<Material>(effectType == 1u ? GlobalAssets.sMat_ImageFade : GlobalAssets.sMat_CusScreenFade);
                if (mat == null)
                    return;

                GameObject canvasObj = new GameObject("Canvas");
                canvasObj.transform.parent = UIManager.mRoot;
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;

                GameObject rawImageObj = new GameObject("Raw Image");
                rawImageObj.transform.parent = canvas.transform;
                RawImage rawImage = rawImageObj.AddComponent<RawImage>();
                RectTransform rt = rawImage.GetComponent<RectTransform>();// = new Vector2(5000, 5000);
                rt.anchorMin = new Vector2(0, 0);
                rt.anchorMax = new Vector2(1, 1);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
                rawImage.color = Color.white;

                int rtScale = 1;
                if(effectType == 3u || effectType == 5u)
                    rtScale = 3;

                RenderTexture renderTexture = RenderTexture.GetTemporary(Screen.width / rtScale, Screen.height / rtScale, 0);
                
                virtualCamera.TargetCamera.targetTexture = renderTexture;// RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
                virtualCamera.TargetCamera.Render();
                virtualCamera.TargetCamera.targetTexture = null;
                rawImage.texture = renderTexture;
                
                rawImage.material = mat;

                if (effectType != 1u)
                {
                    int eStyle;
                    if (effectType == 3u)
                        eStyle = 3;
                    else if (effectType == 4u)
                        eStyle = 1;
                    else
                        eStyle = 2;

                    mat.SetFloat("_Style", eStyle);
                }
                
                bool isFadeIn = false;
                float time = 2f;
                Timer.Register(time, () =>
                {
                    RenderTexture.ReleaseTemporary(renderTexture);
                    
                    UnityEngine.Object.DestroyImmediate(canvasObj);

                    func?.Invoke();

                }, (f) =>
                {
                    if (isFadeIn == false && f >= 1f)
                    {
                        virtualCamera.TargetCamera.targetTexture = renderTexture;// RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
                        virtualCamera.TargetCamera.Render();
                        virtualCamera.TargetCamera.targetTexture = null;
                        rawImage.texture = renderTexture;

                        isFadeIn = true;

                        if (effectType == 3u || effectType == 4u)
                            mat.SetFloat("_Style", 4);
                    }

                    if (effectType == 1u)
                    {
                        if (isFadeIn)
                        {
                            mat.SetFloat("_Schedule", 2 - f);
                        }
                        else
                        {
                            mat.SetFloat("_Schedule", f);
                        }
                    }
                    else
                    {
                        if (isFadeIn)
                        {
                            mat.SetFloat("_Blend", 2f - f);
                        }
                        else
                        {
                            mat.SetFloat("_Blend", f);
                        }
                    }
                }, false, false);
            }
            else
            {
                GameObject canvasObj = new GameObject("Canvas");
                canvasObj.transform.parent = UIManager.mRoot;
                Canvas canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 9999;

                GameObject rawImageObj = new GameObject("Raw Image");
                rawImageObj.transform.parent = canvas.transform;
                RawImage rawImage = rawImageObj.AddComponent<RawImage>();
                rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(5000, 5000);
                rawImage.color = Color.black;

                float time = 0f;
                Timer.Register(time, () =>
                {
                    UnityEngine.Object.DestroyImmediate(canvasObj);
                    func?.Invoke();
                }, (f) =>
                {
                    rawImage.color = new Color(0, 0, 0, 1 - f / time);
                }, false, false);
            }
        }
    }
}
