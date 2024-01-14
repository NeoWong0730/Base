using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Logic.Core;
using Lib.Core;
using Table;

namespace Logic
{
    public class UIRuleParam
    {
        public uint TitlelanId = 4069u; //默认规则说明
        public string TitleValue;
        public string StrContent = "";
        public Vector3 Pos = Vector3.zero;
    }

    public class UI_Rule : UIBase
    {
        private Transform transAni;
        private Text _textTitle;
        private Text _textContent;
        private RectTransform _rectTransform;
        private UIRuleParam _ruleParam;

        protected override void OnLoaded()
        {
            transAni = transform.Find("Animator");
            _textTitle = transform.Find("Animator/View_Rule/Image_Bg/Image_Title/Text_Title").GetComponent<Text>();
            _textContent = transform.Find("Animator/View_Rule/Image_Bg/Text_Tips").GetComponent<Text>();
            _rectTransform = transform.Find("Animator/View_Rule/Image_Bg").GetComponent<RectTransform>();
            Button btn = transform.Find("Black").GetComponent<Button>();
            btn.onClick.AddListener(OnClickClose);
        }

        protected override void OnOpen(object arg1 = null)
        {
            _ruleParam = null;
            if (arg1 != null)
            {
                _ruleParam = (UIRuleParam)arg1;
            }
        }

        protected override void OnShow()
        {
            if(_ruleParam != null)
            {
                if (!_ruleParam.Pos.Equals(Vector3.zero))
                {
                    Vector2 screenPoint = new Vector2(_ruleParam.Pos.x, _ruleParam.Pos.y);
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(gameObject.GetComponent<RectTransform>(), screenPoint, CameraManager.mUICamera, out Vector3 pos);
                    transAni.position = pos;
                }
                if (_ruleParam.TitleValue != null)
                {
                    _textTitle.text = LanguageHelper.GetTextContent(_ruleParam.TitlelanId, _ruleParam.TitleValue);
                }
                else
                {
                    _textTitle.text = LanguageHelper.GetTextContent(_ruleParam.TitlelanId);
                }
                _textContent.text = _ruleParam.StrContent;

                LayoutRebuilder.ForceRebuildLayoutImmediate(_rectTransform);
            }
        }

        protected override void OnHide()
        {

        }

        private void OnClickClose()
        {
           CloseSelf();
        }
    }
}



