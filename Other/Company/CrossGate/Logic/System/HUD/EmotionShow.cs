using Logic;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    //心情符号
    public class EmotionShow : IHUDComponent
    {
        public GameObject mRootGameObject;
        private RectTransform rectTransform
        {
            get
            {
                return mRootGameObject == null ? null : mRootGameObject.transform as RectTransform;
            }
        }

        public Transform target;
        private Transform parent;
        private Vector3 offest;
        private CSVMoodSymbol.Data cSVMoodSymbolData;
        private Timer timer;
        private Action<EmotionShow> onRecyle;
        public Action onComplete;

        private HUDPositionCorrect positionCorrect;

        public void Construct(GameObject gameObject,uint emotionId,Action<EmotionShow> _onRecyle ,Action _onComplete)
        {
            mRootGameObject = gameObject;
            onRecyle = _onRecyle;
            onComplete = _onComplete;
            cSVMoodSymbolData = CSVMoodSymbol.Instance.GetConfData(emotionId);
            offest = new Vector3(0, cSVMoodSymbolData.offset / 100f, 0);
            ParseCp();
            Refresh();
            timer = Timer.Register(cSVMoodSymbolData.displayTime, () => 
            {
                onRecyle?.Invoke(this);
                onComplete?.Invoke();
            });
            positionCorrect = mRootGameObject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect.CalOffest(offest);
        }

        public void SetTarget(Transform transform)
        {
            positionCorrect.SetTarget(transform);
            target = transform;
        }

        private void ParseCp()
        {
            parent = mRootGameObject.transform.Find("View_StoryEmotion");
        }

        public void Dispose()
        {
            positionCorrect.Dispose();
            timer?.Cancel();
            mRootGameObject.SetActive(false);
            onComplete = null;
        }
        
        private void Refresh()
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                parent.GetChild(i).gameObject.SetActive((i + 1) == cSVMoodSymbolData.type);
            }
        }
    }
}


