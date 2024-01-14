using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Table;
using Packet;
using Logic.Core;

namespace Logic
{
    public class UI_Partner_Fetter_Detail_List
    {
        public class PartnerCell
        {
            private Transform transform;

            private Image imgIcon;
            private Image imgLock;
            private Text txtName;
            private Text txtRuneLv;
            private Text txtRuneNum;
            private Transform transLock;
            private Button btnDetail;
            private Button btnAssemble;

            private uint partnerId;

            public void Init(Transform trans)
            {
                transform = trans;

                imgIcon = transform.Find("Image_Icon").GetComponent<Image>();
                imgLock = transform.Find("Image_Lock").GetComponent<Image>();
                txtName = transform.Find("Image_Name/Text_Name").GetComponent<Text>();
                txtRuneLv = transform.Find("Text_RuneLevel").GetComponent<Text>();
                txtRuneNum = transform.Find("Text_RuneNum").GetComponent<Text>();
                transLock = transform.Find("lock");
                btnDetail = transform.Find("Btn_Details").GetComponent<Button>();
                btnDetail.onClick.AddListener(OnClickDetail);
                btnAssemble = transform.Find("Btn_Assemble").GetComponent<Button>();
                btnAssemble.onClick.AddListener(OnClickAssemble);
            }

            private void OnClickAssemble()
            {
                Sys_Partner.Instance.eventEmitter.Trigger(Sys_Partner.EEvents.OnFetterToRuneEquip, this.partnerId);
            }

            private void OnClickDetail()
            {
                UIManager.OpenUI(EUIID.UI_PartnerReview, false, this.partnerId);
            }

            public void UpdateInfo(uint partnerId)
            {
                this.partnerId = partnerId;
                CSVPartner.Data data = CSVPartner.Instance.GetConfData(this.partnerId);
                ImageHelper.SetIcon(imgIcon, data.headid);
                txtName.text = LanguageHelper.GetTextContent(data.name);

                bool isUnlock = Sys_Partner.Instance.IsUnLock(this.partnerId);
                imgLock.gameObject.SetActive(!isUnlock);
                transLock.gameObject.SetActive(!isUnlock);
                txtRuneLv.text = "";
                txtRuneNum.text = "";
                if (isUnlock)
                {
                    Partner partner = Sys_Partner.Instance.GetPartnerInfo(this.partnerId);
                    if (partner != null) 
                    {
                        txtRuneLv.text = LanguageHelper.GetTextContent(2006076, partner.TotalRuneLv.ToString());
                        txtRuneNum.text = LanguageHelper.GetTextContent(2006077, partner.TotalGoldRune.ToString());
                    }  
                }

                btnAssemble.gameObject.SetActive(isUnlock);
                btnDetail.gameObject.SetActive(!isUnlock);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;

        private List<uint> listIds = new List<uint>();

        public void Init(Transform trans)
        {
            transform = trans;

            _infinityGrid = transform.GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            PartnerCell entry = new PartnerCell();
            entry.Init(cell.mRootTransform);
            //entry.AddListener(OnSelectIndex);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PartnerCell entry = cell.mUserData as PartnerCell;
            entry.UpdateInfo(listIds[index]);
        }

        public void UpdateInfo(uint groupId)
        {
            listIds.Clear();
            listIds.AddRange(CSVBond.Instance.GetConfData(groupId).group_partnerID);

            _infinityGrid.CellCount = listIds.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(0);
        }
    }
}
