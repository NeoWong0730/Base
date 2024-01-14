using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Partner_Review_Right_Info
    {
        private Transform transform;

        private UI_Partner_Review_Right_Info_Polygon_Attr polygon;
        private UI_Partner_Review_Right_Info_Basic basic;
        private UI_Partner_Review_Right_Info_Skill skill;
        private UI_Partner_Review_Right_Info_Passive_Skill passiveSkill;

        private uint _infoId;

        public void Init(Transform trans)
        {
            transform = trans;

            polygon = new UI_Partner_Review_Right_Info_Polygon_Attr();
            polygon.Init(transform.Find("Radar"));

            basic = new UI_Partner_Review_Right_Info_Basic();
            basic.Init(transform.Find("Basic_Attr"));

            skill = new UI_Partner_Review_Right_Info_Skill();
            skill.Init(transform.Find("Scroll View/Viewport/Content/Skill_Grid1"));

            passiveSkill = new UI_Partner_Review_Right_Info_Passive_Skill();
            passiveSkill.Init(transform.Find("Scroll View/Viewport/Content/Skill_Grid2"));
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void UpdateInfo(uint infoId)
        {
            _infoId = infoId;

            polygon.Hide();
            basic.Hide();

            bool isUnlock = Sys_Partner.Instance.IsUnLock(_infoId);
            if (isUnlock)
            {
                basic.Show();
                basic.UpdateInfo(_infoId);
            }
            else
            {
                polygon.Show();
                polygon.UpdateInfo(_infoId);
            }
            
            skill.UpdateInfo(_infoId);
            passiveSkill.UpdateInfo(_infoId);
        }
    }
}


