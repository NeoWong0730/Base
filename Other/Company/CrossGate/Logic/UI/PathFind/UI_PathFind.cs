using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_PathFind : UIBase
    {
        //private RectTransform imgTrans;
        ////private GameObject fxGo;
        ////private ParticleSystem particle;
        //private Vector3 localScale;
        ////private Vector3 particleLocalScale;
        ////private Vector3 fxEuler;
        //private float paraValue = 22.5f;
        //protected override void OnLoaded()
        //{            
        //    //imgTrans = transform.Find("Animator/Image_BG").GetComponent<RectTransform>();
        //    //localScale = imgTrans.localScale;
        //    //eularRotation = imgTrans.localEulerAngles;
        //
        //    //fxGo = transform.Find("Animator/Fx_ui_PathFind").gameObject;
        //    //fxEuler = fxGo.transform.localEulerAngles;
        //    //particle = arry[0];
        //    //particleLocalScale = particle.transform.localScale;
        //}        

        //protected override void OnUpdate()
        //{
        //    base.OnUpdate();
        //}        

        //private void CalRotation(ref float scale, ref float fxEulerY)
        //{
        //    if (GameCenter.mainHero != null && GameCenter.mainHero.transform != null)
        //    {
        //        //scale = GameCenter.mainHero.transform.forward.x < 0 ? Mathf.Abs(scale) : -Mathf.Abs(scale);
        //        float rotateY = GameCenter.mainHero.transform.localEulerAngles.y;
        //        bool right = rotateY >= 45f && rotateY <= 225;
        //        scale = right ? -Mathf.Abs(scale) : Mathf.Abs(scale);
        //        fxEulerY = right ? 0f : 180f;
        //        //if (rotateY > (225f - paraValue) && rotateY <= (225f + paraValue))  //225, bottom
        //        //{
        //        //    scale = Mathf.Abs(scale);
        //        //    rotate = -90f;
        //        //}
        //        //else if (rotateY > (270f - paraValue) && rotateY <= (270f + paraValue))
        //        //{
        //        //    scale = Mathf.Abs(scale);
        //        //    rotate = -45f;
        //        //}
        //        //else if (rotateY > (315f - paraValue) && rotateY <= (315f + paraValue)) //325, left
        //        //{
        //        //    scale = Mathf.Abs(scale);
        //        //    rotate = 0f;
        //        //}
        //        //else if (rotateY > (315f + paraValue) || rotateY <= (0f + paraValue))
        //        //{
        //        //    scale = Mathf.Abs(scale);
        //        //    rotate = 45f;
        //        //}
        //        //else if (rotateY > (45f - paraValue) && rotateY <= (45f + paraValue)) //top
        //        //{
        //        //    scale = Mathf.Abs(scale);
        //        //    rotate = 90f;
        //        //}
        //        //else if (rotateY > (90f - paraValue) && rotateY <= (90f + paraValue))
        //        //{
        //        //    scale = -Mathf.Abs(scale);
        //        //    rotate = -45f;
        //        //}
        //        //else if (rotateY > (135f - paraValue) && rotateY <= (135f + paraValue)) //right
        //        //{
        //        //    scale = -Mathf.Abs(scale);
        //        //    rotate = 0;
        //        //}
        //        //else
        //        //{
        //        //    scale = -Mathf.Abs(scale);
        //        //    rotate = 45f;
        //        //}
        //    }
        //}
    }
}
