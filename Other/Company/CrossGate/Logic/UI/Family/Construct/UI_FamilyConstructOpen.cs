using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using System;


namespace Logic
{
    public class UI_FamilyConstructOpen : UIComponent
    {
        public Button arrowBtn;
        public Button gotoBtn;
        public GameObject arrowLeftGo;
        public GameObject arrowRightGo;
        public bool arrowShow = false;
        public Text coinNumText;
        public Text leftCoinNumText;
        public GameObject leftGo;        
        private List<UI_ConstructLevelSlider> constructLevelInfos = new List<UI_ConstructLevelSlider>();

        protected override void Loaded()
        {
            arrowBtn = transform.Find("Button").GetComponent<Button>();
            arrowBtn.onClick.AddListener(ArrowBtnBtnClicked);
            coinNumText =transform.Find("Text_Coin").GetComponent<Text>();
            leftGo = transform.Find("Left").gameObject;
            arrowLeftGo = transform.Find("Button/Image1").gameObject;
            arrowRightGo = transform.Find("Button/Image2").gameObject;
            gotoBtn = transform.Find("Left/Btn_03").GetComponent<Button>();
            gotoBtn.onClick.AddListener(GotoBtnBtnClicked);
            leftCoinNumText = transform.Find("Left/Image_BG/Text_Title/Text_Coin").GetComponent<Text>();

            var values = System.Enum.GetValues(typeof(EConstructs));
            for (int i = 0, count = values.Length; i < count; i++)
            {
                EConstructs type = (EConstructs)values.GetValue(i);
                UI_ConstructLevelSlider constructSlider = null;
                string path = string.Empty;
                switch (type)
                {
                    case EConstructs.Agriculture:
                        {
                            path = "Left/Content/Agri";
                        }
                        break;
                    case EConstructs.Business:
                        {
                            path = "Left/Content/Business";
                        }
                        break;
                    case EConstructs.Security:
                        {
                            path = "Left/Content/Safe";
                        }
                        break;
                    case EConstructs.Religion:
                        {
                            path = "Left/Content/Rei";
                        }
                        break;
                    case EConstructs.Technology:
                        {
                            path = "Left/Content/Science";
                        }
                        break;
                }
                if (!string.IsNullOrEmpty(path))
                    constructSlider = AddComponent<UI_ConstructLevelSlider>(transform.Find(path));
                constructLevelInfos.Add(constructSlider);
            }
        }

        public void SetSys()
        {
            bool show = Sys_Family.Instance.IsNeedShowFamilyConstructWindows();
            SetState(show);
            if (show)
            {
                ResetView();
            }
        }

        public void Init()
        {
            bool show = Sys_Family.Instance.IsNeedShowFamilyConstructWindows();
            SetState(show);
            if (show)
            {
                arrowShow = false;
            }
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            uint level = Sys_Family.Instance.familyData.GetConstructLevel();
            leftCoinNumText.text = Sys_Family.Instance.familyData.GetGuidStamina().ToString();
            CSVFamilyProsperity.Data familyConstructLevelData = CSVFamilyProsperity.Instance.GetConfData(level);
            if (null != familyConstructLevelData)
            {
                var values = System.Enum.GetValues(typeof(EConstructs));
                for (int i = 0, count = values.Length; i < count; i++)
                {
                    EConstructs type = (EConstructs)values.GetValue(i);
                    constructLevelInfos[i].SetSliderInfo(type, Sys_Family.Instance.GetClientDataExp(type, familyConstructLevelData), Sys_Family.Instance.familyData.GetConstructExp(type));
                }
            }
        }
       
        /// <summary>
        /// 设置是否显示 
        /// </summary>
        /// <param name="state">界面是否打开 true 为打开</param>
        public void SetState(bool state)
        {
            if(null != transform && null != transform.gameObject)
            {
                transform.gameObject.SetActive(state);
                if (state)
                {
                    coinNumText.text = Sys_Family.Instance.familyData.GetGuidStamina().ToString();
                    ArrowState();
                }
            }
        }

        private void ArrowState()
        {
            arrowLeftGo.SetActive(!arrowShow);
            arrowRightGo.SetActive(arrowShow);
            leftGo.gameObject.SetActive(arrowShow);
            if (arrowShow)
            {
                RefreshView();
            }
        }

        private void ResetView()
        {
            coinNumText.text = Sys_Family.Instance.familyData.GetGuidStamina().ToString();
            RefreshView();
        }

        private void ArrowBtnBtnClicked()
        {
            arrowShow = !arrowShow;
            ArrowState();            
        }

        private void GotoBtnBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Family_Construct);
        }
    }
}
