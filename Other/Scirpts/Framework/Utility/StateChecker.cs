using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework
{
    // ���ڴ��������򣬽���/�뿪 ����Ķ�̬���
    // ���Ӻ��ĵĹ����ǣ� ��Update��һֱ������ʽ ת��Ϊ event��ʽ
    public class StateChecker
    {
        private Func<bool> checker;

        public Action<bool, bool> onValueChanged;

        private int _checkFrequency = 8;
        public int checkFrequency
        {
            get
            {
                return _checkFrequency;
            }
            set
            {
                _checkFrequency = value;
                if (_checkFrequency <= 0)
                {
                    _checkFrequency = 1;
                }
            }
        }

        private bool _state = false;
        public bool State
        {
            get
            {
                return _state;
            }
            private set
            {
                if (_state != value)
                {
                    bool old = _state;
                    _state = value;
                    onValueChanged?.Invoke(old, _state);
                }
            }
        }

        public StateChecker(bool originalState, Func<bool> checker, Action<bool, bool> onValueChanged, int frequency = 1)
        {
            this.checker = checker;
            this.onValueChanged = onValueChanged;

            _state = originalState;
            _checkFrequency = frequency;
        }
        public StateChecker(Func<bool> originalState, Func<bool> checker, Action<bool, bool> onValueChanged, int frequency = 1)
        {
            this.checker = checker;
            this.onValueChanged = onValueChanged;

            _state = originalState.Invoke();
            _checkFrequency = frequency;
        }

        public void Check(bool focrce = false)
        {
            if (focrce || Time.frameCount % checkFrequency == 0)
            {
                State = checker.Invoke();
            }
        }
    }
}