using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public enum EPartnerType
    {
        Career = 0,
        Area = 1
    }

    public class UI_Partner_List_Top : UIParseCommon
    {
        private Button btnArrow;
        private Image imgArrow;
        private Text textSelected;

        private GameObject labSelectObj;
        private List<Toggle> listToggles = new List<Toggle>();

        private IListener listener;

        private EPartnerType curType = EPartnerType.Career;

        protected override void Parse()
        {
            btnArrow = transform.Find("View_Select01/Btn_Arrow").GetComponent<Button>();
            btnArrow.onClick.AddListener(OnClickBtnArrow);

            imgArrow = transform.Find("View_Select01/Btn_Arrow/Image_Arrow").GetComponent<Image>();
            textSelected = transform.Find("View_Select01/Text").GetComponent<Text>();

            labSelectObj = transform.Find("View_Select01/Lab_Select").gameObject;
            labSelectObj.GetComponent<ToggleGroup>().allowSwitchOff = true;

            int length = Enum.GetValues(typeof(EPartnerType)).Length;
            for (int i = 0; i < length; ++i)
            {
                Toggle toggle = labSelectObj.transform.Find($"SelectNow0{i + 1}").GetComponent<Toggle>();
                toggle.isOn = false;
                listToggles.Add(toggle);
                toggle.onValueChanged.AddListener((_isOn) =>
                {
                    OnToggleClick(_isOn, listToggles.IndexOf(toggle));
                });
            }
        }

        public override void Show()
        {
            listToggles[(int)EPartnerType.Career].isOn = true;
        }

        public override void Hide()
        {
            //foreach (Toggle toggle in listToggles)
            //{
            //    toggle.isOn = false;
            //}

            labSelectObj.SetActive(false);
            imgArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        }

        private void OnClickBtnArrow()
        {
            bool isExpand = labSelectObj.activeSelf;
            labSelectObj.SetActive(!isExpand);

            float rotateZ = isExpand ? 90f : 0f;
            imgArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        private void OnToggleClick(bool _select, int _index)
        {
            if (_select)
            {
                curType = (EPartnerType)_index;
                listener?.OnSelectType(curType);

                labSelectObj.SetActive(false);

                textSelected.text = LanguageHelper.GetTextContent(2006004 + (uint)_index);
                imgArrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSelectType(EPartnerType _type);
        }
    }
}
