using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Packet;
using Table;
using Logic.Core;

namespace Logic
{
    public class UI_Equipment_Inlay_Slot : UIParseCommon
    {
        public uint inlayPos;
        public uint jewelInfoId;

        private Item0_Layout layout;
        private RawImage imgPlus;
        private Image imgSelect;
        private Button btnMinus;

        private Text gemName;
        private Text propName;
        private Text propValue;

        private Image imgUp;
        private Button btnImprove;

        private ulong _equipUId;

        private IListener listener;

        private bool selected = false;
        private GameObject particleGo = null;

        protected override void Parse()
        {
            base.Parse();

            particleGo = transform.Find("GemItem/Fx_ui_item01").gameObject;

            layout = new Item0_Layout();
            layout.BindGameObject(transform.Find("GemItem/Btn_Item").gameObject);
            layout.btnItem.onClick.AddListener(OnClickSelect);

            imgPlus = transform.Find("GemItem/Image_Plus").GetComponent<RawImage>();
            imgSelect = transform.Find("GemItem/Image_Select").GetComponent<Image>();
            btnMinus = transform.Find("GemItem/Button_Minus").GetComponent<Button>();
            btnMinus.onClick.AddListener(OnClickMinus);

            gemName = transform.Find("Text_Name").GetComponent<Text>();

            imgUp = transform.Find("GemItem/Image_Up").GetComponent<Image>();
            btnImprove = transform.Find("GemItem/Image_Improve").GetComponent<Button>();
            btnImprove.onClick.AddListener(() =>
            {
                JewelQuickImproveData data = new JewelQuickImproveData();
                data.equipUId = _equipUId;
                data.jewelInfoId = jewelInfoId;
                data.slotIndex = inlayPos;

                UIManager.OpenUI(EUIID.UI_Jewel_Upgrade, false, data);
            });

            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfInlay, OnNtfInlay, true);
        }

        public override void OnDestroy()
        {
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnJewelNtfInlay, OnNtfInlay, false);
        }

        private void OnClickMinus()
        {
            listener?.OnUnloadJewelPos(inlayPos);
        }

        private void OnClickSelect()
        {
            listener?.OnSelectInlayPos(inlayPos);
        }

        private void OnNtfInlay()
        {
            if (selected)
            {
                particleGo.SetActive(false);
                particleGo.SetActive(true);
            }
        }

        public void ResiterListener(IListener _listener)
        {
            listener = _listener;
        }

        public void OnSelect(bool _select)
        {
            imgSelect.gameObject.SetActive(_select);

            btnMinus.gameObject.SetActive(_select && jewelInfoId != 0);

            selected = _select;
        }

        public void UpdateInfo(uint infoId, ItemData curEquip)
        {
            jewelInfoId = infoId;
            _equipUId = curEquip != null ? curEquip.Uuid : 0L;

            bool isUp = false;
            bool isImprove = false;

            if (jewelInfoId != 0)
            {
                CSVItem.Data jewelItem = CSVItem.Instance.GetConfData(jewelInfoId);

                layout.SetData(jewelItem, true);

                TextHelper.SetText(gemName, jewelItem.name_id);

                imgPlus.gameObject.SetActive(false);

                isUp = Sys_Equip.Instance.IsEquiped(curEquip) && Sys_Equip.Instance.IsJewelCanReplaceHigh(curEquip, jewelInfoId);
                isImprove = Sys_Equip.Instance.IsJewelCanImprove(curEquip, jewelInfoId);
            }
            else
            {
                layout.imgQuality.gameObject.SetActive(false);
                layout.imgIcon.gameObject.SetActive(false);
                gemName.text = "";
                imgPlus.gameObject.SetActive(true);
                btnMinus.gameObject.SetActive(false);

                if (curEquip != null)
                {
                    isUp = Sys_Equip.Instance.IsEquiped(curEquip) && Sys_Equip.Instance.IsSlotCanInlay(curEquip);
                }
            }

            imgUp.gameObject.SetActive(isUp);
            btnImprove.gameObject.SetActive(isImprove);
        }

        public interface IListener
        {
            void OnSelectInlayPos(uint _pos);
            void OnUnloadJewelPos(uint _pos);
        }
    }
}


