using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Lib.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Sequence_Layout
    {
        public Transform transform;
        public GameObject infoGo;
        public GameObject item01Go;
        public GameObject item02Go;
        public GameObject title02Go;
        public Button closeBtn;
        public Text title;

        public void Init(Transform transform)
        {
            this.transform = transform;
            infoGo = transform.Find("Animator/Open/View_Content/VerticalGrid").gameObject;
            item01Go = transform.Find("Animator/Open/View_Content/VerticalGrid/Grid01/Item").gameObject;
            item02Go = transform.Find("Animator/Open/View_Content/VerticalGrid/Grid02/Item").gameObject;
            title02Go = transform.Find("Animator/Open/View_Content/VerticalGrid/Image_Title02").gameObject;
            closeBtn = transform.Find("Animator/Open/Btn_Open").GetComponent<Button>();
            title = transform.Find("Animator/Open/View_BG/Text_Title").GetComponent<Text>();
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

    public class UI_Sequence_Item : UIComponent
    {
        public Image leftIcon;
        public Text leftName;
        public Image rightIcon;
        public Text rightName;
        public BattleUnit battleUnit;

        public UI_Sequence_Item(BattleUnit _battleUnit) : base()
        {
            battleUnit = _battleUnit;
        }

        protected override void Loaded()
        {
            leftIcon = transform.Find("Icon_Left").GetComponent<Image>();
            leftName = transform.Find("Icon_Left/Text").GetComponent<Text>();
            rightIcon = transform.Find("Icon_Right").GetComponent<Image>();
            rightName = transform.Find("Icon_Right/Text").GetComponent<Text>();
        }

        public void RefreshItem()
        {
            bool selfCamp = false;
            if (Net_Combat.Instance.m_IsWatchBattle)
            {
                BattleUnit watchbattleUnit = MobManager.Instance.GetBeWatchBattleUnit();
                selfCamp = CombatHelp.IsSameCamp(battleUnit, battleUnit);
            }
            else
            {
                selfCamp = CombatHelp.IsSameCamp(battleUnit, GameCenter.mainFightHero.battleUnit);
            }
            leftIcon.gameObject.SetActive(!selfCamp);
            rightIcon.gameObject.SetActive(selfCamp);
            if (battleUnit.UnitType == (uint)UnitType.Monster)
            {
                gameObject.SetActive(true);
                CSVMonster.Instance.TryGetValue(battleUnit.UnitInfoId, out CSVMonster.Data csvMonsterData);               
                if (csvMonsterData != null)
                {
                    if (selfCamp)
                    {
                        ImageHelper.SetIcon(rightIcon, GetMonsterIconIdByRank(csvMonsterData.monster_rank));
                        rightName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(csvMonsterData.monster_name), 2007252);
                    }
                    else
                    {
                        ImageHelper.SetIcon(leftIcon, GetMonsterIconIdByRank(csvMonsterData.monster_rank));
                        leftName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(csvMonsterData.monster_name), 2007252);
                    }
                }
            }
            else if (battleUnit.UnitType == (uint)UnitType.Pet)
            {
                gameObject.SetActive(true);
                uint.TryParse(CSVParam.Instance.GetConfData(623).str_value, out uint petIconid);
                CSVPetNew.Data csvPetNewData = CSVPetNew.Instance.GetConfData(battleUnit.UnitInfoId);
                if (selfCamp)
                {
                    ImageHelper.SetIcon(rightIcon, petIconid);
                    if (battleUnit.PetName.IsEmpty || battleUnit.PetName.Length==0)
                    {
                        rightName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(csvPetNewData.name), 2007254);

                    }
                    else
                    {
                        rightName.text = LanguageHelper.GetLanguageColorWordsFormat(battleUnit.PetName.ToStringUtf8(), 2007254);
                    }
                }
                else
                {
                    ImageHelper.SetIcon(leftIcon, petIconid);
                    if (battleUnit.PetName.IsEmpty || battleUnit.PetName.Length == 0)
                    {
                        leftName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(csvPetNewData.name), 2007254);

                    }
                    else
                    {
                        leftName.text = LanguageHelper.GetLanguageColorWordsFormat(battleUnit.PetName.ToStringUtf8(), 2007254);
                    }
                }

            }
            else if (battleUnit.UnitType == (uint)UnitType.Partner)
            {
                gameObject.SetActive(true);
                uint proId = CSVPartner.Instance.GetConfData(battleUnit.UnitInfoId).profession;
                if (selfCamp)
                {
                    ImageHelper.SetIcon(rightIcon, CSVCareer.Instance.GetConfData(proId).logo_icon);
                    rightName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(CSVPartner.Instance.GetConfData(battleUnit.UnitInfoId).name), 2007253);

                }
                else
                {
                    ImageHelper.SetIcon(leftIcon, CSVCareer.Instance.GetConfData(proId).logo_icon);
                    leftName.text = LanguageHelper.GetLanguageColorWordsFormat(LanguageHelper.GetTextContent(CSVPartner.Instance.GetConfData(battleUnit.UnitInfoId).name), 2007253);
                }

            }
            else if (battleUnit.UnitType == (uint)UnitType.Hero || battleUnit.ServerUnitType == 8)
            {
                gameObject.SetActive(true);
                if (battleUnit.ServerUnitType == 8)
                {
                    CSVMonster.Instance.TryGetValue((uint)battleUnit.RoleId, out CSVMonster.Data csvMonsterData);
                    if (csvMonsterData != null)
                    {
                        rightName.text = LanguageHelper.GetTextContent(csvMonsterData.monster_name);
                        leftName.text = LanguageHelper.GetTextContent(csvMonsterData.monster_name);
                    }
                }
                else
                {
                    rightName.text = LanguageHelper.GetLanguageColorWordsFormat(battleUnit.RoleName.ToStringUtf8(), 2007251);
                    leftName.text = LanguageHelper.GetLanguageColorWordsFormat(battleUnit.RoleName.ToStringUtf8(), 2007251);
                }
                if (battleUnit.RoleCareer ==0 || battleUnit.RoleCareer == (uint)ECareerType.None)
                {
                    if (selfCamp)
                    {
                        ImageHelper.SetIcon(rightIcon, CSVCareer.Instance.GetConfData(100).logo_icon);
                    }
                    else
                    {
                        ImageHelper.SetIcon(leftIcon, CSVCareer.Instance.GetConfData(100).logo_icon);
                    }
                }
                else
                {
                    if (selfCamp)
                    {
                        ImageHelper.SetIcon(rightIcon, CSVCareer.Instance.GetConfData(battleUnit.RoleCareer).logo_icon);
                    }
                    else
                    {
                        ImageHelper.SetIcon(leftIcon, CSVCareer.Instance.GetConfData(battleUnit.RoleCareer).logo_icon);
                    }
                }
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        private uint GetMonsterIconIdByRank(uint rankId)
        {
            uint id = 0;
            if (rankId == 1)
            {
                id = 620;
            }
            else if (rankId == 2)
            {
                id = 621;
            }
            else if (rankId == 3)
            {
                id = 622;
            }
            else
            {
                id = 623;
            }
            uint.TryParse(CSVParam.Instance.GetConfData(id).str_value, out uint Iconid);
            return Iconid;
        }
    }

    public class UI_Sequence : UIBase, UI_Sequence_Layout.IListener
    {
        private UI_Sequence_Layout layout = new UI_Sequence_Layout();
        private List<UI_Sequence_Item> itemList = new List<UI_Sequence_Item>();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            if (Net_Combat.Instance.m_TempAttackCountDic.Count == 0 || GameCenter.fightControl.sequenceRound==0)
            {
                layout.infoGo.SetActive(false);
            }
            else
            {
                SetMessage();
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnSetExcuteTurns, OnSetExcuteTurns, toRegister);
        }        

        private void OnSetExcuteTurns()
        {
            if (Net_Combat.Instance.m_TempAttackCountDic.Count == 0)
            {
                return;
            }
            SetMessage();
        }

        private void SetMessage()
        {
            layout.infoGo.SetActive(true);
            itemList.Clear();
            DefaultBuffList();
            bool hasSecond = false;
            layout.title.text = LanguageHelper.GetTextContent(3000011, GameCenter.fightControl.sequenceRound .ToString());
            foreach (var item in Net_Combat.Instance.m_TempAttackCountDic)
            {
                MobEntity mobEntity = MobManager.Instance.GetMob(item.Key);
                if(mobEntity == null)
                {
                    return;
                }
                MobCombatComponent mobCombatComponent = mobEntity.m_MobCombatComponent;
                BattleUnit battleUnit = mobEntity.m_MobCombatComponent.m_BattleUnit;
                if (mobCombatComponent != null && battleUnit != null)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(layout.item01Go, layout.item01Go.transform.parent);
                    UI_Sequence_Item sequenceItem = new UI_Sequence_Item(battleUnit);
                    sequenceItem.Init(go.transform);
                    sequenceItem.RefreshItem();
                    itemList.Add(sequenceItem);
                    if (item.Value > 1)
                    {
                        GameObject go02 = GameObject.Instantiate<GameObject>(layout.item02Go, layout.item02Go.transform.parent);
                        sequenceItem.Init(go02.transform);
                        sequenceItem.RefreshItem();
                        itemList.Add(sequenceItem);
                        hasSecond = true;
                    }
                }
            }
            layout.item01Go.SetActive(false);
            layout.item02Go.SetActive(false);
            layout.title02Go.SetActive(hasSecond);
            layout.item02Go.transform.parent.gameObject.SetActive(hasSecond);
            ForceRebuildLayout(layout.infoGo);
        }

        private void DefaultBuffList()
        {
            layout.item01Go.SetActive(true);
            layout.item02Go.SetActive(true);
            for (int i = 0; i < itemList.Count; ++i) { itemList[i].OnDestroy(); }
            FrameworkTool.DestroyChildren(layout.item01Go.transform.parent.gameObject, layout.item01Go.transform.name);
            FrameworkTool.DestroyChildren(layout.item02Go.transform.parent.gameObject, layout.item02Go.transform.name);
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

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Sequence);
        }
    }
}
