using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using System;

namespace Logic
{
    public class UI_Repair_EquipRoot : UIParseCommon
    {
        public ulong UId { get; private set; } = 0;
        public int GridIndex { get; private set; }
        private ItemData curItem;

        //UI
        private EquipItem equipItem;

        private Text textName;
        private Text textLevel;
        private Text textScore;

        private bool isUp;
        private Image img_Up;

        private Image imgSelect;

        private Toggle toggle;

        private Action<UI_Repair_EquipRoot> selectAction;

        protected override void Parse()
        {
            toggle = gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((select) =>
            {
                if (select)
                {
                    selectAction?.Invoke(this);
                }
            });

            equipItem = new EquipItem();
            equipItem.Bind(transform.Find("EquipItem").gameObject);

            textName = transform.Find("Text_Name").GetComponent<Text>();

            textLevel = transform.Find("Text_Repair").GetComponent<Text>();

            textScore = transform.Find("Text_Repair/Text_RepairNum").GetComponent<Text>();

            img_Up = transform.Find("Image_Up").GetComponent<Image>();

            imgSelect = transform.Find("Image_Select").GetComponent<Image>();
        }

        public void AddListener(Action<UI_Repair_EquipRoot> _action)
        {
            selectAction = _action;
        }

        public  void UpdateEquipInfo(ulong _uId, int _index)
        {
            UId = _uId;
            GridIndex = _index;

            curItem = Sys_Equip.Instance.GetItemData(_uId);

            equipItem.SetData(curItem);
            TextHelper.SetText(textName, curItem.cSVItemData.name_id);

            CSVEquipment.Data equipData = CSVEquipment.Instance.GetConfData(curItem.Id);
            textLevel.text = equipData.equipment_level.ToString();

            equipItem.img_Equiped.gameObject.SetActive(Sys_Equip.Instance.IsEquiped(curItem));

            img_Up.gameObject.SetActive(isUp);
        }

        public void ToggleOff()
        {
            toggle.isOn = false;
        }

        public void OnSelect(bool _select)
        {
            imgSelect.gameObject.SetActive(_select);
        }

        public void Refresh()
        {
            if (UId != 0)
            {
                UpdateEquipInfo(UId, GridIndex);
            }
        }
    }
}