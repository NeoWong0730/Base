using DG.Tweening;
using Lib.Core;
using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class SkillShow: Anim_BaseShow
    {
        public override void Construct(AnimData animData, CSVDamageShowConfig.Data cSVDamageShowConfigData, Action<Anim_BaseShow> _onPlayCompleted = null)
        {
            base.Construct(animData, cSVDamageShowConfigData, _onPlayCompleted);

            positionCorrect.NeedCorrectAtSkillPlay = true;
            positionCorrect.NeedFollow = false;
            positionCorrect.SetTarget(animData.battleObj.transform);
            positionCorrect.SetbaseOffest(new Vector3(offsetX, offsetY, 0));

            if (animData.bUseTrans)
            {
                //InitPos_Trans();
                positionCorrect.CalPos_Trans();
            }
            else
            {
                //InitPos_Vec3(animData.pos);
                positionCorrect.CalPos_Vec(animData.pos);
            }
            Show(mAnimData.content);
        }

        public override void Initizal()
        {
            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(103);
            string[] str = cSVParamData.str_value.Split(',');
            mPlayTimer = Convert.ToSingle(str[0]) / 10000;
            mShowTimer = Convert.ToSingle(str[1]) / 10000;
            mHideTimer = Convert.ToSingle(str[2]) / 10000;
            //CSVParam.Data cSVParamData2 = CSVParam.Instance.GetConfData(110);
            //string[] str2 = cSVParamData2.str_value.Split(',');
            //offsetX = Convert.ToSingle(str2[3]);
            //offsetY= Convert.ToSingle(str2[4]);

            string s = CSVParam.Instance.GetConfData(110).str_value;
            string[] s1 = s.Split('|');
            string res = s1[mAnimData.clientNum];
            string res1 = res.Split('&')[1];
            offsetX = 0;
            offsetY = Convert.ToSingle(res1);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnPlayOver()
        {
            LogicOver = true;
            onPlayCompleted.Invoke(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            positionCorrect.Dispose();
        }

        public override void Show( string content )
        {
            mText.text = content;
            bStartShow = true;
            mText.text = content;
            DOTween.To(() => mCanvasGroup.alpha, x => mCanvasGroup.alpha = x, 1, mShowTimer);
        }
        
        //public override void InitPos_Trans()
        //{
        //    if (mTarget != null)
        //    {
        //        if (CameraManager.mSkillPlayCamera == null)
        //        {
        //            CameraManager.World2UI(mRootGameObject, mTarget.position + new Vector3(offsetX, offsetY, 0), CameraManager.mCamera, UIManager.mUICamera);
        //            rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //        }
        //        else
        //        {
        //            CameraManager.World2UI(mRootGameObject, mTarget.position + new Vector3(0, 1.5f, 0), CameraManager.mSkillPlayCamera, UIManager.mUICamera, 800, 480,
        //               CameraManager.relativePos.x, CameraManager.relativePos.y);
        //            rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //        }
        //    }
        //}
    }
}

