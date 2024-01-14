using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using System;
using Logic.Core;

namespace Logic
{
    public class UI_EquipIconRoot : UIParseCommon
    {
        private ulong _uId = 0;

        private ItemData curItem;

        private CP_Toggle cpToggle;
        private EquipItem equipItem;

        private Image imgSelect;
        private Text textName;
        private Image img_Up;

        private GameObject view_Score;
        private Text textLevel;
        private Text textScore;

        private GameObject view_Repair;
        private Text textRepair;

        private GameObject view_Jewel;
        private List<Image> jewelList = new List<Image>();

        private bool _IsEquiped;
        private bool _IsUp;

        private Action<ulong> selectAction;

        protected override void Parse()
        {
            cpToggle = gameObject.GetComponent<CP_Toggle>();
            cpToggle.onValueChanged.AddListener(OnClickToggle);

            equipItem = new EquipItem();
            equipItem.Bind(transform.Find("EquipItem").gameObject);
            equipItem.Layout.btnItem.onClick.AddListener(OnClickEquipment);

            imgSelect = transform.Find("Image_Select").GetComponent<Image>();
            img_Up = transform.Find("Image_Up").GetComponent<Image>();

            textName = transform.Find("Text_Name").GetComponent<Text>();

            view_Score = transform.Find("View_Score").gameObject;
            textLevel = view_Score.transform.Find("Text_LevelNum").GetComponent<Text>();
            textScore = view_Score.transform.Find("Text_Point/Text_PointNum").GetComponent<Text>();

            view_Repair = transform.Find("View_Repair").gameObject;
            textRepair = view_Repair.transform.Find("Text_RepairNum").GetComponent<Text>();

            view_Jewel = transform.Find("View_Gem").gameObject;

            jewelList.Add(view_Jewel.transform.Find("Image_Gem").GetComponent<Image>());
            jewelList.Add(view_Jewel.transform.Find("Image_Gem (1)").GetComponent<Image>());
            jewelList.Add(view_Jewel.transform.Find("Image_Gem (2)").GetComponent<Image>());
        }

        private void OnClickToggle(bool isOn)
        {
            if (isOn)
            {
                Sys_Equip.Instance.CurOpEquipUId = curItem.Uuid;
                selectAction?.Invoke(_uId);
            }
        }

        private void OnClickEquipment()
        {
            if (curItem != null)
            {
                EquipTipsData tipData = new EquipTipsData();
                tipData.equip = curItem;
                tipData.isCompare = false;

                UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
            }
        }

        public void AddListener(Action<ulong> _action)
        {
            selectAction = _action;
        }

        public void UpdateEquipInfo(ulong uId)
        {
            _uId = uId;

            _IsEquiped = false;
            _IsUp = false;

            curItem = Sys_Equip.Instance.GetItemData(_uId);
            if (null == curItem)
            {
                Debug.LogErrorFormat("未找到 Uid = {0} 的装备", _uId);
                return;
            }

            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(curItem.Id);
            equipItem.SetData(curItem);

            _IsEquiped = Sys_Equip.Instance.IsEquiped(curItem);

            equipItem.img_Equiped.gameObject.SetActive(_IsEquiped);

            textName.text = Sys_Equip.Instance.GetEquipmentName(curItem);

            img_Up.gameObject.SetActive(false);
            view_Score.SetActive(false);
            view_Repair.SetActive(false);
            view_Jewel.SetActive(false);

            switch (Sys_Equip.Instance.curOperationType)
            {
                case Sys_Equip.EquipmentOperations.Inlay:
                    {
                        view_Jewel.SetActive(true);

                        _IsUp = _IsEquiped && Sys_Equip.Instance.IsSlotCanInlay(curItem);

                        for (int i = 0; i < jewelList.Count; ++i)
                        {
                            jewelList[i].gameObject.SetActive(i < equipInfo.jewel_number);

                            Image jewelIcon = jewelList[i].transform.Find("Image").GetComponent<Image>();
                            jewelIcon.enabled = false;
                        }

                        for (int i = 0; i < curItem.Equip.JewelinfoId.Count; ++i)
                        {
                            uint jewelInfoId = curItem.Equip.JewelinfoId[i];
                            if (jewelInfoId != 0)
                            {
                                jewelList[i].gameObject.SetActive(true);
                                UpdateJewel(jewelList[i], jewelInfoId);
                            }
                        }

                        img_Up.gameObject.SetActive(_IsUp);
                    }
                    break;
                case Sys_Equip.EquipmentOperations.Smelt:
                case Sys_Equip.EquipmentOperations.Quenching:
                case Sys_Equip.EquipmentOperations.Repair:
                case Sys_Equip.EquipmentOperations.Make:
                    {
                        view_Score.SetActive(true);
                        textLevel.text = LanguageHelper.GetTextContent(1000002, equipInfo.TransLevel().ToString());
                        textScore.text = Sys_Equip.Instance.CalEquipTotalScore(curItem).ToString();
                    }
                    break;
                default:
                    break;
            }

            bool isSelect = Sys_Equip.Instance.CurOpEquipUId == curItem.Uuid;
            cpToggle.SetSelected(isSelect, true);
        }

        private void UpdateJewel(Image imgJewel, uint jewelInfoId)
        {
            CSVItem.Data jewelItem = CSVItem.Instance.GetConfData(jewelInfoId);
            Image jewelIcon = imgJewel.transform.Find("Image").GetComponent<Image>();
            jewelIcon.enabled = true;
            ImageHelper.SetIcon(jewelIcon, jewelItem.icon_id);

            if (_IsEquiped && !_IsUp)
            {
                _IsUp = Sys_Equip.Instance.IsJewelCanReplaceHigh(curItem, jewelInfoId) || Sys_Equip.Instance.IsJewelBagCanImprove(curItem, jewelInfoId);
            }
        }

        public void OnSelect(bool _select)
        {
            imgSelect.gameObject.SetActive(_select);
        }

        public void Refresh()
        {
            if (_uId != 0)
            {
                UpdateEquipInfo(_uId);
            }
        }
    }
}