using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

namespace NWFramework
{
    [Window(UILayer.System, fromResources:true)]
    internal sealed class LogUI : UIWindow
    {
        private Stack<string> _errorTextString = new Stack<string>();

        private Text m_textError;
        private Button m_btnClose;

        public override void ScriptGenerator()
        {
            m_textError = FindChildComponent<Text>("m_textError");
            m_btnClose = FindChildComponent<Button>("m_btnClose");
            m_btnClose.onClick.AddListener(OnClickCloseBtn);
        }

        private void OnClickCloseBtn()
        {
            PopErrorLog().Forget();
        }

        public override void OnRefresh()
        {
            _errorTextString.Push(UserData.ToString());
            m_textError.text = UserData.ToString();
        }

        private async UniTaskVoid PopErrorLog()
        {
            if (_errorTextString.Count <= 0)
            {
                await UniTask.Yield();
                Close();
                return;
            }

            string error = _errorTextString.Pop();
            m_textError.text = error;
        }
    }
}