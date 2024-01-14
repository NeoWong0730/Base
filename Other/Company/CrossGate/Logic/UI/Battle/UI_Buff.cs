using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Packet;

namespace Logic
{
    public class UI_Buff_Layout
    {
        public Text name;
        public Text profession;
        public Text profix;
        public Text bloodnum;
        public Text magicnum;
        public Text race;

        public Slider blood;
        public Slider magic;
        public GameObject buffGo;
        public GameObject ItemRoot;
        public GameObject eleRoot;
        public RectTransform ScrollRt;
        private Transform transform;
        public RectTransform bloodRT;
        public Button closeBtn;
        public GameObject passRoot;

        public GameObject reputationGo;
        public Text reputationTitle;
        public Text reputationLevel;
        public GameObject shieldGo;
        public Text shieldNum;
        public Slider shieldSlider;

        public void Init(Transform transform)
        {
            this.transform = transform;
            name = transform.Find("Animator/Content/Player_Root/Text_Name").GetComponent<Text>();
            profession = transform.Find("Animator/Content/Player_Root/Text_Profession").GetComponent<Text>();
            profix = transform.Find("Animator/Content/Player_Root/ Text_qianzhui").GetComponent<Text>();
            bloodnum = transform.Find("Animator/Content/Player_Root/Text_Blood/Text_Percent").GetComponent<Text>();
            magicnum = transform.Find("Animator/Content/Player_Root/Text_Magic/Text_Percent").GetComponent<Text>();
            blood = transform.Find("Animator/Content/Player_Root/Text_Blood/Slider_Blood").GetComponent<Slider>();
            magic = transform.Find("Animator/Content/Player_Root/Text_Magic/Slider_Magic").GetComponent<Slider>();

            buffGo = transform.Find("Animator/Content/Scroll_View/Viewport/Item").gameObject;
            ItemRoot = transform.Find("Animator/Content/Scroll_View/Viewport").gameObject;
            eleRoot = transform.Find("Animator/Content/Player_Root/Crystal/Image_Attr").gameObject;
            bloodRT = transform.Find("Animator/Content/Player_Root/Text_Blood").GetComponent<RectTransform>(); ;
            ScrollRt = transform.Find("Animator/Content/Scroll_View").GetComponent<RectTransform>();
            closeBtn = transform.Find("Animator/Content/Player_Root/Image_Close/Btn_Close").GetComponent<Button>();
            passRoot = transform.Find("PassRoot").gameObject;

            reputationGo= transform.Find("Animator/Content/Player_Root/Text_Reputation").gameObject;
            reputationLevel = transform.Find("Animator/Content/Player_Root/Text_Reputation/Text_Lv").GetComponent<Text>();
            reputationTitle = transform.Find("Animator/Content/Player_Root/Text_Reputation/Text_Name").GetComponent<Text>();
            shieldGo = transform.Find("Animator/Content/Player_Root/Text_Blood/Shield").gameObject;
            shieldNum = transform.Find("Animator/Content/Player_Root/Text_Blood/Shield/Text/Num").GetComponent<Text>();
            shieldSlider = transform.Find("Animator/Content/Player_Root/Text_Blood/Shield/Slider_Shield").GetComponent<Slider>();
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

    public class UI_Buff_Item : UIComponent
    {
        private Text name;
        private Text tips;
        private Text describe;
        private Text roundNum;
        private Image icon;
        private GameObject unknowGo;
        private GameObject roundGo;
        private GameObject contentGo;

        private CSVBuff.Data csvBuffData;
        private MobBuffComponent.BuffData mobBuffData;
        private BattleUnit battleUnit;

        public UI_Buff_Item(CSVBuff.Data _buffdata, MobBuffComponent.BuffData _mobBuffData, BattleUnit _battleUnit) : base()
        {
            csvBuffData = _buffdata;
            mobBuffData = _mobBuffData;
            battleUnit = _battleUnit;
        }

        protected override void Loaded()
        {
            name = transform.Find("Image_BG/Text_Message/Text_Buff_Name").GetComponent<Text>();
            tips = transform.Find("Image_BG/Text_Message/Text_Tip").GetComponent<Text>();
            describe = transform.Find("Image_BG/Text_Message/").GetComponent<Text>();
            roundNum = transform.Find("Image_BG/Text_Message/Image_Num/Text").GetComponent<Text>();
            icon = transform.Find("Image_BG/Text_Message/Image_Buff_Icon").GetComponent<Image>();
            unknowGo = transform.Find("Image_BG/Text_Message/Image_Buff_Unknow").gameObject;
            roundGo = transform.Find("Image_BG/Text_Message/Image_Num").gameObject;
            contentGo = transform.Find("Image_BG").gameObject;
        }

        public  void RefreshItem()
        {
            name.text = LanguageHelper.GetTextContent(csvBuffData.name);
            if(csvBuffData.id== 317230010&& battleUnit!=null) //特殊处理 宇宙之气
            {
                describe.text = LanguageHelper.GetTextContent(csvBuffData.desc, battleUnit.CurGas.ToString(), battleUnit.MaxGas.ToString());
            }
            else
            {
                describe.text = LanguageHelper.GetTextContent(csvBuffData.desc);
            }
            ImageHelper.SetIcon(icon, csvBuffData.icon);
            roundGo.SetActive(mobBuffData.m_Overlay > 1);
            roundNum.text = mobBuffData.m_Overlay.ToString();
            if (csvBuffData.show_buff_duration)
            {
                if (csvBuffData.duration_type == 1)
                { 
                    tips.text = LanguageHelper.GetTextContent(490000001, mobBuffData.m_Count.ToString());
                }
                else
                {
                    tips.text = LanguageHelper.GetTextContent(490000002, mobBuffData.m_Count.ToString());
                }
            }
            else
            {
                tips.text = string.Empty;
            }
            unknowGo.SetActive(false);
            ContentSizeFitter[] fitter = describe.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
            contentGo.GetComponent<ContentSizeFitter>().enabled = describe.rectTransform.rect.height > 60;  
        }
    }

    public class UI_Buff : UIBase, UI_Buff_Layout.IListener
    {
        private UI_Buff_Layout layout = new UI_Buff_Layout();
        private List<UI_Buff_Item> buffList = new List<UI_Buff_Item>();
        private GameObject battleGo;

        protected override void OnOpen(object arg)
        {
            battleGo = (GameObject)arg;
        }
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            AddClickPass();
        }

        private void AddClickPass()
        {
            for (int i = 0; i < layout.passRoot.transform.childCount; ++i)
            {
                layout.passRoot.transform.GetChild(i).GetComponent<Button>().onClick.AddListener(OnClose_ButtonClicked);
                layout.passRoot.transform.GetChild(i).gameObject.AddComponent<ClickPass>();
            }
        }

        protected override void OnShow()
        {
            MobEntity mobEntity = MobManager.Instance.GetMobByGameObject(battleGo);
            if (mobEntity != null)
            {
                MobCombatComponent mobCombatComponent = mobEntity.m_MobCombatComponent;
                bool hasRevertBuff = false;
                MobBuffComponent mobBuffComponent = mobEntity.GetComponent<MobBuffComponent>();
                if (mobBuffComponent != null)
                {
                    List<MobBuffComponent.BuffData> buffs = new List<MobBuffComponent.BuffData>();
                    for (int i = 0; i < mobBuffComponent.m_Buffs.Count; ++i)
                    {
                        if (mobBuffComponent.m_Buffs[i].m_BuffTb.is_show != 0)
                        {
                            buffs.Add(mobBuffComponent.m_Buffs[i]);
                        }
                        if (mobBuffComponent.m_Buffs[i].m_BuffTb.buff_effective == 99)
                        {
                            hasRevertBuff = true;
                        }
                    }
                    if (buffs.Count == 0)
                    {
                        layout.ScrollRt.gameObject.SetActive(false);
                    }
                    else
                    {
                        layout.ScrollRt.gameObject.SetActive(true);
                        AddBuffList(buffs, mobCombatComponent.m_BattleUnit);
                    }
                }
                else
                {
                    layout.ScrollRt.gameObject.SetActive(false);
                }
                SetRoleMessage(mobCombatComponent, hasRevertBuff);
                ForceRebuildLayout(layout.ScrollRt.transform.parent.gameObject);
            }
            else     //处理没有战斗单位的情况
            {
                UIManager.CloseUI(EUIID.UI_Buff);
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            if (toRegister)
            {
                Sys_Input.Instance.onTouchUp += OnTpuchUp;
            }
            else
            {
                Sys_Input.Instance.onTouchUp -= OnTpuchUp;
            }
        }

        private void OnTpuchUp(Vector2 obj)
        {
            UIManager.CloseUI(EUIID.UI_Buff);
        }

        protected override void OnClose()
        {
            battleGo = null;
        }

        protected override void OnDestroy()
        {
            battleGo = null;
        }

        protected override void OnHide()
        {
            battleGo = null;
            DefaultBuffList();
        }

        private void SetRoleMessage(MobCombatComponent mobCombatComponent, bool hasRevertBuff)
        {
            if (mobCombatComponent.m_BattleUnit == null)
            {
                return;
            }
            layout.profix.gameObject.SetActive(false);
            layout.reputationGo.SetActive(false);
            CSVBattleType.Data csvBattleTypeData = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            bool selfCamp = false;
            if (Net_Combat.Instance.m_IsWatchBattle)
            {
                BattleUnit battleUnit = MobManager.Instance.GetBeWatchBattleUnit();
                selfCamp = CombatHelp.IsSameCamp(mobCombatComponent.m_BattleUnit, battleUnit);
            }
            else if (Net_Combat.Instance.m_IsVideo)
            {
                selfCamp = true;
            }
            else
            {
                selfCamp = CombatHelp.IsSameCamp(mobCombatComponent.m_BattleUnit, GameCenter.mainFightHero.battleUnit);
            }
            if (selfCamp)
            {
                layout.eleRoot.SetActive(csvBattleTypeData.show_self_element);
            }
            else
            {
                layout.eleRoot.SetActive(csvBattleTypeData.show_enemy_element);
            }
            if (mobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Monster)
            {     
                CSVMonster.Instance.TryGetValue(mobCombatComponent.m_BattleUnit.UnitInfoId, out CSVMonster.Data csvMonsterData);               
                if (csvMonsterData != null)
                {
                    layout.name.text = LanguageHelper.GetTextContent(csvMonsterData.monster_name);
                    layout.profession.text = LanguageHelper.GetTextContent(CSVGenus.Instance.GetConfData(csvMonsterData.genus).rale_name);
                    FrameworkTool.CreateChildList(layout.eleRoot.transform, csvMonsterData.template_attr.Count);
                    for (int i = 0; i < csvMonsterData.template_attr.Count; ++i)
                    {
                        InitEleAttr(i, csvMonsterData.template_attr[i][0], csvMonsterData.template_attr[i][1], hasRevertBuff);
                    }
                    if (csvMonsterData.ex_name != 0)
                    {
                        layout.profix.gameObject.SetActive(true);
                        TextHelper.SetText(layout.profix, csvMonsterData.ex_name);
                    }
                }
            }
            else if(mobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Pet)
            {
                CSVPetNew.Data csvPetNewData = CSVPetNew.Instance.GetConfData(mobCombatComponent.m_BattleUnit.UnitInfoId);
                if(mobCombatComponent.m_BattleUnit.PetName.IsEmpty || mobCombatComponent.m_BattleUnit.PetName.Length == 0)
                {
                    layout.name.text = LanguageHelper.GetTextContent(csvPetNewData.name);
                }
                else
                {
                    layout.name.text = mobCombatComponent.m_BattleUnit.PetName.ToStringUtf8();
                }
                layout.profession.text = LanguageHelper.GetTextContent(CSVGenus.Instance.GetConfData(csvPetNewData.race).rale_name);

                FrameworkTool.CreateChildList(layout.eleRoot.transform, mobCombatComponent.m_BattleUnit.EleAttr.Count);
                for (int i = 0; i < mobCombatComponent.m_BattleUnit.EleAttr.Count; ++i)
                {
                    InitEleAttr(i, mobCombatComponent.m_BattleUnit.EleAttr[i].AttrId, mobCombatComponent.m_BattleUnit.EleAttr[i].Value, hasRevertBuff);
                }
            }
            else if (mobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Partner)
            {
                layout.name.text = LanguageHelper.GetTextContent(CSVPartner.Instance.GetConfData(mobCombatComponent.m_BattleUnit.UnitInfoId).name);
                layout.profession.text = LanguageHelper.GetTextContent(CSVPartner.Instance.GetConfData(mobCombatComponent.m_BattleUnit.UnitInfoId).occupation);
                FrameworkTool.CreateChildList(layout.eleRoot.transform, mobCombatComponent.m_BattleUnit.EleAttr.Count);
                for (int i = 0; i < mobCombatComponent.m_BattleUnit.EleAttr.Count; ++i)
                {
                    InitEleAttr(i, mobCombatComponent.m_BattleUnit.EleAttr[i].AttrId, mobCombatComponent.m_BattleUnit.EleAttr[i].Value, hasRevertBuff);
                }
            }
            else if (mobCombatComponent.m_BattleUnit.UnitType == (uint)UnitType.Hero || mobCombatComponent.m_BattleUnit.ServerUnitType == 8)
            {
                layout.reputationGo.SetActive(true);
                if (mobCombatComponent.m_BattleUnit.ServerUnitType == 8)
                {
                    CSVMonster.Instance.TryGetValue((uint)mobCombatComponent.m_BattleUnit.RoleId, out CSVMonster.Data csvMonsterData);
                    if (csvMonsterData != null)
                    {
                        layout.name.text = LanguageHelper.GetTextContent(csvMonsterData.monster_name);
                    }
                }
                else
                {
                    layout.name.text = mobCombatComponent.m_BattleUnit.RoleName.ToStringUtf8();
                }
                uint reputationLevel = mobCombatComponent.m_BattleUnit.ReputationLvl;
                uint danLevel = (reputationLevel + 100) / 100;
                uint specificLevel = reputationLevel % 100;
               if( CSVFameRank.Instance.TryGetValue(danLevel, out CSVFameRank.Data csvFameRankData))
                {
                    layout.reputationTitle.text = LanguageHelper.GetTextContent(csvFameRankData.name);
                    layout.reputationLevel.text = LanguageHelper.GetTextContent(10963, specificLevel.ToString());

                }
                else
                {
                    if (danLevel != 0)
                    {
                        csvFameRankData = CSVFameRank.Instance.GetConfData(10);
                        layout.reputationTitle.text = LanguageHelper.GetTextContent(csvFameRankData.name);
                        layout.reputationLevel.text = LanguageHelper.GetTextContent(10963, 100.ToString());
                    }
                    else
                    {
                        layout.reputationTitle.text = string.Empty;
                        layout.reputationLevel.text = string.Empty;
                    }
                }
                if (mobCombatComponent.m_BattleUnit.RoleId == Sys_Role.Instance.Role.RoleId)
                {
                    layout.name.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                    if (GameCenter.mainHero.careerComponent.CurCarrerType == ECareerType.None )
                    {
                        layout.profession.text = string.Empty;
                    }
                    else
                    {
                        layout.profession.text = LanguageHelper.GetTextContent(CSVCareer.Instance.GetConfData((uint)GameCenter.mainHero.careerComponent.CurCarrerType).name);
                    }
                }
                else if (Sys_Team.Instance.HaveTeam && selfCamp)
                {
                    TeamMem team = Sys_Team.Instance.getTeamMem(mobCombatComponent.m_BattleUnit.RoleId);
                    if (team != null && CSVCareer.Instance.TryGetValue(team.Career, out CSVCareer.Data data))
                    {
                        layout.profession.text = LanguageHelper.GetTextContent(data.name);
                    }
                    else
                    {
                        layout.profession.text = string.Empty;
                    }
                }
                else
                {
                    if (Sys_Fight.Instance.FightHeros.TryGetValue(mobCombatComponent.m_BattleUnit.RoleId, out FightHero hero))
                    {
                        if (hero.careerComponent.CurCarrerType == ECareerType.None || hero.careerComponent.CurCarrerType==0)
                            layout.profession.text = string.Empty;
                        else
                            layout.profession.text = LanguageHelper.GetTextContent(CSVCareer.Instance.GetConfData((uint)hero.careerComponent.CurCarrerType).name);
                    }
                    else
                    {
                        layout.profession.text = string.Empty;
                    }
                }
                FrameworkTool.CreateChildList(layout.eleRoot.transform, mobCombatComponent.m_BattleUnit.EleAttr.Count);
                for (int i = 0; i < mobCombatComponent.m_BattleUnit.EleAttr.Count; ++i)
                {
                    InitEleAttr(i, mobCombatComponent.m_BattleUnit.EleAttr[i].AttrId, mobCombatComponent.m_BattleUnit.EleAttr[i].Value, hasRevertBuff);
                }
            }

            if ((csvBattleTypeData.show_self_hp && selfCamp) || (csvBattleTypeData.show_enemy_hp && !selfCamp))
            {
                if (mobCombatComponent.m_BattleUnit.CurHp != 0)
                {
                    layout.blood.value = (float)mobCombatComponent.m_BattleUnit.CurHp / (float)mobCombatComponent.m_BattleUnit.MaxHp;
                    layout.bloodnum.text = LanguageHelper.GetTextContent(2010321, mobCombatComponent.m_BattleUnit.CurHp.ToString(), mobCombatComponent.m_BattleUnit.MaxHp.ToString());
                }
                else
                {
                    layout.blood.value = 0;
                    layout.bloodnum.text = LanguageHelper.GetTextContent(2010321, "0", ((float)mobCombatComponent.m_BattleUnit.MaxHp).ToString());
                }
                if (mobCombatComponent.m_BattleUnit.CurShield != 0)
                {
                    layout.shieldGo.SetActive(true);
                    layout.shieldSlider.value = (float)mobCombatComponent.m_BattleUnit.CurShield / (float)mobCombatComponent.m_BattleUnit.MaxShield;
                    layout.shieldNum.text = LanguageHelper.GetTextContent(2010321, mobCombatComponent.m_BattleUnit.CurShield.ToString(), mobCombatComponent.m_BattleUnit.MaxShield.ToString());
                }
                else
                {
                    layout.shieldGo.SetActive(false);
                }
            }
            else
            {
                layout.blood.value = 1;
                layout.bloodnum.text = LanguageHelper.GetTextContent(3000009);
                layout.shieldGo.SetActive(false);
            }

            if ((csvBattleTypeData.show_self_mp && selfCamp) || (csvBattleTypeData.show_enemy_mp && !selfCamp))
            {
                if (mobCombatComponent.m_BattleUnit.CurMp != 0)
                {
                    layout.magic.value = (float)mobCombatComponent.m_BattleUnit.CurMp / (float)mobCombatComponent.m_BattleUnit.MaxMp;
                    layout.magicnum.text = LanguageHelper.GetTextContent(2010321, mobCombatComponent.m_BattleUnit.CurMp.ToString(), mobCombatComponent.m_BattleUnit.MaxMp.ToString());
                }
                else
                {
                    layout.magic.value = 0;
                    layout.magicnum.text = LanguageHelper.GetTextContent(2010321, "0", ((float)mobCombatComponent.m_BattleUnit.MaxMp).ToString());
                }
            }
            else
            {
                layout.magic.value = 1;
                layout.magicnum.text = LanguageHelper.GetTextContent(3000009);
            }
        }

        private void InitEleAttr(int index, uint attrId,uint attrValue,bool hasRevert)
        {
            Transform trans = layout.eleRoot.transform.GetChild(index);
            if (attrValue > 0)
            {
                trans.gameObject.SetActive(true);
                if (hasRevert)
                {
                    if (attrId == 1)
                    {
                        ImageHelper.SetIcon(trans.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(3).attr_icon);
                    }
                    else if (attrId == 2)
                    {
                        ImageHelper.SetIcon(trans.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(4).attr_icon);
                    }
                    else if (attrId == 3)
                    {
                        ImageHelper.SetIcon(trans.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(1).attr_icon);
                    }
                    else if (attrId == 4)
                    {
                        ImageHelper.SetIcon(trans.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(2).attr_icon);
                    }
                }
                else
                {
                    ImageHelper.SetIcon(trans.Find("Image_Attr").GetComponent<Image>(), CSVAttr.Instance.GetConfData(attrId).attr_icon);
                }
                trans.Find("Image_Attr/Text").GetComponent<Text>().text = attrValue.ToString();
            }
            else
            {
                trans.gameObject.SetActive(false);
            }
        }

        private void AddBuffList(List<MobBuffComponent.BuffData> buffs, BattleUnit battleUnit)
        {
            buffList.Clear();
            buffs.Sort((x, y) => y.m_BuffTb.is_show.CompareTo(x.m_BuffTb.is_show));
            for(int i=0;i<buffs.Count;++i)   
            {
                if (buffs[i].m_BuffTb.is_show != 0)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.buffGo, layout.buffGo.transform.parent);
                    UI_Buff_Item buffitem = new UI_Buff_Item(buffs[i].m_BuffTb, buffs[i], battleUnit);
                    buffitem.Init(go.transform);
                    buffitem.RefreshItem();
                    buffList.Add(buffitem);
                }
            }
            layout.buffGo.SetActive(false);
            ForceRebuildLayout(layout.buffGo.transform.parent.gameObject);
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }

        private void DefaultBuffList()
        {
            layout.buffGo.SetActive(true);
            for (int i=0;i<buffList.Count;++i) { buffList[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.ItemRoot, layout.ItemRoot.transform.GetChild(0).transform.name);
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Buff);
        }
    }
}
