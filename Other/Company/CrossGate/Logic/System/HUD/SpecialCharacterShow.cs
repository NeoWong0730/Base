using DG.Tweening;
using Framework;
using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;


namespace Logic
{
    [HUDAnim(AnimType.e_Miss)]
    public class MissCharacterShow : SpecialCharacterShow
    {
       
    }

    public class ErrorCharactorShow : SpecialCharacterShow
    {

    }

    public class SpecialCharacterShow : Anim_BaseShow
    {
        public override void Construct(AnimData animData, CSVDamageShowConfig.Data cSVDamageShowConfigData,Action<Anim_BaseShow> _onPlayCompleted = null)
        {
            base.Construct(animData, cSVDamageShowConfigData, _onPlayCompleted);

            positionCorrect.NeedCorrectAtSkillPlay = true;
            positionCorrect.NeedFollow = false;
            positionCorrect.SetbaseOffest(new Vector3(offsetX, offsetY, 0));

            if (animData.bUseTrans)
            {
                //InitPos_Trans();
                positionCorrect.SetTarget(animData.battleObj.transform);
                positionCorrect.CalPos_Trans();
            }
            else
            {
                //InitPos_Vec3(animData.pos);
                positionCorrect.CalPos_Vec(animData.pos);
            }
            Show(mAnimData.content);
            DoAction();
        }

        public override void Initizal()
        {
            base.Initizal();
        }

        public override void Show(int finnaldamage, int floatingdamage)
        {
            base.Show(finnaldamage, floatingdamage);
        }

        public override void Show(string str)
        {
            mText.font = FontManager.GetFont(GlobalAssets.sFont_Special);// "Font/Special_font.fontsettings"
            base.Show(str);
        }


        public override void Update()
        {
            base.Update();
        }

        public override void Dispose()
        {
            base.Dispose();
            positionCorrect.Dispose();
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
        //            CameraManager.World2UI(mRootGameObject, mTarget.position + new Vector3(0, 1.5f, 0), CameraManager.mSkillPlayCamera, UIManager.mUICamera);
        //            rect.position = new Vector3(rect.position.x, rect.position.y,
        //                UIManager.mUICamera.transform.position.z + 100);
        //            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x * Screen.width / 800 * 800 / 1280 + CameraManager.relativePos.x,
        //               rect.anchoredPosition.y * Screen.height / 480 * 480 / 720 + CameraManager.relativePos.y);
        //        }
        //    }
        //}

        //public override void InitPos_Vec3(Vector3 vector3)
        //{
        //    if (CameraManager.mSkillPlayCamera == null)
        //    {
        //        CameraManager.World2UI(mRootGameObject, vector3, CameraManager.mCamera, UIManager.mUICamera);
        //        rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //    }
        //    else
        //    {
        //        CameraManager.World2UI(mRootGameObject, vector3, CameraManager.mSkillPlayCamera, UIManager.mUICamera, 800, 480,
        //               CameraManager.relativePos.x, CameraManager.relativePos.y);
        //        rect.position = new Vector3(rect.position.x, rect.position.y, UIManager.mUICamera.transform.position.z + 100);
        //    }
        //}
    }
}
