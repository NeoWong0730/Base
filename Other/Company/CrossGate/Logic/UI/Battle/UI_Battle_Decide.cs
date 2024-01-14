using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Packet;
using System;

namespace Logic
{
    public class UI_Battle_Decide_Item : UIComponent
    {
        private Text name;
        private Text heroHp;
        private Text heroMp;
        private Text petHp;
        private Text petMp;
        private Text petCallTime;
        private Text useNormalTime;
        private Text useSpecialTime;
        private GroupScore.Types.RoleScore roleScore;
        private bool isSelf;

        protected override void Loaded()
        {
            heroHp = transform.Find("Image0/Text").GetComponent<Text>();
            heroMp = transform.Find("Image1/Text").GetComponent<Text>();
            petHp = transform.Find("Image2/Text").GetComponent<Text>();
            petMp = transform.Find("Image3/Text").GetComponent<Text>();
            petCallTime = transform.Find("Image4/Text").GetComponent<Text>();
            useNormalTime = transform.Find("Image5/Text").GetComponent<Text>();
            useSpecialTime = transform.Find("Image6/Text").GetComponent<Text>();
            name = transform.Find("Image_Item/Text").GetComponent<Text>();
        }

        public UI_Battle_Decide_Item(GroupScore.Types.RoleScore _roleScore,bool _isSelf)  : base()
        {
            roleScore = _roleScore;
            isSelf = _isSelf;
        }

        public override void Show()
        {
            if (isSelf)
            {
                heroHp.text = roleScore.RoleHpScore.ToString();
                heroMp.text = roleScore.RoleMpScore.ToString();
                petHp.text = roleScore.PetHpScore.ToString();
                petMp.text = roleScore.PetMpScore.ToString();
            }
            else
            {
                heroHp.text = "???";
                heroMp.text = "???";
                petHp.text = "???";
                petMp.text = "???";
            }
            petCallTime.text = roleScore.PetSummonScore.ToString();
            useNormalTime.text = roleScore.ItemScore.ToString();
            useSpecialTime.text = roleScore.SpecialItemScore.ToString();
            if (MobManager.Instance.m_MobDic[roleScore.UnitId].m_MobCombatComponent != null && MobManager.Instance.m_MobDic[roleScore.UnitId].m_MobCombatComponent.m_BattleUnit != null)
            {
                name.text = MobManager.Instance.m_MobDic[roleScore.UnitId].m_MobCombatComponent.m_BattleUnit.RoleName.ToStringUtf8();
            }

        }

        public override void Hide()
        {
            roleScore = null;
        }
    }

    public class UI_Battle_Decide : UIBase
    {
        private Text maxRound;
        private Text selfPoint;
        private Text enemyPoint;
        private GameObject selfItemRoot;
        private GameObject enemyItemRoot;
        private Button closeBtn;

        private List<UI_Battle_Decide_Item> items = new List<UI_Battle_Decide_Item>();
        private GroupScore selfData = null;
        private GroupScore enemyData = null;
        protected override void OnLoaded()
        {
            maxRound = transform.Find("Animator/Image_Maxbg/Text_Max").GetComponent<Text>();
            selfPoint = transform.Find("Animator/List/Text_Point/Text_Value").GetComponent<Text>();
            enemyPoint = transform.Find("Animator/List (1)/Text_Point/Text_Value").GetComponent<Text>();
            selfItemRoot = transform.Find("Animator/List/Scroll View/Viewport/Content").gameObject;
            enemyItemRoot = transform.Find("Animator/List (1)/Scroll View/Viewport/Content").gameObject;
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }

        protected override void OnShow()
        {
            CSVBattleType.Data csvBattleTypeData = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            maxRound.text = csvBattleTypeData.max_round.ToString();
            SelfInfo();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Net_Combat.Instance.eventEmitter.Handle(Net_Combat.EEvents.OnScoreInfo, OnScoreInfo, toRegister);
        }

        private void OnScoreInfo()
        {
            SelfInfo();
        }

        protected override void OnHide()
        {
            DefaultItems();
            selfData = null;
            enemyData = null;
        }

        private void SelfInfo()
        {
            if (GameCenter.fightControl == null || GameCenter.fightControl.groupScores.Count != 2)
            {
                return;
            }
            items.Clear();
            DefaultItems();
            if (Net_Combat.Instance.m_IsWatchBattle)
            {
                selfData = GameCenter.fightControl.groupScores[0];
                enemyData = GameCenter.fightControl.groupScores[1];
            }
            else
            {
                if (GameCenter.mainFightHero.battleUnit.Side == 0)
                {
                    if (GameCenter.fightControl.groupScores[0].GroupId == 1)
                    {
                        selfData = GameCenter.fightControl.groupScores[0];
                        enemyData = GameCenter.fightControl.groupScores[1];
                    }
                    else
                    {
                        enemyData = GameCenter.fightControl.groupScores[0];
                        selfData = GameCenter.fightControl.groupScores[1];
                    }
                }
                else
                {
                    if (GameCenter.fightControl.groupScores[0].GroupId == 2)
                    {
                        selfData = GameCenter.fightControl.groupScores[0];
                        enemyData = GameCenter.fightControl.groupScores[1];
                    }
                    else
                    {
                        enemyData = GameCenter.fightControl.groupScores[0];
                        selfData = GameCenter.fightControl.groupScores[1];
                    }
                }
            }
            selfPoint.text = selfData.TotalScore.ToString();
            enemyPoint.text = enemyData.TotalScore.ToString();
            for (int i = 0; i < selfItemRoot.transform.childCount; ++i)
            {
                Transform trans = selfItemRoot.transform.GetChild(i);
                if (selfData.RoleScore.Count > i )
                {
                    UI_Battle_Decide_Item item = new UI_Battle_Decide_Item(selfData.RoleScore[i], !Net_Combat.Instance.m_IsWatchBattle);
                    item.Init(trans);
                    item.Show();
                    items.Add(item);
                }
                else
                {
                    trans.gameObject.SetActive(false);
                }
            }
            for (int i = 0; i < enemyItemRoot.transform.childCount; ++i)
            {
                Transform trans = enemyItemRoot.transform.GetChild(i);
                if (enemyData.RoleScore.Count > i )
                {
                    UI_Battle_Decide_Item item = new UI_Battle_Decide_Item(enemyData.RoleScore[i],false);
                item.Init(enemyItemRoot.transform.GetChild(i));
                item.Show();
                items.Add(item);
                }
                else
                {
                    trans.gameObject.SetActive(false);
                }
            }
        }

        private void DefaultItems()
        {
            for(int i = 0; i < items.Count; ++i)
            {
                items[i].OnDestroy();
            }
        }

        private void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Battle_Decide);
        }
    }
}
