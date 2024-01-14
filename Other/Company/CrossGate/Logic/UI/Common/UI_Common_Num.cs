using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_Common_Num
    {
        private Transform transform;

        private RectTransform rectTransform;
        private Button _btnNum;
        private Text _textNum;

        private uint _numValue;
        private bool _isFirstInput = false;
        private System.Action<uint> _inputChange;
        private System.Action<uint> _inputEnd;
        private Sys_NumInput.NumInputOffsetDir _OffsetDir = Sys_NumInput.NumInputOffsetDir.Top;

        private uint _maxCount;

        public void Init(Transform trans, uint maxCount=0)
        {
            transform = trans;
            rectTransform = transform.GetComponent<RectTransform>();

            _btnNum = transform.GetComponent<Button>();
            _btnNum.onClick.AddListener(OnClickNum);

            _textNum = transform.Find("Text").GetComponent<Text>();
            _maxCount = maxCount;
        }

        public void RegChange(System.Action<uint> action)
        {
            _inputChange = action;
        }

        public void RegEnd(System.Action<uint> action)
        {
            _inputEnd = action;
        }

        public void SetOffset(Sys_NumInput.NumInputOffsetDir offsetDir)
        {
            _OffsetDir = offsetDir;
        }

        public void SetData(uint num, string dataType = "")
        {
            _numValue = num;
            _textNum.text = _numValue.ToString(dataType);
        }

        public void Dsiplay(string content)
        {
            _textNum.text = content;
        }

        public void Reset()
        {
            _numValue = 0u;
            _textNum.text = "";
        }

        public void SetDefault(string str)
        {
            _textNum.text = str;
        }

        private void OnClickNum()
        {
            _isFirstInput = false;
            UIManager.OpenUI(EUIID.UI_NumInput, false, new Sys_NumInput.NumInputPrama(rectTransform.position, _OffsetDir));
            Sys_NumInput.Instance.eventEmitter.Handle<int>(Sys_NumInput.EEvents.OnNumInput, OnNumInput, true);
            Sys_NumInput.Instance.eventEmitter.Handle(Sys_NumInput.EEvents.OnNumInputEnd, OnNumInputEnd, true);
        }

        private void OnNumInput(int num)
        {
            if (!_isFirstInput)     //第一次输入覆盖
            {
                _isFirstInput = true;
                if (num >= 0)
                {
                    _numValue = (uint)num;
                }
                else
                {
                    _numValue /= 10;
                }
            }
            else
            {
                if (num >= 0)
                {
                    _numValue = (uint)(_numValue * 10 + num);
                }
                else
                {
                    _numValue /= 10;
                }
            }
            uint max = _maxCount == 0 ? Sys_NumInput.Instance.GetInputValueMax() : _maxCount;
            //输入最大限制
            if (_numValue > max)
            {
                _numValue = max;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011201,_numValue.ToString()));
            }

            _textNum.text = _numValue.ToString();

            _inputChange?.Invoke(_numValue);
        }

        private void OnNumInputEnd()
        {
            Sys_NumInput.Instance.eventEmitter.Handle<int>(Sys_NumInput.EEvents.OnNumInput, OnNumInput, false);
            Sys_NumInput.Instance.eventEmitter.Handle(Sys_NumInput.EEvents.OnNumInputEnd, OnNumInputEnd, false);

            if (_isFirstInput) //如果有输入，才调用end
                _inputEnd?.Invoke(_numValue);
        }
    }
}


