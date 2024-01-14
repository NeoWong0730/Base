using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_Pet_Demon_Detail_Layout
    {
        private Button closeBtn;
        private Button downBtn;
        private Button upExpBtn;
        public Transform topTransform;

        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew03/Btn_Close").GetComponent<Button>();

            downBtn = transform.Find("Animator/View_Content/Btn_01").GetComponent<Button>();

            upExpBtn = transform.Find("Animator/View_Content/Btn_02").GetComponent<Button>();

            topTransform = transform.Find("Animator/View_Content");
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            upExpBtn.onClick.AddListener(listener.UpExpBtnClicked);
            downBtn.onClick.AddListener(listener.DownBtnClicked);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void UpExpBtnClicked();

            void DownBtnClicked();
        }
    }

    public class UI_Pet_DemonDetailTop : UIComponent
    {
        private GameObject leftPresentSkillGo;
        private GameObject leftNextSkillGo;

        private GameObject rightPresentSkillGo;
        private GameObject rightNextSkillGo;

        private Button leftPresentSkillBtn;
        private Button leftNextSkillBtn;
        private Button rightPresentSkillBtn;
        private Button rightNextSkillBtn;


        private PetSoulBeadInfo sphereTemp;
        protected CSVSoulBead.Data sphereData;
        private Text nameText;
        private Text levelText;
        private Text scoreText;

        private Slider expSlider;
        private Text percentText;
        private Image sphereImage;

        protected override void Loaded()
        {
            nameText = transform.Find("Text_Name").GetComponent<Text>();
            levelText = transform.Find("Text_Name/Text").GetComponent<Text>();
            scoreText = transform.Find("Text_Name/Text_Score").GetComponent<Text>();

            expSlider = transform.Find("EXP/Slider_Lv").GetComponent<Slider>();
            percentText =transform.Find("EXP/Text_Percent").GetComponent<Text>();
            sphereImage = transform.Find("Image/Image4").GetComponent<Image>();


            leftPresentSkillGo = transform.Find("Skill/Skill1").gameObject;
            leftPresentSkillBtn = transform.Find("Skill/Skill1/Button").GetComponent<Button>();
            leftPresentSkillBtn.onClick.AddListener(Skill1BtnClicked);

            rightPresentSkillGo = transform.Find("Skill/Skill2").gameObject;
            rightPresentSkillBtn = transform.Find("Skill/Skill2/Button").GetComponent<Button>();
            rightPresentSkillBtn.onClick.AddListener(Skill2BtnClicked);

        }
        private void Skill1BtnClicked()
        {
            uint skill = sphereTemp.SkillIds[0];
            if (skill > 0)
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index, 1));
            //UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skill, 0));
        }

        private void Skill2BtnClicked()
        {
            uint skill = sphereTemp.SkillIds[1];
            if (skill > 0)
                UIManager.OpenUI(EUIID.UI_Pet_DemonSkill_Tips, false, new Tuple<uint, uint, uint>(sphereTemp.Type, sphereTemp.Index, 2));
            //UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(skill, 0));
        }

        public void SetView(PetSoulBeadInfo sphereTemp)
        {
            if (null != sphereTemp)
            {
                this.sphereTemp = sphereTemp;
                uint level = sphereTemp.Level;
                var sphereId = sphereTemp.Type * 10000 + level;
                sphereData = CSVSoulBead.Instance.GetConfData(sphereId);
                if (null != sphereData)
                {
                    TextHelper.SetText(scoreText, 680003034, sphereData.score.ToString());
                    ImageHelper.SetIcon(sphereImage, sphereData.icon);
                    TextHelper.SetText(levelText, 680003020, level.ToString());
                    TextHelper.SetText(nameText, 680003010u + sphereData.type - 1);
                    if(sphereData.exp == 0)
                    {
                        expSlider.value = 1;
                    }
                    else
                    {
                        expSlider.value = (sphereTemp.Exp + 0f) / sphereData.exp;
                    }
                    percentText.gameObject.SetActive(sphereData.exp != 0);
                    TextHelper.SetText(percentText, 680003039, sphereTemp.Exp.ToString(), sphereData.exp.ToString());
                }
                uint skill1 = sphereTemp.SkillIds[0];
                uint skill2 = sphereTemp.SkillIds[1];
                SetPreSentSkill(leftPresentSkillGo, skill1);
                SetPreSentSkill(rightPresentSkillGo, skill2);
            }
        }

        private void SetPreSentSkill(GameObject skillGo, uint skillId)
        {
            bool hasSkill = skillId > 0;
            var levelText = skillGo.transform.Find("Text_Level").GetComponent<Text>();
            if (hasSkill)
            {
                if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);

                    if (skillInfo != null)
                    {
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in  CSVActiveSkillInfoData", skillId);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(levelText, 680003020, skillInfo.level.ToString());
                        ImageHelper.SetIcon(skillGo.transform.Find("PetSkillItem01/Image_Skill").GetComponent<Image>(), skillInfo.icon);
                        ImageHelper.GetPetSkillQuality_Frame(skillGo.transform.Find("PetSkillItem01/Image_Quality").GetComponent<Image>(), (int)skillInfo.quality);
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                    }
                }
                var unlockGo = skillGo.transform.Find("Lock").gameObject;
                var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                levelGroundImageGo?.SetActive(true);
                unlockGo?.SetActive(false);
            }
            else
            {
                var lockData = CSVSoulBead.Instance.GetSkillLockLevelData(sphereData.type);
                if (null != lockData)
                {
                    TextHelper.SetText(levelText, 680003035, lockData.level.ToString());//解锁等级
                    var unlockGo = skillGo.transform.Find("Lock").gameObject;
                    var levelGroundImageGo = skillGo.transform.Find("Image_Level").gameObject;
                    levelGroundImageGo?.SetActive(false);
                    unlockGo?.SetActive(true);
                }
            }
            skillGo.transform.Find("PetSkillItem01").gameObject.SetActive(hasSkill);
        }

    }


    public class UI_Pet_Demon_Detail : UIBase, UI_Pet_Demon_Detail_Layout.IListener
    {
        private UI_Pet_Demon_Detail_Layout layout = new UI_Pet_Demon_Detail_Layout();
        private UI_Pet_DemonDetailTop demonDetailTop;
        private PetSoulBeadInfo petSoulBeadInfo;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            demonDetailTop = AddComponent<UI_Pet_DemonDetailTop>(layout.topTransform);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnRefreshDemonSpiritSkill, OnRefreshDemonSpiritSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnDemonSpiritUpgrade, OnRefreshDemonSpiritSkill, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle(Sys_Pet.EEvents.OnSaveOrDelectDemonSpiritSphereSkill, OnRefreshDemonSpiritSkill, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            petSoulBeadInfo = arg as PetSoulBeadInfo;
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if(null != petSoulBeadInfo)
            {
                demonDetailTop.SetView(petSoulBeadInfo);
            }
        }

        private void OnRefreshDemonSpiritSkill()
        {
            if (null != petSoulBeadInfo)
            {
                petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(petSoulBeadInfo.Type, petSoulBeadInfo.Index);
                demonDetailTop.SetView(petSoulBeadInfo);
            }
        }

        private void OnRefreshDemonSpiritSkill(uint isCrit)
        {
            if (null != petSoulBeadInfo)
            {
                petSoulBeadInfo = Sys_Pet.Instance.GetPetDemonSpiritSphereInfo(petSoulBeadInfo.Type, petSoulBeadInfo.Index);
                demonDetailTop.SetView(petSoulBeadInfo);
            }
        }


        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Pet_Demon_Detail);
        }


        public void UpExpBtnClicked()
        {
            if (null != petSoulBeadInfo)
            {
                UIManager.OpenUI(EUIID.UI_Pet_DemonUpgrade, false, petSoulBeadInfo);
            }
        }

        public void DownBtnClicked()
        {
            if (null != petSoulBeadInfo)
            {
                Sys_Pet.Instance.PetSoulAssembleBeadReq(petSoulBeadInfo.PetUid, petSoulBeadInfo.Index, petSoulBeadInfo.Type, 2);
                UIManager.CloseUI(EUIID.UI_Pet_Demon_Detail);
            }
        }
    }
}