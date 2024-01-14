using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Lib.Core;
using DG.Tweening;

namespace Logic
{
    public class UI_Energyspar_Main_Left_Ceil
    {
        public GameObject gameObject;
        private Button selectBtn;
        private Image iconImage;
        private GameObject selectGo;
        private Text levelText;
        private GameObject lockGo;
        private GameObject smallStarGo;
        private Transform starGroumpGo;

        public CSVStone.Data energysparData;
        public StoneSkillData currentStoneData;
        private Animator unlockAni;
        private Animator otherAnis;
        private GameObject starAniGo;
        private Action<UI_Energyspar_Main_Left_Ceil> action;
        public bool isWaitAni;
        
        public void Init(Transform transform)
        {
            gameObject = transform.gameObject;
            selectBtn = transform.Find("Button").GetComponent<Button>();
            selectBtn.onClick.AddListener(OnSelectBtnClick);
            iconImage = transform.Find("Button/Icon").GetComponent<Image>();
            otherAnis = iconImage.GetComponent<Animator>();
            selectGo = transform.Find("Button/Image_Select").gameObject;
            levelText = transform.Find("Level_bg/Text_Level").GetComponent<Text>();
            lockGo = transform.Find("Image_Lock").gameObject;
            unlockAni = transform.Find("Image_Lock_Animator").GetComponent<Animator>();
            smallStarGo = transform.Find("Star_Dark").gameObject;
            starGroumpGo = transform.Find("StarGroup");
            starAniGo = transform.Find("Button/Icon/Fx_ui_trail_02").gameObject;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetCeilData(CSVStone.Data data)
        {
            energysparData = data;
            AnimatorGameObjectInit();
            isWaitAni = false;
            UpdateInfo();

        }

        private void AnimatorGameObjectInit()
        {
            for (int i = 0; i < unlockAni.transform.childCount; i++)
            {
                unlockAni.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < iconImage.transform.childCount; i++)
            {
                Transform trans = iconImage.transform.GetChild(i);
                trans.gameObject.SetActive(false);
                if(trans.name == "Fx_ui_trail_02")
                {
                    trans.localPosition = Vector3.zero;
                }
            }           
        }

        private void UpdateInfo()
        {
            currentStoneData = Sys_StoneSkill.Instance.GetServerDataById(energysparData.id);
            if(energysparData != null)
            {
                ImageHelper.SetIcon(iconImage, energysparData.icon);
            }           
            if (currentStoneData != null)
            {
                TextHelper.SetText(levelText, LanguageHelper.GetTextContent(2021003, currentStoneData.powerStoneUnit.Level.ToString()));
                FrameworkTool.DestroyChildren(starGroumpGo.gameObject);
                for (int i = 0; i < currentStoneData.powerStoneUnit.Stage; i++)
                {
                    GameObject go = GameObject.Instantiate<GameObject>(smallStarGo, starGroumpGo);
                    Small_Star small_Star = new Small_Star();
                    small_Star.BindGameobject(go.transform);
                    small_Star.SetState(true);
                }

                if(Sys_StoneSkill.Instance.CanAdvance(energysparData.id))
                {
                    GameObject go = GameObject.Instantiate<GameObject>(smallStarGo, starGroumpGo);
                    Small_Star small_Star = new Small_Star();
                    small_Star.BindGameobject(go.transform);
                    small_Star.SetState(false);                    
                }
                levelText.transform.parent.gameObject.SetActive(true);
                ImageHelper.SetImageGray(iconImage, false);
                starGroumpGo.gameObject.SetActive(true);
                lockGo.SetActive(false);
            }
            else
            {
                levelText.transform.parent.gameObject.SetActive(false);
                starGroumpGo.gameObject.SetActive(false);
                lockGo.SetActive(energysparData.level_limit > Sys_Role.Instance.Role.Level);
                if (Sys_StoneSkill.Instance.nextLevelLock.Contains(energysparData.id))
                {
                    unlockAni.Play("Lock_Open", -1, 0);
                    Sys_StoneSkill.Instance.RemoveLevelUnLcokList(energysparData.id);
                }
                    
                ImageHelper.SetImageGray(iconImage, true);
            }
        }

        public void Reset()
        {
            UpdateInfo();
        }

        public void ActiveAni()
        {
            otherAnis.Play("Fx_ui_Energyspar01_Open", -1, 0); 
        }

        public void AdvanceStoneAni(Transform trans)
        {
            AnimatorGameObjectInit();
            starAniGo.SetActive(true);
            starAniGo.transform.DOLocalMove(new Vector3(-gameObject.transform.localPosition.x, -gameObject.transform.localPosition.y, 0), 1.5f).OnComplete(() =>
            {
                starAniGo.SetActive(false);
                Sys_StoneSkill.Instance.eventEmitter.Trigger(Sys_StoneSkill.EEvents.FlyStarEnd);

            });
        }

        public void WaiteAnimator()
        {
            isWaitAni = true;
        }

        public void PlayWaitAnimator()
        { 
            if(isWaitAni)
            {
                isWaitAni = false;
                if (Sys_StoneSkill.Instance.ChenckData(energysparData.id))
                {
                    otherAnis.Play("Fx_ui_Skillup_Open", -1, 0);
                    UpdateInfo();
                }
                else
                {
                    otherAnis.Play("Fx_ui_dianji01_Open", -1, 0);
                }
            }            
        }

        public void AddActionListen(Action<UI_Energyspar_Main_Left_Ceil> action)
        {
            this.action = action;
        }        

        public void OnSelectBtnClick()
        {
            action?.Invoke(this);
        }

        public void SetSelectState(bool state)
        {
            if(!selectGo.activeSelf == state)
                selectGo.SetActive(state);
        }
    }
}
