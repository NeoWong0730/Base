using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;

namespace Logic
{
    public partial class UI_Goddness_Charac:UIBase, UI_Goddness_Charac_Layout.IListener
    {
        UI_Goddness_Charac_Layout m_Layout = new UI_Goddness_Charac_Layout();
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }
        protected override void OnOpen(object arg)
        {            
            Sys_GoddnessTrial.Instance.SendTopicProperty();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.TopicProperty, OnRefreshProperty, toRegister);
        }


        protected override void OnShow()
        {
            Refresh();
        }
        private void Refresh()
        {
            var data = CSVGoddessTopic.Instance.GetConfData(Sys_GoddnessTrial.Instance.SelectID);

            if (data == null)
                return;

            int count = 0;

            int index = (int)Sys_GoddnessTrial.Instance.TopicProperty;
            var skills = data.monsterCharacter[index];
            var skillscount = skills.Count;

            bool ishaveskill = false;
            if (skills.Count > 0 && skills[0] != 0)
            {
                count += skillscount;
                ishaveskill = true;
            }


            var ais = data.aiCharacter[index];
            var aiscount = ais.Count;
            bool ishaveai = false;
            if (ais.Count > 0 && ais[0] != 0)
            {
                count += aiscount;
                ishaveai = true;
            }

            m_Layout.SetPropeItemCount(count);
            int itemindex = 0;
            if (ishaveskill)
            {
                
                for (int i = 0; i < skillscount; i++,itemindex++)
                {
                    var skilldata = CSVPassiveSkillInfo.Instance.GetConfData(skills[i]);

                    m_Layout.SetPropeName(itemindex, skilldata.name);
                    m_Layout.SetPropeInfo(itemindex, skilldata.desc);
                    m_Layout.SetPropeIcon(itemindex, skilldata.icon);
                }
            }

            if (ishaveai)
            {
                for (int i = 0; i < aiscount; i++,itemindex++)
                {
                    var aidata = CSVAICharacter.Instance.GetConfData(ais[i]);

                    m_Layout.SetPropeName(itemindex, aidata.titleLan);
                    m_Layout.SetPropeInfo(itemindex, aidata.disLan);
                    m_Layout.SetPropeIcon(itemindex, aidata.iconId);
                }
            }

        }

        private void OnRefreshProperty()
        {
            Refresh();
        }
    }

    public partial class UI_Goddness_Charac : UIBase, UI_Goddness_Charac_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }
    }
}
