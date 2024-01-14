using Lib.AssetLoader;
using Lib.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using Framework;

namespace Logic
{
    public class UI_TeamMain_AutoFind_Layout
    {
        //自动匹配
        private Transform m_AutoFind;
        private Button m_BtnExitAutoFind;
        private Text m_TexTitle;

        UI_TeamMain_Layout.IListener m_listener;
        public void Show()
        {
            m_AutoFind.gameObject.SetActive(true);
        }

        public void Hide()
        {
            m_AutoFind.gameObject.SetActive(false);
        }

        public void Load(Transform parent)
        {
            m_AutoFind = parent.Find("TeamShow/Team_Match");

            m_BtnExitAutoFind = m_AutoFind.Find("Button_Quite").GetComponent<Button>();

            m_TexTitle = m_AutoFind.Find("Text_Title").GetComponent<Text>();


            m_BtnExitAutoFind.onClick.AddListener(OnClickExit);

        }
        public void RegisterEvents(UI_TeamMain_Layout.IListener listener, bool state)
        {
            m_listener = listener;
        }

        private void OnClickExit()
        {
            m_listener?.ExitAutoFind();

        }

        public void SetTitle(string str)
        {
            m_TexTitle.text = str;
        }
    }

}
