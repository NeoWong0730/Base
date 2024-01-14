using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;
using Lib.Core;

namespace Logic
{
    public class UI_Trade_Search_MagicCore_Sort 
    {
        public class CoreCell
        {
            private Transform transform;
            
            private Button btnBg;
            private Text txtName;
            private Image imgIcon;

            private uint m_CoreId;
            public void Init(Transform trans)
            {
                transform = trans;

                btnBg = transform.Find("Image_BG").GetComponent<Button>();
                btnBg.onClick.AddListener(OnClick);
                txtName = transform.Find("Text_name").GetComponent<Text>();
                imgIcon = transform.Find("PetItem/Image_Icon").GetComponent<Image>();
            }

            public void UpdateInfo(uint coreId)
            {
                m_CoreId = coreId;
                CSVItem.Data data = CSVItem.Instance.GetConfData(m_CoreId);
                if (data != null)
                {
                    ImageHelper.SetIcon(imgIcon, data.icon_id);
                    txtName.text = LanguageHelper.GetTextContent(data.name_id);
                }
            }

            private void OnClick()
            {
                Sys_Trade.Instance.eventEmitter.Trigger(Sys_Trade.EEvents.OnSelectCore, m_CoreId);
            }
        }

        private Transform transform;

        private InfinityGrid _infinityGrid;
        private List<uint> m_listCores;
        
        public void Init(Transform trans)
        {
            transform = trans;

            Button btnClose = transform.Find("Interrcept_Image").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { Hide(); });

            _infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
            
            UpdateInfo();
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            CoreCell core = new CoreCell();
            core.Init(cell.mRootTransform);
            cell.BindUserData(core);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            CoreCell core = cell.mUserData as CoreCell;
            core.UpdateInfo(m_listCores[index]);
        }

        private void UpdateInfo()
        {
            if (m_listCores == null)
                m_listCores = new List<uint>();
            m_listCores.Clear();

            int count = CSVPetEquip.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVPetEquip.Data data = CSVPetEquip.Instance.GetByIndex(i);
                if (data.sale_least == 1)
                {
                    m_listCores.Add(data.id);
                }
            }

            _infinityGrid.CellCount = m_listCores.Count;
            _infinityGrid.ForceRefreshActiveCell();
        }
    }
}


