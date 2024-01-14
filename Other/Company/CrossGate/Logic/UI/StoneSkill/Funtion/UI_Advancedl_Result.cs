using System.Collections.Generic;
using UnityEngine.UI;
using Logic.Core;
using System;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Advancedl_Result : UIBase
    {
        private uint currentId;
        private Image skillImage;
        private GameObject starGo;
        private Transform starGroup;
        private Button closeBtn;
        protected override void OnLoaded()
        {
            skillImage = transform.Find("Animator/View/Image_Skillbg/Image_Icon").GetComponent<Image>();
            closeBtn = transform.Find("Image_BG").GetComponent<Button>();
            starGo = transform.Find("Animator/View/Image_Skillbg/Star_Dark").gameObject;
            starGroup = transform.Find("Animator/View/Image_Skillbg/StarGroup");
            closeBtn.onClick.AddListener(CloseBtnClick);
        }

        protected override void OnOpen(object arg)
        {
            if (null != arg)
                currentId = (uint)arg;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            CSVStone.Data currentData = CSVStone.Instance.GetConfData(currentId);
            if (null != currentData)
            {
                ImageHelper.SetIcon(skillImage, currentData.icon);
                StoneSkillData severData = Sys_StoneSkill.Instance.GetServerDataById(currentId);
                FrameworkTool.DestroyChildren(starGroup.gameObject);
                if (null != severData)
                {
                    uint currentState = severData.powerStoneUnit.Stage;
                    for (int i = 0; i < currentData.max_stage; i++)
                    {
                        bool isLight = i < currentState;
                        GameObject go = GameObject.Instantiate<GameObject>(starGo, starGroup);
                        Small_Star small_Star = new Small_Star();
                        small_Star.BindGameobject(go.transform);
                        small_Star.SetState(isLight);
                    }
                }
            }
        }

        protected override void OnHide()
        {

        }

        private void CloseBtnClick()
        {
            Sys_StoneSkill.Instance.eventEmitter.Trigger(Sys_StoneSkill.EEvents.AdvancedResultClose, currentId);
            UIManager.CloseUI(EUIID.UI_Advancedl_Result);
        }
    }
}

