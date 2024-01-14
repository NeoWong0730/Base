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
using System;

namespace Logic
{
    public class UI_TeamMain_MenuTeamMem_Layout
    {
        //点击队伍成员菜单
        private Transform m_MenuTeamMem;
        private Transform m_MenuTeamMemContent;
        private Button m_BtnClose;
        public Action<int> ClickMenuAction;

        public Action onClickCloseAction;
        class MenuTeamMemItem
        {
            public Transform transform;
            public Button m_Btn;
            public Text m_Text;

            public int Index { get; set; }

           // private int Command;
            public uint Commamd { set { TextHelper.SetText(m_Text, value); } }
            public void OnClick()
            {
                CallBackAction?.Invoke(Index);
            }

            public Action<int> CallBackAction;
        }
        private List<MenuTeamMemItem> menuTeamMemItems = new List<MenuTeamMemItem>();

        public void Show()
        {
            m_MenuTeamMem.gameObject.SetActive(true);

            //ContentSizeFitter sizeFit  = m_MenuTeamMem.GetComponent<ContentSizeFitter>();

            //sizeFit.SetLayoutVertical();
        }

        public void Hide()
        {
            m_MenuTeamMem.gameObject.SetActive(false);
        }

        public void Load(Transform parent)
        {
            m_MenuTeamMem = parent.Find("TeamShow/Team_Button");

            m_BtnClose = m_MenuTeamMem.GetComponent<Button>();


            m_MenuTeamMemContent = m_MenuTeamMem.Find("menu");

            for (int i = 0; i < m_MenuTeamMemContent.childCount; i++)
            {
                Transform transform = m_MenuTeamMemContent.Find("Button0" + (i + 1));

                if (transform == null)
                    continue;

                MenuTeamMemItem item = new MenuTeamMemItem();
                LoadItem(transform, item);
                menuTeamMemItems.Add(item);
                item.Index = i;

                transform.gameObject.SetActive(false);
            }


            if (m_BtnClose != null)
            {
                m_BtnClose.onClick.AddListener(OnClickClose);
            }
        }

        private void LoadItem(Transform transform, MenuTeamMemItem item)
        {
            item.transform = transform;
            item.m_Btn = transform.GetComponent<Button>();
            item.m_Text = transform.Find("Text").GetComponent<Text>();

            item.m_Btn.onClick.AddListener(item.OnClick);

        }
        private Vector3[] menuPoints = { Vector3.zero,Vector3.zero,Vector3.zero,Vector3.zero};
        public void Show(Vector3[] points)
        {
            Show();

            RectTransform rect = m_MenuTeamMemContent as RectTransform;

            rect.GetWorldCorners(menuPoints);

            Vector3 offvec = rect.position + (points[1] - menuPoints[2]);

            rect.position = offvec;

        }


        public void SetMenu(List<uint>command)
        {
            for (int i = 0; i < menuTeamMemItems.Count; i++)
            {
                menuTeamMemItems[i].transform.gameObject.SetActive(i < command.Count);

                if (i < command.Count)
                {
                    menuTeamMemItems[i].Commamd = command[i];
                    menuTeamMemItems[i].CallBackAction = OnClickMenu;
                }
            }

            m_MenuTeamMemContent.gameObject.SetActive(command.Count > 0);
        }

        private void OnClickMenu(int index)
        {
            ClickMenuAction?.Invoke(index);
        }

        private void OnClickClose()
        {
            onClickCloseAction?.Invoke();
        }
    }


}
