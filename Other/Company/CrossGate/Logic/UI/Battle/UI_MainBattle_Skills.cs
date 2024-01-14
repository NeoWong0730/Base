using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{  
    public class UI_MainBattle_Skills_Layout
    {
        public Transform transform;
        public GameObject skillGo;
        public Button closeBtn;
        public Image headIcon;
        public Text name;
        public GameObject viewLeft;
        public GameObject scrollskillGo;
        public GameObject noskillView;
        public GameObject skillmessageGo;
        public GameObject nootherskillGo;
        public GameObject nopetskillGo;
        public GameObject noroleskillGo;

        public void Init(Transform transform)
        {
            this.transform = transform;
            skillGo = transform.Find("View_Main/Scroll_View/Viewport/Item").gameObject;
            closeBtn = transform.Find("Button_Close").GetComponent<Button>();
            headIcon = transform.Find("View_Main/Image_Title/Image_Frame/Image_Icon").GetComponent<Image>();
            name = transform.Find("View_Main/Image_Title/Text_Name").GetComponent<Text>();
            viewLeft = transform.Find("View_Left").gameObject;
            scrollskillGo = transform.Find("View_Main/Scroll_View").gameObject;
            noskillView = transform.Find("View_NoSkills").gameObject;
            noroleskillGo = transform.Find("View_NoSkills/Image_Icon").gameObject;
            nopetskillGo = transform.Find("View_NoSkills/Image_Icon_Pet").gameObject;
            skillmessageGo = transform.Find("View_Message").gameObject;
            nootherskillGo = transform.Find("View_Main/Image_Nootherskill").gameObject;
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

    public class UI_Skill : UIComponent
    {
        private GameObject normalshow;
        private GameObject selectshow;
        private GameObject forbidshow;
        private GameObject talentBgGo;
        private GameObject noMpGo;
        private GameObject mpInfoGo;
        private Button normalBtn;
        private Button forbidBtn;
        private Button cdGo;
        private Image icon;
        private Image typeicon;
        private Image normalBtnImage;
        private Text level;
        private Text name;
        private Text cdText;
        private Text mpNum;
        private Action<uint,bool> onClickSkill;
        private uint id;
        private static GameObject preObj=null;
        public bool isSkill;
        private bool isfirst;
        public bool isAvailable;
        private bool ispetattack;
        private bool haveCd;
        private long cdTime;

        private CSVActiveSkillInfo.Data csvActiveSkillInfoData;
        private CSVActiveSkill.Data csvActiveSkillData;
        private SkillColdDwonInfo cdInfo;
        private uint skillLimitId;
        private CSVChoseSkillLimit.Data csvChoseSkillLimitData;
        private List<MobBuffComponent.BuffData> buffs=new List<MobBuffComponent.BuffData> ();
        private List<MobEntity> m_EnemytList = new List<MobEntity>();

        public UI_Skill(Action<uint,bool> action,uint _id,bool _isfirst,bool _ispetattack) : base()
        {
            onClickSkill = action;
            id = _id;
            isfirst = _isfirst;
            ispetattack = _ispetattack;
        }

        protected override void Loaded()
        {
            normalshow = transform.Find("Button_Normal").gameObject;            
            selectshow = transform.Find("Image_Select").gameObject;
            forbidshow = transform.Find("Button_Forbid").gameObject;
            talentBgGo = transform.Find("Image_TalentIBG").gameObject;
            noMpGo = transform.Find("MP_Insufficient_Mask").gameObject;
            mpInfoGo= transform.Find("SkillMP_bg").gameObject;
            normalBtn = normalshow.GetComponent<Button>();
            normalBtn.onClick.AddListener(OnnormalBtnClick);
            normalBtn.GetComponent<UI_LongPressButton>().onStartPress.AddListener(OnnormalBtnLongClick);
            cdGo = transform.Find("Image").GetComponent<Button>();
            cdGo.onClick.AddListener(OnCdClicked);
            cdGo.GetNeedComponent<UI_LongPressButton>().onStartPress.AddListener(OnnormalBtnLongClick);
            forbidBtn = forbidshow.GetComponent<Button>();
            icon = transform.Find("Image_Icon").GetComponent<Image>();
            typeicon = transform.Find("Image_Icon/Image_Tip").GetComponent<Image>();
            normalBtnImage = normalshow.GetComponent<Image>();
            level = transform.Find("Text_Level").GetComponent<Text>();
            name = transform.Find("Text_Name").GetComponent<Text>();
            cdText = transform.Find("Image/Text_Level (1)").GetComponent<Text>();
            mpNum = transform.Find("SkillMP_bg/Text").GetComponent<Text>();
        }

        private void OnCdClicked()
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4405, cdTime.ToString()));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnSkillMessageOff, OnSkillMessageOff, toRegister);
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnRoundNtf, OnRoundNtf, toRegister);
        }

        private void OnSkillMessageOff()
        {
            if (selectshow == null )
            {              
                return;
            }
            else
            {
                selectshow.SetActive(false);
                preObj = null;
            }
        }

        private void OnRoundNtf()
        {
            UpdateSkillCd();
        }

        private void OnnormalBtnLongClick()
        {
            if (preObj != null && preObj.activeInHierarchy)
            {
                preObj.SetActive(false);              
            }
            selectshow.SetActive(true);
            onClickSkill(id,ispetattack);
            preObj = selectshow;
        }

        private void OnnormalBtnClick()
        {
            if (skillLimitId != 0)
            {
                if (skillLimitId == 101)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(csvChoseSkillLimitData.tips, GetNeedWeaponType(id)));
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(csvChoseSkillLimitData.tips));
                }
                AudioUtil.PlayAudio(csvChoseSkillLimitData.audio);
            }
            else
            {
                if (Sys_Fight.Instance.AutoFightData.AutoState)
                {
                    if (Sys_Fight.Instance.HasPet())
                    {
                        if (isfirst)
                        {
                            Sys_Fight.Instance.SetAutoSkillReq(id, isfirst, true);
                        }
                        else
                        {
                            Sys_Fight.Instance.SetAutoSkillReq(id, isfirst, false);
                        }
                    }
                    else
                    {
                        Sys_Fight.Instance.SetAutoSkillReq(id, isfirst, true);
                    }
                    if (isSkill)
                    {
                        GameCenter.fightControl.CanUseSkill = false;
                    }

                }
                else
                {
                    GameCenter.fightControl.AttackById(id, 0);
                    UIManager.CloseUI(EUIID.UI_MainBattle_Skills);
                    UIManager.CloseUI(EUIID.UI_MainBattle_SparSkills);
                }
                if(csvActiveSkillData!=null && GameCenter.mainFightHero.battleUnit.CurMp < csvActiveSkillData.mana_cost)
                {
                    Sys_Hint.Instance.PushContent_InBattle(LanguageHelper.GetTextContent(3000010));
                }
            }
        }

        private string GetNeedWeaponType(uint id)
        {
            string str=null;
            for(int i=0;i< csvActiveSkillInfoData.require_weapon_type.Count;++i)
            {
                str += LanguageHelper.GetTextContent(290000000 + csvActiveSkillInfoData.require_weapon_type[i]);
            }
            return str;
        }

        public void Refresh(uint key,bool isPetAttack,bool isPetSkill,bool isTalentSkill=false)
        {
            MobEntity mobEntity = MobManager.Instance.GetMob(GameCenter.mainFightHero.battleUnit.UnitId);
            if (mobEntity != null)
            {
                MobBuffComponent mobBuffComponent = mobEntity.GetComponent<MobBuffComponent>();
                if (mobBuffComponent != null)
                {
                    buffs = mobBuffComponent.m_Buffs;
                }
            }
            csvActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(key);
            csvActiveSkillData = CSVActiveSkill.Instance.GetConfData(key);
            if (key == 1001 && isPetAttack)
            {
                ImageHelper.SetIcon(icon, 992302);
            }
            else
            {
                ImageHelper.SetIcon(icon, csvActiveSkillInfoData.icon);
            }
            if (isPetSkill)
            {
                level.text = String.Empty;
            }
            else
            {
                level.text = csvActiveSkillInfoData.level.ToString();
            }
            name.text = LanguageHelper.GetTextContent(csvActiveSkillInfoData.name);
            if (csvActiveSkillData == null || csvActiveSkillData.mana_cost == 0)
            {
                mpInfoGo.SetActive(false);
            }
            else
            {
                uint mpCost = 0;
                if (Sys_Skill.Instance.commonSkillInfos.ContainsKey(csvActiveSkillInfoData.skill_id)) //特殊处理 得意技能的耗魔
                {
                    mpCost =csvActiveSkillData.mana_cost * uint.Parse(CSVParam.Instance.GetConfData(908).str_value) / 10;
                }
                else
                {
                    mpCost = csvActiveSkillData.mana_cost;
                }
                if ((isPetSkill && GameCenter.mainFightPet.battleUnit.CurMp < mpCost) || (!isPetSkill && GameCenter.mainFightHero.battleUnit.CurMp < mpCost))
                {
                    noMpGo.SetActive(true);
                    mpNum.text = "<color=red>" + mpCost.ToString() + "</color>";
                }
                else
                {
                    noMpGo.SetActive(false);
                    mpNum.text = "<color=#F7F3F0>" + mpCost.ToString() + "</color>";
                }
                mpInfoGo.SetActive(true);
            }
            skillLimitId =GameCenter.fightControl.CheckSkillLimitId(id,isAvailable,isPetSkill, buffs);
            csvChoseSkillLimitData = CSVChoseSkillLimit.Instance.GetConfData(skillLimitId);
            typeicon.gameObject.SetActive(csvActiveSkillInfoData.typeicon != 0);
            if (csvActiveSkillInfoData.typeicon!=0)
            {
                ImageHelper.SetIcon(typeicon, csvActiveSkillInfoData.typeicon);
            }
            if (skillLimitId != 0)             
            {
                ImageHelper.SetImageGray(icon, true);
                ImageHelper.SetImageGray(typeicon, true);
            }
            UpdateSkillCd();
            talentBgGo.SetActive(isTalentSkill);

            if (id == 211)
            {
                ImageHelper.SetIcon(normalBtnImage, 992925);
            }
            else
            {
                ImageHelper.SetIcon(normalBtnImage, 992924);
            }
        }

        private void UpdateSkillCd()
        {
            if (skillLimitId != 601)
                return;
            cdInfo = GameCenter.fightControl.HaveSkillCold(id);
            if (csvActiveSkillData.cold_time != 0 && cdInfo != null)
            {
                cdGo.gameObject.SetActive(true);
                cdTime =csvActiveSkillData.cold_time - (Net_Combat.Instance.m_CurRound - cdInfo.LastRound)+1;
                cdText.text = cdTime.ToString();
            }
            else
            {
                cdGo.gameObject.SetActive(false);
            }
        }
    }
   
    public class UI_SkillMessage : UIComponent
    {
        private Image icon;
        private Text name;
        private Text describe;
        private Button close;
        private bool isStone;
        private Transform point;
        private Text manaCost;

        private CSVActiveSkillInfo.Data cSVActiveSkillInfoData;

        protected override void Loaded()
        {
            icon = transform.Find("Image_Icon_Frame/Image_Icon").GetComponent<Image>();
            name = transform.Find("Text_Name").GetComponent<Text>();
            describe = transform.Find("Image_BG/Image_Frame/Text_Describe").GetComponent<Text>();       
            transform.Find("Button_Close").GetComponent<Button>().onClick.AddListener(OnClick);
            manaCost = transform.Find("Text_Magic/Text").GetComponent<Text>();
        }

        private void OnClick()
        {
            gameObject.SetActive(false);
            if (isStone)
            {
                FrameworkTool.DestroyChildren(point.parent.gameObject,point.transform.name);
            }
            Net_Combat.Instance.eventEmitter.Trigger(Net_Combat.EEvents.OnSkillMessageOff);
        }

        public void Refreshmessage(uint key,bool ispetattack,bool isstone)
        {
            cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(key);
            isStone = isstone;
            if (key == 1001 && ispetattack)
            {
                ImageHelper.SetIcon(icon, 992302);
            }
            else
            {
                ImageHelper.SetIcon(icon, cSVActiveSkillInfoData.icon);
            }
            name.text = LanguageHelper.GetTextContent(cSVActiveSkillInfoData.name);
            manaCost.transform.parent.gameObject.SetActive(!isStone);
            if (isStone)
            {
                point = transform.Find("Text_Energy/EnergyGroup/EnergyDark").transform;
                FrameworkTool.CreateChildList(point.parent, (int)CSVActiveSkill.Instance.GetConfData(key).energy_cost);
            }
            if (CSVActiveSkill.Instance.TryGetValue(key, out CSVActiveSkill.Data data))
            { 
               if(Sys_Skill.Instance.commonSkillInfos.ContainsKey(CSVActiveSkillInfo.Instance.GetConfData(data.id).skill_id)) //特殊处理 得意技能的耗魔
                {
                    manaCost.text = (data.mana_cost * int.Parse(CSVParam.Instance.GetConfData(908).str_value) / 10).ToString();
                }
                else
                {
                    manaCost.text = data.mana_cost.ToString();
                }
            }
            else
            {
                manaCost.text = 0.ToString();
            }
            if(CSVActiveSkillEffective.Instance.TryGetValue(cSVActiveSkillInfoData.effect_id,out CSVActiveSkillEffective.Data dataEff))
            {
                describe.text = Sys_Skill.Instance.GetSkillDesc(cSVActiveSkillInfoData.id);
            }
            else
            {
                describe.text = LanguageHelper.GetTextContent(cSVActiveSkillInfoData.desc);
            }
            if (describe.transform.parent != null)
            {
                FrameworkTool.ForceRebuildLayout(describe.transform.parent.gameObject);
            }
        }
    }

    public class UI_MainBattle_Skills : UIBase, UI_MainBattle_Skills_Layout.IListener
    {
        private UI_MainBattle_Skills_Layout layout = new UI_MainBattle_Skills_Layout();
        private List<UI_Skill> skills;
        private List<uint> petskills;
        private UI_SkillMessage skillmessage;
        private bool isfirst;

        protected override void OnLoaded()
        {
            skills = new List<UI_Skill>();
            petskills = new List<uint>();
            layout.Init(transform);
            layout.RegisterEvents(this);
            skillmessage = AddComponent<UI_SkillMessage>(layout.skillmessageGo.transform);
        }

        protected override void OnOpen(object arg)
        {            
            if(arg!=null)
            isfirst = (bool) arg ;
        }

        protected override void OnShow()
        {
            SetValue();
            Vector3 pos = new Vector3();
            pos = layout.skillGo.GetComponentInParent<RectTransform>().position;
            AddList();
            layout.skillmessageGo.SetActive(false);   
            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnOpenAutoSkill, true);
        }

        protected override void OnHide()
        {
            layout.scrollskillGo.SetActive(true);
            layout.noskillView.SetActive(false);
            layout.viewLeft.SetActive(false);
            DefaultSkill();
        }

        private void SetValue()
        {
            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && Sys_Fight.Instance.HasPet())
            {
                layout.name.text = CSVLanguage.Instance.GetConfData(GameCenter.mainFightPet.cSVPetData.name).words;
                ImageHelper.SetIcon(layout.headIcon, GameCenter.mainFightPet.cSVPetData.icon_id);
            }
            else
            {
                layout.name.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
                ImageHelper.SetIcon(layout.headIcon, heroData.headid);
            }
        }

        private void AddList()
        {
            skills.Clear();
            petskills.Clear();
            DefaultSkill();

            if (GameCenter.fightControl.operationState == FightControl.EOperationState.WaitForSecondOperation && Sys_Fight.Instance.HasPet())
            {
                GetPetSkills();
                SetSkillItem(petskills);
                SetNormalItem(102);
                layout.skillGo.SetActive(false);
                if (skills.Count == 0 && !Sys_Fight.Instance.AutoFightData.AutoState)
                {
                    layout.scrollskillGo.SetActive(false);
                    layout.noskillView.SetActive(true);
                    layout.nopetskillGo.SetActive(true);
                    layout.noroleskillGo.SetActive(false);
                }
                SetAiSkill(true);
            }
            else
            {
                SetHoldingSkillItem(GameCenter.mainFightHero.heroSkillComponent.GetHoldingActiveSkillsExt((uint)GameCenter.mainFightHero.battleUnit.WeaponId));
                SetNormalItem(101);
                SetAiSkill(false);
            }
        }

        private void GetPetSkills()
        {
            List<uint> list = GameCenter.fightControl.GetFightPetSkills((uint)GameCenter.mainFightPet.battleUnit.PetId);
            for (int i = 0; i < list.Count; ++i)
            {
                if (CSVActiveSkill.Instance.TryGetValue(list[i], out CSVActiveSkill.Data data))
                {
                    petskills.Add(list[i]);
                }
            }
            petskills.Sort();
        }

        private void OnSelectSkill(uint id,bool ispetattack)
        {
            layout.skillmessageGo.SetActive(true);
            skillmessage.Refreshmessage(id, ispetattack,false);
         }

        private void SetNormalItem(uint autoId)
        {
            if (Sys_Fight.Instance.AutoFightData.AutoState)
            {
                string[] array = CSVAutoBattle.Instance.GetConfData(autoId).order_normal.Split('|');
                uint[] skillarray = Array.ConvertAll(array, uint.Parse);
                for (int i=0;i< skillarray.Length;++i)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.skillGo, layout.skillGo.transform.parent);
                    go.SetActive(true);
                    bool ispetattack=false;
                    if (autoId == 102 && skillarray[i] == 1001)
                    {
                        ispetattack = true;
                    }
                    UI_Skill skill = new UI_Skill(OnSelectSkill, skillarray[i], isfirst, ispetattack);
                    skill.Init(go.transform);
                    skill.isSkill = false;
                    skill.isAvailable = true;
                    skill.Refresh(skillarray[i], ispetattack,false);
                    skills.Add(skill);
                }
            }
        }

        private void SetAiSkill(bool ispetattack)
        {
            if (Sys_Fight.Instance.AutoFightData.AutoState)
            {
                CSVActiveSkillInfo.Data cSVActiveSkillInfoData = CSVActiveSkillInfo.Instance.GetConfData(211);
                GameObject go = GameObject.Instantiate<GameObject>(layout.skillGo, layout.skillGo.transform.parent);
                go.SetActive(true);

                UI_Skill skill = new UI_Skill(OnSelectSkill, cSVActiveSkillInfoData.id, isfirst, ispetattack);
                skill.Init(go.transform);
                skill.isSkill = false;
                skill.isAvailable = true;
                skill.Refresh(cSVActiveSkillInfoData.id, ispetattack, false);
                skills.Add(skill);
            }
        }

        private void SetSkillItem(List<uint>list)
        {
            for (int i=0;i< list.Count;++i)
            {
                if (list[i] > 100)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.skillGo, layout.skillGo.transform.parent);
                    UI_Skill skill = new UI_Skill(OnSelectSkill, list[i], isfirst,false);
                    skill.Init(go.transform);
                    skill.isSkill = true;
                    skill.isAvailable = true ;
                    skill.Refresh(list[i], false,true);
                    skills.Add(skill);
                }
            }
        }

        private void SetHoldingSkillItem(List<SkillComponent.SkillData> list)
        {
            skills.Clear();
            for (int i=0;i< list.Count;++i)
            {
                GameObject go = GameObject.Instantiate<GameObject>(layout.skillGo, layout.skillGo.transform.parent);
                UI_Skill skill = new UI_Skill(OnSelectSkill, list[i].CSVActiveSkillInfoData.id, isfirst, false);
                skill.Init(go.transform);
                skill.isSkill = true;
                skill.isAvailable = list[i].Available;
                skill.Refresh(list[i].CSVActiveSkillInfoData.id, false, false);
                skills.Add(skill);
            }
            layout.skillGo.SetActive(false);
            if (skills.Count == 0&& !Sys_Fight.Instance.AutoFightData.AutoState)
            {
                layout.scrollskillGo.SetActive(false);
                layout.noskillView.SetActive(true);
                layout.noroleskillGo.SetActive(true);
                layout.nopetskillGo.SetActive(false);
            }
        }

        private void DefaultSkill()
        {
            layout.skillGo.SetActive(true);
            layout.nootherskillGo.SetActive(false);
            for (int i=0;i< skills.Count;++i) { skills[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.skillGo.transform.parent.gameObject, layout.skillGo.transform.name);
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_MainBattle_Skills);
            Net_Combat.Instance.eventEmitter.Trigger<bool>(Net_Combat.EEvents.OnOpenAutoSkill, false);
        }
    }
}
