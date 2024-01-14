using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Framework;
using DG.Tweening;
using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 血条 蓝条显示
    /// </summary>
    public class BDShow : IHUDComponent
    {
        public class AttrData
        {
            public uint id;
            public uint val;
            public AttrData(uint _id, uint _val)
            {
                id = _id;
                val = _val;
            }
        }
        private uint battleId;

        public GameObject mRootGameObject;
        private RectTransform rectTransform
        {
            get
            {
                return mRootGameObject.transform as RectTransform;
            }
        }

        public GameObject mBattleOrderFlag;
        private RectTransform battleFlagRect
        {
            get
            {
                return mBattleOrderFlag.transform as RectTransform;
            }
        }

        public GameObject mBattleOrderName;

        private RectTransform battleOrderNameRect
        {
            get
            {
                return mBattleOrderName.transform as RectTransform;
            }
        }

        public GameObject mBattleSelect;

        private RectTransform battleSelectRect
        {
            get
            {
                return mBattleSelect.transform as RectTransform;
            }
        }

        private Image mImageBG_boss_NoMp;
        private Image mImageBG_commom_NoMp;
        private GameObject mImageBG_commom_Mp;
        private GameObject mImageBG_boss_Mp;
        private Image m_Hp_Excessive;
        private Image m_Hp_Actual;
        private Image m_Mp_Actual;
        private GameObject m_BloodRoot;
        private GameObject m_AttrRoot;
        private GameObject m_arrowRoot;
        private GameObject m_MonsterEliteHead;
        private GameObject m_MonsterBossHead;
        private Image m_Attr1;
        private Image m_Attr2;
        private Image m_Attr3;
        private Image m_Attr4;
        private Text m_Level;
        private Text m_Name;
        private GameObject m_TimeSandRoot;
        private GameObject m_SelectedFx;
        private GameObject m_SparSkillRoot;

        private Image imgRace;//名字左边的种族图标
        private Image imgCareer;//名字下面的职业图标
        private Image imgShieldBar;//护盾条
        private Image imgEnergyBar;//能量条(气)
        private GameObject goEnergyBg;//能量条背景

        private HUDPositionCorrect positionCorrect_root;
        private HUDPositionCorrect positionCorrect_battleOrderFlag;
        private HUDPositionCorrect positionCorrect_bttleOrderName;
        private HUDPositionCorrect positionCorrect_battleSelect;

        private float m_CurHp;
        private float CurHp
        {
            get
            {
                return m_CurHp;
            }
            set
            {
                if (m_CurHp != value)
                {
                    m_CurHp = value;
                    m_Hp_Excessive.fillAmount = value;
                }
            }
        }

        private float m_TargetHp;
        private float TargetHp
        {
            get
            {
                return m_TargetHp;
            }
            set
            {
                if (m_TargetHp != value)
                {
                    m_TargetHp = value;
                    m_Hp_Actual.fillAmount = value;
                    HP_Dirty = true;
                }
            }
        }

        private float m_CurMp;
        private float CurMp
        {
            get
            {
                return m_CurMp;
            }
            set
            {
                m_CurMp = value;
                m_Mp_Actual.fillAmount = value;
            }
        }
        private float m_CurShieldValue =  -1;//当前护盾值
        private float CurShieldValue
        {
            get
            {
                return m_CurShieldValue;
            }
            set
            {
                if (m_CurShieldValue != value)
                {

                    m_CurShieldValue = value;
                    imgShieldBar.fillAmount = value;
                }
            }
        }

        private float m_CurEnergyValue = -1;//当前能量值
        private float CurEnergyValue
        {
            get
            {
                return m_CurEnergyValue;
            }
            set
            {
                if (m_CurEnergyValue != value)
                {
                    m_CurEnergyValue = value;
                    imgEnergyBar.fillAmount = value;
                }
            }
        }
        //private float m_targetMp;
        //private float TargetMp
        //{
        //    get
        //    {
        //        return m_targetMp;
        //    }
        //    set
        //    {
        //        if (m_targetMp != value)
        //        {
        //            m_targetMp = value;
        //            m_Mp_Actual.fillAmount = value;
        //            //MP_Dirty = true;
        //        }
        //    }
        //}

        private bool HP_Dirty = false;
        //private bool MP_Dirty = false;

        private Vector2 bdTimeSandOffest;
        private Vector2 arrowOffest;
        private Vector2 nameOffest;
        private int bgType = 0; //0普通 1boss

        private Transform target;
        private Transform bodyPoint;
        private Transform feetPoint;
        private Vector3 bodyPointOffest;
        private Vector3 feetPointOffest;

        private Timer timer;

        public void Construct(uint _battleId, GameObject gameObject, GameObject battleOrderFlag, GameObject battleOrderName, GameObject combatSelect)
        {
            battleId = _battleId;
            mRootGameObject = gameObject;
            mBattleOrderFlag = battleOrderFlag;
            mBattleOrderName = battleOrderName;
            mBattleSelect = combatSelect;
            ParseCp();
            string[] bdtimesand = CSVParam.Instance.GetConfData(631).str_value.Split('|');
            bdTimeSandOffest = new Vector2(int.Parse(bdtimesand[0]), int.Parse(bdtimesand[1]));
            string[] arrow = CSVParam.Instance.GetConfData(630).str_value.Split('|');
            arrowOffest = new Vector2(int.Parse(arrow[0]), int.Parse(arrow[1]));
            string[] name = CSVParam.Instance.GetConfData(634).str_value.Split('|');
            nameOffest = new Vector2(int.Parse(name[0]), int.Parse(name[1]));
            string[] bodyPointOffests = CSVParam.Instance.GetConfData(645).str_value.Split('|');
            bodyPointOffest = new Vector3(int.Parse(bodyPointOffests[0]) / 1000f, int.Parse(bodyPointOffests[1]) / 1000f, int.Parse(bodyPointOffests[2]) / 1000f);
            string[] feetPointOffests = CSVParam.Instance.GetConfData(644).str_value.Split('|');
            feetPointOffest = new Vector3(int.Parse(feetPointOffests[0]) / 1000f, int.Parse(feetPointOffests[1]) / 1000f, int.Parse(feetPointOffests[2]) / 1000f);

            positionCorrect_root = mRootGameObject.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect_root.CalOffest(new Vector3(0, 2, 0));
            positionCorrect_root.NeedCorrectAtSkillPlay = true;

            positionCorrect_battleOrderFlag = mBattleOrderFlag.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect_battleOrderFlag.CalOffest(bodyPointOffest);

            positionCorrect_bttleOrderName = mBattleOrderName.GetNeedComponent<HUDPositionCorrect>();
            positionCorrect_bttleOrderName.CalOffest(feetPointOffest);

            positionCorrect_battleSelect = mBattleSelect.GetNeedComponent<HUDPositionCorrect>();
        }


        private void ParseCp()
        {
            m_BloodRoot = mRootGameObject.transform.Find("Blood").gameObject;
            mImageBG_boss_NoMp = mRootGameObject.transform.Find("Blood/Image_BG").GetComponent<Image>();
            mImageBG_commom_NoMp = mRootGameObject.transform.Find("Blood/Image_Common").GetComponent<Image>();
            mImageBG_commom_Mp = mRootGameObject.transform.Find("Blood/Image_Common").gameObject;
            mImageBG_boss_Mp = mRootGameObject.transform.Find("Blood/Image_BG").gameObject;
            m_Hp_Excessive = mRootGameObject.transform.Find("Blood/Blood_Excessive").GetComponent<Image>();
            m_Hp_Actual = mRootGameObject.transform.Find("Blood/Blood_Actual").GetComponent<Image>();
            m_Mp_Actual = mRootGameObject.transform.Find("Blood/Magic_Actual").GetComponent<Image>();
            m_MonsterBossHead = mRootGameObject.transform.Find("Blood/Image_Boss").gameObject;
            m_MonsterEliteHead = mRootGameObject.transform.Find("Blood/Image_Elite").gameObject;
            m_arrowRoot = mRootGameObject.transform.Find("ArrowRoot").gameObject;
            m_AttrRoot = mRootGameObject.transform.Find("attrRoot").gameObject;
            m_Attr1 = m_AttrRoot.transform.Find("attr1").GetComponent<Image>();
            m_Attr2 = m_AttrRoot.transform.Find("attr2").GetComponent<Image>();
            m_Attr3 = m_AttrRoot.transform.Find("attr3").GetComponent<Image>();
            m_Attr4 = m_AttrRoot.transform.Find("attr4").GetComponent<Image>();
            m_Level = m_AttrRoot.transform.Find("surf/Text").GetComponent<Text>();
            m_Name = mRootGameObject.transform.Find("Text_Name").GetComponent<Text>();
            m_TimeSandRoot = mRootGameObject.transform.Find("TimeSandRoot").gameObject;
            //m_SelectRoot = mRootGameObject.transform.Find("SelectRoot").gameObject;
            m_SelectedFx = mBattleSelect.transform.Find("Image/Fx_ui_dianji01").gameObject;
            m_SparSkillRoot = mRootGameObject.transform.Find("SparSkillRoot").gameObject;

            imgRace = mRootGameObject.transform.Find("Text_Name/Image").GetComponent<Image>();
            imgCareer = mRootGameObject.transform.Find("Text_Name/Image_Profession").GetComponent<Image>();
            imgShieldBar = mRootGameObject.transform.Find("Blood/Blood_Actual1").GetComponent<Image>();
            imgEnergyBar = mRootGameObject.transform.Find("Blood/Ep_Actual").GetComponent<Image>();
            goEnergyBg = mRootGameObject.transform.Find("Blood/Image_Common_Ep").gameObject;
        }

        public void SetTarget(Transform transform)
        {
            target = transform;
            positionCorrect_root.SetTarget(transform);

            bodyPoint = MobManager.Instance.GetPosByMobBind(battleId, 2);
            feetPoint = MobManager.Instance.GetPosByMobBind(battleId, 3);

            positionCorrect_battleOrderFlag.SetTarget(feetPoint);
            positionCorrect_battleSelect.SetTarget(bodyPoint);
            positionCorrect_bttleOrderName.SetTarget(feetPoint);

            CalChildOffest();
        }

        public void Init(AttributeData attributeData)
        {
            CurMp = 1;
            CurHp = 1;
            TargetHp = 1;
            m_CurShieldValue = -1;
            CurShieldValue = 0;
            m_CurEnergyValue = -1;
            CurEnergyValue = 0;
            //TargetMp = 1;
            ClearBattleFlag();
            m_BloodRoot.SetActive(true);

            rectTransform.localScale = Vector3.one;
            battleFlagRect.localScale = Vector3.one;
            battleOrderNameRect.localScale = Vector3.one;

            for (int i = 0; i < m_BloodRoot.transform.childCount; i++)
            {
                m_BloodRoot.transform.GetChild(i).gameObject.SetActive(true);
            }
            mImageBG_boss_NoMp.gameObject.SetActive(false);
            mImageBG_commom_NoMp.gameObject.SetActive(false);
            m_Name.gameObject.SetActive(true);
            imgRace.gameObject.SetActive(false);
            goEnergyBg.SetActive(attributeData.RoleCareer == (uint)ECareerType.Fighter);
#if UNITY_EDITOR
            mRootGameObject.name = attributeData.fightUnitType.ToString();
#endif
            InitAttr(attributeData);
        }

        public void SetArrow(bool active)
        {
            m_arrowRoot.SetActive(active);
        }

        public void SetSparSkill(bool active)
        {
            m_SparSkillRoot.SetActive(active);
            //m_BloodRoot.SetActive(!active);
            //m_AttrRoot.SetActive(!active);
            int point = (int)Net_Combat.Instance.m_EnergePoint;
            int lastpoint = (int)Net_Combat.Instance.m_LastRounfEnergePoint;
            Transform SparSkillGroup = m_SparSkillRoot.transform.Find("EnergyGroup");
            List<GameObject> list = new List<GameObject>();
            int num = 0;
            for (int i = 0; i < 10; ++i)
            {
                list.Add(SparSkillGroup.GetChild(i).Find("EnergyLight").gameObject);
                if (lastpoint == 0)
                {
                    num = point;
                }
                else
                {
                    num = lastpoint;
                }
                if (i < num)
                {
                    SparSkillGroup.GetChild(i).Find("EnergyLight").gameObject.SetActive(true);
                }
                else
                {
                    SparSkillGroup.GetChild(i).Find("EnergyLight").gameObject.SetActive(false);
                }
            }
            if (active)
            {
                float normalTime;
                float perTime;
                float.TryParse(CSVParam.Instance.GetConfData(778).str_value, out normalTime);
                float.TryParse(CSVParam.Instance.GetConfData(779).str_value, out perTime);
                int index = 0;
                float lightTotleTime;
                if (lastpoint == 0 || lastpoint == point)
                {
                    lightTotleTime = normalTime / 1000;
                }
                else if (lastpoint > point)
                {
                    lightTotleTime = perTime / 1000 * (lastpoint - point + 1);
                }
                else
                {
                    lightTotleTime = perTime / 1000 * (point - lastpoint + 1);
                }
                timer = Timer.Register(lightTotleTime, () =>
                   {
                       timer.Cancel();
                       m_SparSkillRoot.SetActive(false);
                       m_BloodRoot.SetActive(true);
                       m_AttrRoot.SetActive(true);
                       foreach (var item in list)
                       {
                           item.SetActive(false);
                       }
                   }, (time) =>
                   {
                       if (lastpoint == 0 || lastpoint == point)
                       {
                       }
                       else
                       {
                           float t = time - index * (perTime / 1000);
                           if (t >= perTime / 1000)
                           {
                               if (lastpoint > point)
                               {
                                   if (lastpoint - index >= 1)
                                   {
                                       list[lastpoint - index - 1].SetActive(false);
                                   }
                               }
                               else
                               {
                                   if (lastpoint + index < 10)
                                   {
                                       list[lastpoint + index].SetActive(true);
                                   }
                               }
                               index++;
                           }
                       }
                   }, false, false);
            }
            else
            {
                foreach (var item in list)
                {
                    item.SetActive(false);
                }
            }
        }

        private void InitAttr(AttributeData attributeData)
        {
            if (attributeData.fightUnitType == (uint)EFightActorType.Monster)
            {
                CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(attributeData.notPlayerAttr);
                if (cSVMonsterData != null)
                {
                    //正常走怪物逻辑
                    if (cSVMonsterData.name_colour == 0)
                    {
                        Sys_HUD.Instance.SetFightNameText(EFightActorType.Monster, m_Name, cSVMonsterData.monster_name);
                        m_Level.text = attributeData.UnitLevel.ToString();
                        m_AttrRoot.SetActive(true);
                        if (cSVMonsterData.monster_rank == 1)
                        {
                            Monster_Normal();
                        }
                        if (cSVMonsterData.monster_rank == 2)
                        {
                            Monster_Elite();
                        }
                        if (cSVMonsterData.monster_rank == 3)
                        {
                            Monster_Boss();
                        }
                    }
                    //机器人怪物
                    else
                    {
                        HideMosterHead();
                        m_AttrRoot.SetActive(true);
                        Sys_HUD.Instance.SetFightNameText(EFightActorType.Pet, m_Name, cSVMonsterData.monster_name);
                        m_Level.text = attributeData.UnitLevel.ToString();
                    }
                    List<AttrData> _attrs = new List<AttrData>();
                    for (int i = 0; i < cSVMonsterData.template_attr.Count; i++)
                    {
                        List<uint> item = cSVMonsterData.template_attr[i];
                        if (item[0] == 1 || item[0] == 2 || item[0] == 3 || item[0] == 4)
                        {
                            AttrData attrData = new AttrData(item[0], item[1]);
                            _attrs.Add(attrData);
                        }
                    }
                    SetAttr(_attrs);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Debug.LogErrorFormat("不显示monsterName,cSVMonsterData =null .... id: {0}", attributeData.notPlayerAttr);
                    }
                }
                return;
            }
            HideMosterHead(); //隐藏boss 精英 头像
            m_AttrRoot.SetActive(true);
            List<AttrData> attrs = new List<AttrData>();
            if (attributeData.fightUnitType == (uint)EFightActorType.Hero)
            {
                if (attributeData.notPlayerAttr != 0)//怪物模拟真人
                {
                    CSVMonster.Data cSVMonsterData = CSVMonster.Instance.GetConfData(attributeData.notPlayerAttr);
                    if (cSVMonsterData != null)
                    {
                        Sys_HUD.Instance.SetFightNameText(EFightActorType.Hero, m_Name, cSVMonsterData.monster_name);
                    }
                }
                else
                {
                    Sys_HUD.Instance.SetFightNameText(EFightActorType.Hero, m_Name, attributeData.playerName);
                }
                Dictionary<uint, uint>.Enumerator enumerator = attributeData.playerAttr.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    AttrData attrData = new AttrData(enumerator.Current.Key, enumerator.Current.Value);
                    attrs.Add(attrData);
                }
                SetAttr(attrs);
            }
            else
            {
                List<List<uint>> vs = new List<List<uint>>();
                if (attributeData.fightUnitType == (uint)EFightActorType.Pet)
                {
                    CSVPetNew.Data cSVPetData = CSVPetNew.Instance.GetConfData(attributeData.notPlayerAttr);
                    if (cSVPetData != null)
                    {
                        if (attributeData.petName == string.Empty)
                        {
                            Sys_HUD.Instance.SetFightNameText(EFightActorType.Pet, m_Name, cSVPetData.name);
                        }
                        else
                        {
                            Sys_HUD.Instance.SetFightNameText(EFightActorType.Pet, m_Name, attributeData.petName);
                        }
                        vs = CSVPetNew.Instance.GetConfData(attributeData.notPlayerAttr).init_attr;
                    }
                }
                else if (attributeData.fightUnitType == (uint)EFightActorType.Partner)
                {
                    CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(attributeData.notPlayerAttr);
                    if (cSVPartnerData != null)
                    {
                        Sys_HUD.Instance.SetFightNameText(EFightActorType.Partner, m_Name, cSVPartnerData.name);
                        vs = CSVPartnerLevel.Instance.GetUniqData(cSVPartnerData.id, (uint)attributeData.UnitLevel).attribute;
                    }
                }
                if (vs != null)
                {
                    foreach (var item in vs)
                    {
                        if (item[0] == 1 || item[0] == 2 || item[0] == 3 || item[0] == 4)
                        {
                            AttrData attrData = new AttrData(item[0], item[1]);
                            attrs.Add(attrData);
                        }
                    }
                    SetAttr(attrs);
                }
            }
            m_Level.text = attributeData.UnitLevel.ToString();
            SetRoleShapeShiftIcon(attributeData.ShapeShiftId);
            SetRoleCareerIcon(attributeData.RoleCareer);
        }


        private void SetAttr(List<AttrData> attrs)
        {
            if (attrs.Count == 0)
            {
                m_Attr1.gameObject.SetActive(false);
                m_Attr2.gameObject.SetActive(false);
                m_Attr3.gameObject.SetActive(false);
                m_Attr4.gameObject.SetActive(false);
            }
            else if (attrs.Count == 1)
            {
                m_Attr1.gameObject.SetActive(true);
                m_Attr2.gameObject.SetActive(false);
                m_Attr3.gameObject.SetActive(false);
                m_Attr4.gameObject.SetActive(false);
                m_Attr1.fillAmount = 1;
                uint parmId = attrs[0].id + 401;
                string str = CSVParam.Instance.GetConfData(parmId).str_value;
                string[] colors = str.Split('|');
                m_Attr1.color = new Color(float.Parse(colors[0]) / 255f, float.Parse(colors[1]) / 255f, float.Parse(colors[2]) / 255f);
            }
            else if (attrs.Count == 2)
            {
                m_Attr1.gameObject.SetActive(true);
                m_Attr2.gameObject.SetActive(true);
                m_Attr3.gameObject.SetActive(false);
                m_Attr4.gameObject.SetActive(false);
                attrs.Sort((attr1, attr2) => { return attr1.val.CompareTo(attr2.val); });
                uint total = attrs[0].val + attrs[1].val;
                m_Attr1.fillAmount = 1;
                m_Attr2.fillAmount = (float)attrs[0].val / (float)total;

                for (int i = 0; i < attrs.Count; i++)
                {
                    uint attr1_paremId = attrs[i].id + 401;
                    string str_attr1_paremId = CSVParam.Instance.GetConfData(attr1_paremId).str_value;
                    string[] colors_attr1 = str_attr1_paremId.Split('|');

                    float r = float.Parse(colors_attr1[0]) / 255f;
                    float g = float.Parse(colors_attr1[1]) / 255f;
                    float b = float.Parse(colors_attr1[2]) / 255f;
                    Color temp = new Color(r, g, b);
                    if (i == 0)
                    {
                        m_Attr2.color = temp;
                    }
                    else
                    {
                        m_Attr1.color = temp;
                    }
                }
            }
            else if (attrs.Count == 3)
            {
                m_Attr1.gameObject.SetActive(true);
                m_Attr2.gameObject.SetActive(true);
                m_Attr3.gameObject.SetActive(true);
                m_Attr4.gameObject.SetActive(false);
                attrs.Sort((attr1, attr2) => { return attr1.val.CompareTo(attr2.val); });
                uint total = attrs[0].val + attrs[1].val + attrs[2].val;
                m_Attr1.fillAmount = 1;
                m_Attr3.fillAmount = (float)attrs[0].val / (float)total;
                m_Attr2.fillAmount = ((float)attrs[0].val + (float)attrs[1].val) / (float)total;
                for (int i = 0; i < attrs.Count; i++)
                {
                    uint attr1_paremId = attrs[i].id + 401;
                    string str_attr1_paremId = CSVParam.Instance.GetConfData(attr1_paremId).str_value;
                    string[] colors_attr1 = str_attr1_paremId.Split('|');

                    float r = float.Parse(colors_attr1[0]) / 255f;
                    float g = float.Parse(colors_attr1[1]) / 255f;
                    float b = float.Parse(colors_attr1[2]) / 255f;
                    Color temp = new Color(r, g, b);
                    if (i == 0)
                    {
                        m_Attr3.color = temp;
                    }
                    else if (i == 1)
                    {
                        m_Attr2.color = temp;
                    }
                    else
                    {
                        m_Attr1.color = temp;
                    }
                }
            }
            else if (attrs.Count == 4)
            {
                m_Attr1.gameObject.SetActive(true);
                m_Attr2.gameObject.SetActive(true);
                m_Attr3.gameObject.SetActive(true);
                m_Attr4.gameObject.SetActive(true);
                attrs.Sort((attr1, attr2) => { return attr1.val.CompareTo(attr2.val); });
                uint total = attrs[0].val + attrs[1].val + attrs[2].val + attrs[3].val;
                m_Attr1.fillAmount = 1;
                m_Attr4.fillAmount = (float)attrs[0].val / (float)total;
                m_Attr3.fillAmount = ((float)attrs[0].val + (float)attrs[1].val) / (float)total;
                m_Attr2.fillAmount = ((float)attrs[0].val + (float)attrs[1].val + (float)attrs[2].val) / (float)total;
                for (int i = 0; i < attrs.Count; i++)
                {
                    uint attr1_paremId = attrs[i].id + 401;
                    string str_attr1_paremId = CSVParam.Instance.GetConfData(attr1_paremId).str_value;
                    string[] colors_attr1 = str_attr1_paremId.Split('|');

                    float r = float.Parse(colors_attr1[0]) / 255f;
                    float g = float.Parse(colors_attr1[1]) / 255f;
                    float b = float.Parse(colors_attr1[2]) / 255f;
                    Color temp = new Color(r, g, b);
                    if (i == 0)
                    {
                        m_Attr4.color = temp;
                    }
                    else if (i == 1)
                    {
                        m_Attr3.color = temp;
                    }
                    else if (i == 2)
                    {
                        m_Attr2.color = temp;
                    }
                    else
                    {
                        m_Attr1.color = temp;
                    }
                }
            }
        }



        public void OnTriggerBattleInstructionFlag(bool show, string content)
        {
            mBattleOrderFlag.SetActive(show);
            mBattleOrderName.SetActive(show);
            if (show)
            {
                battleFlagRect.localScale = Vector3.one;
                battleOrderNameRect.localScale = Vector3.one;
                mBattleOrderName.transform.Find("Text").GetComponent<Text>().text = content;
            }
            else
            {
                battleFlagRect.localScale = Vector3.zero;
                battleOrderNameRect.localScale = Vector3.zero;
            }
        }

        public void ClearBattleFlag()
        {
            mBattleOrderFlag.SetActive(false);
            mBattleOrderName.SetActive(false);
        }

        public void Update()
        {
            if (HP_Dirty)
            {
                if (CurHp > TargetHp)
                {
                    CurHp = Mathf.Lerp(CurHp, TargetHp, Sys_HUD.Instance.GetDeltaTime() / Mathf.Abs(CurHp - TargetHp));
                }
                else
                {
                    CurHp = TargetHp;
                }
                if (Mathf.Abs(CurHp - TargetHp) <= 0.01f)
                {
                    CurHp = TargetHp;
                    HP_Dirty = false;
                }
            }
            //if (MP_Dirty)
            //{
            //    if (CurMp > TargetMp)
            //    {
            //        CurMp = Mathf.Lerp(CurMp, TargetMp, Time.deltaTime / Mathf.Abs(CurMp - TargetMp));
            //    }
            //    else
            //    {
            //        CurMp = TargetMp;
            //    }
            //    if (Mathf.Abs(CurMp - TargetMp) <= 0.01f)
            //    {
            //        CurMp = TargetMp;
            //        MP_Dirty = false;
            //    }
            //}
        }


        private void CalChildOffest()
        {
            (m_BloodRoot.transform as RectTransform).anchoredPosition += bdTimeSandOffest;
            (m_AttrRoot.transform as RectTransform).anchoredPosition += bdTimeSandOffest;
            (m_TimeSandRoot.transform as RectTransform).anchoredPosition += bdTimeSandOffest;
            (m_arrowRoot.transform as RectTransform).anchoredPosition += arrowOffest;
            m_Name.rectTransform.anchoredPosition += nameOffest;
        }

        private void ResetChildOffest()
        {
            if (m_BloodRoot)
            {
                (m_BloodRoot.transform as RectTransform).anchoredPosition -= bdTimeSandOffest;
            }
            if (m_AttrRoot)
            {
                (m_AttrRoot.transform as RectTransform).anchoredPosition -= bdTimeSandOffest;
            }
            if (m_TimeSandRoot)
            {
                (m_TimeSandRoot.transform as RectTransform).anchoredPosition -= bdTimeSandOffest;
            }
            if (m_arrowRoot)
            {
                (m_arrowRoot.transform as RectTransform).anchoredPosition -= arrowOffest;
            }
            if (m_Name)
            {
                m_Name.rectTransform.anchoredPosition -= nameOffest;
            }
        }

        public void UpdateBlood(float target)
        {
            TargetHp = target;
        }

        public void UpdateMp(float target)
        {
            CurMp = target;
        }

        public void UpdateTimeSand(bool isActive)
        {
            m_TimeSandRoot.SetActive(isActive);
        }

        public void ShowOrHideSelect(bool isShow)
        {
            m_SelectedFx.SetActive(false);
            mBattleSelect.SetActive(isShow);
        }

        public void ShowSelected()
        {
            m_SelectedFx.SetActive(true);
        }

        public void ShowOrHide(bool flag)
        {
            if (!flag)
            {
                rectTransform.localScale = Vector3.zero;
                battleFlagRect.localScale = Vector3.zero;
                battleOrderNameRect.localScale = Vector3.zero;
                //rectTransform.gameObject.SetActive(false);
                //battleFlagRect.gameObject.SetActive(false);
                //battleOrderNameRect.gameObject.SetActive(false);
            }
            else
            {
                rectTransform.localScale = Vector3.one;
                battleFlagRect.localScale = Vector3.one;
                battleOrderNameRect.localScale = Vector3.one;
                //rectTransform.gameObject.SetActive(true);
                //battleFlagRect.gameObject.SetActive(true);
                //battleOrderNameRect.gameObject.SetActive(true);
            }
        }

        public void HideHpAttr()
        {
            m_BloodRoot.SetActive(false);
            m_AttrRoot.SetActive(false);
        }

        public void HideMp()
        {
            m_Mp_Actual.gameObject.SetActive(false);
            if (bgType == 0) //普通
            {
                mImageBG_commom_NoMp.gameObject.SetActive(true);
                mImageBG_commom_Mp.SetActive(false);
            }
            else
            {
                mImageBG_boss_NoMp.gameObject.SetActive(true);
                mImageBG_boss_Mp.SetActive(false);
            }
        }

        public void HideAttr()
        {
            m_AttrRoot.SetActive(false);
        }


        public void HideName()
        {
            m_Name.gameObject.SetActive(false);
        }

        private void HideMosterHead()
        {
            m_MonsterEliteHead.SetActive(false);
            m_MonsterBossHead.SetActive(false);
        }

        private void Monster_Normal()
        {
            mImageBG_commom_Mp.SetActive(true);
            mImageBG_boss_Mp.SetActive(false);
            m_MonsterEliteHead.SetActive(false);
            m_MonsterBossHead.SetActive(false);
            bgType = 0;
        }

        private void Monster_Boss()
        {
            mImageBG_commom_Mp.SetActive(false);
            mImageBG_boss_Mp.SetActive(true);
            m_MonsterEliteHead.SetActive(false);
            m_MonsterBossHead.SetActive(true);
            bgType = 1;
        }

        private void Monster_Elite()
        {
            mImageBG_commom_Mp.SetActive(false);
            mImageBG_boss_Mp.SetActive(true);
            m_MonsterEliteHead.SetActive(true);
            m_MonsterBossHead.SetActive(false);
            bgType = 1;
        }


        public void Dispose()
        {
            positionCorrect_root.Dispose();
            positionCorrect_battleOrderFlag.Dispose();
            positionCorrect_bttleOrderName.Dispose();
            positionCorrect_battleSelect.Dispose();
            ResetChildOffest();
            CurHp = TargetHp = 0;
            CurMp = 0;
            m_CurShieldValue = -1;
            CurShieldValue = 0;
            m_CurEnergyValue = -1;
            CurEnergyValue = 0;
            mImageBG_boss_NoMp.gameObject.SetActive(false);
            mImageBG_commom_NoMp.gameObject.SetActive(false);
            imgRace.gameObject.SetActive(false);
            mRootGameObject.SetActive(false);
            mBattleOrderName.SetActive(false);
            mBattleOrderFlag.SetActive(false);
            mBattleSelect.SetActive(false);
            m_SelectedFx.SetActive(false);
        }

        /// <summary>
        /// 设置角色HUD变身icon 输入变身id
        /// </summary>
        public void SetRoleShapeShiftIcon(uint shapeShiftId)
        {
            if (shapeShiftId > 0u)
            {
                var csvTrans = CSVTransform.Instance.GetConfData(shapeShiftId);
                if (csvTrans != null)
                {
                    imgRace.gameObject.SetActive(true);
                    ImageHelper.SetIcon(imgRace, csvTrans.genus_icon);
                    return;
                }
            }
            imgRace.gameObject.SetActive(false);
        }

        /// <summary>
        /// 设置角色职业icon (目前只有pvp敌对角色显示)
        /// </summary>
        private void SetRoleCareerIcon(uint careerId)
        {
            var csvCareer = CSVCareer.Instance.GetConfData(careerId);
            if (csvCareer != null)
            {
                imgCareer.gameObject.SetActive(true);
                ImageHelper.SetIcon(imgCareer, csvCareer.logo_icon);
                return;
            }
            imgCareer.gameObject.SetActive(false);
        }
        /// <summary>
        /// 隐藏角色职业icon
        /// </summary>
        public void HideRoleCareerIcon()
        {
            imgCareer.gameObject.SetActive(false);
        }

        /// <summary>
        /// 更新护盾值
        /// </summary>
        public void UpdateShieldValue(float target)
        {
            CurShieldValue = target;
        }
        /// <summary>
        /// 更新能量值（气）
        /// </summary>
        public void UpdateEnemgyValue(float target)
        {
            CurEnergyValue = target;
        }
    }
}

