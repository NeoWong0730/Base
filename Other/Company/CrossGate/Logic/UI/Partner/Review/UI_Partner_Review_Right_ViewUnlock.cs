using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Partner_Review_Right_ViewUnlock
    {
        private Transform transform;

        //private UI_Partner_Review_Right_ViewLockInfo viewInfo;
        private UI_Partner_Review_Right_ViewInfo viewInfo;
        private UI_Partner_Review_Right_Info_Basic basic;
        //private UI_Partner_Review_Right_Info viewReview;
        private UI_Partner_Review_Right_Info_Skill skillInfo;
        private UI_Partner_Review_Right_Property viewProperty;

        private List<CP_Toggle> listToggles = new List<CP_Toggle>(2);

        public void Init(Transform trans)
        {
            transform = trans;

            viewInfo = new UI_Partner_Review_Right_ViewInfo();
            viewInfo.Init(transform.Find("View_Info"));

            basic = new UI_Partner_Review_Right_Info_Basic();
            basic.Init(transform.Find("Basic_Attr"));

            skillInfo = new UI_Partner_Review_Right_Info_Skill();
            skillInfo.Init(transform.Find("View_Skill/Scroll View"));

            viewProperty = new UI_Partner_Review_Right_Property();
            viewProperty.Init(transform.Find("View_Property"));

            Transform transPage = transform.Find("Menu_Level");
            for (int i = 0; i < transPage.childCount; ++i)
            {
                GameObject go = transPage.GetChild(i).gameObject;
                CP_Toggle toggle = go.GetComponent<CP_Toggle>();
                if (toggle != null)
                {
                    listToggles.Add(toggle);
                    toggle.onValueChanged.AddListener((isOn) =>
                    {
                        OnClickPage(isOn, listToggles.IndexOf(toggle));
                    });
                }
               
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void OnDestroy()
        {

        }

        private void OnClickPage(bool _select, int _index)
        {
            if (_select)
            {
                bool showInfo = _index == 0;
                if (showInfo)
                {
                    skillInfo.Show();
                    viewProperty.Hide();
                }
                else
                {
                    skillInfo.Hide();
                    viewProperty.Show();
                }
            }
        }

        public void UpdateInfo(uint infoId)
        {
            viewInfo.UpdateInfo(infoId);
            basic.UpdateInfo(infoId);
            skillInfo.UpdateInfo(infoId);
            viewProperty.UpdateInfo(infoId);

            listToggles[0].SetSelected(true, true);
            //bool isUnlock = Sys_Partner.Instance.IsUnLock(infoId);
            //listToggles[1].enabled = isUnlock;
            //ImageHelper.SetImageGray(listToggles[1], !isUnlock, true);
        }
    }
}


