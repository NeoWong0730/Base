using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_Partner_Config_Right_Cell_Partner : UIParseCommon
    {
        private uint infoId;
        private int formationIndex;
        private int posIndex;
        private PartnerItem01 partnerItem;
        private Toggle toggle;

        protected override void Parse()
        {
            partnerItem = new PartnerItem01();
            partnerItem.Bind(gameObject);
            partnerItem.btnBlank.onClick.AddListener(OnClickBlank);

            toggle = gameObject.GetComponent<Toggle>();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                OnClickSelect(isOn);
            });
        }

        public override void Show()
        {
            Sys_Partner.Instance.eventEmitter.Handle<Sys_Partner.PartnerFormOperation>(Sys_Partner.EEvents.OnFormSelectNotification, OnSelectFormPos, true);
        }

        public override void Hide()
        {
            Sys_Partner.Instance.eventEmitter.Handle<Sys_Partner.PartnerFormOperation>(Sys_Partner.EEvents.OnFormSelectNotification, OnSelectFormPos, false);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void OnClickBlank()
        {
            Sys_Partner.PartnerFormOperation opData = new Sys_Partner.PartnerFormOperation();
            opData.partnerId = infoId;
            opData.formIndex = formationIndex;
            opData.posIndex = posIndex;

            Sys_Partner.Instance.OnDownFormOrReplace(opData);
        }

        private void OnClickSelect(bool _isOn)
        {
            if (_isOn)
            {
                Sys_Partner.PartnerFormOperation opData = new Sys_Partner.PartnerFormOperation();
                opData.partnerId = infoId;
                opData.formIndex = formationIndex;
                opData.posIndex = posIndex;

                Sys_Partner.Instance.OnSelectFormPos(opData);
            }
        }

        private void OnSelectFormPos(Sys_Partner.PartnerFormOperation _formData)
        {
            bool isSelected = Sys_Partner.Instance.IsSelectedFormPos(formationIndex, posIndex);
            partnerItem.imgSelect.gameObject.SetActive(isSelected);
            toggle.isOn = isSelected;

            if (infoId != 0)
            {
                if (_formData.formIndex == formationIndex)
                {
                    if (_formData.posIndex == posIndex)
                    {
                        partnerItem.imgBlank.gameObject.SetActive(true);
                        partnerItem.imgArrowDown.gameObject.SetActive(true);
                        partnerItem.imgLeftRight.gameObject.SetActive(false);
                    }
                    else if (_formData.partnerId != 0)
                    {
                        partnerItem.imgBlank.gameObject.SetActive(true);
                        partnerItem.imgArrowDown.gameObject.SetActive(false);
                        partnerItem.imgLeftRight.gameObject.SetActive(true);
                    }
                    else
                    {
                        partnerItem.imgBlank.gameObject.SetActive(false);
                        partnerItem.imgArrowDown.gameObject.SetActive(false);
                        partnerItem.imgLeftRight.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void UpdateInfo(uint _infoId, int _formIndex, int _index)
        {
            infoId = _infoId;
            formationIndex = _formIndex;
            posIndex = _index;

            toggle.isOn = false;

            bool isSelected = Sys_Partner.Instance.IsSelectedFormPos(formationIndex, posIndex);
            partnerItem.imgSelect.gameObject.SetActive(isSelected);

            bool isCanOp = false;
            if (infoId != 0)
                isCanOp = true;

            if (!isCanOp)
            {
                isCanOp = Sys_Partner.Instance.IsPosCanOp(formationIndex, posIndex);
            }

            toggle.enabled = isCanOp;

            partnerItem.HideBlank();
            partnerItem.imgIcon.gameObject.SetActive(infoId != 0);
            partnerItem.imgAdd.gameObject.SetActive(infoId == 0 && isCanOp);

            if (infoId != 0)
            {
                //更换阵容图片
                CSVPartner.Data baseData = CSVPartner.Instance.GetConfData(infoId);
                ImageHelper.SetIcon(partnerItem.imgIcon, baseData.battle_headID);
            }
        }
    }
}
