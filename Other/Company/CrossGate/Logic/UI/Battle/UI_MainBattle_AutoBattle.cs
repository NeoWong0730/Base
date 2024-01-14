using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_MainBattle_AutoBattle : UIComponent
    {
        public Button btnRole01;
        public Image roleIcon01;
        public Text roleLevel01;
        public Button btnRole02;
        public Image roleIcon02;
        public Text roleLevel02;
        public Button btnPet;
        public Image petIcon;
        public Text petLevel;
        public Toggle autofightToggle;
        public GameObject autofightOff;
        public GameObject autofightOn;

        public float passtime = Constants.OPERATIONTIME;
        public bool flag = false;
     //   public bool isGuide = false;
        private List<SkillComponent.SkillData> tempSkilldic = new List<SkillComponent.SkillData>();
        private List<uint> AvaliableSkilldic = new List<uint>();


        protected override void Loaded()
        {
            base.Loaded();

            btnRole01 = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Role01").GetComponent<Button>();
            btnRole01.onClick.AddListener(OnAutoRole01);
            roleIcon01 = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Role01/Image_Icon").GetComponent<Image>();
            roleLevel01 = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Role01/Image_Level/Text").GetComponent<Text>();

            btnRole02 = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Role02").GetComponent<Button>();
            btnRole02.onClick.AddListener(OnAutoRole02);
            roleIcon02 = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Role02/Image_Icon").GetComponent<Image>();
            roleLevel02 = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Role02/Image_Level/Text").GetComponent<Text>();

            btnPet = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Pet").GetComponent<Button>();
            btnPet.onClick.AddListener(OnAutoPet);
            petIcon = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Pet/Image_Icon").GetComponent<Image>();
            petLevel = transform.Find("Toggle_AutoFight/AutoFight_On/Grid01/Button_Pet/Image_Level/Text").GetComponent<Text>();

            autofightToggle = transform.Find("Toggle_AutoFight").GetComponent<Toggle>();
            autofightToggle.onValueChanged.AddListener(onautofightToggleChange);
            autofightOff = transform.Find("Toggle_AutoFight/AutoFight_Off").gameObject;
            autofightOn = transform.Find("Toggle_AutoFight/AutoFight_On").gameObject;
        }

        public void ProcessEventsRegiste(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Net_Combat.Instance.eventEmitter.Handle<bool>(Net_Combat.EEvents.OnAutoFight, OnAutoFight, toRegister);
        }

        private void OnAutoFight(bool isOn)
        {
            autofightToggle.SetIsOnWithoutNotify(isOn);
        }

        private void onautofightToggleChange(bool isOn)
        {
            Sys_Fight.Instance.AutoFightReq(!Sys_Fight.Instance.AutoFightData.AutoState);
        }

        private void OnAutoRole01()
        {
            GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForFirstOperation;
            UIManager.OpenUI(EUIID.UI_MainBattle_Skills, false, true);
        }

        private void OnAutoRole02()
        {
            GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForSecondOperation;
            UIManager.OpenUI(EUIID.UI_MainBattle_Skills, false, false);
        }

        private void OnAutoPet()
        {
            GameCenter.fightControl.operationState = FightControl.EOperationState.WaitForSecondOperation;
            UIManager.OpenUI(EUIID.UI_MainBattle_Skills, false, false);
        }

        public void RefreshShow(AutoBattleSkillEvt skillevt,bool isIconUpdata)
        {
            if (GameCenter.mainFightHero != null)
            {
                tempSkilldic.Clear();
                tempSkilldic = ActiveList();
                if (GameCenter.fightControl.isGuide)
                {
                    GameCenter.fightControl.isGuide = false;
                    if (tempSkilldic.Count == 0)
                    {
                        ImageHelper.SetIcon(roleIcon01, Constants.NEARNORMALATTACKID);
                        roleLevel01.text = CSVActiveSkillInfo.Instance.GetConfData(Constants.NEARNORMALATTACKID).level.ToString();
                        if(!isIconUpdata)
                            Sys_Fight.Instance.SetAutoSkillReq(Constants.NEARNORMALATTACKID, true, true);
                    }
                    else
                    {
                        ImageHelper.SetIcon(roleIcon01, tempSkilldic[0].CSVActiveSkillInfoData.icon);
                        roleLevel01.text = CSVActiveSkillInfo.Instance.GetConfData(tempSkilldic[0].CSVActiveSkillInfoData.id).level.ToString();
                        if (!isIconUpdata)
                            Sys_Fight.Instance.SetAutoSkillReq(tempSkilldic[0].CSVActiveSkillInfoData.id, true, true);
                    }
                    if (GameCenter.mainFightPet != null)
                    {
                        if (GameCenter.mainFightPet.fightPetSkillComponent.GetHoldingActiveSkillsExt((uint)GameCenter.mainFightPet.battleUnit.WeaponId).Count == 0)
                        {
                            ImageHelper.SetIcon(petIcon, 992302);
                            petLevel.text = CSVActiveSkillInfo.Instance.GetConfData(Constants.NEARNORMALATTACKID).level.ToString();
                            if (!isIconUpdata)
                                Sys_Fight.Instance.SetAutoSkillReq(Constants.NEARNORMALATTACKID, false, false);
                        }
                        else
                        {
                            SkillComponent.SkillData data = GameCenter.mainFightPet.fightPetSkillComponent.GetHoldingActiveSkillsExt((uint)GameCenter.mainFightPet.battleUnit.WeaponId)[0];
                            ImageHelper.SetIcon(petIcon, CSVActiveSkillInfo.Instance.GetConfData(data.CSVActiveSkillInfoData.id).icon);
                            petLevel.text = CSVActiveSkillInfo.Instance.GetConfData(data.CSVActiveSkillInfoData.id).level.ToString();
                            if (!isIconUpdata)
                                Sys_Fight.Instance.SetAutoSkillReq(data.CSVActiveSkillInfoData.id, false, false);
                        }
                    }
                    else
                    {
                        ImageHelper.SetIcon(roleIcon02, CSVActiveSkillInfo.Instance.GetConfData( skillevt.heroid2).icon);
                        roleLevel02.text = CSVActiveSkillInfo.Instance.GetConfData(skillevt.heroid2).level.ToString();
                    }
                }
                else
                {
                    if (skillevt.heroid != 0)
                    {
                        if (IsAvaliable(skillevt.heroid) || skillevt.heroid <= 102 || skillevt.heroid==211)
                        {
                            ImageHelper.SetIcon(roleIcon01, CSVActiveSkillInfo.Instance.GetConfData(skillevt.heroid).icon);
                            roleLevel01.text = CSVActiveSkillInfo.Instance.GetConfData(skillevt.heroid).level.ToString();
                        }
                        else
                        {
                            uint updateskillinfoid = 0;
                            if (isUpdateSkill(skillevt.heroid, out updateskillinfoid)&& IsAvaliable(updateskillinfoid))
                            {
                                ImageHelper.SetIcon(roleIcon01, CSVActiveSkillInfo.Instance.GetConfData(updateskillinfoid).icon);
                                roleLevel01.text = CSVActiveSkillInfo.Instance.GetConfData(updateskillinfoid).level.ToString();
                                if (!isIconUpdata)
                                    Sys_Fight.Instance.SetAutoSkillReq(updateskillinfoid, true, true);
                            }
                            else
                            {
                                CSVActiveSkillInfo.Data data01 = CSVActiveSkillInfo.Instance.GetConfData(CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId).auto_battle[0]);
                                ImageHelper.SetIcon(roleIcon01, data01.icon);
                                roleLevel01.text = data01.level.ToString();
                                if (!isIconUpdata)
                                    Sys_Fight.Instance.SetAutoSkillReq(data01.active_skillid,true);
                            }
                        }
                    }
                    if (skillevt.heroid2 != 0&& GameCenter.mainFightPet == null)
                    {
                        if (IsAvaliable(skillevt.heroid2) || skillevt.heroid2 <= 102 || skillevt.heroid2==211)
                        {
                            ImageHelper.SetIcon(roleIcon02, CSVActiveSkillInfo.Instance.GetConfData(skillevt.heroid2).icon);
                            roleLevel02.text = CSVActiveSkillInfo.Instance.GetConfData(skillevt.heroid2).level.ToString();
                        }
                        else
                        {
                            uint updateskillinfoid = 0;
                            if (isUpdateSkill(skillevt.heroid2, out updateskillinfoid))
                            {
                                ImageHelper.SetIcon(roleIcon02, CSVActiveSkillInfo.Instance.GetConfData(updateskillinfoid).icon);
                                roleLevel02.text = CSVActiveSkillInfo.Instance.GetConfData(updateskillinfoid).level.ToString();
                                if (!isIconUpdata)
                                    Sys_Fight.Instance.SetAutoSkillReq(updateskillinfoid, false, true);
                            }
                            else
                            {
                                CSVActiveSkillInfo.Data data02 = CSVActiveSkillInfo.Instance.GetConfData(CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId).auto_battle[1]);
                                ImageHelper.SetIcon(roleIcon02, data02.icon);
                                roleLevel02.text = data02.level.ToString();
                                if (!isIconUpdata)
                                    Sys_Fight.Instance.SetAutoSkillReq(data02.active_skillid, false);

                            }
                        }
                    }
                    if (skillevt.petid != 0 && GameCenter.mainFightPet != null)
                    {
                        ClientPet pet = Sys_Pet.Instance.GetFightPetClient((uint)GameCenter.mainFightPet.battleUnit.PetId);
                        if (pet != null)
                        {
                            if (pet.GetPetSkillList().Contains(skillevt.petid)|| skillevt.petid==211)
                            {
                                ImageHelper.SetIcon(petIcon, CSVActiveSkillInfo.Instance.GetConfData(skillevt.petid).icon);
                                petLevel.text = CSVActiveSkillInfo.Instance.GetConfData(skillevt.petid).level.ToString();
                                if (skillevt.isPetSkillUpdate)
                                {
                                    Sys_Fight.Instance.SetAutoSkillReq(skillevt.petid, true, false);
                                }
                            }
                            else
                            {
                                if (skillevt.petid == 1001)
                                {
                                    ImageHelper.SetIcon(petIcon, 992302);
                                    petLevel.text = CSVActiveSkillInfo.Instance.GetConfData(1001).level.ToString();
                                    if (!isIconUpdata)
                                        Sys_Fight.Instance.SetAutoSkillReq(skillevt.petid, true, false);
                                }
                                else if (skillevt.petid == 101)
                                {
                                    ImageHelper.SetIcon(petIcon, CSVActiveSkillInfo.Instance.GetConfData(101).icon);
                                    petLevel.text = CSVActiveSkillInfo.Instance.GetConfData(101).level.ToString();
                                    if (!isIconUpdata)
                                        Sys_Fight.Instance.SetAutoSkillReq(skillevt.petid, true, false);
                                }
                            }
                        }
                    }
                    if (skillevt.heroid > 1001 || skillevt.heroid2 > 1001)
                    {
                        GameCenter.fightControl.CanUseSkill = false;
                    }
                    else
                    {
                        GameCenter.fightControl.CanUseSkill = true;
                    }
                }
            }

            if (Sys_Fight.Instance.HasPet())
            {

                btnPet.gameObject.SetActive(true);
                btnRole02.gameObject.SetActive(false);
            }
            else
            {
                btnPet.gameObject.SetActive(false);
                btnRole02.gameObject.SetActive(true);
            }
        }

        private List<SkillComponent.SkillData>ActiveList()
        {
            tempSkilldic.Clear();
            tempSkilldic = GameCenter.mainFightHero.heroSkillComponent.GetHoldingActiveSkillsExt((uint)GameCenter.mainFightHero.battleUnit.WeaponId);
            //if (GameCenter.mainFightHero.battleUnit.UnitInfoId == 1080)
            //{
            //    tempSkilldic = GameCenter.mainFightHero.heroSkillComponent.GetHoldingActiveSkillsExt();
            //}
            //else
            //{
            //    tempSkilldic = GameCenter.mainFightHero.heroSkillComponent.GetHoldingActiveSkillsExt();
            //}
            for (int i = tempSkilldic.Count - 1; i >= 0; i--)
            {

                if (Sys_Attr.Instance.pkAttrs[27] < CSVActiveSkill.Instance.GetConfData(tempSkilldic[i].CSVActiveSkillInfoData.id).min_spirit || GameCenter.fightControl.HaveSkillCold(tempSkilldic[i].CSVActiveSkillInfoData.id) != null)
                {
                    tempSkilldic.Remove(tempSkilldic[i]);
                }
            }
            return tempSkilldic;
        }

        private bool IsAvaliable(uint skillinfoid)
        {
            tempSkilldic.Clear();
            tempSkilldic = ActiveList();
            AvaliableSkilldic.Clear();
            for (int i=0;i< tempSkilldic.Count;++i)
            {
                if (tempSkilldic[i].Available)
                {
                    AvaliableSkilldic.Add(tempSkilldic[i].CSVActiveSkillInfoData.id);
                }
            }
            if (AvaliableSkilldic.Contains(skillinfoid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool isUpdateSkill(uint skillinfoid, out uint updateskillinfoid)
        {
            tempSkilldic.Clear();
            tempSkilldic = ActiveList();
            for (int i = 0; i < tempSkilldic.Count; ++i)
            {
                if (tempSkilldic[i].CSVActiveSkillInfoData.skill_id == CSVActiveSkillInfo.Instance.GetConfData(skillinfoid).skill_id)
                {
                    updateskillinfoid = ActiveList()[i].CSVActiveSkillInfoData.id;
                    return true;
                }
            }
            updateskillinfoid = 0;
            return false;
        }
    }
}
