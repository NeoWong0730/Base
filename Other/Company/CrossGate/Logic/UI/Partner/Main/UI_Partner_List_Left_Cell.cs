using System;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Partner_List_Left_Cell : UIParseCommon
    {
        //UI
        private CP_Toggle toggle;
        private Image imgProfession;
        private Text textShow;
        private Image imgProfessionLight;
        private Text textLight;
        //private GameObject selectObj;

        private int gridIndex;
        private Action<int> _action;

        protected override void Parse()
        {
            toggle = transform.gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(OnClickToggle);

            imgProfession = toggle.transform.Find("Image_Frame/Image_Profession").GetComponent<Image>();
            textShow = toggle.transform.Find("Text_Dark").GetComponent<Text>();

            imgProfessionLight = toggle.transform.Find("Image_Select/Image_Profession").GetComponent<Image>();
            textLight = toggle.transform.Find("Image_Select/Text_Light").GetComponent<Text>();
            //selectObj = toggle.transform.Find("Image_Select").gameObject;
        }

        public override void Show()
        {
            
        }

        public override void Hide()
        {
            
        }

        private void OnClickToggle(bool isOn)
        {
            if (isOn)
            {
                _action?.Invoke(gridIndex);
            }
        }

        public void AddListener(Action<int> action)
        {
            _action = action;
        }

        //public void ToggleOff()
        //{
        //    toggle.SetSelected(false, true);
        //}

        public void UpdateInfo(uint lanId, uint iconId, int index)
        {
            gridIndex = index;

            if (lanId == 0) //all
            {
                textShow.text = LanguageHelper.GetTextContent(2006003);
                textLight.text = LanguageHelper.GetTextContent(2006003);
            }
            else
            {
                textShow.text = LanguageHelper.GetTextContent(lanId);
                textLight.text = LanguageHelper.GetTextContent(lanId);
                ImageHelper.SetIcon(imgProfession, iconId);
                ImageHelper.SetIcon(imgProfessionLight, iconId);
            }

            toggle.SetSelected(Sys_Partner.Instance.SelectIndex == index, true);
        }
    }
}
