using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Logic.Core;
using System;

using DG.Tweening;

namespace Logic
{
    public class UI_Chat_VoiceInput : UIBase
    {
        private Image _slider;
        private Text _txt;
        private Tweener tweener;

        protected override void OnLoaded()
        {
            _slider = transform.Find("Animator/Image_Bottom/Image_Slider").GetComponent<Image>();
            _txt = transform.Find("Animator/Image_TextBottom/Text").GetComponent<Text>();
        }

        protected override void OnShow()
        {
            _Refresh();            
        }

        protected override void OnHide()
        {
            if (tweener != null)
            {
                tweener.Kill(false);
                tweener = null;
            }            
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Chat.Instance.eventEmitter.Handle(Sys_Chat.EEvents.VoiceRecordStateChange, OnVoiceRecordStateChange, toRegister);
        }

        private void OnVoiceRecordStateChange()
        {
            _Refresh();
        }

        private void _Refresh()
        {
            if (tweener != null)
            {
                tweener.Kill(false);
                tweener = null;
            }                

            if (Sys_Chat.Instance.eRecordState == Sys_Chat.ERecodeVoice.Recording)
            {
                if (Sys_Chat.Instance.IsVaildRecord)
                {                 
                    _txt.text = LanguageHelper.GetTextContent(11924);//"手指滑开，取消发送";
                }
                else
                {                    
                    _txt.text = LanguageHelper.GetTextContent(11925); //"松开手指，取消发送";
                }

                float current = Time.unscaledTime - Sys_Chat.Instance.fStartRecordTime;
                
                _slider.fillAmount = current / Sys_Chat.Instance.fMaxRecordTime;
                tweener = _slider.DOFillAmount(1, Sys_Chat.Instance.fMaxRecordTime - current).SetEase(Ease.Linear);
            }
            else
            {
                CloseSelf(true);
            }                
        }
    }
}