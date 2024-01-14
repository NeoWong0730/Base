using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_MainBattle_SparSkills_Layout
    {
        public Transform transform;
        public GameObject skillGo;
        public Button closeBtn;
        public Image headIcon;
        public Text name;
        public Text tips;
        public GameObject scrollskillGo;
        public GameObject skillmessageGo;
        public GameObject noskillGo;
        public GameObject pointGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            skillGo = transform.Find("View_Main/Scroll_View/Viewport/Item").gameObject;
            closeBtn = transform.Find("Button_Close").GetComponent<Button>();
            headIcon = transform.Find("View_Main/Image_Title/Image_Frame/Image_Icon").GetComponent<Image>();
            name = transform.Find("View_Main/Image_Title/Text_Name").GetComponent<Text>();
            tips = transform.Find("View_Main/Text_Tips").GetComponent<Text>();
            scrollskillGo = transform.Find("View_Main/Scroll_View").gameObject;
            skillmessageGo = transform.Find("View_Message").gameObject;
            noskillGo = transform.Find("View_Main/Image_Nootherskill").gameObject;
            pointGo = transform.Find("View_Main/Text_Energy/EnergyGroup/EnergyDark").gameObject;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
        }
    }

    public class UI_MainBattle_SparSkills : UIBase, UI_MainBattle_SparSkills_Layout.IListener
    {
        private UI_MainBattle_SparSkills_Layout layout = new UI_MainBattle_SparSkills_Layout();
        private List<UI_Skill> skills;
        private UI_SkillMessage skillmessage;
        private bool isfirst;
        private Timer timer;

        protected override void OnLoaded()
        {
            skills = new List<UI_Skill>();
            layout.Init(transform);
            layout.RegisterEvents(this);
            skillmessage = AddComponent<UI_SkillMessage>(layout.skillmessageGo.transform);
        }

        protected override void OnShow()
        {
            SetValue();
            Vector3 pos = new Vector3();
            pos = layout.skillGo.GetComponentInParent<RectTransform>().position;
            AddList();
            layout.skillmessageGo.SetActive(false);
        }

        protected override void OnHide()
        {
            layout.scrollskillGo.SetActive(true);
            layout.pointGo.transform.parent.gameObject.SetActive(true);
            FrameworkTool.DestroyChildren(layout.pointGo.transform.parent.gameObject, layout.pointGo.transform.name);
            DefaultSkill();
        }

        private void SetValue()
        {
            uint perAddCount;
            uint firstCount;
            uint maxCount;
            uint.TryParse(CSVParam.Instance.GetConfData(721).str_value,out perAddCount);
            uint.TryParse(CSVParam.Instance.GetConfData(722).str_value, out firstCount);
            uint.TryParse(CSVParam.Instance.GetConfData(720).str_value, out maxCount);
            layout.name.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
            layout.tips.text = LanguageHelper.GetTextContent(2021042, perAddCount.ToString(), firstCount.ToString(), maxCount.ToString());
            CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
            ImageHelper.SetIcon(layout.headIcon, heroData.headid);
            FrameworkTool.CreateChildList(layout.pointGo.transform.parent,10);
            int count = (int)Net_Combat.Instance.m_EnergePoint;
            for (int i = 0; i < layout.pointGo.transform.parent.childCount; i++)
            {
                Transform child = layout.pointGo.transform.parent.GetChild(i);
                if (i < count)
                {
                    child.Find("EnergyLight").gameObject.SetActive(true);
                }
                else
                {
                    child.Find("EnergyLight").gameObject.SetActive(false);
                }
            }
        }

        private void AddList()
        {
            skills.Clear();
            foreach (var skillInfo in Sys_StoneSkill.Instance.serverDataDic)
            {
                uint skillId = Sys_StoneSkill.Instance.GetPowerStoneActiveSkill(skillInfo.Value.powerStoneUnit.Id, skillInfo.Value.powerStoneUnit.Level);
                if (CSVActiveSkill.Instance.ContainsKey(skillId))
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.skillGo, layout.skillGo.transform.parent);
                    UI_Skill skill = new UI_Skill(OnSelectSkill, skillId, isfirst, false);
                    skill.Init(go.transform);
                    skill.isSkill = true;
                    skill.isAvailable = true;
                    skill.Refresh(skillId, false, false, true);
                    skills.Add(skill);
                }
            }
            layout.skillGo.SetActive(false);
            if (skills.Count == 0 && !Sys_Fight.Instance.AutoFightData.AutoState)
            {
                layout.scrollskillGo.SetActive(false);
            }
        }

        private void OnSelectSkill(uint id, bool ispetattack)
        {
            layout.skillmessageGo.SetActive(true);
            skillmessage.Refreshmessage(id, ispetattack,true);
        }

        private void DefaultSkill()
        {
            layout.skillGo.SetActive(true);
            for (int i=0;i< skills.Count;++i) { skills[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.skillGo.transform.parent.gameObject, layout.skillGo.transform.name);
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_SparSkills);
        }
    }
}
