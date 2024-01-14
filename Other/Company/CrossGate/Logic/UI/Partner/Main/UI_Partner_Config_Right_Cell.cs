using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_Partner_Config_Right_Cell : UIParseCommon
    {
        public int formationIndex;
        private PartnerFormation formation;

        //UI
        private CP_Toggle toggle;
        private Button btnBg;
        private Text textConfig0;
        private Text textConfig1;
        private List<UI_Partner_Config_Right_Cell_Partner> listPartnerItems = new List<UI_Partner_Config_Right_Cell_Partner>();

        protected override void Parse()
        {
            toggle = transform.gameObject.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                OnToggleClick(isOn, formationIndex);
            });

            btnBg = transform.Find("Image_BG").GetComponent<Button>();
            btnBg.onClick.AddListener(OnClickBg);

            textConfig0 = transform.Find("Text").GetComponent<Text>();
            textConfig1 = transform.Find("Text/Text").GetComponent<Text>();

            Transform grid = transform.Find("Grid");
            for (int i = 0; i < grid.childCount; ++i)
            {
                Transform trans = grid.transform.GetChild(i);
                UI_Partner_Config_Right_Cell_Partner partner = new UI_Partner_Config_Right_Cell_Partner();
                partner.Init(trans);
                listPartnerItems.Add(partner);
            }
        }

        public override void Show()
        {
            foreach (UI_Partner_Config_Right_Cell_Partner posPartner in listPartnerItems)
            {
                posPartner.Show();
            }


            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormRefreshNotification, OnRefresh, false);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormRefreshNotification, OnRefresh, true);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormationSelectedNtf, OnFromationSelected, false);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormationSelectedNtf, OnFromationSelected, true);
        }

        public override void Hide()
        {
            foreach (UI_Partner_Config_Right_Cell_Partner posPartner in listPartnerItems)
            {
                posPartner.Hide();
            }

            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormRefreshNotification, OnRefresh, false);
            Sys_Partner.Instance.eventEmitter.Handle(Sys_Partner.EEvents.OnFormationSelectedNtf, OnFromationSelected, false);
        }

        private void OnToggleClick(bool _select, int _index)
        {
            if (_select)
            {
                Sys_Partner.Instance.SetSelectFormation(_index);
            }
        }

        private void OnClickBg()
        {
            Sys_Partner.Instance.ClearSelectState();
        }

        private void OnRefresh()
        {
            UpdateInfo(formationIndex);
        }

        private void OnFromationSelected()
        {
            //bool isSelected = Sys_Partner.Instance.IsSelectedFormation(formationIndex);
            //ImageHelper.SetGraphicGray(btnBg, !isSelected);
        }

        public void UpdateInfo(int index)
        {
            formationIndex = index;
            formation = Sys_Partner.Instance.GetFormationByIndex(formationIndex);

            bool isSelected = Sys_Partner.Instance.IsSelectedFormation(index);
            toggle.SetSelected(isSelected, true);

            if (formation != null)
            {
                for (int i = 0; i < listPartnerItems.Count; ++i)
                {
                    listPartnerItems[i].UpdateInfo(formation.Pa[i], formationIndex, i);
                }
            }
            else
            {
                Debug.LogError("Partner formation error!!");
            }

            //textConfig0.text = textConfig1.text = LanguageHelper.GetTextContent((uint)(2006006 + index));
        }
    }
}
