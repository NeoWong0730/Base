using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using Table;
using DG.Tweening.Plugins.Options;
using DG.Tweening.Core;

namespace Logic
{
    public class NpcBubbleShow : IHUDComponent
    {
        public ulong actorId { get; private set; }

        public Transform target { get; private set; }
        public GameObject mRootGameobject;
        public Vector2 offset;
        private Vector3 offset_model;
        private Timer m_TextPlayTimer;
        private uint destroyTimer;
        private TansformWorldToScreen.WorldToScreenData _worldToScreenData;
        private TansformWorldToScreen _tansformWorldToScreen;

        private Action<NpcBubbleShow> m_OnRecycle;
        private Action m_OnComplete;
        private bool b_Valid = false;

        public RectTransform bg         //bg
        {
            get
            {
                if (mRootGameobject == null)
                {
                    return null;
                }
                return mRootGameobject.transform as RectTransform;
            }
        }

        private Text templeteText;

        public EmojiText mEmojiText;

        TextGenerator _textGenerator = null;
        TextGenerationSettings _textGenerationSettings;
        private TweenerCore<string, string, StringOptions> m_TweenerCore;


        public void Construct(GameObject gameObject, Text templete, TansformWorldToScreen tansformWorldToScreen, uint _timer, ulong _actorId = 0, int _clientNum = -1)
        {
            b_Valid = true;
            mRootGameobject = gameObject;
            destroyTimer = _timer;
            actorId = _actorId;

            string vs = CSVParam.Instance.GetConfData(197).str_value;
            string[] s = vs.Split('|');
            offset_model = new Vector3(float.Parse(s[0]), float.Parse(s[1]), float.Parse(s[2]));

            _tansformWorldToScreen = tansformWorldToScreen;
            _worldToScreenData = tansformWorldToScreen.Request();
            _worldToScreenData.to = gameObject.transform as RectTransform;

            templeteText = templete;
            RectTransform templateTextRT = templeteText.transform as RectTransform;
            LayoutRebuilder.ForceRebuildLayoutImmediate(templateTextRT);
            _textGenerator = templeteText.cachedTextGenerator;
            _textGenerationSettings = templeteText.GetGenerationSettings((templeteText.transform as RectTransform).sizeDelta);
            _textGenerationSettings.richText = true;

            _textGenerationSettings.scaleFactor = 1f;            _textGenerationSettings.horizontalOverflow = HorizontalWrapMode.Wrap;
            //positionCorrect = mRootGameobject.GetNeedComponent<HUDPositionCorrect>();
            //positionCorrect.SetbaseOffest(offset_model);
            mEmojiText = mRootGameobject.transform.Find("Text").GetComponent<EmojiText>();
            mEmojiText.text = string.Empty;
            m_TextPlayTimer?.Cancel();
        }

        public void SetTarget(Transform _transform)
        {
            //positionCorrect.SetTarget(_transform);
            _worldToScreenData.from = _transform;
        }

        public void CalNpcOffest(Vector3 _offest)
        {
            //positionCorrect.CalOffest(_offest);
            _worldToScreenData.positionOffset = _offest + offset_model;
        }
        public void SetContent(string content, float bubbleTextInterval, Action<NpcBubbleShow> onRecycle, Action complete = null)
        {
            m_TweenerCore?.Kill();
            m_OnRecycle = onRecycle;
            m_OnComplete = complete;
            bg.sizeDelta = new Vector2(CalculateWSize(content), CalculateHSize(content));
            //mEmojiText.text = content;
            float NeetTime = (float)(content.Length - 1) * bubbleTextInterval;
            m_TweenerCore = mEmojiText.DOText(content, NeetTime);
            m_TweenerCore.onComplete = OnTextPlayEnded;
        }

        private void OnTextPlayEnded()
        {
            if (b_Valid)
            {
                m_TextPlayTimer = Timer.Register(destroyTimer, OnBubbleRecycle);
            }
        }

        private void OnBubbleRecycle()
        {
            m_OnComplete?.Invoke();
            m_OnRecycle?.Invoke(this);
        }

        private float CalculateHSize(string content)
        {
            float h = _textGenerator.GetPreferredHeight(content, _textGenerationSettings);
            return h;
        }

        private float CalculateWSize(string content)
        {
            float w = _textGenerator.GetPreferredWidth(content, _textGenerationSettings);
            w = Mathf.Min(w, 200);
            return w + 8;
        }

        public void Dispose()
        {
            //positionCorrect.Dispose();
            m_TweenerCore?.Kill();
            b_Valid = false;
            m_OnRecycle = null;
            m_OnComplete = null;
            if (mEmojiText)
            {
                mEmojiText.text = string.Empty;
            }
            _tansformWorldToScreen.Release(ref _worldToScreenData);
            destroyTimer = 0;
            m_TextPlayTimer?.Cancel();
            mRootGameobject.SetActive(false);
        }
    }
}


