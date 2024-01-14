using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Table;
using Logic.Core;
using Lib.Core;

namespace Logic
{
    public class PetSkillItem02
    {
        public Transform transform;

        public Image skillImage;
        public Image qualityImage;
        public GameObject select01Go;
        public GameObject select02Go;
        public GameObject buildGo; //改造节点
        public GameObject uniqueGo; // 专属节点
        public GameObject mountGo; // 骑术专属技能
        public GameObject demonSpiritGo; // 魔魂专属节点
        public Text skillText;
        public GameObject fxGo;
        private Action onClick;
        private Action onLongPressed;

        private uint skillId;
        private bool bLongPress = true;
        private bool isInit = false;
        public void Bind(GameObject go)
        {
            transform = go.transform;

            skillImage = transform.Find("Image_Skill").GetComponent<Image>();
            qualityImage = transform.Find("Image_Quality").GetComponent<Image>();
            select01Go = transform.Find("Image_Select01").gameObject;
            select02Go = transform.Find("Image_Select02").gameObject;
            transform.Find("View_Rare").gameObject.SetActive(true);
            buildGo = transform.Find("View_Rare/Image_Gaizao").gameObject;
            uniqueGo = transform.Find("View_Rare/Image_Zhuanshu").gameObject;
            mountGo = transform.Find("View_Rare/Image_Qishu")?.gameObject;
            demonSpiritGo = transform.Find("View_Rare/Image_Mohun")?.gameObject;
            skillText = transform.Find("View_State/Text").GetComponent<Text>();
            fxGo = transform.Find("Image_Skill/Fx_ui_Select03")?.gameObject;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(skillImage.gameObject);
            eventListener.ClearEvents();
            eventListener.AddEventListener(EventTriggerType.PointerClick, (ret) => { OnClicked(); });
            
            UI_LongPressButton uI_LongPressButton = skillImage.gameObject.GetNeedComponent<UI_LongPressButton>();
            uI_LongPressButton.onStartPress.RemoveAllListeners();
            uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
        }

        public void AddClickListener(Action onclicked = null, Action onlongPressed = null)
        {
            onClick = onclicked;
            onLongPressed = onlongPressed;
        }

        public void EnableLongPress(bool longPress)
        {
            bLongPress = longPress;
        }

        private void OnClicked()
        {
            onClick?.Invoke();
        }

        private void OnLongPressed()
        {
            if (bLongPress)
            {
                //open tip
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>( skillId, 0 ));

                onLongPressed?.Invoke();
            }
        }

        public void SetDate(uint skillId)
        {
            this.skillId = skillId;

            if (Sys_Skill.Instance.IsActiveSkill(skillId)) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(skillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(skillImage, skillInfo.icon);
                    ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
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
                    ImageHelper.SetIcon(skillImage, skillInfo.icon);
                    ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                }
                else
                {
                    Debug.LogErrorFormat("not found skillId={0} in CSVPassiveSkillInfoData", skillId);
                }
            }
        }
    }
}

