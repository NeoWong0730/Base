using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Framework;
using DG.Tweening;
using System;
using Lib.Core;

namespace Logic
{
    public class SecondActionShow : IHUDComponent
    {
        private Action<SecondActionShow> action;
        private CanvasGroup secondActionGroup;

        private Transform target;
        public GameObject mRootGameObject;

        private float secondActionShowTime;
        private float secondActionStayTime;
        private float secondActionHideTime;
        private Vector3 offest;
        private HUDPositionCorrect positionCorrect;

        private RectTransform rectTransform
        {
            get
            {
                return mRootGameObject.transform as RectTransform;
            }
        }

        public void Construct( GameObject gameObject ,Action<SecondActionShow> _action)
        {
            mRootGameObject = gameObject;
            ParseCp();
            action = _action;
            string[] secondConfig = CSVParam.Instance.GetConfData(643).str_value.Split('|');
            offest = new Vector3(float.Parse(secondConfig[0]), float.Parse(secondConfig[1]), float.Parse(secondConfig[2]));
            secondActionShowTime = float.Parse(secondConfig[3])/1000f;
            secondActionStayTime = float.Parse(secondConfig[4])/1000f;
            secondActionHideTime = float.Parse(secondConfig[5])/1000f;

            positionCorrect = mRootGameObject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect.CalOffest(offest);
        }
        
     

        private void ParseCp()
        {
            secondActionGroup = mRootGameObject.GetComponent<CanvasGroup>();
        }

        public void SetTarget( Transform transform )
        {
            positionCorrect.SetTarget(transform);
        }
       
        
        public void TriggerSecondAction()
        {
            DOTween.To(() => secondActionGroup.alpha, x => secondActionGroup.alpha = x, 1, secondActionShowTime).onComplete+=()=>
            {
                DOTween.To(() => secondActionGroup.alpha, x => secondActionGroup.alpha = x, 1, secondActionStayTime).onComplete += () =>
                {
                    DOTween.To(() => secondActionGroup.alpha, x => secondActionGroup.alpha = x, 0, secondActionHideTime).onComplete += () =>
                    {
                        action.Invoke(this);
                    }; 
                };
            };
        }

      
        public void Dispose()
        {
            positionCorrect.Dispose();
            mRootGameObject.SetActive(false);
            secondActionGroup.alpha = 0;
        }
    }
}

