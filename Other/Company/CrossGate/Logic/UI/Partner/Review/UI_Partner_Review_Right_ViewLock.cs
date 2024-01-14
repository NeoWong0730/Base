using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using System.Collections.Generic;
using Packet;

namespace Logic
{
    public class UI_Partner_Review_Right_ViewLock
    {

        private Transform transform;

        private Slider sliderExp;
        private Text textPercent;

        private UI_Partner_Review_Right_Info_Polygon_Attr polygon;
        private UI_Partner_Review_Right_ViewLockInfo viewInfo;
        private UI_Partner_Review_Right_Info_Skill skillInfo;

        public void Init(Transform trans)
        {
            transform = trans;

            sliderExp = transform.Find("Slider_Exp").GetComponent<Slider>();
            textPercent = transform.Find("Slider_Exp/Text_Percent").GetComponent<Text>();

            polygon = new UI_Partner_Review_Right_Info_Polygon_Attr();
            polygon.Init(transform.Find("Radar"));

            viewInfo = new UI_Partner_Review_Right_ViewLockInfo();
            viewInfo.Init(transform.Find("View_Lock"));

            skillInfo = new UI_Partner_Review_Right_Info_Skill();
            skillInfo.Init(transform.Find("Scroll View"));
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
            viewInfo.OnDestroy();
        }

        public void UpdateInfo(uint infoId)
        {
            polygon.UpdateInfo(infoId);
            viewInfo.UpdateInfo(infoId);
            skillInfo.UpdateInfo(infoId);

            sliderExp.value = 0;
            textPercent.text = "";
        }
    }
}


