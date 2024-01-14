using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Table;
using System;
using Logic.Core;

namespace Logic
{
    public class SkillItem01
    {
        public Transform transform;

        public Button btnNormal;
        public Image imgSelect;
        public Image imgIcon;

        private Image imgType;
        public Text textLevel;
        public Text textName;

        private uint mSkillId;
        private Action<uint> mAction;
        private Action<uint> onLongPressed;

        private bool bLongPress = true;

        public void Bind(GameObject go)
        {
            transform = go.transform;

            btnNormal = transform.Find("Button_Normal").GetComponent<Button>();
            //btnNormal.onClick.AddListener(OnClickSkill);
            imgSelect = transform.Find("Image_Select").GetComponent<Image>();
            imgIcon = transform.Find("Image_Icon").GetComponent<Image>();

            imgType = transform.Find("Image_Icon/Image_Tip").GetComponent<Image>();
            textLevel = transform.Find("Text_Level").GetComponent<Text>();
            textLevel.text = "";
            textName = transform.Find("Text_Name").GetComponent<Text>();

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(btnNormal.gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (ret) => { OnClickSkill(); });

            UI_LongPressButton uI_LongPressButton = btnNormal.gameObject.AddComponent<UI_LongPressButton>();
            uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
        }

        private void OnClickSkill()
        {
            UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(mSkillId, 0));

            if (mAction != null)
            {
                mAction.Invoke(mSkillId);
            }
        }

        private void OnLongPressed()
        {
            if (bLongPress)
            {
                onLongPressed?.Invoke(mSkillId);
            }
        }

        public void SetData(uint skillId)
        {
            mSkillId = skillId;

            if (Sys_Skill.Instance.IsActiveSkill(mSkillId)) //主动技能
            {
                CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(mSkillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(imgIcon, skillInfo.icon);
                    textName.text = LanguageHelper.GetTextContent(skillInfo.name);
                    //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                }
                else
                {
                    Debug.LogErrorFormat("CSVActiveSkillInfo 未找到 skillId={0}", mSkillId);
                }

                imgType.gameObject.SetActive(true);
                ImageHelper.SetIcon(imgType, skillInfo.typeicon);
            }
            else
            {
                CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(mSkillId);
                if (skillInfo != null)
                {
                    ImageHelper.SetIcon(imgIcon, skillInfo.icon);
                    textName.text = LanguageHelper.GetTextContent(skillInfo.name);
                    //ImageHelper.GetPetSkillQuality_Frame(qualityImage, (int)skillInfo.quality);
                }
                else
                {
                    Debug.LogErrorFormat("CSVPassiveSkillInfo 未找到 skillId={0}", mSkillId);
                }

                imgType.gameObject.SetActive(false);
            }
        }

        public void OnEnableBtn(bool isEnable)
        {
            btnNormal.enabled = isEnable;
        }

        public void RegisterAction(Action<uint> action)
        {
            mAction = action;
        }

        public void RegisterLongPress(Action<uint> action)
        {
            onLongPressed = action;
        }
    }
}

