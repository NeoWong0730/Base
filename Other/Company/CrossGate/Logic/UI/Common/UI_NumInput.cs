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
    public class UI_NumInput : UIBase
    {
        private class Num : UIComponent
        {
            private Button m_Btn;

            private int m_Num;
            private bool m_Close;            
            protected override void Loaded()
            {
                m_Btn = transform.Find("Image_BG").GetComponent<Button>();
                m_Btn.onClick.AddListener(OnClickNum);
            }

            private void OnClickNum()
            {
                if (m_Close)
                {
                    UIManager.CloseUI(EUIID.UI_NumInput);
                }
                else
                {
                    //发送通知 
                    Sys_NumInput.Instance.eventEmitter.Trigger(Sys_NumInput.EEvents.OnNumInput, m_Num);
                }
            }

            public void SetNum(int num, bool close = false)
            {
                m_Num = num;
                m_Close = close;
            }
        }


        private Button _btnClose;

        private Num[] nums = new Num[12];
        private RectTransform _TransRect;

        private Sys_NumInput.NumInputPrama _param = null;
        private Vector2 _size = Vector2.zero;

        protected override void OnLoaded()
        {
            _btnClose = transform.Find("Image_BG").GetComponent<Button>();
            _btnClose.onClick.AddListener(OnClickClose);

            _TransRect = transform.Find("Num_Input").GetComponent<RectTransform>();
            _size = _TransRect.rect.size;

            for (int i = 0; i < 10; ++i)
            {
                string str = string.Format("Num_Input/Rect/Item{0}", i);
                nums[i] = AddComponent<Num>(transform.Find(str));
                nums[i].SetNum(i);
            }

            nums[10] = AddComponent<Num>(transform.Find("Num_Input/Rect/Item10"));
            nums[10].SetNum(-1);

            nums[11] = AddComponent<Num>(transform.Find("Num_Input/Rect/Item11"));
            nums[11].SetNum(0, true);
        }

        protected override void OnOpen(object arg1 = null)
        {
            _param = null;
            if (arg1 != null)
            {
                _param = (Sys_NumInput.NumInputPrama)arg1;
            }
        }

        protected override void OnShow()
        {
            switch (_param.Dir)
            {
                case Sys_NumInput.NumInputOffsetDir.None:
                    _TransRect.position = _param.Position;
                    break;
                case Sys_NumInput.NumInputOffsetDir.Left:
                    _TransRect.position = _param.Position;
                    _TransRect.localPosition -= new Vector3(_size.x * 0.5f, 0f, 0f);
                    break;
                case Sys_NumInput.NumInputOffsetDir.Right:
                    _TransRect.position = _param.Position;
                    _TransRect.localPosition += new Vector3(_size.x * 0.5f, 0f, 0f);
                    break;
                case Sys_NumInput.NumInputOffsetDir.Top:
                    _TransRect.position = _param.Position;
                    _TransRect.localPosition += new Vector3(0f, _size.y * 0.5f, 0f);
                    break;
                case Sys_NumInput.NumInputOffsetDir.Bottom:
                    _TransRect.position = _param.Position;
                    _TransRect.localPosition -= new Vector3(0f, _size.y * 0.5f, 0f);
                    break;
            }
        }

        protected override void OnHide()
        {
            Sys_NumInput.Instance.eventEmitter.Trigger(Sys_NumInput.EEvents.OnNumInputEnd);
        }

        private void OnClickClose()
        {
            UIManager.CloseUI(EUIID.UI_NumInput);
        }
    }
}



