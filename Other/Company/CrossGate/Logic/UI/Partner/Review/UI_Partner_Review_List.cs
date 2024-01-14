using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Lib.Core;
using Table;

namespace Logic
{
    public class UI_Partner_Review_List
    {
        private class PartnerCell
        {
            private Transform transform;

            private CP_Toggle _toggle;
            private Image _imgFrame;
            private Image _imgIcon;
            private Image _imgFight;
            private RawImage _rawImgLock;

            private uint _partnerId;
            private Action<uint> _action;

            public void Init(Transform trans)
            {
                transform = trans;

                _toggle = transform.GetComponent<CP_Toggle>();
                _toggle.onValueChanged.AddListener(OnClickToggle);

                _imgFrame = transform.Find("Background").GetComponent<Image>();
                _imgIcon = transform.Find("Icon").GetComponent<Image>();
                _imgFight = transform.Find("Imag_Fight").GetComponent<Image>();
                _rawImgLock = transform.Find("Image_Lock").GetComponent<RawImage>();
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(_partnerId);
                }
            }

            public void Register(Action<uint> action)
            {
                _action = action;
            }

            public void UpdateInfo(uint partnerId)
            {
                _partnerId = partnerId;

                CSVPartner.Data data = CSVPartner.Instance.GetConfData(partnerId);

                CSVPartnerQuality.Data qualityData = CSVPartnerQuality.Instance.GetConfData(data.quality);
                ImageHelper.SetIcon(_imgFrame, qualityData.profile);

                ImageHelper.SetIcon(_imgIcon, data.battle_headID);
                _rawImgLock.gameObject.SetActive(!Sys_Partner.Instance.IsUnLock(partnerId));

                _toggle.SetSelected(partnerId == Sys_Partner.Instance.SelectPartnerId, true);

                _imgFight.gameObject.SetActive(Sys_Partner.Instance.IsInForm(_partnerId));
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;

        private List<uint> listIds = new List<uint>();

        private IListener _listener;

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            PartnerCell entry = new PartnerCell();
            entry.Init(cell.mRootTransform);
            entry.Register(OnSelectParnter);

            cell.BindUserData(entry);

            //dicEquipments.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PartnerCell entry = cell.mUserData as PartnerCell;
            entry.UpdateInfo(listIds[index]);
            //entry.SetData(hornType == 0 ? Sys_Chat.Instance.mSingleServerHornDatas[index] : Sys_Chat.Instance.mFullServerHornDatas[index]);
        }

        private void OnSelectParnter(uint partnerId)
        {
            _listener?.OnSelectParnter(partnerId);
        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        public void UpdateInfo(uint infoId)
        {
            Sys_Partner.Instance.SelectPartnerId = infoId;
            listIds = Sys_Partner.Instance.GetPartnerReviewList(infoId);

            _infinityGrid.CellCount = listIds.Count;
            _infinityGrid.ForceRefreshActiveCell();

            int index = listIds.IndexOf(infoId);
            if (index >= 0)
                _infinityGrid.MoveToIndex(index); 
            //visualGridCount = listIds.Count;
            //gridGroup.SetAmount(visualGridCount);
        }

        public interface IListener
        {
            void OnSelectParnter(uint infoId);
        }
    }
}


