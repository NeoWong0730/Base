using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Framework;
using Logic.Core;
using DG.Tweening;

namespace Logic
{
    public class UI_Loading2 : UIBase
    {
        RawImage mRawImage = null;
        RenderTexture mRenderTexture;
        GameObject mIcon = null;
        Animator mAnimator = null;

        float _startTime = 0;
        float _loadedTime = 0;
        bool _sceneLoaded = false;

        protected override void OnLoaded()
        {            
            mRawImage = transform.Find("Image_Loading").GetComponent<RawImage>();
            mIcon = transform.Find("UI_Map_Go").gameObject;
            mAnimator = mIcon.GetComponent<Animator>();
        }

        protected override void OnOpen(object arg)
        {
            _sceneLoaded = false;

            //if (mRenderTexture != null)
            //{
            //    RenderTexture.ReleaseTemporary(mRenderTexture);
            //    mRenderTexture = null;
            //}
            //
            //CameraManager.mCamera.targetTexture = mRenderTexture = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            //
            //int recodeMask = CameraManager.mCamera.cullingMask;
            //CameraManager.mCamera.cullingMask = (int)(ELayerMask.Build | ELayerMask.Default | ELayerMask.Terrain | ELayerMask.Grass | ELayerMask.Water | ELayerMask.SmallObject | ELayerMask.Tree);
            //
            //CameraManager.mCamera.Render();
            //
            //CameraManager.mCamera.cullingMask = recodeMask;
            //
            //CameraManager.mCamera.targetTexture = null;

            if(bLoaded)
            {
                Material material = mRawImage.materialForRendering;
                material.SetFloat(Consts.ID_Schedule, 0);
            }
        }

        protected override void OnShow()
        {
            _startTime = Time.unscaledTime;
            //mRawImage.texture = mRenderTexture;
            //mRawImage.color = Color.white;
            /*            mRawImage.DOColor(new Color(1, 1, 1, 0), 1f);*/
            Material material = mRawImage.materialForRendering;
            material.SetFloat(Consts.ID_Schedule, 0);
        }

        //protected override void OnHide()
        //{
        //    //RenderTexture.ReleaseTemporary(mRenderTexture);
        //    //mRawImage.texture = null;
        //    //mRenderTexture = null;
        //}
        //
        //protected override void OnClose()
        //{
        //    //if (mRenderTexture != null)
        //    //{
        //    //    RenderTexture.ReleaseTemporary(mRenderTexture);
        //    //    mRenderTexture = null;
        //    //}
        //}

        bool isShow = false;
        protected override void OnLateUpdate(float dt, float usdt)
        {            
            float currentTime = Time.unscaledTime;
            float schedule = currentTime - _startTime;

            if (_sceneLoaded)
            {
                schedule = schedule / GameCenter.fSwitchMapFadeInTime;

                //float alphaSchedule = currentTime - _loadedTime;
                if (schedule >= 1.0f)
                {
                    UIManager.CloseUI(EUIID.UI_Loading2, false, false);
                }
                else
                {
                    Material material = mRawImage.materialForRendering;
                    material.SetFloat(Consts.ID_Schedule, Mathf.Clamp01(schedule));
                    //mRawImage.color = Color.Lerp(Color.white, new Color(1, 1, 1, 0), alphaSchedule);
                }
            }
            else
            {
                if (GameCenter.nLoadStage >= 3)
                {
                    _startTime = currentTime;
                    _sceneLoaded = true;

                    mIcon.SetActive(false);
                    isShow = false;
                }
                else
                {
                    schedule = schedule / GameCenter.fSwitchMapFadeOutTime;

                    Material material = mRawImage.materialForRendering;
                    material.SetFloat(Consts.ID_Schedule, 1 - Mathf.Clamp01(schedule));

                    if (schedule >= 1 && !isShow)
                    {
                        mIcon.SetActive(true);
                        mAnimator.Update(0);
                        isShow = true;
                    }
                }
            }                        
        }
    }

}