using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_Partner_Review_Right
    {
        private Transform transform;

        

        //private UI_Partner_Review_Right_ViewInfo viewInfo;
        //private UI_Partner_Review_Right_Info viewReview;
        //private UI_Partner_Review_Right_Property viewProperty;
        private UI_Partner_Review_Right_ViewLock viewLock;
        private UI_Partner_Review_Right_ViewUnlock viewUnlock;

        public void Init(Transform trans)
        {
            transform = trans;

            //Transform transPage = transform.Find("Menu_Level");
            //for (int i = 0; i < transPage.childCount; ++i)
            //{
            //    GameObject go = transPage.GetChild(i).gameObject;
            //    CP_Toggle toggle = go.GetComponent<CP_Toggle>();
            //    listToggles.Add(toggle);
            //    toggle.onValueChanged.AddListener((isOn) =>
            //    {
            //        OnClickPage(isOn, listToggles.IndexOf(toggle));
            //    });
            //}

            //viewInfo = new UI_Partner_Review_Right_ViewInfo();
            //viewInfo.Init(transform.Find("View_Info"));

            //viewReview = new UI_Partner_Review_Right_Info();
            //viewReview.Init(transform.Find("View_Review"));

            //viewProperty = new UI_Partner_Review_Right_Property();
            //viewProperty.Init(transform.Find("View_Property"));

            viewLock = new UI_Partner_Review_Right_ViewLock();
            viewLock.Init(transform.Find("View_Lock"));

            viewUnlock = new UI_Partner_Review_Right_ViewUnlock();
            viewUnlock.Init(transform.Find("View_Unlock"));
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
            viewLock?.OnDestroy();
        }

        public void UpdateInfo(uint infoId)
        {
            //viewLock.UpdateInfo(infoId);
            //viewInfo.UpdateInfo(infoId);
            //viewReview.Show();
            //viewReview.UpdateInfo(infoId);
            //viewProperty.Hide();
            //viewProperty.UpdateInfo(infoId);

            //listToggles[0].SetSelected(true, true);
            //bool isUnlock = Sys_Partner.Instance.IsUnLock(infoId);
            //listToggles[1].enabled = isUnlock;
            //ImageHelper.SetImageGray(listToggles[1], !isUnlock, true);

            bool isUnLock = Sys_Partner.Instance.IsUnLock(infoId);
            if (!isUnLock)
            {
                viewUnlock.Hide();
                viewLock.Show();
                viewLock.UpdateInfo(infoId);
            }
            else
            {
                viewLock.Hide();
                viewUnlock.Show();
                viewUnlock.UpdateInfo(infoId);
            }
        }
    }
}


